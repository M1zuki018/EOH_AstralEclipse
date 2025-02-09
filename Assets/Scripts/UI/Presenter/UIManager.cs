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
    [SerializeField] private DamageAmountUIPool _damegeUIPool; //ダメージ量を表示するテキストのオブジェクトプール
    [SerializeField] private QuestUpdateUI _questUpdateUI; //クエスト更新時に一瞬表示するUI
    [SerializeField] private MiniMapUI _miniMapUI;
    [SerializeField] private SliderUI _bossHPUI; //ボスのHPゲージ
    [SerializeField] private SliderUI _bossWillUI; //ボスのWillゲージ
    [SerializeField] private SliderUI _bossDpsCheakUI; //DPSチェック用のゲージ
    [SerializeField] private TextUI _bossName; //ボスの名前UI
    [SerializeField] private TextUI _bossRemainingHP; //ボスの残りHPのパーセント表記のUI
    [SerializeField] private TextUI _questMessage; //クエスト中の警告を表示するテキスト
    [SerializeField] private IconUI _firstExplain; //最初のゲーム説明のテキストウィンドウ
    [SerializeField] private TextUI _gameStartText; //「GameStart」の文字UI
    
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
    
    /// <summary>プレイヤーのHPゲージを非表示にする</summary>
    public void HidePlayerHP() => _playerHP.Hide();
    
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
    public void UpdateQuestText(string text) => _questText?.SetText(text);

    /// <summary>エネミーのHPバーを表示</summary>
    public void ShowEnemyHP(EnemyBrain enemy) => _enemyHpSliders[enemy].IsActive();
    
    /// <summary>エネミーのHPバーを隠す</summary>
    public void HideEnemyHP(EnemyBrain enemy) => _enemyHpSliders[enemy]?.IsNotActive();

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

    /// <summary>クエストテキストとミニマップを表示する</summary>
    public void ShowRightUI()
    {
        _questText.Show();
        _miniMapUI.Show();
    }
    
    /// <summary>クエストテキストとミニマップを非表示にする</summary>
    public void HideRightUI()
    {
        _questText.Hide();
        _miniMapUI.Hide();
    }

    /// <summary>ダメージテキストを表示する</summary>
    public void ShowDamageAmount(int damage, Transform target)
    {
        DamageAmountUI damageAmount = _damegeUIPool.GetInstance();
        if (target.gameObject.CompareTag("Player"))
        {
            damageAmount.Show(damage, target, false, true); //プレイヤーの場合
        }
        else
        {
            damageAmount.Show(damage, target, false, false); //敵の場合
        }
        
    }

    /// <summary>クエスト更新時に黄色い光のアニメーションを表示する</summary>
    public void QuestUpdate() => _questUpdateUI.Show();
    
    /// <summary>ボスのHPゲージを更新する</summary>
    public void UpdateBossHP(int currentHP) => _bossHPUI.SetValue(currentHP);
    
    /// <summary>ボスのHPゲージを初期化する</summary>
    public void InitializeBossHP(int maxValue, int defaultValue) => _bossHPUI.InitializeValue(maxValue, defaultValue);
    
    /// <summary>ボスのWillゲージを更新する</summary>
    public void UpdateBossWill(int value) => _bossWillUI.SetValue(value);
    
    /// <summary>ボスのWillゲージを初期化する</summary>
    public void InitializeBossWill(int maxValue, int defaultValue) => _bossWillUI.InitializeValue(maxValue, defaultValue);
    
    /// <summary>ボスのDPSチェック用ゲージを更新する</summary>
    public void UpdateBossDpsSlider(int value) => _bossDpsCheakUI.SetValue(value);
    
    /// <summary>ボスのDPSチェック用ゲージを初期化する</summary>
    public void InitializeBossDpsSlider(int maxValue, int defaultValue) => _bossDpsCheakUI.InitializeValue(maxValue, defaultValue);
    
    /// <summary>ボスのDPSチェック用ゲージを表示する</summary>
    public void ShowBossDpsSlider() => _bossDpsCheakUI.Show();
    
    /// <summary>ボスのDPSチェック用ゲージを非表示にする</summary>
    public void HideBossDpsSlider() => _bossDpsCheakUI.Hide();
    
    /// <summary>ボスの名前表記を更新する</summary>
    public void UpdateBossName(string text) => _bossName.SetText(text);
    
    /// <summary>ボスの残りHPパーセント表示を更新する</summary>
    public void UpdateRemainingHP(int value) => _bossRemainingHP.SetText($"{value}%");

    /// <summary>ボスに関連したUIを表示する</summary>
    public void ShowBossUI()
    {
        _bossHPUI.Show();
        _bossWillUI.Show();
        _bossName.Show();
        _bossRemainingHP.Show();
    }
    
    /// <summary>ボスに関連したUIを非表示にする</summary>
    public void HideBossUI()
    {
        _bossHPUI.Hide();
        _bossWillUI.Hide();
        _bossName.Hide();
        _bossRemainingHP.Hide();
    }

    /// <summary>クエスト進行中の警告を表示する</summary>
    public void ShowQuestMessage() => _questMessage.Show();
    
    /// <summary>クエスト進行中の警告を非表示にする</summary>
    public void HideQuestMessage() => _questMessage.Hide();
    
    /// <summary>ゲーム開始時の説明を表示する</summary>
    public void ShowFirstText() => _firstExplain.Show();
    
    /// <summary>ゲーム開始時の説明を非表示にする</summary>
    public void HideFirstText() => _firstExplain.Hide();
    
    /// <summary>「GameStart」テキストを表示する</summary>
    public void ShowStartText() => _gameStartText.Show();
    
    
    /// <summary>「GameStart」テキストを非表示にする</summary>
    public void HideStartText() => _gameStartText.Hide();
}
