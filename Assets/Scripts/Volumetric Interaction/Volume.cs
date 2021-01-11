using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using VolumetricInteraction.Utility;

namespace VolumetricInteraction
{
    [ExecuteAlways, AddComponentMenu("Volumetric Interaction/Volume")]
    public class Volume : ActorBase
    {
        private readonly List<Source> _sources = new List<Source>();


        public int Count => _sources.Count;

        
        #region Unity Event Functions

        private void OnEnable() => manager.Add(this);
        
        private void Start() => OnEnable();
        
        private void OnDisable() => manager.Remove(this);
        
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

            manager.Add(source);
            _sources.Remove(source);
        }

        public void Clear()
        {
            foreach (Source source in _sources)
                manager.Add(source);
            
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

        public override void DrawDebug()
        {
            DrawBounds();
            
            foreach (Source source in _sources)
                source.DrawDebug();
        }

        private void DrawBounds()
        {
            Handles.zTest = CompareFunction.LessEqual;
            Handles.color = manager.FocusVolume == this ? Color.white : Color.black;
            Handles.matrix = transform.localToWorldMatrix;
            
            Handles.DrawWireCube(Vector3.zero, Vector3.one);
        }

        #endregion
    }
}