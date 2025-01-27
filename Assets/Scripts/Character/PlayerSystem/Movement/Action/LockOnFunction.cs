using System;
using System.Collections.Generic;
using Cinemachine;
using Cysharp.Threading.Tasks;
using PlayerSystem.ActionFunction;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// ロックオン機能
/// </summary>
public class LockOnFunction : MonoBehaviour, ILockOnable
{
    [SerializeField, HighlightIfNull] private CinemachineTargetGroup _targetGroup; //シネマシーンのカメラ
    [SerializeField, Comment("判定を行うカメラ")] private Camera _camera; 
    [SerializeField, Comment("カメラ中心からのロックオン範囲")] private float _lockOnRadius = 0.5f;
    [SerializeField, HighlightIfNull] private ReadyForBattleChecker _battleChecker; //敵の検出範囲管理を行うクラス
    
    private readonly ReactiveProperty<Transform> _lockedOnEnemy = new ReactiveProperty<Transform>(); //現在ロックオンしている敵を保持する
    private Transform _lockedEnemy; //ロックオン中の敵
    private Transform _defaultFocusTarget; //VirtualCameraで初期状態でLook Atに設定されているトランスフォームを保持する
    
    private IDisposable _updateSubscription;
    
    private void Start()
    {
        if (_battleChecker == null)
        {
            Debug.Assert(_battleChecker != null);
            return;
        }
        
        _updateSubscription = Observable
            .Interval(TimeSpan.FromSeconds(0.5f))
            .Subscribe(_ =>
            {
                if (_battleChecker.EnemiesInRange.Count > 0)
                {
                    if (_lockedOnEnemy == null || _lockedOnEnemy.Value.GetComponent<EnemyBrain>()) //ロックオンしている敵がいない場合、検索を行う
                    {
                        UpdateEnemiesInRange(); //範囲内の敵を更新し、必要であればロックオンを開始する
                    }
                }
                else
                {
                    ClearLockOn(); //敵がいなくなったらロックオン解除
                }
            })
            .AddTo(this);
        
        //ロックオン状態の監視
        _lockedOnEnemy.Subscribe(OnLockOnTargetChanged).AddTo(this);
    }
    
    private void OnDestroy()
    {
        //解除
        _updateSubscription?.Dispose();
        _lockedOnEnemy.Dispose();
    }
    
    /// <summary>
    /// InputActionで入力があった時に呼ばれるメソッド
    /// </summary>
    public void LockOn()
    {
        if (_battleChecker.EnemiesInRange.Count == 0) //リストに敵がいなかった場合
        {
            Debug.Log("ロックオン可能な敵がいません");
            _lockedOnEnemy.Value = null;
            CameraManager.Instance.UseCamera(0);
            return;
        }

        Transform nextTarget = SelectNextLockOnTarget(); //別の敵をロックオンする
        _lockedOnEnemy.Value = nextTarget;
        _targetGroup.RemoveMember(_targetGroup.m_Targets[1].target); //必ず消してからセットする
        _targetGroup.AddMember(nextTarget.transform.GetChild(3), 1, 0.16f);
        CameraManager.Instance.UseCamera(1);
    }

    #region 視界内の敵のリストを作成するメソッドまとめ

    /// <summary>
    /// 視界内の敵リストを更新する
    /// </summary>
    private void UpdateEnemiesInRange()
    {
        List<Transform> inRangeEnemies = new List<Transform>(); //視界にいる敵のリスト

        //探知範囲内にいる敵が死亡状態ではないか確認し、リストに追加する
        foreach (EnemyBrain enemy in _battleChecker.EnemiesInRange)
        {
            if (!IsEnemyValid(enemy)) continue;
            inRangeEnemies.Add(enemy.transform);
        }
        
        // ロックオン可能な敵がいなければロックオンのアイコンを隠す
        if (inRangeEnemies.Count == 0)
        {
            _lockedOnEnemy.Value = null;
            return;
        }

        _lockedOnEnemy.Value = SelectLockOnTarget(inRangeEnemies); //新しいロックオン対象を選択する
    }
    
    /// <summary>
    /// ロックオン対象を選択
    /// </summary>
    private Transform SelectLockOnTarget(List<Transform> enemies)
    {
        Transform bestTarget = null;
        float closestDistance = float.MaxValue;

        //ロックオン可能な敵のリストの中からロックオン対象を探す
        foreach (Transform enemy in enemies)
        {
            Vector3 screenPoint = _camera.WorldToScreenPoint(enemy.position); //判定中のエネミーが描画されている座標
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f); //画面の中心
            float distanceFromCenter = Vector2.Distance(screenPoint, screenCenter); //エネミーの画面の中心からの距離を計算

            //カメラの中心から指定した範囲内に敵がいた場合、その敵をロックオンする
            if (distanceFromCenter <= _lockOnRadius * Screen.height) return enemy;
            
            //次に視界方向にいる敵を優先
            if (Vector3.Dot(_camera.transform.forward, (enemy.position - _camera.transform.position).normalized) > 0.8f)
            {
                return enemy;
            }
            
            //カメラの中心から指定した範囲の中に敵がいなかった場合、プレイヤーから最も近い敵を選択
            float worldDistance = Vector3.Distance(transform.position, enemy.position);
            if (worldDistance < closestDistance)
            {
                closestDistance = worldDistance;
                bestTarget = enemy;
            }
        }
        
        return bestTarget;
    }
    #endregion
    
    /// <summary>
    /// ロックオンしているターゲットが変更されたときに呼び出されるメソッド
    /// </summary>
    private void OnLockOnTargetChanged(Transform newTarget)
    {
        if (newTarget != null) //新しいターゲットがいた場合
        {
            UIManager.Instance.SetLockOnUI(newTarget);
            AudioManager.Instance.PlaySE(2);
        }
        else //次のターゲットがいない場合
        {
            UIManager.Instance.HideLockOnUI();
            CameraManager.Instance.UseCamera(0);
            Debug.Log("ロックオン可能な敵がいません");
        }
    }
    
    /// <summary>
    /// 既にロックオンしている状態で再びロックオンの入力を受け取った時、次のロックオン対象を選択する
    /// </summary>
    private Transform SelectNextLockOnTarget()
    {
        Transform currentTarget = _lockedOnEnemy.Value;
        Transform bestTarget = null;
        float closestDistance = float.MaxValue;

        foreach (var enemy in _battleChecker.EnemiesInRange)
        {
            if (!IsEnemyValid(enemy)) continue; //敵が死亡していないか判定する
            
            if (enemy.transform == currentTarget) continue;　//現在のターゲットは省く

            //プレイヤーに最も近い敵を探す
            float worldDistance = Vector3.Distance(transform.position, enemy.transform.position);
            if (worldDistance < closestDistance)
            {
                closestDistance = worldDistance;
                bestTarget = enemy.transform;
            }
        }

        return bestTarget ?? currentTarget; // 次のターゲットが見つからなければ、現在のターゲットを維持
    }
    
    /// <summary>
    /// 有効な敵かの判定
    /// EnemyBrainが取得できているか・死亡していないかチェックする
    /// </summary>
    private bool IsEnemyValid(EnemyBrain enemy)
    {
        return enemy != null && !enemy.GetComponent<Health>().IsDead;
    }

    /// <summary>
    /// ロックオン解除用のメソッド
    /// </summary>
    public void ClearLockOn()
    {
        _lockedOnEnemy.Value = null;
        CameraManager.Instance.UseCamera(0);
    }
}
