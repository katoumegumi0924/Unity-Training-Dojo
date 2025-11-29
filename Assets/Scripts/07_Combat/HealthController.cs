using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour,IDamageable
{
    [Header("基础属性")]
    public int maxHealth;

    public int currentHealth {  get; private set; }

    //死亡标志
    public bool isDead => currentHealth <= 0;

    //事件系统,方便 UI 和 动画 监听
    public event Action OnTakeDamage;
    public event Action<int> OnHealthChanged;
    public event Action OnDeath;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    // Start is called before the first frame update
    void Start()
    {
        // 初始通知一下 UI        ?
        OnHealthChanged?.Invoke(currentHealth);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Die()
    {
        Debug.Log($"{name} 挂了！");
        OnDeath?.Invoke();

        // 暂时先简单销毁，后面我们会做更酷的溶解特效
        //Destroy(gameObject, 1f); // 延迟2秒销毁，留时间播动画
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        //受伤扣除血量
        currentHealth -= damage;

        //血量不能低于0
        currentHealth = Math.Max(currentHealth, 0);

        //触发受伤和血量变化事件
        OnTakeDamage?.Invoke();
        OnHealthChanged?.Invoke(damage);

        Debug.Log($"{name} 受到 {damage} 点伤害，剩余: {currentHealth}");

        if ( currentHealth <= 0)
        {
            Die();
        }
    }
}
