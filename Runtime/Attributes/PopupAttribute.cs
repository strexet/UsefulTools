using System;
using UnityEngine;

namespace UsefulTools.Runtime.Attributes
{
    public class PopupAttribute : PropertyAttribute
    {
        public readonly Type ContainingType;
        public readonly string PropertyName;

        public PopupAttribute(Type containingType, string propertyName)
        {
            ContainingType = containingType;
            PropertyName = propertyName;
        }
    }
}