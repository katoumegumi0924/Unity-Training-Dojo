using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public Transform slotParent;    // 格子的父物体 (也就是 Panel 自己)
    public GameObject slotPrefab;   // 格子预制体

    // Start is called before the first frame update
    void Start()
    {
        //订阅数据更新事件
        InventoryManager.instance.OnInventoryChanged += RefreshUI;

        RefreshUI();
    }

    private void OnDestroy()
    {
        if( InventoryManager.instance != null)
        {
            InventoryManager.instance.OnInventoryChanged -= RefreshUI;
        }   
    }

    //核心逻辑：刷新UI界面
    void RefreshUI()
    {
        // 1. 先把旧的格子全删了 (暴力刷新法，简单粗暴)
        // (面试进阶：可以用对象池优化这里，但初级阶段先 Destroy)
        foreach( Transform child in slotParent)
        {
            Destroy(child.gameObject);
        }

        //2. 遍历背包数据，生成新格子
        foreach (InventorySlot slotData in InventoryManager.instance.backpack )
        {
            //生成格子
            GameObject newSlot = Instantiate(slotPrefab, slotParent);

            // 获取 UI 组件并赋值
            InventorySlotUI ui = newSlot.GetComponent<InventorySlotUI>();
            if (ui != null)
            {
                ui.SetItem(slotData.itemData, slotData.amount);
            }
        }
    }
}
