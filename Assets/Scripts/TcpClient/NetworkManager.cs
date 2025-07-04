using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;


public class NetworkManager
{
    public static Socket clientSocket;
    public static byte[] ip;
    public static int port;

    public event Action<byte[]> OnDataReceived;
    public event Action OnConnected;
    public static Action OnDetectDisConnect;

    public void Connect()
    {
        //UnityEngine.Debug.Log("넷웟 연결시도해보기");

        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPAddress ipAddress = new IPAddress(ip);
        IPEndPoint endPoint = new IPEndPoint(ipAddress, port);
        clientSocket.BeginConnect(endPoint, ConnectCallback, null);
    }

    private void ConnectCallback(IAsyncResult result)
    {
        try
        {
            // UnityEngine.Debug.Log("컨넥콜백");
            clientSocket.EndConnect(result);
            OnConnected?.Invoke();
            BeginReceive();
            StartDetectSocketClose();
        }
        catch
        {
            Connect(); // retry
        }
    }

    private void BeginReceive()
    {
        byte[] headerBuffer = new byte[2];
        if (clientSocket.Connected)
        {
            clientSocket.BeginReceive(headerBuffer, 0, 2, 0, ReceiveCallback, headerBuffer);
        }

    }

    private void ReceiveCallback(IAsyncResult result)
    {
        try
        {
            // DebugManager.instance.EnqueMessege("매니저에서 받음");
            byte[] msgLengthBuff = result.AsyncState as byte[]; //받을그릇을 2개로 받기 - 메시지 길이 정의
            ushort msgLength = EndianChanger.NetToHost(msgLengthBuff);

            byte[] recvBuffer = new byte[msgLength];
            byte[] recvData = new byte[msgLength];
            int recv = 0;
            int recvIdx = 0;
            int rest = msgLength;
            do
            {
                recv = clientSocket.Receive(recvBuffer);
                Buffer.BlockCopy(recvBuffer, 0, recvData, recvIdx, recv);
                recvIdx += recv;
                rest -= recv;
                recvBuffer = new byte[rest];//퍼올 버퍼 크기 수정
                if (recv == 0)
                {
                    //만약 남은게있으면 어떡함?
                    break;
                }
            } while (rest >= 1);

            OnDataReceived?.Invoke(recvData);
            BeginReceive();
        }
        catch
        {
            // 연결 실패 또는 중단 처리
        }
    }

    public void Send(byte[] data)
    {
        // DebugManager.instance.EnqueMessege("매니저에서 샌드");
        ushort length = (ushort)data.Length;
        byte[] header = EndianChanger.HostToNet(length);
        byte[] packet = new byte[header.Length + data.Length];
        Buffer.BlockCopy(header, 0, packet, 0, header.Length);
        Buffer.BlockCopy(data, 0, packet, header.Length, data.Length);
        //clientSocket.Send(packet);


        int rest = (length + header.Length);
        int send = 0;
        do
        {
            byte[] sendPacket = new byte[rest];
            Buffer.BlockCopy(packet, packet.Length - rest, sendPacket, 0, rest);
            send = clientSocket.Send(sendPacket);
            rest -= send;
        } while (rest >= 1);
    }

    private bool clearDis = false;
    public void Disconnect()
    {
       // UnityEngine.Debug.Log("깔끔한 종료");
        //클라의 요청으로 인해 종료된 경우
        clearDis = true;
        clientSocket?.Close();
        clientSocket?.Dispose();
        clientSocket = null;

    }

    public void InvalidDisConnect()
    {
        //테스트를 위해 ip 전환처럼 클라 요청없이 연결이 끊긴경우
        clientSocket?.Close();
        clientSocket?.Dispose();
        clientSocket = null;
    }

    private CancellationTokenSource disconnectCheckCts;
    private async void StartDetectSocketClose()
    {
        disconnectCheckCts = new CancellationTokenSource();
        var token = disconnectCheckCts.Token;

        while (!token.IsCancellationRequested)
        {
            await Task.Delay(500); // 0.5초마다 확인
            if (clearDis == false && (clientSocket == null || !clientSocket.Connected))
            {
                //UnityEngine.Debug.Log("소켓 끊긴거 확인 재연결");
                OnDetectDisConnect?.Invoke();
                Connect(); //연결했던 소켓에 재연결 시도
                break;
            }

            //// 추가: 실제로 소켓 상태가 정상인지 확인하려면 아래 코드로 체크
            //bool isSocketAlive = !(clientSocket.Poll(1, SelectMode.SelectRead) && clientSocket.Available == 0);

            //if (!isSocketAlive)
            //{
            //    Debug.LogWarning("소켓 Poll 검사에서 끊김 감지됨");
            //    OnDisconnected?.Invoke();
            //    break;
            //}
        }
    }
}
