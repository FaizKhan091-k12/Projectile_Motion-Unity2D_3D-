using UnityEngine;
using UnityEngine.UI;
public class ToggleController : MonoBehaviour
{
    [Header("Show Calculation Panel")]
    [SerializeField] private Toggle showpanelCalculations_Toggle;
    [SerializeField] private GameObject[] calcualtionPanel;
    void Start()
    {
       
        if(!showpanelCalculations_Toggle) return; 
        showpanelCalculations_Toggle.onValueChanged.AddListener(ShowCalculationsPanel);
        ShowCalculationsPanel(showpanelCalculations_Toggle.isOn);
    }

    void ShowCalculationsPanel(bool value)
    {
        if(calcualtionPanel.Length <= 0) return;
        for (int i = 0; i < calcualtionPanel.Length; i++)
        {
            calcualtionPanel[i].SetActive(false);
            if (value)
            {
                calcualtionPanel[0].SetActive(true);
            }
            else
            {
                calcualtionPanel[1].SetActive(true);
            }
           
            
        }
    }
}
