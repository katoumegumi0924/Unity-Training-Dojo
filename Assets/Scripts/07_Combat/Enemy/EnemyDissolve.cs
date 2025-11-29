using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.AI;

[RequireComponent(typeof(HealthController))]
public class EnemyDissolve : MonoBehaviour
{
    private HealthController health;
    // 数组，防止怪物有多个部件(头、身、武器)
    private Renderer[] renderers;

    public float dissolveDuration = 2f;

    private void Awake()
    {
        health = GetComponent<HealthController>();
        // 获取所有子物体的渲染器（比如盔甲、剑、身体可能是分开的）
        renderers = GetComponentsInChildren<Renderer>();
    }

    private void OnEnable()
    {
        health.OnDeath += DisablePhysics;
    }

    private void OnDisable()
    {
        health.OnDeath -= DisablePhysics;
    }

    void DisablePhysics()
    {
        var collider = GetComponent<Collider>();
        if (collider) collider.enabled = false;

        var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent) agent.enabled = false;

        // 注意：这里不再启动协程，等待动画事件来呼叫
    }

    public void StartDissoloveEffect()
    {
        StartCoroutine(DissolveProcess());
    } 

    IEnumerator DissolveProcess()
    {
        //开始溶解
        foreach (var r in renderers)
        {
            //确保材质中有"_DissolveAmount"属性
            if (r.material.HasProperty("_DissolveAmount"))
            {
                r.material.DOFloat(1f, "_DissolveAmount", dissolveDuration);
            }
        }

        // 等待溶解结束
        yield return new WaitForSeconds(dissolveDuration);

        // 销毁
        Destroy(gameObject);
    }
    
}
