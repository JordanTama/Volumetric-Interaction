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
        
        public abstract void DrawDebug();
        
#endif
        
        #endregion
        
    }
}