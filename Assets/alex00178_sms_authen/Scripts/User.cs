using System.Collections.Generic;

[System.Serializable]
public class User
{
    public string name;
    public string phoneNumber;
    public List<string> visits;

    public User(string name, string phoneNumber, List<string> visits)
    {
        this.name = name;
        this.phoneNumber = phoneNumber;
        this.visits = visits;
    }
}
