using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI
{
	interface IDIContainer
	{
		void AddType(Type abstractType, IImplementation implementation, bool replace = false);
		bool Contains(Type type);
		bool TryGetInstance<AbstractT>(out AbstractT instance) where AbstractT : class;
		AbstractT GetInstance<AbstractT>() where AbstractT : class;
		bool TryGetInstance(Type t, out object instance);
		object GetInstance(Type t);
		Type GetImplementationType(Type t);
		IImplementation GetImplementation(Type t);
		void Clear();
	}
	interface IImplementation
	{
		object GetObject();
		Type GetImplementationType();
		bool IsAutoConstructed { get; }
	}

	class Transient<T> : IImplementation where T : class
	{
		Func<T> factory = null;

		public bool IsAutoConstructed { get; private set; }

		public object GetObject()
		{
			return factory?.Invoke();
		}

		public Type GetImplementationType() => typeof(T);

		public Transient(Func<T> factory, bool isAutoConstructed)
		{
			this.factory = factory;
			IsAutoConstructed = isAutoConstructed;
		}
	}

	class Singleton<T> : IImplementation where T : class
	{
		T obj;
		public bool IsAutoConstructed { get; private set; }

		public object GetObject()
		{
			return obj;
		}
		public Type GetImplementationType() => typeof(T);

		public Singleton(T obj, bool isAutoConstructed)
		{
			this.obj = obj;
			IsAutoConstructed = isAutoConstructed;
		}
	}

	class DIContainer : IDIContainer
	{
		Dictionary<Type, IImplementation> abstractionsRealizations = new Dictionary<Type, IImplementation>();

		public void AddType(Type abstractType, IImplementation implementation, bool replace = false)
		{
			if (!abstractionsRealizations.ContainsKey(abstractType))
				abstractionsRealizations.Add(abstractType, implementation);
			else if (replace)
				abstractionsRealizations[abstractType] = implementation;
			else
				throw new Exception($"Type {abstractType.FullName} is already in DI container. " +
					"If you want to replace it, use replace flag");
		}

		public void Clear()
		{
			abstractionsRealizations.Clear();
		}

		public bool Contains(Type type)
		{
			return abstractionsRealizations.ContainsKey(type);
		}

		public IImplementation GetImplementation(Type t)
		{
			return abstractionsRealizations[t];
		}

		public Type GetImplementationType(Type t)
		{
			return abstractionsRealizations[t].GetImplementationType();
		}

		public AbstractT GetInstance<AbstractT>() where AbstractT : class
		{
			return (AbstractT)abstractionsRealizations[typeof(AbstractT)].GetObject();
		}

		public object GetInstance(Type t)
		{
			return abstractionsRealizations[t].GetObject();
		}

		public bool TryGetInstance<AbstractT>(out AbstractT instance) where AbstractT : class
		{
			if (abstractionsRealizations.TryGetValue(typeof(AbstractT), out IImplementation value))
			{
				instance = (AbstractT)value.GetObject();
				return true;
			}
			instance = null;
			return false;
		}

		public bool TryGetInstance(Type t, out object instance)
		{
			if (abstractionsRealizations.TryGetValue(t, out IImplementation value))
			{
				instance = value.GetObject();
				return true;
			}
			instance = null;
			return false;
		}
	}
}
