using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class UsersContainer
{
    public List<User> users;

    public UsersContainer(List<User> users)
    {
        this.users = users;
    }

    int GetUserID(string phoneNumber)
    {
        return users.FindIndex(u => u.phoneNumber == phoneNumber);
    }

    public bool IsContains(string phoneNumber)
    {
        var numbers = users.Select(u => u.phoneNumber);
        return numbers.Contains(phoneNumber);
    }

    public string GetUserName(string phoneNumber)
    {
        return users.Where(u => u.phoneNumber == phoneNumber).FirstOrDefault().name;
    }

    public void UpdateUser(string phoneNumber, List<string> visits)
    {
        if(!IsContains(phoneNumber))
        {
            Debug.LogError($"user {phoneNumber} missing in db");
        }

        int id = GetUserID(phoneNumber);
        users[id] = new User(users[id].name, users[id].phoneNumber, visits);
    }

    public void AddUser(string name, string phoneNumber, List<string> visits, out bool added)
    {
        if(IsContains(phoneNumber))
        {
            added = false;
            Debug.LogWarning($"user {phoneNumber} already exist in db");
            return;
        }

        User newUser = new User(name, phoneNumber, visits);
        users.Add(newUser);
        added = true;
    }
}
