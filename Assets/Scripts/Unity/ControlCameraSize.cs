using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlCameraSize : MonoBehaviour
{
    public const float m_defaultWidht = 2f; //���� �ʺ�
    public const float m_defaultHeight = 1.5f; //���� ����
    public const float m_defaultSize =5; //���� ������

    public void Start()
    {
        CalSize();
    }

    

    private void CalSize()
    {
        float m_curWidht = Screen.width;
        float m_curHeight = Screen.height;

        //����׷��� size�� ������ ������ �ǹ� �� ��¥ ���δ� *2
        float baseHeight = m_defaultHeight * m_defaultSize * 2;
        float widhtHeightRatio = m_defaultWidht / m_defaultHeight; //����, ���� ���� ����
        float baseWidth = baseHeight * widhtHeightRatio; // ���� ���� ���� ���ؼ� ���� �ʺ� ����
        //baseWidht�� ���� �����Ϸ��� �ʺ�. 

        //�Ʒ� ������ ���� �����̰� 
        //�� ������ ��� size�� ���ؼ� 20�� ���;��� 20 = x * ����/ x = 20 / ����
        float curWidhtHeightRatio = (float)m_curWidht / m_curHeight;
        float m_curSize = (baseWidth / curWidhtHeightRatio) * 0.5f; //�׸��� ���������� �����̱� ������ /2 ����

        Camera.main.orthographicSize = m_curSize;
    }

    private void Update()
    {
        CalSize();
    }
}
