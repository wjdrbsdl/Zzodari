using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerData
{
    public string ID;
    public int number; //식별번호
    public int restCardCount;
    public int badPoint;
    public int rank;
    public bool isMe = false;
}


public enum ReqRoomType
{
    Ready, Start, RoomOut, Chat,
    IDRegister, PartyData, RoomName,
    ShuffleCard, PutDownCard, SelectCard,
    ArrangeTurn,
    StageReady, StageOver, GameOver,
    ReqGameOver, ResRoomJoinFail,
    Draw, UserOrder,
    InValidCard, ArrangeRoomMaster

}

public class PlayClient : MonoBehaviour
{
    #region 변수
    public static int pid;
    public static string id;
    public List<CardData> haveCardList; //내가 들고 있는 카드
    public List<CardData> giveCardList; //전에 내가 냈던 카드
    public List<CardData> putDownList; //바닥에 깔린 카드
    public bool isMyTurn = false;
    public bool isGameStart = false;
    public bool isReady = false; //손님인경우 게임 레뒤했는가 
    public int gameTurn = 0; //카드 제출이 진행된 턴 1번부터
    public InGameData inGameData;
    public NetworkManager networkManager;
    #endregion

    private void Start()
    {
        putDownList = new();
        haveCardList = new();
        giveCardList = new();
        if (inGameData == null)
            inGameData = new InGameData();

        networkManager = new NetworkManager();
        networkManager.Connect();
        networkManager.OnConnected += ReqRegisterClientID;
        networkManager.OnDataReceived += OnCallBackRecieve;
    }

    public InGameData GetInGameData()
    {
        if (inGameData == null)
        {
            inGameData = new InGameData();
        }
        return inGameData;
    }

    #region 데이터 수신
    private void OnCallBackRecieve(byte[] recvData)
    {
        ReqRoomType reqType = (ReqRoomType)recvData[0];
        ColorConsole.SystemDebug(reqType + "받은 길이 " + recvData.Length.ToString());
        HandleReceiveData(reqType, recvData);
    }

    private void HandleReceiveData(ReqRoomType _reqType, byte[] _validData)
    {
        if (_reqType == ReqRoomType.Chat)
        {
            ResChat(_validData);
        }
        else if (_reqType == ReqRoomType.ResRoomJoinFail)
        {
            ResRoomJoinFail();
        }
        else if (_reqType == ReqRoomType.ArrangeRoomMaster)
        {
            ResArrangeRoomMaster(_validData);
        }
        else if(_reqType == ReqRoomType.Ready)
        {
            ResGameReady(_validData);
        }
        else if (_reqType == ReqRoomType.Start)
        {
            SetNewGame();
            ResGameStart(_validData);

        }
        else if (_reqType == ReqRoomType.SelectCard)
        {
            ResSelectCard(_validData);
        }
        else if (_reqType == ReqRoomType.InValidCard)
        {
            //  Debug.Log("부정 발견");
            ResInvalidCard(_validData);
        }
        else if (_reqType == ReqRoomType.UserOrder)
        {
            ResUserOrder(_validData);
        }

        else if (_reqType == ReqRoomType.PutDownCard)
        {
            ResPutDownCard(_validData);
        }
        else if (_reqType == ReqRoomType.ArrangeTurn)
        {
            ResTurnPlayer(_validData); //인게임데이터로 내턴인지, 누구턴인지 기록
        }
        else if (_reqType == ReqRoomType.PartyData)
        {
            //idRegister에 반환되는 타입.
            ResRegisterClientIDToPartyID(_validData);
        }
        else if (_reqType == ReqRoomType.StageOver)
        {
            ResStageOver(_validData);
            ResetStage();
            WaitStageResult();

        }
        else if (_reqType == ReqRoomType.RoomName)
        {
            ResRoomName(_validData);
        }
        else if (_reqType == ReqRoomType.GameOver)
        {
            ResGameOver(_validData);
            ResetStage();
            SetGameOver();
        }
    }
    #endregion

    #region 로직 파트

    private void SetNewGame()
    {
        //보유 카드는 통신응답에서 진행
        giveCardList.Clear();
        putDownList.Clear();
        gameTurn = 0;
        SetMyTurn(false);

        //스테이지 종료후 다시 시작하는 경우일땐 게임은 진행중임. 
        if (isGameStart == false)
        {
            isGameStart = true;
            inGameData.ResetBadPoint();
        }

    }

    private void ResetStage()
    {
        ColorConsole.Default("스테이지 리셋");
        giveCardList.Clear();
        putDownList.Clear();
        haveCardList.Clear();
        gameTurn = 0;
        SetMyTurn(false);
        SetMyCardList(); //유니티 -ResetStage에서 보유카드 클리어하고, 그상태로 UI 갱신
    }

    private void SetGameOver()
    {
        isGameStart = false;
    }

    #region 카드 내기

    public bool PutDownCards(List<CardData> _selectCards)
    {
        if (isGameStart == false)
        {
            return false;
        }

        if (isMyTurn == false)
        {
            ColorConsole.RuleWarning("자기 차례가 아닙니다.");
            return false;
        }
        //낼 수 있는 카드 인지 체크
        if (CheckSelectCard(_selectCards))
        {
            //낼 수 있으면 제출
            SetMyTurn(false); //내턴 넘김으로 수정
            ReqSelectCard(new List<CardData>()); //최종 제출시엔 선택칸은 비어있음.
            ReqPutDownCard(_selectCards);
            return true;
        }
        return false;

    }

    public bool PutDownPass()
    {
        //빈거 넘김

        return PutDownCards(new List<CardData>()); //패스버튼
    }

    private bool CheckSelectCard(List<CardData> _selectCards)
    {
        CardRule cardRule = new CardRule();
        TMixture selectCardValue = new TMixture();
        if (cardRule.IsVarid(_selectCards, out selectCardValue) == false)
        {
            ColorConsole.RuleWarning("유효한 조합이 아닙니다.");
            return false;
        }

        //혼자 크기 비교 위해서 아래비교, 내가 제출한건 무조건 전걸로 진행 

        //선택된 카드를 현재 낼 수 있는지 판단해서 bool 반환
        if (gameTurn == 1)
        {
            //첫번째 턴이면 보유한 카드에 스페이드 3 있어야 가능 한걸로 
            foreach (CardData card in _selectCards)
            {
                if (card.Compare(CardData.minClass, CardData.minRealValue) == 0)
                {
                    return true;
                }
            }
            ColorConsole.RuleWarning($"첫 시작은 {CardData.minClass}{CardData.minRealValue}을 포함해야 합니다.");
            return false;
        }

        //처음이 아니면 내가 낸건지 체크 - 내가 낸거면 자유롭게 내기 가능
        if (CheckAllPass())
        {
            if (_selectCards.Count == 0)
            {
                ColorConsole.RuleWarning("올 패스 받았습니다. 자유롭게 낼 수 있되 패스는 불가 합니다.");
                return false;
            }
            return true;
        }


        if (selectCardValue.mixture == EMixtureType.Pass)
        {
            //패스한거면 그냥 통과
            return true;
        }

        //이전것과 비교
        TMixture putDownValue = new TMixture();
        cardRule.CheckValidRule(putDownList, out putDownValue);

        ColorConsole.Default($"이전꺼 {putDownValue.mixture}:{putDownValue.mainCardClass}:{putDownValue.mainRealValue}" +
            $"\n제출용 {selectCardValue.mixture}:{selectCardValue.mainCardClass}:{selectCardValue.mainRealValue}:");
        //비교 안되는 타입이면 (앞에 낸것과 다른 유형이면) 실패
        if (cardRule.TryCompare(putDownValue, selectCardValue, out int compareValue) == false)
        {
            ColorConsole.RuleWarning("다른 유형의 조합입니다.");
            return false;
        }
        //compareValue는 이전꺼에서 현재껄 뺀거 - 즉 양수면 전께 큰거 
        if (compareValue > 0)
        {
            //이전것보다 작아도 실패
            ColorConsole.RuleWarning("전 보다 작습니다.");
            return false;
        }

        ColorConsole.Default("전 보다 크다");
        return true;
    }

    private bool CheckAllPass()
    {
        inGameData.SetAllPass(false);
        if (putDownList.Count == 0)
        {
            return false;
        }

        ColorConsole.Default("올 패스인지 체크");
        giveCardList.Sort();
        putDownList.Sort();

        //정렬해서 냈던 카드가 있으면 올 패스 된거.
        if (giveCardList.Count >= 1)
        {
            if (giveCardList[0].CompareTo(putDownList[0]) == 0)
            {
                //하나라도 내가 냈던거랑 같으면 내가 냈던거
                ColorConsole.Default("올 패스 받았음");
                inGameData.SetAllPass(true);
                return true;
            }
            return false;
        }

        return false;

    }

    private void SetMyTurn(bool _turn)
    {
        isMyTurn = _turn;
    }
    #endregion 

    #region 카드 리스트 관리
    private void ResetGiveCard()
    {
        //내가 냈던 카드 초기화
        giveCardList.Clear();
    }

    private void RecordGiveCard(List<CardData> _cardList)
    {
        for (int i = 0; i < _cardList.Count; i++)
        {
            giveCardList.Add(_cardList[i]);
        }
    }

    private void ResetPutDownCard()
    {
        putDownList.Clear();
    }

    private void AddPutDownCard(CardData _card)
    {
        putDownList.Add(_card);
    }

    private void RemoveHaveCard(List<CardData> _removeList)
    {
        //보유 카드에서 내려놓은 카드 제거
        for (int i = 0; i < _removeList.Count; i++)
        {
            CardData target = _removeList[i];
            for (int j = 0; j < haveCardList.Count; j++)
            {
                if (haveCardList[j].CompareTo(target) == 0)
                {
                    haveCardList.RemoveAt(j);
                    break;
                }
            }
        }
    }

    public void SortCardList()
    {
        haveCardList.Sort();

        Action action = () =>
        {
            if (isMyTurn)
            {
                CardManager.instance.ResetSelectCards(); // 내 차례라면 냈던 카드도 회수
            }
            CardManager.instance.ResetHandCards(); //내 카드 정렬할때 
        };
        CardManager.instance.callBack.Enqueue(action); //인큐

        if (isMyTurn)
        {
            ReqSelectCard(new List<CardData>()); //정렬로 냈던 카드 다 회수한걸로 전달
        }

    }

    public List<CardData> GetHaveCardList()
    {
        return haveCardList;
    }
    #endregion

    public void ExceedTimer()
    {
        //시간 초과 
        if (PutDownPass() == false)
        {
            //만약 패스 불가라면 -> 올패스에서 자기차례
            //제일 작은 카드 1장 내기
            List<CardData> putList = new();
            SortCardList(); //첫턴에 시간초과시 가장 낮은 카드 제출 위해서
            putList.Add(GetHaveCardList()[0]);
            PutDownCards(putList);
        }
    }

    private void CountTurn()
    {
        gameTurn++;
    }
    #endregion


    #region 통신 파트
    #region 게임 시작

    private void SendMessege(byte[] _sendData)
    {
        networkManager.Send(_sendData);
        return;
    }

    public void ReqGameStart()
    {
        //시작한 상태면 못하게
        if (isGameStart)
            return;

        byte[] reqStart = { (byte)ReqRoomType.Start };
        SendMessege(reqStart);
    }

    public void ReqGameReady()
    {

        /*레디 데이터
              * [0] 요구코드 ready
              * [1] 요구한 pid
              */

        //시작한 상태면 못하게
        if (isGameStart)
            return;

        byte[] reqReady = { (byte)ReqRoomType.Ready, (byte)pid };
        SendMessege(reqReady);
    }

    public void ResGameReady(byte[] _resData)
    {
        //플레이어 준비 상태 전달하기
        /*
         * [0] 코드
         * [pid]
         * [state = 0이면 false]
         */
        for (int i = 1; i < _resData.Length; i+=2 )
        {
            Debug.Log($"{_resData[i]}번 유저 레뒤 상태 {_resData[i+1]}"); 
        }
    }

    private void ResGameStart(byte[] _resDate)
    {
        /*
         * 셔플된 카드데이터
         * [0] 응답코드
         * [1] 카드 장수
         * [2] 번부터 2개씩 카드가 생성
         */
        MakeMyCard(_resDate, 2);
        inGameData.ReStart();
    }

    private void SetMyCardList()
    {
        Action action = () =>
        {
            CardManager.instance.SetHaveCard(haveCardList); //처음 내카드를 세팅하는곳
        };
        CardManager.instance.callBack.Enqueue(action); // 내 카드리스트 세팅
    }

    private void ResetTurnCard()
    {
        Action action = () =>
        {
            //순서를 맞춰야함 - 리셋하면서 obj 비활성화를 시키기 때문에 손먼저하고 
            //카드를 해버리면 손에있던 obj가 비활성화됨
            CardManager.instance.ResetSelectCards();//턴이 갱신되었을때
            CardManager.instance.ResetHandCards(); //턴이 갱신되었을때
        };
        CardManager.instance.callBack.Enqueue(action); // 내카드리스트 리셋
    }

    #endregion

    #region 플레이 서버에 내 아이디 등록
    public void ReqRegisterClientID()
    {
        //Debug.Log("아이디 기록 클라이언트 이벤트로 호출");
        byte[] reqID = new byte[] { (byte)ReqRoomType.IDRegister, (byte)pid };
        SendMessege(reqID);
        inGameData.myNumber = pid;
        inGameData.SetMyInfo(pid, id);
    }

    public void ResRoomName(byte[] _data)
    {
        byte[] nameByte = new byte[_data.Length - 1];
        Buffer.BlockCopy(_data, 1, nameByte, 0, nameByte.Length);
        string roomName = Encoding.Unicode.GetString(nameByte);
        inGameData.SetRoomName(roomName);

    }

    public void ResRegisterClientIDToPartyID(byte[] _data)
    {
        //자기 id를 알려주면 다른 모든 참가 아이디를 반환받음
        /*
          * [0] 응답코드 PartyIDes,
          * [1] ID를 받은 유효한 파티원 수
          * [2] 각 파티원 정보 길이 -- pid, id (둘다 byte긴 함)
          * [3] 0번 파티원부터 정보 입력
          */
        int userIdx = 0;
        List<string> idList = new List<string>();
        List<int> pidList = new();
        for (int i = 3; i < _data.Length; i += _data[2])
        {

            int pid = _data[i];
            pidList.Add(pid);
            string id = _data[i + 1].ToString();
            ColorConsole.Default(id + "번 참가");
            idList.Add(id);
            userIdx++;
        }
        inGameData.RecordIdList(pidList, idList);
    }
    #endregion

    #region 방나가기
    public void ReqRoomOut()
    {
        ColorConsole.Default("클라가 나가기 요청");
        byte[] reqRoomOut = new byte[] { (byte)ReqRoomType.RoomOut, (byte)pid };
        SendMessege(reqRoomOut);

        DisConnet();

        SendOutCallBack();
    }

    public void ResRoomJoinFail()
    {
        ColorConsole.Default("플레이 클라가 방 입장 못했음");

        DisConnet();

        SendOutCallBack();
    }

    public void ResArrangeRoomMaster(byte[] _receiveData)
    {
        /*
         * [0] 응답 코드 룸마스터
         * [1] 마스터 아이디 
         */

        inGameData.SetRoomMaster(_receiveData[1]);
    }

    private void SendOutCallBack()
    {
        Action outCallBack = () =>
        {
            SceneManager.LoadScene("LobbyScene");
        };
        ClientManager.IsRoomOut = true;
        CardManager.instance.callBack.Enqueue(outCallBack); //나가기
    }

    #endregion

    #region 카드 제출
    public void ReqSelectCard(List<CardData> _cardDataList)
    {
        /*
         * [0] 요청 코드 putdownCard
         * [1] 플레이어 id
         * [2] 낸 카드 숫자
         * [3] 카드 구성
         */
        ColorConsole.Default("카드 제출 요청");
        List<byte> reqCardList = new();
        reqCardList.Add((byte)ReqRoomType.SelectCard);
        reqCardList.Add((byte)pid);
        reqCardList.Add((byte)_cardDataList.Count);
        for (int i = 0; i < _cardDataList.Count; i++)
        {
            reqCardList.Add((byte)_cardDataList[i].cardClass);
            reqCardList.Add((byte)_cardDataList[i].num);
        }
        byte[] reqData = reqCardList.ToArray();
        SendMessege(reqData);
    }

    private void ResSelectCard(byte[] _data)
    {
        //유저가 어떤 카드를 냈는지 전달
        /*
        * [0] 요청 코드 putdownCard
        * [1] 플레이어 id
        * [2] 낸 카드 숫자
        * [3] 카드 구성
        */
        //자기가 낸 경우엔 응답 없음. 

        List<CardData> selectCardList = new(); //제출된 카드리스트 정리
        for (int i = 3; i < _data.Length; i += 2)
        {
            CardClass cardClass = (CardClass)_data[i];
            int num = _data[i + 1];
            CardData card = new CardData(cardClass, num); //카드 생성
            selectCardList.Add(card);
        }


        //본인이 낸거라면 본인 카드에서 제외
        ResetSelectZone(_data[1].ToString(), selectCardList);
    }

    private void ResInvalidCard(byte[] _data)
    {
        //유저의 카드 정보가 잘못된 카드 정보인경우, 
        //서버로부터 다시 유저의 카드데이터를 받아서 카드값을 갱신한다. 
        //유저가 어떤 카드를 냈는지 전달
        /*
        * [0] 요청 코드 inValidCard
        * [1] 플레이어 id
        * [2] 현재턴 Id
        * [3] 카드 구성 시작
         */
        //자기가 낸 경우엔 응답 없음. 
        MakeMyCard(_data, 3); //부정사용자의 카드 리셋
        int curId = _data[2];
        if (inGameData.myNumber == curId)
        {
            Debug.Log("아직 내 차례라서 내차례로 전환");
            SetMyTurn(true);
        }
    }

    private void MakeMyCard(byte[] _cardData, int _startIdx)
    {
        haveCardList.Clear();
        for (int i = _startIdx; i < _cardData.Length; i += 2)
        {
            //i번째는 카드 무늬, i+1에는 카드 넘버가 있음
            CardData card = new CardData((CardClass)_cardData[i], _cardData[i + 1]);
            haveCardList.Add(card);
        }

        SetMyCardList();
    }

    private void ResUserOrder(byte[] _data)
    {
        /*
          * [0] 코드 - ReqRoomType.UserOrder
          * [1] 참가 인원수
          * [2]부터 해당아이디 Length, 아이디값 반복.
          * [2+ Length+1] ~ 반복
          */
        int userCount = _data[1];
        int idLengthIndex = 2;
        List<string> idOrderList = new();
        for (int i = 0; i < userCount; i++)
        {
            int idLength = _data[idLengthIndex];//아이디 길이 
            byte[] idByte = new byte[idLength];
            Buffer.BlockCopy(_data, idLengthIndex + 1, idByte, 0, idLength);
            string id = Encoding.Unicode.GetString(idByte);
            idOrderList.Add(id);
            idLengthIndex = idLengthIndex + idLength + 1;//다음 아이디 길이 인덱스를 가리키고
        }
        inGameData.SetUserOrder(idOrderList);
    }

    private void ResetSelectZone(string _id, List<CardData> _cardList)
    {
        string putPlayerID = _id; //카드 낸사람
        Action action = () => CardManager.instance.ShowOtherCard(putPlayerID, _cardList);
        CardManager.instance.callBack.Enqueue(action);
    }

    private void ReqPutDownCard(List<CardData> _cardDataList)
    {
        /*
         * [0] 요청 코드 putdownCard
         * [1] 플레이어 id
         * [2] 낸 카드 숫자
         * [3] 카드 구성
         */
        ColorConsole.Default("카드 제출 요청");
        List<byte> reqCardList = new();
        reqCardList.Add((byte)ReqRoomType.PutDownCard);
        reqCardList.Add((byte)pid);
        reqCardList.Add((byte)_cardDataList.Count);
        for (int i = 0; i < _cardDataList.Count; i++)
        {
            reqCardList.Add((byte)_cardDataList[i].cardClass);
            reqCardList.Add((byte)_cardDataList[i].num);
        }
        byte[] reqData = reqCardList.ToArray();
        SendMessege(reqData);
        inGameData.PutDown();
    }

    private void ResPutDownCard(byte[] _data)
    {
        //유저가 어떤 카드를 냈는지 전달
        /*
        * [0] 요청 코드 putdownCard
        * [1] 플레이어 id
        * [2] 낸 카드 숫자
        * [3] 카드 구성
        */
        CardRule rule = new CardRule();
        string putPlayerID = _data[1].ToString(); //카드 낸사람
        //본인의 행위였다면
        if (_data[1] == pid)
        {
            //이전에 냈던건 초기화
            ResetGiveCard();
            // SortCardList(); // 손패 정렬
        }
        if (_data[2] == 0)
        {
            //방금 낸 카드가 없으면
            //이전 카드 덮어 쓰지 않고
            //본인의 카드 제거나 이전 카드 기록 안함
            ColorConsole.Default("전 사람 패쓰했음");
            ResetTurnCard();//패쓰 했을때 셀렉존 카드 갱신 위해서 
            rule.CheckValidRule(new List<CardData>(), out TMixture _passMixture);
            inGameData.SetPutDownCardInfo(_passMixture, 0, putPlayerID);
            return;
        }
        //바닥에 깔린 카드 갱신
        ResetPutDownCard();
        for (int i = 3; i < _data.Length; i += 2)
        {
            CardClass cardClass = (CardClass)_data[i];
            int num = _data[i + 1];
            CardData card = new CardData(cardClass, num); //카드 생성
            AddPutDownCard(card);
        }

        rule.CheckValidRule(putDownList, out TMixture _mixture);
        ColorConsole.Default($"{_data[1]}유저가 제출한 카드 {_mixture.mixture}:{_mixture.mainCardClass}:{_mixture.mainRealValue}");
        inGameData.SetPutDownCardInfo(_mixture, _mixture.cardCount, putPlayerID);
        //본인이 낸거라면 본인 카드에서 제외
        if (_data[1] == pid)
        {
            //내가 카드를 낸경우
            RemoveHaveCard(putDownList);
            RecordGiveCard(putDownList);
        }
        ResetTurnCard();
    }
    #endregion

    #region 턴 정하기
    private void ResTurnPlayer(byte[] _data)
    {
        /*
         * [0] 응답코드 ArrangeTurn
         * [1] 차례 ID
         */
        ColorConsole.Default("턴 지정 들어옴 " + _data[1] + " 내 아이디 " + id);
        isMyTurn = pid == _data[1];
        if (isMyTurn)
        {
            ColorConsole.Default("내 차례");
            CheckAllPass();
        }

        CountTurn(); //턴을 지정하는건 새로운 턴이 된거
        inGameData.SetCurTurnInfo(_data[1].ToString(), gameTurn, isMyTurn);
    }
    #endregion

    #region 판, 게임 종료
    private void ResStageOver(byte[] _data)
    {
        /*
            * [0] 응답코드 스테이지오버
            * [1] 사람 수 - 4
            * [2] 아이디
            * [3] 보유 카드 수 반복
            */

        for (int i = 2; i < _data.Length; i += 2)
        {
            string playerId = _data[i].ToString();
            inGameData.PlusBadPoints(playerId, _data[i + 1]);
        }

    }

    private void WaitStageResult()
    {
        //스테이지 종료후 뭔가 기다리기 
        inGameData.StageReady();
    }

    public void WaitStageResultCallBack()
    {
        // Debug.Log("스테이지 준비 콜백");
        ReqStageReady();
    }

    private void ReqStageReady()
    {
        //유저가 다음판 할 준비 되었다고 알리기 
        /*
         * [0] 요구코드 stageReady
         * [1] 내 아이디
         */
        byte[] stageReadyDate = new byte[] { (byte)ReqRoomType.StageReady, (byte)pid };
        SendMessege(stageReadyDate);
    }

    private void ResGameOver(byte[] _data)
    {
        /*
        * [0] 종료코드 GameOver
        * [1] 유저수 - 순위대로 정렬
        * [2] 보낼 정보 데이터 길이 일단 2
        * [3] 유저 ID
        * [4] 유저 벌점
        */

        List<(int, int)> scoreList = new();
        int rank = 1;

        for (int i = 3; i < _data.Length; i += _data[2])
        {
            ColorConsole.Default($"{_data[i]}의 벌점 :{_data[i + 1]}");
            (int, int) idWithScore = (_data[i], _data[i + 1]);
            scoreList.Add(idWithScore);
            inGameData.FinalScore(_data[i].ToString(), _data[i + 1], rank);
            rank++;
        }

    }
    #endregion

    #region 채팅 

    private void ResChat(byte[] _receiveData)
    {
        string convertStr = Encoding.Unicode.GetString(_receiveData, 1, _receiveData.Length - 1);
        char first = convertStr[0];

        int max = Math.Min(convertStr.Length - 1, convertStr.Length);
        string split = convertStr.Substring(1, max);
        if (first == ' ')
        {
            split = "[당신]:" + split;
        }
        else
        {
            int number = _receiveData[0];
            split = "[" + number.ToString() + "]:" + split;
        }
        ColorConsole.Default(split);

    }
    #endregion
    #endregion

    private void OnDisable()
    {
        //  Debug.Log("유니티에서 끌때 소켓 종료");
        DisConnet();
    }

    private void DisConnet()
    {
        networkManager.Disconnect();
    }
}

