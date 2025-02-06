using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

/// <summary>
/// ボスの攻撃メソッドをまとめたもの
/// </summary>
public class BossAttackPattern : MonoBehaviour
{
    [SerializeField] private Transform _target; //プレイヤーのTransform
    [SerializeField] private EnemyCombat _combat;
    [SerializeField] private LaserParticle _laserParticle; //水平レーザーのパーティクル
    [SerializeField] private GameObject _verticalLaserPrefab; //垂直レーザーのプレハブ
    [SerializeField] private GameObject _thornPrefab; //茨攻撃のプレハブ
    [SerializeField] private GameObject _abovePrefab; //上から降ってくる攻撃のプレハブ
    [SerializeField] private GameObject _magicCirclePrefab; //魔法陣のプレハブ
    [SerializeField] private Volume _timeStopVolume; //時間停止中のGlobalVolume
    [SerializeField] private AudioMixerGroup _bgmMixer; 
    [SerializeField] private Material _glitchy; //グリッチシェーダーをかけたマテリアル

    [SerializeField] private GameObject _boss;
    [SerializeField] private GameObject _shadowPrefab;
    
    private Animator _animator;
    public Animator Animator => _animator;
    private PlayerInput _playerInput;
    private Material _defaultMaterial;
    private GameObject _shadowObj;
    private CharacterController _cc;
    public Vector3 DefaultTransform { get; set; }
    
    private List<GameObject> _verticalLasers = new List<GameObject>();
    private List<MagicCircleControl> _magicCircles = new List<MagicCircleControl>();
    private float _speed = 120f; //垂直レーザーのスピード
    private float _premotionTime = 1f; //茨攻撃の予兆時間

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _defaultMaterial = RenderSettings.skybox;
        _playerInput = _target.gameObject.GetComponent<PlayerInput>();
        _cc = GetComponent<CharacterController>();
    }
    
    /// <summary>
    /// 水平方向のレーザー
    /// </summary>
    public void HorizontalLaser(Transform position, float effectTime)
    {
        float elapsedTime = 0f;
        _laserParticle.transform.position = position.position; //レーザーの始点を調整
        _laserParticle.LaserEffect.SetActive(true);
        
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
                _laserParticle.LaserEffect.SetActive(false);
            })
            .AddTo(this);
    }

    #region 垂直レーザー

    /// <summary>
    /// 垂直方向のレーザーを生成して位置を変更してからリストに加える
    /// </summary>
    public void GenerateVerticalLaser(Vector3 position)
    {
        GameObject obj = Instantiate(_verticalLaserPrefab);
        obj.transform.position = position;
        var controller = obj.GetComponent<VirticalLaserControl>();
        controller.SetCombat(_combat);
        _verticalLasers.Add(obj);
    }

    /// <summary>
    /// 垂直方向のレーザーのリストをリセットする
    /// </summary>
    public void ResetVerticalLasers()
    {
        _verticalLasers.Clear();
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

    #endregion
    
    #region 茨
    /// <summary>
    /// 茨を生成する。プレイヤーの位置を一定時間ごとに更新してターゲット設定
    /// </summary>
    public void GenerateThorns()
    {
        _animator.SetInteger("AttackType", 2);
        _animator.SetTrigger("Attack");
        
        GameObject thorn = Instantiate(_thornPrefab); //予兆エリアを生成
        thorn.TryGetComponent(out ThornContorl thornCtrl);
        thornCtrl.SetCombat(_combat);
        
        float elapsedTime = 0f;
        Observable
            .EveryUpdate()
            .TakeWhile(_ => elapsedTime < _premotionTime) //予兆時間の間だけ行う
            .Subscribe(_ =>
            {
                elapsedTime += Time.deltaTime;
                thorn.transform.position =
                    new Vector3(_target.transform.position.x, 0.2f, _target.transform.position.z); //プレイヤーの足元に表示
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
        await UniTask.Delay(520);
        
        _animator.SetTrigger("Attack");
        
        await UniTask.Delay(980); //少し時間を置く
        thornCtrl.ChangedMesh(); //メッシュ変更とオブジェクト破棄
    }

    #endregion

    #region パターン2

    /// <summary>
    /// 頭上から落とす広範囲攻撃(走って避ける)
    /// </summary>
    public async void AttackFromAbove()
    {
        _animator.SetInteger("AttackType", 3);
        _animator.SetTrigger("Attack");
        
        //プレイヤーの頭上にエリアを生成
        GameObject aboveObj = Instantiate(_abovePrefab);
        aboveObj.TryGetComponent(out AboveControl aboveCtrl);
        aboveCtrl.SetCombat(_combat);
        
        await UniTask.Delay(1400); //待って避けられるようにする
        
        aboveCtrl.Attack();
        _animator.SetTrigger("Attack");
        
        await UniTask.Delay(1600);
        
        //TODO:強力なダメージ＋吹き飛ばしを実装
    }
    
    /// <summary>
    /// 影に潜る
    /// </summary>
    public void ShadowLatent()
    {
        _cc.Move(DefaultTransform - transform.position); //デフォルトのTransformとの差分だけ移動させておく
        _shadowObj = Instantiate(_shadowPrefab, transform); //子オブジェクトに影オブジェクトを追加して保持
        _boss.transform.DOMoveY(-2.5f, 1.5f).OnComplete(() => ShadowMove()); //影に潜る
    }
    
    /// <summary>
    /// 影移動
    /// </summary>
    public void ShadowMove()
    {
        _cc.Move(new Vector3(0f, -4.9f, 0f)); //オブジェクトの位置をずらす
        
        float moveSpeed = 10f; //移動速度
        float snakeAmplitude = 0.3f; //振れ幅
        float snakeFrequency = 1.5f; //周波数
        float elapsedTime = 0f; //経過時間
        
        //移動処理
        Observable
            .EveryUpdate()
            .TakeWhile(_ => Vector3.Distance(transform.position, _target.position) > 1f) //プレイヤーとの距離が1f以下になるまで処理を行う
            .Subscribe(_ =>
            {
                elapsedTime += Time.deltaTime;
                
                Vector3 direction = (_target.position - transform.position).normalized; //プレイヤーとのベクトルを求める
                Vector3 sideVector = Vector3.Cross(Vector3.up, direction);
                
                float snakeOffset = Mathf.Sin(elapsedTime * snakeFrequency) * snakeAmplitude; //サイン波を求める
                Vector3 moveVector =  (direction * moveSpeed * Time.deltaTime) + (sideVector * snakeOffset * 0.5f); //移動量を計算

                _cc.Move(moveVector); //高さは固定
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
        _boss.transform.DOMoveY(0, 0.4f); //影から出る
        _shadowObj.SetActive(false);

        await UniTask.Delay(500); //一瞬おいてから攻撃開始
        
        ShadowFire();
        Destroy(_shadowObj);
    }

    /// <summary>
    /// 近接攻撃
    /// </summary>
    private void ShadowFire()
    {
        Debug.Log("攻撃");
        _animator.SetInteger("AttackType", 5);
        _animator.SetTrigger("Attack");
    }

    #endregion

    /// <summary>
    /// 時間操作
    /// </summary>
    public async void TimeControl()
    {
        Debug.Log("時間操作攻撃開始");
     
        _animator.SetInteger("AttackType", 6);
        _animator.SetTrigger("Attack");
        
        //TODO:背景が歪むエフェクト
        _timeStopVolume.enabled = true; //色彩をモノクロに変更
        Time.timeScale = 0.1f; //減速
        
        //演出
        Debug.Log("演出作成予定");
        SpawnMagicCircle(); //魔法陣を展開
        //TODO: 空中にエネルギーが集まる
        //TODO: プレイヤーの移動速度や攻撃速度が低下。ボスは通常の1.5倍速で移動しながら攻撃する
        
        await UniTask.Delay(800);
        //await UniTask.Delay(100); //テスト用
        
        Debug.Log("時間停止発動の合図の演出");
        
        _animator.SetTrigger("Attack"); //発動の演出
        _playerInput.DeactivateInput(); //一瞬操作できなくする
        //TODO: 画面に時計の針や歯車エフェクトを一瞬表示して「発動の合図」
        
        await UniTask.Delay(200); //演出を待つ
        
        FireTimeAttack();
    }

    /// <summary>
    /// 時間減速解除後の攻撃
    /// </summary>
    private async void FireTimeAttack()
    {
        _timeStopVolume.enabled = false; //画面のフィルターを元に戻す
        Time.timeScale = 1f; //時間の進みを戻す
        _playerInput.ActivateInput(); //プレイヤーの入力を解放
        
        //画面がフラッシュする
        
        //攻撃を放つ
        foreach (var magicCircleCtrl in _magicCircles)
        {
            magicCircleCtrl.Player = _target;
            magicCircleCtrl.Combat = _combat;
            magicCircleCtrl.Fire();
        }
        
        await UniTask.Delay(1500);
        
        //魔法陣のオブジェクトを削除したあと、リストをクリアする
        foreach (var magicCircle in _magicCircles)
        {
            Destroy(magicCircle.gameObject);
        }
        _magicCircles.Clear();
    }

    #region 魔法陣

    /// <summary>
    /// 魔法陣を生成する
    /// </summary>
    private void SpawnMagicCircle()
    {
        int countLower = 4; //下段に表示する魔法陣の数
        int countUpper = 5; //上段に表示する魔法陣の数
        float yOffsetLower = 0f; //ボスのY座標からどれだけ下に配置するか
        float yOffsetUpper = 12f; //ボスのY座標からどれだけ上に配置するか
        
        SpawnRow(countLower, yOffsetLower); //下段を生成
        SpawnRow(countUpper, yOffsetUpper); //上段を生成
    }

    /// <summary>
    /// 魔法陣のプレハブを生成する処理
    /// </summary>
    private void SpawnRow(int count, float baseYOffset)
    {
        float spacing = 18f; //魔法陣同士の間隔
        float yDropFactor = 2f; //外側に向かうほど下げるY座標の変化量
        
        float startX = DefaultTransform.x - (spacing * (count - 1) / 2); 
        float centerIndex = (count - 1) / 2f; 
        
        for (int i = 0; i < count; i++)
        {
            float x = startX + (i * spacing);
            float yDrop = Mathf.Abs(i - centerIndex) * yDropFactor; // 中心から離れるほど下げる
            float y =DefaultTransform.y + baseYOffset - yDrop;

            Vector3 position = new Vector3(x, y, DefaultTransform.z);
            GameObject magicCircle = Instantiate(_magicCirclePrefab, position, Quaternion.identity);
            _magicCircles.Add(magicCircle.GetComponent<MagicCircleControl>()); //リストに追加
        }
    }
    
    #endregion
    
    

    /// <summary>
    /// 死に際の時間操作（残りHP10%で発動）
    /// 画面の色彩が徐々に色が抜けていく 周囲の空間が歪み始め、背景エフェクトが「時空の裂け目」みたいになる。
    /// UIの時計やカウントダウン的な演出を画面端に表示（「00:03… 00:02… 00:01…」）。BGMがフェードアウトし、無音になる（緊張感を増す）。
    /// </summary>
    public async void FinalTimeControl()
    {
        //演出
        //時計の針を高速回転させる
        
        await UniTask.Delay(500); //演出を待つ
        
        Time.timeScale = 0.1f; //遅延
        _timeStopVolume.enabled = true; //画面のモノクロ化
        RenderSettings.skybox = _glitchy; //Skyboxを変更
        //_bgmMixer.audioMixer.SetFloat("MasterVolume", 1f); //音をくぐもらせる
        _playerInput.DeactivateInput(); //プレイヤーの入力を制限
        
        await UniTask.Delay(200);
        
        TimeStop();
    }

    /// <summary>
    /// 完全時間停止（約4〜5秒）
    /// ボスは 自由に動きながらプレイヤーの周囲を回り、攻撃の準備 をする。巨大な魔法陣を展開し、解除時にフィールド全体を攻撃。
    /// </summary>
    public async void TimeStop()
    {
        Debug.Log("時間停止");
        //BGMのvolumeを完全にゼロにする
        //SEにもエコーがかかったような遅延音に聞こえるような効果をかける
        
        await UniTask.Delay(300);
        
        FinalAttack();
    }

    /// <summary>
    /// 時間解除 & 強力な攻撃発動（約1秒）
    /// 解除直前に「時計の針が高速回転」→「一瞬だけ時が動き出すエフェクト」。タイムスケールを一気に1.0に戻し、色彩が元に戻る（急激なコントラスト変化）。
    /// </summary>
    public void FinalAttack()
    {
        Debug.Log("攻撃");
        Time.timeScale = 1f;
        _timeStopVolume.enabled = false; //画面エフェクトを通常に戻す
        RenderSettings.skybox = _defaultMaterial; //Skyboxを元に戻す
        _playerInput.ActivateInput(); //入力制限解除
        //即死攻撃の処理
    }

    /// <summary>
    /// フィニッシュムーブ 本気の時間操作の後、ボスは弱体化（移動が遅くなる・攻撃が単調になる）。
    /// プレイヤーが最後の攻撃を決めるチャンス。ボス撃破時、時間が一瞬スローモーションになり、「完全に時が崩壊する」
    /// </summary>
    public void After()
    {
        
    }
}
