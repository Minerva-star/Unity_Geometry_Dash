using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{

    public enum GameState
    {
        Playing,
        Paused,
        GameOver,
        Victory
    }

    private GameState currentState = GameState.Playing;

    public PlayerBehavior player;
    public UIManager uiManager;
    public BackgroundMover background;

    [Header("距离计算")]
    public Transform finishLine; // 终点线物体
    public float totalMapLength = 242f; // 手动设置的总长度（如果finishLine为空则使用此值）
    private float currentProgress = 0f;



    public GameObject pausePanel;
    public GameObject gameOverUI;
    public TextMeshProUGUI finalProgressText;
    public Button restartButton;


    private float highScore = 0f;


    private void Start()
    {

        LoadHighScore();
        InitializeGame();
        SetupUI();
    }

    private void LoadHighScore()
    {
        highScore = PlayerPrefs.GetFloat("HighScore", 0f);
    }

    private void SaveHighScore()
    {
        // 保存最高记录到PlayerPrefs
        PlayerPrefs.SetFloat("HighScore", highScore);
        PlayerPrefs.Save(); // 立即保存
        print($"保存最高记录: {highScore:F1}%");
    }


    private void InitializeGame()
    {
        currentState = GameState.Playing;
        currentProgress = 0f;
        background.SetPaused(false);
        player.OnPlayerDeath += OnPlayerDeath;
        gameOverUI.SetActive(false);

        if (uiManager != null)
        {  uiManager.SetPauseButtonVisible(true);
         uiManager.SetCurrentProgressVisible(true);    
        }
        
        GameObject[] loseParticles = GameObject.FindGameObjectsWithTag("LoseParticle");
        foreach (GameObject particle in loseParticles)
        {
            Destroy(particle);
        }



    }

    private void SetupUI()
    {
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);

        if (pausePanel != null) pausePanel.SetActive(false);
        if (gameOverUI != null) gameOverUI.SetActive(false);


    }


    private void Update()
    {
        if (currentState != GameState.Playing) return;

        UpdateProgress();
    }


    private void UpdateProgress()
    {
        // 计算实际的总长度
        float actualTotalLength = GetTotalMapLength();
        
        // 计算进度百分比
        currentProgress = (player.transform.position.x) / actualTotalLength * 100;
        currentProgress = Mathf.Clamp(currentProgress, 0f, 100f);
        uiManager.UpdateProgressText(currentProgress);
    }
    
    // 获取总地图长度
    private float GetTotalMapLength()
    {
        // 如果设置了终点线，使用终点线的X坐标作为总长度
        if (finishLine != null)
        {
            return finishLine.position.x;
        }
        else
        {
            // 否则使用手动设置的值
            return totalMapLength;
        }
    }

    private void OnPlayerDeath()
    {
        currentState = GameState.GameOver;
        gameOverUI.SetActive(true);
        if (background != null)
            background.SetPaused(true); // 暂停背景

        //最高记录
        if (currentProgress > highScore)
        {
            highScore = currentProgress;
            SaveHighScore();
        }
        SetMusicPause(true);
        uiManager.ShowGameOverUI(currentProgress, highScore);
        uiManager.SetCurrentProgressVisible(false);    
    }

    public float GetHighScore()
    {
        return highScore;
    }

    // 重置最高记录的方法（可选）
    public void ResetHighScore()
    {
        highScore = 0f;
        PlayerPrefs.DeleteKey("HighScore");
        print("最高记录已重置");
    }

    public void TogglePause()
    {
        if (currentState == GameState.Playing)
        {
            PauseGame();
        }
        else if (currentState == GameState.Paused)
        {
            ResumeGame();
        }
    }

    private void PauseGame()
    {
        currentState = GameState.Paused;
        Time.timeScale = 0f;

        player.SetGamePaused(true);
        background.SetPaused(true);
        SetMusicPause(true);
        
        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }

    }


    private void ResumeGame()
    {
        currentState = GameState.Playing;
        Time.timeScale = 1f;

        if (player != null)
        {
            player.SetGamePaused(false);
        }
        if (background != null)
        {
            background.SetPaused(false);
        }
        SetMusicPause(false); // 恢复音乐播放
        
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
        if (uiManager != null)
            uiManager.SetPauseButtonVisible(true); // 显示暂停按钮
    }



    public void RestartGame()
    {
        Time.timeScale = 1f;
        player.Reset();
        if (background != null)
            background.ResetBackground(); 
        // 重新播放音乐
        var cam = Camera.main;
        if (cam != null)
        {
            var audio = cam.GetComponent<AudioSource>();
            if (audio != null)
            {
                audio.Stop();
                audio.Play();
            }
        }

        InitializeGame();

    }

    public void SetMusicPause( bool paused)
    {
        // 重新播放音乐
        var cam = Camera.main;
        if (cam != null)
        {
            var audio = cam.GetComponent<AudioSource>();
            if (paused)
                audio.Pause();
            else
                audio.UnPause();
        }
    }
    
    // 设置终点线
    public void SetFinishLine(Transform finishLineTransform)
    {
        finishLine = finishLineTransform;
        Debug.Log($"设置终点线位置: {finishLine.position.x}");
    }
    
    // 获取当前总长度
    public float GetCurrentTotalLength()
    {
        return GetTotalMapLength();
    }
    
    // 检查玩家是否到达终点
    public bool IsPlayerAtFinish()
    {
        if (finishLine == null) return false;
        return player.transform.position.x >= finishLine.position.x;
    }
    
    // 获取玩家到终点的距离
    public float GetDistanceToFinish()
    {
        if (finishLine == null) return 0f;
        return finishLine.position.x - player.transform.position.x;
    }



}
