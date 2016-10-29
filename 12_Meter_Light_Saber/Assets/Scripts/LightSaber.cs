using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))]
public class LightSaber : MonoBehaviour
{
    public float length = 12;
    public float angle = 0;
    public float offset = 0;
    
    public float upperAngle = Mathf.PI / 4;
    public float lowerAngle = Mathf.PI / 4;

    public float prepareDuration = 0.1f;
    public float swingDuration = 0.2f;
    public float frozenDuration = 0.1f;

    public bool Swinging { get; private set; }

    private LineRenderer line;

    void OnEnable()
    {
        line = GetComponent<LineRenderer>();
        line.enabled = false;
    }
    
    private float startAngle, endAngle, swingTime;
    
    // Update is called once per frame
    void FixedUpdate()
    {
        if (Swinging)
        {
            swingTime += Time.fixedDeltaTime;

            if (swingTime < prepareDuration)
            {
                angle = startAngle;
            }
            else if (swingTime < prepareDuration + swingDuration)
            {
                float t = swingTime / swingDuration;
                angle = Mathf.Lerp(startAngle, endAngle, t);
            }
            else if (swingTime < prepareDuration + swingDuration + frozenDuration)
            {
                angle = endAngle;
            }
            else
            {
                Swinging = false;
            }
        }

        line.enabled = Swinging;
        if (Swinging)
        {
            Vector3 start = transform.position;
            Vector3 tip = transform.position;

            start.x = start.x + Mathf.Sin(angle) * offset;
            start.y = start.y + Mathf.Cos(angle) * offset;

            line.SetPosition(0, start);

            tip.x = tip.x + Mathf.Sin(angle) * (offset + length);
            tip.y = tip.y + Mathf.Cos(angle) * (offset + length);

            line.SetPosition(1, tip);
        }
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
        swingTime = 0.0f;
        Swinging = true;
    }
}
