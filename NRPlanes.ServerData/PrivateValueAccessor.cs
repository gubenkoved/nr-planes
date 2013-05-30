using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace NRPlanes.ServerData
{
    /// <summary>
    /// Gives possibility to change private values by reflection.
    /// Can be used with private fields and properties.
    /// Class can be used only in this assembly
    /// </summary>
    internal class PrivateValueAccessor
    {
        private Action<object, object> _setValueDelegate;

        public PrivateValueAccessor(Type instanceType, string valueName)
        {
            FieldInfo fInfo = instanceType.GetField(valueName, BindingFlags.Instance | BindingFlags.NonPublic);

            if (fInfo != null) // we have field
            {
                _setValueDelegate = fInfo.SetValue;
            }
            else // we have property with non-public setetter
            {
                // search public property
                PropertyInfo pInfo = instanceType.GetProperty(valueName, BindingFlags.Instance | BindingFlags.Public); 
                // get private setter method
                MethodInfo mInfo = pInfo.GetSetMethod(true);

                _setValueDelegate = (obj, value) => mInfo.Invoke(obj, new[] { value });
            }
        }

        public void SetValue(object instance, object value)
        {
            _setValueDelegate.Invoke(instance, value);
        }
    }
}
