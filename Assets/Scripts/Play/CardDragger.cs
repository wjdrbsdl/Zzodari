using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardDragger : MonoBehaviour
{
    public CardManager m_cardManager;
    public CardObject m_dragObject;
    public bool isDragging = false;
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
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // z�� ���� 0���� ���� (2D ȯ�濡���� ���� z���� 0���� ����)
            mousePosition.z = 0;

            // 2D ���̸� ���ϴ�
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            if (hit.collider != null)
            {
                if(hit.collider.name == "Test")
                {
                    isDragging = true;
                    m_dragObject = hit.collider.GetComponent<CardObject>();
                    m_dragObject.m_isDragging = true;
                }
                Debug.Log(hit.collider.name);
            }
        }
    }

    private void Dragging()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            Debug.Log("���콺 ������ ��");
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0; //��ũ���󿡼� z��ǥ�� 0
            mousePosition.y -= 0.2f; //�巡�� ������Ʈ ��ġ�� ���� �Ʒ��� ����������
            if (isDragging)
                m_dragObject.transform.position = mousePosition;

            // z�� ���� 0���� ���� (2D ȯ�濡���� ���� z���� 0���� ����)
         

            // 2D ���̸� ���ϴ�
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            if (hit.collider != null)
            {
                Debug.Log(hit.collider.name);
            }
        }
    }

    public LayerMask m_endMask;
    private void EndDrag()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Debug.Log("���콺 ��");
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            isDragging = false;
            // z�� ���� 0���� ���� (2D ȯ�濡���� ���� z���� 0���� ����)
            mousePosition.z = 0;

            // 2D ���̸� ���ϴ�
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, m_endMask);
            bool isSelectZone = false;
            if (hit.collider != null)
            {
               isSelectZone = true;
            }

            //�巡�� ���̴�������Ʈ�� ������
            if(m_dragObject != null)
            {
                //�Ŵ������� �������ְ�
                m_cardManager.EndDrag(m_dragObject, isSelectZone);
                //�巡�� ������ �ʱ�ȭ
                m_dragObject.m_isDragging = false;
                m_dragObject = null;
            }
        }
    }
}
