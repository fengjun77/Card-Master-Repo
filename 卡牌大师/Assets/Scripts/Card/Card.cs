using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class Card : MonoBehaviour
{
    [SerializeField] private SpriteRenderer illustration;
    [SerializeField] private TextMeshPro cardName;
    [SerializeField] private TextMeshPro actionCost;
    [SerializeField] private TextMeshPro cardDescription;
    private BoxCollider2D col;

    private Vector3 originalScale; //原始缩放
    private Vector3 originalPos; //原始坐标

    private SortingGroup sg;
    private int originalSortingOrder;
    //是否在被拖拽(静态保证只有一个在被拖拽)
    private static bool isBeingDragger;
    //当前卡牌信息
    private CardData currentCardData;
    //手牌用于出牌
    [SerializeField] private PlayerHand playerHand;
    //是否在出牌区
    public bool IsInPlayZone { get; set; }

    void Awake()
    {
        sg = GetComponent<SortingGroup>();
        col = GetComponent<BoxCollider2D>();
    }

    void Start()
    {
        originalScale = transform.localScale;
        originalPos = transform.position;
        originalSortingOrder = sg.sortingOrder;
    }

    /// <summary>
    /// 加载卡牌数据
    /// </summary>
    public void LoadCardData(CardData cardData)
    {
        currentCardData = cardData;
        illustration.sprite = cardData.illustration;
        cardName.text = cardData.cardName;
        actionCost.text = cardData.actionCost.ToString();
        cardDescription.text = cardData.cardDescription;
    }

    void OnMouseEnter()
    {
        if(isBeingDragger)
            return;

        transform.localScale = originalScale * 1.6f;
        transform.position += new Vector3(0,1.5f,0);
        sg.sortingOrder = originalSortingOrder + 1;
    }

    void OnMouseExit()
    {
        if(isBeingDragger)
            return;
        transform.localScale = originalScale;
        transform.position = originalPos;
        sg.sortingOrder = originalSortingOrder;
    }

    void OnMouseDrag()
    {
        isBeingDragger = true;
        gameObject.transform.position = GetMousePosition();
    }

    void OnMouseUp()
    {
        isBeingDragger = false;

        if (IsInPlayZone)
        {
            // 由 PlayerHand.PlayCard 内部检查回合状态
            playerHand.PlayCard(this);
            return;
        }

        transform.localScale = originalScale;
        transform.position = originalPos;
        sg.sortingOrder = originalSortingOrder;
    }

    void OnMouseDown()
    {
        isBeingDragger = false;
        transform.localScale = originalScale;
        transform.position = originalPos;
        sg.sortingOrder = originalSortingOrder;
    }

    private Vector3 GetMousePosition()
    {
        Vector3 mousePosition = Mouse.current.position.ReadValue();
        mousePosition.z = transform.position.z - Camera.main.transform.position.z;
        return Camera.main.ScreenToWorldPoint(mousePosition);
    }

    public CardData GetCardData() => currentCardData;

    private void OnDestroy()
    {
        isBeingDragger = false;
    }

    public void SetPlayerHand(PlayerHand hand)
    {
        playerHand = hand;
    }

    public void UpdateOriginalPos()
    {
        originalPos = transform.position;
    }

    /// <summary>
    /// 控制卡牌交互功能
    /// </summary>
    public void SetInteractable(bool interactable)
    {
        col.enabled = interactable;
    }
}
