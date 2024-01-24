using UnityEngine;

namespace UsefulTools.Editor.Tools
{
    public interface ITagHelper
    {
        string GetTagStringFromObject(GameObject objectWithTag, string payload = "");
        void SetTagStringForObject(string tag, GameObject objectWithTag, string payload = "");
    }
}