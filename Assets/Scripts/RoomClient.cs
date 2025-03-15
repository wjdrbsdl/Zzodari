using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public enum ReqType
{
    RoomMake, RoomStart, RoomOut
}

public class RoomClient
{

    public Socket clientSocket;
    public int port = 5000;
    public string ip;
    public int id;
    public MeetState meetState = MeetState.Lobby;
    Action unityCallBack;
    public RoomClient(string _ip, int _id, Action _callBack)
    {
        ip = _ip;
        id = _id;
        unityCallBack = _callBack;
    }

    public void Connect()
    {
        Debug.Log("연결 시도");
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPAddress ipAddress = IPAddress.Parse(ip);
        IPEndPoint endPoint = new IPEndPoint(ipAddress, port);
        //메인스레드인지 테스트
        unityCallBack?.Invoke();
        return;
        clientSocket.BeginConnect(endPoint, new AsyncCallback(CallBackConnect), clientSocket);
    }

    private void CallBackConnect(IAsyncResult _result)
    {
        Debug.Log("콜백 받음");
        try
        {
            //연결 되었으면 자료 받을 준비, 상태 준비
            Debug.Log("클라 연결 콜백");
            
            byte[] buff = new byte[100];
            clientSocket.BeginReceive(buff, 0, buff.Length, 0, CallBackReceive, buff);
            unityCallBack?.Invoke(); //직접실행
            //UnityMainThreadDispatcher.Enqueue(unityCallBack);  //업데이트에서 실행
        }
        catch(Exception e)
        {
            Debug.Log("클라 연결 끊김" + e.Message);
        }
        
    }

    private void CallBackReceive(IAsyncResult _result)
    {
        Debug.Log("클라 리십 콜백");
        byte[] receiveBuff = _result.AsyncState as byte[];

        //ReqType reqType = (ReqType)receiveBuff[0];
        //if (reqType == ReqType.RoomMake)
        //{
        //    ResRoomJoin(receiveBuff);
        //}
        //else if (reqType == ReqType.RoomStart)
        //{
        //    ResRoomStart(receiveBuff);
        //}

        clientSocket.BeginReceive(receiveBuff, 0, receiveBuff.Length, 0, CallBackReceive, receiveBuff);
    }
}
