using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDragger : MonoBehaviour
{
    public CardObject m_dragObject;

    // Update is called once per frame
    void Update()
    {
        StartDrag();
        Dragging();
        EndDrag();
    }

    private void StartDrag()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Debug.Log("���콺 Ŭ�� ��");
        }
    }

    private void Dragging()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            Debug.Log("���콺 ������ ��");
        }
    }

    private void EndDrag()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Debug.Log("���콺 ��");
        }
    }
}
