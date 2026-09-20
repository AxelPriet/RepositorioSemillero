using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "Sound", menuName = "Audio/Nuevo Sonido", order = 1)]
public class SoundSO : ScriptableObject
{
    [Header("Identificador")]
    [SerializeField] private string id; 

    [Header("Audio")]
    [SerializeField] private AudioClip clip;

    [Header("Configuración")]
    [SerializeField] private AudioMixerGroup mixerGroup;
    [SerializeField, Range(0f, 1f)] private float volume = 1f;
    [SerializeField] private Vector2 pitchRange = new Vector2(1f, 1f);
    [SerializeField] private bool loop = false;
    [SerializeField] private bool playOnAwake = false;

    [Header("3D (opcional)")]
    [SerializeField] private bool is3D = false;
    [SerializeField] private float minDistance = 1f;
    [SerializeField] private float maxDistance = 20f;

    public string ID => id;
    public AudioClip Clip => clip;
    public AudioMixerGroup MixerGroup => mixerGroup;
    public float Volume => volume;
    public Vector2 PitchRange => pitchRange;
    public bool Loop => loop;
    public bool PlayOnAwake => playOnAwake;
    public bool Is3D => is3D;
    public float MinDistance => minDistance;
    public float MaxDistance => maxDistance;

    public float GetRandomPitch()
    {
        return Random.Range(pitchRange.x, pitchRange.y);
    }
}