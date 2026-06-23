using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    [SerializeField] private Deck deck;
    [SerializeField] private Transform[] cardSlots;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private int startingHandSize = 2;
    private List<Card> cardsInHand = new List<Card>();

    [SerializeField] private DiscardPile discardPile;
    [SerializeField] private ActionPoints actionPoints;

    void Start()
    {
        // 初始发牌不消耗行动点数
        for (int i = 0; i < startingHandSize; i++)
        {
            DrawCardInternal();
        }
    }

    // ─── 抽牌 ───

    /// <summary>玩家点击牌堆抽牌（消耗 1 行动点）</summary>
    public void DrawNextCard()
    {
        if (actionPoints != null && !actionPoints.CanAfford(1))
        {
            Debug.Log($"行动点数不足，无法抽牌（当前 {actionPoints.CurrentPoints}）");
            return;
        }

        if (!DrawCardInternal())
            return;

        actionPoints?.TrySpend(1);
    }

    /// <summary>内部抽牌逻辑（无消耗），成功返回 true</summary>
    private bool DrawCardInternal()
    {
        if (cardSlots == null || cardsInHand.Count >= cardSlots.Length)
            return false;

        CardData cardData = deck.DrawCard();
        if (cardData == null) return false;

        int slotIndex = cardsInHand.Count;
        GameObject newCard = Instantiate(cardPrefab, cardSlots[slotIndex].position, Quaternion.identity);
        Card card = newCard.GetComponent<Card>();
        card.LoadCardData(cardData);
        card.SetPlayerHand(this);

        cardsInHand.Add(card);
        cardsInHand[slotIndex].transform.SetParent(cardSlots[slotIndex]);
        return true;
    }

    // ─── 出牌 ───

    public void PlayCard(Card card)
    {
        if (TurnSystem.Instance != null && !TurnSystem.Instance.CanPlayCard)
            return;

        CardData cardData = card.GetCardData();

        // 检查行动点数
        if (actionPoints != null && !actionPoints.CanAfford(cardData.actionCost))
        {
            Debug.Log($"行动点数不足（需要 {cardData.actionCost}，当前 {actionPoints.CurrentPoints}）");
            return;
        }
        actionPoints?.TrySpend(cardData.actionCost);

        cardsInHand.Remove(card);
        discardPile.DiscardCard(cardData);
        Destroy(card.gameObject);
        RepositionCards();

        EventCenter.CardPlayedEvent(cardData);
    }

    // ─── 辅助 ───

    public List<Card> GetCardsInHand() => cardsInHand;

    private void RepositionCards()
    {
        for (int i = 0; i < cardsInHand.Count; i++)
            cardsInHand[i].transform.SetParent(null);

        for (int i = 0; i < cardsInHand.Count; i++)
        {
            cardsInHand[i].transform.SetParent(cardSlots[i]);
            cardsInHand[i].transform.position = cardSlots[i].position;
            cardsInHand[i].UpdateOriginalPos();
        }
    }
}
