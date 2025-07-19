using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class ClientManager : MonoBehaviour
{
    public static bool IsRoomOut = false;
    public static ClientManager instance;
    public InputField inputText;
    public UniteLobClient lobClient; //생성된 녀석
    //public LobbyClient lobClient;

    public PanelObj connectPanel;
    public PanelObj lobbyPanel;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        SoundManager.Instance.PlayBgm(BGMType.BgmLobby);
        //룸에서 로비로 돌아온 경우
        if (IsRoomOut && FixedValue.ServerIp != null)
        {
            lobClient = Instantiate(PrefabManager.instance.uniteLobClient);
            lobClient.ip = FixedValue.ServerIp.ToString();
            lobClient.id = 1;
            lobClient.Connect();
            IsRoomOut = false;
            return;
        }
        ControlPanel(EPanelType.ConnectUI);
        StartCoroutine(CoConnect());
    }

    IEnumerator CoConnect()
    {
        while (true)
        {
            //2초마다 
            if(FixedValue.ServerIp == null)
            {
                //2초마다 
                PopUpManager.Instance.SendPopMessege("아이피 파싱중");
                yield return new WaitForSeconds(1f); 
            }
            else
            {
                PopUpManager.Instance.SendPopMessege("콘넥트 시작"+FixedValue.ServerIp);
                ConnectLobby(FixedValue.ServerIp);
                break;
            }
            
        }
    }

    private void ConnectLobby(IPAddress ip)
    {
        lobClient = Instantiate(PrefabManager.instance.uniteLobClient);
        lobClient.ip = ip.ToString();
        lobClient.id = 1;
        lobClient.Connect();
    }

    public void OnClickRoom()
    {
        if(lobClient != null)
        {
            lobClient.OnClickReqRoomJoin();
        }
    }

    public void OnClickRoomJoin(string _roomName)
    {
        if (lobClient != null)
        {
            lobClient.ReqRoomJoin(_roomName);
        }
    }

    public void OnClickReqRoomList()
    {
        lobClient.ReqRoomList(); //클 매니저 버튼으로 호출
    }

    private void ControlPanel(EPanelType _onPanel)
    {
        connectPanel.OnOff(_onPanel);
        lobbyPanel.OnOff(_onPanel);
    }

    public void ConnectResult(bool _isConnect)
    {
        if (_isConnect)
        {
            ControlPanel(EPanelType.LobbyUI);
        }
        else
        {
            ControlPanel(EPanelType.ConnectUI);
        }
    }

    Queue<Action> callBackQue = new();
    private void Update()
    {
        if (callBackQue.TryDequeue(out Action callBack))
        {
            callBack.Invoke();
        }
    }

    public void EnqueAction(Action _action)
    {
        callBackQue.Enqueue(_action);
    }
}
