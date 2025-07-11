using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ButtonTimer : MonoBehaviour
{
    //누른 버튼 1초 후 활성하기
    [SerializeField] private Button _button;
    [SerializeField] private float _time = 1f;
    private IEnumerator _coTimer;
    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(Timer);
    }

    private void Timer()
    {
        if(_coTimer != null)
        {
            StopCoroutine(_coTimer);
        }
        _coTimer = CoTimer();
        StartCoroutine(_coTimer);
    }

    IEnumerator CoTimer()
    {
        _button.interactable = false;
        yield return new WaitForSeconds(_time);
        _button.interactable = true;
    }

    private void OnDisable()
    {
        if (_coTimer != null)
        {
            StopCoroutine(_coTimer);
        }
    }
}
