using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "CardData")]
public class CardData : ScriptableObject
{
    public Sprite illustration; //卡片图标
    public string cardName; //卡片名称
    public string cardDescription; //卡片描述
    public int actionCost; //行动点数

    public int attackPower;
    public int healPower;
}
