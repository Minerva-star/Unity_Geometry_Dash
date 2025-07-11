using UnityEngine;

public class SpeedZoneExample : MonoBehaviour
{
    [Header("示例设置")]
    public GameObject speedZonePrefab; // 速度区域预制体
    public Transform[] spawnPoints; // 生成点
    
    [Header("测试按钮")]
    public bool createSpeedZone = false;
    public bool createSlowZone = false;
    public bool createTemporaryZone = false;
    
    private PlayerBehavior player;
    
    private void Start()
    {
        // 获取玩家引用
        player = FindObjectOfType<PlayerBehavior>();
        
        if (player != null)
        {
            // 订阅速度改变事件
            player.OnSpeedChanged += OnPlayerSpeedChanged;
        }
    }
    
    private void Update()
    {
        // 测试按钮
        if (createSpeedZone)
        {
            createSpeedZone = false;
            CreateSpeedZone(2f, "加速区域", 5f);
        }
        
        if (createSlowZone)
        {
            createSlowZone = false;
            CreateSpeedZone(0.5f, "减速区域", -1f);
        }
        
        if (createTemporaryZone)
        {
            createTemporaryZone = false;
            CreateSpeedZone(1.5f, "临时加速", 3f);
        }
        
        // 键盘快捷键测试
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CreateSpeedZone(2f, "键盘加速", 5f);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CreateSpeedZone(0.5f, "键盘减速", 3f);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (player != null)
            {
                player.ClearAllSpeedModifiers();
            }
        }
    }
    
    // 创建速度区域
    public void CreateSpeedZone(float multiplier, string name, float duration)
    {
        if (speedZonePrefab == null)
        {
            Debug.LogWarning("请设置速度区域预制体！");
            return;
        }
        
        Vector3 spawnPosition = Vector3.zero;
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            spawnPosition = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
        }
        else
        {
            // 如果没有设置生成点，在玩家前方生成
            if (player != null)
            {
                spawnPosition = player.transform.position + Vector3.right * 10f;
            }
        }
        
        GameObject zoneObj = Instantiate(speedZonePrefab, spawnPosition, Quaternion.identity);
        SpeedZone speedZone = zoneObj.GetComponent<SpeedZone>();
        
        if (speedZone != null)
        {
            speedZone.zoneName = name;
            speedZone.speedMultiplier = multiplier;
            speedZone.duration = duration;
            
            // 根据倍率设置颜色
            if (multiplier > 1f)
            {
                speedZone.zoneColor = Color.green; // 加速区域用绿色
            }
            else if (multiplier < 1f)
            {
                speedZone.zoneColor = Color.red; // 减速区域用红色
            }
            else
            {
                speedZone.zoneColor = Color.yellow; // 正常速度用黄色
            }
            
            Debug.Log($"创建速度区域: {name}, 倍率: {multiplier}, 持续时间: {duration}");
        }
    }
    
    // 玩家速度改变事件回调
    private void OnPlayerSpeedChanged(float newSpeed)
    {
        Debug.Log($"玩家速度改变为: {newSpeed}");
        
        // 可以在这里添加UI更新、音效播放等逻辑
        if (newSpeed > player.baseRightVelocity)
        {
            // 加速时的效果
            Debug.Log("玩家正在加速！");
        }
        else if (newSpeed < player.baseRightVelocity)
        {
            // 减速时的效果
            Debug.Log("玩家正在减速！");
        }
    }
    
    // 通过代码直接添加速度修改器
    public void AddSpeedModifierDirectly()
    {
        if (player != null)
        {
            player.AddSpeedModifier("代码添加", 1.8f, 4f);
        }
    }
    
    // 移除特定速度修改器
    public void RemoveSpeedModifier(string name)
    {
        if (player != null)
        {
            player.RemoveSpeedModifier(name);
        }
    }
    
    // 获取当前速度信息
    public void PrintSpeedInfo()
    {
        if (player != null)
        {
            string info = player.GetSpeedInfo();
            Debug.Log("当前速度信息:\n" + info);
        }
    }
    
    private void OnDestroy()
    {
        // 取消订阅事件
        if (player != null)
        {
            player.OnSpeedChanged -= OnPlayerSpeedChanged;
        }
    }
    
    // 在编辑器中显示测试按钮
    private void OnGUI()
    {
        if (!Application.isPlaying) return;
        
        GUILayout.BeginArea(new Rect(10, 10, 200, 200));
        GUILayout.Label("速度区域测试");
        
        if (GUILayout.Button("创建加速区域 (2x, 5秒)"))
        {
            CreateSpeedZone(2f, "加速区域", 5f);
        }
        
        if (GUILayout.Button("创建减速区域 (0.5x, 永久)"))
        {
            CreateSpeedZone(0.5f, "减速区域", -1f);
        }
        
        if (GUILayout.Button("创建临时加速 (1.5x, 3秒)"))
        {
            CreateSpeedZone(1.5f, "临时加速", 3f);
        }
        
        if (GUILayout.Button("清除所有修改器"))
        {
            if (player != null)
            {
                player.ClearAllSpeedModifiers();
            }
        }
        
        if (GUILayout.Button("打印速度信息"))
        {
            PrintSpeedInfo();
        }
        
        GUILayout.EndArea();
    }
} 