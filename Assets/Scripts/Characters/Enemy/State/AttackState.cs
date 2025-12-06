using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : IState
{
    private EnemyController manager;

    private float attackTimer = 0;


    public AttackState(EnemyController manager)
    {
        this.manager = manager;
    }

    public void OnEnter()
    {
        //停止移动 进行攻击
        manager.agent.isStopped = true;
        //设置攻击状态颜色
        manager.SetColor(Color.red);
        Debug.Log("进入状态：攻击 (Attack) !!!");

        // 进入时重置计时器，让它能“立刻”或者“稍微等一下”再攻击
        // 这里设为 attackInterval，让它进状态后立刻能打第一下
        attackTimer = manager.stats.attackInterval;
    }

    public void OnExit()
    {
        //退出时 恢复移动
        manager.agent.isStopped = false;
    }

    public void OnUpdate()
    {
        //始终面向玩家
        // --- 只在水平方向旋转 (防止仰头) ---
        Vector3 targetPos = GameManager.instance.Player.position;
        targetPos.y = manager.transform.position.y; // 强行把目标高度设为和自己一样
        manager.transform.LookAt(targetPos);

        //距离检测
        float dis = Vector3.Distance(manager.transform.position, GameManager.instance.Player.position);
        //距离超过了攻击距离，切换到追击状态
        if (dis > manager.stats.chaseResumeDistance)
        {
            manager.TransitionToState(manager.ChaseState);
        }

        //攻击逻辑
        attackTimer += Time.deltaTime;
        if( attackTimer >= manager.stats.attackInterval)
        {
            //Attack();

            // 切换到攻击动画
            //manager.anim.SetTrigger("Attack");
            manager.animView.TriggerAttack();
            attackTimer = 0;
        }
    }

    private void Attack()
    {
        //Debug.Log($"[攻击] 造成伤害！时间: {Time.time}");
        // 1. 获取玩家身上的接口
        // 这种写法非常经典：我不需要知道它是 PlayerController 还是 NPC，只要它能受伤就行
        var targetHealth = GameManager.instance.Player.GetComponent<IDamageable>();

        if (targetHealth != null)
        {
            targetHealth.TakeDamage(10); // 扣 10 血
        }
    }

    //由关键帧动画事件调用
    public void AnimationTriggerAttack()
    {
        // 真正的扣血逻辑放在这里
        var target = GameManager.instance.Player.GetComponent<IDamageable>();
        if (target != null) target.TakeDamage(10);
    }
}
