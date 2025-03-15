using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugManager : MonoBehaviour
{
    public static DebugManager instance;
    public TMP_Text tmpText;
    void Start()
    {
        instance = this;
    }

    Queue<string> messegeQueue = new();
    public void EnqueMessege(string msg)
    {
        messegeQueue.Enqueue(msg);
    }

    // Update is called once per frame
    void Update()
    {
        if(messegeQueue.TryDequeue(out string msg))
        {
            tmpText.text += (msg + "\n");
        }
    }
}
