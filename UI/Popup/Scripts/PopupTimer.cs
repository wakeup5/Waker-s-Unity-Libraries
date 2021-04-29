using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Waker.Popups
{
    [RequireComponent(typeof(PopupBase))]
    public class PopupTimer : MonoBehaviour, IPopupCallback
    {
        [SerializeField] private Slider timerSlider;
        [SerializeField] private float time;

        [SerializeField] public UnityEvent onTimerEnd;

        private Coroutine timerCoroutine;

        public void OnOpened()
        {
            if (timerCoroutine != null)
            {
                StopCoroutine(timerCoroutine);
                timerCoroutine = null;
            }

            timerCoroutine = StartCoroutine(StartTimer());
        }

        private IEnumerator StartTimer()
        {
            timerSlider.minValue = 0f;
            timerSlider.minValue = 1f;

            float t = time;
            while (t > 0f)
            {
                yield return null;
                t -= Time.deltaTime;
                timerSlider.value = t / time;
            }

            onTimerEnd.Invoke();
        }

        public void OnClosed()
        {
            if (timerCoroutine != null)
            {
                StopCoroutine(timerCoroutine);
                timerCoroutine = null;
            }
        }

        public void OnConfirmed()
        {
            
        }

        public void OnCanceled()
        {
            
        }
    }
}