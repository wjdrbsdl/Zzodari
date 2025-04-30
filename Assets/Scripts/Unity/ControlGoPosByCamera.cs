using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ControlGoPosByCamera : MonoBehaviour
{

    public float m_defaultX; //초기 x위치
    public float m_defaultY; //초기 y위치 
    public Vector3 m_originPos;
    private void Start()
    {
        m_defaultX = gameObject.transform.position.x;
        m_defaultY = gameObject.transform.position.y;
        m_originPos = gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        CalPos();
    }

    private void CalPos()
    {
        float reviseSize = Camera.main.orthographicSize;
        float defaultSize = ControlCameraSize.m_defaultSize;
        float reviseRatio = reviseSize / defaultSize;
        float revisePosY = m_defaultY * reviseRatio;
        gameObject.transform.position = new Vector3(m_defaultX, revisePosY, 0);
    }

}
