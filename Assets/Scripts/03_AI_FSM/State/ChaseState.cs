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

    public void OnEnter()
    {
        //设置追击速度
        manager.agent.speed = manager.chaseSpeed;
        Debug.Log("进入状态：追击 (Chase) !!!");
    }

    public void OnExit()
    {
        // 离开时，把速度重置（或者停下来）
        manager.agent.ResetPath();
    }

    public void OnUpdate()
    {
        ////检测玩家位置
        //float dis = Vector3.Distance(manager.transform.position, manager.Player.position);

        ////在追击范围内 继续追击逻辑
        //if( dis < manager.detectRange)
        //{
        //    manager.agent.SetDestination(manager.Player.position);
        //}
        ////玩家离开追击范围
        //else
        //{
        //    //切换回巡逻状态
        //    manager.TranstionToState(new PatrolState(manager));
        //}

        // --- 1. 追击逻辑 ---
        // 每帧都把目标设为玩家的位置
        manager.agent.SetDestination(GameManager.instance.Player.position);

        //检测玩家位置
        float dist = Vector3.Distance(manager.transform.position, GameManager.instance.Player.position);

        if (dist > manager.detectRange * 1.5f) // 乘以1.5是为了防止在边界反复横跳（防抖）
        {
            manager.TransitionToState(manager.PatrolState);
        }
    }
}
    


    

