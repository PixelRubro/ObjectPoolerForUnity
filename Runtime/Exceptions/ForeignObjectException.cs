using System;
using System.Collections.Generic;
using UnityEngine;

namespace PixelSpark.ObjectPooler.Exceptions
{
    public class ForeignObjectException : Exception
    {
        private object error;

        public ForeignObjectException(object error)
        {
            this.error = error;
        }
    }
}
