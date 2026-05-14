using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class MinijuegoRCP : MonoBehaviour
{
    [Header("UI")]
    public RectTransform indicador;
    public RectTransform zonaVerde;
    [SerializeField] private TextMeshProUGUI textoPuntos;
    [SerializeField] private TextMeshProUGUI textoErrores;
    [SerializeField] private TextMeshProUGUI textoInstrucciones;

    [Header("Configuración")]
    public float velocidad = 700f;
    public int puntosNecesarios = 5;
    [SerializeField] private float tiempoPausa = 0.3f;
    [SerializeField] private int maxErrores = 3;
    [SerializeField] private string nombreEscenaPrincipal = "Main";

    [Header("Límites del Indicador")]
    [SerializeField] private bool usarLimitesManuales = true;
    [SerializeField] private float limiteInferior = -150f;
    [SerializeField] private float limiteSuperior = 150f;

    private float direccion = 1f;
    private float minY;
    private float maxY;
    private int puntos = 0;
    private int errores = 0;
    private bool enPausa = false;
    private bool juegoActivo = true;
    [SerializeField] private int minigameIndex;

    private PlayerControls playerControls;

    void Start()
    {
        if (usarLimitesManuales)
        {
            minY = limiteInferior;
            maxY = limiteSuperior;
        }
        else
        {
            float alturaBarra = ((RectTransform)indicador.parent).rect.height;
            minY = -alturaBarra / 2 + (indicador.rect.height / 2);
            maxY = alturaBarra / 2 - (indicador.rect.height / 2);
        }

        playerControls = InputHandler.Instance.GetControls();
        playerControls.Gameplay.Compress.performed += OnCompress;

        ActualizarUI();
        textoInstrucciones.text = "Presiona ESPACIO cuando el indicador esté en la zona verde";
    }

    void Update()
    {
        if (juegoActivo)
            MoverIndicador();
    }

    private void OnDestroy()
    {
        if (playerControls != null)
            playerControls.Gameplay.Compress.performed -= OnCompress;
    }

    private void ActualizarUI()
    {
        textoPuntos.text = $"Aciertos: {puntos}/{puntosNecesarios}";
        textoErrores.text = $"Errores: {errores}/{maxErrores}";
    }

    private void OnCompress(InputAction.CallbackContext context)
    {
        if (juegoActivo && !enPausa)
            VerificarZona();
    }

    void VerificarZona()
    {
        float indicadorY = indicador.anchoredPosition.y;
        float zonaMin = zonaVerde.anchoredPosition.y - (zonaVerde.rect.height / 2);
        float zonaMax = zonaVerde.anchoredPosition.y + (zonaVerde.rect.height / 2);

        if (indicadorY >= zonaMin && indicadorY <= zonaMax)
        {
            puntos++;
            StartCoroutine(PausaIndicador());
            ActualizarUI();

            if (puntos >= puntosNecesarios)
                StartCoroutine(CompletarMinijuego());
        }
        else
        {
            errores++;
            ActualizarUI();

            if (errores >= maxErrores)
                StartCoroutine(FallarMinijuego());
        }
    }

    void MoverIndicador()
    {
        if (enPausa) return;

        Vector2 pos = indicador.anchoredPosition;
        pos.y += direccion * velocidad * Time.deltaTime;

        if (pos.y >= maxY)
        {
            pos.y = maxY;
            direccion = -1;
        }
        else if (pos.y <= minY)
        {
            pos.y = minY;
            direccion = 1;
        }

        indicador.anchoredPosition = pos;
    }

    private IEnumerator PausaIndicador()
    {
        enPausa = true;
        yield return new WaitForSeconds(tiempoPausa);
        enPausa = false;
    }

    private IEnumerator CompletarMinijuego()
    {
        juegoActivo = false;
        textoPuntos.text = "¡COMPLETADO!";
        textoErrores.text = "¡Bien hecho!";
        textoInstrucciones.text = "RCP exitoso";

        yield return new WaitForSeconds(1.5f);

        GuideManager.Instance.SetPendingDialogue(GuideManager.GuideEvent.FinClinica);
        GameProgressManager.Instance.CompleteMinigame(minigameIndex);
        SceneManager.LoadScene(nombreEscenaPrincipal, LoadSceneMode.Single);
    }

    private IEnumerator FallarMinijuego()
    {
        juegoActivo = false;
        textoPuntos.text = "¡INTÉNTALO";
        textoErrores.text = "DE NUEVO!";
        textoInstrucciones.text = "Demasiados errores";

        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(nombreEscenaPrincipal, LoadSceneMode.Single);
    }

    private void OnDrawGizmosSelected()
    {
        if (!usarLimitesManuales || indicador == null) return;

        Transform parent = indicador.parent;
        if (parent == null) return;

        Vector3 worldPos = parent.TransformPoint(Vector3.zero);
        Vector3 up = parent.up;
        Vector3 right = parent.right;

        Vector3 inferior = worldPos + up * limiteInferior;
        Vector3 superior = worldPos + up * limiteSuperior;

        float lineWidth = 100f;
        Vector3 izquierda = -right * lineWidth;
        Vector3 derecha = right * lineWidth;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(inferior + izquierda, inferior + derecha);
        Gizmos.DrawLine(superior + izquierda, superior + derecha);
        Gizmos.DrawLine(inferior, superior);

        Gizmos.color = new Color(1, 0, 0, 0.2f);
        Vector3 topLeft = superior + izquierda;
        Vector3 topRight = superior + derecha;
        Vector3 bottomLeft = inferior + izquierda;
        Vector3 bottomRight = inferior + derecha;
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }
}