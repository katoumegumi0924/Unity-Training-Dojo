using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    //动画组件
    private Animator anim;

    //字符串转HashID 性能优化
    //// static readonly 保证全游戏只算一次，极快
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int DieHash = Animator.StringToHash("IsDead");
    private static readonly int HurtHash = Animator.StringToHash("Hurt");


    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    //封装方法，对外部只提供行为，不暴露参数名
    // 增加 dampTime 参数，默认值给 0.1f
    public void SetMoveSpeed(float velocity, float dampTime = 0.1f)
    {
        // 如果 dampTime 是 0，就不用 damp 版本的方法，或者传 0 进去
        if (dampTime <= 0)
        {
            anim.SetFloat(SpeedHash, velocity); // 瞬变！
        }
        else
        {
            anim.SetFloat(SpeedHash, velocity, dampTime, Time.deltaTime); // 渐变
        }
    }

    public void TriggerAttack()
    {
        anim.SetTrigger(AttackHash);
    }

    public void TriggerHurt()
    {
        anim.SetTrigger(HurtHash);
    }

    public void TriggleSkill(string skillName)
    {
        anim.SetTrigger( skillName );
    }

    public void SetBoolDie( bool isDead )
    {
        anim.SetBool(DieHash, isDead);
    }

    // 如果需要由动画事件调用
    public void OnAnimationEvent_Hit()
    {
        // 通知 Controller...
    }
}
