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
    public int calculatePoints = 5;
    public bool calculatePathWithSteps = false;

    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        rb = GetComponent<Rigidbody2D>();

        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                forceImpulser = 20f;
                break;
            case RuntimePlatform.WindowsEditor:
                forceImpulser = 1000f;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        PlayingOnPC();

        TouchDeviceControl();
    }

    void TouchDeviceControl()
    {
        if (Input.touchCount == 1)
        {
            Time.timeScale = 0.001f;
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                StartCoroutine(DebugText.instance.PrintText("Start Touch", 1.0f, Color.green));
                clickOrigin = Input.GetTouch(0).position;
                clickDestiny = Input.GetTouch(0).position;
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                clickDestiny = Input.GetTouch(0).position;
                calculatedPoints = Plot(rb, transform.position, clickOrigin - clickDestiny, calculatePoints);
                DrawPoints();
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                clickDestiny = Input.GetTouch(0).position;
                StartCoroutine(DebugText.instance.PrintText("ENDED Touch", 1.0f, Color.blue));
                ApplyForce(clickOrigin - clickDestiny);
            }
        }
    }
    void PlayingOnPC()
    {
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

    }


    /// <summary>
    /// Aplica una fuerza en la direccion dada así como vuelve el tiempo a la normalidad.
    /// </summary>
    /// <param name="direction"></param>
    void ApplyForce(Vector2 direction)
    {
        direction = new Vector2(direction.x / Screen.width, direction.y / Screen.height) * forceImpulser;
        StartCoroutine(DebugText.instance.PrintText("Add force " + direction, 1.0f, Color.red));
        rb.velocity = Vector2.zero;
        rb.AddForce(direction, ForceMode2D.Impulse);
        Time.timeScale = 1.0f;
        ClearVisualPath();
    }

    /// <summary>
    /// Función que calcula los puntos de la parábola que hará un rigidbopdy dada una posición y una velocidad.
    /// </summary>
    /// <param name="rigidbody"></param>
    /// <param name="pos"></param>
    /// <param name="velocity"></param>
    /// <param name="steps"></param>
    /// <returns></returns>
    public Vector3[] Plot(Rigidbody2D rigidbody, Vector2 pos, Vector2 velocity, int steps)
    {
        velocity = new Vector2(velocity.x / Screen.width, velocity.y / Screen.height) * forceImpulser;
        Vector3[] results = new Vector3[steps];
        float timestep = 0.15f;
        if(calculatePathWithSteps)
        {
            timestep = Time.fixedDeltaTime / Physics2D.velocityIterations;
        }
        

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

        return results;
    }
    /// <summary>
    /// Encargada de dibujar la parábola del tiro del jugador
    /// </summary>
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
    /// <summary>
    /// Elimina los puntos de la parábola
    /// </summary>
    void ClearVisualPath()
    {
        for (int i = 0; i < visualPath.Count; i++)
        {
            Destroy(visualPath[i]);
        }

        visualPath.Clear();

    }
}
