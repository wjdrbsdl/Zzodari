using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class PlayManager
{
    public List<CardData> haveCardList; //내가 들고 있는 카드
    public List<CardData> giveCardList; //전에 내가 냈던 카드
    public List<CardData> putDownList; //바닥에 깔린 카드
    public bool isMyTurn = false;
    public bool isGameStart = false;
    public int gameTurn = 0; //카드 제출이 진행된 턴 1번부터
    public InGameData inGameData;

    #region 로직 파트

    private void SetNewGame()
    {
        //보유 카드는 통신응답에서 진행
        giveCardList.Clear();
        putDownList.Clear();
        gameTurn = 0;
        SetMyTurn(false);

        //스테이지 종료후 다시 시작하는 경우일땐 게임은 진행중임. 
        if (isGameStart == false)
        {
            isGameStart = true;
            inGameData.ResetBadPoint();
        }

    }

    private void ResetStage()
    {
        ColorConsole.Default("스테이지 리셋");
        giveCardList.Clear();
        putDownList.Clear();
        haveCardList.Clear();
        gameTurn = 0;
        SetMyTurn(false);
        수정필요();
        //SetMyCardList(); //유니티 -ResetStage에서 보유카드 클리어하고, 그상태로 UI 갱신
    }

    private void SetGameOver()
    {
        isGameStart = false;
    }

    #region 카드 내기

    public bool PutDownCards(List<CardData> _selectCards)
    {
        if (isGameStart == false)
        {
            return false;
        }

        if (isMyTurn == false)
        {
            ColorConsole.RuleWarning("자기 차례가 아닙니다.");
            return false;
        }
        //낼 수 있는 카드 인지 체크
        if (CheckSelectCard(_selectCards))
        {
            //낼 수 있으면 제출
            SetMyTurn(false); //내턴 넘김으로 수정
            수정필요();
            //ReqSelectCard(new List<CardData>()); //최종 제출시엔 선택칸은 비어있음.
            //ReqPutDownCard(_selectCards);
            return true;
        }
        return false;

    }

    public bool PutDownPass()
    {
        //빈거 넘김

        return PutDownCards(new List<CardData>()); //패스버튼
    }

    private bool CheckSelectCard(List<CardData> _selectCards)
    {
        CardRule cardRule = new CardRule();
        TMixture selectCardValue = new TMixture();
        if (cardRule.IsVarid(_selectCards, out selectCardValue) == false)
        {
            ColorConsole.RuleWarning("유효한 조합이 아닙니다.");
            return false;
        }

        //혼자 크기 비교 위해서 아래비교, 내가 제출한건 무조건 전걸로 진행 

        //선택된 카드를 현재 낼 수 있는지 판단해서 bool 반환
        if (gameTurn == 1)
        {
            //첫번째 턴이면 보유한 카드에 스페이드 3 있어야 가능 한걸로 
            foreach (CardData card in _selectCards)
            {
                if (card.Compare(CardData.minClass, CardData.minRealValue) == 0)
                {
                    return true;
                }
            }
            ColorConsole.RuleWarning($"첫 시작은 {CardData.minClass}{CardData.minRealValue}을 포함해야 합니다.");
            return false;
        }

        //처음이 아니면 내가 낸건지 체크 - 내가 낸거면 자유롭게 내기 가능
        if (CheckAllPass())
        {
            if (_selectCards.Count == 0)
            {
                ColorConsole.RuleWarning("올 패스 받았습니다. 자유롭게 낼 수 있되 패스는 불가 합니다.");
                return false;
            }
            return true;
        }


        if (selectCardValue.mixture == EMixtureType.Pass)
        {
            //패스한거면 그냥 통과
            return true;
        }

        //이전것과 비교
        TMixture putDownValue = new TMixture();
        cardRule.CheckValidRule(putDownList, out putDownValue);

        ColorConsole.Default($"이전꺼 {putDownValue.mixture}:{putDownValue.mainCardClass}:{putDownValue.mainRealValue}" +
            $"\n제출용 {selectCardValue.mixture}:{selectCardValue.mainCardClass}:{selectCardValue.mainRealValue}:");
        //비교 안되는 타입이면 (앞에 낸것과 다른 유형이면) 실패
        if (cardRule.TryCompare(putDownValue, selectCardValue, out int compareValue) == false)
        {
            ColorConsole.RuleWarning("다른 유형의 조합입니다.");
            return false;
        }
        //compareValue는 이전꺼에서 현재껄 뺀거 - 즉 양수면 전께 큰거 
        if (compareValue > 0)
        {
            //이전것보다 작아도 실패
            ColorConsole.RuleWarning("전 보다 작습니다.");
            return false;
        }

        ColorConsole.Default("전 보다 크다");
        return true;
    }

    private bool CheckAllPass()
    {
        inGameData.SetAllPass(false);
        if (putDownList.Count == 0)
        {
            return false;
        }

        ColorConsole.Default("올 패스인지 체크");
        giveCardList.Sort();
        putDownList.Sort();

        //정렬해서 냈던 카드가 있으면 올 패스 된거.
        if (giveCardList.Count >= 1)
        {
            if (giveCardList[0].CompareTo(putDownList[0]) == 0)
            {
                //하나라도 내가 냈던거랑 같으면 내가 냈던거
                ColorConsole.Default("올 패스 받았음");
                inGameData.SetAllPass(true);
                return true;
            }
            return false;
        }

        return false;

    }

    private void SetMyTurn(bool _turn)
    {
        isMyTurn = _turn;
    }
    #endregion 

    #region 카드 리스트 관리
    private void ResetGiveCard()
    {
        //내가 냈던 카드 초기화
        giveCardList.Clear();
    }

    private void RecordGiveCard(List<CardData> _cardList)
    {
        for (int i = 0; i < _cardList.Count; i++)
        {
            giveCardList.Add(_cardList[i]);
        }
    }

    private void ResetPutDownCard()
    {
        putDownList.Clear();
    }

    private void AddPutDownCard(CardData _card)
    {
        putDownList.Add(_card);
    }

    private void RemoveHaveCard(List<CardData> _removeList)
    {
        //보유 카드에서 내려놓은 카드 제거
        for (int i = 0; i < _removeList.Count; i++)
        {
            CardData target = _removeList[i];
            for (int j = 0; j < haveCardList.Count; j++)
            {
                if (haveCardList[j].CompareTo(target) == 0)
                {
                    haveCardList.RemoveAt(j);
                    break;
                }
            }
        }
    }

    public void SortCardList()
    {
        haveCardList.Sort();

        Action action = () =>
        {
            if (isMyTurn)
            {
                CardManager.instance.ResetSelectCards(); // 내 차례라면 냈던 카드도 회수
            }
            CardManager.instance.ResetHandCards(); //내 카드 정렬할때 
        };
        CardManager.instance.callBack.Enqueue(action); //인큐

        if (isMyTurn)
        {
            수정필요();
           //ReqSelectCard(new List<CardData>()); //정렬로 냈던 카드 다 회수한걸로 전달
        }

    }

    public List<CardData> GetHaveCardList()
    {
        return haveCardList;
    }
    #endregion

    public void ExceedTimer()
    {
        //시간 초과 
        if (PutDownPass() == false)
        {
            //만약 패스 불가라면 -> 올패스에서 자기차례
            //제일 작은 카드 1장 내기
            List<CardData> putList = new();
            SortCardList(); //첫턴에 시간초과시 가장 낮은 카드 제출 위해서
            putList.Add(GetHaveCardList()[0]);
            PutDownCards(putList);
        }
    }

    private void CountTurn()
    {
        gameTurn++;
    }
    #endregion


    private void 수정필요()
    {

    }
}

