using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// カメラの管理を行うクラス
/// </summary>
public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    //カメラ
    [SerializeField] private List<CinemachineVirtualCamera> _virtualCameras = new List<CinemachineVirtualCamera>();
    [SerializeField] private CinemachineTargetGroup _targetGroup;
    [SerializeField] private CinemachineImpulseSource _impulseSource;
    
    //Volume
    [SerializeField] private Volume _volume;
    private MotionBlur _motionBlur;
    private Vignette _vignette;
    private ChromaticAberration _chromaticAberration;
    private Bloom _bloom;

    private int _currentCameraIndex; //現在のカメラ
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
        
        _volume.profile.TryGet(out _motionBlur);
        _volume.profile.TryGet(out _vignette);
        _volume.profile.TryGet(out _chromaticAberration);
        
    }

    /// <summary>
    /// Indexで指定したカメラを使用する
    /// </summary>
    public void UseCamera(int index)
    {
        for(int i = 0; i < _virtualCameras.Count; i++)
        {
            if (i != index)
            {
                _virtualCameras[i].Priority = 10; //その他のカメラ
            }
            else
            {
                _virtualCameras[i].Priority = 15; //使用するカメラ
                _currentCameraIndex = index;
            }
        }
    }

    /// <summary>
    /// ターゲットグループを使用します
    /// </summary>
    public void UseTargetGroup(Transform newTarget, float weight, float radius)
    {
        if (_targetGroup.m_Targets.Length > 1)
        {
            _targetGroup.RemoveMember(_targetGroup.m_Targets[1].target);
        }
        _targetGroup.AddMember(newTarget, weight, radius);
    }

    /// <summary>
    /// ターゲットグループから自身を削除します
    /// </summary>
    public void DeregisterTargetGroup(Transform target)
    {
        if (_targetGroup.m_Targets.Length > 0)
        {
            _targetGroup.RemoveMember(target);
        }
    }

    /// <summary>
    /// カメラを揺らす
    /// </summary>
    public void TriggerCameraShake()
    {
        _impulseSource.GenerateImpulse();
    }

    /// <summary>
    /// ステップ中のエフェクト
    /// </summary>
    public void StepEffect()
    {
        //FOVの調整
        _virtualCameras[_currentCameraIndex].m_Lens.FieldOfView
            = Mathf.Lerp(_virtualCameras[_currentCameraIndex].m_Lens.FieldOfView, _virtualCameras[_currentCameraIndex].m_Lens.FieldOfView + 2, 0.2f);

        _motionBlur.intensity.value = Mathf.Lerp(_motionBlur.intensity.value, 0.8f, 0.2f); //モーションブラー
        _vignette.intensity.value = Mathf.Lerp(_vignette.intensity.value, 0.5f, 0.2f); //ビネット
        _chromaticAberration.intensity.value = Mathf.Lerp(_chromaticAberration.intensity.value, 0.5f, 0.2f); //色収差
    }
    
    public void StepEffectEnd()
    {
        _virtualCameras[_currentCameraIndex].m_Lens.FieldOfView
            = Mathf.Lerp(_virtualCameras[_currentCameraIndex].m_Lens.FieldOfView, _virtualCameras[_currentCameraIndex].m_Lens.FieldOfView - 2, 0.3f);

        _motionBlur.intensity.value = Mathf.Lerp(_motionBlur.intensity.value, 0.2f, 0.3f);
        _vignette.intensity.value = Mathf.Lerp(_vignette.intensity.value, 0.25f, 0.3f);
        _chromaticAberration.intensity.value = Mathf.Lerp(_chromaticAberration.intensity.value, 0.05f, 0.3f);
    }
}
