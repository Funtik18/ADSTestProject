using Cysharp.Threading.Tasks;
using Moduls;
using System;
using UnityEngine;

namespace Casual.ADS
{
    public sealed class AdMaxInterstitial : AdInterstitial
    {
        private event Action< bool > _callback;
        
        private string _placement;
        private bool _isClicked;
		private int _retryAttempt;

		private AdUnit _adUnit;

		public AdMaxInterstitial( AdSettings settings )
		{
#if UNITY_ANDROID || UNITY_EDITOR
			_adUnit = settings.ApplovinSettings.Android.Interstitial;
#elif UNITY_IOS
            _adUnit = settings.ApplovinSettings.IOS.Interstitial;
#endif

			MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += AdDisplayFailedHandler;
			MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += AdLoadFailedHandler;

			MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += AdLoadedHandler;

			MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += AdDisplayedHandler;
			MaxSdkCallbacks.Interstitial.OnAdClickedEvent += AdClickedHandler;
			MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += AdHiddenHandler;

			//MaxSdkCallbacks.Interstitial.OnAdReviewCreativeIdGeneratedEvent;
		}

		public override void Show( string placement, Action< bool > callback )
		{
			if ( IsReady() )
			{
				Debug.Log( $"[AdSystem] Interstitial try show ad: {placement}" );

				_placement = placement;
				_callback = callback;
				
				_isClicked = false;
				
				// MaxSdk.SetMuted( true );
				MaxSdk.ShowInterstitial( _adUnit.Identifier );
			}
			else
			{
				Load();
			}
		}

		public override bool IsReady()
		{
			return MaxSdk.IsInitialized() ? MaxSdk.IsInterstitialReady(_adUnit.Identifier) : false;
		}

		public override void Load()
		{
			Debug.Log($"[AdSystem] Interstitial ad start load.");

			MaxSdk.LoadInterstitial(_adUnit.Identifier);
		}
		

		private void AdLoadedHandler(string adUnitId, MaxSdkBase.AdInfo adInfo)
		{
			// MaxSdk.IsInterstitialReady(adUnitId) now returns 'true'
			Debug.Log($"[AdSystem] Interstitial ad loaded and ready to show.");

			_retryAttempt = 0;
			
			LoadedAndReadyToShow();
		}
		
		private void AdLoadFailedHandler(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
		{
			Debug.LogError($"[AdSystem] Interstitial Load Failed: {errorInfo}");

			// AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds)
			_retryAttempt++;
			double retryDelay = Math.Pow(2, Math.Min(6, _retryAttempt));
			UniTaskUtils.DelayedCallAsync( (float) retryDelay, Load ).Forget();
		}

		private void AdDisplayedHandler(string adUnitId, MaxSdkBase.AdInfo adInfo)
		{
			Debug.Log($"[AdSystem] Interstitial ad displayed.");

			IsShowing = true;
		}

		private void AdDisplayFailedHandler(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
		{
			Debug.LogError($"[AdSystem] Interstitial ad failed to display.");

			IsShowing = false;

			// Interstitial ad failed to display. AppLovin recommends that you load the next ad.
			Load();
			
			_callback?.Invoke( false );
			_callback = null;
		}
		
		private void AdHiddenHandler(string adUnitId, MaxSdkBase.AdInfo adInfo)
		{
			Debug.Log( "[AdSystem] Interstitial ad is hidden. Pre-load the next ad." );

			IsShowing = false;

			Load();
			
			_callback?.Invoke( true );
			_callback = null;
		}
		
		private void AdClickedHandler(string adUnitId, MaxSdkBase.AdInfo adInfo)
		{
			Debug.Log( "[AdSystem] Interstitial ad was clicked." );

			_isClicked = true;
		}
    }
}