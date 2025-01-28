using UniRx;
using UnityEngine;

/// <summary>
/// 通常攻撃1段目の補正
/// </summary>
public class NormalAttack_First : MonoBehaviour
{
    [SerializeField] private float _approachSpeed = 5f; //突進速度
    [SerializeField] private float _attackDistance = 4f; //有効距離
    [SerializeField] private float _adjustDistance = 1.5f; //補正がかかる距離
    [SerializeField] private float _slashFream = 0.3f; //0~1
    private Transform _target; //ロックオン中の敵
    private Animator _animator; //Animator
    private bool _isAttacking = false;
    private float _distance;
    private CharacterController _cc;

    private void Start()
    {
        _cc = GetComponent<CharacterController>();
    }
    
    private void Update()
    {
        if (_isAttacking && _target != null)
        {
            HandleApproach();
        }
        
    }

    /// <summary>
    /// 攻撃開始時に呼び出される処理
    /// </summary>
    public void StartAttack(Transform target)
    {
        _target = target;
        _distance = Vector3.Distance(transform.position, _target.position); //敵との距離を計算
        
        Debug.Log(_distance);
        
        if (_distance > _adjustDistance && _distance < _attackDistance) //補正がかかる距離よりも遠く、かつ有効距離内にいる場合
        {
            //突進の処理を行う
            _isAttacking = true;
            Observable
                .EveryUpdate()
                .Where(_ => _distance < _adjustDistance)
                .Subscribe(_ => TriggerSlash())
                .AddTo(this);
        }
        else
        {
            TriggerSlash(); //斬撃モーションを再生する
        }
    }
    
    /// <summary>
    /// 突進処理
    /// </summary>
    private void HandleApproach()
    {
        Debug.Log("補正中：攻撃１段階");
        _distance = Vector3.Distance(transform.position, _target.position); //距離を更新
                    
        // 突進処理: プレイヤーを敵に近づける
        Vector3 direction = (_target.position - transform.position).normalized;
        _cc.Move(direction * _approachSpeed * Time.deltaTime);

        // 敵の方向を向く
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
    }
    
    /// <summary>
    /// 斬撃モーションに移る
    /// </summary>
    private void TriggerSlash()
    {
        _isAttacking = false;

    }
}
