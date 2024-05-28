using UnityEngine;

namespace Casual.ADS
{
    [System.Serializable]
    public sealed class AdBannerUnit : AdUnit
    {
        [ field: SerializeField ] public int RefreshTimeout { get; private set; } = 10;
    }
}