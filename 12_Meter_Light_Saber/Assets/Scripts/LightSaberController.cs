using UnityEngine;
using System.Collections;

public class LightSaberController : MonoBehaviour
{
    public LightSaber[] sabers;

    private Camera cam;

    private Vector2 aimingDir = Vector2.up;
    private float aimingAngle;

    
    private float lastTrigger = 0.0f;
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

        if (aimingDir.magnitude > 0.0f)
        {
            aimingAngle = Mathf.Deg2Rad * Vector2.Angle(Vector2.up, aimingDir.normalized);
            if (aimingDir.x < 0) aimingAngle = -aimingAngle;
        }

        bool swinging = false;
        for (int i = 0; i < sabers.Length; i++)
        {
            swinging = swinging || sabers[i].Swinging;
        }

        if (!swinging && (Input.GetButtonDown("Fire1") || Input.GetAxisRaw("Fire1") - lastTrigger > 0.2f))
        {
            for (int i = 0; i < sabers.Length; i++)
            {
                sabers[i].Swing(aimingAngle);
            }
        }

        lastTrigger = Input.GetAxisRaw("Fire1");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Vector3 dir = aimingDir;
        Gizmos.DrawLine(transform.position, transform.position + dir.normalized);
    }
}
