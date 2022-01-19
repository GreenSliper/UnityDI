using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DI
{
	public static class DI
	{
		static IDIContainer DIContainer = new DIContainer();
		static List<DIClass> injectQueue = new List<DIClass>();

		/// <summary>
		/// Used for inspector-serialized classes for working with DInject-ed fields
		/// </summary>
		/// <param name="gameObject"></param>
		public static void AddToInjectQueue(DIClass gameObject)
		{
			if (!injectQueue.Contains(gameObject))
				injectQueue.Add(gameObject);
		}
		public static void InitInjectQueue()
		{
			foreach (var c in injectQueue)
				InjectFields(c);
			injectQueue.Clear();
		}

		public static void InjectFields<T>(T obj)
		{
			foreach (var field in obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
			{
				if (field.GetCustomAttributes(false).Any(x => x.GetType() == typeof(DInjectAttribute)))
					field.SetValue(obj, GetType(field.FieldType));
			}
		}


		/// <summary>
		/// Creates object each time it's requesterd. Uses factory to produce objects
		/// </summary>
		/// <typeparam name="T">implementation</typeparam>
		/// <param name="replace">Replace existing implementatiom</param>
		public static void AddTransient<T>(Type abstraction, Func<T> factory, bool replace = false) where T : class
		{
			if (abstraction.IsAssignableFrom(typeof(T)))
				DIContainer.AddType(abstraction, new Transient<T>(factory, false), replace);
			else
				throw new Exception($"Type {typeof(T).FullName} does not implement or inherit {abstraction.FullName}");
		}

		/// <summary>
		/// Creates object each time it's requesterd. Uses DIConstruction-defined constructor 
		/// </summary>
		/// <typeparam name="A">Abstraction</typeparam>
		/// <typeparam name="R">Implementation</typeparam>
		/// <param name="replace">Replace existing implementatiom</param>
		public static void AddTransient<A, R>(bool replace = false) where R : class, A
		{
			if (typeof(A).IsAssignableFrom(typeof(R)))
				DIContainer.AddType(typeof(A), new Transient<R>(DIConstruction.CreateFactory<R>(DIContainer), true), replace);
			else
				throw new Exception($"Type {typeof(R).FullName} does not implement or inherit {typeof(A).FullName}");
		}

		/// <summary>
		/// Creates object immediately (once). Uses DIConstruction-defined constructor
		/// </summary>
		/// <typeparam name="A">Abstraction</typeparam>
		/// <typeparam name="R">Implementation</typeparam>
		/// <param name="replace">Replace existing implementatiom</param>
		public static void AddSingleton<A, R>(bool replace = false) where R : class, A
		{
			DIContainer.AddType(typeof(A), new Singleton<R>(
				DIConstruction.CreateFactory<R>(DIContainer).Invoke(), true),
				replace);
		}

		/// <summary>
		/// Uses given object as singleton
		/// </summary>
		/// <typeparam name="A">Abstraction</typeparam>
		/// <typeparam name="R">Implementation</typeparam>
		/// <param name="obj">Object, that will be used</param>
		/// <param name="replace">Replace existing implementatiom</param>
		public static void AddSingleton<A, R>(R obj, bool replace = false) where R : class, A
		{
			DIContainer.AddType(typeof(A), new Singleton<R>(obj, false), replace);
		}

		/// <summary>
		/// Get implementation of a registered type
		/// </summary>
		/// <typeparam name="T">Requested type</typeparam>
		/// <returns>Implementation object</returns>
		public static T GetType<T>() where T : class 
			=> (T)GetType(typeof(T));

		public static object GetType(Type tp)
		{
			if (DIContainer.TryGetInstance(tp, out var val))
				return val;
			throw new Exception($"Requested type {tp.GetType().FullName} not registered");
		}

		public static void ClearDependencies()
		{
			DIContainer.Clear();
			injectQueue.Clear();
		}
	}
}