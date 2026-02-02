using UnityEngine;
using UnityEngine.Events;

public class KeyPadActivator : MonoBehaviour, Interactable
{
    [SerializeField] private GameObject keypadUI;
    [SerializeField] private string puzzleCode = "1234";
    [SerializeField] public UnityEvent onCorrectCode;

    public void Interact()
    {
        var keyPad = keypadUI.GetComponent<KeyPad>();
        if (keyPad != null)
        {
            keyPad.SetCode(puzzleCode, onCorrectCode);
            keypadUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}