using System;
using System.Collections;

using UnityEngine;


/// <summary>
/// 이용권 관리
/// 0이 되면 타이머 돌기 시작
/// 타이머 다 되면 x만큼 충전
/// </summary>
public class TicketManager : MonoBehaviour
{
    public static TicketManager Instance;
    private float _chargeTime = 180f;
    public int MaxChance = 3;
    public int CurChance = 0;
    public int ChargeCount = 3;
    public float RestTime = 0f;

    public Action<float> OnChangeRestTime;

    public Action<int> OnChangeTicketAmount;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }

        if(HaveTicket() == false)
        {
            StartCharge();
        }
    }


    public void Update()
    {
#if CHEAT
        if (Input.GetKeyDown(KeyCode.F5))
        {
            ChargeTicket(ChargeCount);
        }
#endif
        if(isUseResrvate == true)
        {
            UseTicket();
            isUseResrvate = false;
        }
    }


    public bool HaveTicket()
    {
        return CurChance > 0;
    }


    bool isUseResrvate = false;
    public void ResUseTicket()
    {
        //서버로부터 유즈 티켓을 받은경우
        isUseResrvate = true;
    }

    public void UseTicket()
    {
        if (CurChance <= 0)
        {
            //문제있다.
            return;
        }

        CurChance -= 1;
        OnChangeTicketAmount?.Invoke(CurChance);
        if(CurChance == 0)
        {
            StartCharge(); //남은티켓이 0이면 저절로 시작하기
        }
    }

    public void ChargeTicket(int amount)
    {
        CurChance += amount;
        CurChance = Math.Min(MaxChance, CurChance);
        OnChangeTicketAmount?.Invoke(CurChance);
        StopCharge(); //충전 했으면 기존 충전중이던건 취소 
    }


    private void StopCharge()
    {
        if(chargeCorutine != null)
        {
            StopCoroutine(chargeCorutine);
            chargeCorutine = null;
        }
        
    }
    IEnumerator chargeCorutine;
    private void StartCharge()
    {
        if(chargeCorutine != null)
        {
            //이미 진행중인 충전이 있으면 안함
            return;
        }
        RestTime = _chargeTime;
        chargeCorutine = CoChargeTicket();
        StartCoroutine(chargeCorutine);
    }

    private IEnumerator CoChargeTicket()
    {
        float alarmTime = 1f; //1초마다 변화 알리기
        while (RestTime > 0)
        {
            RestTime -= Time.deltaTime;
            alarmTime -= Time.deltaTime;
            if (alarmTime <= 0)
            {
                OnChangeRestTime?.Invoke(RestTime);
            }
            yield return null;
        }
        ChargeTicket(ChargeCount);
        chargeCorutine = null;
    }


}
