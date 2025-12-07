using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//背包格子UI
public class InventorySlotUI : MonoBehaviour
{
    public Image iconImage;
    public TMP_Text amountText;

    //自身的Button组件
    private Button button;

    //当前各自的数据
    private ItemData currentItem;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void Start()
    {
        if( button!=null)
        {
            //监听点击
            button.onClick.AddListener(OnSlotClicked);
        }
    }

    //设置格子内容
    public void SetItem( ItemData data, int amount )
    {
        //保存当前格子数据
        currentItem = data;

        iconImage.sprite = data.icon;
        amountText.text = amount.ToString();

        // 如果图标为空，可以隐藏 Image (可选优化)
        if (data.icon == null) iconImage.enabled = false;
        else iconImage.enabled = true;
    }

    //处理点击格子的逻辑
    private void OnSlotClicked()
    {
        if( currentItem != null)
        {
            //通知UI管理器处理逻辑
            InventoryManager.instance.OnItemClicked(currentItem); 
        }
    }
}
