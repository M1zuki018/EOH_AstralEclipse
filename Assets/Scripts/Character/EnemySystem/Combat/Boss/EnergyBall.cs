using System;
using DG.Tweening;
using PlayerSystem.Fight;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// 追尾するエネルギー弾の管理スクリプト
/// </summary>
public class EnergyBall : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;  // 移動速度
    [SerializeField] private float _moveDuration = 2.5f; //追尾にかける時間
    [SerializeField] private float _straightSpeed = 70f; //直線移動速度
    [SerializeField] private float _overshootAmount = 1.2f; //目的地をどれくらいオーバーシュートするか
    [SerializeField] private float _damageMag = 0.5f;
    
    private Transform _player;  // 追尾対象（プレイヤー）
    private ICombat _combat;
    private bool _isChasing = true;  // 追尾中かどうか
    private bool _isFire = false; //発射されたかどうか
    private Vector3 _straightDirection; // 直線移動用の方向ベクトル
    
    /// <summary>
    /// 初期化
    /// </summary>
    public void SetPlayer(Transform player, ICombat combat)
    {
        _player = player;   
        _combat = combat;
    }

    /// <summary>
    /// 外から呼び出す。エネルギー弾を発射する
    /// </summary>
    public void Fire()
    {
        _isFire = true;
    }

    private void Update()
    {
        if (_player == null || !_isFire) return; //追尾対象となるプレイヤーがいなければ以降の処理は行わない
        
        //目標地点をオーバーシュートする地点を設定
        Vector3 targetPosition = _player.position;
        Vector3 overshootPosition = targetPosition + (targetPosition - transform.position).normalized * _overshootAmount;

        // DOTweenでカーブ移動
        transform
            .DOMove(overshootPosition, _moveDuration)
            .SetEase(Ease.OutBack) // オーバーシュート後に戻る
            .OnComplete(() =>
            {
                _isChasing = false;
                _straightDirection = (targetPosition - transform.position).normalized;

                // 一定時間経過後、直線移動開始
                Observable.EveryUpdate()
                    .TakeUntilDestroy(this)
                    .Subscribe(_ =>
                    {
                        transform.position += _straightDirection * _straightSpeed * Time.deltaTime;
                    })
                    .AddTo(this);
            });

        // プレイヤーのZ座標を下回ったら直線移動に移行
        Observable.EveryUpdate()
            .TakeUntilDestroy(this)
            .Where(_ => _isChasing && transform.position.z < targetPosition.z)
            .Subscribe(_ =>
            {
                _isChasing = false;
                transform.DOKill(); // 追尾アニメーションを止める
                _straightDirection = (targetPosition - transform.position).normalized;
            })
            .AddTo(this);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Ground")) // プレイヤーまたは地面に衝突
        {
            if (other.CompareTag("Player"))
            {
                other.TryGetComponent(out IDamageable damageable);
                _combat.DamageHandler.ApplyDamage(
                    target: damageable, //攻撃対象
                    baseDamage: Mathf.CeilToInt(_combat.BaseAttackPower * _damageMag), //攻撃力 
                    defense: 0, //相手の防御力
                    attacker: gameObject); //攻撃を加えるキャラクターのゲームオブジェクト
            }
            
            Destroy(gameObject); // エネルギー弾を削除
        }
    }

    private void OnDestroy()
    {
        transform.DOKill(); //アニメーションを止める
    }
}