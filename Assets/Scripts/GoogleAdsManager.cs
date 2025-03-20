using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class GoogleAdsManager : MonoBehaviour
{
    public static GoogleAdsManager Instance;

    [SerializeField] private AdConfig adConfig; // Reference to the ScriptableObject

    private BannerView bannerAd;
    private InterstitialAd interstitialAd;
    private RewardedAd rewardedAd;

    private Action onInterstitialAdClosed;
    private Action onRewardedAdCompleted;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        MobileAds.Initialize(initStatus => { Debug.Log("Google Ads Initialized"); });
    }

    #region Banner Ad
    public void RequestBannerAd()
    {
        bannerAd = new BannerView(adConfig.bannerAdId, AdSize.Banner, AdPosition.Bottom);
        bannerAd.LoadAd(new AdRequest());
    }

    public void ShowBannerAd() => bannerAd?.Show();
    public void HideBannerAd() => bannerAd?.Hide();
    #endregion

    #region Interstitial Ad
    public void RequestInterstitialAd()
    {
        InterstitialAd.Load(adConfig.interstitialAdId, new AdRequest(), (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null)
            {
                Debug.Log("Interstitial Ad Failed to Load: " + error.GetMessage());
                return;
            }
            interstitialAd = ad;
            interstitialAd.OnAdFullScreenContentClosed += HandleInterstitialAdClosed;
            Debug.Log("Interstitial Ad Loaded Successfully");
        });
    }

    public bool IsInterstitialAdLoaded() => interstitialAd != null && interstitialAd.CanShowAd();

    public void ShowInterstitialAd(Action callback = null)
    {
        if (IsInterstitialAdLoaded())
        {
            onInterstitialAdClosed = callback;
            interstitialAd.Show();
        }
        else
        {
            Debug.Log("Interstitial Ad Not Ready Yet");
            callback?.Invoke();
        }
    }

    private void HandleInterstitialAdClosed()
    {
        Debug.Log("Interstitial Ad Closed");
        onInterstitialAdClosed?.Invoke();
        RequestInterstitialAd();
    }
    #endregion

    #region Rewarded Ad
    public void RequestRewardedAd()
    {
        RewardedAd.Load(adConfig.rewardedAdId, new AdRequest(), (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null)
            {
                Debug.Log("Rewarded Ad Failed to Load: " + error.GetMessage());
                return;
            }
            rewardedAd = ad;
            rewardedAd.OnAdFullScreenContentClosed += HandleRewardedAdClosed;
            Debug.Log("Rewarded Ad Loaded Successfully");
        });
    }

    public bool IsRewardedAdLoaded() => rewardedAd != null && rewardedAd.CanShowAd();

    public void ShowRewardedAd(Action callback = null)
    {
        if (IsRewardedAdLoaded())
        {
            onRewardedAdCompleted = callback;
            rewardedAd.Show((Reward reward) =>
            {
                HandleUserEarnedReward(reward);
            });
        }
        else
        {
            Debug.Log("Rewarded Ad Not Ready Yet");
            callback?.Invoke();
        }
    }

    private void HandleUserEarnedReward(Reward reward)
    {
        Debug.Log("User Earned Reward: " + reward.Amount);
    }

    private void HandleRewardedAdClosed()
    {
        Debug.Log("Rewarded Ad Closed");
        onRewardedAdCompleted?.Invoke();
        RequestRewardedAd();
    }
    #endregion
}
