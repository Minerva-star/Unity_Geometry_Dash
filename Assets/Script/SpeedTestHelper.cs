using UnityEngine;

public class SpeedTestHelper : MonoBehaviour
{
    [Header("测试设置")]
    public PlayerBehavior player;
    public bool enableDebugInfo = true;
    
    [Header("手动测试")]
    public bool testSpeedUp = false;
    public bool testSpeedDown = false;
    public bool clearAllModifiers = false;
    
    private void Start()
    {
        if (player == null)
        {
            player = FindObjectOfType<PlayerBehavior>();
        }
        
        if (player != null)
        {
            // 订阅速度改变事件
            player.OnSpeedChanged += OnPlayerSpeedChanged;
        }
    }
    
    private void Update()
    {
        // 手动测试按钮
        if (testSpeedUp)
        {
            testSpeedUp = false;
            TestSpeedUp();
        }
        
        if (testSpeedDown)
        {
            testSpeedDown = false;
            TestSpeedDown();
        }
        
        if (clearAllModifiers)
        {
            clearAllModifiers = false;
            ClearAllModifiers();
        }
        
        // 键盘快捷键测试
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TestSpeedUp();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            TestSpeedDown();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ClearAllModifiers();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            PrintSpeedInfo();
        }
    }
    
    // 测试加速
    public void TestSpeedUp()
    {
        if (player != null)
        {
            player.AddSpeedModifier("测试加速", 2f, 5f);
            Debug.Log("添加测试加速效果：2倍速度，持续5秒");
        }
    }
    
    // 测试减速
    public void TestSpeedDown()
    {
        if (player != null)
        {
            player.AddSpeedModifier("测试减速", 0.5f, 3f);
            Debug.Log("添加测试减速效果：0.5倍速度，持续3秒");
        }
    }
    
    // 清除所有修改器
    public void ClearAllModifiers()
    {
        if (player != null)
        {
            player.ClearAllSpeedModifiers();
            Debug.Log("清除所有速度修改器");
        }
    }
    
    // 打印速度信息
    public void PrintSpeedInfo()
    {
        if (player != null)
        {
            string info = player.GetSpeedInfo();
            Debug.Log("当前速度信息:\n" + info);
        }
    }
    
    // 速度改变事件回调
    private void OnPlayerSpeedChanged(float newSpeed)
    {
        if (enableDebugInfo)
        {
            Debug.Log($"玩家速度改变: {newSpeed}");
        }
    }
    
    // 在编辑器中显示测试按钮
    private void OnGUI()
    {
        if (!Application.isPlaying) return;
        
        GUILayout.BeginArea(new Rect(10, 250, 200, 200));
        GUILayout.Label("速度测试助手");
        
        if (GUILayout.Button("测试加速 (2x, 5秒)"))
        {
            TestSpeedUp();
        }
        
        if (GUILayout.Button("测试减速 (0.5x, 3秒)"))
        {
            TestSpeedDown();
        }
        
        if (GUILayout.Button("清除所有效果"))
        {
            ClearAllModifiers();
        }
        
        if (GUILayout.Button("打印速度信息"))
        {
            PrintSpeedInfo();
        }
        
        GUILayout.EndArea();
    }
    
    private void OnDestroy()
    {
        if (player != null)
        {
            player.OnSpeedChanged -= OnPlayerSpeedChanged;
        }
    }
} 