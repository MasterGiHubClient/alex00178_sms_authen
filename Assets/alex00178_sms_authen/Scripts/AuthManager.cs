using UnityEngine.UI;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Collections;
using System;
using UnityEngine.Networking;

public class AuthManager : MonoBehaviour
{
    const float waitSec = 90.0f;
    static string currentPhoneInput;

    [SerializeField] CredentialsData credentialsData;

    public static string code;

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
                currentPhoneInput = string.Empty;
                phoneInput.text = string.Empty;

                return;
            }

            loginBtn.interactable = true;
            phoneInput.text = MaskPhoneNumber(currentPhoneInput);
        });

        loginBtn.onClick.AddListener(() =>
        {
            StartCoroutine(TryGetCode(_code =>
            {
                code = _code.ToString();

                UIManager.Instance.Open(1);
                StartCoroutine(nameof(Waiting));

                descrText.text = $"Код был отправлен на {MaskPhoneNumber(PhoneNumber)}";

                Debug.Log(code);
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
    }

    IEnumerator TryGetCode(Action<int> getCodeAction)
    {
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
