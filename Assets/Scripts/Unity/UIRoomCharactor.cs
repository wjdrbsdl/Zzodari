using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRoomCharactor : MonoBehaviour
{

    public RoomCharactorSlot[] m_roomCharSlots;

    public void Renew(InGameData _gameData)
    {
        List<PlayerData> pDataList = _gameData.m_partyList;
        for (int i = 0; i < pDataList.Count; i++)
        {
            m_roomCharSlots[i].SetPlayerData(pDataList[i]);
            m_roomCharSlots[i].RenewPlayerData();
        }
        for (int i = pDataList.Count; i < 4; i++)
        {
            m_roomCharSlots[i].ResetPlayerData();
        }
    }

}
