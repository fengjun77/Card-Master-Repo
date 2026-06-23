using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Health health;
    private Animator anim;

    void Awake()
    {
        health =  GetComponent<Health>();
        anim = GetComponentInChildren<Animator>();
    }

    void OnEnable()
    {
        EventCenter.OnBossHit += HandleHit;
        EventCenter.OnBossDead += Dead;
    }

    void OnDisable()
    {
        EventCenter.OnBossHit -= HandleHit;
        EventCenter.OnBossDead -= Dead;
    }

    private void HandleHit(CardData cardData)
    {
        health.TakeDamage(cardData.attackPower);
    }

    private void Dead()
    {
        anim.Play("Dead");
        Destroy(gameObject, 3f);
    }
}
