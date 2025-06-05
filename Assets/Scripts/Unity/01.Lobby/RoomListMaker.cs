using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomListMaker : MonoBehaviour
{
    public static RoomListMaker instance;
    public Queue<List<RoomData>> roomDataQueue = new();
    public RoomDataUI[] m_roomDataUIs;
    public int m_page = 0;

    public List<RoomData> curRoomDataList = new();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        ShowRoomList();
    }

    // Update is called once per frame
    void Update()
    {
        if(roomDataQueue.TryDequeue(out List<RoomData> _result))
        {
            curRoomDataList = _result;
            ShowRoomList();
        }
    }

    private void ShowRoomList()
    {
        int min = Mathf.Min(m_roomDataUIs.Length, curRoomDataList.Count);
        for (int i = 0; i < min; i++)
        {
            m_roomDataUIs[i].gameObject.SetActive(true);
            m_roomDataUIs[i].SetRoomData(curRoomDataList[i]);
        }
        for (int i = min; i < m_roomDataUIs.Length; i++)
        {
            m_roomDataUIs[i].gameObject.SetActive(false);
        }
    }
}
