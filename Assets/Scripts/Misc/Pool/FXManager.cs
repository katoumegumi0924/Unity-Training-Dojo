using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXManager : MonoBehaviour
{
    public static FXManager instance { get; private set; }
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //特效预制体
    public GameObject fxPrefab;
    //初始对象池大小
    public int poolSize = 10;

    //使用队列池子存储空闲的对象
    private Queue<GameObject> poolQueue = new Queue<GameObject> ();
    private Stack<GameObject> poolStack = new Stack<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        //初始化，生成对象池中的内容
        for(int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(fxPrefab, transform);
            obj.SetActive(false);
            //poolQueue.Enqueue(obj);
            poolStack.Push(obj);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //取出对象池中的对象进行展示
    public void ShowEffect( Vector3 pos )
    {
        GameObject obj;

        //检测对象池中是否还有空闲对象
        if( poolStack.Count > 0)
        {
            //还有对象可以直接取出来使用
            //obj = poolQueue.Dequeue();  
            obj = poolStack.Pop();
        }
        else
        {
            //没有空闲对象了，创建一个新的
            obj = Instantiate(fxPrefab, transform);
        }

        //设置特效位置并激活
        obj.transform.position = pos;
        obj.SetActive(true);
    }

    //把对象放回池中
    public void ReturnEffect( GameObject obj )
    {
        obj.SetActive(false);       //禁用
        //poolQueue.Enqueue(obj);     //放回队尾
        poolStack.Push (obj);
    }
}
