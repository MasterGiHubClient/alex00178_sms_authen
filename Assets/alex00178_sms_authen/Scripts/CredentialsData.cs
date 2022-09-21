using UnityEngine;

[CreateAssetMenu(fileName = "New credentials data", menuName = "Create new credentials data")]
public class CredentialsData : ScriptableObject
{
    [SerializeField] string login;
    [SerializeField] string password;

    public string Login
    {
        get => login;
    }

    public string Password
    {
        get => password;
    }
}
