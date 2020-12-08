﻿using System;
using UnityEngine;

namespace VolumetricInteraction
{
    public abstract class ActorBase : MonoBehaviour
    {
        [SerializeField] protected Manager manager;
        [SerializeField] protected bool debugVisualisation;

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