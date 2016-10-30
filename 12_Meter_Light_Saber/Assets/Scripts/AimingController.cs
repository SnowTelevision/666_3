using UnityEngine;
using System.Collections;

public class AimingController : MonoBehaviour
{
    public Vector2 aimingDir = Vector2.up;
    public float aimingAngle;

    private Camera cam;
    
    void Update()
    {
        Vector2 stickDir = new Vector2(Input.GetAxis("RStickX"), Input.GetAxis("RStickY"));

        if (stickDir.magnitude > 0.0f)
        {
            aimingDir = stickDir;
        }
        else
        {
            if (!cam)
                cam = FindObjectOfType<Camera>();

            aimingDir = cam.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        }

        aimingDir.Normalize();

        if (aimingDir.magnitude > 0.0f)
        {
            aimingAngle = Mathf.Deg2Rad * Vector2.Angle(Vector2.up, aimingDir.normalized);
            if (aimingDir.x < 0) aimingAngle = -aimingAngle;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Vector3 dir = aimingDir;
        Gizmos.DrawLine(transform.position, transform.position + dir.normalized);
    }
}
