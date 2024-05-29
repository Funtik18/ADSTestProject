using Facebook.Unity;
using UnityEngine;

namespace Game
{
    public sealed class FacebookManager
    {
        public FacebookManager()
        {
            if ( !FB.IsInitialized )
            {
                Debug.Log( "[FacebookManager] Facebook SDK Start Initialization." );
                // Initialize the Facebook SDK
                FB.Init( InitCompletedHandler, UnityHidedHandler );
            }
            else
            {
                Debug.Log( "[FacebookManager] Facebook SDK Already Initialized." );
                // Already initialized, signal an app activation App Event
                FB.ActivateApp();
            }
        }

        private void InitCompletedHandler()
        {
            if ( FB.IsInitialized )
            {
                Debug.Log( "[FacebookManager] Facebook SDK Initialized." );
                // Signal an app activation App Event
                FB.ActivateApp();
                // Continue with Facebook SDK
                // ...
            }
            else
            {
                Debug.LogWarning( "[FacebookManager] Failed to Initialize the Facebook SDK." );
            }
        }

        private void UnityHidedHandler( bool isGameShown )
        {
            if ( !isGameShown )
            {
                // Pause the game - we will need to hide
                Time.timeScale = 0;
            }
            else
            {
                // Resume the game - we're getting focus again
                Time.timeScale = 1;
            }
        }
    }
}