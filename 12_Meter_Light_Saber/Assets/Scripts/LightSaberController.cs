using UnityEngine;
using System.Collections;

public class LightSaberController : MonoBehaviour
{
    public LightSaber[] sabers;

    public AimingController aiming;
    
    private float lastTrigger = 0.0f;
    void Update()
    {
        bool swinging = false;
        for (int i = 0; i < sabers.Length; i++)
        {
            swinging = swinging || sabers[i].Swinging;
        }

        if (!swinging && (Input.GetButtonDown("Fire1") || Input.GetAxisRaw("Fire1") - lastTrigger > 0.2f))
        {
            for (int i = 0; i < sabers.Length; i++)
            {
                sabers[i].Swing(aiming.aimingAngle);
            }
        }

        lastTrigger = Input.GetAxisRaw("Fire1");
    }


}
