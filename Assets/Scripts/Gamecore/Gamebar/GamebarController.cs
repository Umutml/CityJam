using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using Managers;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Gamecore
{
    public class GamebarController : MonoBehaviour
    {
        public static Action OnCollectableDestroyed;
        [SerializeField] private GamebarSlot[] gamebarSlots;
        [SerializeField] private float slotHeightDiff = 10f;
        [SerializeField] private float slotZAxisDiff = 3f;

        [SerializeField] private Camera uiCamera;
        [SerializeField] private Object destroyParticleFX;
        private readonly float _destroyAnimationDuration = 0.4f;
        private readonly float _destroyPositionUpDiff = 4f;
        private readonly float _JumpAnimationDuration = 0.3f;
        private readonly float _moveAnimationDuration = 1f;

        private void Awake()
        {
            LevelManager.OnLevelLoaded += ResetBarSlots;
        }

        private void Update()
        {
            CheckFor3SameCollectables();
            ShiftCollectablesLeft();
        }

        private void OnDestroy()
        {
            LevelManager.OnLevelLoaded -= ResetBarSlots;
        }

        private void CheckFor3SameCollectables()
        {
            var collecteds = new List<Collectable>();
            foreach (var slot in gamebarSlots)
            {
                if (slot.IsOccupied && !slot.GetIsAnimating())
                {
                    if (slot.GetOccupyingObject() == null) continue;
                    collecteds.Add(slot.GetOccupyingObject().GetComponent<Collectable>());
                }
            }

            if (collecteds.Count < 3)
                return;

            bool merged = false;

            for (var i = 0; i < collecteds.Count; i++)
            {
                var collectableType = collecteds[i].GetCollectableType();
                var sameTypeCollectables = collecteds.Where(collectable => collectable.GetCollectableType() == collectableType).ToList();

                if (sameTypeCollectables.Count == 3)
                {
                    merged = true;
                    var centeredPosition = sameTypeCollectables.Aggregate(Vector3.zero, (current, collectable) => current + collectable.transform.position) / 3;
                    var calculatedCenteredPosition = centeredPosition + Vector3.up * _destroyPositionUpDiff;
                    foreach (var collectable in sameTypeCollectables)
                    {
                        collectable.transform.DOMove(calculatedCenteredPosition, _destroyAnimationDuration).SetEase(Ease.InBack).OnComplete(() =>
                        {
                            LevelManager.Instance.AddBuilding(collectable.GetCollectableType());
                            collectable.DestroyCollectable();
                            OnCollectableDestroyed?.Invoke();
                        });

                        var slot = gamebarSlots.First(s => s.GetOccupyingObject() == collectable.gameObject);
                        slot.ClearOccupyingObject();
                        slot.SetOccupied(false);
                    }

                    PlayDestroyFX(calculatedCenteredPosition, _destroyAnimationDuration);
                    break;
                }
            }

            if (!merged && IsAllSlotsOccupied())
            {
                if(LevelManager.IsLevelPlaying)
                    GameOver();
            }
        }

        private void GameOver()
        {
            LevelManager.Instance.GameOver();
            Debug.Log("Game Over");
        }

        private void ResetBarSlots()
        {
            foreach (var slot in gamebarSlots)
            {
                slot.ClearOccupyingObject();
                slot.SetOccupied(false);
            }
        }

        private async void PlayDestroyFX(Vector3 position, float duration = 1.5f)
        {
            await Task.Delay((int)(duration * 1000));
            var particle = Instantiate(destroyParticleFX, position, Quaternion.identity);
            Destroy(particle, duration);
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
            Vector3 screenPosition = RectTransformUtility.WorldToScreenPoint(uiCamera, slot.transform.position);
            var centeredY = screenPosition.y - slotHeightDiff;
            var worldPosition = uiCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, centeredY, uiCamera.nearClipPlane + slotZAxisDiff));

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
            for (var i = 0; i < gamebarSlots.Length - 1; i++)
            {
                if (!gamebarSlots[i].IsOccupied)
                {
                    for (var j = i + 1; j < gamebarSlots.Length; j++)
                    {
                        if (gamebarSlots[j].IsOccupied && !gamebarSlots[j].GetIsAnimating())
                        {
                            var collectable = gamebarSlots[j].GetOccupyingObject().GetComponent<Collectable>();
                            DoJumpCollectableToSlot(collectable, gamebarSlots[i]);

                            gamebarSlots[i].SetOccupyingObject(collectable.gameObject);
                            gamebarSlots[i].SetOccupied(true);

                            gamebarSlots[j].ClearOccupyingObject();
                            gamebarSlots[j].SetOccupied(false);
                            return; // Exit the method after the first jump
                        }
                    }
                }
            }
        }

        private bool IsAllSlotsOccupied()
        {
            var allSlotsOccupied = gamebarSlots.All(slot => slot.IsOccupied); // Check if all slots are occupied
            var isAnySlotAnimating = gamebarSlots.Any(slot => slot.GetIsAnimating()); // Check if any slot is animating
            return allSlotsOccupied && !isAnySlotAnimating;
        }
        
        private void DoJumpCollectableToSlot(Collectable collectable, GamebarSlot slot)
        {
            Vector3 screenPosition = RectTransformUtility.WorldToScreenPoint(uiCamera, slot.transform.position);
            var centeredY = screenPosition.y - slotHeightDiff;
            var worldPosition = uiCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, centeredY, uiCamera.nearClipPlane + slotZAxisDiff));

            slot.SetAnimating(true);
            collectable.transform.DOJump(worldPosition, 1, 1, _JumpAnimationDuration).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                collectable.Highlight(false);
                collectable.Bounce();
                slot.Bounce();
                slot.SetAnimating(false);
            });
        }
    }
}
