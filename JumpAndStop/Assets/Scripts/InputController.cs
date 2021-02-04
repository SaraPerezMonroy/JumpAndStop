using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    private Rigidbody2D rb;

    //Multiplicador de la potencia del impulso
    public float forceImpulser = 1.0f;
    //Se almacena la información del inicio del click/tap y al soltarlo
    private Vector3 clickOrigin;
    private Vector3 clickDestiny;
    //Almacena información del touch para móviles
    private Touch infoTouch;

    Vector3[] calculatedPoints;
    private List<GameObject> visualPath = new List<GameObject>();
    public GameObject pathDot;
    public int calculatePoints = 250;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

        #if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
        {
            clickOrigin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            rb.velocity = Vector2.zero;
            Time.timeScale = 0.001f;
        }
        
        if (Input.GetMouseButton(0))
        {
            calculatedPoints = Plot(rb, transform.position, clickOrigin - Camera.main.ScreenToWorldPoint(Input.mousePosition), calculatePoints);
            DrawPoints();
        }

        if (Input.GetMouseButtonUp(0))
        {
            clickDestiny = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 forceDirection = clickOrigin - clickDestiny;
            ApplyForce(forceDirection);
        }
#elif UNITY_IOS || UNITY_ANDROID
        if(Input.touchCount > 0)
        {
            infoTouch = Input.GetTouch(0);
            Time.timeScale = 0.001f;

        }
        else
        {
            AppyForce(Touch.deltaPosition);
            infoTouch = null;
        }

#endif


    }

    void ApplyForce(Vector2 direction)
    {

        rb.velocity = Vector2.zero;
        rb.AddForce(direction * forceImpulser, ForceMode2D.Impulse);
        Time.timeScale = 1.0f;
        ClearVisualPath();
    }



    void ApplyForce()
    {
        Vector3 forceDirection = clickOrigin - clickDestiny;
        rb.velocity = Vector2.zero;
        rb.AddForce(forceDirection * forceImpulser, ForceMode2D.Impulse);
        Time.timeScale = 1.0f;
    }

    public static Vector3[] Plot(Rigidbody2D rigidbody, Vector2 pos, Vector2 velocity, int steps)
    {
        
        Vector3[] results = new Vector3[steps];

        float timestep = Time.fixedDeltaTime / Physics2D.velocityIterations;
        Vector2 gravityAccel = Physics2D.gravity * rigidbody.gravityScale * timestep * timestep;
        float drag = 1f - timestep * rigidbody.drag;
        Vector2 moveStep = velocity * timestep;

        for (int i = 0; i < steps; ++i)
        {
            moveStep += gravityAccel;
            moveStep *= drag;
            pos += moveStep;
            results[i] = pos;
        }

        Debug.Log(results.ToString());
        return results;
    }

    void DrawPoints()
    {
        ClearVisualPath();

        for (int i = 0; i < calculatedPoints.Length; i++)
        {
            visualPath.Add(Instantiate(pathDot, calculatedPoints[i], Quaternion.identity));
            Color newColor = visualPath[i].GetComponent<SpriteRenderer>().color;
            newColor.a = (float)((float)(calculatedPoints.Length - i) / (float)calculatedPoints.Length);
            visualPath[i].GetComponent<SpriteRenderer>().color = newColor;
        }
    }

    void ClearVisualPath()
    {
        for (int i = 0; i < visualPath.Count; i++)
        {
            Destroy(visualPath[i]);
        }

        visualPath.Clear();

    }
}
