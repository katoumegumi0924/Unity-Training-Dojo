using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//受击状态
public class HurtState : IState
{  
    public HurtState(EnemyController manager)
    {
        this.manager = manager;
    }

    private EnemyController manager;
    private float stunTime = 0.5f;      //硬直时间
    private float timer = 0;

    public void OnEnter()
    {
        //进入被击状态时
        //停止移动
        manager.agent.isStopped = true;
        manager.agent.velocity = Vector3.zero;

        //播放受击动画
        manager.animView.TriggerHurt();

        //播放一个受击特效？？

        timer = 0;
    }

    public void OnExit()
    {
        //退出受击状态时恢复移动
        manager.agent.isStopped = false;
    }

    public void OnUpdate()
    {
        //计时，硬直结束后切换到追击
        timer += Time.deltaTime;
        if( timer >= stunTime)
        {
            //Debug.Log("死亡状态： " + manager.health.isDead);
            ////如果以死亡  切换到死亡状态
            //if( manager.health.isDead)
            //{
            //    manager.TransitionToState(manager.DeadState);
            //}
            //else
            //{
            //    manager.TransitionToState(manager.ChaseState);
            //}
            manager.TransitionToState(manager.ChaseState);

        }

    }
}
    
