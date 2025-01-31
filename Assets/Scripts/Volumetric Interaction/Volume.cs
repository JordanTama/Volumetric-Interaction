﻿using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace VolumetricInteraction
{
    [ExecuteAlways, AddComponentMenu("Volumetric Interaction/Volume")]
    public class Volume : ActorBase
    {
        private readonly List<Source> _sources = new List<Source>();

        private const DebugFlag DebugFlag = VolumetricInteraction.DebugFlag.Volume;

        public int Count => _sources.Count;

        
        #region Unity Event Functions

        private void OnEnable() => Core.Add(this);
        
        private void OnDisable() => Core.Remove(this);
        
        #endregion
        
        
        #region Custom Event Functions

        public override void OnTick()
        {
            foreach (Source source in _sources)
                source.OnTick();
        }
        
        #endregion


        #region Data Management

        public void Add(Source source)
        {
            if (_sources.Contains(source)) return;
            
            _sources.Add(source);
        }
        
        public void Remove(Source source)
        {
            if (!_sources.Contains(source)) return;

            Core.Add(source);
            _sources.Remove(source);
        }

        public void Clear()
        {
            foreach (Source source in _sources)
                Core.Add(source);
            
            _sources.Clear();
        }
        
        public void Clean()
        {
            for (int i = _sources.Count - 1; i >= 0; i--)
            {
                if (Bounds(_sources[i])) continue;

                Remove(_sources[i]);
            }
        }
        
        #endregion
        
        
        #region Querying

        public bool Bounds(Source source)
        {
            Vector3 localPosition = transform.InverseTransformPoint(source.Position);
            Vector3 prevLocalPosition = transform.InverseTransformPoint(source.PreviousPosition);

            return Bounds(localPosition) || Bounds(prevLocalPosition);
        }

        private bool Bounds(Vector3 localPosition)
        {
            return Mathf.Abs(localPosition.x) <= .5f
                   && Mathf.Abs(localPosition.y) <= .5f
                   && Mathf.Abs(localPosition.z) <= .5f;
        }
        
        public Source GetSource(int index) => _sources[index];
        
        #endregion


        #region Debug
        
#if UNITY_EDITOR

        protected override DebugFlag GetDebugFlag()
        {
            return DebugFlag.Volume;
        }

        public override void DrawDebug()
        {
            foreach (Source source in _sources)
                source.DrawDebug();
            
            if ((Settings.DebugFlags & GetDebugFlag()) == 0)
                return;
            
            DrawBounds();
        }

        private void DrawBounds()
        {
            Handles.zTest = CompareFunction.LessEqual;
            Handles.color = Core.FocusVolume == this ? Color.white : Color.black;
            Handles.matrix = transform.localToWorldMatrix;
            
            Handles.DrawWireCube(Vector3.zero, Vector3.one);
        }
        
#endif

        #endregion
    }
}