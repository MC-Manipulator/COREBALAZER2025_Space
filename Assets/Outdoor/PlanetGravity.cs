using UnityEngine;

public class PlanetGravity : MonoBehaviour
{
    public float gravityForce = 9.8f; // 引力强度
    public float orbitRadius = 1f;   // 轨道判定半径

    public bool isActiveGravity = true;


    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Spaceship"))
        {
            SpaceshipController ship = other.GetComponent<SpaceshipController>();
            if (ship.GetCurrentPlanet() == this)
            { // 只有当前主导星球生效
              // 施加引力的代码
                Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
                Vector2 direction = (transform.position - other.transform.position).normalized;
                rb.AddForce(direction * gravityForce * Time.deltaTime);
            }
        }
    }
}