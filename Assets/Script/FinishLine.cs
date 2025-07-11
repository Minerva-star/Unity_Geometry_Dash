using UnityEngine;

public class FinishLine : MonoBehaviour
{
    [Header("终点线设置")]
    public bool autoRegisterToGameManager = true; // 是否自动注册到GameManager
    public Color gizmoColor = Color.green; // 在Scene视图中显示的颜色
    
    [Header("终点效果")]
    public bool showFinishEffect = true; // 是否显示终点效果
    public GameObject finishEffectPrefab; // 终点特效预制体
    
    private GameManager gameManager;
    
    private void Start()
    {
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
    
    // 玩家到达终点时的处理
    private void OnPlayerReachedFinish()
    {
        if (showFinishEffect)
        {
            ShowFinishEffect();
        }
        
        Debug.Log("玩家到达终点！");
        
        // 可以在这里添加胜利逻辑
        // 比如显示胜利UI、播放胜利音效等
    }
    
    // 显示终点特效
    private void ShowFinishEffect()
    {
        if (finishEffectPrefab != null)
        {
            Instantiate(finishEffectPrefab, transform.position, transform.rotation);
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