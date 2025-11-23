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
                InventoryManager.instance.AddItem(itemData, amount);
            }

            // 3. 视觉反馈：销毁场景里的这个模型
            // (进阶思考：如果是对象池管理的，这里应该 ReturnToPool，这里先简单Destroy)
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
