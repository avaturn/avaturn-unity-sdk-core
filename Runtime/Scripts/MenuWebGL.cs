using UnityEngine;
using UnityEngine.UI;

namespace Avaturn
{
    public class MenuWebGL : MonoBehaviour
    {
        [SerializeField] private KeyCode _key;
        [SerializeField] private IframeControllerWebGL _frameController;
        [SerializeField] private GameObject _openButton, _hideButton;
        [SerializeField] private bool _isOpen;
        [SerializeField] private bool _isLockCursor;

        void Update()
        {
            if (Input.GetKeyDown(_key))
            {
                Switch();
            }
        }

        public void Open()
        {
            DefinedSwitch(true);
        }

        public void Close()
        {
            DefinedSwitch(false);
        }

        public void Switch()
        {
            DefinedSwitch(!_isOpen);
        }

        private void DefinedSwitch(bool isOpen)
        {
            _isOpen = isOpen;
            _frameController.ChangeVisibility();
            _hideButton.SetActive(_isOpen);
            _openButton.SetActive(!_isOpen);

            if (_isLockCursor)
            {
                Cursor.visible = _isOpen;
                Cursor.lockState = _isOpen ? CursorLockMode.None : CursorLockMode.Locked;
            }
        }
    }
}
