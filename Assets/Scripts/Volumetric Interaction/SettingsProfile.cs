using UnityEngine;

namespace VolumetricInteraction
{
    /*
     * TODO: Custom editor should:
     *      Re-initialise core when a setting has been changed.
     *      Get the names of the kernels in the assigned shader and make the field a dropdown selection.
     */
    public class SettingsProfile : ScriptableObject
    {
        public Vector3Int resolution;
        public FilterMode filterMode;
        public ComputeShader computeShader;
        public int mainKernelId;
        public string computeResultName;
        public float decaySpeed;
    }
}