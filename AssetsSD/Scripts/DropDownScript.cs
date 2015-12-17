using UnityEngine;
using System.Collections;
using System;

public class DropDownScript : MonoBehaviour
{
    public Vector2 speed = new Vector2(0, 8);

    /// <summary>
    /// Moving direction
    /// </summary>
    public Vector2 direction = new Vector2(0, -1);

    private Vector2 movement;

    private bool isMoving;
    private float destinationY;

    void Update()
    {
        // 2 - Movement
        if (!isMoving) return;
        movement = new Vector3(
            speed.x * direction.x,
            speed.y * direction.y, 0);
        movement *= Time.deltaTime;

        if (Math.Abs(transform.localPosition.y - destinationY) < Math.Abs(movement.y))
        {
            movement.y = transform.localPosition.y - destinationY;
            isMoving = false;
        }
        transform.Translate(movement);
    }

    public void MoveTo(float y)
    {
        destinationY = y;
        isMoving = true;
        
    }

}
