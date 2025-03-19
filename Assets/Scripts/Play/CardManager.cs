using System;
using System.Collections.Generic;
using UnityEngine;


public class CardManager : MonoBehaviour
{
    public static CardManager intance;

    public ArrangeCardObj m_arrangeCard;
    public Transform cardHands;
    public CardObject cardSample;
    public CardObject[] cards;

    void Awake()
    {
        intance = this;
    }
    private void Start()
    {
        MakeCardObject();
    }

    public Action callBack;

    private void Update()
    {
        if(callBack!= null)
        {
            callBack();
            callBack = null;
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

    public void SetHaveCard(List<CardData> _haveCardList)
    {
        //있는 만큼 켜고 세팅
        for (int i = 0; i < _haveCardList.Count; i++)
        {
            cards[i].SetCardData(_haveCardList[i]);
            cards[i].gameObject.SetActive(true);
        }
        //나머진 끔
        for (int i = _haveCardList.Count; i < 13; i++)
        {
            cards[i].gameObject.SetActive(false);
        }

        CardObject[] activeCards = cardHands.GetComponentsInChildren<CardObject>();
        m_arrangeCard.ArrangePosition(activeCards);
    }
}
