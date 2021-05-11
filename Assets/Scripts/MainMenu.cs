using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public RectTransform mainPanel;
    public TMP_Text mainTitle;
    public Image telon;
    public Transform[] buttons;
    public Toggle skipIntro;
    public string[] links;

    public Image pj1, pj2, bird;
    public Sprite pj1Angry, pj2Angry;
    private AudioSource _audio;
    [SerializeField] private AudioClip birdClip;

    private IEnumerator Start()
    {
        Time.timeScale = 1f;
        _audio = GetComponent<AudioSource>();
        telon.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        Init();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            Init();
        }
    }

    private void Init()
    {
        //mainPanel.sizeDelta = CameraLetterBox.Singleton.r.size;
        //mainPanel.SetLeft(131.1794f); // 131.1794
        //mainPanel.SetRight(131.1794f);
        //mainPanel.ForceUpdateRectTransforms();
        
        Sequence mySequence = DOTween.Sequence();
        telon.gameObject.SetActive(true);
        telon.DOFade(1f, 0f).Complete();
        mySequence.Append(telon.DOFade(0f, 1f));
        
        mySequence.Append(bird.transform.DOMoveX(
            bird.transform.position.x + 850, 1.43f, true).From().SetEase(Ease.OutQuart).SetDelay(0.3f).OnComplete(() =>
        {
            pj1.sprite = pj1Angry;
            pj2.sprite = pj2Angry;
            var anim = bird.GetComponent<Animator>();
            anim.SetTrigger("shout");
            anim.speed = 1.5f;
            _audio.clip = birdClip;
            _audio.Play();
        }));
        mySequence.Join(pj1.transform.DOMoveX(
            pj1.transform.position.x - 800, 1.35f, true).From().SetEase(Ease.OutQuart).SetDelay(0.3f));
        mySequence.Join(pj2.transform.DOMoveX(
            pj2.transform.position.x - 800, 1.35f, true).From().SetEase(Ease.OutQuart).SetDelay(0.3f));
        
        
        // mySequence.AppendInterval(0.4f);
        mySequence.Append(mainTitle.DOFade(0f, 0.6f).From().SetEase(Ease.InOutQuint));
        mySequence.Join(mainTitle.transform.DOScale(Vector3.one * 5, 0.7f).From().SetEase(Ease.OutBack, 0.6f));
        mySequence.AppendInterval(0.7f);
        
        // mainTitle.DOPunchScale(Vector3.one * 1.1f, 0.2f, 10, 0f);
        // Camera.main.DOShakePosition(0.1f, 0.2f, 3);
        for (int i = 0; i < buttons.Length; i++)
        {
            mySequence.Join(buttons[i].GetComponentInChildren<Image>().DOFade(0f, 2f).From().SetDelay(i*0.02f));
            mySequence.Join(buttons[i].GetComponentInChildren<TMP_Text>().DOFade(0f, 2f).From().SetDelay(i*0.02f));
            mySequence.Join(buttons[i].DOMoveY(
                    buttons[i].position.y - 1000, 2f, true).
                From().SetDelay(i*0.02f).SetEase(Ease.OutBack, 0.6f));
        }
    }

    public void Play(int nPlayers)
    {
        PlayerPrefs.SetInt("players", nPlayers);
        PlayerPrefs.SetInt("skipIntro", skipIntro.isOn ? 1 : 0);
        SceneManager.LoadScene(1);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void OpenLink(int index)
    {
        Application.OpenURL(links[index]);
    }
}
