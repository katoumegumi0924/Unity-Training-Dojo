using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : IState
{
    private EnemyController manager;

    public ChaseState(EnemyController manager)
    {
        this.manager = manager;
    }

    // --- 优化变量 ---
    private float repathTimer = 0f;
    private float repathInterval = 0.2f; // 每 0.2 秒重新算一次路

    public void OnEnter()
    {
        //设置追击速度
        manager.agent.speed = manager.stats.chaseSpeed;
        //设置追击状态颜色
        manager.SetColor(Color.yellow);
        //设置动画参数
        manager.anim.SetFloat("Speed", 3.5f);
        Debug.Log("进入状态：追击 (Chase) !!!");
    }

    public void OnExit()
    {
        // 离开时，把速度重置（或者停下来）
        manager.agent.velocity = Vector3.zero;
        //manager.agent.ResetPath();

        manager.anim.SetFloat("Speed", 0f);
    }

    public void OnUpdate()
    {
        // --- 1. 追击逻辑 ---
        // 每0.2s把目标设为玩家的位置，限制agent.SetDestination的频率，优化性能
        repathTimer += Time.deltaTime;
        if(repathTimer > repathInterval)
        {
            manager.agent.SetDestination(GameManager.instance.Player.position);
            repathTimer = 0f;
        }
        

        //检测玩家位置
        float dist = Vector3.Distance(manager.transform.position, GameManager.instance.Player.position);

        //如果看不见玩家了 切换到巡逻状态
        if( !manager.vision.canSeePlayer)
        {
            manager.TransitionToState(manager.PatrolState);
            return;
        }
        //玩家位置进入攻击范围 切换到攻击状态
        if( dist < manager.stats.attackRange )
        {
            manager.TransitionToState(manager.AttackState);
            return;
        }
    }
}
    


    

