using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DI
{
    /// <summary>
	/// Used for field injections (DIMono)
	/// </summary>

    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class DInjectAttribute : Attribute
    {
        public DInjectAttribute()
        {
        }
    }
}