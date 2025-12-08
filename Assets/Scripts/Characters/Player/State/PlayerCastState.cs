using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//施法状态，所有技能都由这个状态管理
public class PlayerCastState : IState
{
    private PlayerStateMachine fsm;

    public PlayerCastState(PlayerStateMachine fsm)
    {
        this.fsm = fsm;
    }

    //要施放的技能信息
    private SkillSlot currentSkill;
    private Transform currentTarget;

    private Vector3 targetPoint; // 鼠标点击的地面坐标

    //接收参数方法
    public void SetSkill( SkillSlot slot, Transform target, Vector3 point )
    {
        this.currentSkill = slot;    
        this.currentTarget = target;
        this.targetPoint = point;
    }

    public void OnEnter()
    {
        //施法期间停止移动
        fsm.player.agent.isStopped = true;
        fsm.player.agent.velocity = Vector3.zero;
        fsm.player.agent.ResetPath();
        fsm.player.characterAnimation.SetMoveSpeed(0);

        //面向施法目标（如果有）
        if( currentTarget != null)
        {
            fsm.player.transform.LookAt( new Vector3( currentTarget.position.x, fsm.player.transform.position.y, currentTarget.position.z ) );
        }
        //没有目标，看向鼠标指定的施法点
        else
        {
            Vector3 lookPos = targetPoint;
            lookPos.y = fsm.player.transform.position.y;
            fsm.player.transform.LookAt( lookPos );
        }

        //播放对应的施法动画
        if( !string.IsNullOrEmpty(currentSkill.data.animTriggerName))
        {
            fsm.player.characterAnimation.TriggleSkill(currentSkill.data.animTriggerName);
        }

        //协程处理延迟伤害 和 结束施法状态
        // 4. 开启协程：处理“延迟伤害”和“状态结束”
        // 注意：IState 是纯类，不能开启协程，需要借用 player (MonoBehaviour) 来开
        fsm.StartCoroutine(CastRoutine());
    }

    public void OnExit()
    {
        // 恢复行动自由
        fsm.player.agent.isStopped = false;

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
        currentSkill.data.strategy.Cast(fsm.player.transform, currentTarget, targetPoint, currentSkill.data);
            
        //等待技能后摇
        float remainingTime = currentSkill.data.castDuration - currentSkill.data.damageDelay;
        if( remainingTime > 0 )
        {
            yield return new WaitForSeconds(remainingTime);
        }

        //施法结束，返回Idle状态
        fsm.SwitchState(fsm.playerIdleState);
    }
}
