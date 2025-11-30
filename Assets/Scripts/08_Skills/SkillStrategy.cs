using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 继承 ScriptableObject，这样每个技能逻辑都可以是一个单独的文件
public abstract class SkillStrategy : ScriptableObject
{
    // 定义一个抽象方法：释放技能
    // caster: 施法者（谁放的？）
    // target: 目标（打谁？可能是空）
    public abstract void Cast(Transform caster, Transform target);
}
