
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class Tweener
{
    private static object _syncObj = new object();
    private static Tweener _inst = null;

    public static Tweener get()
    {
        if (_inst == null)
        {
            lock (_syncObj)
            {
                if (_inst == null)
                {
                    _inst = new Tweener();
                }
            }
        }

        return _inst;
    }

    private bool _isVisible = false;

    public bool isVisible
    {
        get
        {
            return _isVisible;
        }
        set
        {
            _isVisible = value;
        }
    }

    private Dictionary<object, TransSet> _registry;
    //private ComponentContainer _components;
    List<object> _keys = new List<object>();

    public Tweener()
    {
        _registry = new Dictionary<object, TransSet>();
        //_components = new ComponentContainer();
    }



    class TransSet
    {
        public object obj;
        public List<CompletedCallback> callbacks = new List<CompletedCallback>();

        private List<string> _names = new List<string>();
        private Dictionary<string, IChangeSet> _changeSets = new Dictionary<string, IChangeSet>();

        public bool update(GameTime gameTime)
        {

            for (int i = 0; i < _names.Count;)
            {
                string name = _names[i];

                IChangeSet changeSet = _changeSets[name];

                if (!changeSet.update(obj, gameTime))
                {
                    _names.RemoveAt(i);
                    _changeSets.Remove(name);
                    continue;
                }

                ++i;
            }

            bool isDone = _names.Count == 0;

            if (isDone)
            {
                foreach (CompletedCallback callback in callbacks)
                {
                    callback(obj);
                }
            }


            return !isDone;
        }



        public void updateProperties(GameTime gameTime, IDictionary<string, object> properties, double duration, ETransition transition)
        {
            double startTime = gameTime.TotalGameTime.TotalSeconds;
            double endTime = startTime + duration;

            foreach (KeyValuePair<string, object> item in properties)
            {
                string name = item.Key;

                IProperty property = null;

                System.Reflection.PropertyInfo proInfo = obj.GetType().GetProperty(name);

                if (proInfo != null)
                {
                    property = new PropertyProperty() { property = proInfo };
                }


                if (property == null)
                {
                    System.Reflection.FieldInfo fieldInfo = obj.GetType().GetField(name);

                    if (fieldInfo != null)
                    {
                        property = new FieldProperty() { field = fieldInfo };
                    }
                }

                if (property == null)
                {
                    continue;
                }

                Type proType = property.getType();
                IChangeSet changeSet = null;
                TransitionCall transCall = EaseUtils.easeLinear;


                switch (transition)
                {
                    case ETransition.EaseInElastic:
                        transCall = EaseUtils.easeInElastic;
                        break;
                    case ETransition.EaseOutElastic:
                        transCall = EaseUtils.easeOutElastic;
                        break;
                    case ETransition.EaseInOutElastic:
                        transCall = EaseUtils.easeInOutElastic;
                        break;
                    case ETransition.EaseInCubic:
                        transCall = EaseUtils.easeInCubic;
                        break;
                    case ETransition.EaseOutCubic:
                        transCall = EaseUtils.easeOutCubic;
                        break;
                    case ETransition.Linear:
                        transCall = EaseUtils.easeLinear;
                        break;
                }

                if (proType == typeof(float))
                {
                    float beginValue = (float)(property.getValue(obj, null));
                    float gotoValue = float.Parse(item.Value.ToString());
                    float changeValue = gotoValue - beginValue;

                    FloatSet floatSet = new FloatSet(transCall)
                    {
                        name = name,
                        beginValue = beginValue,
                        changeValue = changeValue,
                        startTime = startTime,
                        endTime = endTime,
                        duration = duration,
                        property = property,
                    };
                    changeSet = floatSet;


                }
                else if (proType == typeof(int))
                {
                    int beginValue = (int)(property.getValue(obj, null));
                    int gotoValue = int.Parse(item.Value.ToString());
                    int changeValue = gotoValue - beginValue;

                    changeSet = new IntSet(transCall)
                    {
                        name = name,
                        beginValue = beginValue,
                        changeValue = changeValue,
                        startTime = startTime,
                        endTime = endTime,
                        duration = duration,
                        property = property,
                    };

                }

                if (changeSet != null)
                {
                    if (!_names.Contains(name))
                    {
                        _names.Add(name);
                    }

                    _changeSets[name] = changeSet;
                }

            }
        }

        interface IProperty
        {
            object getValue(object obj, object[] index);
            void setValue(object obj, object value, object[] index);
            Type getType();
        }


        class PropertyProperty : IProperty
        {
            public System.Reflection.PropertyInfo property;

            public Type getType()
            {
                return property.PropertyType;
            }


            public void setValue(object obj, object value, object[] index)
            {
                property.SetValue(obj, value, index);
            }

            public object getValue(object obj, object[] index)
            {
                return property.GetValue(obj, index);
            }
        }

        class FieldProperty : IProperty
        {
            public System.Reflection.FieldInfo field;

            public Type getType()
            {
                return field.FieldType;
            }

            public object getValue(object obj, object[] index)
            {
                return field.GetValue(obj);
            }

            public void setValue(object obj, object value, object[] index)
            {
                field.SetValue(obj, value);
            }
        }

        public delegate double TransitionCall(double t, double b, double c, double d);

        interface IChangeSet
        {
            bool update(object obj, GameTime gameTime);
        }



        class IntSet : IChangeSet
        {

            public string name;
            public int beginValue;
            public int changeValue;
            public double startTime;
            public double endTime;
            public double duration;
            public IProperty property;
            private TransitionCall _transCall;

            public IntSet(TransitionCall transCall)
            {
                _transCall = transCall;
            }


            public bool update(object obj, GameTime gameTime)
            {
                int value2;
                if (gameTime.TotalGameTime.TotalSeconds < endTime)
                {
                    double t = gameTime.TotalGameTime.TotalSeconds - startTime;
                    value2 = (int)_transCall(t, beginValue, changeValue, duration);

                   // Console.WriteLine(string.Format("Tween t={0} value2={1} gameTime.TotalGameTime={2}", t, value2, gameTime.TotalGameTime));

                    property.setValue(obj, value2, null);
                    return true;
                }
                value2 = beginValue + changeValue;
                property.setValue(obj, value2, null);

                return false;
            }

        }

        class FloatSet : IChangeSet
        {
            public string name;
            public float beginValue;
            public float changeValue;
            public double startTime;
            public double endTime;
            public double duration;
            public IProperty property;

            private TransitionCall _transCall;

            public FloatSet(TransitionCall transCall)
            {
                _transCall = transCall;
            }

            public bool update(object obj, GameTime gameTime)
            {
                float value2;
                if (gameTime.TotalGameTime.TotalSeconds < endTime)
                {
                    double t = gameTime.TotalGameTime.TotalSeconds - startTime;
                    value2 = (float)_transCall(t, beginValue, changeValue, duration);

                    //Debug.Log(string.Format("Tween t={0} value2={1}", t, value2));

                    property.setValue(obj, value2, null);
                    return true;
                }
                value2 = beginValue + changeValue;
                property.setValue(obj, value2, null);

                return false;
            }

        }
    }


    class RegisterInfo
    {
        public ETransition transition;
        public object obj;
        public float time;
        public double startTime;
        public float delay;
        public IDictionary<string, object> properties;
        public CompletedCallback callback;
    }

    private List<RegisterInfo> _registerInfosInit = new List<RegisterInfo>();
    private List<RegisterInfo> _registerInfos = new List<RegisterInfo>();

    private bool _isDestory = false;

    public bool isDestory
    {
        get
        {
            return _isDestory;
        }
    }

    internal void register(object obj, float time, float delay, ETransition transition, IDictionary<string, object> properties, CompletedCallback callback)
    {
        _registerInfosInit.Add(new RegisterInfo()
        {
            obj = obj,
            time = time,
            delay = delay,
            transition = transition,
            // startTime = Time.time + delay,
            properties = properties,
            callback = callback,
        });

    }


    void _register(RegisterInfo info, GameTime gameTime)
    {
        object obj = info.obj;


        TransSet trans;
        if (!_registry.ContainsKey(obj))
        {
            trans = new TransSet()
            {
                obj = obj
            };
            _registry[obj] = trans;

            if (!_keys.Contains(obj))
            {
                _keys.Add(obj);
            }

        }

        trans = _registry[obj];
        trans.updateProperties(gameTime, info.properties, info.time, info.transition);

        if (info.callback != null)
        {
            trans.callbacks.Add(info.callback);
        }
    }

    public delegate void CompletedCallback(object obj);

    public enum ETransition
    {
        Linear,
        EaseInCubic,
        EaseOutCubic,
        EaseInElastic,
        EaseOutElastic,
        EaseInOutElastic,
    }



    public static void add(object obj, float time, float delay, ETransition transition, IDictionary<string, object> properties, CompletedCallback callback = null)
    {
        Tweener.get().register(obj, time, delay, transition, properties, callback);
    }

    public void destory()
    {

    }


    public void update(GameTime gameTime)
    {
        if (_registerInfosInit.Count > 0)
        {
            for (int i = 0; i < _registerInfosInit.Count;)
            {
                RegisterInfo info = _registerInfosInit[i];
                info.startTime = gameTime.TotalGameTime.TotalSeconds + info.delay;

                _registerInfosInit.RemoveAt(i);
                _registerInfos.Add(info);
            }
        }

        if (_registerInfos.Count > 0)
        {
            for (int i = 0; i < _registerInfos.Count;)
            {
                RegisterInfo info = _registerInfos[i];
                if (gameTime.TotalGameTime.TotalSeconds >= info.startTime)
                {
                    _register(info, gameTime);

                    _registerInfos.RemoveAt(i);
                    continue;
                }

                ++i;
            }
        }

        if (_keys.Count > 0)
        {
            for (int i = 0; i < _keys.Count;)
            {
                object obj = _keys[i];

                TransSet trans = _registry[obj];
                if (!trans.update(gameTime))
                {
                    _keys.RemoveAt(i);

                    if (_registry.ContainsKey(obj))
                    {
                        _registry.Remove(obj);
                    }

                    continue;
                }

                if (_registry.ContainsKey(obj))
                {

                }

                ++i;
            }
        }

        //_components.update(game, gameTime);
    }

   
}


