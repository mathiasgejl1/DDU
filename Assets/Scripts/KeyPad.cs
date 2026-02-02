using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using TMPro;

public class KeyPad : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI codeDisplayText;
    [SerializeField] private Button[] digitButtons;
    [SerializeField] private Button enterButton;
    [SerializeField] private InputActionReference escapeAction;

    [Header("Keypad Settings")]
    [SerializeField] private string correctCode = "1234";
    [SerializeField] private int codeLength = 4;

    [Header("Events")]
    public UnityEvent onCorrectCodeEntered;

    private string _enteredCode = "";

    private void Awake()
    {
        for (int i = 0; i < digitButtons.Length; i++)
        {
            int captured = i;
            digitButtons[i].onClick.AddListener(() => AddDigit(captured.ToString()));
        }
        enterButton.onClick.AddListener(CheckCode);
        UpdateDisplay();
    }

    public void AddDigit(string digit)
    {
        if (_enteredCode.Length < codeLength)
        {
            _enteredCode += digit;
            UpdateDisplay();
        }
    }

    public void CheckCode()
    {
        if (_enteredCode == correctCode)
        {
            onCorrectCodeEntered.Invoke();
        }
        _enteredCode = "";
        UpdateDisplay();
        HideKeypad();
    }

    private void UpdateDisplay()
    {
        codeDisplayText.text = _enteredCode.PadRight(codeLength, '_');
    }

    public void HideKeypad()
    {
        gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void SetCode(string code, UnityEvent onCorrect)
    {
        correctCode = code;
        onCorrectCodeEntered = onCorrect;
        _enteredCode = "";
        UpdateDisplay();
    }

    private void OnEnable()
    {
        if (escapeAction != null)
            escapeAction.action.performed += OnEscape;
        if (escapeAction != null)
            escapeAction.action.Enable();
    }

    private void OnDisable()
    {
        if (escapeAction != null)
            escapeAction.action.performed -= OnEscape;
        if (escapeAction != null)
            escapeAction.action.Disable();
    }

    private void OnEscape(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        HideKeypad();
    }
}