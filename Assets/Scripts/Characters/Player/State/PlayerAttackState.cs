using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//追击状态，目标在技能范围外时，点击敌人需要一个追击状态过度
public class PlayerAttackState : IState
{
    private PlayerController player;
    private SkillSlot skillToUse;       //要使用的技能
    private Transform targetEnemy;      //技能目标

    public PlayerAttackState( PlayerController player )
    {
        this.player = player;
    }

    public void Setup(Transform target, SkillSlot slot)
    {
        this.skillToUse = slot;
        this.targetEnemy = target;
    }

    //设置攻击目标
    public void SetTarget( Transform target )
    {
        this.targetEnemy = target;
    }

    public void OnEnter()
    {
        ////让计时器设为间隔，这样一靠近就能立刻打第一下
        //attackTimer = attackInterval;

        //// 刚进去时，不仅不停，反而要确保可以移动
        //player.agent.isStopped = false;

        //追击状态允许移动
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

        //获取技能射程
        float range = skillToUse.data.castRange;
        float dist = Vector3.Distance(player.transform.position, targetEnemy.position);
        //距离检测
        if( dist > range)
        {
            Debug.Log("dist: "+  dist );
            Debug.Log("range: " + range);
            //距离超过技能范围 追击
            player.agent.SetDestination(targetEnemy.position);
            player.playerAnim.SetMoveSpeed( player.agent.velocity.magnitude );
        }
        else
        {
            //在技能范围内
            if( skillToUse.IsReady )
            {
                // 调用 skillManager 的执行方法ExecuteSkill
                player.GetComponent<PlayerSkillManager>().ExecuteSkill(skillToUse, targetEnemy);
            }
            else
            {
                //技能CD中，
            }
        }        
    }

    public void OnExit() { }
}
