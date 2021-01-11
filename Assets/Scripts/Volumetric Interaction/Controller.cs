using System;
using UnityEngine;

namespace VolumetricInteraction
{
    [ExecuteAlways, AddComponentMenu("Volumetric Interaction/Controller")]
    public class Controller : ActorBase
    {
        [SerializeField] private bool generate;
        [SerializeField] private float timeStep = 0.02f;

        private float _timer;

        private void Awake() => Core.Initialize();

        private void OnEnable() => Awake();

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