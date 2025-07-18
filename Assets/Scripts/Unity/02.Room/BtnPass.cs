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
    public float resetTime = 0.2f; //��ø���� Ŭ�� �ð�

    public void OnClickBtn()
    {
        if(isBtnOn == false)
        {
            return;
        }

        switch (btnType)
        {
            case BtnType.Pass:
                m_pClient.PutDownPass(); //�� ��ȯ�� ���� �������̴��� �ʱ�ȭ�ϰų� ����. 
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
