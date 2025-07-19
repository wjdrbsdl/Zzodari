using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    private Button target;
    [SerializeField] private SFXType sfxSound;


    private void Awake()
    {
        target =  GetComponent<Button>();
    }

    private void Start()
    {
        target.onClick.AddListener(PlaySfx);
    }

    private void PlaySfx()
    {
        SoundManager.Instance.PlaySfx(sfxSound);
    }
}
