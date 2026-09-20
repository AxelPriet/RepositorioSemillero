using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundLibrary", menuName = "Audio/Librería de Sonidos", order = 2)]
public class SoundLibrary : ScriptableObject
{
    [SerializeField] private List<SoundSO> sounds = new List<SoundSO>();

    private Dictionary<string, SoundSO> lookup;

    private void OnEnable()
    {
        BuildLookup();
    }

    private void BuildLookup()
    {
        lookup = new Dictionary<string, SoundSO>();
        foreach (var sound in sounds)
        {
            if (sound == null || string.IsNullOrEmpty(sound.ID)) continue;
            if (!lookup.ContainsKey(sound.ID))
                lookup.Add(sound.ID, sound);
            else
                Debug.LogWarning($"Sonido duplicado con ID: {sound.ID}");
        }
    }

    public SoundSO Get(string id)
    {
        if (lookup == null) BuildLookup();
        return lookup.TryGetValue(id, out SoundSO sound) ? sound : null;
    }
}