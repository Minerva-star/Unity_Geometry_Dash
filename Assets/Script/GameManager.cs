using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public enum GameState
    {
        Playing,
        Paused,
        GameOver
    }

    private GameState currentState = GameState.Playing;

    public PlayerBehavior player;
    public UIManager uiManager;


    public float totalMapLength = 242f;
    private float currentProgress = 0f;



    public GameObject pausePanel;
    public GameObject gameOverUI;
    public Text finalProgressText;
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

        player.OnPlayerDeath += OnPlayerDeath;
        gameOverUI.SetActive(false);


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
        currentProgress = (player.transform.position.x) / totalMapLength * 100;
        currentProgress = Mathf.Clamp(currentProgress, 0f, 100f);
        uiManager.UpdateProgressText(currentProgress);
    }

    private void OnPlayerDeath()
    {
        currentState = GameState.GameOver;
        gameOverUI.SetActive(true);

        //最高记录
        if (currentProgress > highScore)
        {
            highScore = currentProgress;
            SaveHighScore();
        }

        uiManager.ShowGameOverUI(currentProgress, highScore);
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
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

    }



    public void RestartGame()
    {
        Time.timeScale = 1f;
        player.Reset();

        InitializeGame();

    }





}
