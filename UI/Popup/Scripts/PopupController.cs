using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Waker.Popups
{
    public class PopupController : Controller<PopupController>
    {
        [SerializeField] private PopupBase[] allPopups;

        // 모든 팝업 목록
        private Dictionary<string, PopupBase> popups = new Dictionary<string, PopupBase>();

        // 현재 열린 팝업 목록
        private LinkedList<PopupBase> openedPopups = new LinkedList<PopupBase>();

        // protected override void Awake()
        // {
        //     base.Awake();

        //     foreach (var p in allPopups)
        //     {
        //         RegistPopup(p);
        //     }
        // }

        // 팝업을 등록
        internal void RegistPopup(PopupBase popup)
        {
            //this.Log($"등록 요청? {popup.PopupName}");
            if (popups.ContainsKey(popup.PopupName))
            {
                return;
            }

            //this.Log($"등록됨? {popup.PopupName}");
            popups.Add(popup.PopupName, popup);
        }

        // 팝업 등록 해제
        internal void UnregistPopup(PopupBase popup)
        {
            //this.Log($"미등록됨? {popup.PopupName}");
            /// if (!popups.ContainsKey(popup.PopupName))
            // {
            //     return;
            // }

            // popups.Remove(popup.PopupName);
        }

        // 팝업이 열렸을 때 호출
        internal void AddOpenPopup(PopupBase popup)
        {
            if (openedPopups.Contains(popup))
            {
                openedPopups.Remove(popup);
            }

            openedPopups.AddLast(popup);
            RefreshOrder();
        }

        // 팝업이 닫혔을 때 호출
        internal void RemoveOpenPopup(PopupBase popup)
        {
            if (!openedPopups.Contains(popup))
            {
                return;
            }

            openedPopups.Remove(popup);
            RefreshOrder();
        }

        // 팝업이 열린 순서대로 정렬
        private void RefreshOrder()
        {
            int i = 0;
            foreach (var p in openedPopups)
            {
                p.SortingOrder = i++;
            }
        }

        // 등록된 팝업을 찾아 엶.
        public PopupBase Get(string key)
        {
            if (!popups.TryGetValue(key, out var popup))
            {
                Debug.LogError($"{key}이름의 팝업이 존재하지 않습니다.");
                return null;
            }
            
            return popup;
        }

        
        public T Get<T>(string key) where T : MonoBehaviour
        {
            if (!popups.TryGetValue(key, out var popup))
            {
                Debug.LogError($"{key}이름의 팝업이 존재하지 않습니다.");
                return null;
            }

            if (popup is T t)
            {
                return t;
            }

            T c = popup.GetComponent<T>();

            if (c != null)
            {
                return c;
            }

            Debug.LogError($"{key}팝업이 {typeof(T)}가 아니거나 컴포넌트를 포함하지 않습니다.");
            return null;
        }

        // public bool Close(string key)
        // {
        //     if (!popups.TryGetValue(key, out var popup))
        //     {
        //         Debug.LogError($"{key}이름의 팝업이 존재하지 않습니다.");
        //         return false;
        //     }

        //     if (!openedPopups.Contains(popup))
        //     {
        //         Debug.LogError($"{key} 팝업이 열려있는 상태가 아닙니다.");
        //         return false;
        //     }

        //     return true;
        // }

        public bool CloseLast()
        {
            if (openedPopups.Count == 0)
            {
                return false;
            }

            openedPopups.Last.Value.Close();
            return true;
        }

        public void CloseAll()
        {
            foreach (var p in openedPopups.ToList())
            {
                p.Close();
            }
        }
    }
}