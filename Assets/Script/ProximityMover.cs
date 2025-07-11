using UnityEngine;

public class ProximityMover : MonoBehaviour
{
    [Header("触发设置")]
    public Transform player;                  // 玩家对象
    public float triggerDistance = 10f;        // 玩家靠近多近触发
    public bool requireTrigger = true;        // 是否需要靠近才触发

    [Header("移动设置")]
    public Vector3 moveDirection = Vector3.up; // 移动方向（单位向量）
    public float moveDistance = 5f;            // 移动距离
    public float moveSpeed = 2f;               // 移动速度
    public bool loopMovement = false;          // 是否往返循环
    public bool waitAtEnd = false;             // 到达后是否等待再回程
    public float waitTime = 1f;                // 等待时间

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool moving = false;
    private bool returning = false;
    private float waitTimer = 0f;

    private void Start()
    {
        startPosition = transform.position;
        targetPosition = startPosition + moveDirection.normalized * moveDistance;
        if (!requireTrigger)
        {
            moving = true; // 不需要触发时，自动开始运动
        }

    }

    private void Update()
    {
        if (requireTrigger && player != null && !moving)
        {
            float dist = Vector3.Distance(player.position, transform.position);
            if (dist <= triggerDistance)
            {
                moving = true;
            }
        }

        if (moving)
        {
            MoveObject();
        }
    }

    void MoveObject()
    {
        if (!returning)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                if (loopMovement)
                {
                    if (waitAtEnd)
                    {
                        waitTimer += Time.deltaTime;
                        if (waitTimer >= waitTime)
                        {
                            waitTimer = 0f;
                            returning = true;
                        }
                    }
                    else
                    {
                        returning = true;
                    }
                }
                else
                {
                    moving = false;
                }
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, startPosition, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, startPosition) < 0.01f)
            {
                if (waitAtEnd)
                {
                    waitTimer += Time.deltaTime;
                    if (waitTimer >= waitTime)
                    {
                        waitTimer = 0f;
                        returning = false;
                    }
                }
                else
                {
                    returning = false;
                }

                if (!loopMovement)
                {
                    moving = false;
                }
            }
        }
    }

    // 可用于手动触发
    public void TriggerMove()
    {
        moving = true;
    }

    public void ResetPosition()
    {
        transform.position = startPosition;
        moving = false;
        returning = false;
        waitTimer = 0f;
    }
}
