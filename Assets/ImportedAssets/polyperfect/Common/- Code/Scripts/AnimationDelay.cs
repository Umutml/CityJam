using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Polyperfect.Common
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    [DefaultExecutionOrder(-50)]
    public class AnimationDelay : MonoBehaviour
    {
        [Tooltip("Delay to start the animation after")]
        [SerializeField]
        private float ConstantDelay = 0f;

        [Tooltip("An additional random delay to be added")]
        [SerializeField]
        private float RandomDelay = 0f;

        public UnityEvent OnAnimStart;

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(ConstantDelay + Random.Range(0f, RandomDelay));
            GetComponent<Animator>().enabled = true;
            OnAnimStart?.Invoke();
        }

        private void OnValidate()
        {
            ConstantDelay = Mathf.Clamp(ConstantDelay, 0f, float.MaxValue);
            RandomDelay = Mathf.Clamp(RandomDelay, 0f, float.MaxValue);
            GetComponent<Animator>().enabled = false;
        }
    }
}
