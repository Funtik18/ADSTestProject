#if !DISABLE_SRDEBUGGER
using Casual.ADS;
using System.ComponentModel;
using UnityEngine;
using Zenject;

public partial class SROptions
{
    [ Inject ] private readonly AdSystem _adSystem;
    
    public SROptions()
    {
        SRDebug.Instance.PanelVisibilityChanged += OnPanelVisibilityChanged;
    }
    
    #region Global

    [ Category( "0_Global" ) ]
    public void ShowMediationDebugger()
    {
        MaxSdk.ShowMediationDebugger();
    }

    [ Category( "0_Global" ) ]
    public void ShowCreativeDebugger()
    {
        MaxSdk.ShowCreativeDebugger();
    }

    #endregion

    [ Category( "1_ADS" ) ]
    public void ShowBanner()
    {
        _adSystem.Banner.Show( "test" );
    }
    
    [ Category( "1_ADS" ) ]
    public void HideBanner()
    {
        _adSystem.Banner.Hide();
    }
    
    [ Category( "1_ADS" ) ]
    public void ShowInter()
    {
        _adSystem.Interstitial.Show( "test", ( result ) =>
        {
            Debug.LogError( $"[Debug] Result {result}" );
        } );
    }
    
    [ Category( "1_ADS" ) ]
    public void ShowReward()
    {
        _adSystem.Rewarded.Show( "test", ( result ) =>
        {
            Debug.LogError( $"[Debug] Result {result}" );
        } );
    }
    
    private void OnPanelVisibilityChanged( bool isVisible )
    {
        if ( !isVisible )
            return;

        ProjectContext.Instance.Container.Inject( this );
    }
}
#endif