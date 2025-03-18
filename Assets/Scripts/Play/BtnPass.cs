using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtnPass : MonoBehaviour
{
    public PlayClient m_pClient;

    public void OnClickPassBtn()
    {
        //내가 선택중이던것과 별개로 빠르게 패스를 누를 수있고
        m_pClient.PutDownPass(); //불 반환을 통해 선택중이던걸 초기화하거나 가능. 
    }
}
