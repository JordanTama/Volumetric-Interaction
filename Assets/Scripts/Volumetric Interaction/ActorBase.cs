using System;
using UnityEngine;

namespace VolumetricInteraction
{
    public abstract class ActorBase : MonoBehaviour
    {
        #region Custom Event Functions
        
        public virtual void OnTick() {}
        
        #endregion
        

        #region Debug
        
#if UNITY_EDITOR

        protected virtual DebugFlag GetDebugFlag()
        {
            return 0;
        }

        public abstract void DrawDebug();

#endif

        #endregion

    }
}