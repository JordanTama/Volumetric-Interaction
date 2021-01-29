using System;
using UnityEngine;

namespace VolumetricInteraction
{
    // TODO: Change generate to generateInEditor and move both exposed variables to SettingsProfile.
    [ExecuteAlways, AddComponentMenu("Volumetric Interaction/Controller")]
    public class Controller : ActorBase
    {
        private float _timer;
        

        private void OnEnable() => Core.Initialize();

        public override void DrawDebug() => Core.DrawDebug();
        
        private void Update()
        {
            _timer -= Time.deltaTime;
            if (_timer > 0f) return;
            
            Core.InteractionUpdate(Settings.TimeStep - _timer);
            _timer = Settings.TimeStep;
        }

        private void OnDrawGizmos()
        {
            if (Settings.DrawGizmos)
                Core.DrawDebug();
        }
    }
}