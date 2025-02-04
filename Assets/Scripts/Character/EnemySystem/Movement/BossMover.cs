using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// ボスの移動を制御するクラス
/// </summary>
public class BossMover : MonoBehaviour
{
    [SerializeField] private BossAttackPattern _attackPattern;

    private CharacterController _cc;
    private Vector3 _initializePos;
    private int _patternCount;
    private int _pattern2Count; //パターン2で使用する

    private void Start()
    {
        _cc = GetComponent<CharacterController>();
        _initializePos = transform.position;
        transform.position = new Vector3(_initializePos.x, _initializePos.y + 4f, _initializePos.z); //空中に移動
    }

    /// <summary>
    /// ボス戦を始めるメソッド
    /// </summary>
    public void BattleStart()
    {
        Pattern1();
        //Pattern2();
    }

    /// <summary>
    /// 地上で休憩する
    /// </summary>
    private async void Break()
    {
        _cc.Move(new Vector3(0, _initializePos.y - transform.position.y, 0)); //初期位置と現在の位置の差分だけ移動する
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
        _cc.Move(new Vector3(0, 4, 0));
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
        _cc.Move(position - transform.position);
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

        _attackPattern.Animator.SetInteger("AttackType", 1);
        _attackPattern.Animator.SetTrigger("Attack");
        _attackPattern.ResetVerticalLasers();
        //垂直レーザーを生成
        float generatoPos = transform.position.x - 14f;
        for (int i = 0; i < 6; i++)
        {
            generatoPos += 4;
            _attackPattern.GenerateVerticalLaser(new Vector3(generatoPos, transform.position.y, transform.position.z));
        }
        
        await UniTask.Delay(3300);

        _attackPattern.Animator.SetTrigger("Attack");
        
        await UniTask.Delay(700);
        
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
    public void Pattern2()
    {
        _attackPattern.ShadowAttack();
        
        //TODO:ガード成功時：火花のようなエフェクト＋ボスが軽く後退。回避成功時：スローモーションを一瞬入れる
        //TODO: ヒット時：プレイヤーが「のけぞる」 or 「吹き飛ばされる」。
    }

    /// <summary>
    /// 攻撃パターン2の次に進む
    /// </summary>
    public void TransitionPattern2()
    {
        _pattern2Count++;
        if (_pattern2Count == 1)
        {
            //パターン2の一回目の攻撃なら、2回目の攻撃を行う
            Warp(new Vector3(100f, 4f, 230f));
            _attackPattern.ShadowAttack();
        }
        else
        {
            //パターン2の二回目の攻撃なら、地上に降りる
            _patternCount = 2;
            Break();
            _pattern2Count = 0; //リセットする
        }
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
        
        await UniTask.Delay(6000);

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
