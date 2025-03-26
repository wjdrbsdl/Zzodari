using System.Collections;
using UnityEngine;


public enum EPanelType
{
    None, ConnectUI, LobbyUI
}

public class PanelObj : MonoBehaviour
{

    public EPanelType m_panelType;
    public GameObject m_window;

    public void OnOff(EPanelType _onPanel)
    {
        m_window.SetActive(m_panelType == _onPanel);
    }
    
}
