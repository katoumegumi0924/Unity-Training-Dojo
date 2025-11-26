using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class AnimatedInventoryUI : MonoBehaviour
{
    public static AnimatedInventoryUI Instance { get; private set; }



    public Transform slotParent;
    public GameObject slotPrefab;

    public RectTransform panelRect;
    private bool isOpen = false;

    //飞行图标预制体
    public GameObject flyIconPrefab;
    //背包按钮位置
    public RectTransform bagButton;

    private void Awake()
    {
        Instance = this; // 简单单例
        panelRect = GetComponent<RectTransform>();
    }



    // Start is called before the first frame update
    void Start()
    {
        //订阅UI刷新事件
        InventoryManager.instance.OnInventoryChanged += RefreshUI;
        RefreshUI();

        // --- 初始化：瞬间隐藏 ---
        // 把缩放设为 (0, 0, 0)，这样一开始就看不见了
        panelRect.localScale = Vector3.zero;
        isOpen = false;
    }

    void OnDestroy()
    {
        if (InventoryManager.instance != null)
            InventoryManager.instance.OnInventoryChanged -= RefreshUI;
    }

    // Update is called once per frame
    void Update()
    {
        //按下b键，开关背包
        if ( Input.GetKeyDown(KeyCode.B) )
        {
            TogglePanel();
        }
    }

    //开关背包的动态动画
    private void TogglePanel()
    {
        // 防止玩家手速太快，上一个动画还没播完就按键
        // Complete() 会强制立刻完成当前动画，确保状态对齐
        panelRect.DOKill();

        if (isOpen)
        {
            // --- 关闭动画 ---
            // 目标：缩放到 0
            // 时间：0.3 秒
            // 曲线：InBack (先往外鼓一下，再缩回去，像气球漏气)
            panelRect.DOScale(0f, 0.3f).SetEase(Ease.InBack);
            isOpen = false;
        }
        else
        {
            // --- 打开动画 ---
            // 目标：缩放到 1 (正常大小)
            // 时间：0.5 秒
            // 曲线：OutBack (弹出来，稍微超过一点再弹回来，像果冻)
            panelRect.DOScale(1f, 0.5f).SetEase(Ease.OutBack);
            isOpen = true;
        }
    }

    //播放物体飞行动画
    public void PlayFlyAnimation(Vector3 worldPos, Sprite itemIcon)
    {
        // 1. 世界坐标 -> 屏幕坐标
        // 这一步是为了让 UI 图标刚好出现在 3D 物体的位置
        Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        // 2. 生成飞行图标 设置飞行图标位置
        GameObject flyIcon = Instantiate(flyIconPrefab, transform.parent);
        flyIcon.transform.position = screenPos;

        //设置图标图片
        flyIcon.GetComponent<UnityEngine.UI.Image>().sprite = itemIcon;

        //飞行动画
        flyIcon.transform
            .DOMove(bagButton.position, 0.8f)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                //立刻完成bagButto动画并停在终点（复位到原点）,避免短时间捡起多个物体引起抽搐
                bagButton.DOKill(true);

                Destroy(flyIcon);

                bagButton.DOShakeAnchorPos(0.2f, 10);

                
                //动画播放完成之后刷新UI
                RefreshUI();
            } );

        // (可选) 同时慢慢变小
        //flyIcon.transform.DOScale(0.5f, 0.8f);
    }

    // ... RefreshUI 方法  ...
    void RefreshUI()
    {
        //获取当前的背包数据
        var dataList = InventoryManager.instance.backpack;

        //获取当前 UI 面板下所有的子物体数量 和 背包数据中的子物体数量
        int uiCount = slotParent.childCount;
        int dataCount = dataList.Count;

        // --- 阶段 A：数据比格子多，不够用，造新的 ---
        if( dataCount > uiCount)
        {
            int needToAdd = dataCount - uiCount;
            for ( int i = 0; i < needToAdd; i++ )
            {
                Instantiate(slotPrefab, slotParent);
            }
        }
        // --- 阶段 B：数据比格子少，多余的，删掉 ---
        else if( dataCount < uiCount)
        {
            int needToRemove = uiCount - dataCount;
            for( int i = 0;i < needToRemove; i++ )
            {
                Destroy(slotParent.GetChild( uiCount - i - 1 ).gameObject);
            }
        }

        // 此时，UI格子数量一定等于数据数量
        // --- 阶段 C：统一刷新内容 (这是最省性能的，只改属性) ---
        for ( int i = 0; i<dataCount; i++)
        {
            var uiItem = slotParent.GetChild(i).GetComponent<InventorySlotUI>();
            var dataItem = dataList[i];

            //刷新显示
            if( uiItem != null)
            {
                uiItem.SetItem(dataItem.itemData, dataItem.amount);
            }
        }
    }
}
