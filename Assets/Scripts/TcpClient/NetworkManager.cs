using System;
using System.Net;
using System.Net.Sockets;


public class NetworkManager 
{
    public static Socket clientSocket;
    public static byte[] ip;
    public static int port;

    public event Action<byte[]> OnDataReceived;
    public event Action OnConnected;

    public void Connect()
    {
       // UnityEngine.Debug.Log("넷웟 연결시도해보기");

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

    public void Disconnect()
    {
        clientSocket?.Close();
        clientSocket?.Dispose();
    }
}
