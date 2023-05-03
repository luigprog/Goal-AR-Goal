using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

/// <summary>
/// Virtual stick extension.
/// </summary>
public class GuiCircleMovement : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IDragHandler
{
    public static GuiCircleMovement instance;
    private Vector2 direction;
    private Vector3 direction3d;

    private void Awake()
    {
        instance = this;
        direction = direction3d = Vector3.zero;
    }

    public void OnDrag(PointerEventData data)
    {
        direction.x = data.position.x - transform.position.x;
        direction.y = data.position.y - transform.position.y;
        direction.Normalize();

        direction3d.x = direction.x;
        direction3d.y = 0.0f;
        direction3d.z = direction.y;
    }

    public void OnPointerUp(PointerEventData data)
    {
        direction = direction3d = Vector3.zero;
    }

    public void OnPointerDown(PointerEventData data)
    {
        direction.x = data.position.x - transform.position.x;
        direction.y = data.position.y - transform.position.y;
        direction.Normalize();

        direction3d.x = direction.x;
        direction3d.y = 0.0f;
        direction3d.z = direction.y;
    }

    public Vector3 GetMovementDirection()
    {
        return direction3d;
    }
}