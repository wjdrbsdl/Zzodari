using TMPro;
using UnityEngine;

public class UITicket : MonoBehaviour
{
    [SerializeField] private TMP_Text m_ticketState;

    private void Start()
    {
        
    }

    private void SetInfo()
    {
        if(TicketManager.Instance.CurChance >= 0)
        {
            m_ticketState.text = TicketManager.Instance.CurChance + " / " + TicketManager.Instance.MaxChance;
            return;
        }

        m_ticketState.text = FloatToMinuteSecond(TicketManager.Instance.RestTime);
    }

    private string FloatToMinuteSecond(float timeInSeconds)
    {
        int minutes = (int)(timeInSeconds / 60);
        int seconds = (int)(timeInSeconds % 60);
        return $"{minutes:D2}:{seconds:D2}";
    }
}
