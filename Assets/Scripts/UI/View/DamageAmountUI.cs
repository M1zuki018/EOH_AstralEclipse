
using Cysharp.Threading.Tasks;
using DG.Tweening;
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
    private Vector3 _startScale = Vector3.one; //初期サイズ
    
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
    public async void Show(int damage, Transform target, bool isCritical, bool isPlayerHit)
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
        
        //描画のための処理
        _rectTransform.position = screenPosition; //移動
        SetText(damage.ToString()); //値を書き換える
        _canvasGroup.alpha = 1; //透明度をリセット
        
        //演出のための判断
        Vector3 moveDirection = isPlayerHit ? Vector3.down * 15f : Vector3.up * 25f; //被ダメージなら下向きに移動させる
        Color textColor = isPlayerHit ? Color.red : //被ダメなら赤
            (isCritical ? new Color(1f, 0.5f, 0f) : Color.white); //敵のダメージの場合、クリティカルならオレンジにする 
        float scaleMultiplier = isCritical ? 1.5f : 1.2f; //テキストの拡大率。クリティカルならより大きくする
        float duration = isCritical ? 0.6f : 0.3f; //表示時間。クリティカルは長め
        float textSize = isPlayerHit ? 36 : 48;

        _text.color = textColor;
        _text.fontSize = textSize;
        transform.localScale = _startScale * (isCritical ? 1.2f : 1f); //クリティカルなら最初から少し大きく表示する
        
        //アニメーション開始
        Sequence seq = DOTween.Sequence();
        
        //表示（通常→拡大）
        seq.Append(transform.DOScale(_startScale * scaleMultiplier, 0.1f).SetEase(Ease.OutBack));

        if (isCritical)
        {
            //クリティカルは少し揺らす
            seq.Append(transform.DOShakePosition(0.3f, new Vector3(10f, 5f, 0f), 10, 90f)); 
        }

        //上方向に移動させつつフェードアウト
        seq.Append(transform.DOLocalMove(transform.localPosition + moveDirection, duration).SetEase(Ease.OutQuad));
        seq.Join(_text.DOFade(0, duration));

        //完了後はプールに戻す
        seq.OnComplete(() => _pool.ReturnToPool(this));

        seq.Play();
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
