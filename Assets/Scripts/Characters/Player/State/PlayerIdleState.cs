using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : IState
{
    private PlayerController player;

    public PlayerIdleState( PlayerController player )
    {
        this.player = player;
    }

    public void OnEnter()
    {
        Debug.Log("进入Idle");

        //停止移动动画
        if ( player != null)
        {
            //player.anim.SetFloat("Speed", 0f);
            player.playerAnim.SetMoveSpeed(0, 0);
            Debug.Log("Idle重置速度");
        }

        // 这是一个好习惯：切回待机时重置路径，防止意外滑动
        player.agent.ResetPath();
    }

    public void OnExit()
    {
        
    }

    public void OnUpdate()
    {
        
    }
}
