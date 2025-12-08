using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class PlayerStateMachine : MonoBehaviour
{
    //移动目的地
    public Vector3 targetPos;

    //状态机相关
    public StateMachine stateMachine;
    //缓存状态
    public PlayerIdleState playerIdleState;
    public PlayerMoveState playerMoveState;
    public PlayerAttackState playerAttackState;
    public PlayerCastState playerCastState;
    public PlayerAimState playerAimState;

    [Header("输入配置")]
    public LayerMask clickableLayers;

    //状态机 持有 Player类的引用
    public Player player;

    private void Awake()
    {
        player = GetComponent<Player>();

        //初始化状态机
        stateMachine = new StateMachine();
        playerIdleState = new PlayerIdleState(this);
        playerMoveState = new PlayerMoveState(this);
        playerAttackState = new PlayerAttackState(this);
        playerCastState = new PlayerCastState(this);
        playerAimState = new PlayerAimState(this);

    }

    // Start is called before the first frame update
    void Start()
    {
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

        //监听点击 在Update中轮询是否点击鼠标，
        //因为OnClickMap需要检测鼠标指针现在是否悬停在某个 UI 元素上
        //如果使用事件监听，会在Input输入时直接调用OnClickMap，
        //此时当前帧 射线检测还未触发，只能获取上一帧的鼠标位置，会触发警告
        if ( InputManager.Instance != null && InputManager.Instance.IsClickedPress)
        {
            OnClickMap();
        }
    }

    //对接InputManager
    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {

    }

    //处理点击逻辑
    private void OnClickMap()
    {
        // ---核心修复：UI 拦截 ---
        // IsPointerOverGameObject() 的意思是：鼠标指针现在是否悬停在某个 UI 元素上？
        // 如果是，直接 return，不执行后面的射线移动逻辑。
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        //施法状态禁止移动打断
        if ( stateMachine.CurrentState == playerCastState)
        {
            return;
        }

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

                //取出普工技能槽位
                var basicSkillSlot = GetComponent<PlayerSkillManager>().skillSlots[0];

                //切换到追击状态 
                if( basicSkillSlot != null)
                {
                    Debug.Log("hitInfo: " + hitInfo.transform);
                    Debug.Log("basicSkillSlot: " + basicSkillSlot);
                    playerAttackState.Setup(hitInfo.transform, basicSkillSlot);
                    SwitchState(playerAttackState);
                }
                
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

}
