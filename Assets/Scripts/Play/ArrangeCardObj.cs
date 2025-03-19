using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class ArrangeCardObj : MonoBehaviour
{
    public Transform m_centerTrans; //기준점 
    public float m_xGap = 2f;
    public void ArrangePosition(CardObject[] _cardObjectList)
    {
        //가운데 정렬 
        Vector3 arrangePos = m_centerTrans.position;
        int half = _cardObjectList.Length / 2;
        int rest = _cardObjectList.Length % 2;
        float startX = half * m_xGap * -1;
        if(rest == 0)
        {
            startX += m_xGap / 2;
        }
        arrangePos.x += startX;
        for (int i = 0; i < _cardObjectList.Length; i++)
        {
            _cardObjectList[i].SetArrangePos(arrangePos);
            arrangePos.x += m_xGap;
        }

    }

    void Update()
    {
        
    }
}
