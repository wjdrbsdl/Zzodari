using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BtnType
{
    None, Pass, Ready, Start, Quit
}

public class BtnPass : MonoBehaviour
{
    public BtnType btnType = BtnType.None;
    public PlayClient m_pClient;
    public bool isBtnOn = true;
    public float resetTime = 0.2f; //중첩방지 클릭 시간

    public void OnClickBtn()
    {
        if(isBtnOn == false)
        {
            return;
        }

        switch (btnType)
        {
            case BtnType.Pass:
                m_pClient.PutDownPass(); //불 반환을 통해 선택중이던걸 초기화하거나 가능. 
                break;
            case BtnType.Ready:
                if(TicketManager.Instance.HaveTicket() == false)
                {
                    return;
                }
                m_pClient.ReqGameReady();
                break;
            case BtnType.Start:
                if (TicketManager.Instance.HaveTicket() == false)
                {
                    return;
                }
                m_pClient.ReqGameStart();

                break;
            case BtnType.Quit:
                m_pClient.ReqRoomOut();
                break;
        }
        StartTimer();
    }

    private void StartTimer()
    {
        StartCoroutine(CoTimer());
    }

    IEnumerator CoTimer()
    {
        isBtnOn = false;
        yield return new WaitForSeconds(resetTime);
        isBtnOn = true;
    }

    private void OnEnable()
    {
        StopAllCoroutines();
        isBtnOn = true;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        isBtnOn = false;
    }
}
