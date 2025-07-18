﻿
using System.Collections.Generic;

public class InGameData
{
    public int userCount;
    public string[] userIds = new string[4]; //최대 4명

    public int roomMasterPid;
    public int curTurnPid; //현재 유저 pid
    public string curTurnId; //현재 유저
    public string finalCard; //전에 낸 카드
    public int preCardCount;

    public string roomName; //방 이름
    public string myId;
    public int myPid;
    public int badPoint;
    public int myRank;

    public bool allPass = false;
    public bool isMyTurn = false;

    public int curTurn;
    public RoomInfoManager roomManager;

    public List<PlayerData> m_partyList = new List<PlayerData>();

    public InGameData(RoomInfoManager manager)
    {
        roomManager = manager;
    }

    public void SetRoomName(string _name)
    {
        //플클에서 ResRoonName에서 진행
        roomName = _name;
        Enqueue(ReqRoomType.RoomName);
    }

    public void SetCurTurnInfo(int _curPid, int _curTurn, bool _isMyTurn)
    {
        curTurnPid = _curPid;
        curTurnId = _curPid.ToString();
        curTurn = _curTurn;
        isMyTurn = _isMyTurn;
        Enqueue(ReqRoomType.ArrangeTurn);
    }

    public int preCardPid;//패쓰 포함 냈던 사람
    public EMixtureType preMixtureType; //패쓰 포함 가치

    public void SetPutDownCardInfo(TMixture _cardValue, int _cardCount, int _id)
    {
        //유요한 
        preMixtureType = _cardValue.mixture; //마지막과 이전 타입은 다를 수 있음. 
        preCardPid = _id;
        if (preMixtureType != EMixtureType.None && preMixtureType != EMixtureType.Pass)
        {
            finalCard = _cardValue.GetCardShowValue(); //유효한 카드인 경우에만 마지막 카드 값 기록.
        }
        preCardCount = _cardCount;
        PlayerData pData = GetPlayData(_id); //누가 냈는가
        if(pData != null)
        {
            pData.restCardCount -= _cardCount;
        }
        Enqueue(ReqRoomType.PutDownCard);
    }

    #region 플레이 데이터 생성 : 아이디 기록
    public void SetMyInfo(int _myPId, string _id)
    {
        myId = _id; //인게임에 별도로 내아이디 저장
        myPid = _myPId;
        RecordId(myPid, myId); //인게임 관리 플레이어 데이터에 아이디 등록
        GetMyData().isMe = true; //내아이디로 찾아온다음 그 데이터에 나란걸 표시.
        Enqueue(ReqRoomType.IDRegister);
    }

    private void RecordId(int _pid, string _id)
    { 
        //추가하려는 아이디가 플레이 데이터 리스트에 없으면 새롭게 데이터를 만들어서 넣는 부분. 
        PlayerData idData = GetPlayData(_pid);
        if (idData == null)
        {
            PlayerData data = new PlayerData();
            data.PID = _pid;
            data.ID = _id;
            m_partyList.Add(data);
        }
    }

    private void RemoveId(List<string> _idList, PlayerData _pData)
    {
        //참가중이라는 아이디 중에, 참가중이었던 아이디를 봐서 없으면 제거
        for (int i = 0; i < _idList.Count; i++)
        {
            if (_idList[i] == _pData.ID)
            {
                //명부에 있는 아이디면 냅두고
                return;
            }
        }
        //명부에 없으면 제거
        m_partyList.Remove(_pData);
    }

    public void RecordIdList(List<int> _pidList, List<string> _idList)
    {
        //없으면 추가하고, 있던것 중에 없어진거있으면 제거 하고 
        //다중 쓰레드로 변화가 들어오는데 괜찮을까
        for (int i = 0; i < _idList.Count; i++)
        {
            //넘겨온 아이디대로 추가
            RecordId(_pidList[i], _idList[i]);
        }

        for (int i = 0; i < m_partyList.Count; i++)
        {
            RemoveId(_idList, m_partyList[i]);
        }

        Enqueue(ReqRoomType.PartyData);
    }
    #endregion

    public void PlusBadPoints(int _pid, int _point)
    {
        PlayerData idData = GetPlayData(_pid);
        if(idData != null)
        {
            idData.badPoint += _point;
        }
        Enqueue(ReqRoomType.StageOver);
    }

    public void FinalScore(int _pid, int _point, int _rank)
    {

        PlayerData idData = GetPlayData(_pid);
        if (idData != null)
        {
            //최종 결산된 점수가 들어와서 = 로 대입만하면됨.
            idData.badPoint = _point;
            idData.rank = _rank;
        }

        Enqueue(ReqRoomType.GameOver);
    }

    public void ResetBadPoint()
    {
        for (int i = 0; i < m_partyList.Count; i++)
        {
            m_partyList[i].badPoint = 0;
        }
        //Enqueue(ReqRoomType.Start);//남은 카드 수갱신이 더 뒤에 일어나고 큐에 집어 넣으므로 거기서 같이 할꺼

    }

    public void ReStart()
    {
        //남은 카드 숫자 갱신 - 게임 시작할때 
        for (int i = 0; i < m_partyList.Count; i++)
        {
            m_partyList[i].restCardCount = 13;
        }
        Enqueue(ReqRoomType.Start);
    }

    public void SetAllPass(bool _isPass)
    {
        allPass = _isPass;
    }

    public void SetReadyState(int _pid, bool _isReady)
    {
        GetPlayData(_pid).isReady = _isReady;
    }

    public void UpdateReadyUI()
    {
        Enqueue(ReqRoomType.Ready);
    }

    public void SetRoomMaster(int _id)
    {
        roomMasterPid = _id;
        Enqueue(ReqRoomType.ArrangeRoomMaster);
    }

    public void PutDown()
    {
        Enqueue(ReqRoomType.Draw);
    }

    public void StageReady()
    {
        Enqueue(ReqRoomType.StageReady);
    }

    public void SetUserOrder(List<int> _orderList)
    {
        //정해진 순서대로 m_partyList의 순서를 바꾸면됨. 
        //1. 첫번째는 무조건 자신을 넣고 ,
        int myIndex = _orderList.IndexOf(myPid); //순서에서 내 인덱스 순서를 찾고 
        List<PlayerData> orderList = new();
        orderList.Add(m_partyList[0]); 
        for (int i = 1; i < m_partyList.Count; i++)
        {
            //나 다음의 아이디는 
            int orderIndex = (myIndex + i) % m_partyList.Count; //뒤로 순환하므로 +i를 하고 넘어가면 0으로 돌아가도록 설정
            PlayerData orderPlayer = GetPlayData(_orderList[orderIndex]); //아이디로 정보를 찾고
            orderList.Add(orderPlayer);
        }

        //순서 적용된 orderList대로 내 파티 리스트를 재설정
        for (int i = 0; i < m_partyList.Count; i++)
        {
            m_partyList[i] = orderList[i];
        }

    }

    public int GetUserCount()
    {
        return m_partyList.Count;
    }

    private void Enqueue(ReqRoomType _code)
    {
        roomManager.EnqueueCode(_code);
    }

    private PlayerData GetPlayData(int _pid)
    {

        for (int i = 0; i < m_partyList.Count; i++)
        {
            if (m_partyList[i].PID == _pid)
            {
                return m_partyList[i];
            }
        }
        return null;
    }

    public PlayerData GetMyData()
    {
        return GetPlayData(myPid);
    }
}
