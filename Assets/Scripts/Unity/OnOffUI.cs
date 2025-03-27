using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnOffUI : MonoBehaviour
{
    public GameObject m_window;

    public void OnClickOn()
    {
        m_window.SetActive(true);
    }

    public void Off()
    {
        m_window.SetActive(false);
    }
}
