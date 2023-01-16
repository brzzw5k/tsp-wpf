using System;
using System.Runtime.Serialization;

namespace tsp
{
    sealed class CustomizedBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            Type returntype = Type.GetType(String.Format("{0}, {1}", typeName, assemblyName));
            return returntype;
        }

        public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            base.BindToName(serializedType, out assemblyName, out typeName);
            assemblyName = "tsp, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null";
        }
    }
}