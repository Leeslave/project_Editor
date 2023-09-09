using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectM_DG : MonoBehaviour
{
    [SerializeField] GameObject Dust;

    private void Awake()
    {
        int s, k;
        float y;
        GameObject z;
        for(int i = 0; i < 70; i++)
        {
            s = Random.Range(0, 4);
            k = Random.Range(-13, 13);
            y = Random.Range(6f, 9f);
            z = Instantiate(Dust, transform);
            switch (s)
            {
                case 0:
                    z.GetComponent<Rigidbody2D>().AddForce(Vector2.right * Random.Range(0.1f,1f), ForceMode2D.Impulse);
                    z.transform.position = new Vector2(k,y * -1);
                    break;
                case 1:
                    z.GetComponent<Rigidbody2D>().AddForce(Vector2.left * Random.Range(0.1f, 1f), ForceMode2D.Impulse);
                    z.transform.position = new Vector2(k, y * -1);
                    break;
                case 2:
                    z.GetComponent<Rigidbody2D>().AddForce(Vector2.right * Random.Range(0.1f, 1f), ForceMode2D.Impulse);
                    z.transform.position = new Vector2(k, y);
                    break;
                case 3:
                    z.GetComponent<Rigidbody2D>().AddForce(Vector2.left * Random.Range(0.1f, 1f), ForceMode2D.Impulse);
                    z.transform.position = new Vector2(k, y);
                    break;
            }
        }
    }
}
