using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InitialValues : MonoBehaviour
{
   [Header("Initial Values")]
   [SerializeField] TextMeshProUGUI heightText;
   [SerializeField] TextMeshProUGUI angleText;
   [SerializeField] TextMeshProUGUI speedText;
   [SerializeField] TextMeshProUGUI airResistenceText;

   [Header("UI References")]
   [SerializeField] Slider heightSlider;
   [SerializeField] Slider angleSlider;
   [SerializeField] Slider speedSlider;
   [SerializeField] Toggle airResistenceToggle;

   [Header("Panel Controls")] 
   [SerializeField] RectTransform initialValuesPanel; 
   [SerializeField] float mix_Y, max_Y;
   [SerializeField] Button closeButton;
   [SerializeField] EventTrigger moveButton;
   [SerializeField] TextMeshProUGUI tittleText;
   [SerializeField] private TextMeshProUGUI[] texttominimise;
   [Space(5)] [SerializeField] private bool isMinimized;
   [SerializeField] Sprite close_Sprite,minimized_Sprite;
   
   private void Start()
   {
      InitializeValues();
      CloseButtonClicked();
      heightSlider.onValueChanged.AddListener(delegate { heightText.text = heightSlider.value + "m"; });
      angleSlider.onValueChanged.AddListener(delegate { angleText.text = angleSlider.value + "<sup>o</sup>"; });
      speedSlider.onValueChanged.AddListener(delegate { speedText.text = speedSlider.value.ToString("0.0") + "m/s"; });
      airResistenceToggle.onValueChanged.AddListener(delegate
      {
         airResistenceText.color = airResistenceToggle.isOn ? Color.green : Color.red;
         airResistenceText.text = airResistenceToggle.isOn ? "ON" : "OFF";
      });

     
   }

   
   
   private void CloseButtonClicked()
   {
      closeButton.onClick.AddListener(delegate
      {
         isMinimized = !isMinimized;
         if (isMinimized)
         {
            closeButton.image.sprite = minimized_Sprite;
            Vector2 newPos = initialValuesPanel.sizeDelta;
            newPos.y = mix_Y;
            initialValuesPanel.sizeDelta = newPos;

            foreach (TextMeshProUGUI text  in texttominimise)
            {
               text.gameObject.SetActive(false);
            }
         }
         else
         {
            closeButton.image.sprite = close_Sprite;
            Vector2 newPos = initialValuesPanel.sizeDelta;
            newPos.y = max_Y;
            initialValuesPanel.sizeDelta = newPos;
            foreach (TextMeshProUGUI text  in texttominimise)
            {
               text.gameObject.SetActive(true);
            }
         }
       
         
      });
   }

   public void InitializeValues()
   {
      heightText.text = heightSlider.value + "m";
      angleText.text = angleSlider.value + "<sup>o</sup>";
      speedText.text = speedSlider.value.ToString("0.0") + "m/s";
      airResistenceText.color = airResistenceToggle.isOn ? Color.green : Color.red;
      airResistenceText.text = airResistenceToggle.isOn ? "ON" : "OFF";
   }


}
