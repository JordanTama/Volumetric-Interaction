using System;
using System.Collections.Generic;
using UnityEngine;

namespace VolumetricInteraction
{
    [ExecuteAlways, AddComponentMenu("Volumetric Interaction/Source")]
    public class Source : ActorBase
    {
        [SerializeField] private float radius;
        
        private Volume _volume;

        public Vector3 Position => transform.position;
        public float Radius => radius;
        
        
        #region Unity Event Functions

        private void OnEnable() => manager.Add(this);

        private void Start() => OnEnable();

        private void OnDisable()
        {
            if (_volume)
                _volume.Remove(this);
            
            manager.Remove(this);
        }

        #endregion


        #region Data Management

        public void Associate(Volume volume)
        {
            _volume = volume;
        }

        public void Disassociate()
        {
            _volume = null;
        }
        
        #endregion
        
        
        #region Debug

        public override void DrawDebug()
        {
            Gizmos.color = _volume is null ? Color.red : Color.yellow;
            Gizmos.DrawWireSphere(Position, radius);
        }

        #endregion
    }
}