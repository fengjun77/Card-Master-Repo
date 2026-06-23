using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DiscardPile : MonoBehaviour
{
    [SerializeField] private List<CardData> discardPile = new List<CardData>();

    [SerializeField] private GameObject cardPrefab;

    /// <summary>
    /// 丢弃卡片
    /// </summary>
    /// <param name="cardData"></param>
    public void DiscardCard(CardData cardData)
    {
        if(cardData == null) return;
        discardPile.Add(cardData);
        GameObject discardedCard = Instantiate(cardPrefab,transform);

        SortingGroup sortingGroup = discardedCard.GetComponent<SortingGroup>();
        sortingGroup.sortingOrder = discardPile.Count - 1;

        Card cardComponent = discardedCard.GetComponent<Card>();
        cardComponent.LoadCardData(cardData);
        cardComponent.SetInteractable(false);
        
        discardedCard.transform.SetParent(transform);

        discardedCard.transform.localPosition = new Vector3(0f, (discardPile.Count - 1) * -0.1f, 0f);
    }
}
