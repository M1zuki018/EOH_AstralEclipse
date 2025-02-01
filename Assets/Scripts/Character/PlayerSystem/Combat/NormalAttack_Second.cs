using DG.Tweening;
using UniRx;
using UnityEngine;

/// <summary>
/// 通常攻撃2段目の補正
/// </summary>
public class NormalAttack_Second : AttackAdjustBase
{
    [Header("初期設定")]
    [SerializeField] private HitDetectionInfo _hitDetectionInfo;
    [SerializeField] private float _slashFream = 0.14f; //アニメーションの進行度合い。正規化したもの
    [SerializeField] private float _initializeAnimationSpeed = 1.3f; //初期アニメーションスピード
    [SerializeField] private float _forwardDistance = 1.5f; //ロックオンしていない時に移動する距離
    [SerializeField] private AdjustDirection _adjustDirection;
    
    private bool _isAttacking = false; //突進中かどうか
    private float _distance; //敵との距離
    private float _totalDistanceToCover; //_distanceと_adjustDistanceの差
    

    /// <summary>
    /// 攻撃開始時に呼び出される処理
    /// </summary>
    public override void StartAttack()
    {
        _animator.SetFloat("AttackSpeed", _initializeAnimationSpeed);
        
        if (_target != null)
        {
            _adjustDirection.AdjustDirectionToTarget(); //体の向きを敵の方向に合わせる
            transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
            _distance = Vector3.Distance(transform.position, _target.position); //敵との距離を計算
        
            TriggerSlash(); //斬撃モーションを即座に再生する
        }
        else
        {
            TriggerSlash();
        }
    }

    public override void CorrectMovement(Vector3 forwardDirection)
    {
    }

    /// <summary>
    /// 斬撃モーションに移る
    /// </summary>
    private void TriggerSlash()
    {
        _hitDetector.DetectHit(_hitDetectionInfo.Collider, _hitDetectionInfo.Duration); //当たり判定を発生させる
        
        _isAttacking = false;

        if (_target != null) //ターゲットがいる場合、移動補正を行う
        {
            //移動
            float elapsedDistance = 0f; //移動した距離を記録する
        
        
            DOTween.To(
                    () => elapsedDistance,
                    value =>
                    {
                        float delta = value - elapsedDistance; //前回との差分を計算
                        elapsedDistance = value; //現在の値を更新
                 
                        _cc.Move(transform.forward * delta); //差分だけ移動させる
                    },
                    _forwardDistance,
                    0.5f)
                .SetEase(Ease.Linear);
        }
        else
        {
            _animator.applyRootMotion = true;
        }
        
        
        AudioManager.Instance?.PlaySE(3);
    }
}
