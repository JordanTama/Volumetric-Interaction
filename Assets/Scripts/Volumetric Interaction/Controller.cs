using System;
using UnityEngine;

namespace VolumetricInteraction
{
    [ExecuteAlways, AddComponentMenu("Volumetric Interaction/Controller")]
    public class Controller : ActorBase
    {
        private void Awake() => manager.Initialize();

        private void Update() => manager.Update();

        public override void DrawDebug() => manager.DrawDebug();
    }
}