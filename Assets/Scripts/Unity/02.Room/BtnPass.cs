using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtnPass : MonoBehaviour
{
    public PlayClient m_pClient;

    public void OnClickPassBtn()
    {
        //���� �������̴��Ͱ� ������ ������ �н��� ���� ���ְ�
        m_pClient.PutDownPass(); //�� ��ȯ�� ���� �������̴��� �ʱ�ȭ�ϰų� ����. 
    }

    public void OnClickStartBtn()
    {
        m_pClient.ReqGameStart();
    }

    public void OnClickQuitBtn()
    {
        m_pClient.ReqRoomOut();
    }
}
