using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

/// <summary>
/// ボスの影攻撃を管理するクラス
/// </summary>
public class ShadowAttack : MonoBehaviour, IBossAttack
{
    public string AttackName => "ShadowAttack";
    
    [Header("初期設定")]
    [SerializeField, Comment("移動速度")] private float moveSpeed = 10f;
    [SerializeField, Comment("振れ幅")] private float snakeAmplitude = 0.3f;
    [SerializeField, Comment("周波数")] private float snakeFrequency = 1.5f;
    [SerializeField, Comment("ワープの幅(X)")] private Vector2 xRange = new Vector2(85f, 130f);
    [SerializeField, Comment("ワープの幅(Z)")] private Vector2 zRange = new Vector2(210f, 250f);
    
    [SerializeField] private GameObject _shadowPrefab;
    [SerializeField] private Transform _bossObj;
    [SerializeField] private Renderer _bossRenderer;
    [SerializeField] private GameObject _warpPrefab;
    
    private CharacterController _cc;
    private GameObject _shadowObj;
    private Animator _animator;
    private Transform _playerTransform; //プレイヤーの位置参照
    private BossMover _bossMover;

    private int _count = 0;
    [SerializeField] private int _attackDuration = 3;

    private void Start()
    {
        _cc = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _playerTransform = GameObject.FindWithTag("Player").transform; // プレイヤーの参照を取得
        _bossMover = GetComponent<BossMover>();
        SetDissolveValue(0);
    }

    public async UniTask Fire()
    {
        await ShadowLatent(); //影に潜る
    }
    
    /// <summary>
    /// 影に潜る
    /// </summary>
    private async UniTask ShadowLatent()
    {
        //影のオブジェクトを生成する処理
        _shadowObj = Instantiate(_shadowPrefab, transform);
        Quaternion parentRotation = transform.rotation;
        _shadowObj.transform.localRotation = parentRotation * Quaternion.Euler(90f, 0f, 0f); //90度回転させる
        _shadowObj.transform.localPosition = new Vector3(0f, 0.1f, 0f); //影の位置をY=0.1に設定

        //影に潜る処理
        float duration = 1.5f;
        _bossObj.DOMoveY(-2.5f, duration).SetEase(Ease.InOutQuad).AsyncWaitForCompletion(); //ボスのメッシュを含むオブジェクトを移動
        _bossMover.Falling(); //CCのオブジェクト自体を重力を使って地面に接地させる
        UpdateDissolveValue(1, duration);

        await UniTask.Delay(1500);
        
        ShadowMove();
    }

    /// <summary>
    /// 影移動
    /// </summary>
    private void ShadowMove()
    {
        float elapsedTime = 0f; //経過時間
        
        //移動処理
        Observable
            .EveryUpdate()
            .TakeWhile(_ => Vector3.Distance(transform.position, _playerTransform.position) > 1f) //プレイヤーとの距離が1f以下になるまで処理を行う
            .Subscribe(_ =>
            {
                elapsedTime += Time.deltaTime;
                
                Vector3 direction = (_playerTransform.position - transform.position).normalized; //プレイヤーとのベクトルを求める
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
        //実体化処理
        float duration = 0.7f;
        _bossObj.DOMoveY(0.25f, duration).SetEase(Ease.OutQuad); //ボスのメッシュを含むオブジェクトを移動
        UpdateDissolveValue(0, duration);

        await UniTask.Delay(600);
        
        _shadowObj.SetActive(false);

        await UniTask.Delay(300); //一瞬おいてから攻撃開始
        
        ShadowFire();
        Destroy(_shadowObj);
    }

    /// <summary>
    /// 近接攻撃
    /// </summary>
    private async void ShadowFire()
    {
        _animator.SetInteger("AttackType", 5);
        _animator.SetTrigger("Attack");

        await UniTask.Delay((int)(_attackDuration * 1000));
        
        //攻撃無効状態になっていたらDPSチェックに遷移する
        if (_bossMover.IsDamageImmunity)
        {
            _count = 0;
            _bossMover.DPSCheak();
            return;
        }
        
        WarpToPosition().Forget();
    }
    
    /// <summary>
    /// ワープ処理
    /// </summary>
    public async UniTask WarpToPosition()
    {
        Sequence warpSequence = DOTween.Sequence();
        Vector3 initialPosition = _bossObj.position; // 初期位置を保存
        
        float duration = 1f; //縮小にかける時間
        float liftHeight = 1f; //縮小時に上に持ち上げる高さ
        
        GameObject warpHole = Instantiate(_warpPrefab, new Vector3(transform.position.x, 2.5f, transform.position.z - 1),
            Quaternion.identity); //その場にワープホールを召喚
        
        //収縮
        warpSequence.Append(_bossObj.DOScale(new Vector3(1f, 1f, 0.1f), duration / 2).SetEase(Ease.InBack)//Z軸方向に縮める
            .OnComplete(() =>
            {
                _bossObj.DOScale(Vector3.one * 0.1f, duration / 2).SetEase(Ease.OutQuad); //全体的に縮める
                _bossObj.DOMoveY(initialPosition.y + liftHeight, duration / 2).SetEase(Ease.OutQuad); //少し上に持ち上げる
            })); 
        warpSequence.Join(_bossObj.DOLocalMoveZ(-1, duration)); //ワープホール方向に移動させる
        warpSequence.Join(_bossObj.DOShakePosition(duration, 0.1f)); //揺らす
        warpSequence.Join(warpHole.transform.DOScale(Vector3.one * 0.1f, duration)
            .SetEase(Ease.InBack)); // ワープエフェクトも縮小させる
        
        //収縮後、テレポートする処理        
        warpSequence.AppendCallback(() =>
        {
            //テレポート先をランダムに決定する
            float randomX = Random.Range(xRange.x, xRange.y);
            float randomZ = Random.Range(zRange.x, zRange.y);
            Vector3 newPosition = new Vector3(randomX, 2.5f, randomZ);
            
            warpHole.transform.position = new Vector3(randomX, 2.5f, randomZ - 1);
            _cc.Move(newPosition - transform.position); //差分だけ移動させる);
        });
        
        
        //拡大
        duration = 0.7f; //拡大にかける時間
        warpSequence.Append(_bossObj.DOScale(Vector3.one, duration).SetEase(Ease.OutBack)); // 拡大
        warpSequence.Join(warpHole.transform.DOScale(new Vector3(1.5f, 2f, 1f), duration)
            .SetEase(Ease.OutBack)); // ワープエフェクトも拡大

        warpSequence.AppendCallback(() =>
        {
            Destroy(warpHole);

            _count++; //攻撃した回数を増やす
            
            //2回攻撃したら休憩
            if (_count >= 2)
            {
                _count = 0;
                _bossMover.Break();
                return;
            }
            
            ShadowLatent();
        });
    }

    /// <summary>
    /// マテリアルのディゾルブ効果の値をすぐに変更する
    /// </summary>
    private void SetDissolveValue(float cutOff)
    {
        _bossRenderer.materials[0].SetFloat("_Cutoff", cutOff);
        _bossRenderer.materials[1].SetFloat("_Cutoff", cutOff);
    }
    
    /// <summary>
    /// マテリアルのディゾルブ効果の値を徐々に変更する
    /// </summary>
    private void UpdateDissolveValue(float cutOff, float duration)
    {
        _bossRenderer.materials[0].DOFloat(cutOff, "_Cutoff", duration).SetEase(Ease.InOutQuad);
        _bossRenderer.materials[1].DOFloat(cutOff, "_Cutoff", duration).SetEase(Ease.InOutQuad);
    }
}
