using System.Collections;
using System.Collections.Generic;
using PlayerSystem.Fight;
using UnityEngine;
using UniRx;

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
    
    [Header("調整用：検出フレームの範囲")] 
    [Range(0, 1)] [ReadOnlyOnRuntime] [SerializeField] private float _frame;
    [SerializeField] private int _attackType;

    [Header("検出位置と有効範囲の設定")]
    [SerializeField] private Data[] _hitPositions =
    {
        new() //初期設定
        {
            Range = new Vector2(0, 1), //範囲
            Collisions = new[]
            {
                new CollisionData
                {
                    Range = new Vector2(0, 1), //範囲
                    Offset = Vector3.zero, //オフセット
                    Rotation = Vector3.zero, //回転
                    Scale = Vector3.one //スケール
                }
            }
        }
    };
    
    private Animator _animator;
    private readonly List<GameObject> _hitObjects = new();
    private readonly int _locoMotionHash= Animator.StringToHash("Base Layer.LocoMotion"); //避けたいステート
    private int _previousStateHash = -1;
    private bool _isAttacking; //このクラス内で使用する攻撃状態かどうかのフラグ
    private bool _isAttackMotion;
    
    private bool _isHitDetected = false; //ヒット検出フラグ
    private ICombat _combat;
    private float _hitDetectionDuration; //攻撃判定の持続時間
    private Collider[] _hitColliders;

    /// <summary>攻撃の種類</summary>
    public int AttackType
    {
        get => _attackType; 
        set => _attackType = value;
    }
    
    private void OnEnable()
    {
        _combat = GetComponent<ICombat>(); 
        _animator = GetComponentInParent<Animator>();
        _hitObjects.Clear(); //リストの中身をクリアする
    }

    public void DetectHit(Vector3 attackPosition, float attackRange)
    {
        if(_isHitDetected) return; //既にヒットしていたら以降の処理は行わない
        
        // ヒット判定の範囲を指定して範囲内の敵を検出
        _hitColliders = Physics.OverlapSphere(attackPosition, attackRange);

        // ヒットしたオブジェクトにダメージを与える
        foreach (var hitCollider in _hitColliders)
        {
            var target = hitCollider.GetComponent<IDamageable>();
            if (target != null)
            {
                _combat.DamageHandler.ApplyDamage(
                    target:target, //攻撃対象
                    baseDamage:_combat.BaseAttackPower, //攻撃力 
                    defense:0,  //相手の防御力
                    attacker:gameObject); //攻撃を加えるキャラクターのゲームオブジェクト
            }
        }

        _isHitDetected = true;
        StartCoroutine(HitDetectionCooldown());
    }
    
    private IEnumerator HitDetectionCooldown()
    {
        // ヒット判定が終了するまで待つ
        yield return new WaitForSeconds(_hitDetectionDuration);
        _isHitDetected = false;
    }
    
    /// <summary>
    /// 攻撃した時に呼び出す
    /// </summary>
    public List<IDamageable> PerformAttack() => DetectCollisions();

    /// <summary>
    /// 判定検出を実行する
    /// </summary>
    private List<IDamageable> DetectCollisions()
    {
        _isAttacking = true;
        _frame = 0; //初期化
        
        Observable
            .EveryUpdate()
            .TakeWhile(_ => _isAttacking) //攻撃中かつフレームが1未満の時、処理を行う
            .Subscribe(_ =>
            {
                _frame = SetFrameLength(); //攻撃モーション進行
                if (_frame >= 1f)
                {
                    _isAttacking = false;
                }
            }) 
            .AddTo(this);
        
        //処理開始時にリストをクリア
        _hitObjects.Clear();
        List<IDamageable> damageables = new();
        
        var data = _hitPositions[AttackType]; 
        if (!data.IsInRange(_frame)) return null; //効果フレームの範囲外の場合は処理をスキップ

        var activeCollisions = data.GetActiveCollisions(_frame); //有効なHitボックスを取得
        foreach (var col in activeCollisions)
        { 
            // 衝突検出を実行
            var count = CalculateCollisions(col, out var hitResults);
                
            // 範囲内で衝突検出を実行
            for (var i = 0; i < count; i++) 
            { 
                var hit = hitResults[i];
                var hitObject = hit.gameObject;

                //if (!IsValidTarget(hitObject) || _hitObjects.Contains(hitObject)) continue;
                    
                //コライダーを登録。Playerの場合臨戦状態を管理するコライダーが取得されるので、親オブジェクトを参照するようにする
                _hitObjects.Add(hitObject);
                if (hit.TryGetComponent(out IDamageable damageable) || hit.transform.parent.TryGetComponent(out damageable))
                {
                    damageables.Add(damageable);
                }
            }
        }

        return damageables;
    }

    private int CalculateCollisions(CollisionData col, out Collider[] hitResults)
    {
        hitResults = new Collider[50];
        var position = transform.position + transform.TransformVector(col.Offset);
        var rotation = transform.rotation * Quaternion.Euler(col.Rotation);
        return Physics.OverlapBoxNonAlloc(position, col.Scale * 0.5f, hitResults, rotation, _hitLayer);
    }

    /// <summary>
    /// ターゲットがHitTagに設定したタグを持っているか調べる
    /// </summary>
    private bool IsValidTarget(GameObject target)
    {
        foreach (var tag in _hitTags)
        {
            if (target.CompareTag(tag)) return true;
        }
        return _hitTags.Length == 0;
    }

    /// <summary>
    /// アニメーションクリップの時間を正規化する
    /// </summary>
    private float SetFrameLength()
    {
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        
        bool isInLocoMotion = stateInfo.fullPathHash == _locoMotionHash; //現在のモーションがLocoMotionか判定
       
        // 再生中のアニメーションステートが変更された場合
        if (_previousStateHash != stateInfo.fullPathHash && _isAttacking)
        {
            _previousStateHash = stateInfo.fullPathHash; // 現在の状態を保存
            
            if (isInLocoMotion)
            {
                _isAttackMotion = false;
                return 1f; // LocoMotionなら即終了扱いに
            }
            else
            {
                _isAttackMotion = true;
                return 0f; // 攻撃開始時はフレームをリセット
            }
        }

        if (_isAttackMotion)
        {
            return stateInfo.normalizedTime % 1; //再生時間を正規化
        }

        return 1f;
    }
    
    /// <summary>
    /// ギズモを表示する
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (_hitPositions == null || _hitPositions.Length <= _attackType) return;

        var data = _hitPositions[_attackType];
        if (!data.IsInRange(_frame)) return;

        var activeCollisions = data.GetActiveCollisions(_frame);
        foreach (var col in activeCollisions)
        {
            var position = transform.position + transform.TransformVector(col.Offset);
            var rotation = transform.rotation * Quaternion.Euler(col.Rotation);

            Gizmos.color = Color.yellow;
            Gizmos.matrix = Matrix4x4.TRS(position, rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, col.Scale);

            Gizmos.color = new Color(1, 1, 0, 0.2f);
            Gizmos.DrawCube(Vector3.zero, col.Scale);
        }
    }
}