using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardObject : MonoBehaviour
{
    public SpriteRenderer m_cardClassRender;
    public TMP_Text m_cardNum;
    public CardData m_cardData;
    
    public void SetCardData(CardData _cardData)
    {
        m_cardData = _cardData;

        Color spriteColor = Color.black;
        switch(_cardData.cardClass)
        {
            case CardClass.Spade:
                spriteColor = Color.green;
                break;
            case CardClass.Dia:
                spriteColor = Color.red;
                break;
            case CardClass.Heart:
                spriteColor = Color.yellow;
                break;
        }
        m_cardClassRender.color = spriteColor;
        m_cardNum.text = _cardData.num.ToString();
    }
}
