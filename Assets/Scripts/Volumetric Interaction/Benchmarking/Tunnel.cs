using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace VolumetricInteraction.Benchmarking
{
    public class Tunnel : MonoBehaviour
    {
        [SerializeField] private GameObject sourcePrefab;
        [SerializeField] private float sizeMin;
        [SerializeField] private float sizeMax;
        [SerializeField] private float speedMin;
        [SerializeField] private float speedMax;

        private bool _running;
        private Agent[] _agents;
        private Action _callback;
        private float _duration;
        private float _time;
        
        private void Update()
        {
            if (!_running)
                return;

            OnTick();
            bool ended = _time >= _duration;
            
            Logger.Tick();
            
            if (ended)
                OnEnd();
        }

        public void Initialize(Action callback, int count, float duration, int seed = 0)
        {
            Random.InitState(seed);

            _callback = callback;
            _duration = duration;
            _time = 0f;
            
            _agents = new Agent[count];
            for (int i = 0; i < count; i++)
                _agents[i] = SpawnAgent();

            _running = true;
        }

        private void OnTick()
        {
            _time += Time.deltaTime;

            for (int i = 0; i < _agents.Length; i++)
            {
                _agents[i].Move();
                
                if (!_agents[i].ReachedDestination())
                    continue;
                
                _agents[i].Destroy();
                _agents[i] = SpawnAgent();
            }
        }

        private void OnEnd()
        {
            _running = false;
            
            foreach (Agent agent in _agents)
                agent.Destroy();

            _callback?.Invoke();
        }

        private Agent SpawnAgent()
        {
            float speed = Mathf.Lerp(speedMin, speedMax, Random.value);
            float size = Mathf.Lerp(sizeMin, sizeMax, Random.value);
            
            bool positive = Random.value >= 0.5f;
            
            Vector3 origin = GetRandomPosition(positive);
            Vector3 target = GetRandomPosition(!positive);

            GameObject source = Instantiate(sourcePrefab, origin, Quaternion.identity);
            source.transform.localScale = Vector3.one * size;
            
            return new Agent(source, origin, target, speed);
        }

        private Vector3 GetRandomPosition(bool positive)
        {
            return transform.localToWorldMatrix.MultiplyPoint(
                new Vector3(positive ? 0 : 1, Random.value, Random.value) - Vector3.one * 0.5f);
        }


        private struct Agent
        {
            private GameObject source;
            private Vector3 origin;
            private Vector3 target;
            private float speed;

            
            public Agent(GameObject source, Vector3 origin, Vector3 target, float speed)
            {
                this.source = source;
                this.origin = origin;
                this.target = target;
                this.speed = speed;
            }
            

            public void Move()
            {
                source.transform.position =
                    Vector3.MoveTowards(source.transform.position, target, speed * Time.deltaTime);
            }

            public bool ReachedDestination()
            {
                return Mathf.Approximately(0f, Vector3.Distance(source.transform.position, target));
            }

            public void Destroy()
            {
                Object.Destroy(source);
            }
        }

        #region Debugging
        
        private void OnDrawGizmos()
        {
            Handles.matrix = transform.localToWorldMatrix;
            Handles.color = Color.green;
            
            Vector3 negPosition = -Vector3.left * 0.5f;
            Vector3 posPosition = -negPosition;

            Vector3 size = new Vector3(0, 1, 1);
            
            Handles.DrawWireCube(negPosition, size);
            Handles.DrawWireCube(posPosition, size);
        }
        
        #endregion
    }
}