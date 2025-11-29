using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    //相关组件
    //public Animator anim {  get; private set; }
    public CharacterAnimation playerAnim;
    public NavMeshAgent agent { get; private set; }
    public LineRenderer line { get; private set; }

    //移动目的地
    public Vector3 targetPos;

    //状态机相关
    private StateMachine stateMachine;
    //缓存状态
    public PlayerIdleState playerIdleState;
    public PlayerMoveState playerMoveState;
    public PlayerAttackState playerAttackState;

    [Header("输入配置")]
    public LayerMask clickableLayers;

    private void Awake()
    {
        //初始化 获取组件
        // 因为模型通常是 Player 的子物体，所以用 GetComponentInChildren
        playerAnim = GetComponentInChildren<CharacterAnimation>();
        agent = GetComponent<NavMeshAgent>();
        line = GetComponent<LineRenderer>();

        //初始化状态机
        stateMachine = new StateMachine();
        playerIdleState = new PlayerIdleState(this);
        playerMoveState = new PlayerMoveState(this);
        playerAttackState = new PlayerAttackState(this);

        // 默认设置??
        agent.stoppingDistance = 0.1f;
        if (line != null) line.positionCount = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        //此时 InputManager 肯定醒了
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnClick += OnClickMap;
        }

        //第一次进入待机状态
        stateMachine.Initialize(playerIdleState);

        //初始化移动目标位置
        targetPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //轮询调用 当前状态的Update
        stateMachine.Update();
    }

    //对接InputManager
    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnClick -= OnClickMap;
        }
    }

    //处理点击逻辑
    private void OnClickMap()
    {
        //获取鼠标位置
        Vector2 mouseScreenPos = InputManager.Instance.MousePosition;

        //射线检测
        Ray ray = Camera.main.ScreenPointToRay(mouseScreenPos);
        RaycastHit hitInfo;

        if(Physics.Raycast(ray, out hitInfo, 1000, clickableLayers))
        {
            // 尝试从撞到的物体上获取 IDamageable 接口
            IDamageable target = hitInfo.collider.GetComponent<IDamageable>();
            //点到了敌人
            if (target != null)
            {
                // 如果点到了敌人，直接扣血 (或者你可以写一个 PlayerAttackState 去追着打)
                Debug.Log("玩家点击了敌人！");
                //target.TakeDamage(20);

                playerAttackState.SetTarget(hitInfo.transform); // 锁定目标
                SwitchState(playerAttackState); // 切状态
            }
            else
            {
                targetPos = hitInfo.point;
                //切换到Move状态
                SwitchState(playerMoveState);
            }
        }
    }

    //公开切换状态方法
    public void SwitchState( IState newState )
    {
        stateMachine.ChangeState( newState );
    }

    // 专门处理攻击判定的方法
    public void OnAnimationAttackHit()
    {
        Debug.Log("【关键帧触发】刀砍到肉了！开始计算伤害...");

        // 这里写之前的扣血逻辑
        // 1. 获取当前攻击目标
        // 2. 计算距离 (防止挥空)
        // 3. target.TakeDamage(20);

        // 为了测试，先只打印日志
    }

}
