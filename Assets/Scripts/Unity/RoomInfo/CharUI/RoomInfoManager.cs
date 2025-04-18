using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomInfoManager : MonoBehaviour
{
    public static RoomInfoManager instance;

    public UIRoomCharactor m_roomCharUI;
    public TMP_Text m_roomNameText; //방 제목
    public TMP_Text m_preCard; //지금 놓여있는 카드
    public TMP_Text m_helpText;

    public SelectZoneColorController m_selectzoneColor;
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
                    m_roomNameText.text = InputColor("방이름 ") + inGameData.roomName;
                    break;
                case ReqRoomType.PartyData:
                case ReqRoomType.Start:
                    ShowPartyInfo(inGameData);
                    StopUserTimer(inGameData);
                    SetBadPoint(inGameData); //게임시작 후 갱신
                    break;

                case ReqRoomType.StageOver:
                    SetBadPoint(inGameData); //스테이지 종료 후
                    break;
                case ReqRoomType.StageReady:
                    //스테이지 정리에 필요한 시간을 기다렸다가 콜백해줄곳
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
                            m_helpText.text = "클로버 3을 포함 원하는 조합으로 낼 수 있습니다.";
                        }
                        else if (inGameData.allPass)
                        {
                            m_preCard.text = InputColor("전 카드 :") + "올 패쓰!";
                            m_helpText.text = "원하는 조합으로 낼 수 있습니다.";
                        }
                        else
                        {
                            m_helpText.text = inGameData.preCardCount + "장 조합 낼 수 있습니다.";
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
                    SetBadPoint(inGameData); //게임종료후
                    //m_badPoint.text += InputColor(" 순위 : ") + inGameData.myRank.ToString();
                    break;
                case ReqRoomType.Draw:
                    isShow = true;
                    helpTextRestTime = helpTextTime;
                    m_helpText.gameObject.SetActive(true);
                    m_helpText.text = "턴 넘김";
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
        m_preCard.text = InputColor("전 카드 :") + _gameData.preCard; //어떤 카드 냈는지
        ShowPartyInfo(_gameData);
    }

    private void SetBadPoint(InGameData _gameData)
    {
        //
        m_roomCharUI.ReScore(_gameData);
    }

    #region 스테이지 중간 정비시간
    private float waitTime = 3f;
    private float curWaitTime = 0f;
    private bool isWait = false;

    private void ReadyNextStage()
    {
        //  Debug.Log("룸인포에서 준비 시작");
        curWaitTime = waitTime;
        isWait = true;
    }
    private void CheckStageReady()
    {
        //어떻게 할진 모르겠으나 
        //스테이지 종료후 뭐 콜백을 받던해서 준비가 다끝나면
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

    #region 타이머
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
        //시간초과
        //패스 누른 효과
        m_client.ExceedTimer();

    }
    #endregion
}
