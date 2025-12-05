using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponManager : MonoBehaviour
{
    [Header("组件引用")]
    public Transform WeaponSocket;
    private PlayerSkillManager skillManager;

    //运行时状态
    private GameObject currentWeaponModel;              //当前武器模型

    private void Awake()
    {
        skillManager = GetComponent<PlayerSkillManager>();
    }

    //装备武器
    public void EquipWeapon( WeaponItemData newWeapon )
    {
        if (newWeapon == null) return;

        //销毁旧武器模型
        if( currentWeaponModel != null)
        {
            Destroy( currentWeaponModel );
        }

        //生成新武器
        if( newWeapon.weaponModelPrefab != null)
        {
            //创建武器模型
            currentWeaponModel = Instantiate( newWeapon.weaponModelPrefab, WeaponSocket);
            //武器坐标归零
            currentWeaponModel.transform.localPosition = Vector3.zero;
            currentWeaponModel.transform.localRotation = Quaternion.identity;
        }

        //处理技能覆盖逻辑
        if( skillManager !=null && newWeapon.skilloverrides != null)
        {
            //遍历武器的所有技能
            foreach( var overrideData in newWeapon.skilloverrides)
            {
                skillManager.OverrideSkill(overrideData.slotType, overrideData.skillData);
            }
        }
        Debug.Log($"已装备武器：{newWeapon.itemName}");
    }

    // --- 临时测试代码 (按数字键换装) ---
    // 正式版会通过 UI 点击触发，现在先用键盘测逻辑
    [Header("测试用数据")]
    public WeaponItemData testSword;
    public WeaponItemData testStaff;

    void Update()
    {
        // 按 1 换剑，按 2 换法杖
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("按下1");
            EquipWeapon(testSword);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2)) EquipWeapon(testStaff);
    }
}
