using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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

    [Header("windows gameObjects")]
    [SerializeField] GameObject sendCodeGO;
    [SerializeField] GameObject inputCodeGO;
    [SerializeField] GameObject createProfileGO;
    [SerializeField] GameObject profileGO;

    [Header("create user ui")]
    [SerializeField] InputField nameInput;
    [SerializeField] Button addNewUserBtn;

    [Header("profile ui")]
    [SerializeField] Text userNameText;

    [Header("numbers holder")]
    [SerializeField] NumbersInputField numbersInputField;

    bool NameValid
    {
        get => !string.IsNullOrEmpty(nameInput.text) && !string.IsNullOrWhiteSpace(nameInput.text);
    }

    string Name
    {
        get => nameInput.text;
    }

    private void Start()
    {
        _lastWindow = sendCodeGO;
        addNewUserBtn.onClick.AddListener(() =>
        {
            if(!NameValid)
            {
                return;
            }

            UsersManager.Instance.TryAddNewUser(Name, AuthManager.PhoneNumber, new System.Collections.Generic.List<string>());
        });
    }

    public void Open(int windowID)
    {
        GameObject nextWindow = windowID switch
        {
            0 => sendCodeGO,
            1 => inputCodeGO,
            2 => createProfileGO,
            3 => profileGO
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
        Logbox.Instance.Show(0, _trueCode ? "Успешно" : "Код введен неверно");
        if(!_trueCode)
        {
            return;
        }

        bool userAlreadyExist = UsersManager.Instance.AlreadyExist;
        Debug.Log($"{AuthManager.PhoneNumber} exist? - {userAlreadyExist}");
        if(userAlreadyExist)
        {
            userNameText.text = UsersManager.Instance.GetUserName();
            Logbox.Instance.Show(0.25f, $"Добрый день, {userNameText.text}");
        }

        Open(userAlreadyExist ? 3 : 2);
    }

    public void KeyboardOnClick(int value)
    {
        numbersInputField.GetValue(value);
    }
}
