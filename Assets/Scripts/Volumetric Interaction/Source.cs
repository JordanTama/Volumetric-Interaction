using System.Collections.Generic;
using UnityEngine;

namespace VolumetricInteraction
{
    [AddComponentMenu("Volumetric Interaction/Source")]
    public class Source : ActorBase
    {
        [SerializeField] private float radius;
        
        private readonly List<Volume> _volumes = new List<Volume>();

        public Vector3 Position => transform.position;
        
        
        #region Debug

        public override void DrawDebug()
        {
            Gizmos.DrawWireSphere(Position, radius);
        }

        #endregion
    }
}