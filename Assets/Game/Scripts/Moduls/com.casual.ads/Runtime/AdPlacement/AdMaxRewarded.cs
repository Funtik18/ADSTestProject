using Cysharp.Threading.Tasks;
using Moduls;
using System;
using UnityEngine;

namespace Casual.ADS
{
    public sealed class AdMaxRewarded : AdRewarded
    {
        private event Action< bool > _callback;

        private string _placement;
        private bool _isRewarded = false;
        private bool _isClicked = false;
        private int _retryAttempt;
        
        private AdUnit _adUnit;
        
        public AdMaxRewarded( AdSettings settings )
        {
#if UNITY_ANDROID || UNITY_EDITOR
            _adUnit = settings.ApplovinSettings.Android.Rewarded;
#elif UNITY_IOS
            _adUnit = settings.ApplovinSettings.IOS.Rewarded;
#endif

            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += AdLoadedHandler;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += AdLoadFailedHandler;
            
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += AdDisplayedHandler;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += AdFailedToDisplayHandler;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += AdHiddenHandler;

            MaxSdkCallbacks.Rewarded.OnAdClickedEvent += AdClickedHandler;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += AdReceivedRewardHandler;
        }

        public override void Show( string placement, Action<bool> callback = null  )
        {
            if ( IsReady() )
            {
                Debug.Log( $"[AdSystem] Rewarded try show ad: {placement}" );
                _placement = placement;
                _callback = callback;
                
                _isClicked = false;
                _isRewarded = false;
                
                MaxSdk.ShowRewardedAd(_adUnit.Identifier);
            }
            else
            {
                Load();
            }
        }
        
        public override bool IsReady()
        {
            return MaxSdk.IsInitialized() ? MaxSdk.IsRewardedAdReady(_adUnit.Identifier) : false;
        }

        public override void Load()
        {
            Debug.Log( "[AdSystem] Rewarded ad start load." );

            MaxSdk.LoadRewardedAd(_adUnit.Identifier);
        }
        
        private void AdLoadedHandler(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // MaxSdk.IsRewardedAdReady(adUnitId) now returns 'true'.
            Debug.Log( "[AdSystem] Rewarded ad loaded and ready to show." );
            
            _retryAttempt = 0;

            LoadedAndReadyToShow();
        }

        private void AdLoadFailedHandler(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            Debug.LogWarning( "[AdSystem] Rewarded ad failed to load." );

            // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds).
            _retryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, _retryAttempt));
            UniTaskUtils.DelayedCallAsync( (float) retryDelay, Load ).Forget();
        }

        
        private void AdDisplayedHandler( string adUnitId, MaxSdkBase.AdInfo adInfo )
        {
            Debug.Log( "[AdSystem] Rewarded ad displayed." );
            
            IsShowing = true;
        }

        private void AdFailedToDisplayHandler(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            Debug.LogWarning( "[AdSystem] Rewarded ad failed to display." );

            IsShowing = false;
            
            // Rewarded ad failed to display. AppLovin recommends that you load the next ad.
            Load();
            
            _callback?.Invoke( false );
            _callback = null;
        }

        private void AdHiddenHandler(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log( "[AdSystem] Rewarded ad is hidden. Pre-load the next ad." );
            
            IsShowing = false;
            
            Load();
            
            _callback?.Invoke( _isRewarded );
            _callback = null;
        }

        
        private void AdClickedHandler( string adUnitId, MaxSdkBase.AdInfo adInfo )
        {
            Debug.Log( "[AdSystem] Rewarded ad was clicked." );
            
            _isClicked = true;
        }

        private void AdReceivedRewardHandler(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log( "[AdSystem] Rewarded ad displayed and user was rewarded." );

            _isRewarded = true;
        }
    }
}