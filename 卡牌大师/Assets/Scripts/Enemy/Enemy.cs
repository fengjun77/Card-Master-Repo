using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("行动配置")]
    [SerializeField] private CardData[] actionList;
    private int currentActionIndex;

    [Header("引用")]
    [SerializeField] private Health playerHealth;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private GameObject enemySprite;
    [SerializeField] private GameObject healEffect;
    [SerializeField] private GameObject effectRoot;

    [Header("状态效果")]
    [SerializeField] private StatusEffectManager statusManager;
    [SerializeField] private StatusEffectManager playerStatusManager;

    private Health health;
    private Animator anim;
    private Vector3 originalPos;

    void Awake()
    {
        health = GetComponent<Health>();
        anim = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        originalPos = enemySprite != null ? enemySprite.transform.position : transform.position;
    }

    void OnEnable()
    {
        health.OnHealthDepleted += OnDefeated;
        EventCenter.OnEnemyHit += HandleHit;
    }

    void OnDisable()
    {
        health.OnHealthDepleted -= OnDefeated;
        EventCenter.OnEnemyHit -= HandleHit;
    }

    /// <summary>执行下一个行动（由 TurnSystem 每回合调用一次）</summary>
    public IEnumerator ExecuteNextAction()
    {
        if (actionList == null || actionList.Length == 0)
        {
            Debug.LogWarning("怪物行动列表为空");
            yield break;
        }

        CardData action = actionList[currentActionIndex];
        currentActionIndex = (currentActionIndex + 1) % actionList.Length;

        Debug.Log($"怪物执行行动: {action.cardName}");

        bool hasAttack = action.attackPower > 0;
        bool hasTargetStatus = action.statusEffects != null && action.statusEffects.Count > 0;

        // ───── 对敌效果 ─────

        if (hasAttack)
        {
            // 攻击动画，命中时造成伤害 + 施加对敌状态
            Vector3 direction = Vector3.zero;
            if (playerTransform != null)
            {
                float dir = playerTransform.position.x > transform.position.x ? 1f : -1f;
                direction = new Vector3(dir * 5f, 0, 0);
            }

            GameObject sprite = enemySprite != null ? enemySprite : gameObject;
            yield return StartCoroutine(
                CombatEffectHelper.AttackAnimation(
                    sprite, originalPos, direction,
                    anim, "Attack_01",
                    () =>
                    {
                        int damage = playerStatusManager != null
                            ? playerStatusManager.ModifyIncomingDamage(action.attackPower)
                            : action.attackPower;

                        playerHealth?.TakeDamage(damage);
                        playerStatusManager?.ApplyCardEffects(action);
                    }));
        }
        else if (hasTargetStatus)
        {
            // 纯状态卡牌（无攻击力），直接施加到玩家
            playerStatusManager?.ApplyEffects(action.statusEffects);
        }

        // ───── 对己效果 ─────

        if (action.healPower > 0)
        {
            yield return StartCoroutine(
                CombatEffectHelper.HealEffect(health, action.healPower, healEffect, effectRoot));
        }

        if (action.selfStatusEffects != null && action.selfStatusEffects.Count > 0)
        {
            statusManager?.ApplyEffects(action.selfStatusEffects);
        }
    }

    /// <summary>被玩家卡牌击中（伤害 + 状态效果）</summary>
    private void HandleHit(CardData cardData)
    {
        // 护甲/易伤修正
        int finalDamage = statusManager != null
            ? statusManager.ModifyIncomingDamage(cardData.attackPower)
            : cardData.attackPower;

        health.TakeDamage(finalDamage);

        // 应用卡牌上的对敌状态效果到敌人自身
        statusManager?.ApplyCardEffects(cardData);
    }

    private void OnDefeated()
    {
        EventCenter.EnemyDefeatedEvent();
        anim.Play("Dead");
        Destroy(gameObject, 3f);
    }
}
