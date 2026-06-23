using System;
using UnityEngine;

public class ActionPoints : MonoBehaviour
{
    [SerializeField] private int startingPoints = 2;
    [SerializeField] private int perTurnGain = 1;

    private int currentPoints;

    public int CurrentPoints => currentPoints;

    void Awake()
    {
        currentPoints = startingPoints;
    }

    /// <summary>是否能支付</summary>
    public bool CanAfford(int cost) => currentPoints >= cost;

    /// <summary>尝试支付，成功返回 true 并扣点</summary>
    public bool TrySpend(int cost)
    {
        if (currentPoints < cost) return false;
        currentPoints -= cost;
        EventCenter.PointsChangedEvent(currentPoints);
        return true;
    }

    /// <summary>获得点数</summary>
    public void Gain(int amount)
    {
        currentPoints += amount;
        EventCenter.PointsChangedEvent(currentPoints);
    }

    /// <summary>玩家回合开始时获得本回合点数</summary>
    public void OnTurnStart()
    {
        Gain(perTurnGain);
    }
}
