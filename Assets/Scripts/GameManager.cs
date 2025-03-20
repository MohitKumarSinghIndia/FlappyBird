using UnityEngine;
using UnityEngine.UI;
using GoogleMobileAds.Api;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Player player;
    [SerializeField] private Spawner spawner;
    [SerializeField] private Text scoreText;
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject gameOver;
    [SerializeField] private GameObject restartButton;

    public int Score { get; private set; } = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        PauseGame();
        InitializeAds();
    }

    private void InitializeAds()
    {
        GoogleAdsManager.Instance.RequestBannerAd();
        GoogleAdsManager.Instance.RequestInterstitialAd();
        GoogleAdsManager.Instance.RequestRewardedAd();
        GoogleAdsManager.Instance.ShowBannerAd();
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        player.enabled = false;
    }

    public void StartGame()
    {
        Score = 0;
        UpdateScoreText();

        playButton.SetActive(false);
        gameOver.SetActive(false);
        restartButton.SetActive(false);

        Time.timeScale = 1f;
        player.enabled = true;
        ClearPipes();
    }

    public void GameOver()
    {
        restartButton.SetActive(true);
        gameOver.SetActive(true);
        PauseGame();
    }

    public void IncreaseScore()
    {
        Score++;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        scoreText.text = Score.ToString();
    }

    private void ClearPipes()
    {
        foreach (var pipe in FindObjectsByType<Pipes>(FindObjectsSortMode.None))
        {
            Destroy(pipe.gameObject);
        }
    }

    public void RestartGame()
    {
        int randomAdType = Random.Range(0, 2); // 0 for interstitial, 1 for rewarded

        if (randomAdType == 0 && GoogleAdsManager.Instance.IsInterstitialAdLoaded())
        {
            GoogleAdsManager.Instance.ShowInterstitialAd(RestartAfterAd);
        }
        else if (randomAdType == 1 && GoogleAdsManager.Instance.IsRewardedAdLoaded())
        {
            GoogleAdsManager.Instance.ShowRewardedAd(RestartAfterAd);
        }
        else
        {
            RestartAfterAd();
        }
    }

    private void RestartAfterAd()
    {
        StartGame();
    }
}
