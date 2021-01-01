using System;
using UnityEngine;

namespace VolumetricInteraction
{
    [ExecuteAlways, AddComponentMenu("Volumetric Interaction/Controller")]
    public class Controller : ActorBase
    {
        [SerializeField] private float timeStep = 0.02f;

        private float _timer;

        private void Awake() => manager.Initialize();

        public override void DrawDebug() => manager.DrawDebug();
        
        private void Update()
        {
            _timer -= Time.deltaTime;
            if (_timer > 0f) return;
            
            manager.InteractionUpdate(timeStep - _timer);
            _timer = timeStep;
        }
    }
}