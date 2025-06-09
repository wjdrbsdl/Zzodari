using GoogleMobileAds.Api;
using System;
using UnityEngine;

public class AdManager
{
    private RewardedAd rewardedAd;
    public Action OnShowAd;
    public void Initi()
    {
        MobileAds.Initialize(initStatus => {
            Debug.Log("AdMob Initialized");
            LoadRewardedAd();
        });

        ShowRewardedAd();
    }

    public void LoadRewardedAd()
    {
        string adUnitId = "ca-app-pub-3940256099942544/5224354917"; // 테스트 광고 ID

        AdRequest request = new AdRequest();

        RewardedAd.Load(adUnitId, request, (ad, error) =>
        {
            if (error != null)
            {
                Debug.LogError("광고 로드 실패: " + error.GetMessage());
                return;
            }

            rewardedAd = ad;
            Debug.Log("광고 로드 성공");
        });
    }

    public void ShowRewardedAd()
    {
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show((Reward reward) =>
            {
                Debug.Log($"보상 획득: {reward.Amount} {reward.Type}");
                // 서버에 보상 정보 전송 등 추가 로직 처리
                OnShowAd?.Invoke();
            });
        }
        else
        {
            Debug.Log("광고를 아직 사용할 수 없습니다.");
        }
    }
}
