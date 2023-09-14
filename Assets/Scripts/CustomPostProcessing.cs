using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CustomPostProcessing : MonoBehaviour
{
    public Shader postProcessShader;
    private Material _postProcessMaterial;

    private RenderTexture _intermediateTex;
    private RenderTexture _prevTex;

    private void Start()
    {
        _intermediateTex = new RenderTexture(Screen.width, Screen.height, 24);
        _prevTex = new RenderTexture(Screen.width, Screen.height, 24);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (postProcessShader == null)
        {
            Graphics.Blit(source, destination);
            return;
        }

        if (_postProcessMaterial == null)
        {
            _postProcessMaterial = new Material(postProcessShader);
        }

        _postProcessMaterial.SetTexture("_PrevTex", _prevTex);
        Graphics.Blit(source, _intermediateTex, _postProcessMaterial);
        Graphics.Blit(_intermediateTex, destination);

        Graphics.Blit(_intermediateTex, _prevTex);
    }

    private void OnDisable()
    {
        if (_postProcessMaterial)
        {
            DestroyImmediate(_postProcessMaterial);
        }

        DestroyTextures(_intermediateTex, _prevTex);
    }

    private void DestroyTextures(params RenderTexture[] textures)
    {
        foreach (var texture in textures)
        {
            texture.Release();
            DestroyImmediate(texture);
        }
    }
}