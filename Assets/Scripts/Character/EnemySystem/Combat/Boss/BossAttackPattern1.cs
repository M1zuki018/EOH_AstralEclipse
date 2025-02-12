using Cysharp.Threading.Tasks;
using UnityEngine;

public class BossAttackPattern1 : MonoBehaviour, IBossAttack
{
    public string AttackName => "Pattern1";
    
    [Header("攻撃設定")]
    [SerializeField] private BossAttackPattern _attackPattern;
    [SerializeField] private BossMover _bossMover;
    
    [Header("タイミング設定")]
    [SerializeField, Comment("レーザー照射時間")] private float _laserDelay = 3f;
    [SerializeField, Comment("水平レーザー演出時間")] private float _laserPerformance = 3.5f;
    [SerializeField] private float _thornDelay = 5f;
    [SerializeField] private float _thornPlusDelay = 3f;
    [SerializeField] private float _aboveDelay = 4f;
    
    /// <summary>
    /// パターン1開始
    /// </summary>
    public async UniTask Fire()
    {
        await FireHorizontalLaser(); //水平レーザー
        await FireVerticalLaser(); //垂直レーザー
        await FireThornAttack(); //茨攻撃
        await FireAboveAttack(); //頭上からの攻撃
        FinishPattern(); //次の攻撃へ
    }
    
    /// <summary>
    /// パターン1強化版開始
    /// </summary>
    public async UniTask FirePlus()
    {
        Debug.Log("強化版");
        await FireHorizontalLaserPlus(); //水平レーザー(垂直レーザーへの待機時間なし)
        await FireVerticalLaser(10); //垂直レーザー（6本→10本）
        await FireThornAttackPlus(); //茨攻撃(2回→3回。猶予時間短縮)
        await FireAboveAttack(2); //頭上からの攻撃(猶予時間3秒→2秒)
        FinishPattern(); //次の攻撃へ
    }


    /// <summary>
    /// 水平レーザーを発射する
    /// </summary>
    private async UniTask FireHorizontalLaser()
    {
        _attackPattern.HorizontalLaser(transform, _laserDelay);
        await UniTask.Delay((int)(_laserDelay + _laserPerformance) * 1000); //レーザー照射時間＋演出時間
        
        //TODO:レーザーの爆風・床が燃えているなどのエフェクトを作ってもいいかもしれない
    }
    
    /// <summary>
    /// 水平レーザーを発射する
    /// </summary>
    private async UniTask FireHorizontalLaserPlus()
    {
        _attackPattern.HorizontalLaserPlus(transform, _laserDelay);
        await UniTask.Delay((int)_laserDelay * 1000); //レーザー照射時間
        
        //TODO:レーザーの爆風・床が燃えているなどのエフェクトを作ってもいいかもしれない
    }

    /// <summary>
    /// 垂直レーザーの生成と発射
    /// </summary>
    private async UniTask FireVerticalLaser(int piece = 6)
    {
        _attackPattern.ResetVerticalLasers();

        float raserDistance = 4;
        Vector3 offset = transform.right * raserDistance; // ボスの向きに基づいて位置をずらす
        //レーザーの本数の半分に間の間隔をかけたあと、レーザーの間にボスが入るようにする
        Vector3 startPos = transform.position - (transform.right * (piece / 2 * raserDistance - raserDistance / 2));
        
        for (int i = 0; i < piece; i++)
        {
            Vector3 generatePos = startPos + offset * i; // 各レーザーの生成位置を計算
            _attackPattern.GenerateVerticalLaser(generatePos);
        }

        await UniTask.Delay(3300);

        for (int i = 0; i < piece; i++)
        {
            int index = (i % 2 == 0) ? (i / 2) : (piece - 1 - (i / 2));
            _attackPattern.FireVerticalLaser(index);
            await UniTask.Delay(200);
        }
    }

    /// <summary>
    /// 茨攻撃を 2 回行う
    /// </summary>
    private async UniTask FireThornAttack()
    {
        for (int i = 0; i < 2; i++)
        {
            _attackPattern.GenerateThorns(1);
            await UniTask.Delay((int)(_thornDelay * 1000));
        }
    }
    
    /// <summary>
    /// 茨攻撃強化版。短い間隔で 3 回行う
    /// </summary>
    private async UniTask FireThornAttackPlus()
    {
        for (int i = 0; i < 3; i++)
        {
            _attackPattern.GenerateThorns(0.3f, 10);
            await UniTask.Delay((int)(_thornPlusDelay * 1000));
        }
    }

    /// <summary>
    /// 頭上からの攻撃
    /// </summary>
    private async UniTask FireAboveAttack(float delay = 3)
    {
        _attackPattern.AttackFromAbove(delay);
        await UniTask.Delay((int)(_aboveDelay * 1000));
    }

    /// <summary>
    /// パターン1の終了処理
    /// </summary>
    private async void FinishPattern()
    {
        Debug.Log("次のパターンへ移行");

        if (_bossMover != null)
        {
            _bossMover.Break();
        }

        await UniTask.Yield(); // すぐに処理を返す
    }
}
