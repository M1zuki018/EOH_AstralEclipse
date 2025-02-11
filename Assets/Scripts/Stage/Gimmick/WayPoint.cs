using DG.Tweening;
using UnityEngine;

/// <summary>
/// 目標地点を管理する
/// </summary>
public class Waypoint : MonoBehaviour
{
    private WayPointSystem _wayPointSystem;
    private RespawnEvent _respawn;
    private Material _material;

    /// <summary>
    /// 目標地点システムを参照をセットする
    /// </summary>
    public void Initialize(WayPointSystem wps, RespawnEvent respawn)
    {
        _wayPointSystem = wps;
        _respawn = respawn;
        _material = GetComponentInChildren<Renderer>().material; //子の柱状のオブジェクトから
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _respawn.SetRespawn(other.gameObject.transform.position, other.gameObject.transform.rotation); //リスポーン地点を更新
            _wayPointSystem.NextWaypoint(); //次の地点のアイコンを表示する
            AudioManager.Instance.PlaySE(10);
        }
    }

    /// <summary>
    /// アイコンの表示非表示を切り替える
    /// </summary>
    public void SetActive(bool isActive)
    {
        if (isActive)
        {
            gameObject.SetActive(isActive);
            _material.DOFade(0.5f, 0.5f).SetEase(Ease.OutQuart);
            transform.DOMoveY(8f, 0.5f).SetEase(Ease.OutQuart);
        }
        else
        {
            _material.DOFade(0f, 0.5f).SetEase(Ease.OutQuart);
            transform.DOMoveY(12f, 0.5f).SetEase(Ease.OutQuart).OnComplete(() => gameObject.SetActive(isActive));
        }
    }
}