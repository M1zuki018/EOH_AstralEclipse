using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// レーザービームPrefabにつけるクラス
/// </summary>
public class LaserParticle : MonoBehaviour, IBossAttack
{
    public string AttackName => "HorizontalLaser";
    
    [SerializeField] private ParticleSystem _laserEffect;
    public GameObject LaserEffect => _laserEffect.gameObject;

    /// <summary>
    /// レーザーを放つ
    /// </summary>
    public void Fire(Transform firePoint)
    {
        _laserEffect.transform.position = firePoint.position;
        _laserEffect.Play();
    }

    /// <summary>
    /// レーザーを止める
    /// </summary>
    public void Stop()
    {
        _laserEffect.Stop();
    }
    
    public UniTask Fire()
    {
        throw new System.NotImplementedException();
    }
}