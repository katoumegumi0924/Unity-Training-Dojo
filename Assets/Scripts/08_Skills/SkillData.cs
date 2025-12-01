using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//技能静态数据，通过ScriptableObject配置
[CreateAssetMenu(fileName = "NewSkill", menuName ="Skills/Skill Data")]
public class SkillData : ScriptableObject
{
    [Header("基本信息")]
    public string skillName;
    public Sprite icon;
    [TextArea] public string description;

    [Header("战斗参数")]
    public float cooldown = 2.0f;       //冷却时间
    public float manaCost = 10;         //蓝耗
    public float castDuration = 1f;     //施法时间，期间玩家无法移动
    public float damageDelay = 0.5f;    //施法前摇时间

    [Header("表现")]
    public string animTriggerName;

    [Header("核心逻辑 (插卡带)")]
    // 这就是策略模式的精髓！
    // 我们不在这里写逻辑，而是持有一个逻辑策略的引用
    public SkillStrategy strategy;


}
