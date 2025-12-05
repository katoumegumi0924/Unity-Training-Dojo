using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class PlayerSkillManager : MonoBehaviour
{
    [Header("配置技能组")]
    public List<SkillSlot> skillSlots = new List<SkillSlot>();

    [Header("角色基础技能组")]
    public SkillData basePrimary;
    public SkillData baseSkillQ;
    public SkillData baseClassE;
    public SkillData baseUltR;

    //玩家引用，需要获取技能发起的目标
    private PlayerController player;

    //技能指示器
    [HideInInspector]
    public SkillIndicator skillIndicator;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
        // 加上 true参数：意思是 "includeInactive" (包含未激活的物体)
        skillIndicator = GetComponentInChildren<SkillIndicator>(true);

        //初始化技能
        // 即使没有技能，也要占个坑
        skillSlots.Add(new SkillSlot(SkillSlotType.Primary, basePrimary)); // Index 0
        skillSlots.Add(new SkillSlot(SkillSlotType.Skill_1, baseSkillQ));  // Index 1 (Q)
        skillSlots.Add(new SkillSlot(SkillSlotType.Class_1, baseClassE));  // Index 2 (E)
        skillSlots.Add(new SkillSlot(SkillSlotType.Ultimate, baseUltR));   // Index 3 (R)
    }

    // Start is called before the first frame update
    void Start()
    {
        

        if ( InputManager.Instance != null)
        {
            InputManager.Instance.OnUseSkill += OnSkillButtonPress;
        }
    }

    private void OnDestroy()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnUseSkill -= OnSkillButtonPress;
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

        //安全检查 空数据检查 冷却时间检查
        if (index < 0 || index >= skillSlots.Count) return;
        var slot = skillSlots[index];
        if( slot == null || slot.data == null ) return;
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
            //定义最终目标的施法点 用于非锁定技能的方向计算
            Vector3 finalCastPoint;

            //优先尝试物理检测(检测地板碰撞器)，
            if ( TryGetMousePosition(out Vector3 hitPoint))
            {
                finalCastPoint = hitPoint;
            }
            //鼠标没有点击到地板，创建一个数学平面作为保底
            else
            {
                //创建一个位于玩家脚下的水平面
                Plane infiniteFloor = new Plane( Vector3.up, player.transform.position );
                Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.MousePosition);

                if( infiniteFloor.Raycast( ray, out float enterDist))
                {
                    // 获取射线在 distance 处的坐标
                    finalCastPoint = ray.GetPoint(enterDist);
                }
                else
                {
                    //最后的保底(只有当玩家仰望天空，射线平行或射向天空时才会进这里)，保持朝向前方
                    finalCastPoint = player.transform.position + player.transform.forward * 5f;
                }               
            }

            //切换到施法状态 传入计算的坐标
            player.playerCastState.SetSkill(slot, null, finalCastPoint);
            player.SwitchState(player.playerCastState);
        }

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

    // 供 AttackState 追到人后调用
    public void ExecuteSkill( SkillSlot slot, Transform target)
    {
        //检查冷却
        if (!slot.IsReady) return;

        //切换到施法状态
        player.playerCastState.SetSkill(slot, target, target.position);
        player.SwitchState(player.playerCastState);

        //技能开始冷却
        slot.StartCooldown();

    }

    //覆盖指定类型（键位）的技能，供外部调用
    public void OverrideSkill(SkillSlotType type, SkillData newSkill)
    {
        //根据技能类型，查找对应的技能栏位
        var targetSlot = skillSlots.Find( s => s.slotType == type );

        if (targetSlot != null)
        {
            //覆盖对应skillSlot上的技能
            targetSlot.SetOverride( newSkill);

            Debug.Log($"技能槽 [{type}] 已替换为: {newSkill.skillName}");
        }
        else
        {
            Debug.LogWarning($"找不到类型为 {type} 的技能槽！");
        }
    }

}
