using Cysharp.Threading.Tasks;
using UnityEngine;

public class BossAttackPattern1 : MonoBehaviour, IBossAttack
{
    public string AttackName => "Pattern1";
    
    [Header("攻撃設定")]
    [SerializeField] private BossAttackPattern _attackPattern;
    [SerializeField] private BossMover _bossMover;
    
    [Header("タイミング設定")]
    [SerializeField] private float _laserDelay = 3f;
    [SerializeField] private float _thornDelay = 5f;
    [SerializeField] private float _aboveDelay = 4f;
    
    /// <summary>
    /// パターン1開始
    /// </summary>
    public async UniTask Fire()
    {
        Debug.Log("パターン1開始");

        // 1. 水平レーザー（単独実行）
        await FireHorizontalLaser();

        // 2. 垂直レーザー（並列で準備しつつ、時間差で発射）
        await FireVerticalLaser();

        // 3. 茨攻撃 ×2（時間差で実行）
        await FireThornAttack();

        // 4. 頭上からの攻撃
        await FireAboveAttack();

        Debug.Log("パターン1終了");

        // 次の行動へ
        FinishPattern();
    }


    /// <summary>
    /// 水平レーザーを発射する
    /// </summary>
    private async UniTask FireHorizontalLaser()
    {
        _attackPattern.HorizontalLaser(transform, _laserDelay);
        await UniTask.Delay((int)(_laserDelay * 1000));
    }

    /// <summary>
    /// 垂直レーザーの生成と発射
    /// </summary>
    private async UniTask FireVerticalLaser()
    {
        _attackPattern.ResetVerticalLasers();

        float generatoPos = transform.position.x - 14f;
        for (int i = 0; i < 6; i++)
        {
            generatoPos += 4;
            _attackPattern.GenerateVerticalLaser(new Vector3(generatoPos, transform.position.y, transform.position.z));
        }

        await UniTask.Delay(3300);

        for (int i = 0; i < 6; i++)
        {
            int index = (i % 2 == 0) ? (i / 2) : (6 - 1 - (i / 2));
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
            _attackPattern.GenerateThorns();
            await UniTask.Delay((int)(_thornDelay * 1000));
        }
    }

    /// <summary>
    /// 頭上からの攻撃
    /// </summary>
    private async UniTask FireAboveAttack()
    {
        _attackPattern.AttackFromAbove();
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
