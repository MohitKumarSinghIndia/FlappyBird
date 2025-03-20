using UnityEngine;

[CreateAssetMenu(fileName = "AdConfig", menuName = "Ads/AdConfig")]
public class AdConfig : ScriptableObject
{
    public string appId;
    public string bannerAdId;
    public string interstitialAdId;
    public string rewardedAdId;
}
