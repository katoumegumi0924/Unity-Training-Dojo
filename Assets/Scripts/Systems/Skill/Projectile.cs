using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//投射物对象控制脚本
public class Projectile : MonoBehaviour
{
    private float speed;            //技能投射物飞行速度
    private int damage;             //技能伤害
    private Transform target;       //技能追踪目标，如果有
    private Vector3 flyDirection;   //技能投射物飞行方向
    private GameObject hitFXPrefab;       //技能击中特效

    //初始化方法
    public void Setup( Transform target, int damage, float speed, GameObject hitFXPrefab)
    {
        this.target = target;
        this.damage = damage;   
        this.speed = speed;
        this.hitFXPrefab = hitFXPrefab;

        if( target != null)
        {
            //有目标，技能飞向目标    ?
            flyDirection = ( target.position - transform.position ).normalized;
        }
        else
        {
            //没有目标，飞向前方
            flyDirection = transform.forward;
        }

        //五秒后自动销毁
        Destroy( gameObject, 5f );
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //追踪逻辑
        if( target != null)
        {
            //实时修正飞行方向
            flyDirection = (target.position - transform.position).normalized;
        }

        //移动    ?
        transform.Translate(flyDirection * speed * Time.deltaTime, Space.World);

        //朝向飞行方向    ?
        if(flyDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(flyDirection);
        }
    }

    //碰撞逻辑
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"<color=green> 碰撞成功 </color>");

        // 1. 尝试获取受伤接口 
        var damageable = other.GetComponent<IDamageable>();

        //如果碰撞到敌人或者墙体
        if (damageable != null || other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            if( hitFXPrefab != null)
            {
                //播放碰撞特效
                GameObject effectInstance = Instantiate(hitFXPrefab, transform.position, Quaternion.identity);
                Destroy(effectInstance, 5);
            }

            //造成伤害
            if( damageable != null)
            {
                damageable.TakeDamage(damage);
            }
            
            //销毁投射物对象
            //Destroy(gameObject);
        }
    }
}
