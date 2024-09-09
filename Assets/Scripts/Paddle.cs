using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paddle
{
    private Transform transform;
    private float speed = 2.0f;
    private float movementWidth = 2.0f;
    
    public Paddle(Transform transform, float speed, float movementWidth)
    {
        this.transform = transform;
        this.speed = speed;
        this.movementWidth = movementWidth;
    }

    public void Update()
    {
        float input = Input.GetAxis("Horizontal");

        transform.Translate(input * speed * Time.deltaTime, 0f, 0f);
        transform.position = new(
            Mathf.Clamp(transform.position.x, -movementWidth / 2f, movementWidth / 2f),
            transform.position.y
            );
    }
    public void SetBallOnPaddle(Transform ball)
    {
        ball.parent = transform;
        ball.localPosition = new Vector3(0f, 1.5f, 0f);
    }
}
