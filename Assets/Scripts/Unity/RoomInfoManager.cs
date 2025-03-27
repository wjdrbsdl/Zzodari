using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomInfoManager : MonoBehaviour
{
    public static RoomInfoManager instance;

    public TMP_Text m_roomNameText; //�� ����
    public TMP_Text m_partyText; //������
    public TMP_Text m_turnText; //���� ��������
    public TMP_Text m_preCard; //���� �����ִ� ī��

    public PlayClient m_client;

    public Queue<ReqRoomType> reqTypeQueue = new(); //�÷���Ŭ���̾�Ʈ���� � �ڵ� �۾� �ߴ���

    private void Awake()
    {
        instance = this;
    }

    public void EnqueueCode(ReqRoomType _code)
    {
        reqTypeQueue.Enqueue(_code);
    }

    private void Update()
    {
        if(reqTypeQueue.TryDequeue(out ReqRoomType _result))
        {
            switch (_result)
            {
                case ReqRoomType.RoomName:
                    m_roomNameText.text = "���̸� "+ m_client.inGameData.roomName;
                    break;
                case ReqRoomType.PartyData:
                    ShowPartyId();
                    break;
                case ReqRoomType.ArrangeTurn:
                    m_turnText.text = "���� ���� :" + m_client.inGameData.curId;
                    break;
                case ReqRoomType.PutDownCard:
                    m_preCard.text = "�� ī�� :" + m_client.inGameData.preCard;
                    break;
            }
        }
    }

    private void ShowPartyId()
    {
        m_partyText.text = "������ ";
        for (int i = 0; i < m_client.inGameData.userCount; i++)
        {
            m_partyText.text += m_client.inGameData.userIds[i] + " ";
        }
    }
}
