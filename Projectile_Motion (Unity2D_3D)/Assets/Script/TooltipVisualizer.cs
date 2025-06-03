using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipVisualizer : MonoBehaviour
{
   [SerializeField] TextMeshProUGUI tittleText;
   [SerializeField] TextMeshProUGUI contentText;
   [SerializeField] String massText;
   [SerializeField] String gravityText;
   [SerializeField] String diameterText;
   [SerializeField] String angleText;
   [SerializeField] String speedText;
   [SerializeField] String altitudeText;
   [SerializeField] String heightText;
   
   [SerializeField] Button massToolButton;
   [SerializeField] Button gravityToolButton;
   [SerializeField] Button diameterToolButton;
   [SerializeField] Button angleToolButton;
   [SerializeField] Button speedToolButton;
   [SerializeField] Button altitudeToolButton;
   [SerializeField] Button heightToolButton;

   private void Start()
   {
      massToolButton.onClick.AddListener(delegate{tittleText.text = "Mass"; contentText.text = massText;});
      gravityToolButton.onClick.AddListener(delegate{tittleText.text = "Gravity"; contentText.text = gravityText;});
      diameterToolButton.onClick.AddListener(delegate{tittleText.text = "Diameter"; contentText.text = diameterText;});
      angleToolButton.onClick.AddListener(delegate{tittleText.text = "Angle"; contentText.text = angleText;});
      speedToolButton.onClick.AddListener(delegate{tittleText.text = "Speed"; contentText.text = speedText;});
      altitudeToolButton.onClick.AddListener(delegate{tittleText.text = "Altitude"; contentText.text = altitudeText;});
      heightToolButton.onClick.AddListener(delegate{tittleText.text = "Height"; contentText.text = heightText;});
   }
}
