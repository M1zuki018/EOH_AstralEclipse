using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PlayerSystem.Fight;
using UnityEngine;

/// <summary>
/// 攻撃判定を行い、指定されたコライダーを、指定されたフレームの間発生させる
/// </summary>
public class AttackHitDetector : MonoBehaviour
{
    //TODO: システム基盤を変更していく場合、Updateのタイミングを指定できるようにする

    [Header("設定")] 
    [SerializeField, Comment("衝突検出の所有者")] private CharacterBase _owner; 
    [SerializeField, Comment("衝突検出を行うレイヤー")] private LayerMask _hitLayer;
    [SerializeField, Comment("衝突するタグ")] private string[] _hitTags;
    
    private bool _isHitDetected = false; //ヒット検出フラグ
    private ICombat _combat;
    private float _hitDetectionDuration; //攻撃判定の持続時間
    private Collider _detectedCollider; 
    
    private void OnEnable()
    {
        _combat = GetComponent<ICombat>(); 
    }

    /// <summary>
    /// 判定を行うメソッド。IAttackCorrectionから呼び出すように
    /// </summary>
    public void DetectHit(Collider detectedCollider, float duration)
    {
        if(_isHitDetected) return; //既にヒットしていたら以降の処理は行わない
        
        _detectedCollider = detectedCollider;
        _hitDetectionDuration = duration;
        
        // ヒット判定の範囲を指定して範囲内の敵を検出
        if (_detectedCollider.isTrigger)
        {
            //コライダー内の全てのオブジェクトを取得
            Collider[] hitCollidrs = Physics.OverlapBox(_detectedCollider.bounds.center, _detectedCollider.bounds.extents, Quaternion.identity);

            foreach (var hit in hitCollidrs)
            {
                if (_hitTags.Contains(hit.tag)) //タグで衝突したオブジェクトをフィルタリング
                {
                    // ヒットしたオブジェクトにダメージを与える
                    var target = hit.gameObject.GetComponent<IDamageable>();
                    if (target != null)
                    {
                        _combat.DamageHandler.ApplyDamage(
                            target:target, //攻撃対象
                            baseDamage:_combat.BaseAttackPower, //攻撃力 
                            defense:0,  //相手の防御力
                            attacker:gameObject); //攻撃を加えるキャラクターのゲームオブジェクト
                    }
                
                    _isHitDetected = true;
                    StartCoroutine(HitDetectionCooldown());
                }
            }
        }
    }
    
    /// <summary>
    /// ヒット判定の持続時間終了を待ち、ヒット判定をリセットする
    /// </summary>
    private IEnumerator HitDetectionCooldown()
    {
        yield return new WaitForSeconds(_hitDetectionDuration);
        _isHitDetected = false;
    }
}