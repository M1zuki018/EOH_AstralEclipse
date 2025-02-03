using System;
using System.Collections.Generic;
using Cinemachine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
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
    private float _defaultFOV;
    private int _currentCameraIndex; //現在のカメラ
    
    //Volume
    [SerializeField] private Volume _volume;
    private MotionBlur _motionBlur;
    private Vignette _vignette;
    private ChromaticAberration _chromaticAberration;
    private Bloom _bloom;
    
    //そのほか
    [SerializeField] private HitStop _hitStop;
    
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
                _defaultFOV = _virtualCameras[i].m_Lens.FieldOfView; //FOVを更新・保存
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
            = Mathf.Lerp(_defaultFOV, _defaultFOV + 2, 0.2f);

        _motionBlur.intensity.value = Mathf.Lerp(_motionBlur.intensity.value, 0.8f, 0.2f); //モーションブラー
        _vignette.intensity.value = Mathf.Lerp(_vignette.intensity.value, 0.5f, 0.2f); //ビネット
        _chromaticAberration.intensity.value = Mathf.Lerp(_chromaticAberration.intensity.value, 0.5f, 0.2f); //色収差
    }
    
    /// <summary>
    /// ステップエフェクトを解除する
    /// </summary>
    public void EndStepEffect()
    {
        _virtualCameras[_currentCameraIndex].m_Lens.FieldOfView
            = Mathf.Lerp(_defaultFOV, _defaultFOV - 2, 0.3f);

        _motionBlur.intensity.value = Mathf.Lerp(_motionBlur.intensity.value, 0.1f, 0.3f);
        _vignette.intensity.value = Mathf.Lerp(_vignette.intensity.value, 0.25f, 0.3f);
        _chromaticAberration.intensity.value = Mathf.Lerp(_chromaticAberration.intensity.value, 0.05f, 0.3f);
    }

    /// <summary>
    /// ダッシュエフェクト（プレイヤーの一段目の攻撃などに使用）
    /// </summary>
    public void DashEffect()
    {
        // FOVを拡張してスピード感を演出
        DOTween.To(
            () => _virtualCameras[_currentCameraIndex].m_Lens.FieldOfView, 
            x => _virtualCameras[_currentCameraIndex].m_Lens.FieldOfView = x, 
            _defaultFOV + 30, 
            0.2f); // 30度拡大する

        // 画面効果を加える
        _motionBlur.intensity.value = Mathf.Lerp(_motionBlur.intensity.value, 0.8f, 0.3f);
        _vignette.intensity.value = Mathf.Lerp(_vignette.intensity.value, 0.7f, 0.2f); //ビネット
        _chromaticAberration.intensity.value =  Mathf.Lerp(_chromaticAberration.intensity.value, 0.6f, 0.3f);

        // カメラを揺らす（突進の力強さ）
        _virtualCameras[_currentCameraIndex].transform.DOShakePosition(0.3f, 0.5f, 30);
    }
    
    /// <summary>
    /// 突進終了時のエフェクト
    /// </summary>
    public void EndDashEffect()
    {
        // FOVを元に戻す
        DOTween.To(
            () => _virtualCameras[_currentCameraIndex].m_Lens.FieldOfView, 
            x => _virtualCameras[_currentCameraIndex].m_Lens.FieldOfView = x, 
            _defaultFOV, 
            0.3f);

        // 画面効果を元に戻す
        _motionBlur.intensity.value = Mathf.Lerp(_motionBlur.intensity.value, 0.1f, 0.3f);
        _vignette.intensity.value = Mathf.Lerp(_vignette.intensity.value, 0.25f, 0.3f);
        _chromaticAberration.intensity.value = Mathf.Lerp(_chromaticAberration.intensity.value, 0.05f, 0.3f);

        // 軽くズームして衝撃感を出す
        _virtualCameras[_currentCameraIndex].transform.DOShakePosition(0.2f, 0.3f, 10);
    }
    
    /// <summary>
    /// 回転斬りのエフェクト（プレイヤーの四段目の攻撃などに使用）
    /// </summary>
    public void TurnEffect()
    {
        // FOVの拡張は控えめ
        DOTween.To(
            () => _virtualCameras[_currentCameraIndex].m_Lens.FieldOfView, 
            x => _virtualCameras[_currentCameraIndex].m_Lens.FieldOfView = x, 
            _defaultFOV - 10, 
            0.2f);

        // 画面効果を加える
        _motionBlur.intensity.value = Mathf.Lerp(_motionBlur.intensity.value, 1.2f, 0.15f);
        _vignette.intensity.value = Mathf.Lerp(_vignette.intensity.value, 0.7f, 0.15f); //ビネット
        _chromaticAberration.intensity.value =  Mathf.Lerp(_chromaticAberration.intensity.value, 1f, 0.15f);

        // カメラを揺らす（突進の力強さ）
        _virtualCameras[_currentCameraIndex].transform.DOShakePosition(0.15f, 0.5f, 30);
    }

    /// <summary>
    /// ヒットストップ
    /// </summary>
    public void ApplyHitStop(float duration)
    {
        _hitStop.ApplyHitStop(duration);
    }
    
    /// <summary>
    /// エフェクトつきのヒットストップ
    /// </summary>
    public async void ApplyHitStopWithEffects(float duration)
    {
        //_vignette.color.value = new Color(1f, 0.5f, 0.26f, 0.5f); //ビネットの色を変更する
        _vignette.intensity.value = Mathf.Lerp(0.5f, _vignette.intensity.value, duration); //ビネット
        _hitStop.ApplyHitStop(duration); //ヒットストップの処理
        
        await UniTask.Delay(TimeSpan.FromSeconds(duration));
        
        //徐々に戻す
        DOTween.To(
            () => _vignette.color.value, 
            x => _vignette.color.value = x, 
            Color.black, duration);
    }
    
}
