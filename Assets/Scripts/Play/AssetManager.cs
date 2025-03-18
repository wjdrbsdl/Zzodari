using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetManager : MonoBehaviour
{
    public static AssetManager intance;

    public Sprite[] m_numbers;
    public Sprite[] m_cardClasses;

    void Awake()
    {
        intance = this;
    }

    public Sprite GetNumber(int _num)
    {
        return m_numbers[_num - 1];
    }

    public Sprite GetClass(CardClass _cardClass)
    {
        return m_cardClasses[(int)_cardClass];
    }
    
}
