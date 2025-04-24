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
    public bool m_isMine = false;

    public void SetCardData(CardData _cardData)
    {
        m_cardData = _cardData;
        m_cardClassRender.sprite = AssetManager.intance.GetClass(_cardData.cardClass);
        m_cardNum.sprite = AssetManager.intance.GetNumber(_cardData.num);
        m_isCurSelect = false;
        m_isDragging = false;
    }

    Transform basePos;
    public void SetBasePos(Transform _transform)
    {
        //기준 transform이 바뀌는거에 따라 추적하기 위해 transform으로 저장
        basePos = _transform;
    }
    float m_startX;
    public void SetStartX(float _x)
    {
        m_startX = _x;
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

        //이전 고정된 tranform 용
        //Vector3 direct = m_arrangePos - transform.position;
        //transform.Translate(direct * m_moveSpeed * Time.deltaTime);

        Vector3 testArragne = basePos.position + new Vector3( m_startX,0,0);
        Vector3 testDirect = testArragne - transform.position;
        transform.Translate(testDirect * m_moveSpeed * Time.deltaTime);
    }

}
