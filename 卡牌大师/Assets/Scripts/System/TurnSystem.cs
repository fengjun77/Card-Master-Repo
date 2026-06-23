using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum TurnPhase
{
    PlayerTurn,
    PlayerBusy,
    EnemyTurn,
}

public class TurnSystem : MonoBehaviour
{
    public static TurnSystem Instance { get; private set; }

    [Header("引用")]
    [SerializeField] private PlayerHand playerHand;
    [SerializeField] private GameObject endTurnButton;
    [SerializeField] private Enemy enemy;
    [SerializeField] private ActionPoints playerActionPoints;

    [Header("状态效果")]
    [SerializeField] private StatusEffectManager playerStatusManager;
    [SerializeField] private StatusEffectManager enemyStatusManager;

    private TurnPhase currentPhase;

    public bool CanPlayCard => currentPhase == TurnPhase.PlayerTurn;
    public TurnPhase CurrentPhase => currentPhase;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void OnEnable()
    {
        EventCenter.OnCardPlayed += OnCardPlayed;
        EventCenter.OnCardEffectCompleted += OnCardEffectCompleted;
    }

    void OnDisable()
    {
        EventCenter.OnCardPlayed -= OnCardPlayed;
        EventCenter.OnCardEffectCompleted -= OnCardEffectCompleted;
    }

    void Start()
    {
        if (endTurnButton != null)
        {
            Button button = endTurnButton.GetComponent<Button>();
            if (button != null)
                button.onClick.AddListener(EndPlayerTurn);
        }

        StartPlayerTurn();
    }

    private void StartPlayerTurn()
    {
        // 行动点增长
        playerActionPoints?.OnTurnStart();

        // 玩家回合开始 → 结算玩家自身的 DOT / 回血
        playerStatusManager?.OnTurnStart();

        currentPhase = TurnPhase.PlayerTurn;
        SetCardsInteractable(true);
        SetEndTurnButtonActive(true);
        Debug.Log("=== 玩家回合 ===");
    }

    private void OnCardPlayed(CardData cardData)
    {
        currentPhase = TurnPhase.PlayerBusy;
        SetCardsInteractable(false);
        SetEndTurnButtonActive(false);
        Debug.Log("卡牌结算中...");
    }

    private void OnCardEffectCompleted()
    {
        Debug.Log("卡牌结算完成");
        currentPhase = TurnPhase.PlayerTurn;
        SetCardsInteractable(true);
        SetEndTurnButtonActive(true);
    }

    public void EndPlayerTurn()
    {
        if (currentPhase != TurnPhase.PlayerTurn) return;

        // 玩家回合结束 → 减少 buff / debuff 持续时间
        playerStatusManager?.OnTurnEnd();

        currentPhase = TurnPhase.EnemyTurn;
        SetCardsInteractable(false);
        SetEndTurnButtonActive(false);
        Debug.Log("=== 敌方回合 ===");

        StartCoroutine(EnemyTurnRoutine());
    }

    private IEnumerator EnemyTurnRoutine()
    {
        // 敌方回合开始 → 结算敌人自身的 DOT / 回血
        enemyStatusManager?.OnTurnStart();

        if (enemy != null)
            yield return StartCoroutine(enemy.ExecuteNextAction());

        // 敌方回合结束 → 减少 buff / debuff 持续时间
        enemyStatusManager?.OnTurnEnd();

        Debug.Log("敌方回合结束，回到玩家回合");
        StartPlayerTurn();
    }

    private void SetCardsInteractable(bool interactable)
    {
        if (playerHand == null) return;
        var cards = playerHand.GetCardsInHand();
        foreach (var card in cards)
        {
            card.SetInteractable(interactable);
        }
    }

    private void SetEndTurnButtonActive(bool active)
    {
        if (endTurnButton != null)
            endTurnButton.SetActive(active);
    }
}
