using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//单个技能栏位
[System.Serializable]
public class SkillSlot
{
    public SkillData data;          //技能静态数据
    public float currentCooldown;   //当前冷却时间

    //辅助属性 技能是否冷却
    public bool IsReady => currentCooldown <= 0;

    //计算技能当前冷却时间
    public void Tick( float deltaTime )
    {
        if( currentCooldown > 0)
        {
            currentCooldown -= deltaTime;
        }
    }

    //技能进入冷却时间
    public void StartCooldown()
    {
        if( data  != null)
        {
            currentCooldown = data.cooldown;
        }
    }
}

public class PlayerSkillManager : MonoBehaviour
{
    [Header("配置技能组")]
    public List<SkillSlot> skillSlots = new List<SkillSlot>();        

    //玩家引用，需要获取技能发起的目标
    private PlayerController player;

    //技能指示器
    public SkillIndicator skillIndicator;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
        // 加上 true参数：意思是 "includeInactive" (包含未激活的物体)
        skillIndicator = GetComponentInChildren<SkillIndicator>(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        

        if ( InputManager.Instance != null)
        {
            //InputManager.Instance.OnSkill1 += UseSkill;
            //InputManager.Instance.OnSkill1 += () => CastSkill(0);
            InputManager.Instance.OnSkill1 += () => OnSkillButtonPress(0);
            InputManager.Instance.OnSkill2 += () => OnSkillButtonPress(1);
        }
    }

    private void OnDestroy()
    {
        if (InputManager.Instance != null)
        {
            //InputManager.Instance.OnSkill1 -= UseSkill;
            //InputManager.Instance.OnSkill1 += () => CastSkill(0);
            InputManager.Instance.OnSkill1 -= () => OnSkillButtonPress(0);
            InputManager.Instance.OnSkill2 -= () => OnSkillButtonPress(1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //冷却时间倒计时
        foreach( var slot in skillSlots )
        {
            slot.Tick( Time.deltaTime );
        }
    }

    public void OnSkillButtonPress( int index)
    {
        // 如果已经在施法或瞄准，禁止打断（或者根据需求允许打断）
        // 这里先禁止，防止逻辑混乱
        if (player.stateMachine.CurrentState == player.playerCastState ||
            player.stateMachine.CurrentState == player.playerAimState)
        {
            return;
        }

        //安全检查
        if (index < 0 || index >= skillSlots.Count) return;

        //拿出具体的技能
        var slot = skillSlots[index];
        
        //空数据检查
        if( slot == null || slot.data == null ) return;

        //冷却时间检查
        if (!slot.IsReady)
        {
            Debug.Log("技能冷却中");
            return;
        }

        //核心逻辑分流，技能是否需要目标
        if( slot.data.requireTarget)
        {
            //需要瞄准的技能切换到瞄准状态
            player.playerAimState.SetSkill(slot);
            player.SwitchState(player.playerAimState);
        }
        else
        {
            // 获取鼠标在地面的点击点 (用于非锁定技能的方向计算)
            Vector3 mousePoint = Vector3.zero;
            //如果鼠标点击到了地板，朝鼠标位置施法
            if( TryGetMousePosition(out mousePoint))
            {
                //不需要瞄准 直接切换到施法状态
                player.playerCastState.SetSkill(slot, null, mousePoint);
                player.SwitchState(player.playerCastState);
            }
            //鼠标没有点击到地板 朝人物前方施法
            else
            {
                player.playerCastState.SetSkill(slot, null, player.transform.forward + player.transform.position);
            }
            
        }

    }

    //核心方法，根据技能的索引施放具体技能 弃用
    private void CastSkill( int index )
    {
        //安全检查
        if( index < 0 || index >= skillSlots.Count ) return;

        //拿出具体的技能
        var slot = skillSlots[index];
        if( slot == null )  return;         //空槽位

        //冷却时间检查
        if( !slot.IsReady )
        {
            Debug.Log("技能冷却中");
            return;
        }
        
        //目标选择
        //Transform target = FindObjectOfType<EnemyController>()?.transform;
        Transform target = GetTargetUnderMouse();
        if (target == null)
        {
            Debug.Log("需要选择一个目标！");
            return;
        }

        //直接调用的释放技能方式
        ////播放对应动画
        //if( !string.IsNullOrEmpty(slot.data.animTriggerName))
        //{
        //    view.TriggleSkill(slot.data.animTriggerName);
        //}

        //// 4. 执行策略 (插卡带！)
        //// 这里的 strategy 就是你配置的 DirectDamageStrategy
        //slot.data.strategy.Cast(this.transform, target);

        //通过施法状态来释放技能
        var castState = player.playerCastState;
        //传递参数
        castState.SetSkill(slot, target, target.position);

        //切换状态
        player.SwitchState(castState);



        // 5. 进入冷却
        slot.StartCooldown();

        Debug.Log($"使用了技能: {slot.data.skillName}");
    }

    //获取鼠标指向的敌人，作为技能的攻击目标
    public Transform GetTargetUnderMouse()
    {
        //获取鼠标射线
        Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
        RaycastHit hitInfo;

        //射线检测
        //获取Enemy所在的LayerMask
        int enemyLyaer = LayerMask.GetMask("Enemy");

        if(Physics.Raycast(ray, out hitInfo, 100f,enemyLyaer))
        {
            return hitInfo.transform;
        }

        //没有点击到敌人
        return null;
    }

    //辅助瞄准，自动选择离鼠标最近的对象
    public Transform GetSmartTarget()
    {
        Debug.Log($"<color=green> 辅助检测</color>");

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        //优先检测 是否直接点击到了敌人
        int enemyLayerMask = LayerMask.GetMask("Enemy");
        if (Physics.Raycast(ray, out hitInfo, 100f, enemyLayerMask))
        {
            return hitInfo.transform;
        }

        //辅助检测 如果没有直接点击到敌人，检测鼠标点击的地面周围是否存在敌人
        int groundLayerMask = LayerMask.GetMask("Ground");
        if (Physics.Raycast(ray, out hitInfo, 1000f, groundLayerMask))
        {
            Debug.Log($"<color=green> 辅助检测--地面</color>");

            //在鼠标落点 3m 内搜索敌人
            float searchRadius = 5.0f;
            //范围检测
            Collider[] enemies = Physics.OverlapSphere(hitInfo.point, searchRadius, enemyLayerMask);
            Debug.Log($"<color=green> 辅助检测：Collider{enemies == null}</color>");

            //找出最近的一个
            Transform bestTarget = null;
            //用平方距离比较，性能更好
            float closetDistSqr = Mathf.Infinity;

            foreach ( var enemy in enemies )
            {
                //计算敌人与鼠标落点的平方距离
                float dSqr = (enemy.transform.position - hitInfo.point).sqrMagnitude;
                if ( dSqr < closetDistSqr )
                {
                    closetDistSqr = dSqr;
                    bestTarget = enemy.transform;
                }
            }

            if (bestTarget != null)
            {
                // 可选：在这里给 bestTarget 加一个高亮框，提升反馈
                Debug.Log($"<color=green> 辅助瞄准成功</color>");
                return bestTarget;
            }          
        }
        return null; // 真的啥也没点到
    }

    //辅助方法，获取鼠标当前点击的位置
    //Try-Get 模式， 返回 bool 表示是否成功，position 带出结果
    public bool TryGetMousePosition( out Vector3 position )
    {
        Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.MousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("Ground")))
        {
            position = hit.point;
            return true;
        }
        // 虽然赋值了，但外面看到返回 false 就不会用它
        position = Vector3.zero;
        return false;
    }
}
