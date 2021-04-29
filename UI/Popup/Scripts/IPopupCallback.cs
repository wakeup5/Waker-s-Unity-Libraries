using UnityEngine.EventSystems;

namespace Waker.Popups
{
    public interface IPopupCallback : IEventSystemHandler
    {
        void OnOpened();
        void OnClosed();
        void OnConfirmed();
        void OnCanceled();
    }
}