using UnityEngine;

[CreateAssetMenu(fileName = "GuiaMensajes", menuName = "Guia/Mensajes")]
public class GuiaMensajes : ScriptableObject
{
    [Header("Mensajes de Bienvenida")]
    public string[] bienvenida;

    [Header("Al entrar a un minijuego")]
    public string[] entradaMinijuego; 

    [Header("Al completar un minijuego con éxito")]
    public string[] exitoMinijuego;

    [Header("Al fallar un minijuego")]
    public string[] falloMinijuego;

    [Header("Consejos después de varios fallos")]
    public string[] consejosAvanzados;

    [Header("Al obtener una pieza de carnet")]
    public string[] progreso;

    [Header("Mensajes genéricos de orientación")]
    public string[] orientacion;
}