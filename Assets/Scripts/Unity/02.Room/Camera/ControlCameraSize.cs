using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlCameraSize : MonoBehaviour
{
    public const float m_defaultWidht = 2f; //기준 너비
    public const float m_defaultHeight = 1.5f; //높이 비율
    public const float m_defaultSize =5; //기준 사이즈

    public void Start()
    {
        CalSize();
    }

    

    private void CalSize()
    {
        float m_curWidht = Screen.width;
        float m_curHeight = Screen.height;

        //오쏘그래픽 size는 세로의 절반을 의미 즉 진짜 세로는 *2
        float baseHeight = m_defaultHeight * m_defaultSize * 2;
        float widhtHeightRatio = m_defaultWidht / m_defaultHeight; //가로, 세로 비율 구함
        float baseWidth = baseHeight * widhtHeightRatio; // 가로 세로 비율 곱해서 가로 너비 구함
        //baseWidht가 내가 적용하려는 너비. 

        //아래 비율이 현재 비율이고 
        //그 비율에 어떠한 size를 곱해서 20이 나와야함 20 = x * 비율/ x = 20 / 비율
        float curWidhtHeightRatio = (float)m_curWidht / m_curHeight;
        float m_curSize = (baseWidth / curWidhtHeightRatio) * 0.5f; //그리고 오쏘사이즈는 절반이기 때문에 /2 해줌

        Camera.main.orthographicSize = m_curSize;
    }

    private void Update()
    {
        CalSize();
    }
}
