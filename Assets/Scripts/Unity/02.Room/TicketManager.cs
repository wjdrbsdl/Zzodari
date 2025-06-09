using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


/// <summary>
/// 이용권 관리
/// 0이 되면 타이머 돌기 시작
/// 타이머 다 되면 x만큼 충전
/// </summary>
public class TicketManager : MonoBehaviour
{
    private float _chargeTime = 180f;
    private int _maxChance = 3;
    private int _curChance = 0;
    private int _chargeCount = 3;

#if CHEAT
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            ChargeTicket(_chargeCount);
        }
    }
#endif

    public bool HaveTicket()
    {
        return _curChance > 0;
    }

    public void UseTicket()
    {
        if (_curChance <= 0)
        {
            //문제있다.
            return;
        }

        _curChance -= 1;
        if(_curChance == 0)
        {
            StartCharge(); //남은티켓이 0이면 저절로 시작하기
        }
    }

    public void ChargeTicket(int amount)
    {
        _curChance += amount;
        _curChance = Math.Min(_maxChance, _curChance);
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
        chargeCorutine = CoChargeTicket();
        StartCoroutine(chargeCorutine);
    }

    private IEnumerator CoChargeTicket()
    {
        yield return new WaitForSeconds(_chargeTime);
        ChargeTicket(_chargeCount);
        chargeCorutine = null;
    }


}
