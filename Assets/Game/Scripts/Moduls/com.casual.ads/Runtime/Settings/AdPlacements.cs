using UnityEngine;

namespace Casual.ADS
{
    [ System.Serializable ]
    public sealed class AdPlacements
    {
        [ field: SerializeField ] public AdBannerUnit Banner { get; private set; }
        [ field: SerializeField ] public AdUnit Interstitial { get; private set; }
        [ field: SerializeField ] public AdUnit Rewarded { get; private set; }
    }
}