
using System.Collections.Generic;

public class InGameData
{
    public int userCount;
    public string[] userIds = new string[4]; //최대 4명

    public string curId; //현재 유저
    public string preCard; //전에 낸 카드
    public int preCardCount;

    public string roomName; //방 이름
    public string myId;
    public int badPoint;
    public int myRank;

    public bool allPass = false;
    public bool isMyTurn = false;

    public int curTurn;

    public PlayerData m_myData;
    public List<PlayerData> m_partyList = new List<PlayerData>();

    public void SetRoomName(string _name)
    {
        //플클에서 ResRoonName에서 진행
        roomName = _name;
        Enqueue(ReqRoomType.RoomName);
    }

    public void SetCurTurnInfo(string _curId, int _curTurn, bool _isMyTurn)
    {
        curId = _curId;
        curTurn = _curTurn;
        isMyTurn = _isMyTurn;
        Enqueue(ReqRoomType.ArrangeTurn);
    }

    public void SetPutDownCardInfo(string _preCard, int _cardCount, string _id)
    {
        preCard = _preCard;
        preCardCount = _cardCount;
        PlayerData pData = GetPlayData(_id); //누가 냈는가
        if(pData != null)
        {
            pData.restCardCount -= _cardCount;
        }
        Enqueue(ReqRoomType.PutDownCard);
    }

    #region 플레이 데이터 생성 : 아이디 기록
    public void SetMyId(string _myId)
    {
        myId = _myId;
        PlayerData myData = new PlayerData();
        myData.ID = _myId;
        myData.isMe = true;
        RecordId(_myId);
        Enqueue(ReqRoomType.IDRegister);
    }

    private void RecordId(string _id)
    {
        PlayerData idData = GetPlayData(_id);
        if (idData == null)
        {
            PlayerData data = new PlayerData();
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

    public void RecordIdList(List<string> _idList)
    {
        //없으면 추가하고, 있던것 중에 없어진거있으면 제거 하고 
        //다중 쓰레드로 변화가 들어오는데 괜찮을까
        for (int i = 0; i < _idList.Count; i++)
        {
            //넘겨온 아이디대로 추가
            RecordId(_idList[i]);
        }

        for (int i = 0; i < m_partyList.Count; i++)
        {
            RemoveId(_idList, m_partyList[i]);
        }

        Enqueue(ReqRoomType.PartyData);
    }
    #endregion

 

    public void PlusBadPoints(string _id, int _point)
    {
        PlayerData idData = GetPlayData(_id);
        if(idData != null)
        {
            idData.badPoint += _point;
        }
    }

    public void FinalScore(int _point, int _rank)
    {
        badPoint = _point;
        myRank = _rank;
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
    public void ReSetRestCard()
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

    public void PutDown()
    {
        Enqueue(ReqRoomType.Draw);
    }

    private void Enqueue(ReqRoomType _code)
    {
        RoomInfoManager.instance.EnqueueCode(_code);
    }

    private PlayerData GetPlayData(string _id)
    {

        for (int i = 0; i < m_partyList.Count; i++)
        {
            if (m_partyList[i].ID == _id)
            {
                return m_partyList[i];
            }
        }
        return null;
    }

    public PlayerData GetMyData()
    {
        return GetPlayData(myId);
    }
}
