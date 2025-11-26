using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : IState
{
    private PlayerController player;
    public PlayerMoveState(PlayerController player)
    {
        this.player = player;
    }

    List<Vector3> drawPoints = new List<Vector3>();

    public void OnEnter()
    {
        //进状态就设置目标，只设一次
        // 确保目标是有效的
        player.agent.enabled = true;
        player.agent.SetDestination(player.targetPos);
        player.agent.isStopped = false;
    }

    public void OnExit()
    {
        // 清空画线
        if (player.line != null) player.line.positionCount = 0;
    }

    public void OnUpdate()
    {
        //播放移动动画
        if (player.anim != null)
        {
            player.anim.SetFloat("Speed", player.agent.velocity.magnitude, 0.1f, Time.deltaTime);
        }

        // 核心修改 2：标准的 NavMesh 到达检测逻辑
        // A. pathPending: 还在算路吗？在算就别急。
        // B. remainingDistance: 剩下的路还有多长？
        //不算路了 && 距离够近了
        if (!player.agent.pathPending && player.agent.remainingDistance <= player.agent.stoppingDistance)
        {
            // 到了 -> 申请切回 Idle
            // (注意：要确保 hasPath 为 true 或者是刚开始移动，防止还没动就判停)
            //没路了 || 彻底停稳了
            if (!player.agent.hasPath || player.agent.velocity.sqrMagnitude == 0f)
            {
                // 真的到了目的地，切回待机状态
                player.SwitchState(player.playerIdleState);
            }
        }

        //画出移动轨迹
        DrawLine();
    }

    private void DrawLine()
    {
        if( player.line == null ) { return; }
        // 每次画线前，先清空旧数据
        drawPoints.Clear();

        // 加上脚下位置
        drawPoints.Add(player.transform.position);

        if ( player.agent.hasPath)
        {
            drawPoints.AddRange(player.agent.path.corners);
        }

        player.line.positionCount = drawPoints.Count;
        for(int i = 0; i < drawPoints.Count; i++)
        {
            player.line.SetPosition(i, drawPoints[i] + Vector3.up * 0.1f);
        }
    }

}
