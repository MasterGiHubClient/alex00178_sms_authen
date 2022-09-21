using System.Collections.Generic;

[System.Serializable]
public class UsersContainer
{
    public List<User> users;

    public UsersContainer(List<User> users)
    {
        this.users = users;
    }
}
