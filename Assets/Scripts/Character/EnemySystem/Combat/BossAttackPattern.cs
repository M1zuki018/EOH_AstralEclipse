using System.Collections.Generic;
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
    
    private List<GameObject > _verticalLasers = new List<GameObject>();
    private float _speed = 450f; //垂直レーザーのスピード

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
    public void GenerateThorns(Transform position)
    {
        
    }
    
    /// <summary>
    /// 茨の攻撃を行う
    /// 地面にエフェクトを表示し、「茨が生える予兆」 を見せる。茨は 時間経過で消える 
    /// </summary>
    public void FireThorns()
    {
        
    }

    /// <summary>
    /// 頭上から落とす広範囲攻撃(走って避ける)
    /// 「影」エフェクトを地面に表示し、どこに落ちるかを予告。落下地点に入ると、強力なダメージ＋吹き飛ばしを実装。
    /// </summary>
    public void AttackFromAbove(Transform position)
    {
        
    }

    /// <summary>
    /// 影移動　プレイヤーの背後にワープ
    /// </summary>
    public void ShadowMove()
    {
        
    }

    /// <summary>
    /// 影からの攻撃　近接攻撃(攻撃を当てるとのけぞらせることができる)
    /// </summary>
    public void ShadowAttack()
    {
        
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
