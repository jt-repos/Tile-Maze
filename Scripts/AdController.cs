using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;

public class AdController : MonoBehaviour, IUnityAdsListener
{
    [SerializeField] float adIntervalShort;
    [SerializeField] float adIntervalLong;
    [SerializeField] int adsToLongInterval;
    string storeID = "3494883";
    string videoAdID = "video";
    string bannerAdID = "bannerAdd";
    string rewardedVideoAdID = "rewardedVideo";
    string adsPlayedKeyName = "adsPlayed";
    public static float adTimer;
    float bannerRefreshRate = 0.5f;
    float currentAdInterval;
    int adsPlayed;
    bool hasRewardSpawned;
    bool loadBannerAds;
    bool isTimerOn = false;

    //Start is called before the first frame update
    void Awake()
    {
        Advertisement.Initialize(storeID, true);
        Advertisement.AddListener(this);
        StartCoroutine(ShowBannerWhenReady());
    }

    void Start()
    {
        if (!PlayerPrefs.HasKey(adsPlayedKeyName))
        {
            PlayerPrefs.SetInt(adsPlayedKeyName, 0);
        }
        SetAdInterval();
    }

    public void SetAdTimer(bool boolVar)
    {
        isTimerOn = boolVar;
    }

    private void SetAdInterval()
    {
        var adsPlayed = PlayerPrefs.GetInt(adsPlayedKeyName);
        if (adsPlayed >= adsToLongInterval)
        {
            currentAdInterval = adIntervalLong;
        }
        else
        {
            currentAdInterval = adIntervalShort;
        }
    }

    //Update is called once per frame
    void Update()
    {
        if(isTimerOn)
        {
            adTimer += Time.deltaTime;
        }
    }

    IEnumerator ShowBannerWhenReady()
    {
        while (!Advertisement.IsReady(bannerAdID))
        {
            yield return new WaitForSeconds(bannerRefreshRate);
        }
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        Advertisement.Banner.Show(bannerAdID);
    }

    public void PlayVideoAd()
    {
        if (Advertisement.IsReady(videoAdID) && adTimer > currentAdInterval)
        {
            Advertisement.Show(videoAdID);
            var adsPlayed = PlayerPrefs.GetInt(adsPlayedKeyName) + 1;
            PlayerPrefs.SetInt(adsPlayedKeyName, adsPlayed);
            adTimer = 0;
        }
    }

    public void PlayRewardedVideoAd()
    {
        if (Advertisement.IsReady(rewardedVideoAdID))
        {
            hasRewardSpawned = false;
            Advertisement.Show(rewardedVideoAdID);
            adTimer = 0;
        }
    }

    public bool IsAdReady()
    {
        return Advertisement.IsReady();
    }

    void IUnityAdsListener.OnUnityAdsReady(string placementId)
    {
        Debug.Log("Ad Ready");
    }

    void IUnityAdsListener.OnUnityAdsDidError(string message)
    {
        Debug.Log("Ad Error");
    }

    void IUnityAdsListener.OnUnityAdsDidStart(string placementId)
    {
        Debug.Log("Ad Started");
    }

    void IUnityAdsListener.OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        if (showResult == ShowResult.Finished && !hasRewardSpawned)
        {
            hasRewardSpawned = true;
            if(FindObjectOfType<HintController>() != null)
            {
                FindObjectOfType<GameSession>().HideCanvasPlayHint();
            }
        }
        else if (showResult == ShowResult.Skipped)
        {
            Debug.Log("Skipped");
        }
        else if (showResult == ShowResult.Failed)
        {
            Debug.Log("Failed");
        }
    }

    public void SetLoadBannerAds(bool boolVar)
    {
        loadBannerAds = boolVar;
    }
}