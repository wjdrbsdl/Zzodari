using System;
using UnityEngine;
using GoogleMobileAds.Api;


public class AdManager : MonoBehaviour
{
    private RewardedAd rewardedAd;

    // �װ� ���� static �̺�Ʈ ����
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
        string adUnitId = "ca-app-pub-3940256099942544/5224354917"; // �׽�Ʈ ���� ID

        AdRequest request = new AdRequest();

        RewardedAd.Load(adUnitId, request, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null)
            {
               // OnDebug?.Invoke($"���� �ε� ����: {error.GetMessage()}");
                return;
            }

            rewardedAd = ad;
          //  OnDebug?.Invoke("���� �ε� ����");

            // �̺�Ʈ �ڵ鷯 ���
            rewardedAd.OnAdFullScreenContentOpened += HandleAdOpened;
            rewardedAd.OnAdFullScreenContentClosed += HandleAdClosed;
            rewardedAd.OnAdFullScreenContentFailed += HandleAdFailedToShow;
            rewardedAd.OnAdPaid += HandleAdPaid;
        });
    }

    bool isShow = false;
    public void ShowRewardedAd()
    {
        Debug.Log("������");
        if (rewardedAd != null)
        {
            isShowAd = true;
            rewardedAd.Show((Reward reward) =>
            {
                //  OnDebug?.Invoke($"���� ȹ��: {reward.Amount} {reward.Type}");
                isShow = true;
            });
        }
        else
        {
          //  OnDebug?.Invoke("���� ���� �ε���� �ʾҽ��ϴ�.");
        }
    }

    private void Update()
    {
        if (isShow)
        {
            //���� ���� - �ٸ� ������, UI ������ �ʿ��� ���� �����带 �ǵ帮�� �Ǹ� ��
            OnAdRewardEarned?.Invoke();
            isShow = false;
        }
    }

    // �̺�Ʈ �ڵ鷯

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