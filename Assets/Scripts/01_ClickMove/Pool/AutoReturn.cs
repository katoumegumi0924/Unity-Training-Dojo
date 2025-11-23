using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoReturn : MonoBehaviour
{
    public float lifetime = 1.0f;

    private void OnEnable()
    {
        //一秒后 自动归还对象池对象
        Invoke(nameof(ReturnToPool), lifetime);
    }

    void ReturnToPool()
    {
        //自动归还对象池对象
        FXManager.instance.ReturnEffect(this.gameObject);
    }

    // 如果还没到时间就被强制关掉了，要取消倒计时，防止报错
    private void OnDisable()
    {
        CancelInvoke();
    }
}
