using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Apple.ReplayKit;


public enum BGMType
{
    BgmLobby, BgmRoom, BgmPlaying
}

public enum SFXType
{
    CardDraw, CradPut
}

public class SoundManager : SingleManager<SoundManager>
{
    public AudioSource bgmSorce;
    public AudioSource sfxSource; //ȿ���� ����� AudioSource

    public Dictionary<BGMType, AudioClip> bgmDic;
    public Dictionary<SFXType, AudioClip> sfxDic;

    private void Start()
    {
        SetAudioSource();
        SetAudioClip();
    }

    private void SetAudioSource()
    {
        if (bgmSorce == null)
        {
            bgmSorce = gameObject.AddComponent<AudioSource>();
        }
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void SetAudioClip()
    {
        bgmDic = new Dictionary<BGMType, AudioClip>();
        sfxDic = new();

        //���ҽ� ���Ͽ��� Ŭ�� �ܾ����
        AudioClip[] bgmClips = Resources.LoadAll<AudioClip>("Sound/BGM");
        AudioClip[] sfxClips = Resources.LoadAll<AudioClip>("Sound/SFX");

        SetDiction(bgmClips, bgmDic);
        SetDiction(sfxClips, sfxDic);
    }

    private void SetDiction<T>(AudioClip[] _clips, Dictionary<T, AudioClip> _dic) where T : Enum
    {
        for (int i = 0; i < _clips.Length; i++)
        {
            //Ŭ�� �ڿ��� ���鼭 �� �̸��� Ű������ clip �߰� 

            //�̳����� �Ľ�
            if (Enum.TryParse(typeof(T), _clips[i].name, out object result))
            {
                T bgmType = (T)result;
                _dic.Add(bgmType, _clips[i]);
            }
            else
            {
                Debug.LogWarning("BgmType�� " + _clips[i].name + "�� ���ǵǾ� ���� �ʴ�");
            }

        }
    }
}
