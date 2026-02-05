using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    #region Architecture

    public interface IArchitecture
    {
        /// <summary>
        /// 注册系统
        /// </summary>
        /// <param name="instance"></param>
        /// <typeparam name="T"></typeparam>
        void RegisterSystem<T>(T instance) where T : ISystem;
        /// <summary>
        /// 注册Model
        /// </summary>
        /// <param name="instance"></param>
        /// <typeparam name="T"></typeparam>
        void RegisterModel<T>(T instance) where T : IModel;

        /// <summary>
        /// 注册Utility
        /// </summary>
        /// <param name="utility"></param>
        /// <typeparam name="T"></typeparam>
        void RegisterUtility<T>(T utility) where T : IUtility;

        /// <summary>
        /// 获取System
        /// </summary>
        /// <typeparam name="T"></typeparam>
        T GetSystem<T>() where T : class, ISystem;

        /// <summary>
        /// 获取Model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        T GetModel<T>() where T : class, IModel;

        /// <summary>
        /// 获取工具
        /// </summary>
        /// <typeparam name="T"></typeparam>
        T GetUtility<T>() where T : class, IUtility;
        //命令相关
        void SendCommand<T>() where T : ICommand, new();
        void SendCommand<T>(T command) where T : ICommand;
        //查询
        TResult SendQuery<TResult>(IQuery<TResult> query);
        //事件相关
        void SendEvent<T>() where T : new();
        void SendEvent<T>(T e);
        IUnRegister RegisterEvent<T>(Action<T> onEvent);
        void UnRegisterEvent<T>(Action<T> onEvent);

    }

    public abstract class Architecture<T> : IArchitecture where T : Architecture<T>, new()
    {
        /// <summary>
        /// 是否初始化完成
        /// </summary>
        private bool inited = false;
        private List<IModel> models = new List<IModel>();
        private List<ISystem> systems = new List<ISystem>();
        //IOC容器
        private IOCContainer container = new IOCContainer();
        public static Action<T> OnRegissterPatch;
        private static T architecture;
        public static IArchitecture Interface
        {
            get
            {
                if (architecture == null)
                {
                    MakeSureArchitecture();
                }
                return architecture;
            }
        }
        //确保对象不为null
        static void MakeSureArchitecture()
        {
            if (architecture == null)
            {
                architecture = new T();
                architecture.Init();
                //回调函数 把对象传出去
                OnRegissterPatch?.Invoke(architecture);

                //IModel注册完后 初始化 这里的Init是IModel中的Init() 用来代替构造函数的初始化的
                foreach (var architectureModel in architecture.models)
                {
                    architectureModel.Init();
                }
                architecture.models.Clear();
                //ISystem 初始化
                foreach (var architectureSystem in architecture.systems)
                {
                    architectureSystem.Init();
                }
                architecture.systems.Clear();

                architecture.inited = true;
            }
        }

        protected abstract void Init();


        public void RegisterSystem<TSystem>(TSystem system) where TSystem : ISystem
        {
            system.SetArchitecture(this);
            architecture.container.Register<TSystem>(system);
            if (!inited)
                systems.Add(system);
            else
                system.Init();
        }
        public void RegisterModel<TModel>(TModel model) where TModel : IModel
        {
            model.SetArchitecture(this);
            architecture.container.Register<TModel>(model);
            if (!inited)
                models.Add(model);
            else
                model.Init();
        }
        public void RegisterUtility<TUtility>(TUtility utility) where TUtility : IUtility
        {
            container.Register<TUtility>(utility);
        }

        public TSystem GetSystem<TSystem>() where TSystem : class, ISystem
        {
            return container.Get<TSystem>();
        }
        public TModel GetModel<TModel>() where TModel : class, IModel
        {
            return container.Get<TModel>();
        }
        public TUtility GetUtility<TUtility>() where TUtility : class, IUtility
        {
            return container.Get<TUtility>();
        }

        public void SendCommand<TCommand>() where TCommand : ICommand, new()
        {
            var command = new TCommand();
            command.SetArchitecture(this);
            command.Execute();
        }

        public void SendCommand<TCommand>(TCommand command) where TCommand : ICommand
        {
            command.SetArchitecture(this);
            command.Execute();
        }

        private ITypeEventSystem typeEventSystem = new TypeEventSystem();
        public void SendEvent<TEvent>() where TEvent : new()
        {
            typeEventSystem.Send<TEvent>();
        }

        public void SendEvent<TEvent>(TEvent e)
        {
            typeEventSystem.Send<TEvent>(e);
        }

        public IUnRegister RegisterEvent<TEvent>(Action<TEvent> onEvent)
        {
            return typeEventSystem.Register<TEvent>(onEvent);
        }

        public void UnRegisterEvent<TEvent>(Action<TEvent> onEvent)
        {
            typeEventSystem.UnRegister<TEvent>(onEvent);
        }

        public TResult SendQuery<TResult>(IQuery<TResult> query)
        {
            query.SetArchitecture(this);
            return query.Do();
        }
    }
    #endregion

    #region Controller
    public interface IController : IBelongToArchitecture, ICanSendCommand, ICanGetSystem, ICanGetModel, ICanRegisterEvent, ICanSendQuery, ICanGetUtility
    {

    }
    #endregion

    #region System
    public interface ISystem : IBelongToArchitecture, ICanSetArchitecture, ICanGetModel, ICanGetUtility, ICanSendEvent, ICanRegisterEvent, ICanGetSystem
    {
        void Init();
    }

    public abstract class AbstractSystem : ISystem
    {
        private IArchitecture architecture;
        IArchitecture IBelongToArchitecture.GetArchitecture()
        {
            return architecture;
        }

        void ICanSetArchitecture.SetArchitecture(IArchitecture architecture)
        {
            this.architecture = architecture;
        }

        void ISystem.Init()
        {
            OnInit();
        }
        protected abstract void OnInit();
    }
    #endregion

    #region Model
    public interface IModel : IBelongToArchitecture, ICanSetArchitecture, ICanGetUtility, ICanSendEvent
    {
        //用该初始化方法代替构造函数初始化 对象构造完成后调用此方法 避免在构造函数中调用Get方法造成可能的构造函数的递归问题
        void Init();
    }
    //在这里限制Model层
    public abstract class AbstractModel : IModel
    {
        private IArchitecture architecture;
        IArchitecture IBelongToArchitecture.GetArchitecture()
        {
            return architecture;
        }

        void ICanSetArchitecture.SetArchitecture(IArchitecture architecture)
        {
            this.architecture = architecture;
        }


        void IModel.Init()
        {
            OnInit();
        }
        protected abstract void OnInit();

    }
    #endregion

    #region Utility
    public interface IUtility
    {

    }
    #endregion

    #region Command
    public interface ICommand : IBelongToArchitecture, ICanSetArchitecture, ICanGetModel, ICanGetSystem, ICanGetUtility, ICanSendEvent, ICanSendCommand, ICanSendQuery
    {
        void Execute();
    }

    public abstract class AbstractCommand : ICommand
    {
        private IArchitecture architecture;
        void ICommand.Execute()
        {
            OnExecute();
        }

        IArchitecture IBelongToArchitecture.GetArchitecture()
        {
            return architecture;
        }

        void ICanSetArchitecture.SetArchitecture(IArchitecture architecture)
        {
            this.architecture = architecture;
        }
        protected abstract void OnExecute();
    }
    #endregion

    #region Query
    public interface IQuery<TResult> : IBelongToArchitecture, ICanSetArchitecture, ICanGetModel, ICanGetSystem, ICanSendQuery
    {
        TResult Do();
    }

    public abstract class AbstractQuery<T> : IQuery<T>
    {
        public T Do()
        {
            return OnDo();
        }
        protected abstract T OnDo();

        private IArchitecture mArchitecture;

        public IArchitecture GetArchitecture()
        {
            return mArchitecture;
        }

        public void SetArchitecture(IArchitecture architecture)
        {
            mArchitecture = architecture;
        }
    }

    #endregion

    #region Rule
    public interface IBelongToArchitecture
    {
        IArchitecture GetArchitecture();
    }

    public interface ICanSetArchitecture
    {
        void SetArchitecture(IArchitecture architecture);
    }

    public interface ICanGetModel : IBelongToArchitecture
    {

    }
    public static class CanGetModelExtension
    {
        public static T GetModel<T>(this ICanGetModel self) where T : class, IModel
        {
            return self.GetArchitecture().GetModel<T>();
        }
    }

    public interface ICanGetSystem : IBelongToArchitecture
    {

    }

    public static class CanGetSystemExtension
    {
        public static T GetSystem<T>(this ICanGetSystem self) where T : class, ISystem
        {
            return self.GetArchitecture().GetSystem<T>();
        }
    }

    public interface ICanGetUtility : IBelongToArchitecture
    {

    }

    public static class CanGetUtilityExtension
    {
        public static T GetUtility<T>(this ICanGetUtility self) where T : class, IUtility
        {
            return self.GetArchitecture().GetUtility<T>();
        }
    }

    public interface ICanRegisterEvent : IBelongToArchitecture
    {

    }

    //为ICanRegisterEvent 接口 添加的拓展方法 注册事件和注销事件
    public static class CanRegisterEventExtension
    {
        public static IUnRegister RegisterEvent<T>(this ICanRegisterEvent self, Action<T> onEvent)
        {
            return self.GetArchitecture().RegisterEvent<T>(onEvent);
        }

        public static void unRegisterEvent<T>(this ICanRegisterEvent self, Action<T> onEvent)
        {
            self.GetArchitecture().UnRegisterEvent<T>(onEvent);
        }
    }

    public interface ICanSendEvent : IBelongToArchitecture
    {

    }

    public static class CanSendEventExtension
    {
        public static void SendEvent<T>(this ICanSendEvent self) where T : new()
        {
            self.GetArchitecture().SendEvent<T>();
        }
        public static void SendEvent<T>(this ICanSendEvent self, T e)
        {
            self.GetArchitecture().SendEvent<T>(e);
        }
    }

    public interface ICanSendCommand : IBelongToArchitecture
    {

    }
    public static class CanSendCommandExtension
    {
        public static void SendCommand<T>(this ICanSendCommand self) where T : ICommand, new()
        {
            self.GetArchitecture().SendCommand<T>();
        }
        public static void SendCommand<T>(this ICanSendCommand self, T command) where T : ICommand, new()
        {
            self.GetArchitecture().SendCommand<T>(command);
        }
    }

    public interface ICanSendQuery : IBelongToArchitecture
    {

    }

    public static class CanSendQueryExtension
    {
        public static TResult SendQuery<TResult>(this ICanSendQuery self, IQuery<TResult> query)
        {
            return self.GetArchitecture().SendQuery(query);
        }
    }
    #endregion

    #region TypeEventSystem
    public interface ITypeEventSystem
    {
        void Send<T>() where T : new();
        void Send<T>(T e);
        IUnRegister Register<T>(Action<T> onEvent);
        void UnRegister<T>(Action<T> onEvent);
    }

    public interface IUnRegister
    {
        void UnRegister();
    }

    public struct TypeEventSystemUnRegister<T> : IUnRegister
    {
        public ITypeEventSystem typeEventSystem;
        public Action<T> OnEvent;
        //注销事件 该方法时自动注销时调用的
        public void UnRegister()
        {
            typeEventSystem.UnRegister<T>(OnEvent);
            typeEventSystem = null;
            OnEvent = null;
        }
    }


    public class UnRegisterTrigger : MonoBehaviour
    {
        //存储待注销事件的哈希表
        private HashSet<IUnRegister> unRegisters = new HashSet<IUnRegister>();
        //添加该组件时使用该方法记录待注销的事件
        public void AddUnRegister(IUnRegister unRegister)
        {
            unRegisters.Add(unRegister);
        }
        public void UnRegister()
        {
            //注销事件
            foreach (var unRegister in unRegisters)
            {
                unRegister.UnRegister();
            }
            unRegisters.Clear();
        }
    }
    public class UnRegisterOnDestroyTrigger : UnRegisterTrigger
    {
        private void OnDestroy()
        {
            UnRegister();
        }
    }

    public class UnRegisterOnDisableTrigger : UnRegisterTrigger
    {
        private void OnDisable()
        {
            UnRegister();
        }
    }

    public static class UnRegisterExtension
    {

        public static void UnRegisterWhenGameObjectDestroyed(this IUnRegister unRegister, GameObject gameObject)
        {
            var trigger = gameObject.GetComponent<UnRegisterOnDestroyTrigger>();
            if (trigger == null)
            {
                trigger = gameObject.AddComponent<UnRegisterOnDestroyTrigger>();
            }
            //添加待注销事件
            trigger.AddUnRegister(unRegister);
        }

        public static void UnRegisterWhenGameObjectDisabled(this IUnRegister unRegister, GameObject gameObject)
        {
            var trigger = gameObject.GetComponent<UnRegisterOnDisableTrigger>();
            if (trigger == null)
            {
                trigger = gameObject.AddComponent<UnRegisterOnDisableTrigger>();
            }
            //添加待注销事件
            trigger.AddUnRegister(unRegister);
        }
    }

    public class TypeEventSystem : ITypeEventSystem
    {
        public interface IRegistrations
        {

        }

        public class Registrations<T> : IRegistrations
        {
            public Action<T> OnEvent = e => { };

        }

        private Dictionary<Type, IRegistrations> eventRegistrationDic = new Dictionary<Type, IRegistrations>();
        public static readonly TypeEventSystem Global = new TypeEventSystem();

        public IUnRegister Register<T>(Action<T> onEvent)
        {
            var type = typeof(T);
            IRegistrations registrations;
            if (!eventRegistrationDic.TryGetValue(type, out registrations))
            {
                registrations = new Registrations<T>();
                eventRegistrationDic.Add(type, registrations);
            }
            (registrations as Registrations<T>).OnEvent += onEvent;

            return new TypeEventSystemUnRegister<T>()
            {
                OnEvent = onEvent,
                typeEventSystem = this
            };
        }

        public void UnRegister<T>(Action<T> onEvent)
        {
            var type = typeof(T);
            IRegistrations registrations;
            if (eventRegistrationDic.TryGetValue(type, out registrations))
            {
                (registrations as Registrations<T>).OnEvent -= onEvent;
            }
        }
        public void Send<T>() where T : new()
        {
            var e = new T();
            Send<T>(e);
        }
        public void Send<T>(T e)
        {
            var type = typeof(T);
            IRegistrations registrations;
            if (eventRegistrationDic.TryGetValue(type, out registrations))
            {
                (registrations as Registrations<T>).OnEvent?.Invoke(e);
            }
        }

    }

    public interface IOnEvent<T>
    {
        void OnEvent(T e);
    }

    public static class OnGlobalEventExtension
    {
        public static IUnRegister RegisterEvent<T>(this IOnEvent<T> self) where T : struct
        {
            return TypeEventSystem.Global.Register<T>(self.OnEvent);
        }

        public static void UnRegisterEvent<T>(this IOnEvent<T> self) where T : struct
        {
            TypeEventSystem.Global.UnRegister<T>(self.OnEvent);
        }
    }


    #endregion

    #region IOC
    public class IOCContainer
    {
        private Dictionary<Type, object> instances = new Dictionary<Type, object>();

        public void Register<T>(T instance)
        {
            Type key = typeof(T);
            if (instances.ContainsKey(key))
            {
                instances[key] = instance;
            }
            else
            {
                instances.Add(key, instance);
            }
        }

        public T Get<T>() where T : class
        {
            Type key = typeof(T);
            if (instances.TryGetValue(key, out var retInstance))
            {
                return retInstance as T;
            }
            return null;
        }
    }

    #endregion

    #region BindableProperty
    public class BindableProperty<T>
    {
        public BindableProperty(T defaultValue = default)
        {
            mValue = defaultValue;
        }
        private T mValue = default(T);

        public T Value
        {
            get => mValue;
            set
            {
                if (value == null && mValue == null) return;
                if (value != null && value.Equals(mValue)) return;

                mValue = value;
                onValueChanged?.Invoke(value);

            }
        }

        private Action<T> onValueChanged = v => { };

        public IUnRegister Register(Action<T> onValueChanged)
        {
            this.onValueChanged += onValueChanged;
            return new BindablePropertyUnRegister<T>()
            {
                BindabaleProperty = this,
                OnValueChanged = onValueChanged
            };
        }

        public IUnRegister RegisterWithInitValue(Action<T> onValueChanged)
        {
            onValueChanged(mValue);
            return Register(onValueChanged);
        }

        public static implicit operator T(BindableProperty<T> property)
        {
            return property.Value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public void UnRegister(Action<T> onValueChanged)
        {
            this.onValueChanged -= onValueChanged;
        }

    }

    public class BindablePropertyUnRegister<T> : IUnRegister
    {
        public BindableProperty<T> BindabaleProperty { get; set; }
        public Action<T> OnValueChanged { get; set; }

        //自动注销事件
        public void UnRegister()
        {
            BindabaleProperty.UnRegister(OnValueChanged);
            BindabaleProperty = null;
            OnValueChanged = null;
        }
    }
    #endregion

}

