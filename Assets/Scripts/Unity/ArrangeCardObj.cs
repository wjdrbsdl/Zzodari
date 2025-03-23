using System.Collections.Generic;
using UnityEngine;


public class ArrangeCardObj : MonoBehaviour
{
    public Transform m_centerTrans; //기준점 
    public float m_xGap = 2f;
    public List<CardObject> arrangeList = new();

    public void ResetList()
    {
        arrangeList.Clear();
    }

    public void SetCardObjects(CardObject[] _cardObjects)
    {
        for (int i = 0; i < _cardObjects.Length; i++)
        {
            arrangeList.Add(_cardObjects[i]);
        }
        ArrangePosition();
    }

    private void ArrangePosition()
    {
        //가운데 정렬 
        Vector3 arrangePos = m_centerTrans.position;
        int half = arrangeList.Count / 2;
        int rest = arrangeList.Count % 2;
        float startX = half * m_xGap * -1;
        if(rest == 0)
        {
            startX += m_xGap / 2;
        }
        arrangePos.x += startX;
        for (int i = 0; i < arrangeList.Count; i++)
        {
            if (arrangeList[i].gameObject.activeSelf == false)
            {
                continue;
            }
            arrangeList[i].SetArrangePos(arrangePos);
            arrangePos.x += m_xGap;
        }

    }

    public void AddCardObject(CardObject _object)
    {
        arrangeList.Add(_object);
        ArrangePosition();
    }

    public void RemoveCardObject(CardObject _object)
    {
        arrangeList.Remove(_object);
        ArrangePosition();
    }

    public List<CardData> GetCardDataList()
    {
        List<CardData> list = new();
        for (int i = 0; i < arrangeList.Count; i++)
        {
            list.Add(arrangeList[i].m_cardData);
        }
        return list;
    }
}
