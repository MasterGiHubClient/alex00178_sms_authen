using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance
    {
        get
        {
            if(!instance)
            {
                instance = FindObjectOfType<UIManager>();
            }

            return instance;
        }
    }

    GameObject _lastWindow;

    [SerializeField] GameObject sendCodeGO;
    [SerializeField] GameObject inputCodeGO;

    [Space(10)]
    [SerializeField] NumbersInputField numbersInputField;

    private void Start()
    {
        _lastWindow = sendCodeGO;
    }

    public void Open(int windowID)
    {
        GameObject nextWindow = windowID switch
        {
            0 => sendCodeGO,
            1 => inputCodeGO
        };

        if(_lastWindow)
        {
            _lastWindow.SetActive(false);
        }

        nextWindow.SetActive(true);
        _lastWindow = nextWindow;
    }

    public void VerifyCode()
    {
        bool _trueCode = string.Equals(numbersInputField.inputCode,AuthManager.code);
        Debug.Log(_trueCode);
    }

    public void KeyboardOnClick(int value)
    {
        numbersInputField.GetValue(value);
    }
}
