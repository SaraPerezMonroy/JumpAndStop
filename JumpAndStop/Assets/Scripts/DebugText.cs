using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugText : MonoBehaviour
{
    public static DebugText instance;
    public Text labelText;
    public Transform transformScrollView;
    public GameObject textPrefab;
    Color defaultColor = Color.black;

    public bool isDebugging = true;
    // Start is called before the first frame update
    void Awake()
    {
        if (DebugText.instance == null)
        {
            Init();
            
        }
        else
        {
            Destroy(this);
        }
    }

    private void Init()
    {
        instance = this;
        gameObject.SetActive(isDebugging);
    }

    public IEnumerator PrintText(string textToShow, float floatTime)
    {
        StartCoroutine(PrintText(textToShow, floatTime, Color.white));
        yield return new WaitForSeconds(0.0f);
    }

    public IEnumerator PrintText(string textToShow, float floatTime, Color color)
    {
        GameObject newText = Instantiate(textPrefab, transformScrollView);
        newText.GetComponent<Text>().text = textToShow;
        newText.GetComponent<Text>().color = color;
        newText.SetActive(true);

        yield return new WaitForSeconds(floatTime);

        Destroy(newText);
    }
}
