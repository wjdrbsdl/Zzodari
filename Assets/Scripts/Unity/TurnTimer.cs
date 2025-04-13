using System.Collections;
using UnityEngine;


public class TurnTimer : MonoBehaviour
{
    public float turnTime = 20f;
    public float restTime = 0f;
    public bool isMyTurn = false;
    private RoomInfoManager m_roomManager;
    // Use this for initialization
    public void CountMyTurn(RoomInfoManager _roomManager)
    {
        isMyTurn = true;
        restTime = turnTime;
        m_roomManager = _roomManager;
    }

    public void EndMyTurn()
    {
        //시간내에 카드 제출 시 
        isMyTurn = false;
        restTime = 0;
    }

    private void ExceedMyTurn()
    {
        isMyTurn = false;
        restTime = 0;
        //턴 종료되었음을 전달 
        m_roomManager.TimerExceedCallBack();
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isMyTurn == false)
        {
            return;
        }

        restTime -= Time.deltaTime;
        m_roomManager.m_roomNameText.text = restTime.ToString();
        if (restTime <= 0)
        {
            ExceedMyTurn();
        }
    }
}
