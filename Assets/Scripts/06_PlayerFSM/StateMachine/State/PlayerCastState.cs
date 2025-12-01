using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//施法状态，所有技能都由这个状态管理
public class PlayerCastState : IState
{
    private PlayerController player;

    public PlayerCastState(PlayerController player)
    {
        this.player = player;
    }

    //要施放的技能休息
    private SkillSlot currentSkill;
    private Transform currentTarget;

    //接收参数方法
    public void SetSkill( SkillSlot slot, Transform target )
    {
        currentSkill = slot;    
        currentTarget = target;
    }

    public void OnEnter()
    {
        //施法期间停止移动
        player.agent.isStopped = true;
        player.agent.velocity = Vector3.zero;
        player.agent.ResetPath();
        player.playerAnim.SetMoveSpeed(0);

        //面向施法目标（如果有）
        if( currentTarget != null)
        {
            player.transform.LookAt( new Vector3( currentTarget.position.x, player.transform.position.y, currentTarget.position.z ) );
        }

        //播放对应的施法动画
        if( !string.IsNullOrEmpty(currentSkill.data.animTriggerName))
        {
            player.playerAnim.TriggleSkill(currentSkill.data.animTriggerName);
        }

        //协程处理延迟伤害 和 结束施法状态
        // 4. 开启协程：处理“延迟伤害”和“状态结束”
        // 注意：IState 是纯类，不能开启协程，需要借用 player (MonoBehaviour) 来开
        player.StartCoroutine(CastRoutine());
    }

    public void OnExit()
    {
        // 恢复行动自由
        player.agent.isStopped = false;

    }

    public void OnUpdate()
    {

    }

    //施法协程
    IEnumerator CastRoutine()
    {
        //开始计算计算技能冷却
        currentSkill.StartCooldown();

        //等待前摇
        yield return new WaitForSeconds(currentSkill.data.damageDelay);

        //正式释放技能，进行技能伤害判定
        currentSkill.data.strategy.Cast(player.transform, currentTarget);
            
        //等待技能后摇
        float remainingTime = currentSkill.data.castDuration - currentSkill.data.damageDelay;
        if( remainingTime > 0 )
        {
            yield return new WaitForSeconds(remainingTime);
        }

        //施法结束，返回Idle状态
        player.SwitchState(player.playerIdleState);
    }
}
