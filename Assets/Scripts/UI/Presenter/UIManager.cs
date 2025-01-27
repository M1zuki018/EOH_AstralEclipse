using System.Collections.Generic;
using UI.View;
using UnityEngine;

/// <summary>
/// 全UIを管理するクラス
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    
    [SerializeField, Comment("UIの親")] private Transform _uiCanvas;
    [SerializeField] private SliderUI _playerHP; //プレイヤーのHPゲージ
    [SerializeField] private SliderUI _playerTP; //プレイヤーのTPゲージ
    [SerializeField] private List<IconUI> _skillIcons = new List<IconUI>(); //スキルアイコン
    [SerializeField] private GaugeUI _stepGauge; //ステップ用のゲージ
    [SerializeField] private NumberUI _stepCount; //ステップの使用可能回数を表示するテキスト
    [SerializeField] private LockOnUI _lockOnIcon; //ロックオンアイコン
    [SerializeField] private GameObject _enemyHPPrefab; //エネミーのHPゲージのプレハブ
    [SerializeField] private TextUI _questText; //クエストを表示するテキスト
    [SerializeField] private IconUI _fadePanel; //フェード用のパネル
    [SerializeField] private GameObject _damageAmount; //ダメージ量を表示するテキストのプレハブ
    
    private Dictionary<EnemyBrain, EnemyHPSliderUI> _enemyHpSliders = new Dictionary<EnemyBrain, EnemyHPSliderUI>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); //インスタンスを生成
        }
        else
        {
            Destroy(gameObject); //既にあったら破棄する
        }
    }

    /// <summary>プレイヤーのHPゲージを更新する</summary>
    public void UpdatePlayerHP(int currentHP) => _playerHP.SetValue(currentHP);
    
    /// <summary>プレイヤーのHPゲージを初期化する</summary>
    public void InitializePlayerHP(int maxValue, int defaultValue) => _playerHP.InitializeValue(maxValue, defaultValue);
    
    /// <summary>プレイヤーのTPゲージを初期化する</summary>
    public void InitializePlayerTP(int maxValue, int defaultValue) => _playerTP.InitializeValue(maxValue, defaultValue);
    
    /// <summary>プレイヤーのTPゲージを更新する</summary>
    public void UpdatePlayerTP(int value) => _playerTP.SetValue(value);

    /// <summary>スキルアイコンの操作を行う</summary>
    public void SelectedSkillIcon(int index)
    {
        foreach (var icon in _skillIcons)
        {
            if (icon == _skillIcons[index - 1]) icon.Select();
            else icon.Deselect();
        }
    }

    /// <summary>ステップゲージの値を更新する</summary>
    public void UpdateStepGauge(float endValue, float duration) => _stepGauge.ResetAndSetValue(endValue, duration);

    /// <summary>ステップカウントの値を更新する</summary>
    public void UpdateStepCount(int value) => _stepCount.SetNumber(value);

    /// <summary>ステップUIを表示する</summary>
    public void ShowStepUI() => _stepGauge.Show();

    /// <summary>ステップUIを隠す</summary>
    public void HideStepUI() => _stepGauge.Hide();
    
    /// <summary>ロックオンアイコンの位置を変更する</summary>
    public void SetLockOnUI(Transform targetTransform) => _lockOnIcon.IsActive(targetTransform);
    
    /// <summary>ロックオンアイコンを非表示にする</summary>
    public void HideLockOnUI() => _lockOnIcon.IsNotActive();
    
    /// <summary>クエスト内容を表示するUIを更新する</summary>
    public void UpdateQuestText(string text) => _questText.SetText(text);

    /// <summary>エネミーのHPバーを表示</summary>
    public void ShowEnemyHP(EnemyBrain enemy) => _enemyHpSliders[enemy].IsActive();
    
    /// <summary>エネミーのHPバーを隠す</summary>
    public void HideEnemyHP(EnemyBrain enemy) => _enemyHpSliders[enemy].IsNotActive();

    /// <summary>エネミーのHPゲージを更新する</summary>
    public void UpdateEnemyHP(EnemyBrain enemy, int currentHP) => _enemyHpSliders[enemy].SetValue(currentHP);
    
    /// <summary>エネミーのHPスライダーのUIを作成する</summary>
    public void RegisterEnemy(EnemyBrain enemy, int maxHP)
    {
        if (!_enemyHpSliders.ContainsKey(enemy)) //まだディクショナリに未登録だった場合
        {
            GameObject uiObject = Instantiate(_enemyHPPrefab, _uiCanvas); //親のCanvasの子にスライダーを生成
            EnemyHPSliderUI healthUI = uiObject.GetComponent<EnemyHPSliderUI>();
            healthUI.SetTarget(enemy.transform); //対象のエネミーのトランスフォームを渡す
            _enemyHpSliders.Add(enemy, healthUI); //ディクショナリに追加
            healthUI.InitializeValue(maxHP, maxHP); //初期化
        }
    }

    /// <summary>エネミーのHPスライダーのUIを削除する</summary>
    public void UnregisterEnemy(EnemyBrain enemy)
    {
        if (_enemyHpSliders.TryGetValue(enemy, out EnemyHPSliderUI healthUI))
        {
            Destroy(healthUI.gameObject);
            _enemyHpSliders.Remove(enemy);
        }
    }

    /// <summary>フェードパネルを表示する</summary>
    public void FadeOut() => _fadePanel.Show();
    
    /// <summary>フェードパネルを非表示にする</summary>
    public void FadeIn() => _fadePanel.Hide();

    /// <summary>バトル用UIをすべて表示する</summary>
    public void ShowPlayerBattleUI()
    {
        _playerHP.ShowAndSlide();   
        _playerTP.ShowAndSlide();
        foreach (var icon in _skillIcons)
        {
            icon.ShowAndSlide();
        }
    }

    /// <summary>バトル用UIをすべて非表示にする</summary>
    public void HidePlayerBattleUI()
    {
        _playerHP.HideAndSlide();
        _playerTP.HideAndSlide();
        foreach (var icon in _skillIcons)
        {
            icon.HideAndSlide();
        }
    }

    /// <summary>ダメージテキストを表示する</summary>
    public void ShowDamageAmount(int damage)
    {
        GameObject uiObject = Instantiate(_damageAmount, _uiCanvas);
        DamageAmountUI damageAmount = uiObject.GetComponent<DamageAmountUI>();
        damageAmount.Show(damage);
    }
}
