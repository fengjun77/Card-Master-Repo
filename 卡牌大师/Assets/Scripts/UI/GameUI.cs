using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [Header("玩家血量")]
    [SerializeField] private Health playerHealth;
    [SerializeField] private Slider playerHealthSlider;
    [SerializeField] private TextMeshProUGUI playerHealthText;

    [Header("敌人血量")]
    [SerializeField] private Health enemyHealth;
    [SerializeField] private Slider enemyHealthSlider;
    [SerializeField] private TextMeshProUGUI enemyHealthText;

    [Header("行动点数")]
    [SerializeField] private ActionPoints playerActionPoints;
    [SerializeField] private TextMeshProUGUI actionPointsText;

    [Header("状态效果")]
    [SerializeField] private StatusEffectManager playerStatusManager;
    [SerializeField] private StatusEffectManager enemyStatusManager;
    [SerializeField] private TextMeshProUGUI playerStatusText;
    [SerializeField] private TextMeshProUGUI enemyStatusText;

    void Awake()
    {
        UpdatePlayerHealth(playerHealth.MaxHealth,playerHealth.MaxHealth);
        UpdateEnemyHealth(enemyHealth.MaxHealth, enemyHealth.MaxHealth);
    }

    void OnEnable()
    {
        // ─── 玩家血量 ───
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += UpdatePlayerHealth;
            UpdatePlayerHealth(playerHealth.CurrentHealth, playerHealth.MaxHealth);
        }

        // ─── 敌人血量 ───
        if (enemyHealth != null)
        {
            enemyHealth.OnHealthChanged += UpdateEnemyHealth;
            UpdateEnemyHealth(enemyHealth.CurrentHealth, enemyHealth.MaxHealth);
        }

        // ─── 行动点数 ───
        if (playerActionPoints != null)
        {
            EventCenter.OnPointsChanged += UpdateActionPoints;
            UpdateActionPoints(playerActionPoints.CurrentPoints);
        }

        // ─── 状态效果 ───
        if (playerStatusManager != null)
        {
            playerStatusManager.OnEffectChanged += (_, _) => UpdatePlayerStatus();
            UpdatePlayerStatus();
        }
        if (enemyStatusManager != null)
        {
            enemyStatusManager.OnEffectChanged += (_, _) => UpdateEnemyStatus();
            UpdateEnemyStatus();
        }
    }

    void OnDisable()
    {
        if (playerHealth != null)
            playerHealth.OnHealthChanged -= UpdatePlayerHealth;
        if (enemyHealth != null)
            enemyHealth.OnHealthChanged -= UpdateEnemyHealth;
        if (playerActionPoints != null)
            EventCenter.OnPointsChanged -= UpdateActionPoints;
    }

    // ─── 血量 ───

    private void UpdatePlayerHealth(int current, int max)
    {
        if (playerHealthSlider != null)
            playerHealthSlider.value = max > 0 ? (float)current / max : 0f;
        if (playerHealthText != null)
            playerHealthText.text = $"{current}/{max}";
    }

    private void UpdateEnemyHealth(int current, int max)
    {
        if (enemyHealthSlider != null)
            enemyHealthSlider.value = max > 0 ? (float)current / max : 0f;
        if (enemyHealthText != null)
            enemyHealthText.text = $"{current}/{max}";
    }

    // ─── 行动点 ───

    private void UpdateActionPoints(int points)
    {
        if (actionPointsText != null)
            actionPointsText.text = $"行动点: {points}";
    }

    // ─── 状态效果 ───

    private void UpdatePlayerStatus()
    {
        if (playerStatusText != null)
            playerStatusText.text = BuildStatusString(playerStatusManager);
    }

    private void UpdateEnemyStatus()
    {
        if (enemyStatusText != null)
            enemyStatusText.text = BuildStatusString(enemyStatusManager);
    }

    private string BuildStatusString(StatusEffectManager manager)
    {
        if (manager == null) return "";

        var effects = manager.GetAllEffects();
        if (effects.Count == 0) return "";

        string result = "";
        for (int i = 0; i < effects.Count; i++)
        {
            if (i > 0) result += "  ";

            var e = effects[i];
            result += e.type switch
            {
                StatusType.Armor          => $"护甲:{e.amount}",
                StatusType.Vulnerable     => $"易伤:+{e.amount}%",
                StatusType.DamageOverTime => $"DOT:{e.amount}",
                StatusType.Regeneration   => $"回血:{e.amount}",
                _                         => $"{e.type}:{e.amount}",
            };

            if (!e.IsPermanent)
                result += $"({e.remainingTurns}回合)";
        }
        return result;
    }
}
