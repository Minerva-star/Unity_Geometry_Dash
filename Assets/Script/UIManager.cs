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

    // 引用
    public GameManager gameManager;


    private void Start()
    {
        SetupUI();
        ShowMainGameUI();
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
        if (mainGameUI != null) mainGameUI.SetActive(true);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    public void ShowPauseUI()
    {
        if (mainGameUI != null) mainGameUI.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(true);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        SetPauseButtonVisible(false);
    }

    public void ShowGameOverUI(float finalProgress)
    {
        if (mainGameUI != null) mainGameUI.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(true);

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
        finalProgressText.text = $"最终进度: {finalProgress:F1}%";
        highRecordProgressText.text = $"最高记录: {highScore:F1}%";

    }


    private void OnPauseButtonClicked()
    {
        if (gameManager != null)
        {
            gameManager.TogglePause();
        }
    }

    private void OnResumeButtonClicked()
    {
        if (gameManager != null)
        {
            gameManager.SendMessage("TogglePause");
        }
    }

    private void OnRestartButtonClicked()
    {
        if (gameManager != null)
        {
            gameManager.SendMessage("RestartGame");
        }
    }

    private void OnQuitButtonClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
    }





}