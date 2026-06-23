using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    [SerializeField] private Deck deck;
    [SerializeField] private Transform[] cardSlots;
    [SerializeField] private GameObject cardPrefab;
    //起始卡牌数量
    [SerializeField] private int startingHandSize = 2;
    //在手上的卡牌
    private List<Card> cardsInHand = new List<Card>();

    [SerializeField] private DiscardPile discardPile;

    void Start()
    {
        for(int i = 0; i < startingHandSize; i++)
        {
            DrawNextCard();
        }
    }

    /// <summary>
    /// 抽下一张牌
    /// </summary>
    public void DrawNextCard()
    {
        //如果手中没有空区卡牌槽位
        if(cardSlots == null || cardsInHand.Count >= cardSlots.Length)
            return;
    
        //获取下一张牌的数�?
        CardData cardData = deck.DrawCard();
        if(cardData == null) return;

        int slotIndex = cardsInHand.Count;
        //在手牌区实例化出对于卡牌对象
        GameObject newCard = Instantiate(cardPrefab, cardSlots[slotIndex].position, Quaternion.identity);
        Card card = newCard.GetComponent<Card>();
        card.LoadCardData(cardData);
        card.SetPlayerHand(this);

        cardsInHand.Add(card);
        cardsInHand[slotIndex].transform.SetParent(cardSlots[slotIndex]);
    }

    /// <summary>
    /// 使用卡牌
    /// </summary>
    public void PlayCard(Card card)
    {
        CardData cardData = card.GetCardData();
        cardsInHand.Remove(card);
        discardPile.DiscardCard(cardData);

        Destroy(card.gameObject);
        RepositionCards();

        //调用卡牌使用事件
        EventCenter.CardPlayedEvent(cardData);
    }

    /// <summary>
    /// 更新手牌位置
    /// </summary>
    private void RepositionCards()
    {
        //解除所有手牌父子关�?
        for(int i = 0; i < cardsInHand.Count; i++)
        {
            cardsInHand[i].transform.SetParent(null);
        }

        for(int i = 0; i < cardsInHand.Count; i++)
        {
            cardsInHand[i].transform.SetParent(cardSlots[i]);
            cardsInHand[i].transform.position = cardSlots[i].position;
            cardsInHand[i].UpdateOriginalPos();
        }
    }
}

