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
}
