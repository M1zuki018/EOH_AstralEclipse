using UnityEngine;

/// <summary>
/// 壁のぼり中のIK
/// 両手と両足の位置を壁に吸着するように設定する
/// </summary>
public class ClimbingIK : MonoBehaviour
{
    [SerializeField, HighlightIfNull] Animator animator;
    
    public bool IkActive { get; set; }
    
    // 壁のターゲットとなる位置
    [HighlightIfNull] public Transform LeftHandTarget;
    [HighlightIfNull] public Transform RightHandTarget;
    [HighlightIfNull] public Transform LeftFootTarget;
    [HighlightIfNull] public Transform RightFootTarget;

    private void OnAnimatorIK(int layerIndex)
    {
        if (IkActive)
        {
            // IKを有効化
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0.1f);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0.1f);
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0.1f);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0.1f);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0.1f);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 0.1f);
            animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0.1f);
            animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 0.1f);

            // 手と足をターゲットに配置
            if (LeftHandTarget != null)
            {
                animator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandTarget.position);
                animator.SetIKRotation(AvatarIKGoal.LeftHand, LeftHandTarget.rotation);
            }
            if (RightHandTarget != null)
            {
                animator.SetIKPosition(AvatarIKGoal.RightHand, RightHandTarget.position);
                animator.SetIKRotation(AvatarIKGoal.RightHand, RightHandTarget.rotation);
            }
            if (LeftFootTarget != null)
            {
                animator.SetIKPosition(AvatarIKGoal.LeftFoot, LeftFootTarget.position);
                animator.SetIKRotation(AvatarIKGoal.LeftFoot, LeftFootTarget.rotation);
            }
            if (RightFootTarget != null)
            {
                animator.SetIKPosition(AvatarIKGoal.RightFoot, RightFootTarget.position);
                animator.SetIKRotation(AvatarIKGoal.RightFoot, RightFootTarget.rotation);
            }
        }
    }
}
