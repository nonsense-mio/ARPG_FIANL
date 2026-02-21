using System;

namespace Framework
{
    public interface IEasyEvent
    {
        IUnRegister Register(Action onEvent);
    }

    public class EasyEvent : IEasyEvent
    {
        private Action mOnEvent = () => { };

        public IUnRegister Register(Action onEvent)
        {
            mOnEvent += onEvent;
            return new CustomUnRegister(() => { UnRegister(onEvent); });
        }

        public IUnRegister RegisterWithACall(Action onEvent)
        {
            onEvent.Invoke();
            return Register(onEvent);
        }

        public void UnRegister(Action onEvent) => mOnEvent -= onEvent;

        public void Trigger() => mOnEvent?.Invoke();
    }

    public class EasyEvent<T> : IEasyEvent
    {
        private Action<T> mOnEvent = e => { };

        public IUnRegister Register(Action<T> onEvent)
        {
            mOnEvent += onEvent;
            return new CustomUnRegister(() => { UnRegister(onEvent); });
        }

        public void UnRegister(Action<T> onEvent) => mOnEvent -= onEvent;


        public void Trigger(T t) => mOnEvent?.Invoke(t);

        IUnRegister IEasyEvent.Register(Action onEvent)
        {
            return Register(Action);
            void Action(T _) => onEvent();
        }
    }

    public class EasyEvent<T, K> : IEasyEvent
    {
        private Action<T, K> mOnEvent = (t, k) => { };

        public IUnRegister Register(Action<T, K> onEvent)
        {
            mOnEvent += onEvent;
            return new CustomUnRegister(() => { UnRegister(onEvent); });
        }

        public void UnRegister(Action<T, K> onEvent) => mOnEvent -= onEvent;

        public void Trigger(T t, K k) => mOnEvent?.Invoke(t, k);

        IUnRegister IEasyEvent.Register(Action onEvent)
        {
            return Register(Action);
            void Action(T _, K __) => onEvent();
        }
    }

    public class EasyEvent<T, K, S> : IEasyEvent
    {
        private Action<T, K, S> mOnEvent = (t, k, s) => { };

        public IUnRegister Register(Action<T, K, S> onEvent)
        {
            mOnEvent += onEvent;
            return new CustomUnRegister(() => { UnRegister(onEvent); });
        }

        public void UnRegister(Action<T, K, S> onEvent) => mOnEvent -= onEvent;

        public void Trigger(T t, K k, S s) => mOnEvent?.Invoke(t, k, s);

        IUnRegister IEasyEvent.Register(Action onEvent)
        {
            return Register(Action);
            void Action(T _, K __, S ___) => onEvent();
        }
    }

    public struct CustomUnRegister : IUnRegister
    {
        private Action mOnUnRegister { get; set; }
        public CustomUnRegister(Action onUnRegister) => mOnUnRegister = onUnRegister;

        public void UnRegister()
        {
            mOnUnRegister?.Invoke();
            mOnUnRegister = null;
        }

    }
}