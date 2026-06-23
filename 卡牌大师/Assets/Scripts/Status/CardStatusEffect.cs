using System;

/// <summary>卡牌配置中的状态效果数据（在 Inspector 中编辑）</summary>
[Serializable]
public class CardStatusEffect
{
    public StatusType type;
    public int amount;     // 数值
    public int duration;   // 持续回合数（-1 = 永久）
}
