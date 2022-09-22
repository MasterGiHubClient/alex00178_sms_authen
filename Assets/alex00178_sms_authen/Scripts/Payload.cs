[System.Serializable]
public class Payload
{
    public string message;
    public Committer committer;
    public string sha;
    public string content;

    public Payload(string message, Committer committer, string sha, string content)
    {
        this.message = message;
        this.committer = committer;
        this.sha = sha;
        this.content = content;
    }

    [System.Serializable]
    public class Committer
    {
        public string name;
        public string email;

        public Committer(string name, string email)
        {
            this.name = name;
            this.email = email;
        }
    }
}
