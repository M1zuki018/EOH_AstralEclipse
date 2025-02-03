
using Cysharp.Threading.Tasks;
using TMPro;
using UI.Interface;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ダメージの数字を表示します
/// </summary>
public class DamageAmountUI : MonoBehaviour, ITextUI
{
    [SerializeField, HighlightIfNull] private TMP_Text _text;
    [SerializeField, HighlightIfNull] private CanvasGroup _canvasGroup;
    [SerializeField, Comment("オフセット")] private Vector3 _offset = new Vector3(0, 2.5f, 0);
    [SerializeField, Comment("数字の散らばる範囲")] private Vector2 _dispersion = new Vector2(50, 50);
    
    private Camera _mainCamera;
    private Transform _target; //ダメージを表示するキャラクターのTransform
    private RectTransform _rectTransform;
    private DamageAmountUIPool _pool;

    private void Awake()
    {
        _mainCamera = Camera.main;
        _rectTransform = gameObject.transform.parent.GetComponent<RectTransform>();
    }

    /// <summary>
    /// 初期化
    /// </summary>
    public void Initialize(DamageAmountUIPool pool)
    {
        _pool = pool; //オブジェクトプールの参照を保持する
    }

    /// <summary>
    /// ダメージ量を表示する
    /// </summary>
    public async void Show(int damage, Transform target)
    {
        
        if (_mainCamera == null)
        {
            //nullの場合もう一度取得を試してみる
            _mainCamera = Camera.main;
        } 
            
        //数字の散らばりを作成
        Vector3 randomOffset = new Vector3(Random.Range(-_dispersion.x, _dispersion.x), Random.Range(-_dispersion.y, _dispersion.y), 0);
        
        // ワールド座標をスクリーン座標に変換
        Vector3 worldPosition = target.position + _offset + randomOffset;
        Vector3 screenPosition = _mainCamera.WorldToScreenPoint(worldPosition);
        
        screenPosition.z = 0; //Z座標は0に固定する
        
        _rectTransform.position = screenPosition; //移動
        SetText(damage.ToString()); //値を書き換える
        _canvasGroup.alpha = 1;
        
        await UniTask.DelayFrame(100);
        _pool.ReturnToPool(this); //プールに戻す
    }
    
    public void Hide()
    {
        _canvasGroup.alpha = 0;
    }

    /// <summary>
    /// ダメージ量を書き換える
    /// </summary>
    public void SetText(string text) => _text.text = text;
    
    //インターフェースのメソッドは使用しない
    public void Show() { }
    
}
