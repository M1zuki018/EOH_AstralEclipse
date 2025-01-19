using UnityEngine;

namespace PlayerSystem.ActionFunction
{
    public interface IClimbale
    {
        void StartClimbing();
        void EndClimbing();
        void HandleClimbing(Vector3 moveDirection);
        bool IsClimbing { get; }
    }
}