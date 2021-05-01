using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public TMP_Text mainTitle;
    public Image telon;
    public Button[] buttons;
    public Button helpButton;
    
    private void Start()
    {
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
        Sequence mySequence = DOTween.Sequence();
        telon.gameObject.SetActive(true);
        telon.DOFade(1f, 0f).Complete();
        mySequence.Append(telon.DOFade(0f, 1f));
        // mySequence.AppendInterval(0.4f);
        mySequence.Append(mainTitle.DOFade(0f, 0.6f).From().SetEase(Ease.InOutQuint));
        mySequence.Join(mainTitle.transform.DOScale(Vector3.one * 5, 0.7f).From().SetEase(Ease.OutBack, 0.6f));
        mySequence.AppendInterval(0.7f);
        
        // mainTitle.DOPunchScale(Vector3.one * 1.1f, 0.2f, 10, 0f);
        // Camera.main.DOShakePosition(0.1f, 0.2f, 3);
        for (int i = 0; i < buttons.Length; i++)
        {
            mySequence.Join(buttons[i].transform.DOMoveY(
                    buttons[i].transform.position.y - 600, 1.8f, true).
                From().SetDelay(i*0.2f).SetEase(Ease.OutBack, 0.6f));
        }

        mySequence.Append(helpButton.transform.DOMoveX(
            helpButton.transform.position.x - 200, 0.9f, true).From().SetEase(Ease.OutQuart).SetDelay(0.3f));
    }
}
