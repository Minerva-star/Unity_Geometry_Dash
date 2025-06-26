using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    // 移动速度
    public float moveSpeed = 4f;
    
    // 销毁位置
    public float destroyXPosition = -15f;
    
    // 是否暂停
    private bool isPaused = false;
    
    private void Start()
    {
        // 确保有碰撞器
        if (GetComponent<Collider2D>() == null)
        {
            BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
        }
        
        // 设置标签
        gameObject.tag = "Obstacle";
        gameObject.tag = "Obstacle";
        //章章 是 sb
    }
    
    private void Update()
    {
        if (!isPaused)
        {
            // 移动障碍物
            transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
            
            // 检查是否需要销毁
            if (transform.position.x <= destroyXPosition)
            {
                Destroy(gameObject);
            }
        }
    }
    
    // 设置移动速度
    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }
    
    // 暂停/恢复移动
    public void SetPaused(bool paused)
    {
        isPaused = paused;
    }
    
    // 当障碍物被销毁时
    private void OnDestroy()
    {
        // 可以在这里添加销毁特效
    }
} 