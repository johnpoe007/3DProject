using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int score;

    public static GameManager instance;

    public bool paused;
        
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }

        if (Time.timeScale.Equals( 0.0f))
        {
            Time.timeScale = 1.0f;
        }
    }

    public void AddScore(int scoreToGive)
    {
        score += scoreToGive;
        GameUI.instance.UpdateScoreText();
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            TogglePauseGame();
        }
    }
    
    public void TogglePauseGame()
    {
        paused = !paused;
        if (paused)
        {
            Time.timeScale = 0.0f;
        }
        else
        {
            Time.timeScale = 1.0f;
        }

        GameUI.instance.TogglePauseScreen(paused);
    }
    
    public void LevelEnd()
    {
        if (SceneManager.sceneCountInBuildSettings == SceneManager.GetActiveScene().buildIndex + 1)
        {
            WinGame();
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    public void WinGame()
    {
        GameUI.instance.SetEndGameScreen(true);
        Time.timeScale = 0.0f;
    }

    public void GameOver()
    {
        GameUI.instance.SetEndGameScreen(false);
        Time.timeScale = 0.0f;
    }
}
