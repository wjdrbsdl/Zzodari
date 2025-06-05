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

    public void ReScore(InGameData _gameData)
    {
        List<PlayerData> pDataList = _gameData.m_partyList;
        for (int i = 0; i < pDataList.Count; i++)
        {
            m_roomCharSlots[i].ReScore(pDataList[i].badPoint);
        }
    }

    #region 타이머 조절하기
    public void ReTimer(InGameData _gameData)
    {
        string turnId = _gameData.curTurnId;
        List<PlayerData> pDataList = _gameData.m_partyList;
        for (int i = 0; i < pDataList.Count; i++)
        {
            if (pDataList[i].ID == turnId)
            {
                m_roomCharSlots[i].StartTimer();
            }
            else
            {
                m_roomCharSlots[i].StopTimer();
            }
        }
    }

    public void StopTimer(InGameData _gameData)
    {
        List<PlayerData> pDataList = _gameData.m_partyList;
        for (int i = 0; i < pDataList.Count; i++)
        {
            m_roomCharSlots[i].StopTimer();
        }
    }
    #endregion

    public void ShowRank(InGameData _gameData)
    {
        string turnId = _gameData.curTurnId;
        List<PlayerData> pDataList = _gameData.m_partyList;
        for (int i = 0; i < pDataList.Count; i++)
        {
            m_roomCharSlots[i].SetRank();

        }
    }

    public void OffRank(InGameData _gameData)
    {
        string turnId = _gameData.curTurnId;
        List<PlayerData> pDataList = _gameData.m_partyList;
        for (int i = 0; i < pDataList.Count; i++)
        {
            m_roomCharSlots[i].OffRank();
        }
    }

    public void ShowPutDownIcon(InGameData _gameData)
    {
        string preTurnId = _gameData.preCardId;
        EMixtureType preMixture = _gameData.preMixtureType;
        
        List<PlayerData> pDataList = _gameData.m_partyList;
        for (int i = 0; i < pDataList.Count; i++)
        {
            if (pDataList[i].ID == preTurnId)
            {
                m_roomCharSlots[i].ShowActIcon(preMixture);
                break;
            }
           
        }
    }
}
