using System;
using UnityEngine;

namespace VolumetricInteraction
{
    // TODO: Change generate to generateInEditor and move both exposed variables to SettingsProfile.
    [ExecuteAlways, AddComponentMenu("Volumetric Interaction/Controller")]
    public class Controller : ActorBase
    {
        [SerializeField] private bool generate;
        [SerializeField] private float timeStep = 0.02f;

        private float _timer;
        

        private void OnEnable() => Core.Initialize();

        public override void DrawDebug() => Core.DrawDebug();
        
        private void Update()
        {
            _timer -= Time.deltaTime;
            if (!generate || _timer > 0f) return;
            
            Core.InteractionUpdate(timeStep - _timer);
            _timer = timeStep;
        }
    }
}