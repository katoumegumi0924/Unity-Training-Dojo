using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : IState
{
    public DeadState(EnemyController manager)
    {
        this.manager = manager; 
    }

    private EnemyController manager;

    public void OnEnter()
    {
        Debug.Log("进入状态：死亡 (Dead)");

        //停止移动
        manager.agent.isStopped = true;
        manager.agent.enabled = false;

        //播放死亡动画
        manager.animView.SetBoolDie(true);

        //禁用碰撞
        var collider = manager.GetComponent<Collider>();
        if (collider != null )
        {
            collider.enabled = false;
        }
    }

    public void OnExit()
    {
        // 只有复活术才能把它救回来
    }

    public void OnUpdate()
    {
        // 尸体应该什么都不做
    }
}
