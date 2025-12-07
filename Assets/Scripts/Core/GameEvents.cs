using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameEvents 
{
    //一个静态事件 到达目的地时触发
    public static event Action OnArrived;
    //封装触发方法
    public static void TriggerArrived()
    {
        OnArrived?.Invoke();
    }

    //当物品被捡起时触发 (参数：物品世界坐标, 物品数据)
    public static event Action<Vector3, ItemData> OnItemPickedUp;
    public static void TriggerItemPickedUp(Vector3 pos, ItemData item)
    {
        OnItemPickedUp?.Invoke(pos, item);
    }
}
