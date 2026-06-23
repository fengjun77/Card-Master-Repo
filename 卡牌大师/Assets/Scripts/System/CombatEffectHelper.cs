using System;
using System.Collections;
using UnityEngine;

public static class CombatEffectHelper
{
    /// <summary>
    /// 通用前冲攻击动画
    /// </summary>
    /// <param name="sprite">移动的精灵物体</param>
    /// <param name="startPos">起始位置</param>
    /// <param name="offset">移动方向偏移量</param>
    /// <param name="animator">动画控制器</param>
    /// <param name="animName">攻击动画名</param>
    /// <param name="onHitPoint">命中点回调</param>
    /// <param name="forwardDuration">前冲时长</param>
    /// <param name="animDelay">动画播放后的等待时间</param>
    /// <param name="returnDuration">返回时长</param>
    public static IEnumerator AttackAnimation(
        GameObject sprite,
        Vector3 startPos,
        Vector3 offset,
        Animator animator,
        string animName,
        Action onHitPoint,
        float forwardDuration = 1f,
        float animDelay = 0.5f,
        float returnDuration = 0.6f)
    {
        Vector3 targetPos = startPos + offset;

        // 向前移动
        float time = 0;
        while (time < forwardDuration)
        {
            sprite.transform.position = Vector3.Lerp(startPos, targetPos, time / forwardDuration);
            time += Time.deltaTime;
            yield return null;
        }

        // 播放攻击动画 & 等待
        animator.Play(animName);
        yield return new WaitForSeconds(animDelay);

        // 命中点回调（施加伤害等）
        onHitPoint?.Invoke();

        // 返回原位
        time = 0;
        while (time < returnDuration)
        {
            sprite.transform.position = Vector3.Lerp(targetPos, startPos, time / returnDuration);
            time += Time.deltaTime;
            yield return null;
        }

        sprite.transform.position = startPos;
    }

    /// <summary>
    /// 通用治疗特效
    /// </summary>
    public static IEnumerator HealEffect(
        Health targetHealth,
        int healAmount,
        GameObject healPrefab,
        GameObject effectRoot,
        float delay = 0.5f)
    {
        targetHealth.Heal(healAmount);

        if (healPrefab != null && effectRoot != null)
        {
            GameObject effect = UnityEngine.Object.Instantiate(healPrefab, effectRoot.transform);
            UnityEngine.Object.Destroy(effect, 1);
        }

        yield return new WaitForSeconds(delay);
    }
}
