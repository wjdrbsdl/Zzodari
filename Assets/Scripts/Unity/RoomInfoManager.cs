using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomInfoManager : MonoBehaviour
{
    public static RoomInfoManager instance;

    public TMP_Text m_roomNameText; //방 제목
    public TMP_Text m_partyText; //참가원
    public TMP_Text m_turnText; //지금 누구차레
    public TMP_Text m_preCard; //지금 놓여있는 카드

    public PlayClient m_client;

    public Queue<ReqRoomType> reqTypeQueue = new(); //플레이클라이언트에서 어떤 핸들 작업 했는지

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
                    m_roomNameText.text = "방이름 "+ m_client.inGameData.roomName;
                    break;
                case ReqRoomType.PartyData:
                    ShowPartyId();
                    break;
                case ReqRoomType.ArrangeTurn:
                    m_turnText.text = "현재 차례 :" + m_client.inGameData.curId;
                    break;
                case ReqRoomType.PutDownCard:
                    m_preCard.text = "전 카드 :" + m_client.inGameData.preCard;
                    break;
            }
        }
    }

    private void ShowPartyId()
    {
        m_partyText.text = "참가자 ";
        for (int i = 0; i < m_client.inGameData.userCount; i++)
        {
            m_partyText.text += m_client.inGameData.userIds[i] + " ";
        }
    }
}
