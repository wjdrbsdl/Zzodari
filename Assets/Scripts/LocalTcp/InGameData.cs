
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
    public bool allPass = false;

    public int curTurn;

    public void SetRoomName(string _name)
    {
        //플클에서 ResRoonName에서 진행
        roomName = _name;
        Enqueue(ReqRoomType.RoomName);
    }

    public void SetUserCount(int _count)
    {
        //플클에서 ResRegisterClientIDToPartyID 로 응답 받아 아이디 다 기록후 마지막에 유저수 기입
        userCount = _count;
        Enqueue(ReqRoomType.PartyData);
    }

    public void SetCurTurnInfo(string _curId, int _curTurn)
    {
        curId = _curId;
        curTurn = _curTurn;
        Enqueue(ReqRoomType.ArrangeTurn);
    }

    public void SetPreCard(string _preCard, int _cardCount)
    {
        preCard = _preCard;
        preCardCount = _cardCount;
        Enqueue(ReqRoomType.PutDownCard);
    }

    public void SetMyId(string _myId)
    {
        myId = _myId;
        Enqueue(ReqRoomType.IDRegister);
    }

    public void PlusBadPoint(int _point)
    {
        badPoint += _point;
        Enqueue(ReqRoomType.StageOver);
    }

    public void ResetBadPoint()
    {
        badPoint = 0;
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

}
