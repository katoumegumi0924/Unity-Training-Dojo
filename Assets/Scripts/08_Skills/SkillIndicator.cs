using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//技能指示器
public class SkillIndicator : MonoBehaviour
{
    private Transform player;

    private void Awake()
    {
        player = transform.parent;
    }

    //显示
    public void Show( float radius)
    {
        gameObject.SetActive(true);

        transform.localScale = new Vector3(radius * 2, radius * 2, 1f);
    }

    //隐藏
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
