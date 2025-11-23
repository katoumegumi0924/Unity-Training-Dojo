using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    //单例模式
    public static GameManager instance { get; private set; }

    private void Awake()
    {
        //初始化
        if(instance == null)
        {
            instance = this;
            //过场景不销毁
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //UI引用
    public TMP_Text scoreText;
     
    //内部分数数据
    private int score;

    private void Start()
    {
        //初始化显示
        UpdateUI();
    }

    //结合观察者模式 计算分数
    private void OnEnable()
    {
        GameEvents.OnArrived += AddScore;
    }

    private void OnDisable()
    {
        GameEvents.OnArrived -= AddScore;
    }

    //业务逻辑
    private void AddScore()
    {
        score++;
        Debug.Log($"[GameManager] 得分! 当前分数: {score}");
        UpdateUI();
    }

    private void UpdateUI()
    {
        if(scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
    }

}
