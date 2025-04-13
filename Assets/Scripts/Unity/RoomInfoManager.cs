using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomInfoManager : MonoBehaviour
{
    public static RoomInfoManager instance;

    public UIRoomCharactor m_roomCharUI;
    public TMP_Text m_roomNameText; //�� ����
    public TMP_Text m_preCard; //���� �����ִ� ī��
    public TMP_Text m_helpText;

    public SelectZoneColorController m_selectzoneColor;
    public TurnTimer m_turnTimer;
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

    private string InputColor(string _string)
    {
        return $"<color=green>{_string}</color>";
    }

    float helpTextTime = 4.5f;
    float helpTextRestTime = 0f;
    bool isShow = false;
    private void Update()
    {
        if(reqTypeQueue.TryDequeue(out ReqRoomType _result))
        {
            m_helpText.gameObject.SetActive(false);
            InGameData inGameData = m_client.GetInGameData();
            switch (_result)
            {
                case ReqRoomType.RoomName:
                    m_roomNameText.text = InputColor("���̸� ") + inGameData.roomName;
                    break;
                case ReqRoomType.PartyData:
                case ReqRoomType.Start:
                case ReqRoomType.StageOver:
                    ShowPartyInfo(inGameData);
                    break;
                case ReqRoomType.ArrangeTurn:
                    ShowPartyInfo(inGameData);
                    m_selectzoneColor.ChangeColor(inGameData.isMyTurn);
                    isShow = true;
                    helpTextRestTime = helpTextTime;
                    if (inGameData.curId == inGameData.myId)
                    {
                        ShowTimer(inGameData);
                        m_helpText.gameObject.SetActive(true);
                        if(inGameData.curTurn == 1)
                        {
                            m_helpText.text = "Ŭ�ι� 3�� ���� ���ϴ� �������� �� �� �ֽ��ϴ�.";
                        }
                        else if (inGameData.allPass)
                        {
                            m_preCard.text = InputColor("�� ī�� :") + "�� �о�!";
                            m_helpText.text = "���ϴ� �������� �� �� �ֽ��ϴ�.";
                        }
                        else
                        {
                            m_helpText.text = inGameData.preCardCount + "�� ���� �� �� �ֽ��ϴ�.";
                        }
                    }
                    break;
                case ReqRoomType.PutDownCard:
                    ShowPutDownCardInfo(inGameData);
                    
                    break;
                case ReqRoomType.IDRegister:
                    ShowPartyInfo(inGameData);
                    break;
             
                case ReqRoomType.GameOver:
                    ShowPartyInfo(inGameData);
                    //m_badPoint.text += InputColor(" ���� : ") + inGameData.myRank.ToString();
                    break;
                case ReqRoomType.Draw:
                    OffTimer();
                    isShow = true;
                    helpTextRestTime = helpTextTime;
                    m_helpText.gameObject.SetActive(true);
                    m_helpText.text = "�� �ѱ�";
                    break;
            }
        }

        CountGuideShowTime();
    }

    private void CountGuideShowTime()
    {
        if (isShow == false)
            return;

        helpTextRestTime -= Time.deltaTime;
        if (helpTextRestTime < 0)
        {
            m_helpText.gameObject.SetActive(false);
            isShow = false;
        }
    }

    private void ShowPartyInfo(InGameData _gameData)
    {
        m_roomCharUI.Renew(_gameData);
    }

    private void ShowPutDownCardInfo(InGameData _gameData)
    {
        m_preCard.text = InputColor("�� ī�� :") + _gameData.preCard; //� ī�� �´���
        ShowPartyInfo(_gameData);
    }

    private void ShowTimer(InGameData _gameData)
    {
        m_turnTimer.CountMyTurn(this);
    }

    private void OffTimer()
    {
        m_turnTimer.EndMyTurn();
    }

    public void TimerExceedCallBack()
    { 
        //�ð��ʰ�
        //�н� ���� ȿ��
        if(m_client.PutDownPass() == false)
        {
            //���� �н� �Ұ���� -> ���н����� �ڱ�����
            //���� ���� ī�� 1�� ����
            List<CardData> putList = new();
            m_client.SortCardList(); //���� -> ���� ���۽ÿ� �ڵ����� Ŭ�ι� 3������
            putList.Add(m_client.GetHaveCardList()[0]);
            m_client.PutDownCards(putList);
        }
    }
}
