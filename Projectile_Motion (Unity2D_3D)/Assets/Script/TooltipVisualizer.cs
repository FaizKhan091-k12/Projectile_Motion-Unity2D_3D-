using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using UnityEngine.UI.ProceduralImage;

public class TooltipVisualizer : MonoBehaviour
{
   [SerializeField] private ProceduralImage infoIcon;
   [Header("Summary Panel References")]
   [SerializeField] RectTransform content_panel;
   [SerializeField] GameObject summaryPanel;
   [SerializeField] GameObject text_Panel; 
   [SerializeField] float popDelay;
  
   
   [Header("UI Text References")]
   [SerializeField] TextMeshProUGUI tittleText;
   [SerializeField] TextMeshProUGUI contentText;
   [SerializeField] String massText;
   [SerializeField] String gravityText;
   [SerializeField] String diameterText;
   [SerializeField] String angleText;
   [SerializeField] String speedText;
   [SerializeField] String altitudeText;
   [SerializeField] String heightText;
   [SerializeField] TextAsset learningText;
   
   [Header("UI Buttons References")]
   [SerializeField] Button massToolButton;
   [SerializeField] Button gravityToolButton;
   [SerializeField] Button diameterToolButton;
   [SerializeField] Button angleToolButton;
   [SerializeField] Button speedToolButton;
   [SerializeField] Button altitudeToolButton;
   [SerializeField] Button heightToolButton;
   [SerializeField] Button closePanelButton;
   
   [Header("Summary Panel ")]
   [SerializeField] Button learningButton;
   

   private void Start()
   {
      StartCoroutine(InfoIcon());
      PanelPopper();
      summaryPanel.SetActive(false);
      massToolButton.onClick.AddListener(delegate
      {
         tittleText.text = "Mass";
         contentText.text = massText; 
         PanelPopper();
      });
      gravityToolButton.onClick.AddListener(delegate{tittleText.text = "Gravity"; contentText.text = gravityText;PanelPopper();});
      diameterToolButton.onClick.AddListener(delegate{tittleText.text = "Diameter"; contentText.text = diameterText;PanelPopper();});
      angleToolButton.onClick.AddListener(delegate{tittleText.text = "Angle"; contentText.text = angleText;PanelPopper();});
      speedToolButton.onClick.AddListener(delegate{tittleText.text = "Speed"; contentText.text = speedText;PanelPopper();});
      altitudeToolButton.onClick.AddListener(delegate{tittleText.text = "Altitude"; contentText.text = altitudeText;PanelPopper();});
      heightToolButton.onClick.AddListener(delegate{tittleText.text = "Height"; contentText.text = heightText;PanelPopper();});
      closePanelButton.onClick.AddListener(delegate
      { 
         text_Panel.transform.localScale = Vector3.one;
         text_Panel.transform.DOScale(Vector3.zero, .2f).SetEase(Ease.OutBack);
         summaryPanel.SetActive(false);
         
      });
      learningButton.onClick.AddListener(delegate
      { tittleText.text = "Learning";
         contentText.text = learningText.text; 
         PanelPopper();
         
      });
   }


   IEnumerator InfoIcon()
   {
      float duration = 2f; // Full 0→4→0 cycle takes 2 seconds

      while (true)
      {
         float t = Mathf.PingPong(Time.time, duration / 2f) / (duration / 2f);
         infoIcon.FalloffDistance = Mathf.Lerp(0, 4, t);
         yield return null;
      }
   }

   
   private void PanelPopper()
   {
      summaryPanel.SetActive(true);
      text_Panel.transform.localScale = Vector3.zero;
      text_Panel.transform.DOScale(new Vector3(1f, 1f, 1f), popDelay).SetEase(Ease.OutBack);
      ScrollToTop();
   }
   public void ScrollToTop()
   {
       StartCoroutine(SmoothScrollToTop());
   }
   
   private IEnumerator SmoothScrollToTop()
   {
      float duration = 0.5f;
      float elapsed = 0f;

      // Wait one frame to ensure content_panel is updated if it just changed
      yield return null;

      Vector3 startPos = content_panel.transform.localPosition;
      Vector3 targetPos = new Vector3(startPos.x, 0f, startPos.z);

      while (elapsed < duration)
      {
         elapsed += Time.deltaTime;
         float t = elapsed / duration;

         content_panel.transform.localPosition = Vector3.Lerp(startPos, targetPos, t);
         yield return null;
      }

      content_panel.transform.localPosition = targetPos;
   }


}
