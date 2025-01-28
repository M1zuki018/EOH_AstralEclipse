using System;
using System.Collections.Generic;
using PlayerSystem.Fight;
using UnityEngine;
using UniRx;

/// <summary>
/// 攻撃時の衝突判定を管理
/// </summary>
public class AttackHitDetector : MonoBehaviour
{
    //TODO: システム基盤を変更していく場合、Updateのタイミングを指定できるようにする

    [Header("設定")] 
    [SerializeField, Comment("衝突検出の所有者")] private CharacterBase _owner; 
    [SerializeField, Comment("衝突検出を行うレイヤー")] private LayerMask _hitLayer;
    [SerializeField, Comment("衝突するタグ")] private string[] _hitTags;
    
    [Header("検出フレームの範囲")] 
    [Range(0, 1)] [ReadOnlyOnRuntime] [SerializeField] private float _frame;
    [SerializeField] private int _attackType;

    [Header("検出位置と有効範囲の設定")]
    [SerializeField] private Data[] _hitPositions =
    {
        new() //初期設定
        {
            Range = new Vector2(0, 1), //範囲
            Collisions = new[]
            {
                new CollisionData
                {
                    Range = new Vector2(0, 1), //範囲
                    Offset = Vector3.zero, //オフセット
                    Rotation = Vector3.zero, //回転
                    Scale = Vector3.one //スケール
                }
            }
        }
    };
    
    private Transform _transform;
    private Animator _animator;
    private readonly List<GameObject> _hitObjects = new();

    /// <summary>攻撃の種類</summary>
    public int AttackType
    {
        get => _attackType; 
        set => _attackType = value;
    }
    
    private void OnEnable()
    {
        _transform = transform;
        _animator = GetComponentInParent<Animator>();
        _hitObjects.Clear(); //リストの中身をクリアする
    }

    /// <summary>
    /// 攻撃した時に呼び出す
    /// </summary>
    public List<IDamageable> PerformAttack() => DetectCollisions();

    /// <summary>
    /// 判定検出を実行する
    /// </summary>
    private List<IDamageable> DetectCollisions()
    {
        _frame = 0; //初期化
        
        Observable
            .EveryUpdate()
            .TakeWhile(_ => _frame < 1)
            .Subscribe(_ => _frame = SetFrameLength())
            .AddTo(this);
        
        //処理開始時にリストをクリア
        _hitObjects.Clear();
        List<IDamageable> damageables = new();
        
        var data = _hitPositions[AttackType]; 
        if (!data.IsInRange(_frame)) return null; //効果フレームの範囲外の場合は処理をスキップ

        var activeCollisions = data.GetActiveCollisions(_frame); //有効なHitボックスを取得
        foreach (var col in activeCollisions)
        { 
            // 衝突検出を実行
            var count = CalculateCollisions(col, out var hitResults);
            Debug.Log(this.gameObject.transform.parent.name + "衝突した数" + count);
                
            // 範囲内で衝突検出を実行
            for (var i = 0; i < count; i++) 
            { 
                var hit = hitResults[i];
                var hitObject = hit.gameObject;

                //if (!IsValidTarget(hitObject) || _hitObjects.Contains(hitObject)) continue;
                    
                //コライダーを登録。Playerの場合臨戦状態を管理するコライダーが取得されるので、親オブジェクトを参照するようにする
                _hitObjects.Add(hitObject);
                if (hit.TryGetComponent(out IDamageable damageable) || hit.transform.parent.TryGetComponent(out damageable))
                {
                    damageables.Add(damageable);
                }
            }
        }

        return damageables;
    }

    private int CalculateCollisions(CollisionData col, out Collider[] hitResults)
    {
        hitResults = new Collider[50];
        var position = _transform.position + _transform.TransformVector(col.Offset);
        var rotation = _transform.rotation * Quaternion.Euler(col.Rotation);
        return Physics.OverlapBoxNonAlloc(position, col.Scale * 0.5f, hitResults, rotation, _hitLayer);
    }

    /// <summary>
    /// ターゲットがHitTagに設定したタグを持っているか調べる
    /// </summary>
    private bool IsValidTarget(GameObject target)
    {
        foreach (var tag in _hitTags)
        {
            if (target.CompareTag(tag)) return true;
        }
        return _hitTags.Length == 0;
    }

    /// <summary>
    /// アニメーションクリップの時間を正規化する
    /// </summary>
    private float SetFrameLength()
    {
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.normalizedTime % 1; //再生時間を正規化
    }
    
    /// <summary>
    /// ギズモを表示する
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (_hitPositions == null || _hitPositions.Length <= _attackType) return;

        var data = _hitPositions[_attackType];
        if (!data.IsInRange(_frame)) return;

        var activeCollisions = data.GetActiveCollisions(_frame);
        foreach (var col in activeCollisions)
        {
            var position = transform.position + transform.TransformVector(col.Offset);
            var rotation = transform.rotation * Quaternion.Euler(col.Rotation);

            Gizmos.color = Color.yellow;
            Gizmos.matrix = Matrix4x4.TRS(position, rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, col.Scale);

            Gizmos.color = new Color(1, 1, 0, 0.2f);
            Gizmos.DrawCube(Vector3.zero, col.Scale);
            
        }
        
    }
}