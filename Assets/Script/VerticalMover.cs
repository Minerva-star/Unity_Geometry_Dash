using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalMover : MonoBehaviour
{
    [Header("移动设置")]
    public bool moveOnStart = true;        // 是否在开始时移动
    public bool isPaused = false;          // 是否暂停
    public float waitTime = 1f;            // 等待时间
    
    [Header("移动参数")]
    public float moveSpeed = 2f;           // 移动速度
    public float moveDistance = 5f;        // 移动距离
    
    [Header("移动方向")]
    public bool moveUpFirst = true;        // 是否先向上移动
    public bool loopMovement = true;       // 是否循环移动
    
    [Header("移动类型")]
    public bool useSineWave = false;       // 是否使用正弦波移动（平滑）
    
    private Vector3 startPosition;
    private Vector3 upPosition;
    private Vector3 downPosition;
    private bool isMovingUp;
    private float currentTime = 0f;

    private void Start()
    {
        startPosition = transform.position;
        
        // 计算上下位置
        upPosition = startPosition + Vector3.up * moveDistance;
        downPosition = startPosition + Vector3.down * moveDistance;
        
        // 设置初始移动方向
        isMovingUp = moveUpFirst;
        
        if (moveOnStart)
        {
            StartCoroutine(MoveUpAndDown());
        }
    }

    IEnumerator MoveUpAndDown()
    {
        while (loopMovement)
        {
            if (!isPaused)
            {
                // 向上移动
                if (isMovingUp)
                {
                    yield return StartCoroutine(MoveToPosition(upPosition));
                    isMovingUp = false;
                }
                // 向下移动
                else
                {
                    yield return StartCoroutine(MoveToPosition(downPosition));
                    isMovingUp = true;
                }
                
                // 等待时间
                if (waitTime > 0)
                {
                    yield return new WaitForSeconds(waitTime);
                }
            }
            else
            {
                yield return null;
            }
        }
    }

    IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            if (!isPaused)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            }
            yield return null;
        }
    }

    private void Update()
    {
        // 正弦波移动（可选）
        if (useSineWave && !isPaused)
        {
            currentTime += Time.deltaTime * moveSpeed;
            float sineValue = Mathf.Sin(currentTime) * moveDistance;
            transform.position = startPosition + Vector3.up * sineValue;
        }
    }

    // 公共方法
    public void StartMovement()
    {
        if (!loopMovement)
        {
            StartCoroutine(MoveUpAndDown());
        }
    }

    public void StopMovement()
    {
        isPaused = true;
    }

    public void ResumeMovement()
    {
        isPaused = false;
    }

    public void ResetPosition()
    {
        transform.position = startPosition;
        currentTime = 0f;
    }

    // 在Scene视图中显示移动范围
    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(startPosition, Vector3.one);
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(upPosition, Vector3.one);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(downPosition, Vector3.one);
        }
        else
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, Vector3.one);
            Gizmos.DrawWireCube(transform.position + Vector3.up * moveDistance, Vector3.one);
            Gizmos.DrawWireCube(transform.position + Vector3.down * moveDistance, Vector3.one);
        }
    }
}
