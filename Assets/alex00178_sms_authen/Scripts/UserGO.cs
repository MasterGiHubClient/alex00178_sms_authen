using UnityEngine.UI;
using UnityEngine;

public class UserGO : MonoBehaviour
{
    bool expand;

    GameObject expandGO;

    [SerializeField] Text headTextComponent;
    [SerializeField] Button buttonComponent;

    [Space(10)]
    [SerializeField] Text visitsTextComponent;

    private void Start()
    {
        expand = false;
        expandGO = transform.GetChild(1).gameObject;
        expandGO.SetActive(expand);

        buttonComponent.onClick.AddListener(() =>
        {
            expand = !expand;
            expandGO.SetActive(expand);
        });
    }

    public void SetUserInfo(User user)
    {
        visitsTextComponent.text = string.Empty;
        headTextComponent.text = $"{user.phoneNumber}  {user.name}  {user.visits.Count} вход(ов)";

        for(int i = 0; i < user.visits.Count; i++)
        {
            if(i == user.visits.Count - 1)
            {
                visitsTextComponent.text += $"{user.visits[i]}";
            }
            else
            {
                visitsTextComponent.text += $"{user.visits[i]}\n";
            }
        }
    }
}
