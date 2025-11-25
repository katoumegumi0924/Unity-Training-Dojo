using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

#if UNITY_EDITOR
using UnityEditor; // 引入编辑器命名空间用于画图
#endif


//敌人视野范围检测
public class EnemyVision : MonoBehaviour
{
    [Header("视野配置")]
    public float viewRadius = 8f;   //视距，能看多远
    [Range(0, 360)]
    public float viewAngle = 90f;   //视野角度(扇形视野张开的角度)

    public LayerMask targetMask;    //目标层
    public LayerMask obstacleMask;  //障碍物层

    [Header("可视状态")]
    public bool canSeePlayer = false;       //最终结果，能否看见玩家

    // --- 核心功能：检测 ---
    public void CheckSight()
    {
        //默认看不见 每次检测重置
        canSeePlayer = false;

        //范围检测，获取圆形半径内所有的“Target”
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        //遍历范围内所有的“Target”
        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;

            //计算物体方向
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            //进行角度检测，判断目标是否在视野范围内
            if( Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                //与目标的距离
                float dstToTarget = Vector3.Distance(transform.position, target.position);

                //遮挡检测 像目标位置发出射线进行检测
                //如果没有碰到Obstacle 层，说明视线通畅
                if ( !Physics.Raycast( transform.position, dirToTarget,dstToTarget,obstacleMask ))
                {
                    // 三关全过：看见了！
                    canSeePlayer = true;
                    // 如果看见了，画绿线
                    Debug.DrawRay(transform.position, dirToTarget * dstToTarget, Color.green, 0.2f);
                }
                else
                {
                    // 如果被挡住了，画红线
                    Debug.DrawRay(transform.position, dirToTarget * dstToTarget, Color.red, 0.2f);
                }
            }
        }
        Debug.Log("canSeePlayer: " + canSeePlayer);
    }

    private void Start()
    {
        // 开启协程：每 0.2 秒检测一次
        StartCoroutine(FindTargetWithDelay(0.2f));   
    }

    //协程函数 每隔0.2s执行一次CheckSight()
    IEnumerator FindTargetWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            CheckSight();
            
        } 
    }

    // --- 可视化调试 (画出扇形) ---
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // 1. 画视距圆
        Handles.color = new Color(1, 1, 1, 0.2f);
        Handles.DrawWireDisc(transform.position, Vector3.up, viewRadius);

        // 2. 画扇形的两条边
        Vector3 viewAngleA = DirFromAngle(-viewAngle / 2, false);
        Vector3 viewAngleB = DirFromAngle(viewAngle / 2, false);

        Handles.color = new Color(1, 1, 1, 0.5f);
        Handles.DrawLine(transform.position, transform.position + viewAngleA * viewRadius);
        Handles.DrawLine(transform.position, transform.position + viewAngleB * viewRadius);

        // 3. 连线提示
        if (canSeePlayer)
        {
            Handles.color = Color.green;
            // 假设玩家是单例，方便画线
            if (GameManager.instance.Player != null)
                Handles.DrawLine(transform.position, GameManager.instance.Player.position);
        }
    }

    // 辅助函数：根据角度算出方向向量
    private Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        //处理局部坐标
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        // 三角函数公式 (不需要死记，知道是用 Sin/Cos 算坐标即可)
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

#endif
}
