using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VolumetricInteraction
{
    [AddComponentMenu("Volumetric Interaction/Volume")]
    public class Volume : ActorBase
    {
        private readonly List<Source> _sources = new List<Source>();
        
        
        #region Debug

        public override void DrawDebug()
        {
            DrawBounds();
        }

        private void DrawBounds()
        {
            Handles.color = Color.white;
            Handles.matrix = transform.localToWorldMatrix;
            
            Handles.DrawWireCube(Vector3.zero, Vector3.one * 0.5f);
        }

        #endregion
    }
}