using System;
using UnityEngine;

namespace VolumetricInteraction
{
    public abstract class ActorBase : MonoBehaviour
    {
        [SerializeField] protected bool debugVisualisation;


        public virtual void OnTick() {}
        

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