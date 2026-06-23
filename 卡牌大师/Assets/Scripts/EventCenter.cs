using System;

public static class EventCenter
{
    //卡牌使用事件
    public static event Action<CardData> OnCardPlayed;
    public static void CardPlayedEvent(CardData cardData)
    {
        OnCardPlayed?.Invoke(cardData);
    }

    //怪物受伤事件
    public static event Action<CardData> OnBossHit;
    public static void BossHitEvent(CardData cardData)
    {
        OnBossHit?.Invoke(cardData);
    }

    //怪物死亡事件
    public static event Action OnBossDead;
    public static void BossDeadEvent()
    {
        OnBossDead?.Invoke();
    }
}
