using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class ShaderEffect_CorruptedVram : MonoBehaviour {

	public float shift = 10;
	private Texture texture;
	private Material material;

	void Awake ()
	{
		material = new Material( Shader.Find("Hidden/Distortion") );
		texture = Resources.Load<Texture>("Checkerboard-big");
		StartCoroutine(Test());
	}
	IEnumerator Test()
	{
		while (true)
		{
			shift = Random.Range(-100, 100);
			yield return new WaitForSeconds(0.1f);
		}
	}

    void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
		material.SetFloat("_ValueX", shift);
		material.SetTexture("_Texture", texture);
		Graphics.Blit (source, destination, material);
	}
}
