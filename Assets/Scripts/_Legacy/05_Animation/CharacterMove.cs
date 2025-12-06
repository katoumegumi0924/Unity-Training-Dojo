using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterMove : MonoBehaviour
{
    //使用NavMeshAgent组件
    private NavMeshAgent agent;
    //划线组件
    private LineRenderer lineRenderer;
    //动画组件
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        //初始化
        agent = GetComponent<NavMeshAgent>();
        lineRenderer = GetComponent<LineRenderer>();
        // 因为模型通常是 Player 的子物体，所以用 GetComponentInChildren
        anim = GetComponentInChildren<Animator>();

        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 0;

            //把线提高一点点 方便观察
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //获取射线
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //碰撞信息
            RaycastHit hitInfo;

            //发出射线
            if (Physics.Raycast(ray, out hitInfo))
            {
                //调用NavMeshAgent的Api,设置目的地
                //agent自动计算A*路径并移动
                agent.SetDestination(hitInfo.point);
            }
        }

        UpdateAnimation();

        //绘制移动轨迹
        if (lineRenderer != null && agent.hasPath)
        {
            lineRenderer.enabled = true;

            //获取agent计算出的所有拐点
            Vector3[] corners = agent.path.corners;

            //告诉lineRenderer一共包含多少个拐点
            lineRenderer.positionCount = corners.Length;

            //遍历每个点，设置给lineRenderer
            for (int i = 0; i < corners.Length; i++)
            {
                Vector3 elevatedPoint = new Vector3(corners[i].x, 0.1f, corners[i].z);
                lineRenderer.SetPosition(i, elevatedPoint);
            }

        }
        else
        {
            //如果没有轨迹或者到了终点
            //agent.remainingDistance表示剩余距离
            if (agent.remainingDistance < 0.1f)
            {
                lineRenderer.enabled = false;
            }
        }
    }

    //动画处理逻辑
    void UpdateAnimation()
    {
        if( anim != null)
        {
            // 1. 获取 NavMesh 当前的实际速度 (0 ~ 3.5 ~ 5)
            // velocity.magnitude 是向量的长度，代表速率
            float currentSpeed = agent.velocity.magnitude;

            // 2. 传给 Animator
            // 参数名 "Speed" 必须和你等会在 Animator 窗口里建的参数名一模一样！
            // 0.1f 是阻尼时间(DampTime)，让动画切换不那么生硬，平滑过渡
            anim.SetFloat("Speed", currentSpeed, 0.1f, Time.deltaTime);
        }
    }
}
