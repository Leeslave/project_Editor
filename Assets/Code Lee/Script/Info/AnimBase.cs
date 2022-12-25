using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AnimBase : MonoBehaviour {
    public UnityEngine.UI.Text GUITextCtrl;

    // Use this for initialization
    void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

    public virtual IEnumerator PlayAnim() { yield return 0; }
}
