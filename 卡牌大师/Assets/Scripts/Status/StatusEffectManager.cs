using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 状态效果管理器 —— 挂载到每个可受效果的实体（Player / Enemy）上
/// 扩展方法：在 StatusType 加新枚举 → 在 OnTurnStart / ModifyIncomingDamage 加对应逻辑
/// </summary>
public class StatusEffectManager : MonoBehaviour
{
    //当前buff
    private List<StatusEffect> effects = new List<StatusEffect>();
    private Health health;

    // ─── 事件（供 UI 订阅） ───
    public event Action<StatusType, int> OnEffectChanged;  // type, new amount
    public event Action<StatusType> OnEffectExpired;

    void Awake()
    {
        health = GetComponent<Health>();
    }

    // ─── 公开 API ───

    /// <summary>添加 / 叠加状态效果</summary>
    public void AddEffect(StatusType type, int amount, int duration)
    {
        if (amount <= 0) return;

        var existing = effects.Find(e => e.type == type);
        if (existing != null)
        {
            existing.amount += amount;
            // 永久效果不更新 duration，否则取最大值
            if (!existing.IsPermanent)
                existing.remainingTurns = duration < 0 ? -1 : Math.Max(existing.remainingTurns, duration);
        }
        else
        {
            effects.Add(new StatusEffect
            {
                type = type,
                amount = amount,
                remainingTurns = duration < 0 ? -1 : duration,
            });
        }

        OnEffectChanged?.Invoke(type, GetAmount(type));
    }

    /// <summary>移除指定类型的所有效果</summary>
    public void RemoveEffect(StatusType type)
    {
        int removed = effects.RemoveAll(e => e.type == type);
        if (removed > 0)
        {
            OnEffectChanged?.Invoke(type, 0);
            OnEffectExpired?.Invoke(type);
        }
    }

    /// <summary>获取指定类型的累计数值</summary>
    public int GetAmount(StatusType type)
    {
        var effect = effects.Find(e => e.type == type);
        return effect?.amount ?? 0;
    }

    /// <summary>获取指定类型的剩余回合数</summary>
    public int GetRemainingTurns(StatusType type)
    {
        var effect = effects.Find(e => e.type == type);
        return effect?.remainingTurns ?? 0;
    }

   public bool HasEffect(StatusType type) => GetAmount(type) > 0;

    /// <summary>应用 CardStatusEffect 列表到本实体</summary>
    public void ApplyEffects(List<CardStatusEffect> effects)
    {
        if (effects == null) return;
        foreach (var se in effects)
        {
            AddEffect(se.type, se.amount, se.duration);
        }
    }

    /// <summary>把卡牌配置的状态效果应用到本实体</summary>
    public void ApplyCardEffects(CardData cardData)
    {
        ApplyEffects(cardData.statusEffects);
    }

    /// <summary>获取所有活跃效果的只读快照（供 UI 遍历）</summary>
    public IReadOnlyList<StatusEffect> GetAllEffects() => effects.AsReadOnly();

    // ─── 回合生命周期 ───

    /// <summary>实体回合开始时触发（DOT、回血等）</summary>
    public void OnTurnStart()
    {
        for (int i = effects.Count - 1; i >= 0; i--)
        {
            var effect = effects[i];
            switch (effect.type)
            {
                case StatusType.DamageOverTime:
                    health?.TakeDamage(effect.amount);
                    Debug.Log($"持续扣血 {effect.amount} 点");
                    break;

                case StatusType.Regeneration:
                    health?.Heal(effect.amount);
                    Debug.Log($"持续回血 {effect.amount} 点");
                    break;
            }
        }
    }

    /// <summary>实体回合结束时触发（减少持续时间、清除过期效果）</summary>
    public void OnTurnEnd()
    {
        for (int i = effects.Count - 1; i >= 0; i--)
        {
            var effect = effects[i];

            // 永久效果不减回合
            if (effect.IsPermanent) continue;

            effect.remainingTurns--;
            if (effect.remainingTurns <= 0)
            {
                OnEffectExpired?.Invoke(effect.type);
                effects.RemoveAt(i);
                OnEffectChanged?.Invoke(effect.type, 0);
            }
        }
    }

    /// <summary>修改受到的伤害（护甲吸收 → 易伤加成）</summary>
    public int ModifyIncomingDamage(int rawDamage)
    {
        if (rawDamage <= 0) return 0;

        int remaining = rawDamage;

        // 1. 护甲吸收
        var armor = effects.Find(e => e.type == StatusType.Armor);
        if (armor != null && armor.amount > 0)
        {
            int absorbed = Mathf.Min(remaining, armor.amount);
            armor.amount -= absorbed;
            remaining -= absorbed;

            OnEffectChanged?.Invoke(StatusType.Armor, armor.amount);

            if (armor.amount <= 0)
            {
                effects.Remove(armor);
                OnEffectExpired?.Invoke(StatusType.Armor);
            }

            Debug.Log($"护甲吸收了 {absorbed} 点伤害，剩余 {remaining}");
        }

        // 2. 易伤加成（amount 直接作为百分比）
        var vuln = effects.Find(e => e.type == StatusType.Vulnerable);
        if (vuln != null && vuln.amount > 0)
        {
            int bonus = Mathf.RoundToInt(remaining * (vuln.amount / 100f));
            remaining += bonus;
            Debug.Log($"易伤增加 {bonus} 点伤害，共 {remaining}");
        }

        return remaining;
    }
}
