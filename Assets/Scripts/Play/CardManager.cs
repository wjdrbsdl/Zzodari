using System;
using System.Collections.Generic;
using UnityEngine;


public class CardManager : MonoBehaviour
{
    public static CardManager intance;

    public Transform cardHands;
    public CardObject cardSample;

    void Awake()
    {
        intance = this;
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

    public void MakeCard(List<CardData> _haveCardList)
    {
        CardObject[] cardObjects = cardHands.GetComponentsInChildren<CardObject>();
        for (int i = 0; i < _haveCardList.Count - cardObjects.Length; i++)
        {
            CardObject newObj = Instantiate(cardSample);
            newObj.transform.SetParent(cardHands);
        }
        cardObjects = cardHands.GetComponentsInChildren<CardObject>();
        for (int i = 0; i < _haveCardList.Count; i++)
        {
            cardObjects[i].SetCardData(_haveCardList[i]);
        }
        
    }
}
