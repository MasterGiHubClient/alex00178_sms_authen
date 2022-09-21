
[System.Serializable]
public class User
{
    public string name;
    public string[] visits;

    public User(string name, string[] visits)
    {
        this.name = name;
        this.visits = visits;
    }
}
