using UnityEngine;

public class PhysicsWind : MonoBehaviour
{
    [SerializeField] private float windForce = 0.5f;
    [SerializeField] private float windChangeSpeed = 0.3f;
    
    private Rigidbody2D rb2D;  // Для 2D-объектов
    private Rigidbody rb3D;    // Для 3D-объектов
    private bool is2D;         // Определяет тип физики

    private void Start()
    {
        // Проверяем, какой тип Rigidbody используется
        rb2D = GetComponent<Rigidbody2D>();
        rb3D = GetComponent<Rigidbody>();
        
        is2D = (rb2D != null); // true, если объект 2D
    }

    private void FixedUpdate()
    {
        float windDirection = Mathf.Sin(Time.time * windChangeSpeed);
        Vector3 force = new Vector3(windDirection * windForce, 0, 0);
        
        if (is2D)
        {
            rb2D.AddForce(force); // Для 2D
        }
        else if (rb3D != null)
        {
            rb3D.AddForce(force); // Для 3D
        }
    }
}