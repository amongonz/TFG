using UnityEngine;

namespace Rein.Demo
{
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(Rigidbody))]
    [ExecuteAlways]
    public class CharacterTriggerCollider : MonoBehaviour
    {
        public float ExtraMargin;

        private CharacterController _characterController;
        private CapsuleCollider _collider;
        private Rigidbody _rigidBody;

        public void Start()
        {
            _characterController = GetComponentInParent<CharacterController>();

            _collider = GetComponent<CapsuleCollider>();
            _collider.isTrigger = true;
            _collider.direction = 1;

            _rigidBody = GetComponent<Rigidbody>();
            _rigidBody.isKinematic = true;
        }

        public void Update()
        {
            _collider.center = _characterController.center;
            _collider.radius = _characterController.radius + ExtraMargin;
            _collider.height = _characterController.height + ExtraMargin;
        }
    }
}
