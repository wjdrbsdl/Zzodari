using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpManager : Singleton<PopUpManager>
{

    [SerializeField] private PopMessege _messegePrefab;

    private Queue<string> messegeQueue = new();

    private void Start()
    {
      //  NetworkManager.OnDetectDisConnect += SendPopMessege;
    }

    private void Update()
    {
        if (messegeQueue.TryDequeue(out string result))
        {
            PopMessege(result);
        }
    }

    public void SendPopMessege(string messegeStr)
    {
        messegeQueue.Enqueue(messegeStr);
    }

    private void PopMessege(string messegeStr)
    {
        PopMessege messege = Instantiate(_messegePrefab, transform);

        // RectTransform ���� ��������
        RectTransform canvasRect = GetComponent<Canvas>().GetComponent<RectTransform>();
        RectTransform messageRect = messege.GetComponent<RectTransform>();

        // �ؽ�Ʈ ���� ����
        messege.MessegeText.text = messegeStr;
        LayoutRebuilder.ForceRebuildLayoutImmediate(messageRect);

        float canvasWidth = canvasRect.rect.width;
        float canvasHeight = canvasRect.rect.height;
        float msgWidth = messageRect.rect.width;
        float msgHeight = messageRect.rect.height;

        // �߾� ���� �ȿ��� ���� ��ġ
        float minX = -((canvasWidth - msgWidth) / 2f);
        float maxX = ((canvasWidth - msgWidth) / 2f);
        float minY = -((canvasHeight - msgHeight) / 2f);
        float maxY = ((canvasHeight - msgHeight) / 2f);

        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);
        Vector3 randomPos = new Vector3(randomX, randomY, 0f);

        // �޽��� ��ġ ����
        messege.SetInfo(messegeStr, randomPos);
    }
}
