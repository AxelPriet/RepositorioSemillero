using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GuideManager : MonoBehaviour
{
    public static GuideManager Instance;

    public enum GuideEvent
    {
        // Primera aparición
        BienvenidaInicio,

        // Explicación antes de cada minijuego
        ExplicacionClinica,
        ExplicacionFotografia,
        ExplicacionRadio,
        ExplicacionCroma,
        ExplicacionBasket,
        ExplicacionSalaMac,
        ExplicacionCanchaSintetica,
        ExplicacionAulaMagna,
        ExplicacionFisioterapia,
        ExplicacionDibujo,
        ExplicacionLaboratorioSoluciones,
        ExplicacionGym,
        ExplicacionAtelier,
        ExplicacionCensei,
        ExplicacionPoliteca,
        ExplicacionLaboratorio,
        ExplicacionAnfiteatro,

        // Finalización de cada minijuego 
        FinClinica,
        FinFotografia,
        FinRadio,
        FinCroma,
        FinBasket,
        FinSalaMac,
        FinCanchaSintetica,
        FinAulaMagna,
        FinFisioterapia,
        FinDibujo,
        FinLaboratorioSoluciones,
        FinGym,
        FinAtelier,
        FinCensei,
        FinPoliteca,
        FinLaboratorio,
        FinAnfiteatro,
    }

    [Header("Referencia al personaje visual del guía")]
    [SerializeField] private GameObject guideCharacter;

    [Header("Nombre del guía")]
    [SerializeField] private string guideName = "A.A.V.";

    // Diálogos indexados por evento
    private Dictionary<GuideEvent, string[]> dialogues;

    // Registro de eventos ya mostrados (para no repetirlos)
    private HashSet<GuideEvent> shownEvents = new HashSet<GuideEvent>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); return; }

        InicializarDialogos();

        if (guideCharacter != null)
            guideCharacter.SetActive(false);
    }

    // ── API pública ──────────────────────────────────────────

    public void TriggerEvent(GuideEvent guideEvent, System.Action onComplete = null)
    {
        if (shownEvents.Contains(guideEvent))
        {
            onComplete?.Invoke();
            return;
        }

        if (!dialogues.ContainsKey(guideEvent))
        {
            onComplete?.Invoke();
            return;
        }

        shownEvents.Add(guideEvent);
        MostrarGuia(guideEvent, onComplete);
    }

    public void TriggerEventForced(GuideEvent guideEvent, System.Action onComplete = null)
    {
        if (!dialogues.ContainsKey(guideEvent))
        {
            onComplete?.Invoke();
            return;
        }

        MostrarGuia(guideEvent, onComplete);
    }

    public bool EventoMostrado(GuideEvent guideEvent) => shownEvents.Contains(guideEvent);

    // ── Lógica interna ───────────────────────────────────────

    private void MostrarGuia(GuideEvent guideEvent, System.Action onComplete)
    {
        if (guideCharacter != null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Vector3 offset = new Vector3(1.5f, 0f, 0f);
                guideCharacter.transform.position = player.transform.position + offset;
            }
            guideCharacter.SetActive(true);
        }

        string[] lines = dialogues[guideEvent];
        DialogueManager.Instance?.StartDialogue(guideName, lines, () =>
        {
            if (guideCharacter != null)
                guideCharacter.SetActive(false);
            onComplete?.Invoke();
        });
    }

    // ── Diálogos placeholder ─────────────────────────────────

    private void InicializarDialogos()
    {
        dialogues = new Dictionary<GuideEvent, string[]>
        {
            // ── Primera aparición ──
            [GuideEvent.BienvenidaInicio] = new[]
            {
                "¡Ey! Por fin estás aquí. Me presento: soy el Asistente Académico Virtual, pero puedes llamarme A.A.V. Piensa en mí como tu sombra amigable... estaré a tu lado en cada paso de este recorrido. ¿Listo para descubrir lo que la universidad tiene guardado para ti?",
                "Mira a tu alrededor... cada rincón del campus esconde un desafío. Son pequeñas actividades que te acercan a lo que realmente se vive aquí. Y no son solo juegos: al completar cada uno, obtendrás una pieza de un carnet muy especial. Junta todas y... bueno, digamos que habrás cumplido tu misión. ¿Te animas?",
                "Lo mejor de todo... no hay un camino fijo. Puedes ir a donde quieras, en el orden que quieras. Explora los pasillos, asómate a cada aula... y si prestas atención, tal vez encuentres más secretos escondidos por ahí. La universidad está llena de sorpresas para quienes se toman el tiempo de mirar.",
                "Bueno, llegó el momento de dejarte volar solo. Pero no te preocupes, siempre estaré cerca si me necesitas. Confío en que harás un gran recorrido. Ahora ve, explora, aprende y sobre todo... diviértete. ¡Mucha suerte!"
            },

            // ── Explicaciones antes de cada minijuego ──
            [GuideEvent.ExplicacionClinica] = new[]
            {
                "Este es el simulador de RCP. Mira esa barra... sube y baja como un latido. Tu misión es simple pero precisa: presiona ESPACIO justo cuando el indicador esté en verde. Hazlo cinco veces sin fallar tres. ¿Serás capaz de mantener el ritmo?"
            },
            [GuideEvent.ExplicacionFotografia] = new[]
            {
                "Aquí tienes una cámara. Encuadra a la persona dentro del rectángulo y... ¡clic! ESPACIO para capturar el momento. La fotografía también es parte de lo que se aprende aquí. Asegúrate de que esté bien centrada."
            },
            [GuideEvent.ExplicacionRadio] = new[]
            {
                "Antiguas radios, perillas que giran... tu objetivo es encontrar la señal clara. Gira cada perilla hasta que la luz pase de rojo a verde. Paciencia, la frecuencia perfecta está ahí esperando."
            },
            [GuideEvent.ExplicacionCroma] = new[]
            {
                 "Te muestran un color, tú debes encontrarlo. Gira las perillas hasta que el color que ves sea idéntico al de la muestra. Ojo al detalle... las diferencias pueden ser sutiles."
            },
            [GuideEvent.ExplicacionBasket] = new[]
            {
                "Balón en mano... usa las flechas para apuntar y mantén ESPACIO para cargar la fuerza. Suelta en el momento justo. Tres encestes y habrás dominado la cancha."
            },
            [GuideEvent.ExplicacionSalaMac] = new[]
            {
                 "Figuras geométricas esperando tomar forma. Arrastra cada pieza y únelas hasta formar un búho. La paciencia y la observación son tus mejores aliadas aquí."
            },
            [GuideEvent.ExplicacionCanchaSintetica] = new[]
            {
                "Frente al arco. Apunta con las flechas, carga la fuerza con ESPACIO y dispara. Tres goles, tres oportunidades. El portero lo dará todo... ¿tú también?"
            },
            [GuideEvent.ExplicacionAulaMagna] = new[]
            {
                "Una luz que guía... mantén presionado el clic izquierdo y mueve el mouse para seguir al graduado. Acompaña su paso hasta que recoja su diploma. Un momento importante merece ser seguido de cerca."
            },
            [GuideEvent.ExplicacionFisioterapia] = new[]
            {
                 "Agua que necesita el equilibrio perfecto. Usa A y D para abrir las llaves. Si abres las dos, la temperatura se estabiliza. Mantén la tina entre 40 y 60 grados durante cinco segundos. Ni frío, ni caliente... justo como debe ser."
            },
            [GuideEvent.ExplicacionDibujo] = new[]
            {
                "Trazar el contorno de tres figuras. Mantén presionado y sigue la línea... si te sales, sumarás errores. Treinta fallos y tendrás que empezar de nuevo. Mano firme y pulso seguro."
            },
            [GuideEvent.ExplicacionLaboratorioSoluciones] = new[]
            {
                "Cada instrumento mide algo distinto. Lee las capacidades, luego coloca cada uno en el lugar que le corresponde. La precisión es clave en un laboratorio."
            },
            [GuideEvent.ExplicacionGym] = new[]
            {
                "La barra te espera. Presiona ESPACIO una y otra vez para levantarla. Cada repetición te acerca más a tu meta. ¿Cuánto peso puedes levantar?"
            },
            [GuideEvent.ExplicacionAtelier] = new[]
            {
                "Un robot desarmado y su silueta esperando cobrar vida. Lleva cada pieza al lugar que le pertenece. Arrastra con clic izquierdo y devuélvele su forma."
            },
            [GuideEvent.ExplicacionCensei] = new[]
            {
                "Barras con nombre y un gráfico que las espera. Observa, relaciona y coloca cada una donde debe ir. Parece sencillo, pero la precisión importa."
            },
            [GuideEvent.ExplicacionPoliteca] = new[]
            {
                "Libros que esperan su orden. Del más claro al más oscuro... como un arcoíris de tonos. Tómalos y colócalos en la estantería siguiendo la secuencia."
            },
            [GuideEvent.ExplicacionLaboratorio] = new[]
            {
                "Cada instrumento ocupa un espacio. Pasa el mouse sobre ellos para ver su tamaño y luego colócalos en los slots del estante. Debes llenarlo todo sin que sobre ni falte lugar."
            },
            [GuideEvent.ExplicacionAnfiteatro] = new[]
            {
               "Un maniquí espera que le devuelvas sus órganos. Arrastra cada pieza hasta su silueta. Como un rompecabezas anatómico... coloca todo en su lugar."
            },

            // ── Fin de cada minijuego ──
            [GuideEvent.FinClinica] = new[]
            {
                "¡Latidos al ritmo perfecto! Bajando un piso, la fotografía, el croma y la radio te esperan. Como un pequeño festival de medios. Ve a descubrir cuál te llama más."
            },
            [GuideEvent.FinFotografia] = new[]
            {
                "¡Bien capturado! Si bajas más, el aula magna te espera con una ceremonia especial. Y si prefieres el deporte... la cancha de básquet está en el piso superior en el coliseo. Tú eliges."
            },
            [GuideEvent.FinRadio] = new[]
            {
                "¡Bien capturado! Si bajas más, el aula magna te espera con una ceremonia especial. Y si prefieres el deporte... la cancha de básquet está en el piso superior en el coliseo. Tú eliges."
            },
            [GuideEvent.FinCroma] = new[]
            {
                "¡Bien capturado! Si bajas más, el aula magna te espera con una ceremonia especial. Y si prefieres el deporte... la cancha de básquet está en el piso superior en el coliseo. Tú eliges."
            },
            [GuideEvent.FinBasket] = new[]
            {
                "¡Enceste perfecto! Ahora baja a tierra firme. El edificio central tiene su propio ritmo: el censei y la politeca están ahí, esperando tu toque."
            },
            [GuideEvent.FinSalaMac] = new[]
            {
               "¡El búho ha cobrado forma! Sube un piso más. El laboratorio de soluciones te espera con sus instrumentos y medidas exactas. La ciencia también es arte."
            },
            [GuideEvent.FinCanchaSintetica] = new[]
            {
                "¡Gol y victoria! Solo queda un último desafío. El laboratorio de espacios está cerca, en el edificio cuatro. Ve a ocupar cada slot y cierra con broche de oro."
            },
            [GuideEvent.FinAulaMagna] = new[]
            {
                "Un momento inolvidable. ¿Listo para cambiar de aires? Baja al edificio dos. La fisioterapia te espera con sus llaves de agua, y más allá, el salón de dibujo con sus contornos por trazar."
            },
            [GuideEvent.FinFisioterapia] = new[]
            {
                "Equilibrio... lo has hecho muy bien. Ahora cruza hacia el edificio central. El censei y la politeca están cerca. Cada edificio guarda un desafío distinto."
            },
            [GuideEvent.FinDibujo] = new[]
            {
                "Trazos... lo has hecho muy bien. Ahora cruza hacia el edificio central. El censei y la politeca están cerca. Cada edificio guarda un desafío distinto."
            },
            [GuideEvent.FinLaboratorioSoluciones] = new[]
            {
                "¡Medidas perfectas! Ahora cruza hacia el edificio cuatro. El anfiteatro está ahí, con sus órganos por ordenar. Y dos pisos más arriba, otro laboratorio te reta a llenar cada espacio."
            },
            [GuideEvent.FinGym] = new[]
            {
                "¡Fuerza y resistencia! No te alejes mucho... la cancha sintética está ahí nomás. El portero te espera para ver si puedes marcar la diferencia."
            },
            [GuideEvent.FinAtelier] = new[]
            {
               "¡El robot ha vuelto a la vida pieza por pieza! Un trabajo de precisión y paciencia. Ahora dirígete al edificio cuatro. El anfiteatro te espera con su rompecabezas anatómico, y más arriba, otro laboratorio donde cada espacio cuenta. El camino sigue abierto si aun no los has hecho."
            },
            [GuideEvent.FinCensei] = new[]
            {
                "¡Orden y estructura! Ahora ve al edificio múltiple dos. La sala Mac te espera con su búho geométrico, y más arriba, el laboratorio de soluciones. Sigue explorando."
            },
            [GuideEvent.FinPoliteca] = new[]
            {
                 "¡Orden y estructura! Ahora ve al edificio múltiple dos. La sala Mac te espera con su búho geométrico, y más arriba, el laboratorio de soluciones. Sigue explorando."
            },
            [GuideEvent.FinLaboratorio] = new[]
            {
                "¡Cada instrumento en su lugar! Has recorrido cada rincón, has enfrentado cada desafío. Ahora ve por tu recompensa final... el carnet te espera completo."
            },
            [GuideEvent.FinAnfiteatro] = new[]
            {
                 "El cuerpo humano tiene su orden... ¡y tú lo has logrado! Ahora sal al exterior. El gym te espera con su barra por levantar, y cerca, la cancha sintética pide a gritos tres goles."
            },
        };
    }
}