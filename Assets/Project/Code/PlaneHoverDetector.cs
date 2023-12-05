using System;
using UnityEngine;
using UnityEngine.Events;

public class PlaneHoverDetector : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;

    public static event Action<int> OnHover;
    public static event Action<int> OnExit;

    [SerializeField] private int _previousId = 4;
    [SerializeField] private int _currentId = 0;

    private void Start()
    {
        Application.targetFrameRate = 72;
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        Ray ray = new Ray(_mainCamera.transform.position, _mainCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Debug.DrawLine(ray.origin, hit.point);

            bool isParced = int.TryParse(hit.collider.gameObject.name, out _currentId);

            if (!isParced)
                return;

            if (_currentId == _previousId)
                return;

            if (_previousId < 20)
            {
                OnExit?.Invoke(_previousId);
            }

            OnHover?.Invoke(_currentId);

            _previousId = _currentId;
        }
        else
        {
            OnExit?.Invoke(_previousId);
            _previousId = 99;
        }
    }
}
