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
    public TurnTimer m_turnTimer;
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
        if(reqTypeQueue.TryDequeue(out ReqRoomType _result))
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
                    //m_badPoint.text += InputColor(" 순위 : ") + inGameData.myRank.ToString();
                    break;
                case ReqRoomType.Draw:
                    OffTimer();
                    isShow = true;
                    helpTextRestTime = helpTextTime;
                    m_helpText.gameObject.SetActive(true);
                    m_helpText.text = "턴 넘김";
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
        m_preCard.text = InputColor("전 카드 :") + _gameData.preCard; //어떤 카드 냈는지
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
        //시간초과
        //패스 누른 효과
        if(m_client.PutDownPass() == false)
        {
            //만약 패스 불가라면 -> 올패스에서 자기차례
            //제일 작은 카드 1장 내기
            List<CardData> putList = new();
            m_client.SortCardList(); //정렬 -> 최초 시작시에 자동으로 클로버 3내도록
            putList.Add(m_client.GetHaveCardList()[0]);
            m_client.PutDownCards(putList);
        }
    }
}
