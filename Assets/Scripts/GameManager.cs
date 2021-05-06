using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
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
    public TMP_Text ammoText1, ammoText2;

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
        _nPlayers = PlayerPrefs.HasKey("players") ? PlayerPrefs.GetInt("players") : 1;

        PlayerPrefs.SetInt("players", _nPlayers);  // remember nPlayers for next time
        
        _lives = maxLives;
        if (_nPlayers == 1)
            players[1].gameObject.SetActive(false);

        Physics2D.IgnoreLayerCollision(6, 6);

        Pause(false);
    }

    private void Update()
    {
        var gameProgress = EnemyManager.Singleton.count / 100;
        if (!paused)
        {
            _time += Time.deltaTime;
            // progressBar.fillAmount = _time / levelTime;
            progressBar.fillAmount = gameProgress;
            ammoText1.text = players[0].bullets < 0 ? "∞" : players[0].bullets.ToString();
            if (_nPlayers == 1)
                ammoText2.gameObject.SetActive(false);
            else
                ammoText2.text = players[1].bullets < 0 ? "∞" : players[1].bullets.ToString();

            points = players.Select(x => x.Score).Sum();
            pointsText.text = points + " pts";
        }
        
        if (!paused && (_time > levelTime && false || _lives <= 0 || gameProgress >= 1f))
        {
            GameOver();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause(!paused);
        }
        
        // DEBUG:
        if (Input.GetKeyDown(KeyCode.B))
        {
            GameOver();
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
        
        Sequence gameOverSequence = DOTween.Sequence();

        for (int i = 0; i < players.Count; i++)
        {
            deathsTexts[i].text = hitsTexts[i].text = scoreTexts[i].text = "";
            if (i == _nPlayers)
            {
                deathsTexts[i].text = hitsTexts[i].text = scoreTexts[i].text = "-";
                continue;
            }
            
            gameOverSequence.Append(AcumulateText(
                deathsTexts[i], 1.5f,
                new[] {players[i].hits.Count, players[i].hits.Sum()}, "{0} (+{1} pts)"));
            gameOverSequence.Append(AcumulateText(
                hitsTexts[i], 1.5f, 
                new[] {players[i].hitsByEnemy, 10*players[i].hitsByEnemy}, "{0} (-{1} pts)"));
            gameOverSequence.Append(AcumulateText(
                scoreTexts[i], 1.5f, new[] {players[i].Score}, "{0}"));
        }

        gameOverSequence.Append(finalScoreText.DOFade(0f, 0.5f).From());
        gameOverSequence.Join(finalScoreText.transform.DOMoveY(
            finalScoreText.transform.position.y - 800, 0.5f, true).From());
        gameOverSequence.Append(AcumulateText(finalScoreText, 2f, new[] {points}, "{0}"));
        gameOverSequence.Join(finalScoreText.transform.DOScale(Vector3.one * 1.5f, 2f)).SetEase(Ease.OutBack).OnComplete(() =>
        {
            CoolRotate(finalScoreText.transform, Vector3.forward * 12f, 1.6f);
        });
        
        gameOverSequence.SetUpdate(true);
    }

    private Tween AcumulateText(TMP_Text text, float totalDuration, int[] values, string str)
    {
        var max = Mathf.Max(1, values.Max());
        var counts = new int[values.Length];
        var iter = 0;
        var dummy = 0; // this is garbage
        return DOTween.To(() => dummy, x => dummy = x, 100, totalDuration/max).SetLoops(max).SetUpdate(true).OnStepComplete(() =>
        {
            for (var i = 0; i < counts.Length; i++)
            {
                var d = (values[i] * iter) / (1f * max);
                if (counts[i] < values[i] && d % 1 == 0)
                {
                    counts[i]++;
                }
            }
            text.text = string.Format(str, counts.Select(x => x.ToString()).ToArray());
            iter++;
        });
    }

    private Tween CoolRotate(Transform t, Vector3 desiredAngle, float duration)
    {
        return t.DORotate(desiredAngle, duration/2f).SetUpdate(true).OnComplete(() =>
        {
            t.DORotate(-desiredAngle, duration).
                SetEase(Ease.InOutQuad).SetUpdate(true).SetLoops(-1, LoopType.Yoyo);
        });
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
