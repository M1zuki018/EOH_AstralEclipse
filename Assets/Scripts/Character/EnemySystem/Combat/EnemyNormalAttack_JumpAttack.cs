using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 敵のジャンプ攻撃の補正クラス
/// </summary>
public class EnemyNormalAttack_JumpAttack : AttackAdjustBase
{
    [Header("初期設定")]
    [SerializeField] private HitDetectionInfo _hitDetectionInfo;
    private Transform _player;
    private CancellationTokenSource _cts;
    private bool _isAttacking;
    
    private void Start()
    {
        if (_player == null)
        {
            _player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }
    
    /// <summary>
    /// 攻撃処理を中断したい時に呼ぶMethod
    /// </summary>
    public override void CancelAttack()
    {
        if (_isAttacking && _cts != null)
        {
            _cts.Cancel();
            Debug.Log("攻撃がキャンセルされました");
        }
    }
    
    public override async void StartAttack()
    {
        //攻撃開始時の処理
        if(_player == null) return;
        _target = _player;
        _isAttacking = true;
        _cts = new CancellationTokenSource();
        
        if (_target != null)
        {
            _adjustDirection.AdjustDirectionToTargetEarly();  //ターゲットの方向を向く
            _animator.applyRootMotion = true;
        }
        else
        {
            Debug.LogWarning($"{gameObject}：{_target}が存在しません！");
        }
        
        try
        {
            // UniTask.DelayFrame にキャンセルトークンを渡して、不要な待機処理を省略できる
            await UniTask.DelayFrame(150, cancellationToken: _cts.Token);

            Debug.Log("判定");
            _hitDetector.DetectHit(_hitDetectionInfo); // 当たり判定を発生させる
        }
        catch (OperationCanceledException)
        {
            Debug.Log("攻撃処理がキャンセルされました");
        }
        finally
        {
            _isAttacking = false;
            _cts.Dispose();
        }
    }

    public override void CorrectMovement(Vector3 forwardDirection) { }
}
