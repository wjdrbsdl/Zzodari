using System.Collections;
using UnityEngine;


public class TurnTimeCounter : MonoBehaviour
{
    public float turnTime = 20f;
    public float restTime = 0f;
    public bool isMyTurn = false;
    // Use this for initialization
    public void CountMyTurn()
    {
        isMyTurn = true;
        restTime = turnTime;
    }

    private void DoneMyTurn()
    {
        isMyTurn = false;
        //턴 종료되었음을 전달 

    }

    // Update is called once per frame
    void Update()
    {
        if(isMyTurn == false)
        {
            return;
        }

        restTime -= Time.deltaTime;
        if (restTime <= 0)
        {
            DoneMyTurn();
        }
    }
}
