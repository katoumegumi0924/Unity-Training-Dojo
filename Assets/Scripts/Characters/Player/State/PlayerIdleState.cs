using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : IState
{
    private PlayerStateMachine fsm;

    public PlayerIdleState( PlayerStateMachine fsm )
    {
        this.fsm = fsm;
    }

    public void OnEnter()
    {
        Debug.Log("进入Idle");

        //停止移动动画
        if ( fsm.player != null)
        {
            //player.anim.SetFloat("Speed", 0f);
            fsm.player.characterAnimation.SetMoveSpeed(0, 0);
            Debug.Log("Idle重置速度");
        }

        // 这是一个好习惯：切回待机时重置路径，防止意外滑动
        fsm.player.agent.ResetPath();
    }

    public void OnExit()
    {
        
    }

    public void OnUpdate()
    {
        
    }
}
