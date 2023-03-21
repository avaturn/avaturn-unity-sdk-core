using UnityEngine;
using UnityEngine.Events;

namespace Avaturn
{
    public class MenuMobile : MonoBehaviour
    {
        [SerializeField] private bool _isOpen;
        [SerializeField] private IframeControllerMobile _iframe;
        [SerializeField] private UnityEvent _openEvent, _closeEvent;

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
            _iframe.ShowView(_isOpen);
            if (isOpen)
            {
                _openEvent?.Invoke();
            }
            else
            {
                _closeEvent?.Invoke();
            }
        }
    }
}