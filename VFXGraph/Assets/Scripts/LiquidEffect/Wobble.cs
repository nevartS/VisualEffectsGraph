using System;
using UnityEngine;

namespace LiquidEffect
{
    public class Wobble : MonoBehaviour
    {
        [SerializeField] private float _maxWobble = 0.03f;
        [SerializeField] private float _wobbleSpeed = 1f;
        [SerializeField] private float _recovery = 1f;
        
        private Renderer _renderer;
        private Vector3 _lastPosition;
        private Vector3 _lastRotation;
        private Vector3 _angularVelocity;
        private Vector3 _velocity;

        private float _wobbleAmountX;
        private float _wobbleAmountZ;
        private float _wobbleAmountToAddX;
        private float _wobbleAmountToAddZ;
        
        private float _pulse;
        private float _time = 0.5f;

        private void Start()
        {
            _renderer = GetComponent<Renderer>();
        }

        private void FixedUpdate()
        {
            LiquidMath();
        }

        private void LiquidMath()
        {
            _time += Time.deltaTime;
            
            // Decrease wobble over time
            _wobbleAmountToAddX = Mathf.Lerp(_wobbleAmountToAddX, 0, Time.deltaTime * (_recovery));
            _wobbleAmountToAddZ = Mathf.Lerp(_wobbleAmountToAddZ, 0, Time.deltaTime * (_recovery));
            
            // Make a sine wave of the decreasing wobble
            _pulse = 2 * Mathf.PI * _wobbleSpeed;
            _wobbleAmountX = _wobbleAmountToAddX * Mathf.Sin(_pulse * _time);
            _wobbleAmountZ = _wobbleAmountToAddZ * Mathf.Sin(_pulse * _time);
            
            // Send it to the shader
            _renderer.material.SetFloat("_WobbleX", _wobbleAmountX);
            _renderer.material.SetFloat("_WobbleZ", _wobbleAmountZ);
            
            // Velocity
            _velocity = (_lastPosition - transform.position) / Time.deltaTime;
            _angularVelocity = transform.rotation.eulerAngles - _lastRotation;
            
            // Add clamped velocity to wobble
            _wobbleAmountToAddX += Mathf.Clamp((_velocity.x + (_angularVelocity.z * 0.2f)) * _maxWobble, -_maxWobble,
                _maxWobble);
            _wobbleAmountToAddZ += Mathf.Clamp((_velocity.z + (_angularVelocity.x * 0.2f)) * _maxWobble, -_maxWobble,
                _maxWobble);
            
            // Keep last position
            _lastPosition = transform.position;
            _lastRotation = transform.rotation.eulerAngles;
        }
    }
}
