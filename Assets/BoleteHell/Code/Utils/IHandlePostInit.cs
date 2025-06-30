using UnityEngine;

namespace BoleteHell.Code.Utils
{
    // convenience, provides a default empty implementation for OnBeforeSerialize
    public interface IHandlePostInit : ISerializationCallbackReceiver
    {
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            
        }
    }
}