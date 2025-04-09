using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardObject : MonoBehaviour
{
    public SpriteRenderer m_cardClassRender;
    public SpriteRenderer m_cardNum;
    public GameObject m_effectPrefab;
    public Transform m_effectTransform;
    public CardData m_cardData;
    public Vector3 m_arrangePos; //월드상 지정된 좌표 
    public float m_moveSpeed = 3f;
    public bool m_isCurSelect = false; //지금 선택된 카드인가
    public bool m_isDragging = false;

    public void SetCardData(CardData _cardData)
    {
        m_cardData = _cardData;
        m_cardClassRender.sprite = AssetManager.intance.GetClass(_cardData.cardClass);
        m_cardNum.sprite = AssetManager.intance.GetNumber(_cardData.num);
        m_isCurSelect = false;
        m_isDragging = false;
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
        if(m_isDragging == true)
        {
            //플레이어에 의해서 드래깅중이면 끌려가기
            //MakeEffect();
            return;
        }

        if(m_arrangePos == Vector3.zero)
        {
            return;
        }

        Vector3 direct = m_arrangePos - transform.position;
        transform.Translate(direct * m_moveSpeed * Time.deltaTime);
    }

    float ratio = 2f;
    private float rest = 0f;
    private void MakeEffect()
    {
        rest -= Time.deltaTime;
        if(rest < 0)
        {
            Instantiate(m_effectPrefab, m_effectTransform);
            rest = ratio;
        }
    }
}
