using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 通常攻撃最終段の補正
/// </summary>
public class NormalAttack_End : AttackAdjustBase
{
    public override async void StartAttack()
    {
        _animator.SetFloat("AttackSpeed", 1f);
        
        AudioManager.Instance?.PlaySEDelay(3, 100); //右上から切り降ろす
        AudioManager.Instance?.PlaySEDelay(3, 330); //切りながらジャンプ
        
        await UniTask.Delay(330);
        
        _animator.SetFloat("AttackSpeed", 2f);
        
        await UniTask.Delay(370);
        
        AudioManager.Instance?.PlaySEDelay(4, 200); //着地
        _animator.SetFloat("AttackSpeed", 1f);
    }

    public override void CorrectMovement(Vector3 forwardDirection) { }

    public override void AdjustDirectionToTarget() { }
}
