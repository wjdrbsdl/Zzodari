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

    public GameObject m_rankGo;
    public GoOffTimer m_passGo;
    public TMP_Text m_rankText;

    // Use this for initialization
    #region 데이터 세팅 및 갱신
    public void SetPlayerData(PlayerData _pData)
    {
        //매번 하는구만..
        m_playerData = _pData;
     }

    public void ResetPlayerData()
    {
        m_playerData = null;
        m_backGround.gameObject.SetActive(false);
        SetText("", "");
        m_badPoint.text = "";
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
        SetText(m_playerData.ID, m_playerData.restCardCount.ToString());
    }
    #endregion

    #region 점수
    IEnumerator curCoru = null;
    public void ReScore(int _resetPoint)
    {
       // Debug.Log("갱신할 점수 " + _resetPoint);
        if(int.TryParse(m_badPoint.text, out int preScore) == false)
        {
            preScore = 0;
        }
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
    #endregion

    #region 기본 정보 와 색
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

    private void SetText(string _id, string _restCount)
    {
        m_charId.text = "ID : "+_id;

        m_restCardCount.text = "남은 카드 : "+ _restCount;
    }
    #endregion

    #region 타이머
    public void StartTimer()
    {
        m_userTimer.StartTimer(m_playerData);
    }

    public void StopTimer()
    {
        m_userTimer.StopTimer();
    }
    #endregion

    #region 랭크
    public void OffRank()
    {
        m_rankGo.SetActive(false);
    }

    public void SetRank()
    {
        m_rankGo.SetActive(true);
        m_rankText.text = m_playerData.rank.ToString();
    }
    #endregion

    public void ShowPassIcon(EMixtureType _mixtureType)
    {
        if (_mixtureType.Equals(EMixtureType.Pass))
        {
            m_passGo.StartTimer();
        }
    }
}
