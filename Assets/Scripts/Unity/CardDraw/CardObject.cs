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
    public Vector3 m_arrangePos; //����� ������ ��ǥ 
    public float m_moveSpeed = 3f;
    public bool m_isCurSelect = false; //���� ���õ� ī���ΰ�
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
        //���� transform�� �ٲ�°ſ� ���� �����ϱ� ���� transform���� ����
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
            //�÷��̾ ���ؼ� �巡�����̸� ��������
            //MakeEffect();
            return;
        }

        if(m_arrangePos == Vector3.zero)
        {
            return;
        }

        //���� ������ tranform ��
        //Vector3 direct = m_arrangePos - transform.position;
        //transform.Translate(direct * m_moveSpeed * Time.deltaTime);

        Vector3 testArragne = basePos.position + new Vector3( m_startX,0,0);
        Vector3 testDirect = testArragne - transform.position;
        transform.Translate(testDirect * m_moveSpeed * Time.deltaTime);
    }

}
