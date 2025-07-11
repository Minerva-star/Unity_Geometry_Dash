using UnityEngine;

public class SpeedZone : MonoBehaviour
{
    [Header("速度区域设置")]
    public string zoneName = "速度区域";
    public float speedMultiplier = 1.5f; // 速度倍率
    public float duration = -1f; // -1表示永久，正数表示持续时间
    public bool removeOnExit = true; // 离开区域时是否移除效果
    
    [Header("视觉效果")]
    public Color zoneColor = Color.cyan; // 区域颜色
    public bool showDebugInfo = true; // 是否显示调试信息
    
    [Header("音效")]
    public AudioClip enterSound; // 进入音效
    public AudioClip exitSound; // 离开音效
    
    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    
    private void Start()
    {
        // 获取或添加AudioSource组件
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // 获取SpriteRenderer用于视觉效果
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
            spriteRenderer.color = zoneColor;
        }
        
        // 设置标签
        gameObject.tag = "SpeedZone";
        
        if (showDebugInfo)
        {
            Debug.Log($"创建速度区域: {zoneName}, 倍率: {speedMultiplier}, 持续时间: {duration}");
        }
    }
    
    // 玩家进入区域
    public void OnPlayerEnter(PlayerBehavior player)
    {
        if (player == null) return;
        
        // 添加速度修改器
        player.AddSpeedModifier(zoneName, speedMultiplier, duration);
        
        // 播放进入音效
        if (enterSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(enterSound);
        }
        
        // 视觉反馈
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.blue; // 激活时变蓝
        }
        
        if (showDebugInfo)
        {
            Debug.Log($"玩家进入速度区域: {zoneName}");
        }
    }
    
    // 玩家离开区域
    public void OnPlayerExit(PlayerBehavior player)
    {
        if (player == null) return;
        
        // 如果设置为离开时移除效果
        if (removeOnExit)
        {
            player.RemoveSpeedModifier(zoneName);
        }
        
        // 播放离开音效
        if (exitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(exitSound);
        }
        
        // 恢复原始颜色
        if (spriteRenderer != null)
        {
            spriteRenderer.color = zoneColor;
        }
        
        if (showDebugInfo)
        {
            Debug.Log($"玩家离开速度区域: {zoneName}");
        }
    }
    
    // 在编辑器中显示区域范围
    private void OnDrawGizmos()
    {
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider != null)
        {
            Gizmos.color = zoneColor;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(boxCollider.offset, boxCollider.size);
            
            // 显示速度倍率
            Vector3 textPos = transform.position + Vector3.up * 0.5f;
            #if UNITY_EDITOR
            UnityEditor.Handles.Label(textPos, $"{zoneName}\n{speedMultiplier}x");
            #endif
        }
    }
    
    // 在编辑器中选中时显示详细信息
    private void OnDrawGizmosSelected()
    {
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider != null)
        {
            Gizmos.color = new Color(zoneColor.r, zoneColor.g, zoneColor.b, 0.3f);
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(boxCollider.offset, boxCollider.size);
        }
    }
} 