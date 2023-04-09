using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullScreenShader : MonoBehaviour {
    public Shader _shader;
    public Material material;
     
    void Start()
    {
        material = new Material(_shader);
    }
    
    private void OnRenderImage(RenderTexture src, RenderTexture dest) {
        print("yea");
        Graphics.Blit(src, dest, material);
    }
}
