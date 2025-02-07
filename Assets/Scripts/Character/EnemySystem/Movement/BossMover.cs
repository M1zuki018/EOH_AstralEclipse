using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

/// <summary>
/// ボスの移動を制御するクラス
/// </summary>
public class BossMover : MonoBehaviour
{
    [SerializeField] private BossAttackPattern _attackPattern;

    private Health _health;
    private CharacterController _cc;
    private Vector3 _initializePos;
    private int _currentPattern = 0; //現在の攻撃パターン
    private int _pattern2Count; //パターン2で使用する
    private bool _isDPSCheak; //DPSチェック中かどうか

    private readonly List<Func<UniTask>> _attackPatterns = new();

    private void Start()
    {
        //コンポーネントの取得と初期化
        _health = GetComponent<Health>();
        _cc = GetComponent<CharacterController>();
        _initializePos = transform.position;
        transform.position = new Vector3(_initializePos.x, _initializePos.y + 4f, _initializePos.z); //空中に移動
        
        //HPが50%以下になったら攻撃パターンを変更する
        Observable
            .EveryUpdate()
            .Where(_ => _health.CurrentHP <= _health.MaxHP * 0.5f)
            .Take(1)
            .Subscribe(_ => ChangeAttackPattern())
            .AddTo(this);
        
        //HPが10%以下になったらDPSチェックを始める
        Observable
            .EveryUpdate()
            .Where(_ => _health != null && _health.CurrentHP <= _health.MaxHP * 0.1f) //HP10%以下になったら
            .Take(1) //一度だけに制限
            .Subscribe(_ => _attackPattern.FinalTimeControl()) //特殊攻撃パターンを実行
            .AddTo(this);
        
        //攻撃パターンを登録
        _attackPatterns.Add(Pattern1);
        _attackPatterns.Add(Pattern2);
        _attackPatterns.Add(Pattern3);
        
        BattleStart().Forget();
    }

    /// <summary>
    /// ボス戦開始
    /// </summary>
    public async UniTask BattleStart()
    {
        await _attackPatterns[0]();
        /*
        while (!_isDPSCheak)
        {
            await _attackPatterns[_currentPattern]();
            _currentPattern = (_currentPattern + 1) % _attackPatterns.Count;
            await UniTask.Delay(2000);
        }
        */
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
        if(_currentPattern == 1) Emerge();
        else if(_currentPattern == 2) Pattern3();
        else if (_currentPattern == 3)
        {
            _currentPattern = 0; //初期化
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
        if(_currentPattern == 1) Pattern2(); //パターン2に繋げる
        else if (_currentPattern == 0) Pattern1();
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
    public async UniTask Pattern1()
    {
        _attackPattern.StartAttackPattern1();
        /*
        _currentPattern = 1;
        Break();
        */
    }

    /// <summary>
    /// 攻撃パターン2 影移動と近接戦闘
    /// </summary>
    [ContextMenu("Pattern2")]
    public async UniTask Pattern2()
    {

        await _attackPattern.StartShadowAttack();

        //TODO:ガード成功時：火花のようなエフェクト＋ボスが軽く後退。回避成功時：スローモーションを一瞬入れる
        //TODO: ヒット時：プレイヤーが「のけぞる」 or 「吹き飛ばされる」。
    }

    /// <summary>
    /// 攻撃パターン3　時間操作
    /// </summary>
    [ContextMenu("Pattern3")]
    public async UniTask Pattern3()
    {
        Warp(new Vector3(100f, 15f, 250f));
        _attackPattern.DefaultTransform = transform.position;
        
        await UniTask.Delay(500); //発動予兆0.5秒
        
        _attackPattern.TimeControl(); //攻撃
        
        await UniTask.Delay(5000);

        _currentPattern = 3;
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
    
    /// <summary>
    /// 攻撃パターンを変更する
    /// </summary>
    private void ChangeAttackPattern()
    {
        Debug.Log("攻撃パターン変更");
        _attackPatterns.Clear();
        _attackPatterns.Add(Pattern2);
        _attackPatterns.Add(Pattern3);
    }
}
