using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic; // 必须引入这个，因为我们要用 Queue<T>

public class CommandMove : MonoBehaviour
{
    private NavMeshAgent agent;
    private LineRenderer lineRenderer;

    // 1. 定义一个队列，用来存目标点 (Vector3)
    // 提示：Queue<Vector3>
    private Queue<Vector3> moveQueue = new Queue<Vector3>();

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        lineRenderer = GetComponent<LineRenderer>();

        // 这次我们把自动刹车距离调小一点，让它走得准一点
        agent.stoppingDistance = 0.1f;
    }

    void Update()
    {
        // --- 部分 A：生产者 (Producer) ---
        // 负责监听输入，把任务加入队列
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo))
            {
                // 核心操作：入队 (Enqueue)
                // 把点击的位置加入队列，但先不移动
                moveQueue.Enqueue(hitInfo.point);

                // (可选) 可以在这里生成一个小球特效，标记一下点击的位置
            }
        }

        // --- 部分 B：消费者 (Consumer) ---
        // 负责检查状态，如果闲下来了，就从队列取任务
        CheckAndMove();

        // --- 部分 C：画线 ---
        DrawPath();
    }

    void CheckAndMove()
    {
        // 1. 检查是否正在移动中
        // 如果 agent 还有路径没走完 (remainingDistance > 0.2f)，那就别打扰它，直接 return
        // 注意：要额外判断 pathPending，防止计算路径还没算完就误判为到了
        if (!agent.pathPending && agent.remainingDistance > 0.2f)
        {
            return;
        }

        // 2. 如果闲下来了，检查队列里还有没有任务？
        if (moveQueue.Count > 0)
        {
            // 核心操作：出队 (Dequeue)
            // 取出队头的下一个目标点
            Vector3 nextTarget = moveQueue.Dequeue();

            // 让 Agent 走过去
            agent.SetDestination(nextTarget);
        }
    }

    void DrawPath()
    {
        if (lineRenderer == null) return;

        //临时列表，存所有的点
        List<Vector3> allPoints = new List<Vector3>();

        //第一段：正在走的路径的点
        if (agent.hasPath)
        {
            //把agent计算出的所有拐点存入allPoints
            allPoints.AddRange(agent.path.corners);
        }
        else
        {
            //如果还没开始走，存入当前所在位置
            allPoints.Add(transform.position);
        }

        //第二段：队列里排队的点
        if (moveQueue.Count > 0)
        {
            allPoints.AddRange(moveQueue);
        }

        //赋值给lineRenderer
        lineRenderer.positionCount = allPoints.Count;

        for (int i = 0; i < allPoints.Count; i++)
        {
            lineRenderer.SetPosition(i, allPoints[i] + Vector3.up * 0.1f);
        }
    }
}