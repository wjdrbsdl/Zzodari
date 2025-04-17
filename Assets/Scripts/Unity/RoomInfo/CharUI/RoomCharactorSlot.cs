using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class RoomCharactorSlot : MonoBehaviour
{
    public PlayerData m_playerData;
    public Image m_charImage;
    public Image m_backGround;
    public Color m_myColor = Color.green;
    public Color m_otherColor = Color.black;
    public TMP_Text m_charId;
    public TMP_Text m_badPoint;
    public TMP_Text m_restCardCount;
    public UserTurnTimer m_userTimer;

    // Use this for initialization
    public void SetPlayerData(PlayerData _pData)
    {
        m_playerData = _pData;
    }

    public void ResetPlayerData()
    {
        m_playerData = null;
        m_backGround.gameObject.SetActive(false);
        SetText("", "", "");
        
    }

    public void RenewPlayerData()
    {
        //벌점이나 뭐 등등 갱신하기
        if(m_playerData == null)
        {
            return;
        }
        m_backGround.gameObject.SetActive(true);
        SetColor();
        SetText(m_playerData.ID, m_playerData.badPoint.ToString(), m_playerData.restCardCount.ToString());
    }

    public void ResetScore()
    {
        //0점 잡기
        m_badPoint.text = 0.ToString();
    }

    IEnumerator curCoru = null;
    public void ReScore(int _resetPoint)
    {
        int preScore = int.Parse(m_badPoint.text);
        if(curCoru != null)
        {
            StopCoroutine(curCoru);
        }
        curCoru = CoReScore(preScore, _resetPoint);
        StartCoroutine(curCoru);
    }

    IEnumerator CoReScore(int _start, int _end)
    {
        float time = 1f;
        float curTime = 0f;
        while (curTime < time)
        {
            curTime += Time.deltaTime;
            int lerpValue = (int)Mathf.Lerp(_start, _end, curTime / time);
            m_badPoint.text = lerpValue.ToString();
            yield return null;
        }

        curCoru = null;
    }

    private void SetColor()
    {
        if (m_playerData.isMe)
        {
            m_backGround.color = m_myColor;
        }
        else
        {
            m_backGround.color = m_otherColor;
        }
    }

    private void SetText(string _id, string _badPoint, string _restCount)
    {
        m_charId.text = "ID : "+_id;
       // m_badPoint.text = "벌점 : "+ _badPoint;
        m_restCardCount.text = "남은 카드 : "+ _restCount;
    }

    public void StartTimer()
    {
        m_userTimer.StartTimer(m_playerData);
    }

    public void StopTimer()
    {
        m_userTimer.StopTimer();
    }
}
