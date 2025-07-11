using System;
using UnityEngine;
using GoogleMobileAds.Api;


public class AdManager : MonoBehaviour
{
    private RewardedAd rewardedAd;

    // 네가 쓰던 static 이벤트 유지
    public static event Action OnAdRewardEarned;
    public static bool isShowAd = false;

    void Start()
    {
        MobileAds.Initialize(initStatus =>
        {
            LoadRewardedAd();
        });
    }

    private void LoadRewardedAd()
    {
        string adUnitId = "ca-app-pub-3940256099942544/5224354917"; // 테스트 광고 ID

        AdRequest request = new AdRequest();

        RewardedAd.Load(adUnitId, request, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null)
            {
               // OnDebug?.Invoke($"광고 로드 실패: {error.GetMessage()}");
                return;
            }

            rewardedAd = ad;
          //  OnDebug?.Invoke("광고 로드 성공");

            // 이벤트 핸들러 등록
            rewardedAd.OnAdFullScreenContentOpened += HandleAdOpened;
            rewardedAd.OnAdFullScreenContentClosed += HandleAdClosed;
            rewardedAd.OnAdFullScreenContentFailed += HandleAdFailedToShow;
            rewardedAd.OnAdPaid += HandleAdPaid;
        });
    }

    bool isShow = false;
    public void ShowRewardedAd()
    {
        Debug.Log("보여줘");
        if (rewardedAd != null)
        {
            isShowAd = true;
            rewardedAd.Show((Reward reward) =>
            {
                //  OnDebug?.Invoke($"보상 획득: {reward.Amount} {reward.Type}");
                isShow = true;
            });
        }
        else
        {
          //  OnDebug?.Invoke("광고가 아직 로드되지 않았습니다.");
        }
    }

    private void Update()
    {
        if (isShow)
        {
            //광고 집행 - 다른 쓰레드, UI 갱신이 필요한 메인 쓰레드를 건드리게 되면 펑
            OnAdRewardEarned?.Invoke();
            isShow = false;
        }
    }

    // 이벤트 핸들러

    private void HandleAdOpened()
    {
     
    }

    private void HandleAdClosed()
    {
        isShowAd = false;
        LoadRewardedAd();
    }

    private void HandleAdFailedToShow(AdError error)
    {
        isShowAd = false;
    }

    private void HandleAdPaid(AdValue adValue)
    {
    }
}