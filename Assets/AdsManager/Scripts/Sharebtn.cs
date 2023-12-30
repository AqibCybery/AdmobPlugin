using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sharebtn : MonoBehaviour
{

    private bool isFocus = false;
    private bool isProcessing = false;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(ShareText);
    }

    void OnApplicationFocus(bool focus)
    {
        isFocus = focus;
    }

    private void ShareText()
    {

#if UNITY_ANDROID
		if (!isProcessing) {
			StartCoroutine (ShareTextInAnroid ());
		}
#else
        Debug.Log("No sharing set up for this platform.");
#endif
    }



//#if UNITY_ANDROID
	public IEnumerator ShareTextInAnroid () {

		var shareSubject = "Sharing best game i have ever played";
		var shareMessage = "I played this game and enjoy it alot. " +
						   "Get the " + Application.productName+ "from the link below. \nCheers\n\n https://play.google.com/store/apps/details?id=\\"+ Application.identifier;

		isProcessing = true;

		if (!Application.isEditor) {
			//Create intent for action send
			AndroidJavaClass intentClass = 
				new AndroidJavaClass ("android.content.Intent");
			AndroidJavaObject intentObject = 
				new AndroidJavaObject ("android.content.Intent");
			intentObject.Call<AndroidJavaObject> 
				("setAction", intentClass.GetStatic<string> ("ACTION_SEND"));

			//put text and subject extra
			intentObject.Call<AndroidJavaObject> ("setType", "text/plain");
			intentObject.Call<AndroidJavaObject> 
				("putExtra", intentClass.GetStatic<string> ("EXTRA_SUBJECT"), shareSubject);
			intentObject.Call<AndroidJavaObject> 
				("putExtra", intentClass.GetStatic<string> ("EXTRA_TEXT"), shareMessage);

			//call createChooser method of activity class
			AndroidJavaClass unity = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
			AndroidJavaObject currentActivity = 
				unity.GetStatic<AndroidJavaObject> ("currentActivity");
			AndroidJavaObject chooser = 
				intentClass.CallStatic<AndroidJavaObject> 
				("createChooser", intentObject, "Share your high score");
			currentActivity.Call ("startActivity", chooser);
		}

		yield return new WaitUntil (() => isFocus);
		isProcessing = false;
	}
//#endif
    // Start is called before the first frame update
 
    public void ShareGame()
    {
        //NativeShare share= new NativeShare();
        //share.SetTitle(Application.productName);
        ////share.SetText("")
        //share.SetUrl("https://play.google.com/store/apps/details?id="+  Application.identifier);
        //share.Share(); un
    }
}
