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
    public TMP_Text warningText;
    public Scrollbar vertiaclScroll;
    public GameObject debugScroll;
    public bool systemDebug = false;
    public bool msgDebug = false;
    void Start()
    {
        instance = this;
        tmpText.text = "";

        //디버그창 활성화
        if (preText != null)
        {
            preText.gameObject.SetActive(systemDebug);
        }
        if (curText != null)
        {
            curText.gameObject.SetActive(systemDebug);
        }
        if (debugScroll != null)
        {
            debugScroll.gameObject.SetActive(msgDebug);
        }
        if (warningText != null)
        {
            warningText.gameObject.SetActive(false);
        }
    }

    Queue<string> ruleWarningQueue = new();
    public void EnqueRuleMessege(string msg)
    {
        ruleWarningQueue.Enqueue(msg);
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
    float warningTime = 1.5f;
    float warningRemainTime = 0f;
    bool warningOn = false;
    void Update()
    {
        if (messegeQueue.TryDequeue(out string msg) && msgDebug)
        {
            tmpText.text += (msg + "\n");
            Rect rect = tmpText.rectTransform.rect;
            tmpText.rectTransform.sizeDelta = new Vector2(tmpText.rectTransform.sizeDelta.x, rect.height + 36f);
            vertiaclScroll.value = 0;
        }
        if (systemQueue.TryDequeue(out string systeMsg) && systemDebug)
        {
            preText.text = curText.text;
            curText.text = (systeMsg);
        }
        if (ruleWarningQueue.TryDequeue(out string warningMsg))
        {
            warningText.text = warningMsg;
            warningRemainTime = warningTime;
            warningOn = true;
            warningText.gameObject.SetActive(true);
        }
        WarningOnOff();
    }

    private void WarningOnOff()
    {
        if (warningOn == false)
            return;

        warningRemainTime -= Time.deltaTime;
        if (warningRemainTime <= 0)
        {
            warningText.gameObject.SetActive(false);
            warningOn = false;
        }
    }

    public void OnOff()
    {
        msgDebug = !debugScroll.activeSelf;
        debugScroll.SetActive(!debugScroll.activeSelf);
    }
}
