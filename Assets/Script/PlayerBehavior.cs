using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    private Rigidbody2D _rb;
    private SpriteRenderer _renderer;
    private Collider2D _collider;
    private TrailRenderer _trail;
    

    public float jumpForce = 5f;
    public float fallSpeed = 3f;

    public float RightVelocity = 20f;
    public float baseRotation = 0f;
    public float rotationSpeed = 360;
// 测试提交怎么用  喵

    // 屏幕位置控制
    private Camera mainCamera;

    public float screenPositionX = 0.33f; // 屏幕左1/3位置

    // 事件
    public System.Action<float> OnProgressChanged;
    public System.Action OnPlayerDeath;


    // 游戏状态
    private bool isGamePaused = false;
    private bool isGameOver = false;

    public GameObject particleEffectPrefab;

    public Vector3 oldPosition;

    // 跳跃控制
    public float longPressJumpForce = 3f; // 长按时持续的力
    public float maxUpwardVelocity = 8f; // 最大向上速度限制

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
        _renderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
        _trail = GetComponent<TrailRenderer>();

        _rb.gravityScale = 0f;


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
            currentVelocity.x = RightVelocity;

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
        _trail.Clear();

        transform.rotation = Quaternion.Euler(0, 0, baseRotation);

    }


}

