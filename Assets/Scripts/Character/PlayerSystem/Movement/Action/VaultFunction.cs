using PlayerSystem.ActionFunction;
using UnityEngine;

/// <summary>
/// ヴォルトの動きを提供する
/// </summary>
public class VaultFunction : MonoBehaviour, IVaultable
{
    private Transform _targetTransform; //乗り越える障害物
    [SerializeField] private float _startAnimTime;
    [SerializeField] private float _endAnimTime;
    private PlayerMovement _playerMovement;
    private MatchTargetWeightMask _mask = new MatchTargetWeightMask(Vector3.one, 1f);
    private float _timer;
    
    

    private void Update()
    {
        if (_playerMovement is null)
        {
            return;
        }
        
        if (_playerMovement.PlayerState.IsVaulting)
        {
            _playerMovement._animator.MatchTarget(
                _targetTransform.position,
                _targetTransform.rotation,
                AvatarTarget.LeftHand,
                _mask,
                _startAnimTime,
                _endAnimTime);
            _timer += Time.deltaTime;
        }

        //アニメーションの終了判定
        if (_timer >= _endAnimTime)
        {
            _playerMovement.PlayerState.IsVaulting = false;
            _timer = 0;
        }
    }

    public void Vault()
    {
        throw new System.NotImplementedException();
    }
}
