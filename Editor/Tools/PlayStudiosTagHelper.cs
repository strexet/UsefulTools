using UnityEngine;

namespace UsefulTools.Editor.Tools
{
    public class PlayStudiosTagHelper : ITagHelper
    {
        public string GetTagStringFromObject(GameObject objectWithTag, string payload = "")
        {
            string result = string.Empty;

            switch (payload)
            {
                case "GE2":
                    if (objectWithTag.TryGetComponent<GameEngine2.TagComponent>(out var ge2Tag))
                    {
                        result = ge2Tag.Tag;
                    }

                    break;

                case "Lua":
                    if (objectWithTag.TryGetComponent<PlayStudios.GameEngineLua.Tagging.TagComponent>(out var luaTag))
                    {
                        result = luaTag.Tag;
                    }

                    break;
            }

            if (string.IsNullOrEmpty(result))
            {
                Debug.LogError($"[ERROR]<color=red>{nameof(UsefulToolsWindow)}.{nameof(GetTagStringFromObject)}></color> "
                    + $"Tag not found for {objectWithTag.name}", objectWithTag);
            }

            return result;
        }

        public void SetTagStringForObject(string tag, GameObject objectWithTag, string payload = "")
        {
            switch (payload)
            {
                case "GE2":
                    if (objectWithTag.TryGetComponent<GameEngine2.TagComponent>(out var ge2Tag))
                    {
                        ge2Tag.Tag = tag;
                    }
                    else
                    {
                        Debug.LogError($"[ERROR]<color=red>{nameof(UsefulToolsWindow)}.{nameof(GetTagStringFromObject)}></color> "
                            + $"No GE2 TagComponent found for {objectWithTag.name}", objectWithTag);
                    }

                    break;

                case "Lua":
                    if (objectWithTag.TryGetComponent<PlayStudios.GameEngineLua.Tagging.TagComponent>(out var luaTag))
                    {
                        luaTag.Tag = tag;
                    }
                    else
                    {
                        Debug.LogError($"[ERROR]<color=red>{nameof(UsefulToolsWindow)}.{nameof(GetTagStringFromObject)}></color> "
                            + $"No Lua TagComponent found for {objectWithTag.name}", objectWithTag);
                    }

                    break;

                default:
                    Debug.LogError($"[ERROR]<color=red>{nameof(UsefulToolsWindow)}.{nameof(GetTagStringFromObject)}></color> "
                        + $"No TagComponent found for {objectWithTag.name}", objectWithTag);

                    break;
            }
        }
    }
}