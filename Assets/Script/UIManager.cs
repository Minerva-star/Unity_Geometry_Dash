using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    // UI面板
    public GameObject mainGameUI;
    public GameObject pausePanel;
    public GameObject gameOverPanel;
    public GameObject winPanel;
    // 移除tipPanel字段，只保留tipText
    public TextMeshProUGUI tipText; // Tip文本

    // 主游戏UI元素
    public TextMeshProUGUI progressText;
    public Button pauseButton;

    // 暂停面板元素
    public Button resumeButton;
    public Button restartButton;
    public Button quitButton;

    // 游戏结束面板元素
    public TextMeshProUGUI finalProgressText;
    public TextMeshProUGUI highRecordProgressText;
    public Button gameOverRestartButton;
    public Button gameOverQuitButton;

    //胜利面板
    public Button gameWinRestartButton;

    // 引用
    public GameManager gameManager;

    // 尝试次数显示
    public TextMeshProUGUI attemptCountText; // 主游戏界面尝试次数
    public TextMeshProUGUI gameOverAttemptText; // 游戏结束界面尝试次数
    public TextMeshProUGUI winAttemptText; // 胜利界面尝试次数
    //public TextMeshProUGUI pauseAttemptText; // 暂停界面尝试次数


    private void Start()
    {
        SetupUI();
        ShowMainGameUI();
        
        // 检查是否第一次玩
        CheckFirstTimePlay();
        
        // 直接更新尝试次数显示
        UpdateAllAttemptDisplays();
    }

    private void SetupUI()
    {
        // 设置按钮事件
        if (pauseButton != null)
            pauseButton.onClick.AddListener(OnPauseButtonClicked);

        if (resumeButton != null)
            resumeButton.onClick.AddListener(OnResumeButtonClicked);

        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartButtonClicked);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitButtonClicked);

        if (gameOverRestartButton != null)
            gameOverRestartButton.onClick.AddListener(OnRestartButtonClicked);

        if (gameOverQuitButton != null)
            gameOverQuitButton.onClick.AddListener(OnQuitButtonClicked);

        if (gameWinRestartButton != null)
            gameWinRestartButton.onClick.AddListener(OnRestartButtonClicked);
        
        // 订阅尝试次数改变事件
        AttemptCounter attemptCounter = FindObjectOfType<AttemptCounter>();
        if (attemptCounter != null)
        {
            attemptCounter.OnAttemptCountChanged += OnAttemptCountChanged;
        }
    }

    private void CheckFirstTimePlay()
    {
        bool isFirstTime = PlayerPrefs.GetInt("HasPlayedBefore", 0) == 0;
        
        if (isFirstTime)
        {
            ShowTip();
            PlayerPrefs.SetInt("HasPlayedBefore", 1);
            PlayerPrefs.Save();
        }
    }

    private void ShowTip()
    {
        if (tipText != null)
        {
            tipText.gameObject.SetActive(true);
            Debug.Log("显示新手提示");
            
            // 3秒后自动隐藏
            StartCoroutine(AutoHideTip(2f));
        }
    }

    private IEnumerator AutoHideTip(float delay)
    {
        yield return new WaitForSeconds(delay);
        HideTip();
    }

    public void HideTip()
    {
        if (tipText != null)
        {
            tipText.gameObject.SetActive(false);
            Debug.Log("隐藏新手提示");
        }
    }

    // 在 UIManager.cs 里添加控制暂停键显示的方法
    public void SetPauseButtonVisible(bool visible)
    {
        if (pauseButton != null)
            pauseButton.gameObject.SetActive(visible);

    }


    public void SetCurrentProgressVisible(bool visible)
    {
        progressText.gameObject.SetActive(visible);
    }

    public void ShowMainGameUI()
    {
        Debug.Log("显示主游戏界面");
        if (mainGameUI != null) mainGameUI.SetActive(true);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
        
        SetPauseButtonVisible(true);
        SetCurrentProgressVisible(true);
    }

    public void ShowPauseUI()
    {
        Debug.Log("显示暂停界面");
        if (mainGameUI != null) mainGameUI.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(true);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
        SetPauseButtonVisible(false);
    }

    public void ShowGameOverUI(float finalProgress)
    {
        Debug.Log("显示游戏结束界面");
        if (mainGameUI != null) mainGameUI.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (winPanel != null) winPanel.SetActive(false);

        if (finalProgressText != null)
        {
            finalProgressText.text = $"游戏结束!\n最终进度: {finalProgress:F1}%";
        }
    }

    public void UpdateProgressText(float progress)
    {
        if (progressText != null)
        {
            progressText.text = $"当前进度: {progress:F1}%";
        }
    }

    public void ShowGameOverUI(float finalProgress, float highScore)
    {
        Debug.Log("显示游戏结束界面（带最高分）");
        if (mainGameUI != null) mainGameUI.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (winPanel != null) winPanel.SetActive(false);
        
        finalProgressText.text = $"最终进度: {finalProgress:F1}%";
        highRecordProgressText.text = $"最高记录: {highScore:F1}%";

        // 更新尝试次数显示
        UpdateAllAttemptDisplays();
    }

    public void ShowWinPanel()
    {
        Debug.Log("显示胜利界面");
        if (winPanel != null) winPanel.SetActive(true);
        // 其它UI隐藏
        if (mainGameUI != null) mainGameUI.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        // 更新尝试次数显示
        UpdateAllAttemptDisplays();
    }


    private void OnPauseButtonClicked()
    {
        Debug.Log("暂停按钮被点击");
        if (gameManager != null)
        {
            gameManager.TogglePause();
        }
    }

    private void OnResumeButtonClicked()
    {
        Debug.Log("恢复按钮被点击");
        if (gameManager != null)
        {
            gameManager.SendMessage("TogglePause");
        }
    }

    private void OnRestartButtonClicked()
    {
        Debug.Log("重新开始按钮被点击");
        if (gameManager != null)
        {
            gameManager.SendMessage("RestartGame");
            
            // 延迟一帧后强制显示主游戏界面
            StartCoroutine(DelayedShowMainUI());
        }
    }

    private void OnQuitButtonClicked()
    {
        Debug.Log("退出按钮被点击");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
    }

    private void OnAttemptCountChanged(int newAttemptCount)
    {
        UpdateAllAttemptDisplays();
    }

    /// <summary>
    /// 更新所有尝试次数显示
    /// </summary>
    public void UpdateAllAttemptDisplays()
    {
        AttemptCounter attemptCounter = AttemptCounter.Instance;
        if (attemptCounter == null) return;

        int currentAttempts = attemptCounter.GetCurrentAttempts();
        int totalAttempts = attemptCounter.GetTotalAttempts();

        // 更新主游戏界面
        if (attemptCountText != null)
        {
            attemptCountText.text = $"尝试: {currentAttempts}";
        }

        // 更新游戏结束界面
        if (gameOverAttemptText != null)
        {
            gameOverAttemptText.text = $"本次尝试: {currentAttempts}\n总尝试: {totalAttempts}";
        }

        // 更新胜利界面
        if (winAttemptText != null)
        {
            winAttemptText.text = $"通关尝试: {currentAttempts}\n总尝试: {totalAttempts}";
        }

        //// 更新暂停界面
        //if (pauseAttemptText != null)
        //{
        //    pauseAttemptText.text = $"当前尝试: {currentAttempts}";
        //}
    }

    /// <summary>
    /// 更新尝试次数显示（供外部调用）
    /// </summary>
    public void UpdateAttemptCount(int attemptCount)
    {
        UpdateAllAttemptDisplays();
    }

    /// <summary>
    /// 隐藏所有UI界面
    /// </summary>
    public void HideAllUI()
    {
        Debug.Log("隐藏所有UI界面");
        if (mainGameUI != null) mainGameUI.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
    }
    
    /// <summary>
    /// 强制显示主游戏界面（用于重新开始后）
    /// </summary>
    public void ForceShowMainGameUI()
    {
        Debug.Log("强制显示主游戏界面");
        ShowMainGameUI();
        UpdateAllAttemptDisplays();
        
        SetPauseButtonVisible(true);
        SetCurrentProgressVisible(true); // 添加这行
        
        Debug.Log($"mainGameUI active: {mainGameUI?.activeSelf}");
        Debug.Log($"pausePanel active: {pausePanel?.activeSelf}");
        Debug.Log($"gameOverPanel active: {gameOverPanel?.activeSelf}");
        Debug.Log($"winPanel active: {winPanel?.activeSelf}");
    }

    private IEnumerator DelayedShowMainUI()
    {
        yield return null; // 等待一帧
        ForceShowMainGameUI();
        Debug.Log("重新开始后延迟显示主游戏界面");
    }

    [ContextMenu("重置新手状态")]
    public void ResetFirstTimeStatus()
    {
        PlayerPrefs.DeleteKey("HasPlayedBefore");
        Debug.Log("新手状态已重置");
    }


}