using System;
using System.Collections.Generic;
using UnityEngine;


public class CardManager : MonoBehaviour
{
    public static CardManager intance;

    public PlayClient m_pClient;
    public ArrangeCardObj m_arrangeHandCard;
    public ArrangeCardObj m_arrangeSelectCard;
    public Transform cardHands;
    public CardObject cardSample;
    public CardObject[] cards;

    public GameObject m_selectZoneObj;

    public List<CardData> m_haveCardList = new List<CardData>();

    #region
    void Awake()
    {
        intance = this;
    }
    private void Start()
    {
        MakeCardObject();
    }
    #endregion
    public Queue<Action> callBack = new();

    private void Update()
    {
        if(callBack.TryDequeue(out Action action))
        {
            action.Invoke();
        }
    }

    private void MakeCardObject()
    {
        cards = new CardObject[13];
        //카드 최대 수치는 13장으로 정해져있음. 고로 만들어놓기
        for (int i = 0; i < 13; i++)
        {
            CardObject newObj = Instantiate(cardSample);
            newObj.transform.SetParent(cardHands);
            newObj.gameObject.SetActive(false);
            cards[i] = newObj;
        }
        
    }

    //처음 가졌을때?
    public void SetHaveCard(List<CardData> _haveCardList)
    {
        m_haveCardList = _haveCardList;
        //있는 만큼 켜고 세팅
        UpdateCards();
    }

    public void UpdateCards()
    {
        //데이터상 보유카드 변경되었을때
        for (int i = 0; i < m_haveCardList.Count; i++)
        {
            cards[i].SetCardData(m_haveCardList[i]);
            cards[i].gameObject.SetActive(true);
        }
        //나머진 끔
        for (int i = m_haveCardList.Count; i < 13; i++)
        {
            cards[i].gameObject.SetActive(false);
        }

        CardObject[] activeCards = cardHands.GetComponentsInChildren<CardObject>();
        m_arrangeSelectCard.ResetList();
        m_arrangeHandCard.ResetList();
        m_arrangeHandCard.SetCardObjects(activeCards);
    }

    #region 드래그 반응
    public void EndDrag(CardObject _object, bool _isSelectZone)
    {
        Debug.Log("카드를 선택존에 놓았는가" + _isSelectZone);
        //카드 드래그를 끝냈을때, 끝낸 위치가 셀렉존인가. 
        if(_object.m_isCurSelect == _isSelectZone)
        {
            //있던 위치에 놓은 애는 별 조작 없음
            return;
        }

        if (_isSelectZone)
        {
            DragHandToSelect(_object);
        }
        else
        {
            DragSelectToHand(_object);
        }
    }

    private void DragHandToSelect(CardObject _object)
    {
        // 드래그중이던 카드를 셀렉존에 놓았을때
        m_arrangeHandCard.RemoveCardObject(_object);
        m_arrangeSelectCard.AddCardObject(_object);
        _object.m_isCurSelect = true;
    }

    private void DragSelectToHand(CardObject _object)
    {
        m_arrangeSelectCard.RemoveCardObject(_object);
        m_arrangeHandCard.AddCardObject(_object);
        _object.m_isCurSelect = false;
    }
    #endregion

    public void OnClickPutDown()
    {
       bool put = m_pClient.PutDownCards(m_arrangeSelectCard.GetCardDataList());
    }

    public void OnClickSort()
    {
        m_pClient.SortCardList();
    }
}
