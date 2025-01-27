using UniRx;
using UnityEngine;

/// <summary>
/// エネミーがステージから落下したときの処理を書いたクラス
/// </summary>
public class EnemyFall : MonoBehaviour
{
    [SerializeField] private float _fallHeight = -10f;
    private Transform _enemy;

    private void Start()
    {
        _enemy = this.transform;
        
        Observable
            .EveryUpdate()
            .Where(_ => _enemy.position.y < _fallHeight)
            .Take(1)
            .Subscribe(_ =>
            {
                gameObject.SetActive(false);
                
                UIManager.Instance.UnregisterEnemy(this.gameObject.GetComponent<EnemyBrain>()); //HPスライダーを削除する
            })
            .AddTo(this);
    }
}
