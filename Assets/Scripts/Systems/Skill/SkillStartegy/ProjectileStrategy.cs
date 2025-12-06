using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewProjectileStrategy", menuName = "Skills/Strategies/Projectile")]
public class ProjectileStrategy : SkillStrategy
{
    public override void Cast(Transform caster, Transform target, Vector3 point, SkillData data)
    {
        //计算发射点
        // TransformPoint: 把局部坐标 (相对于发射者) 转为 世界坐标
        Vector3 spawnPos = caster.TransformPoint(data.spawnOffset);

        // --- 计算方向逻辑 ---
        Quaternion rotation;

        if (target != null)
        {
            // A. 有锁定目标：朝向目标
            Vector3 dir = (target.position + Vector3.up - spawnPos).normalized;
            rotation = Quaternion.LookRotation(dir);
        }
        else
        {
            // B. 无锁定目标：朝向鼠标点击点 (point) 
            // 忽略 Y 轴高度差异
            Vector3 targetPos = point;
            targetPos.y = spawnPos.y;

            Vector3 dir = (targetPos - spawnPos).normalized;

            // 防御：如果点击点太近导致 dir 为零
            if (dir == Vector3.zero) dir = caster.forward;

            rotation = Quaternion.LookRotation(dir);
        }

        //生成投射物
        GameObject obj = Instantiate(data.skillEff, spawnPos, caster.rotation);

        //把数据传递给投射物对象
        Projectile p = obj.GetComponent<Projectile>();
        if (p != null)
        {
            p.Setup(target, data.damage, data.flySpeed, data.hitEff  );
        }
    }
}
