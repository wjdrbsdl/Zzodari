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
    public UniteLobClient lobClient; //생성된 녀석
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
        Debug.Log(UniteServer.ServerIp);
        if(IsRoomOut && UniteServer.ServerIp != null)
        {
            //이미 접속했었던 상태에서 돌아왔다면
            lobClient = Instantiate(PrefabManager.instance.uniteLobClient);
            lobClient.ip = UniteServer.ServerIp.ToString();
            lobClient.id = 1;
            lobClient.Connect();
            IsRoomOut = false;
            ControlPanel(EPanelType.LobbyUI);
        }
    }

    public void OnClickConnect()
    {
        string ip = inputText.text;
        //올바른 ip인지 체크 후
        if (IsValidForm(ip) == true)
        {
            inputText.text = "";
            Action callBack = () => 
            {
                Debug.Log("유니티 콜백받음");
                DebugManager.instance.EnqueMessege( "test");
                GameObject gameObject = new GameObject();
                gameObject.name = "테스트";
            };
            //lobClient = Instantiate(PrefabManager.instance.lobClient);
            lobClient = Instantiate(PrefabManager.instance.uniteLobClient);
            lobClient.ip = ip;
            lobClient.id = 1;
            lobClient.Connect();
            ControlPanel(EPanelType.LobbyUI);
        }
    }

    public void OnClickRoom()
    {
        if(lobClient != null)
        {
            lobClient.OnClickReqRoomJoin();
        }
    }

    public void OnClickReqRoomList()
    {
        lobClient.ReqRoomList(); //클 매니저 버튼으로 호출
    }

    private bool IsValidForm(string ip)
    {
        //ipv4 가정
        string[] intSplit = ip.Split(".");
        //4구역이 나왔나
        if (intSplit.Length != 4)
        {
            return false;
        }
        //모두 숫자이면서 0이상 255 아래인가
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
}
