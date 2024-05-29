using AudienceNetwork;
using System;
using UnityEditor;
using UnityEngine;

namespace Casual.ADS
{
    public sealed class AdSystem
    {
        public event Action< bool > OnBlockingAdShowingChanged;
        
        public bool IsShowing => Rewarded.IsShowing || Interstitial.IsShowing;
        
        public AdBanner Banner { get; }
        public AdInterstitial Interstitial { get; }
        public AdRewarded Rewarded { get; }

        private AdSettings _settings;

        public AdSystem(
            AdSettings settings,
            AdBanner banner,
            AdInterstitial interstitial,
            AdRewarded rewarded
            )
        {
            _settings = settings ?? throw new ArgumentNullException( nameof(settings) );

            Banner = banner ?? throw new ArgumentNullException( nameof(banner) );
            Interstitial = interstitial ?? throw new ArgumentNullException( nameof(interstitial) );
            Rewarded = rewarded ?? throw new ArgumentNullException( nameof(rewarded) );
            
            MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += AdRevenuePaidHandler;
            MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += AdRevenuePaidHandler;
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += AdRevenuePaidHandler;

            Rewarded.OnShowingChanged += AdShowingChangedHandler;
            Interstitial.OnShowingChanged += AdShowingChangedHandler;

#if UNITY_EDITOR
            ApplovinSetup();
#endif

            MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
            {
                MaxSdk.SetVerboseLogging( true );
                MaxSdk.SetIsAgeRestrictedUser( false );
                MaxSdk.SetDoNotSell(false);
#if DISABLE_SRDEBUGGER
                MaxSdk.SetCreativeDebuggerEnabled(false);
#else
                MaxSdk.SetCreativeDebuggerEnabled( true );
#endif
                
                AudienceNetwork.AdSettings.SetAdvertiserTrackingEnabled( true );
                AudienceNetwork.AdSettings.SetDataProcessingOptions( new string[] {} );
                
#if UNITY_ANDROID
                Debug.Log( $"[AdSystem] Android AppTrackingStatus {sdkConfiguration.AppTrackingStatus}" );
                MaxSdk.SetHasUserConsent( true );
#elif UNITY_IOS || UNITY_IPHONE
                Debug.Log( $"[AdSystem] iOS AppTrackingStatus {sdkConfiguration.AppTrackingStatus}" );
                MaxSdk.SetHasUserConsent( sdkConfiguration.AppTrackingStatus == MaxSdkBase.AppTrackingStatus.Authorized );
#endif
                Debug.Log("[AdSystem] MaxSdk Initialized.");
                
                Interstitial.Load();
                Rewarded.Load();
            };
            
            Debug.Log("[AdSystem] MaxSdk Start Initialization.");
            MaxSdk.SetSdkKey(settings.ApplovinSettings.SDKKey);
            //MaxSdk.SetUserId("USER_ID");
            MaxSdk.InitializeSdk();
        }

#if UNITY_EDITOR
        private void ApplovinSetup()
        {
            AppLovinSettings.Instance.QualityServiceEnabled = true;

            AppLovinSettings.Instance.SdkKey = _settings.ApplovinSettings.SDKKey;
            AppLovinSettings.Instance.AdMobAndroidAppId = _settings.ApplovinSettings.AdMobAndroidAppId;
            AppLovinSettings.Instance.AdMobIosAppId = _settings.ApplovinSettings.AdMobIOSAppId;

            //AppLovinSettings.Instance.ConsentFlowEnabled = false;
            //AppLovinSettings.Instance.ConsentFlowEnabled = consentFlowEnabled;
            //AppLovinSettings.Instance.ConsentFlowPrivacyPolicyUrl = consentFlowPrivacyPolicyUrl;

            EditorUtility.SetDirty(AppLovinSettings.Instance);

            // PlayerSettings.iOS.locationUsageDescription = "Used for best downloading ads";
        }
#endif
        
        private void AdRevenuePaidHandler(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            if (adInfo == null) return;
            
            double revenue = adInfo.Revenue;
            if (revenue < 0.0) return;
            
            MaxSdkBase.SdkConfiguration sdkConfig = MaxSdk.GetSdkConfiguration();
            // Miscellaneous data
            string countryCode = sdkConfig == null ? string.Empty : sdkConfig.CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD" in most cases!
            string networkName = adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
            string adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
            string placement = adInfo.Placement; // The placement this ad's postbacks are tied to
            
            Debug.Log( $"[AdSystem] Ad {adUnitId} revenue paid {adInfo.Revenue}." );
        }

        private void AdShowingChangedHandler()
        {
            OnBlockingAdShowingChanged?.Invoke( Interstitial.IsShowing || Rewarded.IsShowing );
        }
    }
}