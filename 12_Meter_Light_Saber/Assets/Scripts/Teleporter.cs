using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class Teleporter : MonoBehaviour
{
    private BoxCollider2D box = null;
    
    private BoxCollider2D Box
    {
        get
        {
            if (!box)
                box = GetComponent<BoxCollider2D>();
            return box;
        }
    }

    public void Teleport(Vector2 direction, float distance, float searchStep)
    {
        var min = Box.bounds.min;
        var size = Box.bounds.size;
        
        for (; distance > 0.0f; distance -= searchStep)
        {
            var newMin = (Vector2)min + direction * distance;
            {
                RaycastHit2D h = Physics2D.Raycast(newMin, Vector2.right, size.x);
                if (h.collider) continue;
            }
            {
                RaycastHit2D h = Physics2D.Raycast(newMin + Vector2.right * size.x, Vector2.up, size.y);
                if (h.collider) continue;
            }
            {
                RaycastHit2D h = Physics2D.Raycast(newMin + (Vector2)size, Vector2.left, size.x);
                if (h.collider) continue;
            }
            {
                RaycastHit2D h = Physics2D.Raycast(newMin + Vector2.up * size.y, Vector2.down, size.y);
                if (h.collider) continue;
            }

            print("found!");
            var newPos = newMin + (Vector2)Box.bounds.extents - Box.offset;
            transform.position = newPos;
            break;
        }

        print("End!");
    }
}
