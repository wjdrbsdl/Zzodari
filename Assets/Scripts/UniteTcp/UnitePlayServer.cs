﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

public class UnitePlayServer
{
    public List<ClaInfo> roomUser = new();
    public Socket linkSocket; //룸유저 받아들이는 소켓
    public Socket linkStateLobby; //서버로비와 소통하는 소켓 -> 방 상태 바뀔때 신호 
    public Socket linkRoomCount; //서버로비와 소통하는 소켓 -> 방 인원 바뀔때 신호 
    public string roomName;
    public int turnId;
    public RoomState roomState = RoomState.Ready;
    public int port = 5002;
    public UniteServer uniteServ;

    public UnitePlayServer(int portNum, UniteServer _uniteServer, string _roomName)
    {
        uniteServ = _uniteServer;
        port = portNum;
        roomName = _roomName;
    }

    //방장이 호스트가 되어 해당 방에있던 유저들의 입력을 받고 처리 
    #region 연결 및 소켓 세팅
    public void Start()
    {
        ColorConsole.ConsoleColor("룸 서버 시작");
        linkSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, port);
        linkSocket.Bind(endPoint);
        linkSocket.Listen(4);
        MakeCards(); //
        UpdateRemoveSockect();
        linkSocket.BeginAccept(AcceptCallBack, null);
    }

    public class ClaInfo
    {
        public byte[] buffer;
        public Socket workingSocket;
        public int ID = 0; //0이면 아이디를 전달받지 못한 상태
        public int HaveCard = 0;
        public int BadPoint = 0;

        public ClaInfo(int _bufferSize, Socket _claSocket)
        {
            buffer = new byte[_bufferSize];
            workingSocket = _claSocket;

        }

        public void ResetScore()
        {
            HaveCard = 0;
            BadPoint = 0;
        }

    }

    private void AcceptCallBack(IAsyncResult _result)
    {
        try
        {
            ColorConsole.ConsoleColor("룸 서버 수락");
            Socket client = linkSocket.EndAccept(_result);
            ClaInfo newCla = new ClaInfo(2, client);
            roomUser.Add(newCla);
            client.BeginReceive(newCla.buffer, 0, newCla.buffer.Length, 0, DataReceived, newCla);
            SendRoomCount(); //참가에 따른 변경 인원 전달
            linkSocket.BeginAccept(AcceptCallBack, null); //다른거 받을 준비 
        }
        catch
        {
            Console.WriteLine("");
        }

    }

    void DataReceived(IAsyncResult ar)
    {
        try
        {
            // ColorConsole.ConsoleColor("룸     서버로서 리시브");
            ClaInfo cla = (ClaInfo)ar.AsyncState;
            byte[] msgLengthBuff = cla.buffer;

            ushort msgLength = EndianChanger.NetToHost(msgLengthBuff);

            byte[] recvBuffer = new byte[msgLength];
            byte[] recvData = new byte[msgLength];
            int recv = 0;
            int recvIdx = 0;
            int rest = msgLength;
            do
            {
                recv = cla.workingSocket.Receive(recvBuffer);
                Buffer.BlockCopy(recvBuffer, 0, recvData, recvIdx, recv);
                recvIdx += recv;
                rest -= recv;
                recvBuffer = new byte[rest];//퍼올 버퍼 크기 수정
                if (recv == 0)
                {
                    ClaInfo obj = (ClaInfo)ar.AsyncState;
                    ExitClient((byte)obj.ID);
                    return;
                }
            } while (rest >= 1);

            HandleReq(cla, recvData);


            //obj.ClearBuffer();
            if (cla.workingSocket.Connected)
                cla.workingSocket.BeginReceive(cla.buffer, 0, cla.buffer.Length, 0, DataReceived, cla);

        }
        catch
        {

            ClaInfo obj = (ClaInfo)ar.AsyncState;
            ExitClient((byte)obj.ID);
        }

    }
    #endregion

    private void HandleReq(ClaInfo _claInfo, byte[] _reqData)
    {
        ReqRoomType reqType = (ReqRoomType)_reqData[0];

        if (reqType == ReqRoomType.IDRegister)
        {
            _claInfo.ID = _reqData[1];
            AnnouceParty();
        }
        else if (reqType == ReqRoomType.Chat)
        {
            SendChat(_reqData, _claInfo.ID);
        }
        else if (reqType == ReqRoomType.RoomOut)
        {
            /*roomOut 데이터
             * [0] 요구코드 RoomOut
             * [1] 나가려는 아이디
             */
            ExitClient(_reqData[1]);

        }
        else if (reqType == ReqRoomType.Start)
        {
            //게임 시작 가능한 상태인지 체크 
            //방장의 시작인가, 인원이 다 찼는가
            //게임 시작 가능하면 아래 진행

            RoomState = RoomState.Play;
            SendRoomStateToLobbyServer();
            UserScoreReset(); //유저 점수 리셋
                              //섞고
            ShuffleCard();
            ShuffleTurn();
            AnnouceCardArrange(); //카드 나눠주고
            AnnounceTurnPlayer(); //누가시작인지 알려줌
        }
        else if (reqType == ReqRoomType.PutDownCard)
        {
            //1. 다른유저들에게도 제출된 카드 공지
            AnnoucePutDownCard(_reqData);
            //2. 해당 유저가 모든 패를 털었는지 체크
            if (CheckStageOver(_reqData) == false)
            {
                //해당 판이 안끝났으면 계속 진행
                AnnounceTurnPlayer(); //다음 차례 지정해줌 - 위의 풋다운에서 다음 차례 찾아놓음
                return;
            }
            //3. 판이 끝났다면 벌점 계산
            CalBadPoint();
            bool gameOver = CheckGameOver(); //벌점에 따라 게임이 끝났는지 체크
                                             //4. 게임오버인지 체크
            if (gameOver == false)
            {
                //아직 게임 오버가 아니라면
                AnnouceStageOver(); //스테이지 끝났다고 알리고
                StartNexStage(); //다음 스테이지 준비
                return;
            }
            //5. 게임오버 된거면 게임오버 호출
            GameOver();

        }
        else if (reqType == ReqRoomType.StageReady)
        {
            //유저들로 부터 다음 스테이지 진행할 준비가 되었음을 확인
            ResStageReadyPlayer(_reqData);
        }
        else if (reqType == ReqRoomType.ReqGameOver)
        {
            GameOver();
        }
    }


    #region 벌점, 스테이지, 게임 종료 체크
    private void CalBadPoint()
    {
        for (int i = 0; i < roomUser.Count; i++)
        {
            int restCard = roomUser[i].HaveCard;
            if (restCard >= 1)
            {
                roomUser[i].BadPoint += roomUser[i].HaveCard; //남은 장수 만큼 벌점 진행
            }

        }
    }

    int overBadPoint = 13; //
    private bool CheckGameOver()
    {
        //게임 오버 체크 한유저라도 벌점 
        for (int i = 0; i < roomUser.Count; i++)
        {
            if (roomUser[i].BadPoint >= overBadPoint)
            {
                return true;
            }
        }
        return false;
    }

    private bool CheckStageOver(byte[] _putDownCardData)
    {
        /*
         * [0] 요청 코드 putdownCard
         * [1] 플레이어 id
         * [2] 낸 카드 숫자
         * [3] 카드 구성
       */

        //제출했던 유저 아이디
        for (int i = 0; i < roomUser.Count; i++)
        {
            if (roomUser[i].ID == _putDownCardData[1])
            {
                roomUser[i].HaveCard -= _putDownCardData[2];

                if (roomUser[i].HaveCard == 0)
                {
                    return true;
                }
                return false;
            }
        }
        return false;
    }
    #endregion

    #region 게임 진행
    Queue<int> reqStagePlayer = new();
    List<int> confirmStagePlayer = new();
    int curUserCount;
    CardData[] cards;
    private void MakeCards()
    {
        ColorConsole.ConsoleColor("게임 생성");
        cards = new CardData[52];
        int index = 0;
        for (int cardClass = 0; cardClass < 4; cardClass++)
        {
            for (int cardNum = 1; cardNum <= 13; cardNum++)
            {
                CardData card = new CardData((CardClass)cardClass, cardNum);
                cards[index] = card;
                index++;
            }
        }
    }

    private void ShuffleCard()
    {
        ShuffleCard shuffle = new ShuffleCard();
        shuffle.Shuffle(cards);
    }

    private void ShuffleTurn()
    {
        Random ran = new Random();
        for (int i = 0; i < roomUser.Count; i++)
        {
            int ranNum = ran.Next() % roomUser.Count;
            ClaInfo ori = roomUser[i];
            roomUser[i] = roomUser[ranNum];
            roomUser[ranNum] = ori;
        }

        string turn = "";
        for (int i = 0; i < roomUser.Count; i++)
        {
            turn += roomUser[i].ID + " ";
        }
        ColorConsole.ConsoleColor("순서 " + turn);
    }

    private void UserScoreReset()
    {
        for (int i = 0; i < roomUser.Count; i++)
        {
            roomUser[i].ResetScore();
        }
    }

    private void StartNexStage()
    {
        //스테이지가 끝나면 큐를 청소하고,
        //매 프레임마다 큐를 확인해서 준비가 된 id를 확인 
        //중복 id 없이 정원 4명이 모이면 다음 스테이지 시작
        reqStagePlayer.Clear();
        confirmStagePlayer.Clear();
        curUserCount = roomUser.Count; //룸유저 카운트로 ? 
        Task.Run(() =>
        {
            while (confirmStagePlayer.Count < curUserCount)
            {
                if (reqStagePlayer.TryDequeue(out int id))
                {
                    if (confirmStagePlayer.IndexOf(id) == -1)
                    {
                        ColorConsole.ConsoleColor(id + "준비 확인");
                        confirmStagePlayer.Add(id);
                    }
                }
            }
            ColorConsole.ConsoleColor(curUserCount + "명의 준비 확인 다음 스테이지 시작");
                //섞고
            ShuffleCard();
            ShuffleTurn();
            AnnouceCardArrange();
            AnnounceTurnPlayer();
        });


    }

    private void ResStageReadyPlayer(byte[] _reqStageReady)
    {
        //유저가 다음판 할 준비 되었다고 알리기 
        /*
         * [0] 요구코드 stageReady
         * [1] 내 아이디
         */
        //해당 유저가 준비되었다고 요청이 오면, 모두 준비가 끝났는지 확인할 수 있게 큐에 집어넣음.
        //ReadyNextStage에서 계속해서 확인할거임.
        reqStagePlayer.Enqueue(_reqStageReady[1]);
    }

    private void InitGameSetting()
    {
        //게임 시작 위해 레뒤 같은거 대기하기?
        RoomState = RoomState.Ready;
        SendRoomStateToLobbyServer();
    }

    private void GameOver()
    {
        ColorConsole.ConsoleColor("게임 오버");
        AnnouceGameOver();
        InitGameSetting();
    }
    #endregion

    #region 유저 퇴장
    Queue<int> removeQueue = new Queue<int>();
    private void AddRemoveSokect(int _numbering)
    {
        removeQueue.Enqueue(_numbering);
    }

    private void UpdateRemoveSockect()
    {
        Task.Run(() =>
        {
            while (true)
            {
                int removeCount = removeQueue.Count;
                for (int i = 0; i < removeCount; i++)
                {
                    int numbering = removeQueue.Dequeue();
                    for (int x = 0; x < roomUser.Count; x++)
                    {
                        if (numbering == roomUser[x].ID)
                        {

                            roomUser[x].workingSocket.Close();
                            roomUser[x].workingSocket.Dispose();
                            roomUser.RemoveAt(x);

                            Console.WriteLine(numbering + "소켓 제거 유저 남은 수 " + roomUser.Count);
                            AnnouceParty();
                            SendRoomCount(); //유저 나감에 따른 수치 변경

                            //나간 아이디가 아직 손님상태인경우 제외
                            if (numbering != 0 && RoomState == RoomState.Play)
                            {
                                //플레이중에 누가 나갔으면 게임 오버 
                                GameOver();
                            }
                            break;
                        }
                    }
                }
            }
        });

    }
    #endregion

    #region 방상태
    public RoomState RoomState
    {
        get
        {
            return roomState;
        }
        set
        {
            roomState = value;
        }
    }
    #endregion

    #region 통신
    private void SendChat(byte[] msg, int _receiveNumbering)
    {
        for (int i = 0; i < roomUser.Count; i++)
        {
            if (roomUser[i].ID == _receiveNumbering)
            {
                msg[1] = (byte)' ';
            }
            else
            {
                msg[1] = (byte)_receiveNumbering;
            }
            Socket socket = roomUser[i].workingSocket;
            if (socket.Connected)
            {
                SendMessege(socket, msg);
            }

        }
    }

    private void ExitClient(byte _exitID)
    {
        ColorConsole.ConsoleColor($"{_exitID}번 아이디가 나가길 요청 현재인원 :" + roomUser.Count);
        AddRemoveSokect(_exitID);
    }

    private void AnnouceCardArrange()
    {
        int useCardCount = roomUser.Count * 13; //나눠줄 카드 수
        for (int i = useCardCount; i < cards.Length; i++)
        {
            CardData selectCard = cards[i];
            if (selectCard.Compare(CardData.minClass, CardData.minRealValue) == 0)
            {
                //나누지 못한 카드 중에 시작 카드가 있으면, 앞에 어떤것과 해당 카드를 바꿀것. 
                Random ran = new Random();
                int changeIdx = ran.Next(0, useCardCount);
                CardData copy = cards[changeIdx];
                cards[changeIdx] = selectCard;
                cards[i] = copy;
                break;
            }
        }

        //4 명의 유저에게
        int giveCardIndex = 0;
        int cardDataStartIdx = 0;
        for (int userIndex = 0; userIndex < roomUser.Count; userIndex++)
        {
            // (byte)카드클래스, (byte)num 순으로 해당 리스트에 추가
            List<byte> cardList = new();
            cardList.Add((byte)ReqRoomType.Start);//요청 코드
            cardList.Add((byte)13);//줄 숫자
            cardDataStartIdx = cardList.Count(); //패킷에서 카드 시작 인덱스를 저장하기 위해서
            /*
             * [0] 요청코드 게임스타트
             * [1] 카드 숫자
             */
            //13장씩

            for (int cardCount = 1; cardCount <= 13; cardCount++)
            {
                CardData selectCard = cards[giveCardIndex];
                giveCardIndex++;

                cardList.Add((byte)selectCard.cardClass);
                cardList.Add((byte)selectCard.num);
                if (selectCard.Compare(CardData.minClass, CardData.minRealValue) == 0)
                {
                    turnId = roomUser[userIndex].ID; //서버에서 누가 시작인지 알고 있을것. 
                }
            }
            byte[] cardByte = cardList.ToArray();
            roomUser[userIndex].HaveCard = 13; //최초 13장으로 세팅.
            SendMessege(cardByte, userIndex);
        }

    }

    private void AnnouceParty()
    {
        ColorConsole.ConsoleColor("유저 방 참가를 알려줌");
        /*
         * [0] 응답코드 PartyIDes,
         * [1] ID를 받은 유효한 파티원 수
         * [2] 각 파티원 정보 길이
         * [3] 0번 파티원부터 정보 입력
         */
        List<byte> partyData = new();
        int partyInfoLength = 1; //일단 id값만 넘기기 때문에 임시로 1개
        partyData.Add((byte)ReqRoomType.PartyData);
        partyData.Add((byte)roomUser.Count); //일단 모든 파티원 수 담아놓음
        partyData.Add((byte)partyInfoLength);
        byte valid = 0;
        for (int i = 0; i < roomUser.Count; i++)
        {
            if (roomUser[i].ID != 0)
            {
                valid += 1;
                partyData.Add((byte)roomUser[i].ID);
            }
        }
        partyData[1] = valid; //아이디가 밝혀진 애들로 수 조절해서 전달
        byte[] partyDataByte = partyData.ToArray();
        SendMessege(partyDataByte);
    }

    private void AnnounceTurnPlayer()
    {
        ColorConsole.ConsoleColor("턴 지정 " + turnId.ToString());
        byte[] turnData = new byte[] { (byte)ReqRoomType.ArrangeTurn, (byte)turnId };
        SendMessege(turnData);
    }

    private void AnnoucePutDownCard(byte[] _putDownCardData)
    {
        /*
          * [0] 요청 코드 putdownCard
          * [1] 플레이어 id
          * [2] 낸 카드 숫자
          * [3] 카드 구성
        */
        ColorConsole.ConsoleColor("어떤 카드 냈는지 표기");
        //전체 데이터에서 해당 유저가 가진 카드를 빼도 되고
        //다음 차례 역할 지정하기 
        //해당 유저가 보유한 카드에서 감소
        int turnIndex = 0;
        for (int i = 0; i < roomUser.Count; i++)
        {
            if (roomUser[i].ID == _putDownCardData[1])
            {
                //제출한 녀석을 찾아서
                turnIndex = i; //얘 차례 인덱스 뽑고
            }
        }
        SendMessege(_putDownCardData);
        //현재 차례였던애 turnIndex;
        turnId = roomUser[(turnIndex + 1) % roomUser.Count].ID;
    }

    private void AnnouceStageOver()
    {
        /*
         * [0] 응답코드 스테이지오버
         * [1] 사람 수 - 4
         * [2] 아이디
         * [3] 보유 카드 수 반복
         */
        ColorConsole.ConsoleColor("해당 판 종료 공지");
        List<byte> stageOver = new();
        stageOver.Add((byte)ReqRoomType.StageOver);
        stageOver.Add((byte)roomUser.Count); //4명
                                             //사람이 나가서 AI로 해도 roomuser는 4명으로?
        for (int i = 0; i < roomUser.Count; i++)
        {
            stageOver.Add((byte)roomUser[i].ID);
            stageOver.Add((byte)roomUser[i].HaveCard);
        }
        byte[] announceData = stageOver.ToArray();
        SendMessege(announceData);
    }

    private void AnnouceGameOver()
    {
        //게임 오버시 전달할것들
        roomUser.Sort((a, b) => a.BadPoint.CompareTo(b.BadPoint)); //벌점 오름순으로 정렬
        /*
         * [0] 종료코드 GameOver
         * [1] 유저수 - 순위대로 정렬
         * [2] 보낼 정보 데이터 길이 일단 2
         * [3] 유저 ID
         * [4] 유저 벌점
         */
        List<byte> overDate = new();
        overDate.Add((byte)ReqRoomType.GameOver);
        overDate.Add((byte)roomUser.Count);
        overDate.Add(2); //보낼 데이터 길이
        for (int i = 0; i < roomUser.Count; i++)
        {
            overDate.Add((byte)roomUser[i].ID);
            overDate.Add((byte)roomUser[i].BadPoint);
        }
        SendMessege(overDate.ToArray());
    }


    #region 패킷 전달
    private void SendMessege(byte[] _messege)
    {
        for (int i = 0; i < roomUser.Count; i++)
        {
            SendMessege(roomUser[i].workingSocket, _messege);
        }
    }

    private void SendMessege(byte[] _msg, int _target)
    {
        SendMessege(roomUser[_target].workingSocket, _msg);
    }

    private void SendMessege(Socket _target, byte[] _msg)
    {
        ushort msgLength = (ushort)_msg.Length;
        byte[] msgLengthBuff = EndianChanger.HostToNet(msgLength);

        byte[] originPacket = new byte[msgLengthBuff.Length + msgLength];
        Buffer.BlockCopy(msgLengthBuff, 0, originPacket, 0, msgLengthBuff.Length); //패킷 0부터 메시지 길이 버퍼 만큼 복사
        Buffer.BlockCopy(_msg, 0, originPacket, msgLengthBuff.Length, msgLength); //패킷 메시지길이 버퍼 길이 부터, 메시지 복사

        int rest = (msgLength + msgLengthBuff.Length);
        int send = 0;
        do
        {
            byte[] sendPacket = new byte[rest];
            Buffer.BlockCopy(originPacket, originPacket.Length - rest, sendPacket, 0, rest);
            send = _target.Send(sendPacket);
            rest -= send;
        } while (rest >= 1);
    }
    #endregion

    #region 룸 상태 변경 전달
    public void SendRoomStateToLobbyServer()
    {
        IPEndPoint endPoint = new IPEndPoint(UniteServer.ServerIp, 5000);
        linkStateLobby = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        linkStateLobby.BeginConnect(endPoint, LobbyConnectCallBack, linkStateLobby);
    }

    public void LobbyConnectCallBack(IAsyncResult _result)
    {
        try
        {
            ReqChageRoomState();
        }
        catch
        {
            ColorConsole.ConsoleColor("방 서버의 로비서버 접속 실패");
        }
    }

    public void ReqChageRoomState()
    {
        //룸상태 변경 
        /*
         * [0] 코드 RoomState
         * [1] 변경될 타입 RoomState.
         * [2] 방이름 길이
         * [3] 여서부터 방이름
         */

        byte[] name = Encoding.Unicode.GetBytes(roomName);
        byte[] roomStateCode = new byte[] { (byte)ReqLobbyType.RoomState, (byte)RoomState, (byte)name.Length };
        byte[] reqStateByte = roomStateCode.Concat(name).ToArray();
        SendMessege(linkStateLobby, reqStateByte);
        linkStateLobby.Close();
        linkStateLobby.Dispose();
    }
    #endregion

    #region 룸 인원 변경 전달
    public void SendRoomCount()
    {
        IPEndPoint endPoint = new IPEndPoint(UniteServer.ServerIp, 5000);
        linkRoomCount = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        linkRoomCount.BeginConnect(endPoint, RoomCountCallBack, linkRoomCount);
    }

    public void RoomCountCallBack(IAsyncResult _result)
    {
        try
        {
            // Console.WriteLine("방 서버의 로비서버에 인원 변경 전달");
            ReqChangeRoomCount();
        }
        catch
        {
            ColorConsole.ConsoleColor("방수 변경 콜백 실패");
        }
    }

    public void ReqChangeRoomCount()
    {

        ColorConsole.ConsoleColor("방 인원 변경 함수 호출");
        /*
         * [0] 요청타입
         * [1] 현재 유저 수
         * [2] 방 이름 길이
         * [3] 부터 방 제목들
         */

        byte[] name = Encoding.Unicode.GetBytes(roomName);
        byte[] roomCode = new byte[] { (byte)ReqLobbyType.RoomUserCount, (byte)roomUser.Count, (byte)name.Length };
        byte[] resRoomCount = roomCode.Concat(name).ToArray();

        SendMessege(linkRoomCount, resRoomCount);
        linkRoomCount.Close();
        linkRoomCount.Dispose();
    }
    #endregion

    #endregion
}

