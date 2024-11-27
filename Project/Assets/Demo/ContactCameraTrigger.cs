using System.Collections;

using Rein.Cameras;

using UnityEngine;

namespace Rein.Demo
{
    public class ContactCameraTrigger : MonoBehaviour
    {
        public VirtualCamera Camera;

        private MainCameraVirtualizer _mainCamera;
        private bool _isInContact;

        public void Start()
        {
            _mainCamera = FindAnyObjectByType<MainCameraVirtualizer>();
        }

        public void OnTriggerEnter(Collider other)
        {
            _isInContact = other.gameObject.tag == "Player";

            if (_isInContact && Camera)
            {
                _mainCamera.PushCamera(Camera);

                IEnumerator Restore()
                {
                    while (_isInContact)
                    {
                        yield return null;
                    }

                    _mainCamera.RemoveCamera(Camera);
                }

                StartCoroutine(Restore());
            }
        }

        public void OnTriggerExit()
        {
            _isInContact = false;
        }
    }
}
