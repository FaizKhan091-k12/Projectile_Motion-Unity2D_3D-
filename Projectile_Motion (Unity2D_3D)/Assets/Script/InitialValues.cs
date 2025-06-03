using System;
using TMPro;
using UnityEngine;
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


   private void Start()
   {
      InitializeValues();
      heightSlider.onValueChanged.AddListener(delegate { heightText.text = heightSlider.value + "m"; });
      angleSlider.onValueChanged.AddListener(delegate { angleText.text = angleSlider.value + "<sup>o</sup>"; });
      speedSlider.onValueChanged.AddListener(delegate { speedText.text = speedSlider.value.ToString("0.0") + "m/s"; });
      airResistenceToggle.onValueChanged.AddListener(delegate
      {
         airResistenceText.color = airResistenceToggle.isOn ? Color.green : Color.red;
         airResistenceText.text = airResistenceToggle.isOn ? "ON" : "OFF";
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
