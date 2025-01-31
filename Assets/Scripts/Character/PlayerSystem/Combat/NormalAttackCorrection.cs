using PlayerSystem.Fight;
using UnityEngine;

/// <summary>
/// モーションのうちキャラクターの向きや移動を補正するクラス
/// </summary>
public class NormalAttackCorrection : MonoBehaviour, IAttackCorrection
{
    [SerializeField] private float _moveSpeed = 1.0f;
    [SerializeField] private bool _adjustDirection = true;

    private CharacterController _cc;
    private Transform _player;
    private ICombat _combat;

    private void Start()
    {
        _cc = GetComponent<CharacterController>();
        _player = transform;
        _combat = GetComponent<ICombat>();
    }

    /// <summary>
    /// 移動補正の処理
    /// </summary>
    public void CorrectMovement(Vector3 forwardDirection)
    {
        if (_adjustDirection)
        {
            AdjustDirectionToTarget();
        }
        Vector3 move = forwardDirection * _moveSpeed * Time.deltaTime; // 移動量の計算
        _cc.Move(move);  // 実際の移動処理
    }

    /// <summary>
    /// 回転補正の処理
    /// </summary>
    public void AdjustDirectionToTarget()
    {
        _combat?.AdjustDirection.AdjustDirectionToTarget();  //向きの補正
    }
}
