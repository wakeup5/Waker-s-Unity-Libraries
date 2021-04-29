using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Waker.Popups
{
    [RequireComponent(typeof(Canvas), typeof(GraphicRaycaster))]
    public abstract class PopupBase : MonoBehaviour, IPopupCallback
    {
        [Header("Popup Base")]
        [SerializeField] private string popupName;
        [SerializeField] private GameObject context;
        [SerializeField] private Canvas canvasForThis;
        [SerializeField] private bool isPositionAlignmentOnPlay;
        [SerializeField] private bool isDisableOnPlay;

        public event Action onClosed;
        public event Action onCanceled;
        public event Action onConfirmed;

        public string PopupName => popupName;
        public int SortingOrder
        {
            get => canvasForThis.sortingOrder;
            set => canvasForThis.sortingOrder = value;
        }

        public bool IsOpen => Context.activeSelf;

        protected GameObject Context => context;
        protected Canvas CanvasForThis => canvasForThis;


        protected virtual void Reset()
        {
            var c = transform.Find("Context");
            if (c != null) context = c.gameObject;

            canvasForThis = GetComponent<Canvas>();
            canvasForThis.overrideSorting = true;
            canvasForThis.sortingLayerName = "popup";
        }

        protected virtual void Awake()
        {
            if (canvasForThis == null)
            {
                canvasForThis = GetComponent<Canvas>();
            }

            if (string.IsNullOrEmpty(popupName))
            {
                popupName = this.name;
            }

            // 위치 초기화
            if (isPositionAlignmentOnPlay)
            {
                RectTransform r = transform as RectTransform;

                r.anchorMax = Vector2.one;
                r.anchorMin = Vector2.zero;

                r.pivot = new Vector2(0.5f, 0.5f);
                r.anchoredPosition = Vector3.zero;
            }

            // 시작할 때 닫음
            if (isDisableOnPlay)
            {
                Context.SetActive(false);
            }
        }

        protected virtual void OnEnable()
        {
            PopupController.Instance?.RegistPopup(this);
        }

        protected virtual void OnDisable()
        {
            PopupController.Instance?.UnregistPopup(this);
        }

        protected virtual void OnDestroy()
        {

        }

        public virtual void Open()
        {
            ExecuteEvents.Execute<IPopupCallback>(gameObject, null, (i, e) => i.OnOpened());
        }

        public virtual void Close()
        {
            ExecuteEvents.Execute<IPopupCallback>(gameObject, null, (i, e) => i.OnClosed());
            onClosed?.Invoke();

            onClosed = null;
            onConfirmed = null;
            onCanceled = null;
        }

        public virtual void Confirm()
        {
            ExecuteEvents.Execute<IPopupCallback>(gameObject, null, (i, e) => i.OnConfirmed());
            onConfirmed?.Invoke();

            Close();
        }

        public virtual void Cancel()
        {
            ExecuteEvents.Execute<IPopupCallback>(gameObject, null, (i, e) => i.OnCanceled());
            onCanceled?.Invoke();

            Close();
        }

        public virtual void OnOpened()
        {
            context.SetActive(true);
            PopupController.Instance?.AddOpenPopup(this);
        }

        public virtual void OnClosed()
        {
            context.SetActive(false);
            PopupController.Instance?.RemoveOpenPopup(this);
        }

        public virtual void OnConfirmed()
        {
            
        }

        public virtual void OnCanceled()
        {
            
        }
    }

    public class PopupCallback : MonoBehaviour, IPopupCallback
    {
        [SerializeField] public UnityEvent onOpened;
        [SerializeField] public UnityEvent onClosed;
        [SerializeField] public UnityEvent onConfirmed;
        [SerializeField] public UnityEvent onCanceled;

        public void OnCanceled()
        {
            onCanceled.Invoke();
        }

        public void OnClosed()
        {
            onClosed.Invoke();
        }

        public void OnConfirmed()
        {
            onConfirmed.Invoke();
        }

        public void OnOpened()
        {
            onOpened.Invoke();
        }
    }
}