using UnityEngine;

public class SpeedItem : MonoBehaviour
{
    [Header("道具设置")]
    public string itemName = "速度道具";
    public float speedMultiplier = 2f; // 速度倍率
    public float duration = 3f; // 持续时间(秒)
    
    [Header("视觉效果")]
    public Color itemColor = Color.green;
    public bool rotateItem = true;
    public float rotateSpeed = 90f; // 旋转速度(度/秒)
    
    [Header("音效")]
    public AudioClip collectSound;
    
    [Header("粒子效果")]
    public GameObject collectParticle;
    
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    private bool isCollected = false;
    
    private void Start()
    {
        // 获取组件
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        
        // 如果没有AudioSource，添加一个
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // // 设置颜色
        // if (spriteRenderer != null)
        // {
        //     spriteRenderer.color = itemColor;
        // }
        
        // 确保有碰撞器
        Collider2D collider = GetComponent<Collider2D>();
        if (collider == null)
        {
            // 添加圆形碰撞器
            CircleCollider2D circleCollider = gameObject.AddComponent<CircleCollider2D>();
            circleCollider.isTrigger = true;
        }
        else
        {
            collider.isTrigger = true;
        }
        
        // 设置标签
        gameObject.tag = "SpeedItem";
    }
    
    private void Update()
    {
        // 旋转道具
        if (rotateItem && !isCollected)
        {
            transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检查是否是玩家
        if (other.CompareTag("Player") && !isCollected)
        {
            PlayerBehavior player = other.GetComponent<PlayerBehavior>();
            if (player != null)
            {
                CollectItem(player);
            }
        }
    }
    
    private void CollectItem(PlayerBehavior player)
    {
        isCollected = true;
        
        // 给玩家添加速度效果
        player.AddSpeedModifier(itemName, speedMultiplier, duration);
        
        // 播放收集音效
        if (collectSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(collectSound);
        }
        
        // 播放粒子效果
        if (collectParticle != null)
        {
            Instantiate(collectParticle, transform.position, transform.rotation);
        }
        
        // 隐藏道具
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }
        
        // 禁用碰撞器
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }
        
        // 延迟销毁对象
        Destroy(gameObject, collectSound != null ? collectSound.length : 1f);
        
        Debug.Log($"收集道具: {itemName}, 速度倍率: {speedMultiplier}, 持续时间: {duration}秒");
    }
    
    // 在编辑器中显示道具信息
    private void OnDrawGizmos()
    {
        Gizmos.color = itemColor;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        
        #if UNITY_EDITOR
        Vector3 textPos = transform.position + Vector3.up * 0.8f;
        UnityEditor.Handles.Label(textPos, $"{itemName}\n{speedMultiplier}x\n{duration}s");
        #endif
    }
} 