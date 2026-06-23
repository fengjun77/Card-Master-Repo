using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject playerSprite;
    [SerializeField] private GameObject healEffect;
    [SerializeField] private GameObject effectRoot;
    private Animator anim;
    private Health health;
    private Vector3 originalPos;

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        health = GetComponent<Health>();
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
        if(cardData.attackPower > 0)
            Attack(cardData);
        if(cardData.healPower > 0)
            Heal(cardData);
    }

    /// <summary>
    /// 攻击逻辑
    /// </summary>
    /// <param name="cardData"></param>
    private void Attack(CardData cardData)
    {
        StartCoroutine(PlayerAttackAnimation(cardData));
    }

    /// <summary>
    /// 治疗逻辑
    /// </summary>
    /// <param name="cardData"></param>
    private void Heal(CardData cardData)
    {
        health.Heal(cardData.healPower);
        GameObject effect = Instantiate(healEffect, effectRoot.transform);
        
        Destroy(effect, 1);
    }

    private IEnumerator PlayerAttackAnimation(CardData cardData)
    {
        Vector3 targetPos = originalPos + new Vector3(7,0,0);

        float duration = 1f;
        float time = 0;

        while(time < duration)
        {
            playerSprite.transform.position = Vector3.Lerp(originalPos, targetPos, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        anim.Play("Attack_01");
        yield return new WaitForSeconds(.5f);
        EventCenter.BossHitEvent(cardData);

        duration = .6f;
        time = 0;
        while(time < duration)
        {
            playerSprite.transform.position = Vector3.Lerp(targetPos, originalPos, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        playerSprite.transform.position = originalPos;

        yield return null;
    }
}
