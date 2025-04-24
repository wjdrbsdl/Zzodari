using System;
using System.Collections.Generic;
using UnityEngine;


public class CardManager : MonoBehaviour
{
    public static CardManager instance;

    public CardDragger m_dragger;
    public PlayClient m_pClient;
    public ArrangeCardObj m_arrangeHandCard;
    public ArrangeCardObj m_arrangeSelectCard;
    public Transform cardHands;
    public CardObject cardSample;
    public CardObject[] handCards;
    public CardObject[] otherCards; //셀렉 표기용 카드
    public Transform otehrCardTransform;

    public GameObject m_selectZoneObj;
    public GameObject m_guideText; // 드래그해서 끌어라는 가이드 문구
    public List<CardData> m_haveCardList = new List<CardData>();

    #region
    void Awake()
    {
        instance = this;
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
        handCards = new CardObject[13];
        otherCards = new CardObject[13];
        //카드 최대 수치는 13장으로 정해져있음. 고로 만들어놓기
        for (int i = 0; i < 13; i++)
        {
            CardObject newObj = Instantiate(cardSample);
            newObj.transform.SetParent(cardHands);
            newObj.gameObject.SetActive(false);
            newObj.m_isMine = true;
            handCards[i] = newObj;

            CardObject otherObj = Instantiate(cardSample);
            otherObj.transform.SetParent(otehrCardTransform);
            otherObj.gameObject.SetActive(false);
            otherObj.m_isMine = false;
            otherCards[i] = otherObj;
        }
        
    }

    public void ShowOtherCard(string _id, List<CardData> _selectList)
    {
        //Debug.Log("카드 셀렉했다고 정보 들어옴");
        m_dragger.ForceEndDrag(); //들고있던 남의 카드 있으면 강제 드랍 
        m_arrangeSelectCard.ResetList();
        for (int i = 0; i < _selectList.Count; i++)
        {
            otherCards[i].SetCardData(_selectList[i]);
            m_arrangeSelectCard.AddCardObject(otherCards[i]);
        }
    
    }

    //처음 가졌을때?
    public void SetHaveCard(List<CardData> _haveCardList)
    {
        m_haveCardList = _haveCardList;
        //있는 만큼 켜고 세팅
        ResetSelectCards();
        ResetHandCards();
    }

    public void ResetSelectCards()
    {
        m_arrangeSelectCard.ResetList();
    }

    public void ResetHandCards()
    {
        
        m_arrangeHandCard.ResetList();
        for (int i = 0; i < m_haveCardList.Count; i++)
        {
            handCards[i].SetCardData(m_haveCardList[i]);
            m_arrangeHandCard.AddCardObject(handCards[i]);
        }
        ShowGuidText();
    }

    #region 드래그 반응
    public void ForceQuitDrag()
    {
        //강제로 드래그 종료시키기
        m_dragger.ForceEndDrag();
    }

    public void EndDrag(CardObject _object, bool _isSelectZone)
    {
        //Debug.Log("카드를 선택존에 놓았는가" + _isSelectZone);
        //카드 드래그를 끝냈을때, 끝낸 위치가 셀렉존인가. 
        if(_object.m_isCurSelect == _isSelectZone)
        {
            //있던 위치에 놓은 애는 별 조작 없음
            return;
        }

        //내차례가 아니면 못함
        if (m_pClient.isMyTurn == false)
        {
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
        m_pClient.ReqSelectCard(m_arrangeSelectCard.GetCardDataList());
        ShowGuidText();
       
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

    #region 버튼
    public void OnClickPutDown()
    {
       bool put = m_pClient.PutDownCards(m_arrangeSelectCard.GetCardDataList());
    }

    public void OnClickSort()
    {
        m_pClient.SortCardList(); //손패 정렬하기 버튼 누른경우
    }
    #endregion

    private void ShowGuidText()
    {
        if(m_pClient.isGameStart == false)
        {
            //게임 시작전이면 안냇말은 띄어두기
            m_guideText.SetActive(true);
            return;
        }

        if(m_pClient.isMyTurn == false)
        {
            //일단 내턴 아니면 끔 
            m_guideText.SetActive(false);
            return;
        }

        bool noSelcted = (m_arrangeSelectCard.GetCardDataList().Count == 0);
        m_guideText.SetActive(noSelcted);
    }
}
