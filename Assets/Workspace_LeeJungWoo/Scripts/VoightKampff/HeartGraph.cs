using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartGraph : MonoBehaviour
{
    public GameObject[] heartGraph;
    public int bpm;
    
    private double currentTime;

    private void Start()
    {
        currentTime = 0d;
    }

    private void Update()
    {
        currentTime += Time.deltaTime;

        if (currentTime >= 60d / bpm)
        {
            GameObject target = heartGraph[Random.Range(0, heartGraph.Length)];
            GameObject one = Instantiate(target, this.gameObject.transform.position + new Vector3(110f,-3.5f,0), new Quaternion(0, 0, 0, 0), this.gameObject.transform);
            one.GetComponent<Rigidbody>().velocity = new Vector3(-30, 0, 0);
            Destroy(one, 6f);
            currentTime -= 60d / bpm;
        }
    }
}
