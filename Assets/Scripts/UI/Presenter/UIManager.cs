using System.Collections.Generic;
using UI.View;
using UnityEngine;

/// <summary>
/// 全UIを管理するクラス
/// </summary>
public class UIManager : MonoBehaviour
{
    [SerializeField] private SliderUI _playerHP; //プレイヤーのHPゲージ
    [SerializeField] private SliderUI _playerWill; //プレイヤーのWillゲージ
    [SerializeField] private SliderUI _playerTP; //プレイヤーのTPゲージ
    private List<SliderUI> _enemyHPSliders = new List<SliderUI>(); //エネミーのHPゲージのリスト

    /// <summary>プレイヤーのHPゲージを更新する</summary>
    public void UpdatePlayerHP(float value) => _playerHP.SetValue(value);

    /// <summary>プレイヤーのWillゲージを更新する</summary>
    public void UpdatePlayerWill(float value) => _playerWill.SetValue(value);

    /// <summary>プレイヤーのTPゲージを更新する</summary>
    public void UpdatePlayerTP(float value) => _playerTP.SetValue(value);

    /// <summary>エネミーのHPスライダーを更新する</summary>
    public void UpdateEnemyHP(float value, int index) => _enemyHPSliders[index].SetValue(value);
}
