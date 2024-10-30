using UnityEngine;

namespace float_oat.Desktop90.Examples
{
    public class CameraAdjuster : MonoBehaviour
    {
        [SerializeField] private Camera cameraToAdjust = default;

        public void DecreaseFOV()
        {
            if (cameraToAdjust.fieldOfView > 15)
            {
                cameraToAdjust.fieldOfView -= 15f;
            }
        }

        public void IncreaseFOV()
        {
            if (cameraToAdjust.fieldOfView < 180)
            {
                cameraToAdjust.fieldOfView += 15f;
            }
        }
    }
}
