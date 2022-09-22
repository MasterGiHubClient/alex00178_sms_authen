using UnityEngine.UI;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Collections;
using System;
using UnityEngine.Networking;

public class AuthManager : MonoBehaviour
{
    private static AuthManager instance;
    public static AuthManager Instance
    {
        get
        {
            if(!instance)
            {
                instance = FindObjectOfType<AuthManager>();
            }

            return instance;
        }
    }

    const float waitSec = 90.0f;
    static string currentPhoneInput;

    [SerializeField] CredentialsData credentialsData;
    [SerializeField] CredentialsData adminData;

    public static string code;

    public bool CurrentUserIsAdmin
    {
        get => string.Equals(PhoneNumber, adminData.Login);
    }

    [Space(10)]
    [SerializeField] InputField phoneInput;
    [SerializeField] Button loginBtn;

    [Space(10)]
    [SerializeField] GameObject spinnerGO;
    [SerializeField] Text timerText;

    [Space(10)]
    [SerializeField] Text descrText;

    public static string PhoneNumber
    {
        get
        {
            return currentPhoneInput;
        }
    }

    private void OnEnable()
    {
        loginBtn.interactable = false;
        phoneInput.interactable = true;
        phoneInput.characterLimit = 10;
        phoneInput.contentType = InputField.ContentType.DecimalNumber;
    }

    private void Start()
    {
        spinnerGO.SetActive(false);

        phoneInput.onValueChanged.AddListener((s) =>
        {
            currentPhoneInput = s;
        });

        phoneInput.onEndEdit.AddListener((s) =>
        {
            if(currentPhoneInput == null || currentPhoneInput.Length != 10)
            {
                loginBtn.interactable = false;

                phoneInput.characterLimit = 10;
                phoneInput.contentType = InputField.ContentType.DecimalNumber;

                currentPhoneInput = string.Empty;
                phoneInput.text = string.Empty;

                return;
            }

            loginBtn.interactable = true;
            phoneInput.characterLimit = 17;
            phoneInput.contentType = InputField.ContentType.Standard;
            phoneInput.text = MaskPhoneNumber(currentPhoneInput);
        });

        loginBtn.onClick.AddListener(() =>
        {
            StartCoroutine(TryGetCode(_code =>
            {
                code = _code.ToString();
                phoneInput.interactable = false;

                UIManager.Instance.Open(1);
                StartCoroutine(nameof(Waiting));

                descrText.text = $"Код был отправлен на {PhoneNumber}";
                Debug.Log($"verification code: {code}");
            }));
        });
    }

    string MaskPhoneNumber(string value)
    {
        try
        {
            if (!string.IsNullOrEmpty(value))
            {
                value = Regex.Replace(value, @"\D", ""); // to remove special characters
                return string.Concat("+7 ", double.Parse(value).ToString("(###) ###-####"));
            }
            else
            {
                return "Phone number is required !!!";
            }
        }
        catch (Exception)
        {
            return "Something went wrong !!!";
        }
    }

    IEnumerator Waiting()
    {
        spinnerGO.SetActive(true);
        loginBtn.gameObject.SetActive(false);

        float et = 0.0f;
        while(et < waitSec)
        {
            et += Time.deltaTime;
            float totalWait = waitSec - et;
            timerText.text = $"{Mathf.FloorToInt(totalWait)}";

            yield return null;
        }

        loginBtn.gameObject.SetActive(true);
        spinnerGO.SetActive(false);

        phoneInput.interactable = true;
        phoneInput.characterLimit = 10;

    }

    IEnumerator TryGetCode(Action<int> getCodeAction)
    {
        if(CurrentUserIsAdmin)
        {
            code = adminData.Password;
            getCodeAction.Invoke(int.Parse(adminData.Password));
            yield break;
        }

        string url = $"https://smsc.ru/sys/send.php?login={credentialsData.Login}&psw={credentialsData.Password}&phones={PhoneNumber}=&fmt=3&mes=code&call=1";
        Debug.Log(url);
        UnityWebRequest webRequest = UnityWebRequest.Get(url);

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(webRequest.error);
        }
        else
        {
            string jsonResponce = webRequest.downloadHandler.text;
            Responce responce = JsonUtility.FromJson<Responce>(jsonResponce);

            getCodeAction.Invoke(responce.code);
        }
    }

    [Serializable]
    public class Responce
    {
        public int code;
    }
}
