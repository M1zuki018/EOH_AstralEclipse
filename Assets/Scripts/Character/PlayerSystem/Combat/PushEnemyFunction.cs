using UnityEngine;

/// <summary>
/// 攻撃時に敵にぶつかったら、敵を押し返す機能
/// </summary>
public class PushEnemyFunction : MonoBehaviour
{
    [SerializeField] private float _pushForce = 5f;
    [SerializeField] private float _pushRadius = 1f;
    [SerializeField] private LayerMask _enemyLayer; //敵のレイヤー

    private CharacterController _playerController;
    
    private void Start()
    {
        _playerController = GetComponent<CharacterController>();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Enemy")) // 敵と衝突した場合
        {
            // プレイヤーが敵に近づいている状態を作り、押し飛ばす処理を行う
            PushEnemy(hit.gameObject);
        }
    }
    
    /// <summary>
    /// 敵を押し飛ばす処理
    /// </summary>
    private void PushEnemy(GameObject enemy)
    {
        TryGetComponent(out Animator enemyAnimator);
        enemyAnimator.applyRootMotion = false;
        
        // 衝突点を計算
        Vector3 direction = enemy.transform.position - transform.position;  // プレイヤーから敵へのベクトル
        direction.y = 0;  // 水平方向のみの押し飛ばし

        // 敵のCharacterControllerを取得
        CharacterController enemyController = enemy.GetComponent<CharacterController>();
        if (enemyController != null)
        {
            // 敵に加える力を決定
            enemyController.Move(direction.normalized * _pushForce);
        }
        
        enemyAnimator.applyRootMotion = true;
    }
}
