using UnityEngine;

namespace Casual.ADS
{
    [System.Serializable]
    public sealed class MaxSDKSettings
    {
        [ field: SerializeField ] public string SDKKey { get; private set; }
        [ field: SerializeField ] public string UserId { get; private set; }

        [ field: Space ]
        [ field: SerializeField ] public string AdMobAndroidAppId { get; private set; }
        [ field: SerializeField ] public AdPlacements Android { get; private set; }
        [ field: Space ]
        [ field: SerializeField ] public string AdMobIOSAppId { get; private set; }
        [ field: SerializeField ] public AdPlacements IOS { get; private set; }
    }
}