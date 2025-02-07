using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

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
    
    [SerializeField] private GameObject _shadowPrefab;
    
    private CharacterController _cc;
    private GameObject _shadowObj;
    private Animator _animator;
    private Transform _playerTransform; //プレイヤーの位置参照
    
    private void Start()
    {
        _cc = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _playerTransform = GameObject.FindWithTag("Player").transform; // プレイヤーの参照を取得
    }

    public async UniTask Fire()
    {
        Debug.Log("影に潜る → 影移動 → 出現して攻撃 → ワープ → 繰り返し");
        ShadowLatent(); //影に潜る
    }
    
    /// <summary>
    /// 影に潜る
    /// </summary>
    private async void ShadowLatent()
    {
        _cc.Move(Vector3.down * 2.5f); //影に潜る
        _shadowObj = Instantiate(_shadowPrefab, transform); //子オブジェクトに影オブジェクトを追加して保持
        
        await UniTask.Delay(2000); //少し待機
        
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
        //TODO:溶けて出てくるような、ディゾルブ効果をつけたい
        Debug.Log("影が到達");
        // 実体化処理
        _cc.Move(Vector3.up * 2.5f); //影から出現
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
    
    /// <summary>
    /// ワープ処理（攻撃四段目のスクリプトから呼び出される）
    /// </summary>
    public async void WarpToPosition()
    {
        Vector3 position = new Vector3(93, 2, 230);
        _cc.Move(position - transform.position);
        
        await UniTask.Delay(500);

        ShadowLatent();
    }
}
