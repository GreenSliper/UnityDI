using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DI
{
    interface IServices
    {
        void ConfigureServices();
    }
    public class Services : MonoBehaviour
    {
        /// <summary>
		/// Should be loaded before other components start working
		/// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoadRuntimeMethod()
        {
            var svc = FindObjectsOfType<MonoBehaviour>().OfType<IServices>().FirstOrDefault();
            if (svc != null)
                svc.ConfigureServices();
            DI.InitInjectQueue();
        }

        interface IA { }
        interface IB { }
        interface IC { }
        interface ID { }
        interface IE { }
        interface IF { }

        class A : IA{
            IB b; IC c;
            public A(IB b, IC c) { this.b = b; this.c = c; }
        }
        class B : IB{
            ID d; IE e;
            public B(IA a) { }
            public B(ID d, IE e) { this.d = d; this.e = e; }
        }
        class C : IC{
            string name = "C";
        }
        class D : ID{
            string name = "D";
        }
        class E : IE{
            IF f;
            public E(IA a) { }

            public E(IF f) { this.f = f; }
        }
        class F : IF{
            string name = "F";
        }

        bool init = false;
        /// <summary>
		/// Describe all your dependencies here. In other scenes you may call DI.ClearDependencies before re-injecting other ones
		/// </summary>
        public void ConfigureServices()
        {
            if (init)
                return;
            init = true;
            //Always active mono for coroutines (example)
            DI.AddSingleton<MonoBehaviour, Services>(this);
            //TEST (includes circular reference traps)
            DI.AddTransient<IF, F>();
            DI.AddTransient<IE, E>();
            DI.AddTransient<ID, D>();
            DI.AddTransient<IC, C>();
            DI.AddTransient<IB, B>();
            DI.AddTransient<IA, A>();
            var a = DI.GetType<IA>();
            Debug.Log("Services initialized");
        }
        private void Awake()
        {
            ConfigureServices();
        }
    }
}
