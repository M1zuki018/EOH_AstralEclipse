using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using PlayerSystem.Fight;
using UnityEngine;

/// <summary>
/// 茨のプレハブにアタッチするクラス
/// </summary>
public class ThornContorl : MonoBehaviour, IBossAttack
{
    public string AttackName => "Thorn";
    
    [SerializeField] private GameObject _thorn; //棘のオブジェクト
    [SerializeField] private int _damageMag = 4; //ダメージ倍率
    
    [Header("DOTween設定")]
    [SerializeField, Comment("棘が突き出す時間")] private float _duration = 0.5f;
    [SerializeField, Comment("棘の本数")] private int _spikeCount = 28;
    [SerializeField, Comment("棘が突き出す高さ")] private float _spikeHeight = 0.8f;
    [SerializeField, Comment("次の棘が突き出すまでの遅延")] private float _delayBetweenSpikes = 0.02f;
    private List<GameObject> _spikes = new List<GameObject>();
    private CancellationTokenSource _cts;
    
    private Collider _collider;
    private ICombat _combat;

    private void OnEnable()
    {
        _collider = GetComponent<Collider>();
        _collider.enabled = false; //最初は判定をとらない

        //棘のオブジェクトを全てリストに追加する
        for (int i = 0; i < _spikeCount; i++)
        {
            GameObject spike = _thorn.transform.GetChild(i).gameObject;
            _spikes.Add(spike);
            spike.SetActive(false); //非表示にしておく
        }
    }

    public void SetCombat(ICombat newCombat)
    {
        _combat = newCombat;
    }

    /// <summary>
    /// 棘オブジェクトを表示する
    /// </summary>
    public async UniTaskVoid ChangedMesh(CancellationToken token)
    {
        _collider.enabled = true;

        //棘を一本ずつ地上に突き出させる
        for (int i = 0; i < _spikes.Count; i++)
        {
            if (token.IsCancellationRequested) return; //キャンセル

            GameObject spike = _spikes[i];
            spike.SetActive(true);

            spike.transform.localPosition -= new Vector3(0, _spikeHeight, 0); // 地面の下にセット

            //棘を突き出すアニメーション
            spike.transform.DOLocalMoveY(_spikeHeight, _duration).SetEase(Ease.OutQuint);

            //ディレイ時間を決める（だんだん早くなるように）
            int delay = (int)(_delayBetweenSpikes - 0.03f * i) * 1000;
            await UniTask.Delay(delay, cancellationToken: token);
        }

        // 一定時間後に消滅
        await UniTask.Delay((int)(1.7f * 1000), cancellationToken: token);

        // 棘を地面の下へ移動させる
        await _thorn.transform.DOLocalMoveY(-_spikeHeight, 0.5f).SetEase(Ease.OutCubic).ToUniTask();

        //一定時間経過したらオブジェクトを削除する
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        var target = other.gameObject.GetComponent<IDamageable>();
        if (target != null)
        {
            _combat.DamageHandler.ApplyDamage(
                target: target, //攻撃対象
                baseDamage: _combat.BaseAttackPower * _damageMag, //攻撃力 
                defense: 0, //相手の防御力
                attacker: gameObject); //攻撃を加えるキャラクターのゲームオブジェクト
        }
    }
    
    public UniTask Fire()
    {
        throw new System.NotImplementedException();
    }
}
