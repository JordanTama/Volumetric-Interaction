using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.Networking;

namespace VolumetricInteraction.Benchmarking
{
    public static class Logger
    {
        private static FormData _data;
        private static int _frames;
        private static float _delta;


        #region Event Functions
        
        public static void Begin(State state)
        {
            GameObject gameObject = new GameObject
            {
                name = "Current Form Data"
            };
            _data = gameObject.AddComponent<FormData>();
            
            _data.graphicsDeviceName = SystemInfo.graphicsDeviceName;
            _data.processorType = SystemInfo.processorType;
            _data.resolution = ((Vector3Int)state.Resolution).ToString();
            _data.sourceCount = ((int)state.SourceCount).ToString();
            _data.timeStep = ((float)state.TimeStep).ToString(CultureInfo.CurrentCulture);

            _frames = 0;
            _delta = 0;
        }

        public static void Tick()
        {
            _frames++;
            _delta += Time.deltaTime;
        }

        public static void End()
        {
            _delta /= _frames;
            float fps = 1.0f / _delta;
            _data.fps = fps.ToString(CultureInfo.CurrentCulture);
            
            _data.SendData();
        }
        
        #endregion


        private class FormData : MonoBehaviour
        {
            private const string AddressBase =
                "https://docs.google.com/forms/d/e/1FAIpQLSd5R1wlEJKnvJApZPM6st9tjNL9A65ha0l9u5fWV63PSZHCEw/";
            
            private const string ResponseAddress = AddressBase + "formResponse";
            
            private const string GraphicsDeviceNameId = "entry.1585534166";
            private const string ProcessorTypeId = "entry.2046105391";
            private const string ResolutionId = "entry.147192185";
            private const string SourceCountId = "entry.512853493";
            private const string TimeStepId = "entry.1775562960";
            private const string FPSId = "entry.935257789";
            
            public string graphicsDeviceName;
            public string processorType;
            public string resolution;
            public string sourceCount;
            public string timeStep;
            public string fps;


            public void SendData()
            {
                gameObject.name = "Submitting Form Data...";
                StartCoroutine(SendDataCoroutine());
            }
            
            private IEnumerator SendDataCoroutine()
            {
                WWWForm form = new WWWForm();
                
                form.AddField(GraphicsDeviceNameId, graphicsDeviceName);
                form.AddField(ProcessorTypeId, processorType);
                form.AddField(ResolutionId, resolution);
                form.AddField(SourceCountId, sourceCount);
                form.AddField(TimeStepId, timeStep);
                form.AddField(FPSId, fps);

                using (UnityWebRequest w = UnityWebRequest.Post(ResponseAddress, form))
                    yield return w.SendWebRequest();

                Destroy(gameObject);
            }
        }
    }
}