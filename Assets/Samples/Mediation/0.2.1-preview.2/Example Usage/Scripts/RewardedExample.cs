using System;
using UnityEngine;
using Unity.Services.Core;
using System.Threading.Tasks;

namespace Unity.Services.Mediation.Samples
{
    /// <summary>
    /// Sample Implementation of Unity Mediation
    /// </summary>
    public class RewardedExample : MonoBehaviour
    {
        [Header("Ad Unit Ids"), Tooltip("Ad Unit Ids for each platform that represent Mediation waterfalls.")]
        public string androidAdUnitId;
        [Tooltip("Ad Unit Ids for each platform that represent Mediation waterfalls.")]
        public string iosAdUnitId;

        IRewardedAd m_RewardedAd;

        async void Start()
        {
            try
            {
                Debug.Log("Initializing...");
                await UnityServices.InitializeAsync();
                Debug.Log("Initialized!");
                InitializationComplete();
            }
            catch (Exception e)
            {
                InitializationFailed(e);
            }
        }

        public void ShowRewarded()
        {
            if (m_RewardedAd.AdState == AdState.Loaded)
            {
                m_RewardedAd.Show();
            }
        }

        void LoadAd()
        {
            m_RewardedAd.Load();
        }

        void InitializationComplete()
        {
            // Impression Event
            MediationService.Instance.ImpressionEventPublisher.OnImpression += ImpressionEvent;
            
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    m_RewardedAd = MediationService.Instance.CreateRewardedAd(androidAdUnitId);
                    break;

                case RuntimePlatform.IPhonePlayer:
                    m_RewardedAd = MediationService.Instance.CreateRewardedAd(iosAdUnitId);
                    break;
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.LinuxEditor:
                    m_RewardedAd = MediationService.Instance.CreateRewardedAd(!string.IsNullOrEmpty(androidAdUnitId) ? androidAdUnitId : iosAdUnitId);
                    break;
                default:
                    Debug.LogWarning("Mediation service is not available for this platform:" + Application.platform);
                    return;
            }

            // Load Events
            m_RewardedAd.OnLoaded += AdLoaded;
            m_RewardedAd.OnFailedLoad += AdFailedLoad;

            // Show Events
            m_RewardedAd.OnUserRewarded += UserRewarded;
            m_RewardedAd.OnClosed += AdClosed;
            m_RewardedAd.OnShowed += AdShown;
            m_RewardedAd.OnFailedShow += AdFailedShow;
            m_RewardedAd.OnClicked += RewardedAd_OnClicked;

            Debug.Log($"Initialized On Start. Loading Ad...");
            LoadAd();
        }

        private void RewardedAd_OnClicked(object sender, EventArgs e)
        {
            Debug.Log("on clicked");
        }

        void InitializationFailed(Exception error)
        {
            SdkInitializationError initializationError = SdkInitializationError.Unknown;
            if (error is InitializeFailedException initializeFailedException)
            {
                initializationError = initializeFailedException.initializationError;
            }
            Debug.Log($"Initialization Failed: {initializationError}:{error.Message}");
        }

        void AdLoaded(object sender, EventArgs sargs)
        {
            Debug.Log("Loaded Rewarded!");
        }

        void AdFailedLoad(object sender, LoadErrorEventArgs args)
        {
            Debug.Log("Rewarded Load Failure");
            Debug.Log(args.Message);
        }

        void UserRewarded(object sender, RewardEventArgs e)
        {
            Debug.Log($"User Rewarded! Type: {e.Type} Amount: {e.Amount}");
        }

        void AdShown(object sender, EventArgs e)
        {
            Debug.Log("Rewarded Shown!");
        }

        void AdClosed(object sender, EventArgs args)
        {
            Debug.Log("Rewarded Closed! Loading Ad...");
            LoadAd();
        }

        void AdFailedShow(object sender, ShowErrorEventArgs args)
        {
            Debug.Log($"Rewarded failed to show: {args.Message}");
        }

        void ImpressionEvent(object sender, ImpressionEventArgs args)
        {
            var impressionData = args.ImpressionData != null ? JsonUtility.ToJson(args.ImpressionData, true) : "null";
            Debug.Log($"Impression event from ad unit id {args.AdUnitId} : {impressionData}");
        }
    }
}
