using UnityEngine;

public class MaterialAlphaFader : MonoBehaviour
{
   
    [Header("Material and Fade Range")]
    [SerializeField] private Material targetMaterial;
    [SerializeField] private float fromAlpha = 0f;
    [SerializeField] private float toAlpha = 1f;

    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private bool loop = true;

    private float timer = 0f;
    private bool forward = true;

  
    
    void Start()
    {
           targetMaterial = gameObject.GetComponent<MeshRenderer>().material;
            if (targetMaterial == null)
            {
                Debug.LogError("No material assigned.");
                return;
            }

            SetMaterialTransparent();

            Color color = targetMaterial.color;
            color.a = fromAlpha;
            targetMaterial.color = color;
        
       
    }

    void Update()
    {
        if (targetMaterial == null) return;

            timer += Time.deltaTime;
            float t = timer / fadeDuration;

            if (t > 1f)
            {
                if (loop)
                {
                    // Flip direction
                    forward = !forward;
                    timer = 0f;
                }
                else
                {
                    t = 1f;
                }
            }

            float alpha = Mathf.Lerp(forward ? fromAlpha : toAlpha, forward ? toAlpha : fromAlpha, t);

            Color color = targetMaterial.color;
            color.a = alpha;
            targetMaterial.color = color;
        
       
    }

    private void SetMaterialTransparent()
    {
        // Make sure material is set to transparent mode (for legacy Standard Shader)
        targetMaterial.SetFloat("_Mode", 2);
        targetMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        targetMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        targetMaterial.SetInt("_ZWrite", 0);
        targetMaterial.DisableKeyword("_ALPHATEST_ON");
        targetMaterial.EnableKeyword("_ALPHABLEND_ON");
        targetMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        targetMaterial.renderQueue = 3000;
    }
}
