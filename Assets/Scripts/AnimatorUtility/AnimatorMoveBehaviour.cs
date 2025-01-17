using UnityEngine;

/// <summary>
/// StateMachine側からAnimatorMoveControlを制御するためのコンポーネント
/// </summary>
public class AnimatorMoveBehaviour : StateMachineBehaviour
{
    /// <summary>
    /// AnimatorMoveControlの優先度が有効になる期間の設定
    /// </summary>
    [SerializeField, HideInInspector, Range(0, 1)] [Tooltip("AnimationMoveの優先度を上げる期間")]
    private float _start = 0, _end = 1;

    [HideInInspector, SerializeField] private bool _copyed = false;

    [SerializeField, RangeArea(0, 1)] private Vector2 _range;

    private void OnValidate()
    {
        if (_copyed == false)
        {
            _range.x = _start;
            _range.y = _end;
            _copyed = true;
        }
    }

    /// <summary>
    /// キャラクターの移動時に地面検出を考慮するかどうかを決定します
    /// </summary>
    [SerializeField]
    [Tooltip(
        "AnimationMoveでキャラクターを移動させる際に地面検出を使用するかどうかを決定します。例えば、はしごのように上下移動する場合はFalseに設定します。")]
    private bool _useGroundNormal = true;

    public bool UseGroundNormal => _useGroundNormal;

    /// <summary>
    /// キャラクターの移動をControlの代わりにWarpとして処理します
    /// </summary>
    [SerializeField] private bool _isFixedPosition = false;

    public bool IsFixedPosition => _isFixedPosition;

    public bool IsInRange(AnimatorStateInfo info)
    {
        var normalizedTime = info.loop ? info.normalizedTime % 1 : info.normalizedTime;

        return _range.x < normalizedTime && _range.y > normalizedTime;
    }
}