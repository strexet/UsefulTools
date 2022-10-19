﻿using UnityEditor;
using UnityEngine;

namespace UsefulTools.Editor
{
    public class UsefulShortcuts : UnityEditor.Editor
    {
        [MenuItem("Tools/UsefulTools/Pause Game _.")]
        public static void PauseGame() => Debug.Break();

#region uGUITools
        [MenuItem("Tools/UsefulTools/uGUI/Anchors to Corners %[")]
        private static void AnchorsToCorners()
        {
            foreach (var transform in Selection.transforms)
            {
                var t = transform as RectTransform;
                var pt = Selection.activeTransform.parent as RectTransform;

                if (t == null || pt == null)
                {
                    return;
                }

                var newAnchorsMin = new Vector2(t.anchorMin.x + t.offsetMin.x / pt.rect.width,
                    t.anchorMin.y + t.offsetMin.y / pt.rect.height);

                var newAnchorsMax = new Vector2(t.anchorMax.x + t.offsetMax.x / pt.rect.width,
                    t.anchorMax.y + t.offsetMax.y / pt.rect.height);

                t.anchorMin = newAnchorsMin;
                t.anchorMax = newAnchorsMax;
                t.offsetMin = t.offsetMax = new Vector2(0, 0);
            }
        }

        [MenuItem("Tools/UsefulTools/uGUI/Corners to Anchors %]")]
        private static void CornersToAnchors()
        {
            foreach (var transform in Selection.transforms)
            {
                var t = transform as RectTransform;

                if (t == null)
                {
                    return;
                }

                t.offsetMin = t.offsetMax = new Vector2(0, 0);
            }
        }

        [MenuItem("Tools/UsefulTools/uGUI/Mirror Horizontally Around Anchors %;")]
        private static void MirrorHorizontallyAnchors() => MirrorHorizontally(false);

        [MenuItem("Tools/UsefulTools/uGUI/Mirror Horizontally Around Parent Center %:")]
        private static void MirrorHorizontallyParent() => MirrorHorizontally(true);

        private static void MirrorHorizontally(bool mirrorAnchors)
        {
            foreach (var transform in Selection.transforms)
            {
                var t = transform as RectTransform;
                var pt = Selection.activeTransform.parent as RectTransform;

                if (t == null || pt == null)
                {
                    return;
                }

                if (mirrorAnchors)
                {
                    var oldAnchorMin = t.anchorMin;
                    t.anchorMin = new Vector2(1 - t.anchorMax.x, t.anchorMin.y);
                    t.anchorMax = new Vector2(1 - oldAnchorMin.x, t.anchorMax.y);
                }

                var oldOffsetMin = t.offsetMin;
                t.offsetMin = new Vector2(-t.offsetMax.x, t.offsetMin.y);
                t.offsetMax = new Vector2(-oldOffsetMin.x, t.offsetMax.y);

                t.localScale = new Vector3(-t.localScale.x, t.localScale.y, t.localScale.z);
            }
        }

        [MenuItem("Tools/UsefulTools/uGUI/Mirror Vertically Around Anchors %'")]
        private static void MirrorVerticallyAnchors() => MirrorVertically(false);

        [MenuItem("Tools/UsefulTools/uGUI/Mirror Vertically Around Parent Center %\"")]
        private static void MirrorVerticallyParent() => MirrorVertically(true);

        private static void MirrorVertically(bool mirrorAnchors)
        {
            foreach (var transform in Selection.transforms)
            {
                var t = transform as RectTransform;
                var pt = Selection.activeTransform.parent as RectTransform;

                if (t == null || pt == null)
                {
                    return;
                }

                if (mirrorAnchors)
                {
                    var oldAnchorMin = t.anchorMin;
                    t.anchorMin = new Vector2(t.anchorMin.x, 1 - t.anchorMax.y);
                    t.anchorMax = new Vector2(t.anchorMax.x, 1 - oldAnchorMin.y);
                }

                var oldOffsetMin = t.offsetMin;
                t.offsetMin = new Vector2(t.offsetMin.x, -t.offsetMax.y);
                t.offsetMax = new Vector2(t.offsetMax.x, -oldOffsetMin.y);

                t.localScale = new Vector3(t.localScale.x, -t.localScale.y, t.localScale.z);
            }
        }
#endregion
    }
}