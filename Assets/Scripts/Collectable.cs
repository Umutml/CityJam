using DG.Tweening;
using EPOOutline;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    private Outlinable _outlineable;
    private BoxCollider _boxCollider;
    private readonly float _duration = 0.5f;
    private readonly Vector3 _targetRotation = new(-90, 180, 0); // The target rotation need to be UI like rotation

    private void Awake()
    {
        _outlineable = GetComponent<Outlinable>();
        _boxCollider = GetComponent<BoxCollider>();
        _outlineable.enabled = false;
    }

    public void Highlight(bool highlight)
    {
        // Highlight the collectable

        if (highlight)
        {
            // Highlight the collectable 3d object outline,
            _outlineable.OutlineParameters.Color = Color.cyan;
            _outlineable.enabled = true;
            Debug.Log("Highlighting collectable");
        }
        else
        {
            // Remove the highlight from the collectable
        }
    }

    public void TurnObject()
    {
        transform.DORotate(_targetRotation, _duration).SetEase(Ease.OutSine).OnComplete(() =>
        {
            transform.rotation = Quaternion.identity; // Reset the rotation
        });
    }

    public void ScaleObject()
    {
        transform.DOScale(transform.localScale * 0.8f, _duration).SetEase(Ease.OutSine);
    }
    
    public void DisableCollider()
    {
        _boxCollider.enabled = false;
        // Disable the collider of the collectable
    }
    
    public void MoveToUIPosition(Vector3 screenPosition)
    {
        transform.DOMove(screenPosition, 1f).SetEase(Ease.InOutSine);
        // Move the collectable to the UI position
    }
}
