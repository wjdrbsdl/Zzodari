using TMPro;
using UnityEngine;

public class UITicket : MonoBehaviour
{
    [SerializeField] private TMP_Text m_ticketState;

    private void OnEnable()
    {
        TicketManager.Instance.OnChangeRestTime += SetTimerInfo;
        TicketManager.Instance.OnChangeTicketAmount += SetTicketInfo;
    }

    private void OnDisable()
    {
        TicketManager.Instance.OnChangeRestTime -= SetTimerInfo;
        TicketManager.Instance.OnChangeTicketAmount -= SetTicketInfo;
    }

    private void SetTicketInfo(int curChance)
    {
        m_ticketState.text = curChance + " / " + TicketManager.Instance.MaxChance;
    }

    private void SetTimerInfo(float restTime)
    {
        m_ticketState.text = FloatToMinuteSecond(restTime);
    }

    private string FloatToMinuteSecond(float timeInSeconds)
    {
        int minutes = (int)(timeInSeconds / 60);
        int seconds = (int)(timeInSeconds % 60);
        return $"{minutes:D2}:{seconds:D2}";
    }
}
