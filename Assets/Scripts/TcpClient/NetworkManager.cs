using System;
using System.Net;
using System.Net.Sockets;



public class NetworkManager
{
    private Socket clientSocket;
    private byte[] ip;
    private int port;

    public event Action<byte[]> OnDataReceived;
    public event Action OnConnected;

    public void Connect(byte[] ip, int port)
    {
        DebugManager.instance.EnqueMessege("넷웟 연결시도해보기");
        this.ip = ip;
        this.port = port;
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPAddress ipAddress = new IPAddress(ip);
        IPEndPoint endPoint = new IPEndPoint(ipAddress, port);
        clientSocket.BeginConnect(endPoint, ConnectCallback, null);
    }

    private void ConnectCallback(IAsyncResult result)
    {
        try
        {
            DebugManager.instance.EnqueMessege("컨넥콜백");
            clientSocket.EndConnect(result);
            OnConnected?.Invoke();
            BeginReceive();
        }
        catch
        {
            Connect(ip, port); // retry
        }
    }

    private void BeginReceive()
    {
        byte[] headerBuffer = new byte[2];
        clientSocket.BeginReceive(headerBuffer, 0, 2, 0, ReceiveCallback, headerBuffer);
    }

    private void ReceiveCallback(IAsyncResult result)
    {
        try
        {
            byte[] header = (byte[])result.AsyncState;
            ushort msgLength = EndianChanger.NetToHost(header);
            byte[] body = new byte[msgLength];
            int received = clientSocket.Receive(body);

            if (received > 0)
            {
                OnDataReceived?.Invoke(body);
            }
            BeginReceive();
        }
        catch
        {
            // 연결 실패 또는 중단 처리
        }
    }

    public void Send(byte[] data)
    {
        DebugManager.instance.EnqueMessege("매니저에서 샌드");
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
