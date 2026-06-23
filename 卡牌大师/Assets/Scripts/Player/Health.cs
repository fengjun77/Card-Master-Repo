using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int totalHealth = 10;

    private int currentHealth;

    /// <summary>血量归零时触发（由挂载方决定死亡逻辑）</summary>
    public event Action OnHealthDepleted;

    /// <summary>血量变化时触发 (current, max)，供 UI 订阅</summary>
    public event Action<int, int> OnHealthChanged;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => totalHealth;

    void Awake()
    {
        currentHealth = totalHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        OnHealthChanged?.Invoke(currentHealth, totalHealth);

        if (currentHealth <= 0)
        {
            OnHealthDepleted?.Invoke();
        }
    }

    public void Heal(int amount)
    {
        int health = currentHealth + amount;
        currentHealth = Math.Min(health, totalHealth);

        OnHealthChanged?.Invoke(currentHealth, totalHealth);
    }
}
