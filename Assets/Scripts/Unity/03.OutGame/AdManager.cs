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
        string adUnitId = "ca-app-pub-3940256099942544/5224354917"; // �׽�Ʈ ���� ID

        AdRequest request = new AdRequest();

        RewardedAd.Load(adUnitId, request, (ad, error) =>
        {
            if (error != null)
            {
                Debug.LogError("���� �ε� ����: " + error.GetMessage());
                return;
            }

            rewardedAd = ad;
            Debug.Log("���� �ε� ����");
        });
    }

    public void ShowRewardedAd()
    {
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show((Reward reward) =>
            {
                Debug.Log($"���� ȹ��: {reward.Amount} {reward.Type}");
                // ������ ���� ���� ���� �� �߰� ���� ó��
                OnShowAd?.Invoke();
            });
        }
        else
        {
            Debug.Log("���� ���� ����� �� �����ϴ�.");
        }
    }
}
