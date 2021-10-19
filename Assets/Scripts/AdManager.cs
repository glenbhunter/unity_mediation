using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Mediation;
using Unity.Services.Core;
using System;
using UnityEngine.UI;

public class AdManager : MonoBehaviour
{
    private string m_GameID;
    private string m_Rewarded_AdUnit_ID;
    private IRewardedAd m_RewardedAd;

    [SerializeField] Button m_Show_RewardedAd_Button;

    public async void OnClick_InitializeSDK()
    {
        try
        {

#if UNITY_ANDROID
            m_GameID = "4411859";
            m_Rewarded_AdUnit_ID = "Rewarded_Android";
#endif

#if UNITY_IOS
        m_GameID = "4411858";
        m_Rewarded_AdUnit_ID = "Rewarded_iOS";
#endif

            // Initialize mediation package with gameID
            InitializationOptions options = new InitializationOptions();
            options.SetGameId(m_GameID);
            await UnityServices.InitializeAsync(options);

            Initialization_Complete();
        }
        catch(Exception e)
        {
            Initialization_Failed(e);
        }

    }

    public void OnClick_Load_RewardedAd()
    {
        m_RewardedAd.Load();
    }

    public void OnClick_Show_RewardedAd()
    {
        if (m_RewardedAd.AdState == AdState.Loaded)
        {
            m_RewardedAd.Show();
        }
    }

    private void Initialization_Complete()
    {

#if !UNITY_IOS && !UNITY_ANDROID
        Debug.LogError("Platform needs to be set to IOS or Android for Unity Mediation");
        return;
#endif

        m_RewardedAd = MediationService.Instance.CreateRewardedAd(m_Rewarded_AdUnit_ID);

        // Load Events
        m_RewardedAd.OnLoaded += AdLoaded;
        m_RewardedAd.OnFailedLoad += AdFailedToLoad;

        // Show Events
        m_RewardedAd.OnUserRewarded += RewardUser;
        m_RewardedAd.OnClosed += AdClosed;
        m_RewardedAd.OnShowed += AdShown;
        m_RewardedAd.OnFailedShow += AdFailedToShow;
    }

    private void Initialization_Failed(Exception error)
    {
        SdkInitializationError initializationError = SdkInitializationError.Unknown;
        if (error is InitializeFailedException initializeFailedException)
        {
            initializationError = initializeFailedException.initializationError;
        }
        Debug.Log($"Initialization Failed: {initializationError}:{error.Message}");
    }

    // Implement load event callback methods:
    private void AdLoaded(object sender, EventArgs args)
    {
        // Execute logic for the ad loading.
        Debug.Log("Ad Loaded");

        m_Show_RewardedAd_Button.interactable = true;
    }

    private void AdFailedToLoad(object sender, LoadErrorEventArgs args)
    {
        // Execute logic for the ad failing to load.
        Debug.Log("Ad Failed To Load");
    }

    // Implement show event callback methods:
    private void AdShown(object sender, EventArgs args)
    {
        // Execute logic for the ad showing successfully.
        Debug.Log("Ad Shown");

        m_Show_RewardedAd_Button.interactable = false;
    }

    private void AdClicked(object sender, EventArgs args)
    {
        // Execute logic for the user clicking the ad.
        Debug.Log("Ad Clicked");
    }

    private void AdClosed(object sender, EventArgs args)
    {
        // Execute logic for the user closing the ad.
        Debug.Log("Ad Closed");
    }

    private void RewardUser(object sender, EventArgs args)
    {
        // Execute logic for rewarding the user.
        Debug.Log("Rewarded User");
    }

    private void AdFailedToShow(object sender, ShowErrorEventArgs args)
    {
        // Execute logic for the ad failing to show.
        Debug.Log("Ad Failed to show");
    }
}
