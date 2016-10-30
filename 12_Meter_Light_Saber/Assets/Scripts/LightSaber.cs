using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))]
public class LightSaber : MonoBehaviour
{
    public float length = 12;
    public float angle = 0;
    public float offset = 0;
    public float width = 0.2f;

    public float upperAngle = Mathf.PI / 4;
    public float lowerAngle = Mathf.PI / 4;

    public float swingDuration = 0.2f;

    public bool Swinging { get; private set; }

    private LineRenderer line;
    private int layerMask = 0;

    void OnEnable()
    {
        line = GetComponent<LineRenderer>();
        line.enabled = false;
        layerMask = ~LayerMask.GetMask("Player");
    }

    private float startAngle, endAngle, swingTime;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Swinging)
        {
            swingTime += Time.fixedDeltaTime;

            if (swingTime < swingDuration)
            {
                float t = swingTime / swingDuration;
                angle = Mathf.Lerp(startAngle, endAngle, t);
            }
            else
            {
                Swinging = false;
                HideSaber();
            }
        }

        if (line.enabled)
        {
            UpdateSaberLine(true);
        }
    }

    void UpdateSaberLine(bool collisionTest = false)
    {
        Vector3 start = transform.position;
        Vector3 tip = transform.position;

        start.x = start.x + Mathf.Sin(angle) * offset;
        start.y = start.y + Mathf.Cos(angle) * offset;

        line.SetPosition(0, start);

        tip.x = tip.x + Mathf.Sin(angle) * (offset + length);
        tip.y = tip.y + Mathf.Cos(angle) * (offset + length);

        line.SetPosition(1, tip);
        line.SetWidth(width, width);

        if (collisionTest)
        {
            Vector2 dir = (tip - start).normalized;
            Vector2 rDir = new Vector2(dir.y, -dir.x);
            float halfWidth = width * 0.5f;
            Collider2D hitted = null;
            {
                RaycastHit2D hit = Physics2D.Raycast((Vector2)start + rDir * halfWidth, dir, length, layerMask);
                if (hit.collider)
                {
                    hitted = hit.collider;
                }
            }
            {
                RaycastHit2D hit = Physics2D.Raycast((Vector2)start - rDir * halfWidth, dir, length, layerMask);
                if (hit.collider)
                {
                    hitted = hit.collider;
                }
            }
            if (hitted)
            {
                hitted.SendMessage("OnSaberCollided", this, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    public void HideSaber()
    {
        line.enabled = false;
    }

    public void ShowSaber()
    {
        line.enabled = true;
    }

    public void Swing(float aimingAngle)
    {
        if (aimingAngle > 0.0f)
        {
            startAngle = aimingAngle - upperAngle;
            endAngle = aimingAngle + lowerAngle;
        }
        else
        {
            startAngle = aimingAngle + upperAngle;
            endAngle = aimingAngle - lowerAngle;
        }
        angle = startAngle;
        swingTime = 0.0f;
        Swinging = true;
        UpdateSaberLine();
        ShowSaber();
    }

}
