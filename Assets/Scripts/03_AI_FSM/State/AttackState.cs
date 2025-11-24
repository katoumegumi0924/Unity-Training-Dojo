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
        Debug.Log("进入状态：攻击 (Attack) !!!");

        // 进入时重置计时器，让它能“立刻”或者“稍微等一下”再攻击
        // 这里设为 attackInterval，让它进状态后立刻能打第一下
        attackTimer = manager.attackInterval;
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
        // 只有当距离 大于 (攻击距离 + 缓冲距离) 时才追
        // 比如攻击距离是 2，那跑到 2.5 米开外才开始追
        if (dis > manager.attackRange + 0.5f)
        {
            manager.TransitionToState(manager.ChaseState);
        }

        //攻击逻辑 先打印信息代替
        attackTimer += Time.deltaTime;
        if( attackTimer >= manager.attackInterval)
        {
            Attack();
            attackTimer = 0;
        }
    }

    private void Attack()
    {
        Debug.Log($"[攻击] 造成伤害！时间: {Time.time}");
    }
}
