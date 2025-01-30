using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// ボスの移動を制御するクラス
/// </summary>
public class BossMover : MonoBehaviour
{
    [SerializeField] private BossAttackPattern _attackPattern;

    /// <summary>
    /// 地上で休憩する
    /// </summary>
    private async void Break()
    {
        Debug.Log("休み");
        await UniTask.Delay(5000);
    }

    /// <summary>
    /// ゆっくり飛び上がる
    /// </summary>
    private async void Emerge()
    {
        Debug.Log("飛ぶ");
        await UniTask.Delay(5000);
    }

    /// <summary>
    /// 特定の場所へワープする
    /// </summary>
    private void Warp(Transform position)
    {
        
    }
    
    
    /// <summary>
    /// 攻撃パターン1
    /// </summary>
    [ContextMenu("Pattern1")]
    public async void Pattern1()
    {
        _attackPattern.HorizontalLaser(transform, 3f); //水平レーザー
        
        await UniTask.Delay(5000);

        float generatoPos = transform.position.x - 14f;
        for (int i = 0; i < 6; i++)
        {
            generatoPos += 4;
            _attackPattern.GenerateVerticalLaser(new Vector3(generatoPos, transform.position.y, transform.position.z)); //垂直レーザー6本
        }
        
        await UniTask.Delay(5000);
        
        for (int i = 0; i < 6; i++)
        {
            _attackPattern.FireVerticalLaser(); //垂直レーザーを放つ
            //ボス自身も自然に移動させたい
        }
        
        await UniTask.Delay(5000);
        
        _attackPattern.GenerateThorns(transform); //茨を生やす攻撃
        
        await UniTask.Delay(5000);
        
        _attackPattern.FireThorns();
        
        await UniTask.Delay(5000);
        
        _attackPattern.GenerateThorns(transform); //茨を生やす攻撃2回目
        
        await UniTask.Delay(5000);
        
        _attackPattern.FireThorns();
        
        await UniTask.Delay(5000);

        _attackPattern.AttackFromAbove(transform); //頭上からの攻撃
    }

    /// <summary>
    /// 攻撃パターン2　影
    /// </summary>
    public async void Pattern2()
    {
        _attackPattern.ShadowMove();
        _attackPattern.ShadowAttack();

        await UniTask.Delay(5000);
        
        _attackPattern.ShadowMove();
        _attackPattern.ShadowAttack();

    }

    /// <summary>
    /// 攻撃パターン3　時間
    /// </summary>
    public async void Pattern3()
    {
        await UniTask.Delay(500); //発動予兆0.5秒
        
        _attackPattern.TimeControl();
        _attackPattern.FireTimeAttack();
    }

    public async void LastAttack()
    {
        await UniTask.Delay(5000);
        
        _attackPattern.FinalTimeControl();
        _attackPattern.TimeStop();
        _attackPattern.FinalAttack();
    }

    public void After()
    {
        _attackPattern.After();
    }
}
