using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ToggleController : MonoBehaviour
{
  [SerializeField] Toggle toggle;
  [SerializeField] Slider slider;
  [SerializeField] Image[] image;
  [SerializeField] TextMeshProUGUI[] text;
  [SerializeField] TextMeshProUGUI toggleText,initialValtextToggle;
  [SerializeField] private Color offColor, onColor;

  private void Start()
  {
    toggle.onValueChanged.AddListener(delegate { SliderController(); });
    SliderController();
  }


  public void SliderController()
  {
    if (toggle.isOn)
    {
      slider.interactable = true;
      toggle.transform.localScale = new Vector3(1, 1, 1);
      toggle.transform.GetChild(0).GetComponent<Image>().color = onColor;
      toggleText.text = "ON";
      initialValtextToggle.text = "ON";
      foreach (Image image in image)
      {
        Color color = image.color;
        color.a = 1f;
        image.color = color;
      }
      foreach (TextMeshProUGUI image in text)
      {
        Color color = image.color;
        color.a = 1f;
        image.color = color;
      }
    }
    else
    {
      slider.interactable = false;
      toggle.transform.localScale = new Vector3(-1, 1, 1);
      toggle.transform.GetChild(0).GetComponent<Image>().color = offColor;
      toggleText.text = "OFF";
      initialValtextToggle.text = "OFF";
      foreach (Image image in image)
      {
        Color color = image.color;
        color.a = .2f;
        image.color = color;
      }
      foreach (TextMeshProUGUI image in text)
      {
        Color color = image.color;
        color.a = .2f;
        image.color = color;
      }

    }
  }
  
}
