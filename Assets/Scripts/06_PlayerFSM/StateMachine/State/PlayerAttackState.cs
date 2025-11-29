using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAttackState : IState
{
    private PlayerController player;

    public PlayerAttackState( PlayerController player )
    {
        this.player = player;
    }

    //攻击目标
    private Transform targetEnemy;

    // 攻击配置
    private float stopDistance = 2.0f;      // 什么时候停下来打？
    private float resumeChaseDistance = 3.0f; // 什么时候重新开始追？(防抖缓冲)

    private float attackInterval = 1.5f;    // 攻击间隔
    private float attackTimer = 0f;

    //设置攻击目标
    public void SetTarget( Transform target )
    {
        this.targetEnemy = target;
    }

    public void OnEnter()
    {
        //让计时器设为间隔，这样一靠近就能立刻打第一下
        attackTimer = attackInterval;

        // 刚进去时，不仅不停，反而要确保可以移动
        player.agent.isStopped = false;
    }

    public void OnUpdate()
    {
        //目标丢失 切回待机状态
        if( targetEnemy == null )
        {
            player.SwitchState(player.playerIdleState);
            return;
        }

        float dist = Vector3.Distance(player.transform.position, targetEnemy.position);

        //攻击计时器
        attackTimer += Time.deltaTime;

        // 情况 A: 还没追上，或者敌人跑远了 -> 追！
        // 注意这里用了 resumeChaseDistance (3.0) 而不是 2.0
        // 只有当 agent 正在移动时用 2.0 判断，停下来后用 3.0 判断
        bool isMoving = player.agent.hasPath && !player.agent.isStopped;
        float chaseThreshold = isMoving ? stopDistance : resumeChaseDistance;

        //没有到攻击距离 追击
        if (dist > chaseThreshold)
        {
            player.agent.isStopped = false ;
            player.agent.SetDestination(targetEnemy.position);

            // 播放跑动画
            //player.anim.SetFloat("Speed", player.agent.velocity.magnitude, 0.1f, Time.deltaTime);
            player.playerAnim.SetMoveSpeed(player.agent.velocity.magnitude);
        }
        //攻击
        else
        {
            //先停止移动
            player.agent.isStopped = true ;
            //player.anim.SetFloat("Speed", 0f);
            player.playerAnim.SetMoveSpeed(0);

            //面向敌人
            // 简单 LookAt 可能会导致仰头，这里只转 Y 轴
            Vector3 lookPos = targetEnemy.position;
            lookPos.y = player.transform.position.y;
            player.transform.LookAt(lookPos);

            //攻击逻辑
            if ( attackTimer > attackInterval )
            {
                //player.anim.SetTrigger("Attack");
                player.playerAnim.TriggerAttack();
                attackTimer = 0; // 归零

                var targetHealth = targetEnemy.GetComponent<HealthController>();
                if (targetHealth != null)
                {
                    targetHealth.TakeDamage(20);
                    Debug.Log("攻击扣血：-10");
                }
            }

            
        }
    }

    public void OnExit() { }
}
