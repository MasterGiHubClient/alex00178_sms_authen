using UnityEngine;
using UnityEngine.UI;

public class VisitGO : MonoBehaviour
{
    [SerializeField] Text textComponent;

    public void SetVisit(string visitMsg)
    {
        textComponent.text = visitMsg;
    }
}
