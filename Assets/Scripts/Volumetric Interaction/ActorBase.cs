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
        
        public abstract void DrawDebug();
        
        #endregion
        
    }
}