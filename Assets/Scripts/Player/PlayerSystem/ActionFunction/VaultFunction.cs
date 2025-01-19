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
    
    public void HandleVault(PlayerMovement playerMovement)//アニメーターがとってきたいがための引数
    {
        _playerMovement = playerMovement;
        _targetTransform = _playerMovement._valutTargetObjects[0];
        playerMovement._animator.SetTrigger("Vault");//アニメーション再生 
        playerMovement.IsVault = true;
    }

    private void Update()
    {
        if (_playerMovement is null)
        {
            return;
        }
        
        if (_playerMovement.IsVault)
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
            _playerMovement.IsVault = false;
            _timer = 0;
        }
    }

    public void Vault()
    {
        throw new System.NotImplementedException();
    }
}
