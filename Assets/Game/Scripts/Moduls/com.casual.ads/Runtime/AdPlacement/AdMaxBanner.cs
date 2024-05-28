using UnityEngine;

namespace Casual.ADS
{
    public sealed class AdMaxBanner : AdBanner
    {
        private AdBannerUnit _adUnit;
        private string _placement;
        
        public AdMaxBanner( AdSettings settings )
        {
#if UNITY_ANDROID || UNITY_EDITOR
            _adUnit = settings.ApplovinSettings.Android.Banner;
#elif UNITY_IOS
            _adUnit = settings.ApplovinSettings.IOS.Banner;
#endif
            
            MaxSdkCallbacks.Banner.OnAdLoadedEvent += AdLoadedHandler;
            MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += AdLoadFailedHandler;
            MaxSdkCallbacks.Banner.OnAdClickedEvent += AdClickedHandler;
            MaxSdkCallbacks.Banner.OnAdExpandedEvent += AdExpandedHandler;
            MaxSdkCallbacks.Banner.OnAdCollapsedEvent += AdCollapsedHandler;
        }

        public override void Show( string placement )
        {
            if ( IsReady() )
            {
                Debug.Log( $"[AdSystem] Banner try show ad: {placement}" );

                _placement = placement;

                MaxSdk.CreateBanner( _adUnit.Identifier, MaxSdkBase.BannerPosition.BottomCenter );

                MaxSdk.SetBannerExtraParameter( _adUnit.Identifier, "ad_refresh_seconds", _adUnit.RefreshTimeout.ToString() );
                // MaxSdk.SetBannerBackgroundColor(_adUnit.Identifier, Color.black);
                MaxSdk.ShowBanner( _adUnit.Identifier );

                IsShowing = true;
            }
        }

        public override void Hide()
        {
            IsShowing = false;

            MaxSdk.HideBanner( _adUnit.Identifier );
            MaxSdk.DestroyBanner( _adUnit.Identifier );
        }

        public override bool IsReady() => MaxSdk.IsInitialized();

        private void AdLoadedHandler( string adUnitId, MaxSdkBase.AdInfo adInfo )
        {
            Debug.Log( "[AdSystem] Banner ad loaded and ready to show." );
        }

        private void AdLoadFailedHandler( string adUnitId, MaxSdkBase.ErrorInfo errorInfo )
        {
            Debug.LogWarning( "[AdSystem] Banner ad failed to load." );
        }

        private void AdExpandedHandler( string adUnitId, MaxSdkBase.AdInfo adInfo )
        {
            Debug.Log( "[AdSystem] Banner ad expanded." );
        }

        private void AdCollapsedHandler( string adUnitId, MaxSdkBase.AdInfo adInfo )
        {
            Debug.Log( "[AdSystem] Banner ad collapsed." );
        }
        
        private void AdClickedHandler( string adUnitId, MaxSdkBase.AdInfo adInfo )
        {
            Debug.Log( "[AdSystem] Banner ad was clicked." );
        }
    }
}