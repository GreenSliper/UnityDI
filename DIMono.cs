using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DI
{
    /// <summary>
    /// Used for field-injection. Should be inherited, or the class should call DI.InjectFields directly
    /// </summary>
    public class DIMono : MonoBehaviour
    {
        protected virtual void InjectAll() => DI.InjectFields(this);

        public virtual void Awake()
        {
            InjectAll();
        }
    }
}