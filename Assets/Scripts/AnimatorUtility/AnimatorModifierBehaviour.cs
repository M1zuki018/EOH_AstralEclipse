using UnityEngine;

/// <summary>
/// Animatorのステートにキーを登録します
/// </summary>
[SharedBetweenAnimators]
public class AnimatorModifierBehaviour : StateMachineBehaviour
{
    [SerializeField] private PropertyName _key; //登録するキー
    [SerializeField, RangeArea(0,1)] private Vector2 _range = new(0, 1); //キーがアクティブになる範囲
    public string KeyName => _key.ToString().Split(':')[0]; //キー情報の文字列表現。デバッグ用途
    public PropertyName Key => _key; //キー情報

    /// <summary>
    /// 指定されたステートが範囲内にある場合はtrueを返す
    /// </summary>
    public bool IsInRange(in AnimatorStateInfo stateInfo)
    {
        if (stateInfo is { loop: false, normalizedTime: > 1 })
            return false;

        var normalizeTime = stateInfo.normalizedTime % 1;
        var inRange = normalizeTime > _range.x && normalizeTime < _range.y;

        return inRange;
    }
}