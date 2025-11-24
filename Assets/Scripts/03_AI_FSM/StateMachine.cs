using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    // 只读属性，外界只能看不能改
    public IState CurrentState { get; private set; }

    // 1. 初始化 (启动)
    public void Initialize( IState startState)
    {
        CurrentState = startState;
        CurrentState.OnEnter();
    }

    // 2. 切换状态 (核心逻辑)
    public void ChangeState(IState newState)
    {
        // 退出旧状态
        CurrentState?.OnExit();

        // 切换
        CurrentState = newState;

        // 进入新状态
        CurrentState.OnEnter();
    }

    // 3. 运行 (每帧调用)
    public void Update()
    {
        CurrentState?.OnUpdate();
    }

}
