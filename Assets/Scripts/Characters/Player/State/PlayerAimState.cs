using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAimState : IState
{
    private PlayerStateMachine fsm;
    private SkillSlot currentSkill;         //当前要瞄准的技能

    public PlayerAimState(PlayerStateMachine fsm)
    {
        this.fsm = fsm;
    }

    //进入状态前需要设置要瞄准的技能是哪个
    public void SetSkill( SkillSlot slot )
    {
        this.currentSkill = slot;
    }

    public void OnEnter()
    {
        Debug.Log($"[Aim] 开始瞄准: {currentSkill.data.skillName}");

        //瞄准状态停止移动
        fsm.player.agent.isStopped = true;
        fsm.player.agent.velocity = Vector3.zero;
        fsm.player.agent.ResetPath();
        fsm.player.characterAnimation.SetMoveSpeed(0,0);

        //显示指示器
        if( fsm.player.skillManager != null && fsm.player.skillManager.skillIndicator != null)
        {
            fsm.player.skillManager.skillIndicator.Show(currentSkill.data.castRange);
        }

        //监听输入 决定是选择目标释放技能还是取消释放
        if( InputManager.Instance != null)
        {
            InputManager.Instance.OnClick += OnClickToCast;
            // 假设你加了一个右键取消事件，或者按下 ESC
            // InputManager.Instance.OnCancel += OnCancelAim;
        }
    }

    public void OnUpdate()
    {
        //角色实时面向鼠标指向的方向
        UpdateRotation();

        // 2. (可选) 检测右键取消逻辑，如果没有封装事件，可以直接读
        //if (Mouse.current.rightButton.wasPressedThisFrame)
        //{
        //    OnCancelAim();
        //}
    }

    public void OnExit()
    {
        //退出瞄准状态要隐藏指示器
        if( fsm.player.skillManager !=null && fsm.player.skillManager.skillIndicator != null )
        {
            fsm.player.skillManager.skillIndicator.Hide();
        }

        //移除时间监听防止内存泄露
        InputManager.Instance.OnClick -= OnClickToCast;

        Debug.Log("[Aim] 退出瞄准");
    }

    //--- 内部逻辑 ---
    //角色实时面向鼠标指向的方向
    private void UpdateRotation()
    {
        //获取鼠标位置射线
        Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
        RaycastHit hitInfo;

        //
        if( Physics.Raycast( ray, out hitInfo, 1000f, LayerMask.GetMask("Ground") ))
        {
            Vector3 lookPos = hitInfo.point;
            lookPos.y = fsm.player.transform.position.y; // 保持水平
            fsm.player.transform.LookAt( lookPos );
        }
    }

    //确认释放技能
    private void OnClickToCast()
    {
        //获取目标
        //Transform target = skillMgr.GetTargetUnderMouse();
        Transform target = fsm.player.skillManager.GetSmartTarget();

        if ( target != null )
        {
            // --- 距离校验 ---
            float dist = Vector3.Distance(fsm.player.transform.position, target.position);
            if (dist > currentSkill.data.castRange)
            {
                // 可以在这里播一个 "哔哔" 的错误音效，或者飘字 "距离太远"
                Debug.Log($"<color=red>距离太远！当前: {dist:F1}, 射程: {currentSkill.data.castRange}</color>");
                return; // 拦截！不切换状态，不播动画
            }

            //设置 技能释放状态参数
            fsm.playerCastState.SetSkill(currentSkill, target, target.position);
            //切换到技能释放状态
            fsm.SwitchState(fsm.playerCastState);
        }
        //没有点击到敌人 切回待机状态    
        else
        {
            fsm.SwitchState(fsm.playerIdleState);
        }
    }

    private void OnCancelAim()
    {
        // 取消瞄准，回到待机
        fsm.SwitchState(fsm.playerIdleState);
    }
}
