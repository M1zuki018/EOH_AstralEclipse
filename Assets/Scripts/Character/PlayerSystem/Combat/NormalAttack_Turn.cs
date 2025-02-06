using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 通常攻撃4段目の補正
/// </summary>
public class NormalAttack_Turn : AttackAdjustBase
{
    [SerializeField] private HitDetectionInfo[] _hitDetectionInfo;  
    [SerializeField] private EffectPositionInfo _effectPositionInfo;
    [SerializeField, Comment("これ以上近付かない距離")] private float _stopDistance = 1.8f;
    private Vector3 _lastValidPosition; //敵に近付きすぎたときの座標

    public override async void StartAttack()
    {
        _lastValidPosition = transform.position; //初期化
        _effectPool.GetEffect(_effectPositionInfo.Position, _effectPositionInfo.Rotation);
        
        CameraManager.Instance?.TurnEffect();
        _target = _adjustDirection.Target;
        _hitDetector.DetectHit(_hitDetectionInfo[0]); //当たり判定を発生させる
        CameraManager.Instance?.ApplyHitStop(0.007f);
        
        if (_target != null)
        {
            _adjustDirection.AdjustDirectionToTargetEarly();
            _animator.applyRootMotion = true;
        }
        else
        {
            _animator.applyRootMotion = true;
        }

        AudioManager.Instance?.PlaySE(8);
        
        await UniTask.Delay(300);
        
        CameraManager.Instance?.EndDashEffect();
        _hitDetector.DetectHitOnce(_hitDetectionInfo[1]); //回転後の判定
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
}
