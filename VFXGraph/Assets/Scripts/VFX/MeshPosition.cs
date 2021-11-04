using System;
using UnityEngine;

namespace VFX
{
    public class MeshPosition : MonoBehaviour
    {
        [SerializeField] private float _speed = 0.0f;
        [SerializeField] private float _distanceFromCamera = 5.0f;
        [SerializeField] private Vector3 _characterPosition;


        private void Update()
        {
            FollowCharacter();
        }

        private void FollowCharacter()
        {
            Vector3 characterPos = _characterPosition;
            _characterPosition.z = _distanceFromCamera;

            Vector3 characterScreenToWorld = Camera.main.ScreenToWorldPoint(characterPos);
            
            Vector3 position = Vector3.Lerp(transform.position, characterScreenToWorld, 1.0f - Mathf.Exp(-_speed * Time.deltaTime));
            transform.position = position;
        }
    }
}
