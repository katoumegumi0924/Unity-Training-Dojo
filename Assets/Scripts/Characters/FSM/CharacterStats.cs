using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewStats", menuName = "Stats/Character Stats")]
public class CharacterStats : ScriptableObject
{
    [Header("基础属性")]
    public int maxHealth = 100;
    public float moveSpeed = 5f;
    public float turnSpeed = 360f;

    [Header("战斗属性")]
    public float attackRange = 2f;
    public float attackInterval = 1.0f;     //攻速
    public int damage = 10;                 //攻击力

    [Header("AI感知(仅敌人生效)")]
    public float viewAngle = 90f;                // 视野角度
    public float detectRange = 5.0f;             //摄影距离
    public float chaseStopDistance = 2.0f;      //停止追击，进入攻击范围
    public float chaseResumeDistance = 3.0f;    //超出攻击范围，继续追击

    public float chaseSpeed = 5f;               //追击速度，只有敌人生效
}
