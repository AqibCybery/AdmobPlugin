using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GetRewardByAdItem : MonoBehaviour
{
    public RewardType rewardType;
    [HideInInspector]
    public UnityEvent OnReward;
    public Button yes, no;
    public GameObject RevivePanel;
    public GameObject CongratePanel;
    public Text RewardDescription, CongrateText;
    [HideInInspector]
    public int Value;
    [HideInInspector]
    public string ValueKey;
    public Text RewardIsAvalible;
    public Text ReviveDes;

    public void InitReward(string des, RewardType type, Action onreward, Action NoButtonActions = null)
    {
        RevivePanel.SetActive(false);

        Adsmanager.instance.UpdateScreenStatus(CurrentScreen.Reward);
        rewardType = type;
        RewardDescription.text = des;
        OnReward.RemoveAllListeners();
        no.onClick.RemoveAllListeners();
        no.onClick.AddListener(DestroythisBeforeAd);

        OnReward.AddListener(() => onreward());
        if (type == RewardType.RevivePlayer)
        {
            Debug.Log(type);
            RevivePanel.SetActive(true);
            OnReward.AddListener(DestroythisAfterAd);
            ReviveDes.text = des;
        }
        else
        {
            OnReward.AddListener(CongrateActive);
        }
        if (NoButtonActions != null)
            no.onClick.AddListener(() => NoButtonActions());
        gameObject.SetActive(true);

    }
   
    private void OnEnable()
    {
        Time.timeScale = 0;
        if (Adsmanager.instance.IsRewardReady)
        {
            RewardIsAvalible.text = "Reward is Ready";
            Adsmanager.instance._ShowAndroidToastMessage("Reward Ad is Ready");

        }
        else if (Adsmanager.instance.IsrewardedintersitialAdAvailable)
        {
            RewardIsAvalible.text = "Reward is Ready";
            Adsmanager.instance._ShowAndroidToastMessage("Reward Ad is Ready");
            Adsmanager.instance.RequestAndLoadRewardedAd();

        }
        else
        {
            RewardIsAvalible.text = "Reward Ad is Loading";
            Adsmanager.instance._ShowAndroidToastMessage("Reward Ad is Getting Ready");
            Adsmanager.instance.RequestAndLoadRewardedInterstitialAd();
        }
    }
    private void Start()
    {
        yes.onClick.AddListener(RequestReward);

        no.onClick.AddListener(DestroythisBeforeAd);
    }
    void CongrateActive()
    {
        Adsmanager.instance.UpdateScreenStatus(CurrentScreen.Reward);
        RevivePanel.SetActive(false);

        switch (rewardType)
        {
            case RewardType.AddCoins:
                CongrateText.text = "You Got 50 Coins";
                break;
            case RewardType.UnlockNextLevel:
                CongrateText.text = "Level Unlocked";
                break;
            case RewardType.MedPack:
                CongrateText.text = "You Got 1 MedPack";
                break;
            case RewardType.BulletPack:
                CongrateText.text = "You Got 1 BulletPack";
                break;
            case RewardType.Reward2x:
                CongrateText.text = "You Got 2x Reward ";
                break;
            case RewardType.SkipLevel:
                CongrateText.text = "Level Skipped Sucessfully";
                break;
        }
        CongratePanel.SetActive(true);
    }

    public void DestroythisAfterAd()
    {
        switch (rewardType)
        {
            case RewardType.AddCoins:
                Adsmanager.instance.UpdateScreenStatus(CurrentScreen.MainMenu);
                break;
            case RewardType.UnlockNextLevel:
                Adsmanager.instance.UpdateScreenStatus(CurrentScreen.MainMenu);

                break;
            case RewardType.MedPack:
                Adsmanager.instance.UpdateScreenStatus(CurrentScreen.GamePlay);

                break;
            case RewardType.BulletPack:
                Adsmanager.instance.UpdateScreenStatus(CurrentScreen.GamePlay);

                break;
            case RewardType.Reward2x:
                Adsmanager.instance.UpdateScreenStatus(CurrentScreen.clear);

                break;
            case RewardType.SkipLevel:

                Adsmanager.instance.UpdateScreenStatus(CurrentScreen.GamePlay);

                break;

            case RewardType.RevivePlayer:
                Adsmanager.instance.UpdateScreenStatus(CurrentScreen.GamePlay);
                break;
        }

        Time.timeScale = 1;
        gameObject.SetActive(false);
        CongratePanel.SetActive(false);

    }
    public void DestroythisBeforeAd()
    {
        switch (rewardType)
        {
            case RewardType.AddCoins:
                Adsmanager.instance.UpdateScreenStatus(CurrentScreen.MainMenu);
                break;
            case RewardType.UnlockNextLevel:
                Adsmanager.instance.UpdateScreenStatus(CurrentScreen.MainMenu);

                break;
            case RewardType.MedPack:
                Adsmanager.instance.UpdateScreenStatus(CurrentScreen.GamePlay);

                break;
            case RewardType.BulletPack:
                Adsmanager.instance.UpdateScreenStatus(CurrentScreen.GamePlay);

                break;
            case RewardType.Reward2x:
                Adsmanager.instance.UpdateScreenStatus(CurrentScreen.clear);

                break;
            case RewardType.SkipLevel:

                Adsmanager.instance.UpdateScreenStatus(CurrentScreen.pause);

                break;

            case RewardType.RevivePlayer:
                //Adsmanager.instance.ShowInterstitialAd();
                Adsmanager.instance.UpdateScreenStatus(CurrentScreen.GamePlay);
                break;
        }

        Time.timeScale = 1;
        gameObject.SetActive(false);
        CongratePanel.SetActive(false);

    }
    public void GiveBulletPack()
    {
        Adsmanager.instance.UpdateScreenStatus(CurrentScreen.MainMenu);
        gameObject.SetActive(false);
        CongratePanel.SetActive(false);
        Time.timeScale = 1;
    }

   

    void RequestReward()
    {
        RevivePanel.SetActive(false);
        if (Adsmanager.instance.IsRewardReady)
        {
            Adsmanager.instance.ShowRewardedAd(OnReward);
        }
        else if (Adsmanager.instance.IsrewardedintersitialAdAvailable)
        {
            Adsmanager.instance.RequestAndLoadRewardedAd();
            Adsmanager.instance.ShowRewardedInterstitialAd(OnReward);
        }
        else
        {
            Adsmanager.instance.RequestAndLoadRewardedInterstitialAd();
            Adsmanager.instance._ShowAndroidToastMessage("Reward Ad is not Ready Yet");
            RewardIsAvalible.text = "Reward Is Not Ready yet!";
        }
    }
}
public enum RewardType
{
    AddCoins, UnlockNextLevel, RevivePlayer, BulletPack, MedPack, Reward2x, SkipLevel, Congart
}
