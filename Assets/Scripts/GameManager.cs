using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("HUD")]
    public Transform livesParent;
    public GameObject pausePanel;
    public Image progressBar;
    public TMP_Text pointsText;

    [Header("GameOver")]
    public GameObject gameOverPanel;
    public TMP_Text[] deathsTexts, hitsTexts, scoreTexts;
    public TMP_Text finalScoreText;

    [Header("Game")]
    public List<Player> players;
    public int maxLives = 3;
    public int points;
    public float levelTime;
    public List<WeaponStats> weaponStatsList;
    public Transform itemParent;
    
    private int _lives;
    private float _time;
    private int _nPlayers;

    public static GameManager Singleton;
    public static bool paused;

    private void Awake()
    {
        Singleton = this;
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("players"))
            _nPlayers = PlayerPrefs.GetInt("players");  // can only be 1 or 2
        else
            _nPlayers = 1;

        PlayerPrefs.SetInt("players", _nPlayers);  // remember nPlayers for next time
        
        _lives = maxLives;
        if (_nPlayers == 1)
            players[1].gameObject.SetActive(false);
        
        Pause(false);
    }

    private void Update()
    {
        if (!paused)
        {
            _time += Time.deltaTime;
            progressBar.fillAmount = _time / levelTime;

            points = players.Select(x => x.Score).Sum();
            pointsText.text = points + " pts";
        }
        
        if (_time > levelTime || _lives <= 0)
        {
            GameOver();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause(!paused);
        }
    }

    public void Pause(bool value, bool showPanel=true)
    {
        paused = value;
        if (showPanel)
            pausePanel.SetActive(paused);
        Time.timeScale = paused ? 0f : 1f;
        AudioListener.pause = paused;
    }

    private void GameOver()
    {
        Pause(true, false);
        gameOverPanel.SetActive(true);
        for (int i = 0; i < players.Count; i++)
        {
            if (i == _nPlayers)
            {
                deathsTexts[i].text = hitsTexts[i].text = scoreTexts[i].text = "-";
                continue;
            }

            deathsTexts[i].text = players[i].hits.Count + " (+" + players[i].hits.Sum() + " pts)";
            hitsTexts[i].text = players[i].hitsByEnemy + " (-" + 10*players[i].hitsByEnemy + " pts)";
            scoreTexts[i].text = players[i].Score.ToString();
        }

        finalScoreText.text = points.ToString();
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit()
    {
        SceneManager.LoadScene(0);
    }
    
    public void HitByEnemy(Player p, Enemy e)
    {
        _lives--;
        livesParent.GetChild(_lives).gameObject.SetActive(false);
        print("player " + p.playerIdx + " was hit by " + e.name);
        p.hitsByEnemy++;
        p.Score -= 10;
    }

    public void HitEnemy(Player p, Enemy e)
    {
        int score = e.Level;
        p.hits.Add(score);
        p.Score += score;
    }
}
