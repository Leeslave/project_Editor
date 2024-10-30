using UnityEngine;

namespace float_oat.Desktop90.Examples
{
    public class ExamplePlayerControl : MonoBehaviour
    {
        public void Jump()
        {
            GetComponent<Rigidbody>().AddForce(Vector3.up * 50f);
        }
    }
}
