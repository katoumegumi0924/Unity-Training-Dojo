using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickMove : MonoBehaviour
{
    //需要移动的物体
    public Transform Cube;
    public float moveSpeed = 5f;

    //摄像机
    private Camera mainCamera;

    //目标位置
    private Vector3 targetPosition;

    //是否需要移动
    private bool isMoving;

    // 引用画线组件
    private LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        //获取主摄像机
        mainCamera = Camera.main;

        //初始化目标位置为当前位置
        targetPosition = Cube.position;

        //获取划线组件
        lineRenderer = Cube.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if ( Input.GetMouseButtonDown(0) )
        {
            //制造一条从屏幕点击点发出的射线
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            //存储碰撞结果
            RaycastHit hitInfo;
            
            if( Physics.Raycast(ray, out hitInfo))
            {
                //设置移动的目标位置
                targetPosition = hitInfo.point;

                //修正y轴高度
                targetPosition.y = Cube.transform.position.y;

                isMoving = true;
            }
        }

        //处理移动
        if( isMoving && Cube != null)
        {
            Cube.position = Vector3.MoveTowards(Cube.position, targetPosition, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(Cube.position, targetPosition) < 0.01f)
            {
                isMoving = false;
            }
        }

        //画出移动轨迹
        if( lineRenderer != null)
        {
            //让轨迹紧贴地面
            Vector3 startPos = new Vector3(Cube.position.x, 0.1f, Cube.position.z);
            Vector3 endPos = new Vector3(targetPosition.x, 0.1f, targetPosition.z);

            //设置起点
            lineRenderer.SetPosition(0, startPos);
            //设置终点
            lineRenderer.SetPosition(1, endPos);

            //只有在移动时才画线
            lineRenderer.enabled = isMoving;
        }
    }
}
