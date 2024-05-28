using System;

namespace Casual.ADS
{
    public abstract class AdBanner : IAdPlacement
    {
        public event Action OnShowingChanged;

        public bool IsShowing
        {
            get => _isShowing;
            protected set
            {
                _isShowing = value;
                
                OnShowingChanged?.Invoke();
            }
        }
        private bool _isShowing = false;

        public abstract void Show( string placement );

        public abstract void Hide();

        public abstract bool IsReady();
    }
}