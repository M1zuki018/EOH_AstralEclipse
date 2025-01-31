using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// ボスの移動を制御するクラス
/// </summary>
public class BossMover : MonoBehaviour
{
    [SerializeField] private BossAttackPattern _attackPattern;

    private Vector3 _initializePos;
    private int _patternCount;

    private void Start()
    {
        _initializePos = transform.position;
        transform.position = new Vector3(_initializePos.x, _initializePos.y + 4f, _initializePos.z); //空中に移動
    }

    /// <summary>
    /// ボス戦を始めるメソッド
    /// </summary>
    public void BattleStart()
    {
        Pattern1();
    }
    
    /// <summary>
    /// 地上で休憩する
    /// </summary>
    private async void Break()
    {
        transform.DOMoveY(_initializePos.y, 1f);
        Debug.Log("休み");
        await UniTask.Delay(6000);
        
        //次の攻撃に向けて移動する
        if(_patternCount == 1) Emerge();
        else if(_patternCount == 2) Pattern3();
        else if (_patternCount == 3)
        {
            _patternCount = 0; //初期化
            Emerge();
        }
    }

    /// <summary>
    /// ゆっくり飛び上がる
    /// </summary>
    private async void Emerge()
    {
        transform.DOMoveY( 4f, 1f);
        await UniTask.Delay(1000);
        
        //次の攻撃を行う
        if(_patternCount == 1) Pattern2(); //パターン2に繋げる
        else if (_patternCount == 0) Pattern1();
    }

    /// <summary>
    /// 特定の場所へワープする
    /// </summary>
    private void Warp(Vector3 position)
    {
        transform.position = position;
    }
    
    
    /// <summary>
    /// 攻撃パターン1 レーザーメインの回避パート
    /// </summary>
    [ContextMenu("Pattern1")]
    public async void Pattern1()
    {
        //水平レーザーを照射
        _attackPattern.HorizontalLaser(transform, 3f);
        
        await UniTask.Delay(5000);

        //垂直レーザーを生成
        float generatoPos = transform.position.x - 14f;
        for (int i = 0; i < 6; i++)
        {
            generatoPos += 4;
            _attackPattern.GenerateVerticalLaser(new Vector3(generatoPos, transform.position.y, transform.position.z));
        }
        
        await UniTask.Delay(4000);

        //垂直レーザーを放つ
        for (int i = 0; i < 6; i++)
        {
            //順番は0、5、1、4、2、3（外側から内側へ）
            _attackPattern.FireVerticalLaser((i % 2 == 0) ? (i / 2) : (6 - 1 - (i / 2)));
            await UniTask.Delay(200);
        }
        //TODO:ボス自身も自然に移動させたい
        
        await UniTask.Delay(2500);
        
        //茨攻撃（生成→予兆→攻撃→消滅までのセット）
        _attackPattern.GenerateThorns();
        
        await UniTask.Delay(5000);
        
        //茨攻撃2回目
        _attackPattern.GenerateThorns();
        
        await UniTask.Delay(5000);

        //頭上からの攻撃
        _attackPattern.AttackFromAbove();
        
        await UniTask.Delay(4000);

        _patternCount = 1;
        Break();
    }

    /// <summary>
    /// 攻撃パターン2 影移動と近接戦闘
    /// </summary>
    [ContextMenu("Pattern2")]
    public async void Pattern2()
    {
        _attackPattern.ShadowAttack();

        await UniTask.Delay(8000);
        
        Warp(new Vector3(100f, 4f, 230f));
        _attackPattern.ShadowAttack();
        
        //TODO:ガード成功時：火花のようなエフェクト＋ボスが軽く後退。回避成功時：スローモーションを一瞬入れる
        //TODO: ヒット時：プレイヤーが「のけぞる」 or 「吹き飛ばされる」。

        await UniTask.Delay(8000);
        _patternCount = 2;
        Break();
    }

    /// <summary>
    /// 攻撃パターン3　時間操作
    /// </summary>
    [ContextMenu("Pattern3")]
    public async void Pattern3()
    {
        Warp(new Vector3(100f, 15f, 250f));
        
        await UniTask.Delay(500); //発動予兆0.5秒
        
        _attackPattern.TimeControl(); //攻撃
        
        await UniTask.Delay(10000);

        _patternCount = 3;
        Break();
    }

    [ContextMenu("LastAttack")]
    public async void LastAttack()
    {
        _attackPattern.FinalTimeControl();
    }

    public void After()
    {
        _attackPattern.After();
    }
}
