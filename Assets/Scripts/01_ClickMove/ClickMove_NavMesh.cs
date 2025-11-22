using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ClickMove_NavMesh : MonoBehaviour
{
    //使用NavMeshAgent组件
    private NavMeshAgent agent;
    //划线组件
    private LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        //初始化
        agent = GetComponent<NavMeshAgent>();
        lineRenderer = GetComponent<LineRenderer>();

        if( lineRenderer != null)
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
        // 获取 LineRenderer 的材质
        Material lineMat = lineRenderer.material;
        // 让贴图的 X 轴偏移量随时间变化
        // "_MainTex" 是标准 Shader 的主纹理属性名
        lineMat.SetTextureOffset("_MainTex", new Vector2(-Time.time * 2.0f, 0));

        if ( Input.GetMouseButtonDown(0))
        {
            //获取射线
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //碰撞信息
            RaycastHit hitInfo;

            //发出射线
            if(Physics.Raycast(ray, out hitInfo))
            {
                //调用NavMeshAgent的Api,设置目的地
                //agent自动计算A*路径并移动
                agent.SetDestination(hitInfo.point);
            }
        }

        //绘制移动轨迹
        if( lineRenderer != null && agent.hasPath)
        {
            lineRenderer.enabled = true;

            //获取agent计算出的所有拐点
            Vector3[] corners = agent.path.corners;

            //告诉lineRenderer一共包含多少个拐点
            lineRenderer.positionCount = corners.Length;

            //遍历每个点，设置给lineRenderer
            for(int i = 0; i < corners.Length; i++)
            {
                Vector3 elevatedPoint = new Vector3(corners[i].x, 0.1f, corners[i].z);
                lineRenderer.SetPosition(i,elevatedPoint);
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
}
