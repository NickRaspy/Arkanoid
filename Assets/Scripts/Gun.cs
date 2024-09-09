using UnityEngine;
using UnityEngine.UIElements;

public class Gun
{
    private float gunRotationSpeed;
    private Transform gunHolder;
    private Transform gunPoint;
    private Rigidbody bullet;

    public Gun(float gunRotationSpeed, Transform gunHolder, Transform gunPoint, Rigidbody bullet)
    {
        this.gunRotationSpeed = gunRotationSpeed;
        this.gunHolder = gunHolder;
        this.gunPoint = gunPoint;
        this.bullet = bullet;
    }

    public void Update()
    {
        float angle = CalculateAngle();
        gunHolder.rotation = Quaternion.Slerp(gunHolder.rotation, Quaternion.Euler(0f, 0f, angle), Time.deltaTime * gunRotationSpeed);
    }

    public void Shoot()
    {
        Rigidbody newBullet = Object.Instantiate(bullet, gunPoint.position, Quaternion.identity);
        newBullet.AddForce(gunHolder.up * 10f, ForceMode.Impulse);
    }

    float CalculateAngle()
    {
        Vector3 mPos = Input.mousePosition;
        Vector3 bPos = Camera.main.WorldToScreenPoint(gunHolder.position);
        Vector3 delta = mPos - bPos;
        return Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg - 90f;
    }
}
