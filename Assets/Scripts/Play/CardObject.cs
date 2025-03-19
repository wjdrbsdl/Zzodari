using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardObject : MonoBehaviour
{
    public SpriteRenderer m_cardClassRender;
    public SpriteRenderer m_cardNum;
    public CardData m_cardData;
    public Vector3 m_arrangePos; //월드상 지정된 좌표 
    public float m_moveSpeed = 3f;

    public void SetCardData(CardData _cardData)
    {
        m_cardData = _cardData;
        m_cardClassRender.sprite = AssetManager.intance.GetClass(_cardData.cardClass);
        m_cardNum.sprite = AssetManager.intance.GetNumber(_cardData.num);
    }

    public void SetArrangePos(Vector3 _pos)
    {
        m_arrangePos = _pos;
    }

    public void Update()
    {
        Move();
    }

    private void Move()
    {
        if(m_arrangePos == Vector3.zero)
        {
            return;
        }

        Vector3 direct = m_arrangePos - transform.position;
        transform.Translate(direct * m_moveSpeed * Time.deltaTime);
    }
}
