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

        public static bool Active { get; private set; }
        
        public static string DeviceUniqueIdentifier => _data ? _data.deviceUniqueIdentifier : ""; 
        public static string GraphicsDeviceName => _data ? _data.graphicsDeviceName : ""; 
        public static string ProcessorType => _data ? _data.processorType : ""; 
        public static string Resolution => _data ? _data.resolution : ""; 
        public static string SourceCount => _data ? _data.sourceCount : ""; 
        public static string TimeStep => _data ? _data.timeStep : ""; 
        public static string UseBruteForce => _data ? _data.useBruteForce : ""; 
        public static string UseDecay => _data ? _data.useDecay : ""; 
        public static string FPS => _data ? _data.fps : "";
        
        
        #region Event Functions
        
        public static void Begin(State state)
        {
            Active = true;
            
            GameObject gameObject = new GameObject
            {
                name = "Current Form Data"
            };
            _data = gameObject.AddComponent<FormData>();

            _data.deviceUniqueIdentifier = SystemInfo.deviceUniqueIdentifier;
            _data.graphicsDeviceName = SystemInfo.graphicsDeviceName;
            _data.processorType = SystemInfo.processorType;
            _data.resolution = ((Vector3Int)state.Resolution).ToString();
            _data.sourceCount = ((int)state.SourceCount).ToString();
            _data.timeStep = ((float)state.TimeStep).ToString(CultureInfo.CurrentCulture);
            _data.useBruteForce = Settings.UseBruteForce.ToString();
            _data.useDecay = Settings.UseDecay.ToString();

            _frames = 0;
            _delta = 0;
        }

        public static void Tick()
        {
            if (!Active)
                return;
            
            _frames++;
            _delta += Time.deltaTime;
            _data.fps = CalcFPS().ToString(CultureInfo.CurrentCulture);
        }

        public static void End()
        {
            Active = false;
            
            _data.fps = CalcFPS().ToString(CultureInfo.CurrentCulture);
            
            _data.SendData();
        }
        
        #endregion


        private static float CalcFPS()
        {
            return _frames == 0 ? 0f : 1.0f / (_delta / _frames);
        }


        private class FormData : MonoBehaviour
        {
            private const string AddressBase =
                "https://docs.google.com/forms/d/e/1FAIpQLSd5R1wlEJKnvJApZPM6st9tjNL9A65ha0l9u5fWV63PSZHCEw/";
            
            private const string ResponseAddress = AddressBase + "formResponse";

            private const string DeviceUniqueIdentifierId = "entry.245591943";
            private const string GraphicsDeviceNameId = "entry.1585534166";
            private const string ProcessorTypeId = "entry.2046105391";
            private const string ResolutionId = "entry.147192185";
            private const string SourceCountId = "entry.512853493";
            private const string TimeStepId = "entry.1775562960";
            private const string UseBruteForceId = "entry.1565970504";
            private const string UseDecayId = "entry.1422726356";
            private const string FPSId = "entry.935257789";

            public string deviceUniqueIdentifier;
            public string graphicsDeviceName;
            public string processorType;
            public string resolution;
            public string sourceCount;
            public string timeStep;
            public string useBruteForce;
            public string useDecay;
            public string fps;


            public void SendData()
            {
                gameObject.name = "Submitting Form Data...";
                StartCoroutine(SendDataCoroutine());
            }
            
            private IEnumerator SendDataCoroutine()
            {
                WWWForm form = new WWWForm();
                
                form.AddField(DeviceUniqueIdentifierId, deviceUniqueIdentifier);
                form.AddField(GraphicsDeviceNameId, graphicsDeviceName);
                form.AddField(ProcessorTypeId, processorType);
                form.AddField(ResolutionId, resolution);
                form.AddField(SourceCountId, sourceCount);
                form.AddField(TimeStepId, timeStep);
                form.AddField(UseBruteForceId, useBruteForce);
                form.AddField(UseDecayId, useDecay);
                form.AddField(FPSId, fps);

                using (UnityWebRequest w = UnityWebRequest.Post(ResponseAddress, form))
                    yield return w.SendWebRequest();

                Destroy(gameObject);
            }
        }
    }
}