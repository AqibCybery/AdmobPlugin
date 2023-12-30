using UnityEngine.Events;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
#if RemoteConfig
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.RemoteConfig;
#endif
using System.Threading.Tasks;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.Net;
using UnityEngine.Networking;
using System.Threading;
using Google.Play.Review;
#if PLATFORM_IOS
using UnityEngine.iOS;
using Unity.Advertisement.IosSupport;

#endif

public class Adsmanager : MonoBehaviour
{
    private BannerView bannerView, RectangleBanner;
    public FetchedData Data;
    private InterstitialAd interstitialAd;
    private RewardedInterstitialAd rewardedInterstitialAd;
    public CurrentScreen currentScreen = CurrentScreen.other;
    public CurrentScreen SmartBannerScreens, BigBannerScreens;
    //public Text Requestlog;
    public int bannerrequestcount, bigbannerrequestcount, rewardedrequestcount, intersitalrequestcount, rewardedintersitalrequestcount;
    private RewardedAd rewardedAd;
    private readonly TimeSpan APPOPEN_TIMEOUT = TimeSpan.FromHours(4);
    private DateTime appOpenExpireTime;
    private AppOpenAd appOpenAd;

    public GameObject adloadingpanel, appopenbg, SmartBannerBg, BigBannerBg;
    public GetRewardByAdItem RewardAdobj;
    public Text AdsLoadingTime;
    public bool IsDevelopmentBuild;
    private bool isShowingAppOpenAd;
    public AdPosition SmallBannerAdPosition;
    public AdPosition BigBannerAdPosition;
    public string bannerid, bigbannerid, rewardedintersitalid, intertitalid, rewardedid, appopenid;
    public Text testingt;
    public static Adsmanager instance;


    #region UNITY MONOBEHAVIOR METHODS
    public void ClearAdsCache()
    {
        bannerView.Destroy();
        RectangleBanner.Destroy();
        interstitialAd.Destroy();
        rewardedInterstitialAd.Destroy();
        appOpenAd.Destroy();
    }
    private void Awake()
    {
        Time.timeScale = 1;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        instance = this;
        MobileAds.RaiseAdEventsOnUnityMainThread = true;
        DontDestroyOnLoad(gameObject);
        MobileAds.Initialize(HandleInitCompleteAction);

    }
    private void Start()
    {
        Invoke("LoadAds", 2);
#if RemoteConfig

        LoadRemoteConfigData();
#endif

    }

    public void LoadAdsIDS()
    {
        if (Data.AdmobIds.Banner_ID.Length > 5)
        {
            bannerid = Data.AdmobIds.Banner_ID;
            bigbannerid = Data.AdmobIds.Medium_Rectangle_ID;
            intertitalid = Data.AdmobIds.interstitial_ID;
            rewardedid = Data.AdmobIds.Rewarded_ID;
            appopenid = Data.AdmobIds.AppOpen_ID;
            rewardedintersitalid = Data.AdmobIds.Rewarded_interstitial_ID;

        }
    }
    public void LoadAds()
    {
        LoadAd();
    }
    void LoadAd()
    {
        if (PlayerPrefs.GetInt("removeads") == 0)
        {
            RequestAndLoadAppOpenAd(true);
            Invoke(nameof(RequestBannerAd), 2);
            Invoke(nameof(RequestbigBannerAd), 4);
            Invoke(nameof(RequestAndLoadInterstitialAd), 6);
            Invoke(nameof(RequestAndLoadRewardedAd), 7);
            Invoke(nameof(RequestAndLoadRewardedInterstitialAd), 8);

        }
        if (!Data.BigBanner_Reward)
            BigBannerScreens &= ~CurrentScreen.Reward;
        if (!Data.BigBanner_clear)
            BigBannerScreens &= ~CurrentScreen.clear;
        if (!Data.BigBanner_fail)
            BigBannerScreens &= ~CurrentScreen.fail;
        if (!Data.BigBanner_Objective)
            BigBannerScreens &= ~CurrentScreen.Objective;
        if (!Data.BigBanner_Pause)
            BigBannerScreens &= ~CurrentScreen.pause;
        if (!Data.BigBanner_Revive)
            BigBannerScreens &= ~CurrentScreen.Revive;

    }
    private void HandleInitCompleteAction(InitializationStatus initstatus)
    {
        Debug.Log("Initialization complete.");

        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {

            //  statusText.text = "Initialization complete.";
            Debug.Log("Initialization complete.");
        });
    }
    #endregion

    #region AppOpen
    private readonly TimeSpan TIMEOUT = TimeSpan.FromHours(4);
    private DateTime _expireTime;
    public bool OtherAdRunning = false;
    public bool IsAppOpenAdAvailable
    {
        get
        {
            if (appOpenAd == null)
                return false;
            else
            {
                return (appOpenAd.CanShowAd());
            }
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (!pause && !OtherAdRunning)
            ShowAppOpenAd();
    }
    //private void OnApplicationFocus(bool focus)
    //{
    //    if (focus)
    //        ShowAppOpenAd();
    //}
    public void OnAppStateChanged(AppState state)
    {
        // Display the app open ad when the app is foregrounded.
        UnityEngine.Debug.Log("App State is " + state);

        // OnAppStateChanged is not guaranteed to execute on the Unity UI thread.
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            if (state == AppState.Foreground)
            {
                ShowAppOpenAd();
            }
        });
    }
    public void RequestAndLoadAppOpenAd(bool isinstart = false)
    {
        StartCoroutine(CheckInternetConnection((isConnected) =>
        {
            if (isConnected)
            {
                if (PlayerPrefs.GetInt("removeads") == 0)
                {
                    // handle connection status here
                    Debug.Log("Internet Status: " + isConnected);
#if UNITY_EDITOR
                    string adUnitId = "unused";
#elif UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/3419835294";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/5662855259";
#else
        string adUnitId = "unexpected_platform";
#endif

                    var adRequest = new AdRequest();
                    AppOpenAd.Load(appopenid, adRequest, (AppOpenAd ad, LoadAdError error) =>
                        {
                            // If the operation failed with a reason.
                            if (error != null)
                            {
                                Debug.LogError("App open ad failed to load an ad with error : "
                                                + error);
                                return;
                            }

                            // If the operation failed for unknown reasons.
                            // This is an unexpected error, please report this bug if it happens.
                            if (ad == null)
                            {
                                Debug.LogError("Unexpected error: App open ad load event fired with " +
                                               " null ad and null error.");
                                return;
                            }

                            // The operation completed successfully.
                            Debug.Log("App open ad loaded with response : " + ad.GetResponseInfo());
                            appOpenAd = ad;
                            if (isinstart)
                                ShowAppOpenAd();
                            // App open ads can be preloaded for up to 4 hours.
                            _expireTime = DateTime.Now + TIMEOUT;
                            ad.OnAdFullScreenContentOpened += () =>
                            {
                                Debug.Log("App open ad full screen content opened.");

                                // Inform the UI that the ad is consumed and not ready.
                            };
                            // Raised when the ad closed full screen content.
                            ad.OnAdFullScreenContentClosed += () =>
                            {
                                Debug.Log("App open ad full screen content closed.");
                                isShowingAppOpenAd = false;
                                appopenbg.SetActive(false);

                                RequestAndLoadAppOpenAd();
                                UpdateScreenStatus(currentScreen);

                                if (this.appOpenAd != null)
                                {
                                    this.appOpenAd.Destroy();
                                    this.appOpenAd = null;
                                }
                                // It may be useful to load a new ad when the current one is complete.
                            };
                            // Raised when the ad failed to open full screen content.
                            ad.OnAdFullScreenContentFailed += (AdError error) =>
                            {
                                Debug.LogError("App open ad failed to open full screen content with error : "
                                                + error);
                                isShowingAppOpenAd = false;
                                appopenbg.SetActive(false);
                                RequestAndLoadAppOpenAd();
                                UpdateScreenStatus(currentScreen);
                                if (this.appOpenAd != null)
                                {
                                    this.appOpenAd.Destroy();
                                    this.appOpenAd = null;
                                }
                            };


                        });
                }

            }

        }));
    }


    public void ShowAppOpenAd()
    {
        Debug.Log("Showing app open " + IsAppOpenAdAvailable);
        if (!IsAppOpenAdAvailable)
        {
            RequestAndLoadAppOpenAd();
            return;
        }
        hidebanner();
        hidebigabanner();
        appopenbg.SetActive(true);
        isShowingAppOpenAd = true;
        Time.timeScale = 1;
        Invoke("Showappopenwithdelay", 0.2f);

    }
    void Showappopenwithdelay()
    {
        appOpenAd.Show();
    }
    #endregion

    #region RemoteConfig
#if RemoteConfig

    public struct userAttributes { }
    public struct appAttributes { }


    async Task InitializeRemoteConfigAsync()
    {
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    void LoadRemoteConfigData()
    {
        StartCoroutine(CheckInternetConnection(async (isConnected) =>
        {
            // handle connection status here
            Debug.Log("Internet Status Remote Config: " + isConnected);

            if (isConnected)
            {
                await InitializeRemoteConfigAsync();
            }
            else
            {
                if (PlayerPrefs.HasKey("RemoteConfig"))
                {
                    Data = JsonUtility.FromJson<FetchedData>(PlayerPrefs.GetString("RemoteConfig"));
                }
                LoadAdsIDS();

            }

            RemoteConfigService.Instance.FetchCompleted += ApplyRemoteSettings;
            RemoteConfigService.Instance.FetchConfigs(new userAttributes(), new appAttributes());
        }));
        //return Task.CompletedTask;
    }

    void ApplyRemoteSettings(ConfigResponse configResponse)
    {
        Debug.Log(RemoteConfigService.Instance.appConfig.config.ToString());
        Data = JsonUtility.FromJson<FetchedData>(RemoteConfigService.Instance.appConfig.config.ToString());
        PlayerPrefs.SetString("RemoteConfig", RemoteConfigService.Instance.appConfig.config.ToString());
        LoadAdsIDS();

    }
#endif

    #endregion

    #region HELPER METHODS

    IEnumerator CheckInternetConnection(Action<bool> action)
    {
        UnityWebRequest www = new UnityWebRequest("https://google.com");
        yield return www.SendWebRequest();

        Debug.Log(www.result);
        if (www.result == UnityWebRequest.Result.Success)
        {
            action(true);
        }
        else
        {
            action(false);
        }
    }
    public void UpdateScreenStatus(CurrentScreen SelectedScreen)
    {

        currentScreen = SelectedScreen;
        if ((SmartBannerScreens & SelectedScreen) != CurrentScreen.other)
        {
            showbanner();
        }
        else
        {
            hidebanner();
        }

        if ((BigBannerScreens & SelectedScreen) != CurrentScreen.other)
        {
            showbigbanner();
        }
        else
        {
            hidebigabanner();
        }
    }
    private AdRequest CreateAdRequest()
    {
        return new AdRequest();
    }
    public void _ShowAndroidToastMessage(string message)
    {
#if !UNITY_EDITOR
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity, message, 0);
                toastObject.Call("show");
            }));
        }
#endif
    }
    #endregion

    #region BANNER ADS

    public void RequestBannerAd()
    {
        StartCoroutine(CheckInternetConnection((isConnected) =>
        {
            // handle connection status here
            Debug.Log("Internet Status: " + isConnected);
            Debug.Log("Screen size : " + Screen.width / 2);

            if (isConnected)
            {
                if (PlayerPrefs.GetInt("removeads") == 0)
                {
                    if (bannerView != null)
                    {
                        bannerView.Destroy();
                    }
                    AdSize adaptiveSize =
          AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(320);
                    bannerView = new BannerView(bannerid, adaptiveSize, SmallBannerAdPosition);

                    bannerView.LoadAd(CreateAdRequest());
                    bannerrequestcount++;
                }
            }
        }));
    }


    void showbanner()
    {

        if (PlayerPrefs.GetInt("removeads") == 0)
        {
            if (bannerView != null)
            {
                bannerView.Show();
                SmartBannerBg.SetActive(true);
            }
            else
            {
                SmartBannerBg.SetActive(false);

                RequestBannerAd();
            }
        }

    }
    void hidebanner()
    {
        if (bannerView != null)
            bannerView.Hide();
        SmartBannerBg.SetActive(false);

    }

    public void RequestbigBannerAd()
    {
        StartCoroutine(CheckInternetConnection((isConnected) =>
        {
            // handle connection status here
            Debug.Log("Internet Status: " + isConnected);
            if (isConnected)
            {
                if (PlayerPrefs.GetInt("removeads") == 0)
                {
                    if (RectangleBanner != null)
                    {
                        RectangleBanner.Destroy();
                    }
                    AdSize rectangelesize = new AdSize(200, 150);
                    RectangleBanner = new BannerView(bigbannerid,AdSize.MediumRectangle, BigBannerAdPosition);

                    RectangleBanner.LoadAd(CreateAdRequest());
                    bigbannerrequestcount++;
                    RectangleBanner.Hide();
                }
            }
        }));

    }


    void showbigbanner()
    {

        if (PlayerPrefs.GetInt("removeads") == 0)
        {
            if (RectangleBanner != null)
            {
                try
                {
                    RectangleBanner?.Show();
                }catch(Exception e)
                {
                    Debug.Log(e);
                }
                BigBannerBg.SetActive(true);

            }
            else
            {
                BigBannerBg.SetActive(false);
                RequestbigBannerAd();
            }
        }
    }
    void hidebigabanner()
    {
        if (RectangleBanner != null)
            RectangleBanner.Hide();
        BigBannerBg.SetActive(false);

    }

    #endregion

    #region RewardedIntersitial
    public bool IsrewardedintersitialAdAvailable
    {
        get
        {
            if (rewardedInterstitialAd != null)
                return rewardedInterstitialAd.CanShowAd();
            else
            {
                return false;
            };
        }
    }
    public void RequestAndLoadRewardedInterstitialAd()
    {
        Debug.Log("Requesting Rewarded Interstitial ad.");

        // These ad units are configured to always serve test ads.
        string adUnitId = rewardedintersitalid;

        StartCoroutine(CheckInternetConnection((isConnected) =>
        {
            // handle connection status here
            Debug.Log("Internet Status: " + isConnected);
            if (isConnected)
            {
                RewardedInterstitialAd.Load(rewardedintersitalid, CreateAdRequest(), (RewardedInterstitialAd ad, LoadAdError error) =>
               {
                   if (error != null)
                   {
                       Debug.Log("Rewarded Interstitial ad load failed with error: " + error);
                       return;
                   }
                   if (ad == null)
                   {
                       Debug.LogError("Unexpected error: Rewarded interstitial load event fired with null ad and null error.");
                       return;
                   }
                   rewardedInterstitialAd = ad;

                   ad.OnAdFullScreenContentOpened += () =>
                   {
                       OtherAdRunning = true;

                       Debug.Log("Rewarded interstitial ad full screen content opened.");
                   };
                   // Raised when the ad closed full screen content.
                   ad.OnAdFullScreenContentClosed += () =>
                   {
                       StartCoroutine(OtherAdRunningCheck());

                       Debug.Log("Rewarded Interstitial ad dismissed.");
                       UpdateScreenStatus(currentScreen);

                       RequestAndLoadRewardedInterstitialAd();
                       this.rewardedInterstitialAd = null;

                       Debug.Log("Rewarded interstitial ad full screen content closed.");
                   };
                   // Raised when the ad failed to open full screen content.
                   ad.OnAdFullScreenContentFailed += (AdError error) =>
                   {
                       RequestAndLoadRewardedInterstitialAd();
                       UpdateScreenStatus(currentScreen);

                       this.rewardedInterstitialAd = null;
                       Debug.LogError("Rewarded interstitial ad failed to open full screen content" +
                                      " with error : " + error);
                   };
               });
            }
        }));

    }

    public void ShowRewardedInterstitialAd(UnityEvent rewardfunction)
    {
        if (rewardedInterstitialAd != null)
        {
            hidebigabanner();
            hidebanner();
            OtherAdRunning = true;

            rewardedInterstitialAd.Show((reward) =>
            {
                rewardfunction.Invoke();
                Debug.Log("Rewarded Interstitial ad Rewarded : " + reward.Amount);
            });
        }
        else
        {
            RequestAndLoadRewardedInterstitialAd();
            Debug.Log("Rewarded Interstitial ad is not ready yet.");
        }
    }

    #endregion

    #region INTERSTITIAL ADS
    public string ScenetoloadAfterAd = "";
    public void RequestAndLoadInterstitialAd()
    {
        StartCoroutine(CheckInternetConnection((isConnected) =>
        {
            // handle connection status here
            Debug.Log("Internet Status: " + isConnected);
            if (isConnected)
            {
                if (PlayerPrefs.GetInt("removeads") == 0)
                {
                    if (interstitialAd != null)
                    {
                        interstitialAd.Destroy();
                    }
                    InterstitialAd.Load(intertitalid, CreateAdRequest(), (InterstitialAd ad, LoadAdError error) =>
                    {
                        if (ad == null)
                        {
                            Debug.LogError("Unexpected error: Rewarded interstitial load event fired with null ad and null error.");
                            return;
                        }
                        ad.OnAdFullScreenContentOpened += () =>
                        {

                            Debug.Log("Interstitial ad full screen content opened.");
                        };
                        // Raised when the ad closed full screen content.
                        ad.OnAdFullScreenContentClosed += () =>
                        {
                            RequestAndLoadInterstitialAd();
                            StartCoroutine(OtherAdRunningCheck());
                            UpdateScreenStatus(currentScreen);
                            if (ScenetoloadAfterAd != string.Empty)
                            {
                                loadscene.allowSceneActivation = true;

                            }
                            Debug.Log("Interstitial ad full screen content closed.");
                        };
                        // Raised when the ad failed to open full screen content.
                        ad.OnAdFullScreenContentFailed += (AdError error) =>
                        {
                            UpdateScreenStatus(currentScreen);

                            if (ScenetoloadAfterAd != string.Empty)
                                LoadSceneAfterAd();
                            Debug.LogError("Interstitial ad failed to open full screen content with error : "
                                + error);
                        };
                        interstitialAd = ad;

                    });

                }
            }
        }));
    }
    IEnumerator OtherAdRunningCheck()
    {
        OtherAdRunning = true;
        yield return new WaitForSecondsRealtime(2);
        OtherAdRunning = false;

    }
    public AsyncOperation loadscene;
    void LoadSceneAfterAd()
    {

        loadscene = SceneManager.LoadSceneAsync(ScenetoloadAfterAd);
        loadscene.allowSceneActivation = false;

    }
    public void ShowInterstitialAd(string scenename = "")
    {
        ScenetoloadAfterAd = scenename;
        try
        {

            if (PlayerPrefs.GetInt("removeads") == 0)
            {
                Debug.Log("Ads Calling intersitial with scene name :" + scenename);
                if (interstitialAd != null)
                {
                    print("Interstitial ad is not empty");

                    if (interstitialAd.CanShowAd())
                    {
                        print("Interstitial ad  can show");

                        AdsTimer = 3;

                        StartCoroutine(CallCompleteForLoadAds());
                        adloadingpanel?.SetActive(true);

                    }
                    else
                    {
                        print("Interstitial ad is not ready yet.");

                        RequestAndLoadInterstitialAd();
                        if (ScenetoloadAfterAd != string.Empty)

                            SceneManager.LoadSceneAsync(ScenetoloadAfterAd);
                    }
                }
                else
                {
                    print("Interstitial ad is not ready yet.");

                    RequestAndLoadInterstitialAd();
                    if (ScenetoloadAfterAd != string.Empty)

                        SceneManager.LoadSceneAsync(ScenetoloadAfterAd);
                }
            }
            else
            {
                if (ScenetoloadAfterAd != string.Empty)

                    SceneManager.LoadSceneAsync(ScenetoloadAfterAd);
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            if (ScenetoloadAfterAd != string.Empty)

                SceneManager.LoadSceneAsync(ScenetoloadAfterAd);

        }
    }
    float AdsTimer = 3;
    IEnumerator CallCompleteForLoadAds()
    {
        hidebigabanner();
        hidebanner();
        while (true)
        {
            yield return new WaitForSecondsRealtime(0.3f);
            AdsTimer -= 1;
            print("Interstitial ad  is showing");

            AdsLoadingTime.text = AdsTimer.ToString();

            if (AdsTimer < 0)
            {
                print("Interstitial ad  is showed");
                if (ScenetoloadAfterAd != string.Empty)
                    LoadSceneAfterAd();
                OtherAdRunning = true;
                interstitialAd.Show();
                adloadingpanel.SetActive(false);
                break;
            }
        }
    }

    #endregion

    #region REWARDED ADS

    public bool IsRewardReady
    {
        get
        {
            if (rewardedAd != null)
                return rewardedAd.CanShowAd();
            else
            {
                return false;
            };
        }

    }
    public void RequestAndLoadRewardedAd()
    {
        StartCoroutine(CheckInternetConnection((isConnected) =>
        {
            // handle connection status here
            Debug.Log("Internet Status: " + isConnected);
            if (isConnected)
            {
                RewardedAd.Load(rewardedid, CreateAdRequest(), (RewardedAd ad, LoadAdError error) =>
                {
                    // If the operation failed with a reason.
                    if (error != null)
                    {
                        Debug.LogError("Rewarded ad failed to load an ad with error : " + error);
                        return;
                    }
                    if (ad == null)
                    {
                        Debug.LogError("Unexpected error: Rewarded load event fired with null ad and null error.");
                        return;
                    }

                    Debug.Log("Rewarded ad loaded with response : " + ad.GetResponseInfo());
                    rewardedAd = ad;
                    ad.OnAdFullScreenContentOpened += () =>
                    {
                        OtherAdRunning = true;

                        Debug.Log("Rewarded ad full screen content opened.");
                    };
                    // Raised when the ad closed full screen content.
                    ad.OnAdFullScreenContentClosed += () =>
                    {
                        StartCoroutine(OtherAdRunningCheck());

                        RequestAndLoadRewardedAd();
                        UpdateScreenStatus(currentScreen);
                        Debug.Log("Rewarded ad full screen content closed.");
                    };
                    // Raised when the ad failed to open full screen content.
                    ad.OnAdFullScreenContentFailed += (AdError error) =>
                    {
                        RequestAndLoadRewardedAd();
                        UpdateScreenStatus(currentScreen);
                        Debug.LogError("Rewarded ad failed to open full screen content with error : "
                            + error);
                    };
                });
                rewardedrequestcount++;
            }
        }));

    }
    public void ShowRewardedAd(UnityEvent RewardCallBack)
    {


        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            hidebigabanner();
            hidebanner();
            OtherAdRunning = true;

            rewardedAd.Show((Reward reward) =>
            {
                Debug.Log("Reward Ads is Done");
                RewardCallBack.Invoke();
            });
        }
        else
        {
            print("Rewarded ad is not ready yet.");
        }
    }

    #endregion

    #region ReviewInapp
    public void ReviewInappRequest()
    {
        // increase game open counter
        int gameOpenCounter = PlayerPrefs.GetInt("gameOpenCounter", 0) + 1;
        PlayerPrefs.SetInt("gameOpenCounter", gameOpenCounter);
        if (4 == gameOpenCounter)
        {
            PlayerPrefs.SetInt("gameOpenCounter", 0);
#if PLATFORM_IOS
                Device.RequestStoreReview();
#else
            {
                var reviewManager = new ReviewManager();

                // start preloading the review prompt in the background
                var playReviewInfoAsyncOperation = reviewManager.RequestReviewFlow();

                // define a callback after the preloading is done
                playReviewInfoAsyncOperation.Completed += playReviewInfoAsync =>
                {

                    if (playReviewInfoAsync.Error == ReviewErrorCode.NoError)
                    {
                        // display the review prompt
                        StartCoroutine(OtherAdRunningCheck());
                        var playReviewInfo = playReviewInfoAsync.GetResult();
                        reviewManager.LaunchReviewFlow(playReviewInfo);
                    }
                };
            }
#endif
        }
    }
    #endregion

}
#region StatesEnum
[System.Serializable]
public struct FetchedData
{
    public AdmobIdsStruct AdmobIds;
    public bool Pause_btn;
    public bool Next_btn;
    public bool Restart_btn;
    public bool Home_btn;
    public bool Select_Weapon_btn;
    public bool Select_Level_btn;
    public bool Select_Mode_btn;
    public bool Play_btn;
    public bool Ok_Captian_btn;
    public bool On_Level_Complete;
    public bool On_Level_Failed;
    public bool BigBanner_Pause;
    public bool BigBanner_clear;
    public bool BigBanner_fail;
    public bool BigBanner_Reward;
    public bool BigBanner_Revive;
    public bool BigBanner_Objective;
    [System.Serializable]
    public struct AdmobIdsStruct
    {
        public string AppOpen_ID;
        public string Medium_Rectangle_ID;
        public string AppID;
        public string Rewarded_ID;
        public string Rewarded_interstitial_ID;
        public string interstitial_ID;
        public string Banner_ID;
    }
}

[Flags]
public enum CurrentScreen
{
    other = 0, pause = 1, fail = 2, clear = 4, Reward = 8, Revive = 16, MainMenu = 32, GamePlay = 64, Objective = 128

}

#endregion