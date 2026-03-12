using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MinijuegoRCP : MonoBehaviour
{
    [Header("UI")]
    public RectTransform indicador;
    public RectTransform zonaVerde;
    [SerializeField] private TMPro.TextMeshProUGUI textoPuntos;
    [SerializeField] private TMPro.TextMeshProUGUI textoErrores;
    [SerializeField] private GameObject panelInstrucciones;

    [Header("Configuración")]
    public float velocidad = 700f;
    public int puntosNecesarios = 5;
    [SerializeField] private float tiempoPausa = 0.3f;

    private float direccion = 1f;
    private float minY;
    private float maxY;
    private int puntos = 0;
    private int errores = 0;
    private bool enPausa = false;
    private bool juegoActivo = true;

    private PlayerControls playerControls;

    void Start()
    {
        float alturaBarra = ((RectTransform)indicador.parent).rect.height;
        minY = -alturaBarra / 2 + (indicador.rect.height / 2);
        maxY = alturaBarra / 2 - (indicador.rect.height / 2);

        playerControls = InputHandler.Instance.GetControls();
        playerControls.Gameplay.Compress.performed += OnCompress;

        ActualizarUI();

        Debug.Log("Minijuego RCP iniciado");
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
        textoPuntos.text = "Aciertos: " + puntos;
        textoErrores.text = "Errores: " + errores;
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
            Debug.Log("¡Buena compresión! Puntos: " + puntos);
            StartCoroutine(PausaIndicador());
            ActualizarUI();

            if (puntos >= puntosNecesarios)
            {
                StartCoroutine(CompletarMinijuego());
            }
        }
        else
        {
            errores++;
            Debug.Log("Compresión incorrecta");
            ActualizarUI();

            if (errores >= 3)
            {
                StartCoroutine(FallarMinijuego());
            }
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

        yield return new WaitForSeconds(1.5f);

        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }

    private IEnumerator FallarMinijuego()
    {
        juegoActivo = false;
        textoPuntos.text = "¡INTÉNTALO";
        textoErrores.text = "DE NUEVO!";

        yield return new WaitForSeconds(1.5f);

        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }
}