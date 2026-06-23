using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    //牌组卡片信息列表
    [SerializeField] private List<CardData> drawPiles = new List<CardData>();

    [SerializeField] private GameObject cardBack;

    [SerializeField] private PlayerHand playerHand;

    void Start()
    {
        Shuffle();
        DeckDrawVisuals();
    }

    /// <summary>
    /// 从牌堆中抽取最上面一张牌
    /// </summary>
    /// <returns></returns>
    public CardData DrawCard()
    {
        if(drawPiles.Count > 0)
        {
            int topIndex = drawPiles.Count - 1;
            CardData card = drawPiles[topIndex];
            drawPiles.RemoveAt(topIndex);
            return card;
        }

        return null;
    }

    /// <summary>
    /// 实例化牌到牌堆中
    /// </summary>
    private void DeckDrawVisuals()
    {
        for(int i = 0; i < drawPiles.Count; i++)
        {
            GameObject newCardBack = Instantiate(cardBack, transform);
            newCardBack.transform.localPosition = new Vector3(0f, -i * 0.1f, 0f);
        }
    }

    /// <summary>
    /// 洗牌
    /// </summary>
    private void Shuffle()
    {
        for(int i = 0; i < drawPiles.Count; i++)
        {
            CardData card = drawPiles[i];
            int randomIndex = Random.Range(i, drawPiles.Count);
            drawPiles[i] = drawPiles[randomIndex];
            drawPiles[randomIndex] = card;
        }
    }

    private void OnMouseDown()
    {
        if(drawPiles.Count <= 0) return;
        playerHand.DrawNextCard();
    }
}
