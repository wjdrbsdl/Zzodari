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
    public TMP_Text m_myIdText;
    public TMP_Text m_badPoint;
    public TMP_Text m_helpText;

    public SelectZoneColorController m_selectzoneColor;
    public PlayClient m_client;

    public Queue<ReqRoomType> reqTypeQueue = new(); //�÷���Ŭ���̾�Ʈ���� � �ڵ� �۾� �ߴ���

    private void Awake()
    {
        instance = this;
        m_badPoint.text = "0";
    }

    public void EnqueueCode(ReqRoomType _code)
    {
        reqTypeQueue.Enqueue(_code);
    }

    private string InputColor(string _string)
    {
        return $"<color=green>{_string}</color>";
    }

    float showTime = 4.5f;
    float restTime = 0f;
    bool isShow = false;
    private void Update()
    {
        if(reqTypeQueue.TryDequeue(out ReqRoomType _result))
        {
            m_helpText.gameObject.SetActive(false);
            switch (_result)
            {
                case ReqRoomType.RoomName:
                    m_roomNameText.text = InputColor("���̸� ") + m_client.inGameData.roomName;
                    break;
                case ReqRoomType.PartyData:
                    ShowPartyId();
                    break;
                case ReqRoomType.ArrangeTurn:
                    m_turnText.text = InputColor("���� ���� :") + m_client.inGameData.curId;
                    m_selectzoneColor.ChangeColor(m_client.inGameData.isMyTurn);
                    isShow = true;
                    restTime = showTime;
                    if (m_client.inGameData.curId == m_client.inGameData.myId)
                    {
                        m_helpText.gameObject.SetActive(true);
                        if(m_client.inGameData.curTurn == 1)
                        {
                            m_helpText.text = "Ŭ�ι� 3�� ���� ���ϴ� �������� �� �� �ֽ��ϴ�.";
                        }
                        else if (m_client.inGameData.allPass)
                        {
                            m_preCard.text = InputColor("�� ī�� :") + "�� �о�!";
                            m_helpText.text = "���ϴ� �������� �� �� �ֽ��ϴ�.";
                        }
                        else
                        {
                            m_helpText.text = m_client.inGameData.preCardCount + "�� ���� �� �� �ֽ��ϴ�.";
                        }
                    }
                    break;
                case ReqRoomType.PutDownCard:
                    m_preCard.text = InputColor("�� ī�� :") + m_client.inGameData.preCard;
                    break;
                case ReqRoomType.IDRegister:
                    m_myIdText.text = InputColor("�� ���̵� : ") + m_client.inGameData.myId;
                    break;
                case ReqRoomType.Start:
                    m_badPoint.text = InputColor("���� : ") + m_client.inGameData.badPoint.ToString();
                    break;
                case ReqRoomType.StageOver:
                    m_badPoint.text = InputColor("���� : ") + m_client.inGameData.badPoint.ToString();
                    break;
                case ReqRoomType.GameOver:
                    m_badPoint.text = InputColor("���� : ") + m_client.inGameData.badPoint.ToString();
                    m_badPoint.text += InputColor(" ���� : ") + m_client.inGameData.myRank.ToString();
                    break;
                case ReqRoomType.Draw:
                    isShow = true;
                    restTime = showTime;
                    m_helpText.gameObject.SetActive(true);
                    m_helpText.text = "�� �ѱ�";
                    break;
            }
        }

        CountGuide();
    }

    private void CountGuide()
    {
        if (isShow == false)
            return;

        restTime -= Time.deltaTime;
        if (restTime < 0)
        {
            m_helpText.gameObject.SetActive(false);
            isShow = false;
        }
    }

    private void ShowPartyId()
    {
        m_partyText.text = InputColor("������ ");
        for (int i = 0; i < m_client.inGameData.userCount; i++)
        {
            m_partyText.text += m_client.inGameData.userIds[i] + " ";
        }
    }
}
