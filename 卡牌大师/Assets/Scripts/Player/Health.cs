using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int totalHealth = 10;

    private int currentHealth;

    void Awake()
    {
        currentHealth = totalHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"当前目标血量为 {currentHealth}");

        if(currentHealth <= 0)
        {
            //触发死亡逻辑
            EventCenter.BossDeadEvent();
        }
    }

    public void Heal(int amount)
    {
        int health = currentHealth + amount;
        currentHealth = Math.Min(health, totalHealth);
    }
}
