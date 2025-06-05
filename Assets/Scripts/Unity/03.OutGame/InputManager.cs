using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    public InputField m_inputText;

    private void Awake()
    {
        instance = this;
    }

    public string GetRoomName()
    {
        return m_inputText.text;
    }

    public void SetInputTest(string _text)
    {
        m_inputText.text = _text;
    }
}
