using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MeleeScan", menuName = "Skills/Strategies/Melee Scan")]
public class MeleeScanStrategy : SkillStrategy
{
    [Header("近战配置")]
    public int damage = 10;
    public LayerMask attackLayer;                 //攻击对象
    public GameObject hitEff;               //攻击特效

    //攻击距离
    public float attackRadius = 1.5f;       

    public override void Cast(Transform caster, Transform target, Vector3 point)
    {
        //近战攻击逻辑，在挥刀的那一瞬间，在身前画一个检测球（或者盒子），谁在里面谁挨打
        //确认攻击中心点
        Vector3 origin = caster.position + caster.forward * 1.0f;

        //物理检测 球形检测
        Collider[] hits = Physics.OverlapSphere(caster.position,attackRadius, attackLayer);
        if (hits.Length > 0 )
        {
            Debug.Log($"挥砍命中了 {hits.Length} 个敌人！");
        }

        foreach( var hit in hits)
        {
            IDamageable damageable = hit.GetComponent<IDamageable>();
            if ( damageable != null )
            {
                damageable.TakeDamage(damage);

                //播放攻击特效
                if( hitEff != null)
                {
                    // 特效生成在敌人的胸口位置，稍微抬高一点
                    GameObject obj = Instantiate(hitEff, hit.transform.position + Vector3.up, Quaternion.identity);
                    Destroy(obj, 2f);
                }
            }
        }
        //这里可以播放挥砍音效
    }
}
