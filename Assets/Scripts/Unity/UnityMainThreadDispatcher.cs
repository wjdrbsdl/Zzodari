using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static readonly Queue<Action> executionQueue = new Queue<Action>();

    // ���� �����忡�� ������ �۾��� ť�� �ֽ��ϴ�.
    public static void Enqueue(Action action)
    {
        lock (executionQueue)
        {
            executionQueue.Enqueue(action);
        }
    }

    // �� �����Ӹ��� ť�� �ִ� �۾��� �����մϴ�.
    void Update()
    {
        lock (executionQueue)
        {
            while (executionQueue.Count > 0)
            {
                executionQueue.Dequeue().Invoke();
            }
        }
    }
}
