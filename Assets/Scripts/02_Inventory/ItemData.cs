using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("物品基础信息")]
    public int itemID;          // 唯一ID (用于字典查找)
    public string itemName;     // 名字
    public Sprite icon;         // 图标 (UI显示用)
    public ItemType type;       //物品类型

    [TextArea]
    public string description;  // 描述

    [Header("物品属性")]
    public bool isStackable;    // 是否可堆叠
    public int maxStackSize = 1; // 最大堆叠数
}
    
public enum ItemType { Consumble, Weapon, Material}