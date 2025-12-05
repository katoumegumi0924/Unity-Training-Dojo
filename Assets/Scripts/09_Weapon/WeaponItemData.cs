using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon",menuName = "Inventory/Weapon Item Data")]
public class WeaponItemData : ItemData
{
    [Header("武器特有属性")]
    public GameObject weaponModelPrefab;      //武器模型
    public int attackBonus;             //攻击力加成

    [Header("武器技能配置")]
    public List<WeaponSkillOverride> skilloverrides;     //允许一把武器覆盖多个技能

    //在编辑器里自动把 Type 设为 Weapon ？
    private void OnValidate()
    {
        itemType = ItemType.Weapon;
        isStackable = false;            //武器不可堆叠
        maxStackSize = 1;
    }
}

[System.Serializable]
public struct WeaponSkillOverride
{
    public SkillSlotType slotType;      //武器技能覆盖哪个键位？
    public SkillData skillData;         //武器附带的技能数据
}
