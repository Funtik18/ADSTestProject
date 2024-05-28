using System;

namespace Casual.ADS
{
    public abstract class AdRewarded : IAdPlacement
    {
        public event Action OnShowingChanged;
        public event Action OnLoadedAndReadyToShow;

        public bool IsShowing
        {
            get => _isShowing;
            protected set
            {
                _isShowing = value;
                OnShowingChanged?.Invoke();
            }
        }
        protected bool _isShowing = false;

        public void Show( string placement )
        {
            Show( placement, null );
        }
        
        public abstract void Show( string placement, Action<bool> callback );
        public abstract bool IsReady();
        public abstract void Load();

        protected void LoadedAndReadyToShow()
        {
            OnLoadedAndReadyToShow?.Invoke();
        }
    }
}