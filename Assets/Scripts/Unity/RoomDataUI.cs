using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomDataUI : MonoBehaviour
{
    public Text m_nameText;
    public Text m_countText;
    public RoomData m_roomData;

    public void SetRoomData(RoomData _roomData)
    {
        m_roomData = _roomData;
        m_nameText.text = _roomData.roomName;
        m_countText.text = $"{_roomData.curCount}/{RoomData.maxCount}";
    }

    public void OnClickRoomJoin()
    {
        ClientManager.instance.OnClickRoomJoin(m_roomData.roomName);
    }

}
