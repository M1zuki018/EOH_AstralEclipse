using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;

/// <summary>
/// ボスの攻撃メソッドをまとめたもの
/// </summary>
public class BossAttackPattern : MonoBehaviour
{
    [SerializeField] private Transform _target; //プレイヤーのTransform
    [SerializeField] private LaserParticle _laserParticle;
    [SerializeField] private GameObject _verticalLaserPrefab;
    [SerializeField] private GameObject _thornPrefab;
    [SerializeField] private GameObject _abovePrefab;
    
    private List<GameObject > _verticalLasers = new List<GameObject>();
    private float _speed = 120f; //垂直レーザーのスピード
    private float _premotionTime = 1f; //茨攻撃の予兆時間

    /// <summary>
    /// 水平方向のレーザー
    /// </summary>
    public void HorizontalLaser(Transform position, float effectTime)
    {
        float elapsedTime = 0f;
        
        Observable
            .EveryUpdate()
            .TakeWhile(_ => elapsedTime < effectTime) //指定した秒数に達するまで処理を行う
            .Subscribe(_ => 
            { 
                elapsedTime += Time.deltaTime;
                _laserParticle.transform.position = position.position; //レーザーの始点を調整
                _laserParticle.Fire(position);
            }, () =>
            {
                _laserParticle.Stop(); //レーザーを止める
            })
            .AddTo(this);
    }

    /// <summary>
    /// 垂直方向のレーザーを生成して位置を変更してからリストに加える
    /// </summary>
    public void GenerateVerticalLaser(Vector3 position)
    {
        GameObject obj = Instantiate(_verticalLaserPrefab);
        obj.transform.position = position;
        _verticalLasers.Add(obj);
    }

    /// <summary>
    /// レーザーを発射する
    /// 放つ時にプレイヤーと自分のベクトル方向にムーブスピードかけて飛ばす
    /// </summary>
    public void FireVerticalLaser(int index)
    {
        //プレイヤーとのベクトルを求める
        Vector3 direction = (_target.transform.position - _verticalLasers[index].transform.position).normalized;
        
        Observable
            .EveryUpdate()
            .TakeWhile(_ => _verticalLasers[index] != null)
            .Subscribe(_ =>
            {
                // プレイヤーの方向へ一定の速度で移動
                _verticalLasers[index].transform.Translate(direction * _speed * Time.deltaTime, Space.World);
            })
            .AddTo(this);
    }

    /// <summary>
    /// 茨を生成する。プレイヤーの位置を一定時間ごとに更新してターゲット設定
    /// </summary>
    public void GenerateThorns()
    {
        GameObject thorn = Instantiate(_thornPrefab); //予兆エリアを生成
        thorn.TryGetComponent(out ThornContorl thornCtrl);
        
        float elapsedTime = 0f;
        Observable
            .EveryUpdate()
            .TakeWhile(_ => elapsedTime < _premotionTime) //予兆時間の間だけ行う
            .Subscribe(_ =>
            {
                elapsedTime += Time.deltaTime;
                thorn.transform.position =
                    new Vector3(_target.transform.position.x, 0.7f, _target.transform.position.z); //プレイヤーの足元に表示
            }, () => 
            {
                FireThorns(thornCtrl); //攻撃を行う
            }) 
            .AddTo(this);
    }
    
    /// <summary>
    /// 茨の攻撃を行う
    /// </summary>
    private async void FireThorns(ThornContorl thornCtrl)
    {
        await UniTask.Delay(1500); //少し時間を置く
        thornCtrl.ChangedMesh(); //メッシュ変更とオブジェクト破棄
    }

    /// <summary>
    /// 頭上から落とす広範囲攻撃(走って避ける)
    /// </summary>
    public async void AttackFromAbove()
    {
        //プレイヤーの頭上にエリアを生成
        GameObject aboveObj = Instantiate(_abovePrefab);
        aboveObj.transform.position = new Vector3(
            _target.transform.position.x, _target.transform.position.y + 10f, _target.transform.position.z); 
        
        await UniTask.Delay(3000); //待って避けられるようにする
        
        //地面に墜落した後オブジェクト削除
        aboveObj.transform.DOMoveY(-1, 0.5f).OnComplete(() => Destroy(aboveObj)); 
        //TODO:強力なダメージ＋吹き飛ばしを実装
    }

    public async void ShadowAttack()
    {
        ShadowMove();
    }
    
    /// <summary>
    /// 影移動
    /// </summary>
    private void ShadowMove()
    {
        //transform.DOMoveY(-5, 0.5f); //影に潜る
        
        float moveSpeed = 8f; //移動速度
        float snakeAmplitude = 0.3f; //振れ幅
        float snakeFrequency = 1.5f; //周波数
        float elapsedTime = 0f; //経過時間
        
        //移動処理
        Observable
            .EveryUpdate()
            .TakeWhile(_ => Vector3.Distance(transform.position, _target.position) > 0.5f) //プレイヤーとの距離が0.5f以下になるまで処理を行う
            .Subscribe(_ =>
            {
                elapsedTime += Time.deltaTime;
                
                Vector3 direction = (_target.position - transform.position).normalized; //プレイヤーとのベクトルを求める
                Vector3 sideVector = Vector3.Cross(Vector3.up, direction);
                
                float snakeOffset = Mathf.Sin(elapsedTime * snakeFrequency) * snakeAmplitude; //サイン波を求める
                Vector3 moveVector =  (direction * moveSpeed * Time.deltaTime) + (sideVector * snakeOffset * 0.5f); //移動量を計算

                transform.position += moveVector;
                //transform.position += new Vector3(moveVector.x, 0f, moveVector.z); //高さは固定
            }, () =>
            {
                ShadowArrived();
            })
            .AddTo(this);
    }
    
    /// <summary>
    /// 影から実体化する処理
    /// </summary>
    private async void ShadowArrived()
    { 
        //TODO:溶けて出てくるような、ディゾルブ効果をつけたい
        Debug.Log("影が到達");
        // 実体化処理
        //transform.DOMoveY(5f, 0.5f);

        await UniTask.Delay(500); //一瞬おいてから攻撃開始
        
        ShadowFire();
    }

    /// <summary>
    /// 近接攻撃
    /// </summary>
    private void ShadowFire()
    {
        Debug.Log("攻撃");
    }

    /// <summary>
    /// 時間操作　プレイヤーの入力を受け付けない 色彩をモノクロに変更 空中にエネルギーが集まる演出 空が暗くなる or 背景が歪むエフェクトを追加しても良い。
    /// ボスが魔法陣や時計ギミックを展開し、「時間を操る」演出。画面に時計の針や歯車エフェクトを一瞬表示して「発動の合図」。時間減速 or 短時間停止（約1〜2秒）
    /// タイムスケールを 0.3〜0.5 にする。（完全停止はしない）プレイヤーの移動速度や攻撃速度が低下。ボスは通常の1.5倍速で移動しながら攻撃する（スピード感を出す）。
    /// </summary>
    public void TimeControl()
    {
        
    }

    /// <summary>
    /// 解除と同時に何か強力な攻撃 ボスが全力のエネルギー弾を放つ 画面が一瞬フラッシュ
    /// 時間解除と同時に攻撃（0.5秒後）モノクロ解除と同時に「レーザー発射」や「ワープ近接攻撃」を仕掛ける。「当たるとダメージ＋吹き飛ばし」くらいの威力。
    /// </summary>
    public void FireTimeAttack()
    {
        
    }

    /// <summary>
    /// 本気の時間操作（残りHP10%で発動）
    /// 完全な時間停止を行い、プレイヤーを拘束。「モノクロ化」＋「音の消失」＋「重力の変化」など、異常な演出を追加。
    /// 解除と同時に、即死級に近い大技を発動。（ただし回避手段を用意する）
    /// 発動予兆（約2秒）HP10%を切ると、ボスがフィールド中央へワープ。ボスが「両手を掲げる」「時計の針を高速回転させる」などの演出を入れる。
    /// 画面の色彩が徐々に色が抜けていく 周囲の空間が歪み始め、背景エフェクトが「時空の裂け目」みたいになる。
    /// UIの時計やカウントダウン的な演出を画面端に表示（「00:03… 00:02… 00:01…」）。BGMがフェードアウトし、無音になる（緊張感を増す）。
    /// </summary>
    public void FinalTimeControl()
    {
        
    }

    /// <summary>
    /// 完全時間停止（約4〜5秒）
    /// タイムスケールを 0.01以下（ほぼ完全停止） にする。プレイヤーの入力を一切受け付けない（キー操作を無効化）。
    /// 画面全体が 完全なモノクロ になり、SEも「エコーがかかったような遅延音」に変わる。
    /// ボスは 自由に動きながらプレイヤーの周囲を回り、攻撃の準備 をする。巨大な魔法陣を展開し、解除時にフィールド全体を攻撃。
    /// </summary>
    public void TimeStop()
    {
        
    }

    /// <summary>
    /// 時間解除 & 強力な攻撃発動（約1秒）
    /// 解除直前に「時計の針が高速回転」→「一瞬だけ時が動き出すエフェクト」。タイムスケールを一気に1.0に戻し、色彩が元に戻る（急激なコントラスト変化）。
    /// 同時に、強力な攻撃を発動
    ///「フィールド全体に衝撃波」→ 一定範囲外に回避しないと即死。
    /// </summary>
    public void FinalAttack()
    {
        
    }

    /// <summary>
    /// フィニッシュムーブ 本気の時間操作の後、ボスは弱体化（移動が遅くなる・攻撃が単調になる）。
    /// プレイヤーが最後の攻撃を決めるチャンス。ボス撃破時、時間が一瞬スローモーションになり、「完全に時が崩壊する」
    /// </summary>
    public void After()
    {
        
    }
}
