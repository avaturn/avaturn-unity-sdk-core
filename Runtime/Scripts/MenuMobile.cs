using UnityEngine;

public class MenuMobile : MonoBehaviour
{
    [SerializeField] private bool _isOpen;
    [SerializeField] private GameObject _hideButton, _controllCanvas;
    [SerializeField] private IframeControllerMobile _iframe;

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
        _hideButton.SetActive(_isOpen);
        _controllCanvas.SetActive(!_isOpen);
        _iframe.ShowView(_isOpen);
    }
}