using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

/// <summary>
/// カメラの管理を行うクラス
/// </summary>
public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    [SerializeField] private List<CinemachineVirtualCamera> _virtualCameras = new List<CinemachineVirtualCamera>();
    [SerializeField] private CinemachineTargetGroup _targetGroup;
    
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
}
