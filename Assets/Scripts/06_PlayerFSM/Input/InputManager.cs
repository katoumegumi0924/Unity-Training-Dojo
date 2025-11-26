using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    //单例
    public static InputManager Instance { get; private set; }

    // 引用 Unity 自动生成的那个 C# 类
    private GameControls gameControls;

    //对外提供的接口，点击事件
    public event Action OnClick;

    //鼠标位置
    public Vector2 MousePosition => gameControls.Gameplay.MousePosition.ReadValue<Vector2>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            //过场景不销毁
            DontDestroyOnLoad(gameObject);

            //初始化生成的类
            gameControls = new GameControls();
            // --- 绑定事件 ---
            // 当 'Click' 动作发生时(performed)，触发我们的 OnClick 事件
            // ctx 是 context (上下文)，这里我们不需要它，所以忽略
            gameControls.Gameplay.Click.performed += ctx => OnClick?.Invoke();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // --- 重要：开启和关闭 ---
    // New Input System 的对象必须手动 Enable 才能工作！
    private void OnEnable()
    {
        if (gameControls != null)
            gameControls.Enable();
    }

    private void OnDisable()
    {
        if (gameControls != null)
            gameControls.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        // 测试输入
        InputManager.Instance.OnClick += () => Debug.Log("鼠标点击了！位置：" + InputManager.Instance.MousePosition);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
