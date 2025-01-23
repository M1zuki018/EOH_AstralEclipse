using System.Collections.Generic;
using UI.View;
using UnityEngine;

/// <summary>
/// 全UIを管理するクラス
/// </summary>
public class UIManager : MonoBehaviour
{
    [SerializeField, Comment("UIの親")] private Transform _uiCanvas;
    [SerializeField] private SliderUI _playerHP; //プレイヤーのHPゲージ
    [SerializeField] private SliderUI _playerWill; //プレイヤーのWillゲージ
    [SerializeField] private SliderUI _playerTP; //プレイヤーのTPゲージ
    [SerializeField] private List<IconUI> _skillIcons = new List<IconUI>(); //スキルアイコン
    [SerializeField] private GameObject _enemyHPPrefab; //エネミーのHPゲージのプレハブ

    private Dictionary<EnemyBrain, EnemyHPSliderUI> _enemyHpSliders = new Dictionary<EnemyBrain, EnemyHPSliderUI>();
    

    /// <summary>プレイヤーのHPゲージを更新する</summary>
    public void UpdatePlayerHP(int currentHP) => _playerHP.SetValue(currentHP);
    
    /// <summary>プレイヤーのHPゲージを初期化する</summary>
    public void InitializePlayerHP(int maxValue, int defaultValue) => _playerHP.InitializeValue(maxValue, defaultValue);

    /// <summary>プレイヤーのWillゲージを更新する</summary>
    public void UpdatePlayerWill(int value) => _playerWill.SetValue(value);

    /// <summary>プレイヤーのTPゲージを更新する</summary>
    public void UpdatePlayerTP(int value) => _playerTP.SetValue(value);

    /// <summary>スキルアイコンの操作を行う</summary>
    public void SelectedSkillIcon(int index)
    {
        foreach (var icon in _skillIcons)
        {
            if (icon == _skillIcons[index - 1])
            {
                icon.Select();
            }
            else
            {
                icon.Deselect();
            }
        }
        
    }

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
}
