using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoOffTimer : MonoBehaviour
{
    public float m_baseTime = 2f;
    private float m_curtime;

    public void StartTimer()
    {
        m_curtime = m_baseTime;
        gameObject.SetActive(true);
    }

    public void Update()
    {
        m_curtime -= Time.deltaTime;
        if(m_curtime < 0)
        {
            gameObject.SetActive(false);
        }
    }
}
