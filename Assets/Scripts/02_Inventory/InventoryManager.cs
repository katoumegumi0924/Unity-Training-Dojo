using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// --- 1. 定义背包格子类 ---
[System.Serializable]
public class InventorySlot
{
    public ItemData itemData;           //存那种物品 
    public int amount;                  //存了多少个

    //构造函数
    public InventorySlot( ItemData data, int count)
    {
        itemData = data;
        amount = count;
    } 

    //增加数量
    public void AddAmount( int count )
    {
        amount += count;
    }
}

// --- 2. 背包管理器 ---
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //定义一个事件：当背包数据发生变化时触发
    public event Action OnInventoryChanged;

    //背包容器，就是一个格子的列表
    //使用列表方便有序的显示
    public List<InventorySlot> backpack = new List<InventorySlot>();

    
    //核心逻辑；添加物品
    public void AddItem(ItemData data, int count)
    {
        //检查背包中是否存在该物品
        //只有可堆叠的物品才需要检查
        InventorySlot foundSlot = null;

        
        if ( data.isStackable )
        {
            //遍历List查找
            foundSlot = backpack.Find( slot => slot.itemData = data );
        }

        //如果该物品已存在
        if ( foundSlot != null )
        {
            //物品已存在且不可堆叠，直接增加数量
            foundSlot.AddAmount( count );
            Debug.Log($"[背包] {data.itemName} 数量增加，当前: {foundSlot.amount}");
        }
        else
        {
            //物品不存在或者不可堆叠，创建一个新的格子
            InventorySlot newSlot = new InventorySlot( data, count );
            backpack.Add( newSlot );
            Debug.Log($"[背包] 获得新物品: {data.itemName} x{count}");
        }

        // 通知 UI：数据变啦，快刷新！
        OnInventoryChanged?.Invoke();
    }

    // 仅用于测试，正式上线要删掉
    public ItemData testItem; // 把你做好的 ScriptableObject 拖进这里

    private void Update()
    {
        if( Input.GetKeyDown(KeyCode.Space))
        {
            AddItem(testItem, 1);
        }
    }
}
