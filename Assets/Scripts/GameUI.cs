using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    
    public GameObject endScreen;
    public TextMeshProUGUI endScreenHeader;
    public TextMeshProUGUI endScreenScoreText;

    public GameObject pauseScreen;
    
    //instance
    public static GameUI instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        UpdateScoreText();
    }
    
    public void UpdateScoreText()
    {
        scoreText.text = "Score " + GameManager.instance.score;
    }

    public void SetEndGameScreen(bool hasWon)
    {
        endScreen.SetActive(true);
        endScreenScoreText.text = "<b>Score</b>\n" + GameManager.instance.score;

        
        if (hasWon)
        {
            endScreenHeader.color = Color.green;
            endScreenHeader.text = "You Win";
        }
        else
        {
            endScreenHeader.color = Color.red;
            endScreenScoreText.text = "Game Over";
        }
    }

    public void OnRestartButton()
    {
        SceneManager.LoadScene(1);
    }

    public void OnMenuButton()
    {
        if(GameManager.instance.paused)
            GameManager.instance.TogglePauseGame();
        SceneManager.LoadScene(0);
        
    }

    public void TogglePauseScreen(bool paused)
    {
        pauseScreen.SetActive(paused);
    }

    public void OnResumeButton()
    {
        GameManager.instance.TogglePauseGame();
    }
}
