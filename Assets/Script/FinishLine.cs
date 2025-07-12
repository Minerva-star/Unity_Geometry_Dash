using UnityEngine;
using System.Collections;

public class FinishLine : MonoBehaviour
{
    [Header("终点线设置")]
    public bool autoRegisterToGameManager = true; // 是否自动注册到GameManager
    public Color gizmoColor = Color.green; // 在Scene视图中显示的颜色
    
    [Header("终点效果")]
    public bool showFinishEffect = true; // 是否显示终点效果
    // public GameObject finishEffectPrefab; // 终点特效预制体
    
    [Header("胜利音效")]
    public AudioClip victorySound1; // 第一个音效
    public AudioClip victorySound2; // 第二个音效
    public float overlapTime = 3f; // 第一个音效播放多少秒后开始第二个
    
    private GameManager gameManager;
    
    private AudioSource audioSource;
    
    private void Start()
    {
        // 获取或添加AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        if (autoRegisterToGameManager)
        {
            RegisterToGameManager();
        }
    }
    
    // 注册到GameManager
    public void RegisterToGameManager()
    {
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.SetFinishLine(transform);
            Debug.Log($"终点线已注册到GameManager，位置: {transform.position.x}");
        }
        else
        {
            Debug.LogWarning("未找到GameManager，无法注册终点线");
        }
    }
    
    // 检查玩家是否到达终点
    private void Update()
    {
        if (gameManager != null && gameManager.IsPlayerAtFinish())
        {
            OnPlayerReachedFinish();
        }
    }
    
    private bool hasTriggeredWin = false; // 防止多次触发

    // 玩家到达终点时的处理
    private void OnPlayerReachedFinish()
    {
        if (hasTriggeredWin) return;
        hasTriggeredWin = true;

        if (showFinishEffect)
        {
            // ShowFinishEffect();
        }
        
        Debug.Log("玩家到达终点！");

        // 处理通关后的游戏状态
        HandleVictoryState();

        // 弹出胜利面板
        if (gameManager != null && gameManager.uiManager != null)
        {
            gameManager.uiManager.ShowWinPanel();
        }

        // 播放胜利音效序列
        StartCoroutine(PlayVictorySounds());
    }

    // 处理通关后的游戏状态
    private void HandleVictoryState()
    {
        if (gameManager != null)
        {
            // 暂停背景移动
            if (gameManager.background != null)
            {
                gameManager.background.SetPaused(true);
            }

            // 暂停玩家移动
            if (gameManager.player != null)
            {
                gameManager.player.SetGamePaused(true);
                
                // 隐藏玩家
                SpriteRenderer playerRenderer = gameManager.player.GetComponent<SpriteRenderer>();
                if (playerRenderer != null)
                {
                    playerRenderer.enabled = false;
                }
                
                // 隐藏玩家痕迹
                TrailController trailController = gameManager.player.trailController;
                if (trailController != null)
                {
                    trailController.ClearTrail();
                    trailController.enabled = false;
                }
            }

            // 暂停背景音乐
            gameManager.SetMusicPause(true);
        }
    }
    
    // // 显示终点特效
    // private void ShowFinishEffect()
    // {
    //     if (finishEffectPrefab != null)
    //     {
    //         Instantiate(finishEffectPrefab, transform.position, transform.rotation);
    //     }
    // }
    
    private IEnumerator PlayVictorySounds()
    {
        // 使用FinishLine自己的AudioSource播放胜利音效
        if (audioSource != null)
        {
            if (victorySound1 != null)
            {
                audioSource.PlayOneShot(victorySound1);
                Debug.Log("播放第一个胜利音效");
                
                yield return new WaitForSeconds(overlapTime);
                
                if (victorySound2 != null)
                {
                    audioSource.PlayOneShot(victorySound2);
                    Debug.Log("播放第二个胜利音效（与第一个重叠）");
                }
            }
        }
        else
        {
            Debug.LogWarning("FinishLine没有AudioSource组件，无法播放胜利音效");
        }
    }
    
    // 在Scene视图中显示终点线
    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        
        // 绘制一条垂直线表示终点
        Vector3 top = transform.position + Vector3.up * 10f;
        Vector3 bottom = transform.position + Vector3.down * 10f;
        Gizmos.DrawLine(top, bottom);
        
        // 绘制终点标记
        Gizmos.DrawWireCube(transform.position, new Vector3(1f, 20f, 1f));
        
        // 显示终点信息
        #if UNITY_EDITOR
        Vector3 textPos = transform.position + Vector3.up * 12f;
        UnityEditor.Handles.Label(textPos, $"终点线\nX: {transform.position.x:F1}");
        #endif
    }
    
    // 在Scene视图中选中时显示详细信息
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.3f);
        Gizmos.DrawCube(transform.position, new Vector3(1f, 20f, 1f));
    }
    
    // 获取终点位置
    public Vector3 GetFinishPosition()
    {
        return transform.position;
    }
    
    // 设置终点位置
    public void SetFinishPosition(Vector3 position)
    {
        transform.position = position;
        if (gameManager != null)
        {
            gameManager.SetFinishLine(transform);
        }
    }
    
    // 设置终点X坐标
    public void SetFinishX(float x)
    {
        Vector3 pos = transform.position;
        pos.x = x;
        transform.position = pos;
        if (gameManager != null)
        {
            gameManager.SetFinishLine(transform);
        }
    }
} 