using System.IO;
using UnityEditor;
using UnityEngine;

namespace UsefulTools.Editor
{
    public class DumpRenderTexture : EditorWindow
    {
        Camera cameraWithRenderTexture;
        string fileName = "DumpedRenderTexture";
        int textureHeight = 800;

        [MenuItem("Tools/UsefulTools/Dump Render Texture To File")]
        static void Init()
        {
            // EditorWindow.GetWindow() will return the open instance of the specified window or create a new
            // instance if it can't find one. The second parameter is a flag for creating the window as a
            // Utility window; Utility windows cannot be docked like the Scene and Game view windows.
            var window = (DumpRenderTexture) GetWindow(typeof(DumpRenderTexture), false, "DumpRenderTexture");
            window.position = new Rect(window.position.xMin + 100f, window.position.yMin + 100f, 300f, 50f);
        }

        void OnGUI()
        {
            EditorGUILayout.BeginVertical();

            // Draw "Render Texture" field
            GUILayout.BeginHorizontal();
            GUILayout.Label("Render Texture: ", EditorStyles.boldLabel);
            cameraWithRenderTexture =
                EditorGUILayout.ObjectField(cameraWithRenderTexture, typeof(Camera), true) as Camera;
            GUILayout.EndHorizontal();

            // Draw "Texture Height" field
            GUILayout.BeginHorizontal();
            GUILayout.Label("Texture Height: ");
            textureHeight = EditorGUILayout.IntField(textureHeight);
            GUILayout.EndHorizontal();

            // Draw "File Name" field
            GUILayout.BeginHorizontal();
            GUILayout.Label("File Name: ");
            fileName = EditorGUILayout.TextField(fileName);
            GUILayout.EndHorizontal();

            // Draw "Dump to file" button
            if (GUILayout.Button("Dump to file..."))
            {
                SaveCertificate(string.Format("C:\\{0}.png", fileName));
            }

            EditorGUILayout.EndVertical();
        }

        void SaveCertificate(string path)
        {
            var targetTexture = cameraWithRenderTexture.targetTexture;

            RenderTexture.active = targetTexture;

            var newTexture = CreateNewTexture(targetTexture);
            var croppedTex = CroppedTexture(newTexture);
            var bytes = croppedTex.EncodeToPNG();

            File.WriteAllBytes(path, bytes);
        }

        static Texture2D CreateNewTexture(Texture targetTexture)
        {
            var newTexture = new Texture2D(targetTexture.width,
                targetTexture.height,
                TextureFormat.ARGB32, false);

            newTexture
                .ReadPixels(
                    new Rect(0, 0, targetTexture.width,
                        targetTexture.height),
                    0, 0, false);

            newTexture.Apply();
            return newTexture;
        }

        Texture2D CroppedTexture(Texture2D newTexture)
        {
            var colorArrayCropped =
                newTexture.GetPixels(0, newTexture.height - textureHeight, newTexture.width,
                    textureHeight);

            var croppedTex = new Texture2D(newTexture.width, textureHeight,
                TextureFormat.ARGB32, false);

            croppedTex.SetPixels(colorArrayCropped);
            croppedTex = FillInClear(croppedTex, cameraWithRenderTexture.backgroundColor);
            croppedTex.Apply();
            return croppedTex;
        }

        Texture2D FillInClear(Texture2D tex2D, Color whatToFillWith)
        {
            for (var i = 0; i < tex2D.width; i++)
            {
                for (var j = 0; j < tex2D.height; j++)
                {
                    if (tex2D.GetPixel(i, j) == Color.clear)
                    {
                        tex2D.SetPixel(i, j, whatToFillWith);
                    }
                }
            }

            return tex2D;
        }
    }
}