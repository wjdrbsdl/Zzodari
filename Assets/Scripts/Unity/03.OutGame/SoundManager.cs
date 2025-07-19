using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BGMType
{
    BgmLobby, BgmRoom, BgmPlaying
}

public enum SFXType
{
   SfxButton, SfxButtonHigh, SfxButtonLow, SfxClose, SfxLose, SfxWin, CardDraw, CradPut
}

public class SoundManager : Singleton<SoundManager>
{
    public AudioSource bgmSorce;
    public AudioSource sfxSource; //효과음 재생용 AudioSource

    public Dictionary<BGMType, AudioClip> bgmDic;
    public Dictionary<SFXType, AudioClip> sfxDic;

    public void PlayBgm(BGMType bgm)
    {
        if(bgmDic.ContainsKey(bgm) == true)
        {
            bgmSorce.clip = bgmDic[bgm];
            bgmSorce.Play();
        }
    }

    public void PlaySfx(SFXType sfx)
    {
        if(sfxDic.ContainsKey(sfx) == true)
        {
            sfxSource.PlayOneShot(sfxDic[sfx]);
        }
    }

    protected override void NewMake()
    {
        base.NewMake();
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

        //리소스 파일에서 클립 긁어오기
        AudioClip[] bgmClips = Resources.LoadAll<AudioClip>("Sound/BGM");
        AudioClip[] sfxClips = Resources.LoadAll<AudioClip>("Sound/SFX");

        SetDiction(bgmClips, bgmDic);
        SetDiction(sfxClips, sfxDic);
    }

    private void SetDiction<T>(AudioClip[] _clips, Dictionary<T, AudioClip> _dic) where T : Enum
    {
        for (int i = 0; i < _clips.Length; i++)
        {
            //클립 자원을 돌면서 그 이름을 키값으로 clip 추가 

            //이넘으로 파싱
            if (Enum.TryParse(typeof(T), _clips[i].name, out object result))
            {
                T bgmType = (T)result;
                _dic.Add(bgmType, _clips[i]);
            }
            else
            {
                Debug.LogWarning("BgmType에 " + _clips[i].name + "가 정의되어 있지 않다");
            }

        }
    }
}
