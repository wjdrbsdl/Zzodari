using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRoomCharactor : MonoBehaviour
{

    public RoomCharactorSlot[] m_roomCharSlots;
    private InGameData m_gameData;

    public void SetInGameData(InGameData _gameData)
    {
        m_gameData = _gameData;
        List<PlayerData> pDataList = m_gameData.m_partyList;
        for (int i = 0; i < pDataList.Count; i++)
        {
            m_roomCharSlots[i].SetPlayerData(pDataList[i]);
        }
        for(int i = pDataList.Count; i < 4; i++)
        {
            m_roomCharSlots[i].ResetPlayerData();
        }
        Renew();
    }

    public void Renew()
    {
        for (int i = 0; i < m_roomCharSlots.Length; i++)
        {
            m_roomCharSlots[i].RenewPlayerData();
        }
    }

}
