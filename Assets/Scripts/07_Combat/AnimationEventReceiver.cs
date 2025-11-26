using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventReceiver : MonoBehaviour
{
    // 用来持有父物体的控制器引用
    private PlayerController playerCtrl;
    private EnemyController enemyCtrl;

    // Start is called before the first frame update
    void Start()
    {
        //尝试获取父物体上的控制器对象
        playerCtrl = GetComponentInParent<PlayerController>();
        enemyCtrl = GetComponentInParent<EnemyController>();
    }

    //要绑定的事件方法 ??
    public void OnAttackHit()
    {
        // 如果是玩家，通知玩家
        if (playerCtrl != null)
        {
            // 这里需要去 PlayerController 里写一个公开方法来处理
            //playerCtrl.OnAnimationAttackHit();
        }

        // 如果是敌人，通知敌人
        if (enemyCtrl != null)
        {
            Debug.Log("Attack");
        }
    }
}
