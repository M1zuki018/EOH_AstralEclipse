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
    private CinemachineBasicMultiChannelPerlin _noise; // シェイク用ノイズ
    
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
        Instance = this;
        
        _volume.profile.TryGet(out _motionBlur);
        _volume.profile.TryGet(out _vignette);
        _volume.profile.TryGet(out _chromaticAberration);
        
        CinemachineFramingTransposer transposer =
            _virtualCameras[0].GetCinemachineComponent<CinemachineFramingTransposer>();
        transposer.m_CameraDistance = 3f; //メインカメラの距離を初期化
        
        //レーザーカメラの設定
        _noise = _virtualCameras[4].GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        if (_noise != null)
        {
            _noise.m_AmplitudeGain = 0f; // 初期状態ではシェイクなし
            _noise.m_FrequencyGain = 0f; // 振動なし
        }
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

        //カメラのFOVをリセットする処理
        if (index == 0) SetFOV(0,60);
        //else if (index == 4) SetFOV(4, 40);
    }

    /// <summary>
    /// カメラのインデックスと値を指定してFOVを変更する
    /// </summary>
    public void SetFOV(int cameraIndex, float value)
    {
        _virtualCameras[cameraIndex].m_Lens.FieldOfView = value;
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
    /// 汎用的なエフェクト適用メソッド
    /// </summary>
    private void ApplyEffect(float fovChange, float motionBlur, float vignette, float chromaticAberration, float duration)
    {
        //FOVの調整
        DOTween.To(() => _virtualCameras[_currentCameraIndex].m_Lens.FieldOfView,
            x => _virtualCameras[_currentCameraIndex].m_Lens.FieldOfView = x,
            _defaultFOV + fovChange, duration)
            .OnComplete(() => ResetFOV()); //FOVが低くなりすぎていないか確認し必要なら35まで戻す

        //モーションブラー
        DOTween.To(() => _motionBlur.intensity.value, x => _motionBlur.intensity.value = x,
            motionBlur, duration);
        //ビネット
        DOTween.To(() => _vignette.intensity.value, x => _vignette.intensity.value = x,
            vignette, duration);
        //色収差
        DOTween.To(() => _chromaticAberration.intensity.value, x => _chromaticAberration.intensity.value = x,
            chromaticAberration, duration);
    }
    
    /// <summary>
    /// カメラを揺らし方を指定して揺らす
    /// </summary>
    private void ApplyCameraShake(float duration, float strength, int vibrato)
    {
        _virtualCameras[_currentCameraIndex].transform.DOShakePosition(duration, strength, vibrato);
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
    public void StepEffect() => ApplyEffect(2, 0.8f, 0.3f, 0.5f, 0.2f);
    
    /// <summary>
    /// ステップエフェクトを解除する
    /// </summary>
    public void EndStepEffect() => ApplyEffect(-2, 0.1f, 0.25f, 0.05f, 0.3f);

    /// <summary>
    /// ダッシュエフェクト（プレイヤーの一段目の攻撃などに使用）
    /// </summary>
    public void DashEffect()
    {
        ApplyEffect(20, 0.8f, 0.4f, 0.5f, 0.3f);
        ApplyCameraShake(0.3f, 0.5f, 30);
    }
    
    /// <summary>
    /// 突進終了時のエフェクト
    /// </summary>
    public void EndDashEffect()
    {
        ApplyEffect(-20, 0.1f, 0.25f, 0.05f, 0.3f);
        ApplyCameraShake(0.2f, 0.3f, 10);
    }
    
    /// <summary>
    /// 回転斬りのエフェクト（プレイヤーの四段目の攻撃などに使用）
    /// </summary>
    public void TurnEffect()
    {
        ApplyEffect(-20f, 1f, 0.4f, 0.5f, 0.15f);
        ApplyCameraShake(0.15f, 0.5f, 30);
    }
    
    /// <summary>
    /// メインカメラのFOVが35以下になったときに徐々にFOVを戻す
    /// </summary>
    private void ResetFOV()
    {
        if (_currentCameraIndex == 0 && _virtualCameras[_currentCameraIndex].m_Lens.FieldOfView < 45f)
        {
            // 最小値以下になったら、DOTweenを使って自然に45まで戻す
            DOTween.To(() => _virtualCameras[_currentCameraIndex].m_Lens.FieldOfView, 
                x => _virtualCameras[_currentCameraIndex].m_Lens.FieldOfView = x,
                45f, 2f).SetEase(Ease.OutQuad);
        }
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
        //ビネット
        DOTween.To(() => _vignette.color.value, x => _vignette.color.value = x, 
            new Color(1f, 0.2f, 0.2f, 0.5f), duration);
        DOTween.To(() => _vignette.intensity.value, x => _vignette.intensity.value = x,
            0.5f, duration);
        
        _hitStop.ApplyHitStop(duration); //ヒットストップの処理
        
        await UniTask.Delay(TimeSpan.FromSeconds(duration));
        
        //徐々に戻す
        DOTween.To(() => _vignette.color.value, x => _vignette.color.value = x, 
            Color.black, duration);
    }
    
    /// <summary>
    /// レーザー発射前の演出（ズーム）
    /// </summary>
    public void PreLaserShot()
    {
        float zoomDuration = 1.5f; //ズームにかける時間
        
        UseCamera(4); //レーザー演出カメラを優先
        _virtualCameras[4].m_Lens.FieldOfView = 60;
        DOTween.To(() => _virtualCameras[4].m_Lens.FieldOfView, x => _virtualCameras[4].m_Lens.FieldOfView = x, 
            50, zoomDuration).SetEase(Ease.InOutQuad);
    }
    
    /// <summary>
    /// レーザー照射時のカメラの揺れ
    /// </summary>
    public async void CameraShakeOnFire()
    {
        float shakeAmplitude = 2.0f; //振幅（シェイクの強さ）
        float shakeFrequency = 1.5f; //周波数（振動の速さ）
        float shakeDuration = 3f;  //シェイク時間
        
        _noise.m_AmplitudeGain = shakeAmplitude; // シェイク開始
        _noise.m_FrequencyGain = shakeFrequency; // シェイクの振動速度
        
        await UniTask.Delay((int)(shakeDuration * 1000));

        _noise.m_AmplitudeGain = 0; //解除
        _noise.m_FrequencyGain = 0;
    }
    
    /// <summary>
    /// 爆発の余韻を映す（遠景にカメラを引く）
    /// </summary>
    public void ExplosionEffect()
    {
        UseCamera(0); //メインカメラに戻す
        DOTween.To(() => _virtualCameras[4].m_Lens.FieldOfView, x => _virtualCameras[4].m_Lens.FieldOfView = x, 
            60, 1.5f).SetEase(Ease.OutQuad);
    }

    /// <summary>
    /// プレイヤーが死亡した時のカメラ処理
    /// </summary>
    public void PlayerDeath()
    {
        UseCamera(0);
        
        var framingTransposer = _virtualCameras[0].GetCinemachineComponent<CinemachineFramingTransposer>();
        var pov = _virtualCameras[0].GetCinemachineComponent<CinemachinePOV>();
        
        // カメラをプレイヤーに寄せる
        Vector3 targetPosition = GameObject.FindWithTag("Player").transform.position + new Vector3(0, 2, -3);
        _virtualCameras[0].transform.DOMove(targetPosition, 1.5f).SetEase(Ease.InOutCubic);
        
        framingTransposer.m_DeadZoneWidth = 0.1f; 
        framingTransposer.m_DeadZoneHeight = 0.1f;
        
        pov.m_VerticalAxis.Value = 10f;  //POVの調整

        // ゆっくりズームアウト
        DOTween.To(() => _virtualCameras[0].m_Lens.FieldOfView, x => _virtualCameras[0].m_Lens.FieldOfView = x,
            40, 2f).SetEase(Ease.InOutQuad);
    }

    /// <summary>
    /// 一瞬色収差効果をかける
    /// </summary>
    public void ChromaticAberration(float endvalue, float duration)
    {
        float defaultValue = _chromaticAberration.intensity.value;
        //色収差
        DOTween.To(() => _chromaticAberration.intensity.value, x => _chromaticAberration.intensity.value = x,
        endvalue, duration).OnComplete(()=> 
            DOTween.To(() => _chromaticAberration.intensity.value, x => _chromaticAberration.intensity.value = x, 
                defaultValue - endvalue, duration));
    }
}
