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
            Debug.Log("마우스 클릭 됨");
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // z축 값은 0으로 설정 (2D 환경에서는 보통 z축을 0으로 고정)
            mousePosition.z = 0;

            // 2D 레이를 쏩니다
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
            Debug.Log("마우스 누르는 중");
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0; //스크린상에서 z좌표를 0
            mousePosition.y -= 0.2f; //드래그 오브젝트 위치가 조금 아래로 내려가도록
            if (isDragging)
                m_dragObject.transform.position = mousePosition;

            // z축 값은 0으로 설정 (2D 환경에서는 보통 z축을 0으로 고정)
         

            // 2D 레이를 쏩니다
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
            Debug.Log("마우스 뗌");
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            isDragging = false;
            // z축 값은 0으로 설정 (2D 환경에서는 보통 z축을 0으로 고정)
            mousePosition.z = 0;

            // 2D 레이를 쏩니다
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, m_endMask);
            bool isSelectZone = false;
            if (hit.collider != null)
            {
               isSelectZone = true;
            }

            //드래그 중이던오브젝트가 있으면
            if(m_dragObject != null)
            {
                //매니저에게 전달해주고
                m_cardManager.EndDrag(m_dragObject, isSelectZone);
                //드래그 정보는 초기화
                m_dragObject.m_isDragging = false;
                m_dragObject = null;
            }
        }
    }
}
