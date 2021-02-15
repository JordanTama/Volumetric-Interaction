using System;
using UnityEngine;
using Logger = VolumetricInteraction.Benchmarking.Logger;

namespace VolumetricInteraction
{
    [ExecuteAlways, AddComponentMenu("Volumetric Interaction/Controller")]
    public class Controller : ActorBase
    {
        private float _timer;

        
        #region Unity Event Functions

        private void OnEnable()
        {
            Core.Initialize();
        }
        
        private void Update()
        {
            Logger.Update();
            
            _timer -= Time.deltaTime;
            if (_timer > 0f) return;
            
            Logger.StartTick();
            Core.InteractionUpdate(Settings.TimeStep - _timer);
            Logger.EndTick();
            
            _timer = Settings.TimeStep;
        }

        #endregion
        
        
        #region Debugging

#if UNITY_EDITOR
        
        public override void DrawDebug()
        {
            Core.DrawDebug();
        }
        
        private void OnDrawGizmos()
        {
            if (Settings.DrawGizmos)
                Core.DrawDebug();
        }
        
#endif
        
        #endregion
    }
}