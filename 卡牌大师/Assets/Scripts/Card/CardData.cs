using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "CardData")]
public class CardData : ScriptableObject
{
    public Sprite illustration;
    public string cardName;
    public string cardDescription;
    public int actionCost;

    public int attackPower;
    public int healPower;

    [Header("对敌状态效果（施加到目标）")]
    public List<CardStatusEffect> statusEffects = new List<CardStatusEffect>();

    [Header("对己状态效果（施加给自己）")]
    public List<CardStatusEffect> selfStatusEffects = new List<CardStatusEffect>();
}
