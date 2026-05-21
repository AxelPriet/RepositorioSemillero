using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class TransicionEscenas : MonoBehaviour
{
    public static TransicionEscenas Instance { get; private set; }

    [SerializeField] private float transitionTime = 1f;

    private Canvas canvas;
    private Image panelNegro;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        CrearPanelFade();
    }

    private void CrearPanelFade()
    {
        canvas = new GameObject("CanvasTransicion").AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999; 
        canvas.gameObject.AddComponent<CanvasScaler>();
        canvas.gameObject.AddComponent<GraphicRaycaster>();
        DontDestroyOnLoad(canvas.gameObject);

        panelNegro = new GameObject("PanelNegro").AddComponent<Image>();
        panelNegro.transform.SetParent(canvas.transform, false);
        panelNegro.color = new Color(0, 0, 0, 0);
        panelNegro.raycastTarget = false;

        RectTransform rect = panelNegro.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
    }

    public void CargarEscena(string nombreEscena)
    {
        StartCoroutine(Transicion(nombreEscena));
    }

    private IEnumerator Transicion(string nombreEscena)
    {
        yield return StartCoroutine(Fade(0f, 1f));

        SceneManager.LoadScene(nombreEscena, LoadSceneMode.Single);

        yield return new WaitForSeconds(0.1f);

        yield return StartCoroutine(Fade(1f, 0f));
    }

    private IEnumerator Fade(float desde, float hasta)
    {
        float tiempo = 0f;
        while (tiempo < transitionTime)
        {
            tiempo += Time.deltaTime;
            float alpha = Mathf.Lerp(desde, hasta, tiempo / transitionTime);
            panelNegro.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        panelNegro.color = new Color(0, 0, 0, hasta);
    }
}