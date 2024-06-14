using System;

namespace RV.Systems.EventBus
{
    public interface IEventBinding<T> where T : IEvent
    {
        Action<T> OnEvent { get; set; }
        Action OnEventNoArgs { get; set; }
    }

    public class EventBinding<T> : IEventBinding<T> where T : IEvent
    {
        private Action<T> onEvent = _ => { };
        private Action onEventNoArgs = () => { };

        public Action<T> OnEvent
        {
            get => onEvent;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value), "OnEvent cannot be null.");
                onEvent = value;
            }
        }

        public Action OnEventNoArgs
        {
            get => onEventNoArgs;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value), "OnEventNoArgs cannot be null.");
                onEventNoArgs = value;
            }
        }

        public EventBinding(Action<T> onEvent)
        {
            OnEvent = onEvent ?? throw new ArgumentNullException(nameof(onEvent), "onEvent cannot be null.");
        }

        public EventBinding(Action onEventNoArgs)
        {
            OnEventNoArgs = onEventNoArgs ?? throw new ArgumentNullException(nameof(onEventNoArgs), "onEventNoArgs cannot be null.");
        }

        public void Add(Action onEvent)
        {
            if (onEvent == null)
                throw new ArgumentNullException(nameof(onEvent), "onEvent cannot be null.");
            onEventNoArgs += onEvent;
        }

        public void Remove(Action onEvent)
        {
            if (onEvent == null)
                throw new ArgumentNullException(nameof(onEvent), "onEvent cannot be null.");
            onEventNoArgs -= onEvent;
        }

        public void Add(Action<T> onEvent)
        {
            if (onEvent == null)
                throw new ArgumentNullException(nameof(onEvent), "onEvent cannot be null.");
            this.onEvent += onEvent;
        }

        public void Remove(Action<T> onEvent)
        {
            if (onEvent == null)
                throw new ArgumentNullException(nameof(onEvent), "onEvent cannot be null.");
            this.onEvent -= onEvent;
        }
    }
}
