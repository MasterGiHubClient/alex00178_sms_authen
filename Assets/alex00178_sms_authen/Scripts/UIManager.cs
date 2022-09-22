using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

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
    [SerializeField] GameObject adminGO;

    [Header("create user ui")]
    [SerializeField] InputField nameInput;
    [SerializeField] Button addNewUserBtn;

    [Header("profile ui")]
    [SerializeField] Text userNameText;
    [SerializeField] GameObject visitGOPrefab;
    [SerializeField] Transform visitParent;

    [Header("admin ui")]
    [SerializeField] GameObject userGOPrefab;
    [SerializeField] Transform userParent;

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

            UsersManager.Instance.TryAddNewUser(Name, AuthManager.PhoneNumber, new List<string>(), out bool isAdded);
            if(isAdded)
            {
                VerifyCode();
            }
        });
    }

    void ClearLastVisists()
    {
        foreach (Transform t in visitParent)
        {
            Destroy(t.gameObject);
        }

        visitParent.DetachChildren();
    }

    void ClearLastUsers()
    {
        foreach(Transform t in userParent)
        {
            Destroy(t.gameObject);
        }

        userParent.DetachChildren();
    }

    public void Open(int windowID)
    {
        GameObject nextWindow = windowID switch
        {
            0 => sendCodeGO,
            1 => inputCodeGO,
            2 => createProfileGO,
            3 => profileGO,
            4 => adminGO
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
        bool _trueCode = string.Equals(numbersInputField.inputCode, AuthManager.code);
        if(!_trueCode)
        {
            Logbox.Instance.Show(0, "Код введен неверно");
            return;
        }

        bool userAlreadyExist = UsersManager.Instance.AlreadyExist;
        Debug.Log($"{AuthManager.PhoneNumber} exist? - {userAlreadyExist}");
        if(userAlreadyExist && !AuthManager.Instance.CurrentUserIsAdmin)
        {
            UsersManager.Instance.AddNewVisist(DateTime.Now.ToString("dd/MM/yyyy H:mm"));
            userNameText.text = UsersManager.Instance.GetUserName();

            ClearLastVisists();

            foreach (string s in UsersManager.Instance.GetUserVisists())
            {
                Instantiate(visitGOPrefab, visitParent).GetComponent<VisitGO>().SetVisit(s);
            }

            Logbox.Instance.Show(0.1f, $"Добрый день, {userNameText.text}");
        }
        
        if(AuthManager.Instance.CurrentUserIsAdmin)
        {
            ClearLastUsers();

            foreach(User u in UsersManager.Instance.Users)
            {
                Instantiate(userGOPrefab, userParent).GetComponent<UserGO>().SetUserInfo(u);
            }

            Debug.Log($"login as admin {AuthManager.PhoneNumber}");
            Logbox.Instance.Show(0.1f, $"Добрый день, ADMIN");
        }

        Open(!userAlreadyExist ? AuthManager.Instance.CurrentUserIsAdmin ? 4 : 2 : AuthManager.Instance.CurrentUserIsAdmin ? 4 : 3);
    }

    public void KeyboardOnClick(int value)
    {
        numbersInputField.GetValue(value);
    }
}
