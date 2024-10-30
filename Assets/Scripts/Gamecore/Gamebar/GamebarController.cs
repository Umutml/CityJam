using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using Managers;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Gamecore.Gamebar
{
    public class GamebarController : MonoBehaviour
    {
        public static Action OnCollectableDestroyed;
        [SerializeField] private GamebarSlot[] gamebarSlots;
        [SerializeField] private float slotHeightDiff = 10f;
        [SerializeField] private float slotZAxisDiff = 3f;
        [SerializeField] private Camera uiCamera;
        [SerializeField] private Object destroyParticleFX;

        private const float DestroyAnimationDuration = 0.4f;
        private const float DestroyPositionUpDiff = 4f;
        private const float JumpAnimationDuration = 0.3f;
        private const float MoveAnimationDuration = 1f;

        private void Awake()
        {
            LevelManager.OnLevelLoaded += ResetBarSlots;
        }

        private void Update()
        {
            if (!LevelManager.IsLevelPlaying) return;
            CheckFor3SameCollectables();
            ShiftCollectablesLeft();
        }

        private void OnDestroy()
        {
            LevelManager.OnLevelLoaded -= ResetBarSlots;
        }

        private void CheckFor3SameCollectables()
        {
            var collecteds = gamebarSlots
                .Where(slot => slot.IsOccupied && !slot.GetIsAnimating() && slot.GetOccupyingObject() != null)
                .Select(slot => slot.GetOccupyingObject().GetComponent<Collectable.Collectable>())
                .ToList();

            if (collecteds.Count < 3) return;

            foreach (var collectableType in collecteds.Select(c => c.GetCollectableType()).Distinct())
            {
                var sameTypeCollectables = collecteds.Where(c => c.GetCollectableType() == collectableType).ToList();
                if (sameTypeCollectables.Count == 3)
                {
                    MergeCollectables(sameTypeCollectables);
                    return;
                }
            }

            if (IsAllSlotsOccupied() && LevelManager.IsLevelPlaying)
                GameOver();
        }

        private void MergeCollectables(List<Collectable.Collectable> collectables)
        {
            var centerPos = collectables.Aggregate(Vector3.zero, (current, c) => current + c.transform.position) / 3;
            var targetPos = centerPos + Vector3.up * DestroyPositionUpDiff;

            foreach (var collectable in collectables)
            {
                collectable.transform.DOMove(targetPos, DestroyAnimationDuration).SetEase(Ease.InBack).OnComplete(() =>
                {
                    LevelManager.Instance.AddBuilding(collectable.GetCollectableType());
                    collectable.DestroyCollectable();
                    OnCollectableDestroyed?.Invoke();
                });

                var slot = gamebarSlots.First(s => s.GetOccupyingObject() == collectable.gameObject);
                slot.ClearOccupyingObject();
                slot.SetOccupied(false);
            }

            PlayDestroyFX(targetPos, DestroyAnimationDuration);
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

        public void AddCollectableToSlot(Collectable.Collectable collectable)
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

        private void MoveCollectableToSlot(Collectable.Collectable collectable, GamebarSlot slot)
        {
            var screenPos = RectTransformUtility.WorldToScreenPoint(uiCamera, slot.transform.position);
            var worldPos = uiCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y - slotHeightDiff, uiCamera.nearClipPlane + slotZAxisDiff));

            slot.SetAnimating(true);
            
            Sequence sequence = DOTween.Sequence();
            sequence.Append(collectable.transform.DOMoveY(collectable.transform.position.y + 2, 0.2f).SetEase(Ease.OutSine));
            sequence.Append(collectable.transform.DOMove(worldPos, MoveAnimationDuration).SetEase(Ease.InOutSine));
            sequence.OnComplete(() =>
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
                            var collectable = gamebarSlots[j].GetOccupyingObject().GetComponent<Collectable.Collectable>();
                            DoJumpCollectableToSlot(collectable, gamebarSlots[i]);

                            gamebarSlots[i].SetOccupyingObject(collectable.gameObject);
                            gamebarSlots[i].SetOccupied(true);

                            gamebarSlots[j].ClearOccupyingObject();
                            gamebarSlots[j].SetOccupied(false);
                            return;
                        }
                    }
                }
            }
        }

        private bool IsAllSlotsOccupied()
        {
            return gamebarSlots.All(slot => slot.IsOccupied) && !gamebarSlots.Any(slot => slot.GetIsAnimating());
        }

        private void DoJumpCollectableToSlot(Collectable.Collectable collectable, GamebarSlot slot)
        {
            var screenPos = RectTransformUtility.WorldToScreenPoint(uiCamera, slot.transform.position);
            var worldPos = uiCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y - slotHeightDiff, uiCamera.nearClipPlane + slotZAxisDiff));

            slot.SetAnimating(true);
            collectable.transform.DOJump(worldPos, 1, 1, JumpAnimationDuration).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                collectable.Highlight(false);
                collectable.Bounce();
                slot.Bounce();
                slot.SetAnimating(false);
            });
        }
    }
}