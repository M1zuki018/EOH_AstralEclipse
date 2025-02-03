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
    [SerializeField] private Collider _detectedCollider; 
    
    private bool _isHitDetected = false; //ヒット検出フラグ
    private ICombat _combat;
    private float _hitDetectionDuration; //攻撃判定の持続時間
    private Coroutine _hitDetectionCoroutine;
    
    
    private void OnEnable()
    {
        _combat = GetComponentInParent<ICombat>(); 
    }

    /// <summary>
    /// 判定を行うメソッド。IAttackCorrectionから呼び出すように
    /// </summary>
    public void DetectHit(HitDetectionInfo info)
    {
        //初期化
        _detectedCollider.transform.localPosition = info.Position;
        _detectedCollider.transform.localScale = info.Size;
        _detectedCollider.transform.localRotation = info.Rotation;
        _hitDetectionDuration = info.Duration;
        
        //コライダーを有効にする
        _detectedCollider.enabled = true;
        
        // ヒット判定の範囲を指定して範囲内の敵を検出
        if (_detectedCollider.isTrigger)
        {
            Vector3 boxCenter = _detectedCollider.transform.position;
            Vector3 boxSize = _detectedCollider.bounds.size;
            
            //コライダー内の全てのオブジェクトを取得
            Collider[] hitCollidrs = Physics.OverlapBox(boxCenter, boxSize, _detectedCollider.transform.rotation);

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
                    if (_hitDetectionCoroutine != null)
                    {
                        StopCoroutine(_hitDetectionCoroutine);
                    }
                    _hitDetectionCoroutine = StartCoroutine(HitDetectionCooldown());
                }
            }
            
            //衝突した敵がいなかった場合、ここで判定持続時間を待って判定をリセットする
            if (!_isHitDetected)
            {
                if (_hitDetectionCoroutine != null)
                {
                    StopCoroutine(_hitDetectionCoroutine);
                    _detectedCollider.enabled = false; //無効化
                    _isHitDetected = false;
                }
                _hitDetectionCoroutine = StartCoroutine(HitDetectionCooldown());
            }
        }
    }
    
    /// <summary>
    /// 既にヒットしていたら以降の処理は行わない場合の、判定を行うメソッド
    /// </summary>
    public void DetectHitOnce(HitDetectionInfo info)
    {
        if(_isHitDetected) return; //既にヒットしていたら以降の処理は行わない
        DetectHit(info);
    }

    /// <summary>
    /// ヒットしたか取得する
    /// </summary>
    public bool IsHit()
    {
        return _isHitDetected;
    }
    
    /// <summary>
    /// ヒット判定の持続時間終了を待ち、ヒット判定をリセットする
    /// </summary>
    private IEnumerator HitDetectionCooldown()
    {
        yield return new WaitForSeconds(_hitDetectionDuration);
        _detectedCollider.enabled = false; //無効化
        _isHitDetected = false;
        _hitDetectionCoroutine = null;
    }

    /// <summary>
    /// コルーチンをリセットする
    /// </summary>
    public void DetectReset()
    {
        if (_hitDetectionCoroutine != null)
        {
            StopCoroutine(_hitDetectionCoroutine); //コルーチンの処理が行われていたら処理をやめる
            _detectedCollider.enabled = false; //そのうえでコライダーを無効にする
            _isHitDetected = false;
        }
    }
}