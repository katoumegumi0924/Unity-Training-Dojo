    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [Header("配置")]
    public ItemData itemData;       //是什么？由拖入ScriptableObject决定
    public int amount = 1;              //有多少个？

    //--- 交互逻辑：碰撞触发 ---
    private void OnTriggerEnter(Collider other)
    {
        //只有玩家才能捡起物品
        if( other.CompareTag("Player"))
        {
            //调用管理器添加物品到背包
            if( InventoryManager.instance != null)
            {
                //添加数据，静默模式
                InventoryManager.instance.AddItem(itemData, amount, false);

                //发出广播，拾取了物品
                //UI自己处理相关逻辑
                GameEvents.TriggerItemPickedUp(transform.position, itemData);
            }
         Destroy(gameObject);

            Debug.Log($"捡起了 {itemData.itemName} x{amount}");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
