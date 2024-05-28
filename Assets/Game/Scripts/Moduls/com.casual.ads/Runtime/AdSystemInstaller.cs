using UnityEngine;
using Zenject;

namespace Casual.ADS
{
    [CreateAssetMenu(fileName = "AdSystemInstaller", menuName = "Installers/AdSystemInstaller")]
    public sealed class AdSystemInstaller : ScriptableObjectInstaller<AdSystemInstaller>
    {
        [ SerializeField ] private AdSettings _settings;

        public override void InstallBindings()
        {
            Container.BindInstance( _settings );
            
            Container.Bind< AdBanner >().To< AdMaxBanner >().AsSingle();
            Container.Bind< AdInterstitial >().To< AdMaxInterstitial >().AsSingle();
            Container.Bind< AdRewarded >().To< AdMaxRewarded >().AsSingle();
            Container.BindInterfacesAndSelfTo< AdSystem >().AsSingle().NonLazy();
        }
    }
}