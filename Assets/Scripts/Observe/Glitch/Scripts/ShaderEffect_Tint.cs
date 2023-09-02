using System.Collections;
using Unity.Burst.CompilerServices;
using UnityEngine;

[ExecuteInEditMode]
public class ShaderEffect_Tint : MonoBehaviour {

	public float y = 1;
	public float u = 1;
	public float v = 1;
//	public bool swapUV = false;
	private Material material;

	// Creates a private material used to the effect
	void Awake ()
	{
		material = new Material( Shader.Find("Hidden/Tint") );
        StartCoroutine(Test());
    }

    IEnumerator Test()
	{
		while (true)
		{
            y = Random.Range(0, 1f);
			u = Random.Range(0, 1f);
			v = Random.Range(0, 1f);
			
            yield return new WaitForSeconds(0.3f);
        }
    }

    // Postprocess the image
    void OnRenderImage (RenderTexture source, RenderTexture destination)
	{

		material.SetFloat("_ValueX", y);
		material.SetFloat("_ValueY", u);
		material.SetFloat("_ValueZ", v);

		Graphics.Blit (source, destination, material);
	}
}
