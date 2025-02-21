using UnityEngine;

namespace PlayerSystem.Movement
{
    /// <summary>
    /// プレイヤーの足跡を表現するトレイルを管理するクラス
    /// </summary>
    public class PlayerTrailController
    {
        private TrailRenderer _trailRenderer;

        public PlayerTrailController(TrailRenderer trailRenderer)
        {
            _trailRenderer = trailRenderer;
        }

        /// <summary>
        /// 軌跡を表示する
        /// </summary>
        public void EnableTrail()
        {
            _trailRenderer.emitting = true;
        }

        /// <summary>
        /// 軌跡を非表示にする
        /// </summary>
        public void DisableTrail()
        {
            _trailRenderer.emitting = false;
        }
    }
}
