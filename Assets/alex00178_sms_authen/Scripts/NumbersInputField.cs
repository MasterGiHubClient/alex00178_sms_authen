using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class NumbersInputField : MonoBehaviour
{
    [System.Serializable]
    public class NumberCell
    {
        const string defaultValue = "-";

        public bool IsEmpty
        {
            get => string.Equals(textComponent.text, defaultValue);
        }

        public int value;
        public Text textComponent;
        public GameObject go;

        public void ResetValue()
        {
            value = -1;
            textComponent.text = defaultValue;
        }

        public void SetValue(int _value)
        {
            go.GetComponent<Animation>().Play();

            if(_value < 0)
            {
                ResetValue();
                return;
            }

            value = _value;
            textComponent.text = value.ToString();
        }
    }

    bool IsFilled
    {
        get => currentCellID == 5;
    }

    bool IsEmpty
    {
        get => currentCellID == -1;
    }

    int currentCellID;
    [HideInInspector]
    public string inputCode;

    [SerializeField] GameObject pressPrefab;
    [SerializeField] Transform pressParent;

    [Space(10)]
    [SerializeField] NumberCell[] numberCells;

    private void OnEnable()

    {
        currentCellID = -1;
        foreach (NumberCell numberCell in numberCells)
        {
            numberCell.ResetValue();
            numberCell.go.SetActive(false);
        }

        for (int i = 0; i < AuthManager.code.Length; i++)
        {
            numberCells[i].go.SetActive(true);
        }
    }

    public void GetValue(int value)
    {
        int lenght = AuthManager.code.Length;

        for(int i = 0; i < lenght;)
        {
            if (currentCellID < -1)
            {
                currentCellID = -1;
            }

            if (currentCellID > lenght - 1)
            {
                currentCellID = lenght - 1;
            }

            if(value > 0)
            {
                if(IsFilled)
                {
                    return;
                }

                currentCellID++;
                numberCells[currentCellID].SetValue(value);
                Instantiate(pressPrefab, EventSystem.current.currentSelectedGameObject.transform.position, Quaternion.Euler(Vector3.zero), pressParent);
            }
            else
            {
                if(IsEmpty)
                {
                    return;
                }

                numberCells[currentCellID].SetValue(value);
                currentCellID--;
            }

            inputCode = string.Join("", numberCells.Where(n => n.go.activeSelf).Select(i => i.value));
            return;
        }
    }
}
