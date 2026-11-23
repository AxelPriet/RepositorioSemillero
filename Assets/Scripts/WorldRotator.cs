using UnityEngine;
using System.Collections;

public class WorldRotator : MonoBehaviour
{
    public float rotationDuration = 0.5f;

    private bool rotating = false;
    private bool rotated = false;

    public void ToggleRotation()
    {
        if (rotating) return;

        if (!rotated)
            StartCoroutine(RotateSmooth(90f));
        else
            StartCoroutine(RotateSmooth(-90f));

        rotated = !rotated;
    }

    private IEnumerator RotateSmooth(float angle)
    {
        rotating = true;

        Quaternion startRot = transform.rotation;
        Quaternion endRot = startRot * Quaternion.Euler(0f, 0f, angle);

        float time = 0f;

        while (time < rotationDuration)
        {
            time += Time.deltaTime;
            float t = time / rotationDuration;
            transform.rotation = Quaternion.Lerp(startRot, endRot, t);
            yield return null;
        }

        transform.rotation = endRot;
        rotating = false;
    }
}