using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MiniGame_Dibujo : MonoBehaviour
{
    private class DatosFigura
    {
        public string nombre;
        public List<SpriteRenderer> cuadrados = new List<SpriteRenderer>();
        public bool terminada = false;
        public int pintados = 0;
        public float Progreso => pintados / (float)cuadrados.Count;
    }

    [Header("Grupos de figuras")]
    [SerializeField] private Transform grupoCirculo;
    [SerializeField] private Transform grupoCuadrado;
    [SerializeField] private Transform grupoTriangulo;

    [Header("Colores")]
    [SerializeField] private Color colorDefecto = Color.white;
    [SerializeField] private Color colorPintado = Color.black;

    [Header("Errores")]
    [SerializeField] private int erroresMaximos = 30;
    [SerializeField] private float cooldownError = 0.15f;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI textoErrores;
    [SerializeField] private TextMeshProUGUI textoProgreso;
    [SerializeField] private TextMeshProUGUI textoMensaje;

    [Header("Escena")]
    [SerializeField] private string nombreEscenaPrincipal = "SampleScene";
    private Camera camaraCanvas;

    private List<DatosFigura> figuras = new List<DatosFigura>();
    private Camera camara;
    private bool dibujando = false;
    private int erroresActuales = 0;
    private bool juegoTerminado = false;
    private float tiempoUltimoError = -999f;
    private bool sobreFigura = false;
    [SerializeField] private int minigameIndex;

    private void Start()
    {
        camara = Camera.main;

        RegistrarGrupo(grupoCirculo, "Círculo");
        RegistrarGrupo(grupoCuadrado, "Cuadrado");
        RegistrarGrupo(grupoTriangulo, "Triángulo");
        Canvas canvas = GetComponentInParent<Canvas>();
        camaraCanvas = (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                       ? null
                       : camara;

        ActualizarUI();
    }

    private void RegistrarGrupo(Transform grupo, string nombre)
    {
        if (grupo == null) return;

        DatosFigura d = new DatosFigura();
        d.nombre = nombre;

        foreach (Transform hijo in grupo)
        {
            SpriteRenderer sr = hijo.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = colorDefecto;
                d.cuadrados.Add(sr);
            }
        }

        figuras.Add(d);
    }
    private void Update()
    {
        if (juegoTerminado || camara == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame) dibujando = true;
        if (Mouse.current.leftButton.wasReleasedThisFrame) dibujando = false;

        if (dibujando) ProcesarDibujo();
    }
    private void ProcesarDibujo()
    {
        Vector2 posScreen = Mouse.current.position.ReadValue();
        sobreFigura = false;

        Vector3 posMundo = Camera.main.ScreenToWorldPoint(
            new Vector3(posScreen.x, posScreen.y, Camera.main.nearClipPlane));
        posMundo.z = 0;

        RaycastHit2D[] hits = Physics2D.RaycastAll(posMundo, Vector2.zero);

        foreach (var hit in hits)
        {
            if (hit.collider == null) continue;

            SpriteRenderer sr = hit.collider.GetComponent<SpriteRenderer>();
            if (sr == null) continue;

            foreach (var fig in figuras)
            {
                if (fig.terminada) continue;
                if (!fig.cuadrados.Contains(sr)) continue;

                sobreFigura = true;

                if (sr.color != colorPintado)
                {
                    sr.color = colorPintado;
                    fig.pintados++;
                    ActualizarUI();

                    if (!fig.terminada && fig.pintados >= fig.cuadrados.Count)
                        StartCoroutine(FiguraCompletada(fig));
                }
                break;
            }
        }

        if (!sobreFigura && Time.time - tiempoUltimoError > cooldownError)
        {
            erroresActuales++;
            tiempoUltimoError = Time.time;
            ActualizarUI();

            if (erroresActuales >= erroresMaximos)
                StartCoroutine(PerderJuego());
        }
    }

    private IEnumerator FiguraCompletada(DatosFigura fig)
    {
        fig.terminada = true;
        ActualizarUI();

        bool todas = true;
        foreach (var f in figuras) if (!f.terminada) { todas = false; break; }

        if (todas)
        {
            StartCoroutine(GanarJuego());
        }
        else
        {
            textoMensaje.text = $"¡{fig.nombre} completado!";
            yield return new WaitForSeconds(1f);
            textoMensaje.text = "";
        }
    }

    private IEnumerator GanarJuego()
    {
        juegoTerminado = true;
        textoMensaje.text = "¡COMPLETASTE TODO!";
        yield return new WaitForSeconds(2f);

        if (GameProgressManager.Instance != null)
            GameProgressManager.Instance.CompleteMinigame(minigameIndex);
        else
            Debug.LogWarning("GameProgressManager no encontrado en la escena");
        GuideManager.Instance.SetPendingDialogue(GuideManager.GuideEvent.FinDibujo);
        SceneManager.LoadScene(nombreEscenaPrincipal, LoadSceneMode.Single);
    }

    private IEnumerator PerderJuego()
    {
        juegoTerminado = true;
        dibujando = false;
        textoMensaje.text = "¡FALLASTE!\nDemasiados errores";
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(nombreEscenaPrincipal, LoadSceneMode.Single);
    }

    private void ActualizarUI()
    {
        textoErrores.text = $"Errores: {erroresActuales}/{erroresMaximos}";

        float prog = 0f;
        foreach (var f in figuras) prog += f.Progreso;
        textoProgreso.text = $"Progreso: {prog / figuras.Count * 100f:F0}%";
    }
}