using UnityEngine;

/// <summary>
/// 臨戦態勢を判定するクラス
/// </summary>
[RequireComponent(typeof(SphereCollider))]
public class ReadyForBattleChecker : MonoBehaviour
{
    [SerializeField] float _radius;
    [SerializeField] string _tag;
    private SphereCollider _collider;
    public bool ReadyForBattle { get; private set; }

    private void Start()
    {
        _collider = GetComponent<SphereCollider>();
        _collider.radius = _radius; //半径をセットする
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_tag == "Enemy" && other.CompareTag("Player"))
        {
            ReadyForBattle = true; //タグがエネミーの場合は、プレイヤータグを検知する
        }
        else if (_tag == "Player" && other.CompareTag("Enemy"))
        {
            ReadyForBattle = true; //タグがプレイヤーの場合は、エネミータグを検知する
        }
        else
        {
            ReadyForBattle = false;
        }
    }
}
