using Cinemachine;
using UnityEngine;

/// <summary>
/// CinemachineFramingTransposer のカメラ前後方向のコントロールを付与する
/// </summary>
public class CinemachineDollyInputProvider : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _cinemachineVirtualCamera;
    
    [SerializeField] private CinemachineInputProvider _cinemachineInputProvider;
    
    [SerializeField] private float _sensitivity = 1f;
    
    [SerializeField] private float _minDistance = 1f;
    
    [SerializeField] private float _maxDistance = 5f;
    
    public float Sensitivity
    {
        get => _sensitivity;
        set => _sensitivity = value;
    }

    private void OnEnable()
    {
        _cinemachineInputProvider.ZAxis.action.Enable();
    }

    private void OnDisable()
    {
        _cinemachineInputProvider.ZAxis.action.Disable();
    }

    private void Awake()
    {
        var cinemachineComponent = _cinemachineVirtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
        if (cinemachineComponent is CinemachineFramingTransposer cinemachineFramingTransposer)
        {
            _cinemachineInputProvider.ZAxis.action.performed += context =>
            {
                float distance = Mathf.Clamp(cinemachineFramingTransposer.m_CameraDistance + context.ReadValue<float>() * _sensitivity, _minDistance, _maxDistance);
                cinemachineFramingTransposer.m_CameraDistance = distance;
            };
        }
        else
        {
            Debug.LogWarning($"{gameObject.name}:{nameof(CinemachineFramingTransposer)} が存在しません。");
        }
    }
}
