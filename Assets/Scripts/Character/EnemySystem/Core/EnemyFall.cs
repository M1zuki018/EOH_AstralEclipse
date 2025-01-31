using UniRx;
using UnityEngine;

/// <summary>
/// エネミーがステージから落下したときの処理を書いたクラス
/// </summary>
public class EnemyFall : MonoBehaviour
{
    [SerializeField] private float _fallHeight = -10f;
    private Transform _enemy;
    private EnemyBrain _brain;

    private void Start()
    {
        //コンポーネントを取得
        _enemy = transform;
        _brain = GetComponent<EnemyBrain>();
        
        Observable
            .EveryUpdate()
            .Where(_ => _enemy.position.y < _fallHeight)
            .Take(1)
            .Subscribe(_ =>
            {
                gameObject.SetActive(false);
                _brain.Health.TakeDamage(_brain.GetCurrentHP(), GameObject.FindWithTag("Player")); //ダメージを受けて死亡判定にする
                UIManager.Instance.UnregisterEnemy(this.gameObject.GetComponent<EnemyBrain>()); //HPスライダーを削除する
                CameraManager.Instance.DeregisterTargetGroup(this.transform);
                CameraManager.Instance.UseCamera(0);
            })
            .AddTo(this);
    }
}
