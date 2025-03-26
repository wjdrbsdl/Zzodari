using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DebugManager : MonoBehaviour
{
    public static DebugManager instance;
    public TMP_Text tmpText;
    public TMP_Text preText;
    public TMP_Text curText;
    public Scrollbar vertiaclScroll;
    public GameObject debugScroll;
    void Start()
    {
        instance = this;
        tmpText.text = "";
    }

    Queue<string> messegeQueue = new();
    public void EnqueMessege(string msg)
    {
        messegeQueue.Enqueue(msg);
    }

    Queue<string> systemQueue = new();
    public void EnqueSystemMsg(string msg)
    {
        systemQueue.Enqueue(msg);
    }
  
    // Update is called once per frame
    void Update()
    {
        if(messegeQueue.TryDequeue(out string msg))
        {
            tmpText.text += (msg + "\n");
            Rect rect = tmpText.rectTransform.rect;
            tmpText.rectTransform.sizeDelta = new Vector2(tmpText.rectTransform.sizeDelta.x, rect.height + 36f);
            vertiaclScroll.value = 0;
        }
        if (systemQueue.TryDequeue(out string systeMsg))
        {
            preText.text = curText.text;
            curText.text = (systeMsg);
        }
    }

    public void OnOff()
    {
        debugScroll.SetActive(!debugScroll.activeSelf);
    }
}
