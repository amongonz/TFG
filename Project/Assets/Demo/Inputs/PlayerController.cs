using Rein.Inputs;

using UnityEngine;

namespace Rein.Demo
{
    public class PlayerController : MonoBehaviour
    {
        public MovementCompass MovementCompass;
        public float Speed;

        public Vector3 MovementPlaneGizmoOffset;

        private Animator _animator;

        public void Start()
        {
            _animator = GetComponent<Animator>();
        }

        public void Update()
        {
            var movementDir = MovementCompass.OnInputAxis(new(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));
            movementDir.Normalize();

            var speed = Vector3.ProjectOnPlane(Speed * movementDir, Vector3.up);
            if (speed != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(speed);
            }

            _animator.SetFloat("Speed", speed != Vector3.zero ? Speed : 0);
            _animator.SetFloat("Speed Z", speed != Vector3.zero ? Speed : 0);
        }

        public void OnDrawGizmosSelected()
        {
            MovementCompass.DrawGizmosAt(transform.position + MovementPlaneGizmoOffset);
        }
    }
}
