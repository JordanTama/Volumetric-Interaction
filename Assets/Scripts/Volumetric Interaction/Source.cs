using System;
using System.Collections.Generic;
using UnityEngine;

namespace VolumetricInteraction
{
    [ExecuteAlways, AddComponentMenu("Volumetric Interaction/Source")]
    public class Source : ActorBase
    {
        private Volume _volume;

        public Vector3 Position { get; private set; }
        public Vector3 PreviousPosition { get; private set; }

        public float Radius => Mathf.Max(transform.localScale.x, transform.localScale.y, transform.localScale.z) / 2f;
        
        
        #region Unity Event Functions

        private void OnEnable()
        {
            Core.Add(this);
            PreviousPosition = transform.position;
            Position = transform.position;
        }

        private void OnDisable()
        {
            if (_volume)
                _volume.Remove(this);
            
            Core.Remove(this);
        }

        #endregion
        
        
        #region Custom Event Functions

        public override void OnTick()
        {
            PreviousPosition = Position;
            Position = transform.position;
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
        
#if UNITY_EDITOR
        
        public override void DrawDebug()
        {
            Gizmos.color = _volume is null ? Color.red : Color.yellow;
            Gizmos.DrawWireSphere(Position, Radius);
        }
        
#endif

        #endregion
    }
}