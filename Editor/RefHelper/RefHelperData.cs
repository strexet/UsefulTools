using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace UsefulTools.Editor.RefHelper
{
    [Serializable]
    public class RefHelperData
    {
        public int LastSelectedObjectsMaxCount;
        public List<Object> ReferencedObjects;
        public List<Object> LastSelectedObjects;
    }
}