using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//敌人状态机管理器
public class EnemyController : MonoBehaviour
{
    //状态机
    private StateMachine stateMachine;

    //所有状态共享的数据和组件
    [HideInInspector]
    public NavMeshAgent agent;      //寻路组件
    //[HideInInspector]
    //public Transform Player;        //玩家位置

    //参数配置
    public float patrolSpeed = 2.0f;    //巡逻速度
    public float chaseSpeed = 4.0f;     //追击速度
    public float detectRange = 10.0f;   //检测范围

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        // 假设场景里只有一个 Player (Tag 查找)
        // Player = GameObject.FindGameObjectWithTag("Player").transform;

        //初始化状态机
        stateMachine = new StateMachine();
    }

    // Start is called before the first frame update
    void Start()
    {
        // 启动！先进入巡逻状态
        stateMachine.Initialize(new PatrolState(this));
    }

    // Update is called once per frame
    void Update()
    {
        //核心逻辑，在update中反复调用当前状态的OnUpdate
        stateMachine.Update();
    }

    //切换状态的方法 调用状态机实现
    public void TransitionToState( IState newState )
    {
        stateMachine.ChangeState(newState);    
    }
}
