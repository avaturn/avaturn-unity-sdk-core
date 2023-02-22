using UnityEngine;
using UnityEngine.UI;

public class MenuWebGL : MonoBehaviour
{
    [SerializeField] private KeyCode _key;
    [SerializeField] private Text _menuHint;
    [SerializeField] private string _openMenu, _closeMenu;
    [SerializeField] private IframeControllerWebGL _frameController;
    [SerializeField] private GameObject _hideButton;
    [SerializeField] private bool _isOpen;

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
        _menuHint.text = !_isOpen ? _openMenu : _closeMenu;
        _frameController.ChangeVisibility();
        Cursor.visible = _isOpen;
        Cursor.lockState = _isOpen ? CursorLockMode.None : CursorLockMode.Locked;
        _hideButton.SetActive(_isOpen);
    }
}
