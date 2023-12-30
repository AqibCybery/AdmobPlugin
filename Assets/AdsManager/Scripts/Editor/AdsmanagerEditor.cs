using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
using GoogleMobileAds.Editor;
using GoogleMobileAds.Api;



[CustomEditor(typeof(Adsmanager))]
public class AdsmanagerEditor : Editor
{

    public SerializedProperty bannerid;
    public SerializedProperty BigBannerAdPosition, SmallBannerAdPosition;
    public SerializedProperty SmartBannerScreens, BigBannerScreens, currentScreen;
    public SerializedProperty bigbannerid;
    public SerializedProperty SmartBannerBg;
    public SerializedProperty BigBannerBg;
    public SerializedProperty intertitalid;
    public SerializedProperty rewardedintersitalid;
    public SerializedProperty rewardedid;
    public SerializedProperty appopenid;
    public SerializedProperty IsDevelopmentBuild;
    public SerializedProperty RemoteConfig;
    public SerializedProperty OtherAdRunning;
    public SerializedProperty Data;
    public SerializedProperty adloadingpanel, adloadingtime, appopenbg, RewardAdobj;

    void OnEnable()
    {
        SmallBannerAdPosition = serializedObject.FindProperty("SmallBannerAdPosition");
        BigBannerAdPosition = serializedObject.FindProperty("BigBannerAdPosition");
        SmartBannerScreens = serializedObject.FindProperty("SmartBannerScreens");
        BigBannerScreens = serializedObject.FindProperty("BigBannerScreens");
        currentScreen = serializedObject.FindProperty("currentScreen");
        SmartBannerBg = serializedObject.FindProperty("SmartBannerBg");
        BigBannerBg = serializedObject.FindProperty("BigBannerBg");
        bannerid = serializedObject.FindProperty("bannerid");
        bigbannerid = serializedObject.FindProperty("bigbannerid");
        intertitalid = serializedObject.FindProperty("intertitalid");
        rewardedintersitalid = serializedObject.FindProperty("rewardedintersitalid");
        rewardedid = serializedObject.FindProperty("rewardedid");
        appopenid = serializedObject.FindProperty("appopenid");
        IsDevelopmentBuild = serializedObject.FindProperty("IsDevelopmentBuild");
        adloadingpanel = serializedObject.FindProperty("adloadingpanel");
        appopenbg = serializedObject.FindProperty("appopenbg");
        RewardAdobj = serializedObject.FindProperty("RewardAdobj");
        adloadingtime = serializedObject.FindProperty("AdsLoadingTime");
        OtherAdRunning = serializedObject.FindProperty("OtherAdRunning");
        Data = serializedObject.FindProperty("Data");

    }
    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox("AdMob Mediation Adaptor", MessageType.Info);
        Adsmanager myScript = (Adsmanager)target;

        serializedObject.Update();

        EditorGUILayout.PropertyField(SmallBannerAdPosition);
        EditorGUILayout.PropertyField(BigBannerAdPosition);

        EditorGUILayout.PropertyField(SmartBannerScreens);
        EditorGUILayout.PropertyField(BigBannerScreens);
        EditorGUILayout.PropertyField(currentScreen);
        EditorGUILayout.PropertyField(SmartBannerBg);
        EditorGUILayout.PropertyField(BigBannerBg);
        //EditorGUILayout.PropertyField(Requestlog);              

        EditorGUILayout.PropertyField(adloadingpanel);
        EditorGUILayout.PropertyField(appopenbg);
        EditorGUILayout.PropertyField(RewardAdobj);
        EditorGUILayout.PropertyField(adloadingtime);
        EditorGUILayout.PropertyField(IsDevelopmentBuild);
        EditorGUILayout.PropertyField(bannerid);
        EditorGUILayout.PropertyField(bigbannerid);
        EditorGUILayout.PropertyField(intertitalid);
        EditorGUILayout.PropertyField(rewardedintersitalid);
        EditorGUILayout.PropertyField(rewardedid);
        EditorGUILayout.PropertyField(appopenid);
        EditorGUILayout.PropertyField(OtherAdRunning);
        EditorGUILayout.PropertyField(Data);

        if (GUILayout.Button("Config"))
        {
            if (myScript.IsDevelopmentBuild)
            {
                myScript.bannerid = "ca-app-pub-3940256099942544/6300978111";
                myScript.bigbannerid = "ca-app-pub-3940256099942544/6300978111";
                myScript.intertitalid = "ca-app-pub-3940256099942544/1033173712";
                myScript.rewardedid = "ca-app-pub-3940256099942544/5224354917";
                myScript.appopenid = "ca-app-pub-3940256099942544/3419835294";
                myScript.rewardedintersitalid = "ca-app-pub-3940256099942544/5354046379";
                EditorUtility.DisplayDialog("AdMob Adaptor", "AdMob is Configured With Test Ids", "OK");
            }
            else
            {
                //myScript.bannerid = GoogleMobileAdsSettingsIstance.adMobBannerID;
                //myScript.bigbannerid = GoogleMobileAdsSettingsIstance.adMobRectangleBannerID;
                //myScript.intertitalid = GoogleMobileAdsSettingsIstance.adMobIntersitial;
                //myScript.rewardedid = GoogleMobileAdsSettingsIstance.adMobRewarded;
                EditorUtility.DisplayDialog("AdMob Adaptor", "AdMob is Configured With Real Ids", "OK");
            }
        }
        //if (GUILayout.Button("Show Intersitial"))
        //{
        //    myScript.ShowInterstitialAd();
        //}
        serializedObject.ApplyModifiedProperties();
        //DrawDefaultInspector();


    }
}
