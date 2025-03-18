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
    
    public void SetCardData(CardData _cardData)
    {
        m_cardData = _cardData;
        m_cardClassRender.sprite = AssetManager.intance.GetClass(_cardData.cardClass);
        m_cardNum.sprite = AssetManager.intance.GetNumber(_cardData.num);
    }
}
