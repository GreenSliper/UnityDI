using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace DI
{
	static class DIConstruction
	{
		public static Func<T> CreateFactory<T>(IDIContainer container) where T : class
		{
			if (CanConstructType(typeof(T), container))
			{
				return () =>
				{
					if(TryConstructType(typeof(T), container, out var result))
						return (T)result;
					throw new Exception($"Type {typeof(T).FullName} cannot be constructed!");
				};
			}
			throw new Exception($"Type {typeof(T).FullName} cannot be constructed!");
		}

		static bool CanConstructType(Type t, IDIContainer container, HashSet<Type> stack = null)
		{
			if (stack == null)
				stack = new HashSet<Type>();
			stack.Add(t); //avoid circular references
			
			foreach (var c in t.GetTypeInfo()
				.DeclaredConstructors
				.Where(c => c.IsPublic).OrderBy(x => x.GetParameters().Length))
			{
				var parameters = c.GetParameters();

				bool constructorImplementable = true;
				int i = 0;
				foreach (var p in parameters)
				{
					if (stack.Any(x => p.ParameterType.IsAssignableFrom(x))) //if there is a circular reference
						constructorImplementable = false;
					else if (container.Contains(p.ParameterType)) //in container
					{
						var impl = container.GetImplementation(p.ParameterType);
						//but cannot create implementation type object
						if (impl.IsAutoConstructed && !CanConstructType(impl.GetImplementationType(), container, new HashSet<Type>(stack)))
							constructorImplementable = false;
					}
					//not in container, cannot be constructed
					else if (!CanConstructType(p.ParameterType, container, new HashSet<Type>(stack)))
						constructorImplementable = false;

					if (!constructorImplementable)
						break;
				}
				if (constructorImplementable)
					return true;
			}
			return false;
		}

		static bool TryConstructType(Type t, IDIContainer container, out object result, HashSet<Type> stack = null)
		{
			if (stack == null)
				stack = new HashSet<Type>();

			stack.Add(t);
			foreach (var c in t.GetTypeInfo()
				.DeclaredConstructors
				.Where(c => c.IsPublic).OrderBy(x => x.GetParameters().Length))
			{
				var parameters = c.GetParameters();
				object[] prmVals = new object[parameters.Length];

				bool constructorImplementable = true;
				int i = 0;
				foreach (var p in parameters)
				{
					if(stack.Any(x => p.ParameterType.IsAssignableFrom(x))) //if there is a circular reference
					{
						constructorImplementable = false;
						break;
					}
					if (!container.Contains(p.ParameterType))
					{
						//if not in container, try to construct
						if (!TryConstructType(p.ParameterType, container, out var param, new HashSet<Type>(stack)))
						{
							constructorImplementable = false;
							break;
						}
						prmVals[i] = param;
					}
					else
					{
						var impl = container.GetImplementation(p.ParameterType);
						if (impl.IsAutoConstructed)
						{
							//if autoconstructed, try construct implementation
							if (!TryConstructType(impl.GetImplementationType(), container, out var param, new HashSet<Type>(stack)))
							{
								constructorImplementable = false;
								break;
							}
							prmVals[i] = param;
						}
						else //otherwise call user factory (or get singleton etc)
							prmVals[i] = container.GetInstance(p.ParameterType);
					}
					i++;
				}
				if (constructorImplementable)
				{
					result = c.Invoke(prmVals);
					return true;
				}
			}
			result = null;
			return false;
		}
	}
}