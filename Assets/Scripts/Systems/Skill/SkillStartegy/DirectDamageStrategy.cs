using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 这是一个具体的“卡带”，是一个具体的技能
[CreateAssetMenu(fileName = "DirectDamage", menuName = "Skills/Strategies/Direct Damage")]
public class DirectDamageStrategy : SkillStrategy
{
    //实现父类的抽象方法
    public override void Cast(Transform caster, Transform target, Vector3 point, SkillData data)
    {
        if(target == null) return;

        //距离校验，确定在技能范围之内
        float dist = Vector3.Distance(caster.position, target.position);
        if( dist > data.castRange)
        {
            Debug.Log("距离太远，技能未命中！");
            return; // 距离不够，打不到
        }
        

        //扣血逻辑
        IDamageable damageable = target.GetComponent<IDamageable>();
        if(damageable != null)
        {
            damageable.TakeDamage(data.damage);
            Debug.Log($"{caster.name} 使用技能攻击了 {target.name}，造成 {data.damage} 点伤害！");
        }

        // 2. 播放特效
        if (data.skillEff != null)
        {
            // 这里以后可以用对象池优化，现在先 Instantiate，特效位置也需要优化，玩家应该有一个属性可以获取玩家脚下的位置
            GameObject obj = Instantiate(data.skillEff, target.position + Vector3.down, Quaternion.identity);
            Destroy(obj,2);
        }
    }
}
