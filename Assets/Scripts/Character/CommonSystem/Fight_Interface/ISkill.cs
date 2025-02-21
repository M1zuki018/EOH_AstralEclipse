/// <summary>
/// スキル処理のインターフェース
/// </summary>
public interface ISkill
{
    /// <summary>現在のTPとスキルの消費TPを確認し発動可能か返す</summary>
    bool CanUseSkill();
    /// <summary>スキルを使う</summary>
    void UseSkill();
}
