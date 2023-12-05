using UnityEngine;
using DG.Tweening;

public class ScaleAnimator : MonoBehaviour
{

    private int _id = 0;
    private Transform _transform;
    [SerializeField] private Vector3 _initialScale;
    private Vector3 _initialPos;

    private void Awake()
    {
        _id = int.Parse(gameObject.name);
        _transform = transform;
        _initialScale = _transform.localScale;
        _initialPos = _transform.localPosition;

        PlaneHoverDetector.OnHover += OnHover;
        PlaneHoverDetector.OnExit += OnExit;
    }

    private void OnDisable()
    {
        PlaneHoverDetector.OnHover -= OnHover;
        PlaneHoverDetector.OnExit -= OnExit;
    }
    public void OnHover(int id)
    {
        if (id != _id)
            return;

        Debug.Log($"OnHover {id}");
        _transform.DOKill(false);
        _transform.DOScale(_initialScale * 1.5f, 0.75f);

        Vector3 localPositionAfterMove = _initialPos - _transform.forward * 0.3f;
        _transform.DOLocalMove(localPositionAfterMove, 0.75f);
    }

    public void OnExit(int id)
    {
        if (id != _id)
            return;

        Debug.Log($"OnExit {id}");
        _transform.DOKill(false);
        _transform.DOScale(_initialScale, 0.25f);
        _transform.DOLocalMove(_initialPos, 0.25f);
    }
}
