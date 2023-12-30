using Firebase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Analytics;
using System;
using System.Numerics;

public class FireBaseManager : MonoBehaviour
{

    public static FireBaseManager instance;
    public ModeData[] ModeData;
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSecondsRealtime(2);
        try
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    // Create and hold a reference to your FirebaseApp,
                    // where app is a Firebase.FirebaseApp property of your application class.
                    // Set a flag here to indicate whether Firebase is ready to use by your app.
                }
                else
                {
                    UnityEngine.Debug.LogError(System.String.Format(
                      "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                    // Firebase Unity SDK is not safe to use here.
                }
            });

        }
        catch (Exception e) { }
    }
    //public void logFireBaseEvent(CustomEventType typeofevent, string value = "", int intvalue = 0)
    //{
    //    FirebaseAnalytics.LogEvent(typeofevent.ToString(), "get_gems_click", 1);
    //}
   public int GetSortedLevelNumber(int levelno)
    {
        if (levelno < 10)
        {
            return levelno;
        }
        else if (levelno < 20)
        {
            levelno -= 10;
            return levelno;

        }
        else if (levelno < 30)
        {
            levelno -= 20;
            return levelno;


        }
        else if (levelno < 40)
        {

            levelno -= 30;

            return levelno;


        }
        else if (levelno < 50)
        {
            levelno -= 40;
            return levelno;

        }
        else
        {
            return levelno;

        }
    }
    
    public void loglevelSelection(int levelno)
    {
        int sortedlevel = GetSortedLevelNumber(levelno);
        Debug.Log(sortedlevel);
        FirebaseAnalytics.LogEvent(Constants.CurrentModeName + "_Level_0" + sortedlevel);
    }
    public void logModeSelection()
    {
        Debug.Log(Constants.CurrentModeName + "_ModeSelected");
        FirebaseAnalytics.LogEvent(Constants.CurrentModeName + "_ModeSelected");
    }

    public void logbuttonwithdata(string buttonname)
    {
        int sortedlevel = GetSortedLevelNumber(Constants.currentlevel);
        FirebaseAnalytics.LogEvent(Constants.CurrentModeName + "_Level_0" + sortedlevel + "_"+ buttonname);
    }  
    public void logWaveClear(string buttonname,int Wavekills)
    {
        int sortedlevel = GetSortedLevelNumber(Constants.currentlevel);
        FirebaseAnalytics.LogEvent(Constants.CurrentModeName + "_Kills" + Wavekills + "_"+ buttonname);
    }  
    public void logbuttonClick(string buttonname)
    {
        FirebaseAnalytics.LogEvent(buttonname);
    }
    public void logWeaponSelected(string GunName)
    {
        int sortedlevel = GetSortedLevelNumber(Constants.currentlevel);

        FirebaseAnalytics.LogEvent(Constants.CurrentModeName + "_Weapon_" + GunName + "_Level_0" + sortedlevel);
    }
    public void logbuttonaction(string buttonname)
    {
        int sortedlevel = GetSortedLevelNumber(Constants.currentlevel);
        FirebaseAnalytics.LogEvent(Constants.CurrentModeName + "_"+buttonname+ "_Level_0"+ sortedlevel);
    }
    public void logCompleteLevel()
    {
        int sortedlevel = GetSortedLevelNumber(Constants.currentlevel);
        FirebaseAnalytics.LogEvent(Constants.CurrentModeName + "_Level_0" + sortedlevel + "_Complete");
    }
    public void logFailedLevel()
    {
        int sortedlevel = GetSortedLevelNumber(Constants.currentlevel);
        FirebaseAnalytics.LogEvent(Constants.CurrentModeName + "_Level_0" + sortedlevel + "_Failed");
    }
}
[Serializable]
public struct ModeData
{
    public string modename;
    public int Totallevel;
  
}
