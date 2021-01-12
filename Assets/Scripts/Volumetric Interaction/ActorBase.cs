using System;
using UnityEngine;

namespace VolumetricInteraction
{
    public abstract class ActorBase : MonoBehaviour
    {
        [SerializeField] protected bool debugVisualisation;


        #region Custom Event Functions
        
        public virtual void OnTick() {}
        
        #endregion
        

        #region Debug
        
        public abstract void DrawDebug();

        protected virtual void OnDrawGizmos()
        {
            if (debugVisualisation)
                DrawDebug();
        }
        
        #endregion
        
    }
}