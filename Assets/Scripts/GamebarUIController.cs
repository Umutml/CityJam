using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class GamebarUIController : MonoBehaviour
{
    [SerializeField] private GamebarSlot[] gamebarSlots;
    private Camera _mainCamera;
    [SerializeField] private float slotHeightDiff = 15f;
    [SerializeField] private float slotZAxisDiff = 3f;
    private float _moveAnimationDuration = 1f;
    private float _destroyAnimationDuration = 0.3f;
    private float _JumpAnimationDuration = 0.3f;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        CheckFor3SameCollectables();
        ShiftCollectablesLeft();
    }

    private void CheckFor3SameCollectables()
    {
        var collecteds = new List<Collectable>();
        foreach (var slot in gamebarSlots)
        {
            if (slot.IsOccupied && !slot.IsAnimating())
            {
                collecteds.Add(slot.GetOccupyingObject().GetComponent<Collectable>());
            }
        }

        if (collecteds.Count < 3)
            return;

        for (int i = 0; i < collecteds.Count; i++)
        {
            var collectableType = collecteds[i].GetCollectableType();
            var sameTypeCollectables = collecteds.Where(collectable => collectable.GetCollectableType() == collectableType).ToList();

            if (sameTypeCollectables.Count == 3)
            {
                var centeredPosition = sameTypeCollectables.Aggregate(Vector3.zero, (current, collectable) => current + collectable.transform.position) / 3;
                centeredPosition.y += 2f;
                foreach (var collectable in sameTypeCollectables)
                {
                    collectable.transform.DOMove(centeredPosition, _destroyAnimationDuration).OnComplete(() =>
                    {
                        collectable.DestroyCollectable();
                    });

                    var slot = gamebarSlots.First(s => s.GetOccupyingObject() == collectable.gameObject);
                    slot.ClearOccupyingObject();
                    slot.SetOccupied(false);
                }
                break;
            }
        }
    }

    public GamebarSlot[] GetGamebarElements()
    {
        return gamebarSlots;
    }

    public void AddCollectableToSlot(Collectable collectable)
    {
        var firstEmptySlot = GetFirstEmptySlot();
        if (firstEmptySlot == null)
        {
            Debug.LogError("No empty slot found");
            return;
        }

        MoveCollectableToSlot(collectable, firstEmptySlot);
        firstEmptySlot.SetOccupyingObject(collectable.gameObject);
        firstEmptySlot.SetOccupied(true);
    }

    private void MoveCollectableToSlot(Collectable collectable, GamebarSlot slot)
    {
        Vector3 screenPosition = RectTransformUtility.WorldToScreenPoint(_mainCamera, slot.transform.position);
        var centeredY = screenPosition.y - slotHeightDiff;
        var worldPosition = _mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, centeredY, _mainCamera.nearClipPlane + slotZAxisDiff));

        slot.SetAnimating(true);
        collectable.transform.DOMove(worldPosition, _moveAnimationDuration).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            collectable.Highlight(false);
            collectable.Bounce();
            slot.Bounce();
            slot.SetAnimating(false);
        });
    }

    public GamebarSlot GetFirstEmptySlot()
    {
        return gamebarSlots.FirstOrDefault(slot => !slot.IsOccupied);
    }

    private void ShiftCollectablesLeft()
    {
        for (int i = 0; i < gamebarSlots.Length - 1; i++)
        {
            if (!gamebarSlots[i].IsOccupied)
            {
                for (int j = i + 1; j < gamebarSlots.Length; j++)
                {
                    if (gamebarSlots[j].IsOccupied && !gamebarSlots[j].IsAnimating())
                    {
                        var collectable = gamebarSlots[j].GetOccupyingObject().GetComponent<Collectable>();
                        DoJumpCollectableToSlot(collectable, gamebarSlots[i]);
                        
                        gamebarSlots[i].SetOccupyingObject(collectable.gameObject);
                        gamebarSlots[i].SetOccupied(true);

                        gamebarSlots[j].ClearOccupyingObject();
                        gamebarSlots[j].SetOccupied(false);
                        break;
                    }
                }
            }
        }
    }
    
    private void DoJumpCollectableToSlot(Collectable collectable, GamebarSlot slot)
    {
        Vector3 screenPosition = RectTransformUtility.WorldToScreenPoint(_mainCamera, slot.transform.position);
        var centeredY = screenPosition.y - slotHeightDiff;
        var worldPosition = _mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, centeredY, _mainCamera.nearClipPlane + slotZAxisDiff));

        slot.SetAnimating(true);
        collectable.transform.DOJump(worldPosition,1,1, _JumpAnimationDuration).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            collectable.Highlight(false);
            collectable.Bounce();
            slot.Bounce();
            slot.SetAnimating(false);
        });
    }
}
