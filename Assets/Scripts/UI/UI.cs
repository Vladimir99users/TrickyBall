using TMPro;
using UnityEngine;
using UnityEngine.Events;
public class UI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textMoney;
    [SerializeField] private CanvasGroup _groupDeadPanel;
    [SerializeField] private CanvasGroup _groupWinPanel;
    [SerializeField] private MovementOnClick _movement;
    [SerializeField] private int count = 0;
    internal UnityEvent<int> OnCollectMoney = new UnityEvent<int>();
    internal UnityEvent OnActionTrap = new UnityEvent();
    private int _score = 0;

    private void OnEnable()
    {
        ResetDeadPanle();
        ResetWinPanle();
        OnCollectMoney.AddListener(IncreaseCountCoins);
        OnActionTrap.AddListener(ActiveDeadPanel);
    }

    private void ResetDeadPanle()
    {
        _groupDeadPanel.alpha = 0;
       _groupDeadPanel.blocksRaycasts = false;
       _groupDeadPanel.interactable = false;
    }
    private void ResetWinPanle()
    {
       _groupWinPanel.alpha = 0;
       _groupWinPanel.blocksRaycasts = false;
       _groupWinPanel.interactable = false;
    }

    private void OnDisable()
    {
        OnCollectMoney.RemoveListener(IncreaseCountCoins);
        OnActionTrap.RemoveListener(ActiveDeadPanel);
    }
    private void IncreaseCountCoins(int addCoins)
    {
       _score += addCoins;
       count--;
       UpdateText();
       if(count <= 0)
        ActiveWINPanel();
    }
    private void UpdateText()
    {
        _textMoney.text = "Money = " + _score;
    }

    private void ActiveWINPanel()
    {
        _movement.StopPlayer();
        WinPlayer();
    }

    private void WinPlayer()
    {
       _groupWinPanel.alpha = 1;
       _groupWinPanel.blocksRaycasts = true;
       _groupWinPanel.interactable = true;
    }

    private void ActiveDeadPanel()
    {
        _movement.StopPlayer();
        DeadPlayer();
    }

    private void DeadPlayer()
    {
       _groupDeadPanel.alpha = 1;
       _groupDeadPanel.blocksRaycasts = true;
       _groupDeadPanel.interactable = true;
    }

   
}
