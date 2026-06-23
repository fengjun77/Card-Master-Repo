using System;

/// <summary>运行时状态效果实例</summary>
public class StatusEffect
{
    public StatusType type;
    public int amount;          // 效果数值（护甲值、每跳伤害等）
    public int remainingTurns;  // 剩余回合数（-1 = 永久，如护甲）

    //是否是实时效果
    public bool IsExpired => remainingTurns == 0;
    //是否是永久效果
    public bool IsPermanent => remainingTurns < 0;
}
