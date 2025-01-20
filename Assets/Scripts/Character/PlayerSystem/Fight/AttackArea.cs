using UnityEngine;

public class AttackArea : MonoBehaviour
{
    private Collider _collider;
    private EnemyHealth _enemyHealth;
    [SerializeField, HighlightIfNull] PlayerCombat _playerCombat;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out _enemyHealth))
        {
            _enemyHealth.TakeDamage(_playerCombat.AttackDamage);
        }
    }
}
