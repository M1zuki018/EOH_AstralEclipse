using System.Threading;
using Cysharp.Threading.Tasks;
using PlayerSystem.Input;
using PlayerSystem.State;
using UnityEngine;

/// <summary>
/// アニメーションイベントを補助するクラス
/// </summary>
public class CombatAnimationHandler : ViewBase
{
    [SerializeField] private Animator _animator;
    [SerializeField, Comment("当たり判定制御クラス")] private AttackHitDetector _hitDetector;
    [SerializeField, Comment("体の方向を敵に向けるクラス")] private AdjustDirection _adjustDirection;
    [SerializeField, Comment("エフェクト")] private EffectPool _effectPool;
    [SerializeField] private PlayerInputManager _inputManager;
    
    private InputBuffer _inputBuffer;
    private Vector3 _initialPosition;
    private bool _isAttacking; // 攻撃中か
    private Transform _target; // 敵
    private CancellationTokenSource _cts; // キャンセルトークン
    private PlayerBlackBoard _bb;
    private PlayerStateMachine _stateMachine;
    private PlayerActionHandler _playerActionHandler;

    public override UniTask OnStart()
    {
        PlayerInputProcessor processor = _inputManager.IPlayerInputReceiver as PlayerInputProcessor;
        _inputBuffer = processor.InputBuffer;
        PlayerBrain brain = _animator.GetComponent<PlayerBrain>();
        _bb = brain.BB;
        _stateMachine = brain.StateMachine;
        _playerActionHandler = _animator.GetComponent<PlayerController>().PlayerActionHandler;
        
        return base.OnStart(); 
    }
    
    /// <summary>
    /// 攻撃開始時の処理
    /// </summary>
    public void StartAttacking()
    {
        _target = _adjustDirection.Target;
        _isAttacking = true;
        _cts = new CancellationTokenSource();

        if (_target != null)
        {
            _adjustDirection.AdjustDirectionToTarget(); // 体をターゲットの向きに合わせる
        }
    }
    
    /// <summary>
    /// 攻撃処理を中断したい時に呼ぶ
    /// </summary>
    public void CancelAttack()
    {
        if (_isAttacking && _cts != null)
        {
            _cts.Cancel();
            Debug.Log("攻撃がキャンセルされました");
        }
    }

    /// <summary>
    /// InputBufferにCommandがあるか確認する
    /// </summary>
    public void Check()
    {
        // 最優先・ジャンプ。地面についている場合だけジャンプ入力を受け付ける
        if (_inputBuffer.GetBufferedInput(InputNameEnum.Jump) && _bb.IsGrounded)
        {
            _playerActionHandler.Jump();
            return;
        }
        
        // 二番・刀投げアクション
        if (_inputBuffer.GetBufferedInput(InputNameEnum.Action))
        {
            // 現在と逆のbool値をセットする
            // 手放していなかったら手放す＝true / 手放していたら回収する＝false
            if (!_animator.GetBool("IsThrow"))
            {
                _animator.SetBool("IsThrow", true); 
                _playerActionHandler.ThrowWeapon();
            }
            else
            {
                _animator.SetBool("IsThrow", false);
                _playerActionHandler.RecastWeapon();
            }
            
            return;
        }
        
        // 三番・攻撃
        if(_inputBuffer.GetBufferedInput(InputNameEnum.Attack))
        {
            _animator.SetTrigger("Attack");
        }
    }

    /// <summary>Locoモーション再生時に武器を手放していたら手元に戻す</summary>
    public void RecastWeapon()
    {
        if (_bb.IsThrown)
        {
            _animator.SetBool("IsThrow", false);
            _playerActionHandler.RecastWeapon();
        }
    }
    
    #region アニメーション

    /// <summary>ルートモーションを適用</summary>
    public void OnRootMotionEnable() => _animator.applyRootMotion = true;

    /// <summary>ルートモーションを解除</summary>
    public void OnRootMotionDisable() => _animator.applyRootMotion = false;
    
    /// <summary>アニメーション速度を変更</summary>
    public void OnSetAttackSpeed(float speed) => _animator.SetFloat("AttackSpeed", speed);

    /// <summary>体を敵の方向に向ける</summary>
    public void OnAdjustDirectionEnable(float correctionAngle) => _adjustDirection.AdjustDirectionToTarget(correctionAngle);
    
    /// <summary>すぐに体を敵の方向に向ける</summary>
    public void OnAdjustDirectionToTargetEarly() => _adjustDirection.AdjustDirectionToTargetEarly();
    
    /// <summary>初期位置の座標をセットする</summary>
    public void SetInitialPosition() => _initialPosition = transform.position;
    
    /// <summary>位置を初期化する</summary>
    public void ResetPosition() => transform.position = _initialPosition;
    
    #endregion

    #region SE

    /// <summary>SEを再生する</summary>
    public void PlaySE(int index) => AudioManager.Instance?.PlaySE(index);

    #endregion

    #region 当たり判定
    
    /// <summary>当たり判定を発生させる</summary>
    public void OnHitDetect(HitDetectionInfo info) => _hitDetector.DetectHit(info);

    #endregion
    
    #region カメラエフェクト

    /// <summary>ダッシュエフェクト</summary>
    public void DashEffect() => CameraManager.Instance?.DashEffect();
    
    /// <summary>回転斬りのエフェクト</summary>
    public void TurnEffect() => CameraManager.Instance?.TurnEffect();
    
    /// <summary>効果を元に戻す</summary>
    public void EndDashEffect() => CameraManager.Instance?.EndDashEffect();
    
    /// <summary>ヒットストップ</summary>
    public void ApplyHitStop(float hitStop) => CameraManager.Instance?.ApplyHitStop(hitStop);

    #endregion

    #region エフェクト

    /// <summary>エフェクトを表示する</summary>
    public void EffectEnable(Vector3 pos, Quaternion rot) => _effectPool.GetEffect(pos, rot);

    #endregion

    #region フラグ管理

    /// <summary>Attackingフラグをfalseに戻す</summary>
    public void AttackEnd() => _bb.IsAttacking = false;

    /// <summary>ステートマシンを変更する</summary>
    public void AttackFinish()
    {
        _bb.AttackFinishedTrigger = true;
        _bb.IsAttacking = false;
    }

    #endregion
}
