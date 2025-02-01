using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponEffectManager : MonoBehaviour
{
    public static WeaponEffectManager Instance;

    [SerializeField] private ParticleSystem _effect;
    
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
    /// 攻撃エフェクトを再生する
    /// </summary>
    public void PlayEffect(Vector3 position, Quaternion rotation)
    {
        _effect.transform.position = position;
        _effect.transform.rotation = rotation;
        _effect.Play();
    }
}
