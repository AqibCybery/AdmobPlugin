using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveAdsBtn : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
      if(PlayerPrefs.GetInt("Remove_Ads")==1)
            gameObject.SetActive(false);
    }
   public void CompletePurchase()
    {
        gameObject.SetActive(false);

        PlayerPrefs.SetInt("Remove_Ads", 1);
    }
 
}
