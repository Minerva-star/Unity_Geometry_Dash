using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 速度修改器数据结构
[System.Serializable]
public class SpeedModifier
{
    public string modifierName;
    public float speedMultiplier = 1f; // 速度倍率 (1.0 = 正常速度, 2.0 = 双倍速度, 0.5 = 一半速度)
    public float duration = -1f; // -1表示永久，正数表示持续时间
    public bool isActive = false;
    public float remainingTime = 0f;
}

public class PlayerBehavior : MonoBehaviour
{
    private Rigidbody2D _rb;
    public SpriteRenderer _renderer;
    private Collider2D _collider;
    // private TrailRenderer _trail;


    public float jumpForce = 5f;
    public float fallSpeed = 3f;

    public float RightVelocity = 20f;
    public float baseRotation = 0f;
    public float rotationSpeed = 360;
    public TrailController trailController;

    // 速度管理相关
    [Header("速度管理")]
    public float baseRightVelocity = 20f; // 基础向右速度
    public float currentRightVelocity = 20f; // 当前向右速度
    public List<SpeedModifier> activeSpeedModifiers = new List<SpeedModifier>(); // 当前激活的速度修改器
    public bool enableSpeedModifiers = true; // 是否启用速度修改器系统

    // 屏幕位置控制
    private Camera mainCamera;

    public float screenPositionX = 0.33f; // 屏幕左1/3位置

    // 事件
    public System.Action<float> OnProgressChanged;
    public System.Action OnPlayerDeath;
    public System.Action<float> OnSpeedChanged; // 新增：速度改变事件

    // 游戏状态
    private bool isGamePaused = false;
    private bool isGameOver = false;

    public GameObject particleEffectPrefab;

    public Vector3 oldPosition;

    // 跳跃控制
    public float longPressJumpForce = 3f; // 长按时持续的力
    public float maxUpwardVelocity = 8f; // 最大向上速度限制
    public enum ControlMode { JumpClick, FlyClick, FreeMove }
    public ControlMode currentMode = ControlMode.FlyClick;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
        _collider = GetComponent<Collider2D>();
        // _trail = GetComponent<TrailRenderer>();

        _rb.gravityScale = 0f;

        // 初始化速度
        baseRightVelocity = RightVelocity;
        currentRightVelocity = RightVelocity;

        transform.rotation = Quaternion.Euler(0, 0, baseRotation);

        //// 设置初始位置
        //SetPlayerStartPosition();

        //SetPlayerScreenPosition();
    }


    private void Update()
    {


        if (isGamePaused || isGameOver) return;
        // 检测跳跃输入
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            // 直接设置向上的速度，而不是添加力
            // 这样可以避免连续点击时的累积效果
            Vector2 currentVel = _rb.velocity;
            currentVel.y = jumpForce;
            _rb.velocity = currentVel;

        }


        // 检测长按跳跃
        if (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0))
        {
            //// 长按时持续给向上的力，但有速度限制
            //if (_rb.velocity.y < maxUpwardVelocity)
            //{
            //    _rb.AddForce(Vector2.up * longPressJumpForce, ForceMode2D.Force);
            //}

            // 长按时持续给向上的速度，但有速度限制
            Vector2 currentVel = _rb.velocity;
            if (currentVel.y < maxUpwardVelocity)
            {
                currentVel.y += longPressJumpForce * Time.deltaTime;
                _rb.velocity = currentVel;
            }

        }

        // 这个 x, y 是只读的   不能直接修改
        //_rb.velocity.x = RightVelocity;


        //if (Input.GetKey(KeyCode.R)) Retry();

        //if (Input.GetKey(KeyCode.Escape)) MenuManager.LevelListButton();

        UpdateRotation();
        UpdateSpeedModifiers(); // 新增：更新速度修改器

    }

    // 新增：更新速度修改器
    private void UpdateSpeedModifiers()
    {
        if (!enableSpeedModifiers) return;

        // 更新所有激活的速度修改器
        for (int i = activeSpeedModifiers.Count - 1; i >= 0; i--)
        {
            SpeedModifier modifier = activeSpeedModifiers[i];

            if (modifier.duration > 0)
            {
                modifier.remainingTime -= Time.deltaTime;
                if (modifier.remainingTime <= 0)
                {
                    // 移除过期的修改器
                    activeSpeedModifiers.RemoveAt(i);
                    continue;
                }
            }
        }

        // 重新计算当前速度
        CalculateCurrentSpeed();
    }

    // 新增：计算当前速度
    private void CalculateCurrentSpeed()
    {
        float totalMultiplier = 1f;

        foreach (SpeedModifier modifier in activeSpeedModifiers)
        {
            totalMultiplier *= modifier.speedMultiplier;
        }

        float newSpeed = baseRightVelocity * totalMultiplier;

        if (newSpeed != currentRightVelocity)
        {
            currentRightVelocity = newSpeed;
            RightVelocity = newSpeed; // 保持与原有代码的兼容性

            // 触发速度改变事件
            if (OnSpeedChanged != null)
            {
                OnSpeedChanged.Invoke(currentRightVelocity);
            }
            // 通知BackgroundMover更新速度
            NotifyBackgroundMover();

            // 调试信息
            Debug.Log($"速度改变: {currentRightVelocity} (倍率: {totalMultiplier})");
        }
    }

    // 新增：通知BackgroundMover
    private void NotifyBackgroundMover()
    {
        // 查找场景中的BackgroundMover
        BackgroundMover backgroundMover = FindObjectOfType<BackgroundMover>();
        if (backgroundMover != null)
        {
            // 如果BackgroundMover使用玩家速度，它会自动跟随
            // 如果需要手动设置，可以调用：
            backgroundMover.SetMoveSpeed(currentRightVelocity);
            Debug.Log($"通知BackgroundMover，当前速度: {currentRightVelocity}");
        }
    }

    // 新增：添加速度修改器
    public void AddSpeedModifier(string name, float multiplier, float duration = -1f)
    {
        if (!enableSpeedModifiers) return;

        SpeedModifier newModifier = new SpeedModifier
        {
            modifierName = name,
            speedMultiplier = multiplier,
            duration = duration,
            isActive = true,
            remainingTime = duration
        };

        activeSpeedModifiers.Add(newModifier);
        CalculateCurrentSpeed();

        Debug.Log($"添加速度修改器: {name}, 倍率: {multiplier}, 持续时间: {duration}");
    }

    // 新增：移除速度修改器
    public void RemoveSpeedModifier(string name)
    {
        if (!enableSpeedModifiers) return;

        for (int i = activeSpeedModifiers.Count - 1; i >= 0; i--)
        {
            if (activeSpeedModifiers[i].modifierName == name)
            {
                activeSpeedModifiers.RemoveAt(i);
                CalculateCurrentSpeed();
                Debug.Log($"移除速度修改器: {name}");
                break;
            }
        }
    }

    // 新增：清除所有速度修改器
    public void ClearAllSpeedModifiers()
    {
        if (!enableSpeedModifiers) return;

        activeSpeedModifiers.Clear();
        CalculateCurrentSpeed();
        Debug.Log("清除所有速度修改器");
    }

    private void UpdateRotation()
    {
        float targetRotation = baseRotation;
        if (_rb.velocity.y > 0.1f)
        {
            targetRotation = 30f;
        }
        else if (_rb.velocity.y < -0.1f)
        {
            targetRotation = -30f;
        }

        // float currentZ = transform.rotation.eulerAngles.z;
        // float newZ = Mathf.MoveTowards(currentZ, targetRotation, rotationSpeed * Time.deltaTime);
        // transform.rotation = Quaternion.Euler(0, 0, newZ);

        transform.rotation = Quaternion.Euler(0, 0, targetRotation);
    }




    private void FixedUpdate()
    {

        if (isGamePaused || isGameOver) return;

        //// 設置玩家在屏幕1/3处 
        //SetPlayerScreenPosition();

        //_rb.position += Vector2.right * 0.05f;
        if (mainCamera != null)
        {
            // 设置玩家向右的速度
            Vector2 currentVelocity = _rb.velocity;
            currentVelocity.x = currentRightVelocity; // 使用当前计算的速度

            if (!Input.GetKey(KeyCode.Space) && !Input.GetMouseButton(0))
            {
                currentVelocity.y = -fallSpeed;

            }

            _rb.velocity = currentVelocity;

            // 2. 移动相机跟随玩家
            Vector3 cameraPos = mainCamera.transform.position;
            cameraPos.x = transform.position.x; // 相机位置在玩家右侧2个单位
            mainCamera.transform.position = cameraPos;


        }


    }


    //private void SetPlayerStartPosition()
    //{

    //    transform.position = oldPosition;

    //}


    //private void SetPlayerScreenPosition()
    //{
    //    Vector3 screenPos = new Vector3(screenPositionX, 0, 0);
    //    Vector3 worldPos = mainCamera.ViewportToWorldPoint(screenPos);

    //    Vector3 currentPos = transform.position;
    //    currentPos.x = worldPos.x;
    //    transform.position = currentPos;

    //}

    public void OnTriggerEnter2D(Collider2D other)
    { 
        Debug.Log("喔喔喔OnTriggerEnter2D");
        if (isGameOver || isGamePaused) return;

        // 检测障碍物
        if (other.CompareTag("Obstacle"))
        {
            isGameOver = true;

            _rb.velocity = Vector2.zero;
            _rb.isKinematic = true;
            _renderer.enabled = false;



            if (particleEffectPrefab != null)
            {
                Instantiate(particleEffectPrefab, transform.position, particleEffectPrefab.transform.rotation);
            }

            // 触发死亡事件
            if (OnPlayerDeath != null)
            {
                OnPlayerDeath.Invoke();
            }
        }

        // 检测速度道具
        else if (other.CompareTag("SpeedItem"))
        {
            SpeedItem speedItem = other.GetComponent<SpeedItem>();
            if (speedItem != null)
            {
                // SpeedItem会自动处理收集逻辑
                Debug.Log("碰到速度道具！");
            }
        }
        // 检测速度区域（兼容旧版本）
        else if (other.CompareTag("SpeedZone"))
        {
            SpeedZone speedZone = other.GetComponent<SpeedZone>();
            if (speedZone != null)
            {
                speedZone.OnPlayerEnter(this);
                Debug.Log("进入速度区域！");
            }
        }
    }

    // 新增：离开触发器检测
    public void OnTriggerExit2D(Collider2D other)
    {
        if (isGameOver || isGamePaused) return;

        // 检测离开速度区域
        if (other.CompareTag("SpeedZone"))
        {
            SpeedZone speedZone = other.GetComponent<SpeedZone>();
            if (speedZone != null)
            {
                speedZone.OnPlayerExit(this);
            }
        }
    }

    public void SetGamePaused(bool paused)
    {
        isGamePaused = paused;
        if (paused)
        {
            _rb.velocity = Vector2.zero;
        }
    }


    public void Reset()
    {
        isGameOver = false;
        isGamePaused = false;

        _renderer.enabled = true;
        _collider.enabled = true;
        _rb.isKinematic = false;
        transform.position = oldPosition;
        // _trail.Clear();

        transform.rotation = Quaternion.Euler(0, 0, baseRotation);
        if (trailController != null)
            trailController.ClearTrail();

        // 新增：重置速度修改器
        ClearAllSpeedModifiers();
    }

    // 新增：获取当前速度信息（用于调试）
    public string GetSpeedInfo()
    {
        string info = $"基础速度: {baseRightVelocity}, 当前速度: {currentRightVelocity}\n";
        info += $"激活的修改器数量: {activeSpeedModifiers.Count}\n";

        foreach (SpeedModifier modifier in activeSpeedModifiers)
        {
            info += $"- {modifier.modifierName}: {modifier.speedMultiplier}x";
            if (modifier.duration > 0)
            {
                info += $" (剩余时间: {modifier.remainingTime:F1}s)";
            }
            info += "\n";
        }

        return info;
    }

}

