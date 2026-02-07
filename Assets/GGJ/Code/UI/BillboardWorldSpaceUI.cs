using UnityEngine;

namespace GGJ.Code.UI
{
    public class BillboardWorldSpaceUI : MonoBehaviour
    {
        [SerializeField]
        Camera targetCamera;

        [SerializeField]
        bool lockX;

        [SerializeField]
        bool lockY;

        [SerializeField]
        bool lockZ;

        void LateUpdate()
        {
            Camera cameraToUse = targetCamera ? targetCamera : Camera.main;
            if (!cameraToUse) return;

            Vector3 direction = transform.position - cameraToUse.transform.position;
            if (direction.sqrMagnitude < 0.0001f) return;

            Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up);
            Vector3 euler = lookRotation.eulerAngles;

            if (lockX) euler.x = transform.eulerAngles.x;
            if (lockY) euler.y = transform.eulerAngles.y;
            if (lockZ) euler.z = transform.eulerAngles.z;

            transform.rotation = Quaternion.Euler(euler);
        }
    }
}
