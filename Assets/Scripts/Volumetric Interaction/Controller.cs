using System;
using UnityEngine;

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
            _timer -= Time.deltaTime;
            if (_timer > 0f) return;
            
            Core.InteractionUpdate(Settings.TimeStep - _timer);
            _timer = Settings.TimeStep;
        }

        #endregion
        
        
        #region Debugging

        public override void DrawDebug()
        {
            Core.DrawDebug();
        }
        
        private void OnDrawGizmos()
        {
            if (Settings.DrawGizmos)
                Core.DrawDebug();
        }
        
        #endregion
    }
}