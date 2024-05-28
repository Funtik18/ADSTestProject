using UnityEngine;

namespace Casual.ADS
{
    [System.Serializable]
    public sealed class AdSettings
    {
        [ field: SerializeField ] public MaxSDKSettings ApplovinSettings { get; private set; }
    }
}