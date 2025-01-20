using UnityEngine;

/// <summary>
/// ターゲットマッチングのためのステートマシンビヘイビア
/// </summary>
public class MatchPositionSMB : StateMachineBehaviour
{
    public IMatchTarget _target;

    [Header("ターゲットマッチングの設定")] 
    [SerializeField] private AvatarTarget _targetBodyPart = AvatarTarget.Root; //マッチさせたいアバターの箇所
    [SerializeField] private Vector2 _effectiveRange; //マッチを行いたい長さ
    
    [Header("アシスト設定")]
    [SerializeField, Range(0,1)] private float _assistPower = 1; //補正の強さ
    [SerializeField, Range(0,10)] private float _assistDistance = 1; //補正をかける距離
    
    private MatchTargetWeightMask _weightMask;
    private bool _isSkip = false;
    private bool _isInitialized = false;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!_isInitialized)
        {
            var weight = new Vector3(_assistPower, 0, _assistPower);
            _weightMask = new MatchTargetWeightMask(weight, 0);
            _isInitialized = true;
        }
        
        //自分の距離と相手の距離が補正をかける距離より長いかどうか判定する
        _isSkip = Vector3.Distance(_target.TargetPosition, animator.transform.position) > _assistDistance;
    }

    public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(_isSkip || animator.IsInTransition(layerIndex))
            return;

        if (stateInfo.normalizedTime > _effectiveRange.y)
        {
            animator.InterruptMatchTarget(false);
        }
        else
        {
            animator.MatchTarget(
                _target.TargetPosition,
                animator.bodyRotation,
                _targetBodyPart,
                _weightMask,
                _effectiveRange.x, _effectiveRange.y);
        }
    }
}

/// <summary>
/// ターゲットの座標を取得するインターフェース
/// </summary>
public interface IMatchTarget
{
    Vector3 TargetPosition { get; }
}
