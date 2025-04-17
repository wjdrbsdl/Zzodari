using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class ClientManager : MonoBehaviour
{
    public static bool IsRoomOut = false;
    public static ClientManager instance;
    public InputField inputText;
    public UniteLobClient lobClient; //������ �༮
    //public LobbyClient lobClient;

    public PanelObj connectPanel;
    public PanelObj lobbyPanel;

    private void Awake()
    {
        instance = this;
        ControlPanel(EPanelType.ConnectUI);
    }

    private void Start()
    {
       // Debug.Log(UniteServer.ServerIp);
        if(IsRoomOut && UniteServer.ServerIp != null)
        {
            //�̹� �����߾��� ���¿��� ���ƿԴٸ�
            lobClient = Instantiate(PrefabManager.instance.uniteLobClient);
            lobClient.ip = UniteServer.ServerIp.ToString();
            lobClient.id = 1;
            lobClient.Connect();
            IsRoomOut = false;
        }
    }

    public void OnClickConnect()
    {
        string ip = inputText.text;
        //�ùٸ� ip���� üũ ��
        if (IsValidForm(ip) == true)
        {
            inputText.text = "";
            Action callBack = () => 
            {
                Debug.Log("����Ƽ �ݹ����");
                DebugManager.instance.EnqueMessege( "test");
                GameObject gameObject = new GameObject();
                gameObject.name = "�׽�Ʈ";
            };
            //lobClient = Instantiate(PrefabManager.instance.lobClient);
            lobClient = Instantiate(PrefabManager.instance.uniteLobClient);
            lobClient.ip = ip;
            lobClient.id = 1;
            lobClient.Connect();
         
        }
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
        lobClient.ReqRoomList(); //Ŭ �Ŵ��� ��ư���� ȣ��
    }

    private bool IsValidForm(string ip)
    {
        //ipv4 ����
        string[] intSplit = ip.Split(".");
        //4������ ���Գ�
        if (intSplit.Length != 4)
        {
            return false;
        }
        //��� �����̸鼭 0�̻� 255 �Ʒ��ΰ�
        for (int i = 0; i < intSplit.Length; i++)
        {
            if (int.TryParse(intSplit[i], out int num) == false)
            {
                return false;
            }

            if (num < 0)
            {
                return false;
            }

            if (255 < num)
            {
                return false;
            }
        }
        return true;
    }

    string inputStr = "";

    public bool GetInputText(out string text)
    {
        text = inputStr;
        inputStr = "";
        if(text == "")
        {
            return false;
        }
        return true;
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
