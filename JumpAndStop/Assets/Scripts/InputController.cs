using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    private Rigidbody2D rb;
    public float forceImpulser = 20.0f;
    private Vector3 clickOrigin;
    private Vector3 clickDestiny;
    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            clickOrigin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            rb.velocity = Vector2.zero;
            Time.timeScale = 0.001f;
        }

        if (Input.GetMouseButtonUp(0))
        {

            clickDestiny = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 forceDirection = clickOrigin - clickDestiny;
            
            Debug.Log("Add Force" + forceDirection * forceImpulser);
            rb.velocity = Vector2.zero;
            rb.AddForce(forceDirection  * forceImpulser, ForceMode2D.Impulse);
            Time.timeScale = 1.0f;
        }
    }
}
