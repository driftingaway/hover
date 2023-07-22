using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EnableShader : MonoBehaviour
{
    ScriptableRendererFeature renderFeature;
    // Start is called before the first frame update
    void Start()
    {
        UniversalRenderPipelineUtils.SetRendererFeatureActive("Bozo", true);
        renderFeature = UniversalRenderPipelineUtils.GetRendererFeature("Blit");
        print(renderFeature);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
