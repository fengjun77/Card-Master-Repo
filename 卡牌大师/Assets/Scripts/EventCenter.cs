using System;

public static class EventCenter
{
    // 卡牌使用事件
    public static event Action<CardData> OnCardPlayed;
    public static void CardPlayedEvent(CardData cardData)
    {
        OnCardPlayed?.Invoke(cardData);
    }

    // 敌方受伤事件（敌人受击反应）
    public static event Action<CardData> OnEnemyHit;
    public static void EnemyHitEvent(CardData cardData)
    {
        OnEnemyHit?.Invoke(cardData);
    }

    // 敌方死亡事件（其他系统监听）
    public static event Action OnEnemyDefeated;
    public static void EnemyDefeatedEvent()
    {
        OnEnemyDefeated?.Invoke();
    }

    // 卡牌效果结算完成事件
    public static event Action OnCardEffectCompleted;
    public static void CardEffectCompletedEvent()
    {
        OnCardEffectCompleted?.Invoke();
    }

    //行动点数更新事件
    public static event Action<int> OnPointsChanged;
    public static void PointsChangedEvent(int amount)
    {
        OnPointsChanged?.Invoke(amount);
    }
}
