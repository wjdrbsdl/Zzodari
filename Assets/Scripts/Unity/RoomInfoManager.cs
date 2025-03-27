using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomInfoManager : MonoBehaviour
{
    public TMP_Text m_roomNameText; //�� ����
    public TMP_Text m_partyText; //������
    public TMP_Text m_turnText; //���� ��������
    public TMP_Text m_preCard; //���� �����ִ� ī��

    public PlayClient m_client;

    private void Awake()
    {
        m_client.myAction = Test;
    }

    private void Test()
    {
        m_turnText.text = "�׽�Ʈ ����";
        m_turnText.gameObject.SetActive(false);
    }
}
