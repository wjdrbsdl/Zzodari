using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class UserTurnTimer : MonoBehaviour
{
    private float turnTime; //플레이어마다 다를수도
    public float restTime = 0f;
    public bool haveToCount = false; //카운팅 해야하는가
    private bool isMe = false; //나인가
    public RoomInfoManager m_roomManager;
    private float defaultY; //초기 이미지 길이
    private float defaultX;
    public Image timeLine;

    private void Start()
    {
        defaultY = timeLine.gameObject.GetComponent<RectTransform>().sizeDelta.y;
        defaultX = timeLine.gameObject.GetComponent<RectTransform>().sizeDelta.x;
        DrawLineZero();
    }

    public void StartTimer(PlayerData _playerData)
    {
        haveToCount = true;
        isMe = _playerData.isMe; //나인지 할당
        turnTime = 10;
        restTime = turnTime;
    }

    public void StopTimer()
    {
        ResetTimer();
    }

    private void ExceedTimer()
    {
        //턴 종료되었음을 전달 
        if (isMe) //나의 턴 타이머가 종료 되었을때 룸매니저에게 콜백
        {
            m_roomManager?.TimerExceedCallBack();
        }
        
        ResetTimer();
    }

    private void ResetTimer()
    {
        haveToCount = false;
        restTime = 0;
        DrawLineZero();
    }

    // Update is called once per frame
    void Update()
    {
        if(haveToCount == false)
        {
            return;
        }

        
        restTime -= Time.deltaTime;
        DrawTimeLine();
        if (restTime <= 0)
        {
            ExceedTimer();
        }
    }

    private void DrawTimeLine()
    {
        float ratio = restTime;
        if (ratio < 0)
        {
            ratio = 0;
        }
        timeLine.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(defaultX, ratio / turnTime * defaultY);
    }

    private void DrawLineZero()
    {
        timeLine.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(defaultX,0);
    }
}
