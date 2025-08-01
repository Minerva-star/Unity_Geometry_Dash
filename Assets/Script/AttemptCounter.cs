using UnityEngine;
using System;

public class AttemptCounter : MonoBehaviour
{
    [Header("尝试次数设置")]
    public string attemptKey = "AttemptCount"; // PlayerPrefs存储键
    public bool resetOnStart = false; // 是否在开始时重置
    
    [Header("事件")]
    public Action<int> OnAttemptCountChanged; // 尝试次数改变事件
    
    private int currentAttempts = 0;
    private int totalAttempts = 0;
    
    public static AttemptCounter Instance { get; private set; }
    
    private void Awake()
    {
        // 单例模式
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAttemptCount();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        Debug.Log("AttemptCounter Start() 被调用");
        if (resetOnStart)
        {
            ResetCurrentAttempts();
        }
        
        // 确保UI显示当前尝试次数
        if (OnAttemptCountChanged != null)
        {
            OnAttemptCountChanged.Invoke(currentAttempts);
        }
    }
    
    /// <summary>
    /// 增加当前尝试次数
    /// </summary>
    public void IncrementAttempt()
    {
        Debug.Log($"IncrementAttempt() 被调用，当前: {currentAttempts}, 总计: {totalAttempts}");
        currentAttempts++;
        totalAttempts++;
        SaveAttemptCount();
        
        if (OnAttemptCountChanged != null)
        {
            OnAttemptCountChanged.Invoke(currentAttempts);
            Debug.Log($"触发尝试次数改变事件: {currentAttempts}");
        }
        else
        {
            Debug.LogWarning("OnAttemptCountChanged 事件为空！");
        }
        
        Debug.Log($"尝试次数增加完成: {currentAttempts} (总计: {totalAttempts})");
    }
    
    /// <summary>
    /// 重置当前尝试次数（开始新游戏时调用）
    /// </summary>
    public void ResetCurrentAttempts()
    {
        currentAttempts = 0;
        if (OnAttemptCountChanged != null)
        {
            OnAttemptCountChanged.Invoke(currentAttempts);
        }
        Debug.Log("当前尝试次数已重置");
    }
    
    /// <summary>
    /// 获取当前尝试次数
    /// </summary>
    public int GetCurrentAttempts()
    {
        return currentAttempts;
    }
    
    /// <summary>
    /// 获取总尝试次数
    /// </summary>
    public int GetTotalAttempts()
    {
        return totalAttempts;
    }
    
    /// <summary>
    /// 设置当前尝试次数
    /// </summary>
    public void SetCurrentAttempts(int attempts)
    {
        currentAttempts = attempts;
        if (OnAttemptCountChanged != null)
        {
            OnAttemptCountChanged.Invoke(currentAttempts);
        }
    }
    
    /// <summary>
    /// 重置所有尝试次数
    /// </summary>
    public void ResetAllAttempts()
    {
        currentAttempts = 0;
        totalAttempts = 0;
        SaveAttemptCount();
        
        if (OnAttemptCountChanged != null)
        {
            OnAttemptCountChanged.Invoke(currentAttempts);
        }
        
        Debug.Log("所有尝试次数已重置");
    }
    
    /// <summary>
    /// 保存尝试次数到PlayerPrefs
    /// </summary>
    private void SaveAttemptCount()
    {
        PlayerPrefs.SetInt(attemptKey, totalAttempts);
        PlayerPrefs.Save();
    }
    
    /// <summary>
    /// 从PlayerPrefs加载尝试次数
    /// </summary>
    private void LoadAttemptCount()
    {
        totalAttempts = PlayerPrefs.GetInt(attemptKey, 0);
        Debug.Log($"加载总尝试次数: {totalAttempts}");
    }
    
    /// <summary>
    /// 获取尝试次数信息字符串
    /// </summary>
    public string GetAttemptInfo()
    {
        return $"当前: {currentAttempts} | 总计: {totalAttempts}";
    }
    
    /// <summary>
    /// 获取当前尝试次数字符串
    /// </summary>
    public string GetCurrentAttemptText()
    {
        return $"尝试次数: {currentAttempts}";
    }
    
    /// <summary>
    /// 获取总尝试次数字符串
    /// </summary>
    public string GetTotalAttemptText()
    {
        return $"总尝试: {totalAttempts}";
    }
} 