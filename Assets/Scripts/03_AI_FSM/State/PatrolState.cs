using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//巡逻状态
public class PatrolState : IState
{
    //持有状态机管理器的引用 用于自动寻路和切换状态
    private EnemyController manager;
    // 计时器，防止它不停地换目标
    private float wanderTimer;

    //构造函数
    public PatrolState( EnemyController manager )
    {
        this.manager = manager;
    }

    public void OnEnter()
    {
        //设置巡逻速度
        manager.agent.speed = manager.patrolSpeed;
        Debug.Log("进入状态：巡逻 (Patrol)");
        MoveToRandomPos();
    }

    public void OnExit()
    {
        //throw new System.NotImplementedException();
    }

    public void OnUpdate()
    {
        //巡逻逻辑
        //到达目的地 ， 短暂停留后去下一个点
        if( manager.agent.remainingDistance < 0.5f)
        {
            wanderTimer += Time.deltaTime;
            //停留一秒后前往下一个巡逻点
            if(wanderTimer > 1.0f)
            {
                MoveToRandomPos();
                wanderTimer = 0.0f;
            }
        }


        //切换逻辑
        //检测玩家距离
        float dis = Vector3.Distance(manager.transform.position, GameManager.instance.Player.position);

        if( manager.detectRange > dis)
        {
            //发现玩家，切换到追击状态
            manager.TransitionToState(manager.ChaseState);
        }
    }

    //辅助方法，随机找一个半径5范围内的点
    private void MoveToRandomPos()
    {
        //随机找一个半径5m内的点
        Vector3 randomPoint = Random.insideUnitSphere * 5f + manager.transform.position;

        // 这里的 SamplePosition 是为了保证随机点一定在 NavMesh 地面上，防止点到墙里
        UnityEngine.AI.NavMeshHit hit;
        UnityEngine.AI.NavMesh.SamplePosition(randomPoint, out hit, 5f, 1);

        manager.agent.SetDestination(hit.position);
    }
}
