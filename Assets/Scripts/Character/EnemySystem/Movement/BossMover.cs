using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using PlayerSystem.Fight;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// ボスの移動を制御するクラス
/// </summary>
public class BossMover : MonoBehaviour
{
    [SerializeField] private BossAttackPattern _attackPattern;
    [SerializeField] private int _dpsCheckTime = 20;

    private BossHealth _health;
    private CharacterController _cc;
    private Vector3 _initializePos;
    private int _currentPattern = 0; //現在の攻撃パターン
    private int _pattern2Count; //パターン2で使用する
    private bool _isDamageImmunity; //ダメージ無効
    private bool _isDPSCheak; //DPSチェック中かどうか
    private IDisposable _fallingSubscription; //重力を受けて地面に降りる処理を購読
    private IDisposable _movingSubscription; //移動処理を購読
    
    public bool IsDamageImmunity => _isDamageImmunity;
    public bool IsDPSCheak => _isDPSCheak;

    private readonly List<Func<UniTask>> _attackPatterns = new();
    
    private void Start()
    {
        //コンポーネントの取得と初期化
        _health = GetComponent<BossHealth>();
        _cc = GetComponent<CharacterController>();
        _initializePos = transform.position;
        transform.position = new Vector3(_initializePos.x, _initializePos.y + 4f, _initializePos.z); //初期位置に移動

        _health.OnCheckComplete += SuccessDpsCheck; //DPSチェック成功時のイベントを登録
        
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
            .Subscribe(_ => _isDamageImmunity = true) //ダメージ無効状態に変更する
            .AddTo(this);
        
        //攻撃パターンを登録
        _attackPatterns.Add(Pattern1);
        _attackPatterns.Add(Pattern2);
        _attackPatterns.Add(Pattern3);
    }

    private void OnDestroy()
    {
        _health.OnCheckComplete -= SuccessDpsCheck; //解除
    }

    /// <summary>
    /// ボス戦開始
    /// </summary>
    public async UniTask BattleStart()
    {
        await _attackPatterns[0]();
    }

    /// <summary>
    /// 地上で休憩する
    /// </summary>
    public async void Break()
    {
        if (IsDamageImmunity)
        {
            //DPSチェックを始める
            DPSCheak();
            return;
        }

        Falling(); //地面に降りる
        
        await UniTask.Delay(10000);
        
        if (IsDamageImmunity)
        {
            //DPSチェックを始める
            DPSCheak();
            return;
        }
        
        //次の攻撃に向けて移動する
        if (_currentPattern == 1) await _attackPatterns[1]();
        else if (_currentPattern == 2) await _attackPatterns[2]();
        else if (_currentPattern == 3)
        {
            _currentPattern = 0; //初期化
            await _attackPatterns[0]();
        }
    }
    
    /// <summary>
    /// 特定の場所へワープする
    /// </summary>
    private void Warp(Vector3 position)
    {
        _cc.Move(position - transform.position);
    }

    /// <summary>
    /// 重力にしたがって地面に降りる
    /// </summary>
    public void Falling()
    {
        _fallingSubscription = Observable.EveryUpdate()
            .TakeWhile(_ => !_cc.isGrounded) // 地面に着くまで継続
            .Subscribe(_ =>
            {
                _cc.SimpleMove(Vector3.down * 9.81f);
            }, () =>
            {
                Debug.Log("着地");
                _fallingSubscription?.Dispose(); //購読解除
            })
            .AddTo(this);
    }
    
    /// <summary>
    /// 非同期で指定位置に移動する
    /// </summary>
    private async UniTask MoveToAsync(Vector3 targetPosition, float speed)
    {
        // 目的地まで移動
        while (Vector3.Distance(_cc.transform.position, targetPosition) > 0.1f)
        {
            Vector3 direction = (targetPosition - _cc.transform.position).normalized;
            Vector3 moveVector = direction * speed * Time.deltaTime;

            _cc.Move(moveVector);

            // 一フレーム待機
            await UniTask.Yield();
        }
    }
    
    
    /// <summary>
    /// 攻撃パターン1 レーザーメインの回避パート
    /// </summary>
    [ContextMenu("Pattern1")]
    public async UniTask Pattern1()
    {
        // 移動処理の呼び出し
        Vector3 targetPosition =　new Vector3(_initializePos.x, _initializePos.y + 4f, _initializePos.z);  // 移動先は初期位置
        float moveSpeed = 10f;  // 移動速度
        
        //ボスの体の向きをプレイヤーに合わせる
        _attackPattern.Animator.applyRootMotion = false;
        Vector3 direction = _attackPattern.Target.transform.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        await UniTask.WhenAll(
            transform.DORotateQuaternion(targetRotation, 0.5f).ToUniTask(),
            MoveToAsync(targetPosition, moveSpeed)
        );
        
        await _attackPattern.StartAttackPattern1();
        _currentPattern = 1;
    }
    
    /// <summary>
    /// 強化版攻撃パターン1 レーザーメインの回避パート
    /// </summary>
    [ContextMenu("Pattern1Plus")]
    public async UniTask Pattern1Plus()
    {
        // 移動処理の呼び出し
        Vector3 targetPosition =　new Vector3(_initializePos.x, _initializePos.y + 4f, _initializePos.z);  // 移動先は初期位置
        float moveSpeed = 15f;  // 移動速度
        
        //ボスの体の向きをプレイヤーに合わせる
        _attackPattern.Animator.applyRootMotion = false;
        Vector3 direction = _attackPattern.Target.transform.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        await UniTask.WhenAll(
            transform.DORotateQuaternion(targetRotation, 0.5f).ToUniTask(),
            MoveToAsync(targetPosition, moveSpeed)
        );
        
        await _attackPattern.StartAttackPattern1Plus();
        _currentPattern = 1;
    }

    /// <summary>
    /// 攻撃パターン2 影移動と近接戦闘
    /// </summary>
    [ContextMenu("Pattern2")]
    public async UniTask Pattern2()
    {
        await _attackPattern.StartShadowAttack();
        _currentPattern = 2;
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

    /// <summary>
    /// DPSチェックの操作
    /// </summary>
    [ContextMenu("DPSCheak")]
    public void DPSCheak()
    {
        _isDamageImmunity = false; //ダメージ無効状態解除
        _isDPSCheak = true;
        
        //UIの操作
        UIManager.Instance.InitializeBossDpsSlider(_health.CurrentBreakAmount, _health.CurrentBreakAmount);
        UIManager.Instance.ShowBossDpsSlider();
        UIManager.Instance.HideBossUI();
        UIManager.Instance.HidePlayerHP();

        int elapsedTime = _dpsCheckTime;
        //DPSチェックのタイマー
        Observable
            .Interval(TimeSpan.FromSeconds(1)) //1秒ごとにチェックを行う
            .TakeWhile(_ => elapsedTime >= 0) //タイマーが0秒になるまで行う
            .Subscribe(_ =>
            {
                elapsedTime--;

                if (elapsedTime <= 0)
                {
                    LastAttack();
                }
                else if (elapsedTime <= 5)
                {
                    //5秒前からカウントダウンを開始する
                    Debug.Log($"カウントダウン開始 {elapsedTime}");
                }
            })
            .AddTo(this);
    }

    /// <summary>
    /// DPSチェック成功時の処理
    /// </summary>
    private async void SuccessDpsCheck()
    {
        //UIの操作
        UIManager.Instance.HideBossDpsSlider();
        UIManager.Instance.ShowBossUI();
        UIManager.Instance.ShowPlayerHP();
        
        _isDPSCheak = false;
        await _attackPattern.SuccessDpsCheck();
        Break();
    }
    
    /// <summary>
    /// DPSチェック失敗時の処理
    /// </summary>
    [ContextMenu("LastAttack")]
    private async void LastAttack()
    {
        //UIの操作
        UIManager.Instance.HideBossDpsSlider();
        UIManager.Instance.HidePlayerBattleUI();
        UIManager.Instance.HideRightUI();
        
        await MoveToAsync(new Vector3(_initializePos.x, _initializePos.y + 4f, _initializePos.z + 10f), 15f);
        
        _attackPattern.FinalTimeControl();
    }
    
    /// <summary>
    /// 攻撃パターンを変更する
    /// </summary>
    private void ChangeAttackPattern()
    {
        Debug.Log("攻撃パターン変更");
        _attackPatterns.Clear();
        _attackPatterns.Add(Pattern1Plus);
        _attackPatterns.Add(Pattern2);
        _attackPatterns.Add(Pattern3);
    }
}
