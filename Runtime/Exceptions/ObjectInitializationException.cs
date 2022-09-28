using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoftBoiledGames.ObjectPooler.Exceptions
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
