using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DI
{
    /// <summary>
    /// Used for serialized classes, which do NOT call the constructor in the direct way
    /// </summary>
    public class DIClass
    {
        public DIClass() => DI.AddToInjectQueue(this);
    }
}