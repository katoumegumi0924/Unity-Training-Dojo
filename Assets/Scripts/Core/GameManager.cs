using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    //单例模式
    public static GameManager instance { get; private set; }

    // 新增：全局唯一的玩家引用
    // set 是私有的，防止别人乱改；get 是公开的，供大家查询
    public Transform Player {  get; private set; }

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

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if(playerObj != null)
        {
            Player = playerObj.transform;
        }
        else
        {
            Debug.LogError("GameManager: 场景里找不到 Tag 为 Player 的物体！");
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
        //Debug.Log($"[GameManager] 得分! 当前分数: {score}");
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
