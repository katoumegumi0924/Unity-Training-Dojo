using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor; // 引入编辑器专用命名空间

//敌人状态机管理器
public class EnemyController : MonoBehaviour
{
    //状态机
    private StateMachine stateMachine;

    //// 缓存渲染器，为了实现切换状态变色
    private MeshRenderer meshRenderer;

    //动画状态机
    public Animator anim;

    //所有状态共享的数据和组件
    [HideInInspector]
    public NavMeshAgent agent;      //寻路组件
    [HideInInspector]
    public EnemyVision vision;      //视线范围检测

    //配置数据
    //使用ScriptableObject来配置
    [Header("数据配置")]
    public CharacterStats stats;

    //参数配置
    //public float patrolSpeed = 2.0f;    //巡逻速度
    //public float chaseSpeed = 4.0f;     //追击速度
    //public float detectRange = 5.0f;   //追击检测范围
    //public float attackRange = 0.5f;   //攻击检测范围
    //public float attackInterval = 1f; // 攻击间隔 (秒)

    // 定义变量来缓存状态

    public PatrolState PatrolState { get; private set; }
    public ChaseState ChaseState { get; private set; }
    public AttackState AttackState { get; private set; }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        meshRenderer = GetComponent<MeshRenderer>();
        vision = GetComponent<EnemyVision>();

        anim = GetComponentInChildren<Animator>();

        //初始化状态机
        stateMachine = new StateMachine();
        //创建状态缓存
        PatrolState = new PatrolState(this);
        ChaseState = new ChaseState(this);
        AttackState = new AttackState(this);

        //安全检查 确保配置了必要的数据
        if( stats == null)
        {
            Debug.LogError($"{name} 缺少 CharacterStats 数据配置！请在 Inspector 赋值！");
        }
        else
        {
            //初始化通用属性
            agent.speed = stats.moveSpeed;
            agent.angularSpeed = stats.turnSpeed;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // 启动！先进入巡逻状态
        stateMachine.Initialize(PatrolState);
    }

    // Update is called once per frame
    void Update()
    {
        //核心逻辑，在update中反复调用当前状态的OnUpdate
        stateMachine.Update();
    }

    //切换状态的方法 调用状态机实现
    public void TransitionToState(IState newState)
    {
        stateMachine.ChangeState(newState);
    }

    //切换颜色方法
    public void SetColor(Color color)
    {
        if (color != null && meshRenderer != null)
        {
            meshRenderer.material.color = color;
        }
    }

    // 这里的 #if 意思是：只有在 Unity 编辑器模式下，才编译这段代码
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        

        // --- 1. 画检测范围 (黄色) ---
        Handles.color = new Color(1, 0.92f, 0.016f, 0.5f); // 黄色

        // DrawWireDisc(中心点, 法线方向, 半径)
        // Vector3.up 表示圆环是平躺在地上的 (法线朝上)
        Handles.DrawWireDisc(transform.position, Vector3.up, stats.detectRange);

        // 如果你想看实心的圆盘，可以用 DrawSolidDisc (可选)
        //Handles.color = new Color(1, 0.92f, 0.016f, 0.1f); // 很淡的黄色
        //Handles.DrawSolidDisc(transform.position, Vector3.up, detectRange);

        // --- 2. 画攻击范围 (红色) ---
        Handles.color = Color.red;
        Handles.DrawWireDisc(transform.position, Vector3.up, stats.attackRange);

        // 可选：画一条线指向玩家，方便看有没有丢失目标
        //if (GameManager.instance.Player != null)
        //{
        //    Gizmos.color = Color.blue;
        //    Gizmos.DrawLine(transform.position, GameManager.instance.Player.position);
        //}
    }
#endif

}
