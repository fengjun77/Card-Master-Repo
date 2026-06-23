/// <summary>状态效果类型（新增效果只需在此加一项）</summary>
public enum StatusType
{
    Armor,          // 护甲：吸收伤害，持续到消耗完
    Vulnerable,     // 易伤：每层增伤 50%
    DamageOverTime, // 持续扣血：每回合开始造成伤害
    Regeneration,   // 持续回血：每回合开始恢复血量
}
