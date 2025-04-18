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
        if (reqTypeQueue.TryDequeue(out ReqRoomType _result))
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
                    ShowPartyInfo(inGameData);
                    StopUserTimer(inGameData);
                    SetBadPoint(inGameData); //���ӽ��� �� ����
                    break;

                case ReqRoomType.StageOver:
                    SetBadPoint(inGameData); //�������� ���� ��
                    break;
                case ReqRoomType.StageReady:
                    //�������� ������ �ʿ��� �ð��� ��ٷȴٰ� �ݹ����ٰ�
                    ReadyNextStage();
                    break;
                case ReqRoomType.ArrangeTurn:
                    ShowPartyInfo(inGameData);
                    ShowUserTimer(inGameData);
                    m_selectzoneColor.ChangeColor(inGameData.isMyTurn);
                    isShow = true;
                    helpTextRestTime = helpTextTime;
                    if (inGameData.curTurnId == inGameData.myId)
                    {
                        m_helpText.gameObject.SetActive(true);
                        if (inGameData.curTurn == 1)
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
                    StopUserTimer(inGameData);
                    SetBadPoint(inGameData); //����������
                    //m_badPoint.text += InputColor(" ���� : ") + inGameData.myRank.ToString();
                    break;
                case ReqRoomType.Draw:
                    isShow = true;
                    helpTextRestTime = helpTextTime;
                    m_helpText.gameObject.SetActive(true);
                    m_helpText.text = "�� �ѱ�";
                    break;
            }
        }

        CountGuideShowTime();
        CheckStageReady();
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

    private void SetBadPoint(InGameData _gameData)
    {
        //
        m_roomCharUI.ReScore(_gameData);
    }

    #region �������� �߰� ����ð�
    private float waitTime = 3f;
    private float curWaitTime = 0f;
    private bool isWait = false;

    private void ReadyNextStage()
    {
        //  Debug.Log("���������� �غ� ����");
        curWaitTime = waitTime;
        isWait = true;
    }
    private void CheckStageReady()
    {
        //��� ���� �𸣰����� 
        //�������� ������ �� �ݹ��� �޴��ؼ� �غ� �ٳ�����
        if (isWait == false)
        {
            return;
        }

        curWaitTime -= Time.deltaTime;
        if (curWaitTime < 0)
        {
            m_client.WaitStageResultCallBack();
            isWait = false;
        }
    }
    #endregion

    #region Ÿ�̸�
    private void ShowUserTimer(InGameData _gameData)
    {
        m_roomCharUI.ReTimer(_gameData);
    }

    private void StopUserTimer(InGameData _gameData)
    {
        m_roomCharUI.StopTimer(_gameData);
    }

    public void TimerExceedCallBack()
    {
        //�ð��ʰ�
        //�н� ���� ȿ��
        m_client.ExceedTimer();

    }
    #endregion
}
