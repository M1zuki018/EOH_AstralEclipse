using PlayerSystem.State;
using UnityEngine;

namespace PlayerSystem.Movement
{
    /// <summary>
    /// 移動処理を補助する
    /// </summary>
    public class MovementHelper
    {
        private Transform _cameraTransform;
        private PlayerBlackBoard _bb;
        private CharacterController _cc;
        private float _rotationSpeed = 10f;

        public MovementHelper(Transform cameraTransform, PlayerBlackBoard bb,
            CharacterController cc)
        {
            _cameraTransform = cameraTransform;
            _bb = bb;
            _cc = cc;
        }

        /// <summary>
        /// カメラ基準で移動方向を計算し正規化したベクトルを返す
        /// </summary>
        public Vector3 CalculateMoveDirection()
        {
            Vector3 cameraForward = Vector3.ProjectOnPlane(_cameraTransform.forward, Vector3.up).normalized;
            Vector3 cameraRight = Vector3.ProjectOnPlane(_cameraTransform.right, Vector3.up).normalized;
            Vector3 moveDirection = cameraForward * _bb.MoveDirection.z + cameraRight * _bb.MoveDirection.x;
            _bb.CorrectedDirection = moveDirection.normalized; // 黒板の情報を書き換え
            return　moveDirection.normalized;
        }

        /// <summary>
        /// キャラクターの回転をカメラの向きに合わせる
        /// </summary>
        public void RotateCharacter(Vector3 moveDirection)
        {
            if (_bb.MoveDirection.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
                _cc.transform.rotation = Quaternion.Slerp(
                    _cc.transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
            }
        }
    }

}