using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoftBoiledGames.ObjectPooler.Exceptions
{
    public class DuplicateRegisteringException : Exception
    {
        private object error;

        public DuplicateRegisteringException(object error)
        {
            this.error = error;
        }
    }
}
