using System;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private CharacterController controller;
        [SerializeField] private new Camera camera;

        [SerializeField] private float speed;

        private Vector2 _input;
        
        
        private void Update()
        {
            UpdateInput();
            Move();
        }

        
        private void UpdateInput()
        {
            _input.x = Input.GetAxis("Horizontal");
            _input.y = Input.GetAxis("Vertical");
            
            _input.Normalize();
        }

        private void Move()
        {
            float distance = Time.deltaTime * speed;
            Vector3 direction = CalculateHeading();

            Vector3 translation = distance * direction;
            
            controller.Move(translation);
        }

        private Vector3 CalculateHeading()
        {
            return camera.transform.right * _input.x 
                   + Vector3.ProjectOnPlane(camera.transform.forward, transform.up) * _input.y;
        }
    }
}