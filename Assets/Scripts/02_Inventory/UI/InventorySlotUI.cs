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

    //设置格子内容
    public void SetItem( ItemData data, int amount )
    {
        iconImage.sprite = data.icon;
        amountText.text = amount.ToString();

        // 如果图标为空，可以隐藏 Image (可选优化)
        if (data.icon == null) iconImage.enabled = false;
        else iconImage.enabled = true;
    }
}
