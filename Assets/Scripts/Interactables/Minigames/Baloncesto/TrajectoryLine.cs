using UnityEngine;
using System.Collections;

public class TrajectoryLine : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _bulletSpawnPoint;
    [SerializeField] private LineRenderer _lineRenderer;

    [Header("Trayectory Line Smoothness/Length")]
    [SerializeField] private int _segmentCount = 30;
    [SerializeField] private float _timeStep = 0.1f; 
    [SerializeField] private float _maxTime = 3f; 

    [Header("Gravity")]
    [SerializeField] private float _gravityScale = 1f; 

    private Vector2[] _segments;
    private float _projectileSpeed;
    private float _currentAngle;

    private void Awake()
    {
        if (_lineRenderer == null)
            _lineRenderer = GetComponent<LineRenderer>();

        _lineRenderer.positionCount = _segmentCount;
        _lineRenderer.enabled = false;

        _segments = new Vector2[_segmentCount];
    }

    public void ShowTrajectory(float speed, float angle)
    {
        _projectileSpeed = speed;
        _currentAngle = angle;

        _lineRenderer.enabled = true;
        CalculateTrajectory();
    }

    public void HideTrajectory()
    {
        _lineRenderer.enabled = false;
    }

    private void CalculateTrajectory()
    {
        if (_bulletSpawnPoint == null) return;

        // Convertir posición del mundo a posición de pantalla
        Vector3 startPos = Camera.main.WorldToScreenPoint(_bulletSpawnPoint.position);
        startPos.z = 0; // Importante para UI

        _segments[0] = startPos;
        _lineRenderer.SetPosition(0, startPos);

        Vector2 startVelocity = new Vector2(
            Mathf.Cos(_currentAngle * Mathf.Deg2Rad) * _projectileSpeed,
            Mathf.Sin(_currentAngle * Mathf.Deg2Rad) * _projectileSpeed
        );

        for (int i = 1; i < _segmentCount; i++)
        {
            float t = i * _timeStep;
            if (t > _maxTime) break;

            // Calcular en espacio mundial primero
            float worldX = _bulletSpawnPoint.position.x + startVelocity.x * t;
            float worldY = _bulletSpawnPoint.position.y + startVelocity.y * t +
                          (0.5f * Physics2D.gravity.y * _gravityScale * t * t);

            // Convertir a coordenadas de pantalla
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(new Vector3(worldX, worldY, 0));
            screenPoint.z = 0;

            _segments[i] = screenPoint;
            _lineRenderer.SetPosition(i, screenPoint);
        }
    }

    public void UpdateTrajectory(float speed, float angle)
    {
        if (_projectileSpeed != speed || _currentAngle != angle)
        {
            _projectileSpeed = speed;
            _currentAngle = angle;
            CalculateTrajectory();
        }
    }
}
