using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject playerSprite;
    [SerializeField] private GameObject healEffect;
    [SerializeField] private GameObject effectRoot;
    private Animator anim;
    private Health health;
    private StatusEffectManager statusManager;
    private Vector3 originalPos;

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        health = GetComponent<Health>();
        statusManager = GetComponent<StatusEffectManager>();
    }

    void Start()
    {
        originalPos = transform.position;
    }

    void OnEnable()
    {
        EventCenter.OnCardPlayed += HandleCardPlayed;
    }

    void OnDisable()
    {
        EventCenter.OnCardPlayed -= HandleCardPlayed;
    }

    private void HandleCardPlayed(CardData cardData)
    {
        Debug.Log("卡牌被打出了");
        StartCoroutine(HandleCardEffects(cardData));
    }

    private IEnumerator HandleCardEffects(CardData cardData)
    {
        // ───── 对己效果 ─────

        // 治疗
        if (cardData.healPower > 0)
            yield return StartCoroutine(
                CombatEffectHelper.HealEffect(health, cardData.healPower, healEffect, effectRoot));

        // 对己状态（护甲、回血等）
        if (cardData.selfStatusEffects != null && cardData.selfStatusEffects.Count > 0)
            statusManager?.ApplyEffects(cardData.selfStatusEffects);

        // ───── 对敌效果 ─────

        bool hasAttack = cardData.attackPower > 0;
        bool hasTargetStatus = cardData.statusEffects != null && cardData.statusEffects.Count > 0;

        if (hasAttack)
        {
            yield return StartCoroutine(
                CombatEffectHelper.AttackAnimation(
                    playerSprite, originalPos, new Vector3(7, 0, 0),
                    anim, "Attack_01",
                    () => EventCenter.EnemyHitEvent(cardData)));
        }
        else if (hasTargetStatus)
        {
            // 纯状态卡牌，无攻击动画，直接命中敌人
            EventCenter.EnemyHitEvent(cardData);
        }

        // 所有效果完成
        EventCenter.CardEffectCompletedEvent();
    }
}
