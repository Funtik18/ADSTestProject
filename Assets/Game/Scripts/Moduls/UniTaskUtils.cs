using Cysharp.Threading.Tasks;
using System;

namespace Moduls
{
    public static class UniTaskUtils
    {
        public static async UniTask DelayedCallAsync( float delay, Action callback )
        {
            await UniTask.WaitForSeconds( delay );
            callback?.Invoke();
        }
    }
}