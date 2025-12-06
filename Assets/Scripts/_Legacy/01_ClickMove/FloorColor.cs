using UnityEngine;

public class FloorColor : MonoBehaviour
{
    private Material floorMat;

    void Awake()
    {
        //获取地面的材质用于切换颜色
        floorMat = GetComponent<Renderer>().material;
    }

    void OnEnable()
    {
        //订阅事件
        GameEvents.OnArrived += ChangeColor;
    }

    void OnDisable()
    {
        //取消订阅
        GameEvents.OnArrived -= ChangeColor;
    }

    //具体的切换颜色逻辑
    private void ChangeColor()
    {
        //随机生成一个颜色
        Color randomColor = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);

        floorMat.color = randomColor;

        Debug.Log("地板：收到消息，变色成功！");
    }
}
