using UnityEngine;

namespace VolumetricInteraction
{
    [AddComponentMenu("Volumetric Interaction/Controller")]
    public class Controller : ActorBase
    {
        public override void DrawDebug()
        {
            manager.DrawDebug();
        }
    }
}