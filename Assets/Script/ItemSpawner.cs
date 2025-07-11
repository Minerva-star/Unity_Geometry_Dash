using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("道具预制体")]
    public GameObject speedUpItemPrefab; // 加速道具
    public GameObject speedDownItemPrefab; // 减速道具
    
    [Header("生成设置")]
    public Transform[] spawnPoints; // 生成点
    public float spawnInterval = 5f; // 生成间隔
    public bool autoSpawn = true; // 自动生成
    
    [Header("道具配置")]
    public float speedUpMultiplier = 2f; // 加速倍率
    public float speedUpDuration = 3f; // 加速持续时间
    public float speedDownMultiplier = 0.5f; // 减速倍率
    public float speedDownDuration = 2f; // 减速持续时间
    
    private float nextSpawnTime;
    
    private void Start()
    {
        nextSpawnTime = Time.time + spawnInterval;
    }
    
    private void Update()
    {
        if (autoSpawn && Time.time >= nextSpawnTime)
        {
            SpawnRandomItem();
            nextSpawnTime = Time.time + spawnInterval;
        }
        
        // 手动生成测试
        if (Input.GetKeyDown(KeyCode.S))
        {
            SpawnSpeedUpItem();
        }
        
        if (Input.GetKeyDown(KeyCode.D))
        {
            SpawnSpeedDownItem();
        }
    }
    
    // 生成随机道具
    public void SpawnRandomItem()
    {
        if (Random.value > 0.5f)
        {
            SpawnSpeedUpItem();
        }
        else
        {
            SpawnSpeedDownItem();
        }
    }
    
    // 生成加速道具
    public void SpawnSpeedUpItem()
    {
        if (speedUpItemPrefab == null)
        {
            CreateSpeedItem("加速道具", speedUpMultiplier, speedUpDuration, Color.green);
        }
        else
        {
            Vector3 spawnPos = GetRandomSpawnPosition();
            GameObject item = Instantiate(speedUpItemPrefab, spawnPos, Quaternion.identity);
            
            // 配置道具参数
            SpeedItem speedItem = item.GetComponent<SpeedItem>();
            if (speedItem != null)
            {
                speedItem.itemName = "加速道具";
                speedItem.speedMultiplier = speedUpMultiplier;
                speedItem.duration = speedUpDuration;
                speedItem.itemColor = Color.green;
            }
        }
    }
    
    // 生成减速道具
    public void SpawnSpeedDownItem()
    {
        if (speedDownItemPrefab == null)
        {
            CreateSpeedItem("减速道具", speedDownMultiplier, speedDownDuration, Color.red);
        }
        else
        {
            Vector3 spawnPos = GetRandomSpawnPosition();
            GameObject item = Instantiate(speedDownItemPrefab, spawnPos, Quaternion.identity);
            
            // 配置道具参数
            SpeedItem speedItem = item.GetComponent<SpeedItem>();
            if (speedItem != null)
            {
                speedItem.itemName = "减速道具";
                speedItem.speedMultiplier = speedDownMultiplier;
                speedItem.duration = speedDownDuration;
                speedItem.itemColor = Color.red;
            }
        }
    }
    
    // 创建速度道具（如果没有预制体）
    private void CreateSpeedItem(string name, float multiplier, float duration, Color color)
    {
        Vector3 spawnPos = GetRandomSpawnPosition();
        
        // 创建道具对象
        GameObject item = new GameObject(name);
        item.transform.position = spawnPos;
        
        // 添加SpriteRenderer
        SpriteRenderer spriteRenderer = item.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = CreateDefaultSprite();
        spriteRenderer.color = color;
        
        // 添加SpeedItem脚本
        SpeedItem speedItem = item.AddComponent<SpeedItem>();
        speedItem.itemName = name;
        speedItem.speedMultiplier = multiplier;
        speedItem.duration = duration;
        speedItem.itemColor = color;
        
        // 添加碰撞器
        CircleCollider2D collider = item.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        
        // 设置标签
        item.tag = "SpeedItem";
        
        Debug.Log($"创建道具: {name} 在位置 {spawnPos}");
    }
    
    // 获取随机生成位置
    private Vector3 GetRandomSpawnPosition()
    {
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            return spawnPoint.position;
        }
        else
        {
            // 如果没有设置生成点，在玩家附近随机生成
            PlayerBehavior player = FindObjectOfType<PlayerBehavior>();
            if (player != null)
            {
                Vector3 playerPos = player.transform.position;
                float randomX = playerPos.x + Random.Range(5f, 15f);
                float randomY = playerPos.y + Random.Range(-2f, 2f);
                return new Vector3(randomX, randomY, 0);
            }
            else
            {
                // 默认位置
                return new Vector3(Random.Range(-10f, 10f), Random.Range(-5f, 5f), 0);
            }
        }
    }
    
    // 创建默认精灵（如果没有精灵）
    private Sprite CreateDefaultSprite()
    {
        // 创建一个简单的圆形纹理
        int size = 32;
        Texture2D texture = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];
        
        Vector2 center = new Vector2(size / 2f, size / 2f);
        float radius = size / 2f;
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                pixels[y * size + x] = distance <= radius ? Color.white : Color.clear;
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }
    
    // 设置生成间隔
    public void SetSpawnInterval(float interval)
    {
        spawnInterval = interval;
    }
    
    // 开始自动生成
    public void StartAutoSpawn()
    {
        autoSpawn = true;
    }
    
    // 停止自动生成
    public void StopAutoSpawn()
    {
        autoSpawn = false;
    }
    
    // 立即生成一个道具
    public void SpawnItemNow()
    {
        SpawnRandomItem();
    }
} 