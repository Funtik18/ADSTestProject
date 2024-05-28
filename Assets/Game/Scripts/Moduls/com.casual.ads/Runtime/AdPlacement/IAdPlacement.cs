using System;

namespace Casual.ADS
{
    public interface IAdPlacement
    {
        event Action OnShowingChanged;
        
        bool IsShowing { get; }

        void Show( string placement );
        
        bool IsReady();
    }
}