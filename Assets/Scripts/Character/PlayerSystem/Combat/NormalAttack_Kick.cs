using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 通常攻撃3段目の補正
/// </summary>
public class NormalAttack_Kick : AttackAdjustBase
{
    [SerializeField] private HitDetectionInfo _hitDetectionInfo;   

    public override async void StartAttack()
    {
        _hitDetector.DetectHit(_hitDetectionInfo); //当たり判定を発生させる
        
        if (_target != null)
        {
            //TODO:ターゲットがいるときの補正処理を書く
        }
        else
        {
            _animator.applyRootMotion = true;
        }
        
        await UniTask.Delay(50);
     
        AudioManager.Instance?.PlaySE(5);
    }

    public override void CorrectMovement(Vector3 forwardDirection){ }
}
