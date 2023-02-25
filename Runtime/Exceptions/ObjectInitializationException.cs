using System;
using System.Collections.Generic;
using UnityEngine;

namespace PixelSparkStudio.ObjectPooler.Exceptions
{
    public class ObjectInitializationException : Exception
    {
        private object error;

        public ObjectInitializationException(object error)
        {
            this.error = error;
        }
    }
}
