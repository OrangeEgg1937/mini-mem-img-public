using System;
using UnityEngine.Serialization;

namespace Script.Core.GameSystem
{
    [Serializable]
    public class GSResult
    {
        public string err;
        public bool isSuccess;
    }
    
    [Serializable]
    public class GSResult<T> : GSResult
    {
        public T body;
    }
}