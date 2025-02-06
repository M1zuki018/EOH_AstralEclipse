using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// 通常攻撃2段目の補正
/// </summary>
public class NormalAttack_Second : AttackAdjustBase
{
    [Header("初期設定")]
    [SerializeField] private HitDetectionInfo _hitDetectionInfo;
    [SerializeField] private EffectPositionInfo _effectPositionInfo;
    [SerializeField] private float _initializeAnimationSpeed = 1.3f; //初期アニメーションスピード
    [SerializeField, Comment("攻撃時に動ける距離")] private float _forwardDistance = 1.5f;
    [SerializeField, Comment("これ以上近付かない距離")] private float _stopDistance = 1.8f;
    private Vector3 _lastValidPosition; //敵に近付きすぎたときの座標
    
    private float _distance; //敵との距離
    private float _totalDistanceToCover; //_distanceと_adjustDistanceの差
    
    /// <summary>
    /// 攻撃開始時に呼び出される処理
    /// </summary>
    public override void StartAttack()
    {
        _lastValidPosition = transform.position; //初期化
        
        _target = _adjustDirection.Target;
        _animator.SetFloat("AttackSpeed", _initializeAnimationSpeed);
        
        //ターゲットがいる場合の処理
        if (_target != null)
        {
            _adjustDirection.AdjustDirectionToTarget(); //体の向きを敵の方向に合わせる
            TriggerSlash(); //斬撃モーションを再生する
        }
        //ターゲットがいない場合の処理
        else
        {
            TriggerSlash();
        }
    }

    /// <summary>
    /// 斬撃モーションに移る
    /// </summary>
    private async void TriggerSlash()
    {
        //ターゲットがいる場合、移動補正を行う
        if (_target != null) 
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

        await UniTask.Delay(130);
        
        AudioManager.Instance?.PlaySE(3);
        
        await UniTask.Delay(100);

        _effectPool.GetEffect(_effectPositionInfo.Position, _effectPositionInfo.Rotation);
        _hitDetector.DetectHit(_hitDetectionInfo); //当たり判定を発生させる
    }

    public override void CorrectMovement(Vector3 forwardDirection)
    {
        // 敵との距離を測る
        float distanceToEnemy = Vector3.Distance(transform.position, _adjustDirection.Target.position);

        if (distanceToEnemy > _stopDistance)
        {
            // 敵に向かって移動（ルートモーションと補正を併用）
            Vector3 direction = (_adjustDirection.Target.position - transform.position).normalized;
            direction.y = 0; // Y方向の影響を無視
            _cc.Move(direction * Time.deltaTime);
            _lastValidPosition = transform.position; //現在の位置を更新
        }
        else
        {
            // 近づきすぎた場合、前フレームの適正な座標との差分を求めて補正
            Vector3 correction = _lastValidPosition - transform.position;
            correction.y = 0;
            _cc.Move(correction); // 差分を使って元の位置へ引き戻す
        }
    }

    public override void CancelAttack()
    {
        
    }
}
