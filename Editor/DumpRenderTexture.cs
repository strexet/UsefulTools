using System.IO;
using UnityEditor;
using UnityEngine;

namespace UsefulTools.Editor {
	public class DumpRenderTexture : EditorWindow {
		Camera _cameraWithRenderTexture;
		string _fileName      = "DumpedRenderTexture";
		int    _textureHeight = 800;

		[MenuItem("Tools/UsefulTools/Dump Render Texture To File")]
		internal static void Init() {
			// EditorWindow.GetWindow() will return the open instance of the specified window or create a new
			// instance if it can't find one. The second parameter is a flag for creating the window as a
			// Utility window; Utility windows cannot be docked like the Scene and Game view windows.
			var window = (DumpRenderTexture)GetWindow(typeof(DumpRenderTexture), false, "DumpRenderTexture");
			window.position = new Rect(window.position.xMin + 100f, window.position.yMin + 100f, 300f, 50f);
		}

		/// <summary>
		///     Called on GUI events.
		/// </summary>
		internal void OnGUI() {
			EditorGUILayout.BeginVertical();

			GUILayout.BeginHorizontal();
			GUILayout.Label("Render Texture: ", EditorStyles.boldLabel);
			_cameraWithRenderTexture =
				EditorGUILayout.ObjectField(_cameraWithRenderTexture, typeof(Camera), true) as Camera;
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("Texture Height: ");
			_textureHeight = EditorGUILayout.IntField(_textureHeight);
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("File Name: ");
			_fileName = EditorGUILayout.TextField(_fileName);
			GUILayout.EndHorizontal();

			if ( GUILayout.Button("Dump to file...") ) {
				SaveCertificate(string.Format("C:\\{0}.png", _fileName));
			}


			EditorGUILayout.EndVertical();
		}

		public void SaveCertificate(string path) {
			RenderTexture.active = _cameraWithRenderTexture.targetTexture;
			var newTexture = new Texture2D(_cameraWithRenderTexture.targetTexture.width,
				_cameraWithRenderTexture.targetTexture.height,
				TextureFormat.ARGB32, false);
			newTexture
				.ReadPixels(
					new Rect(0, 0, _cameraWithRenderTexture.targetTexture.width,
						_cameraWithRenderTexture.targetTexture.height),
					0, 0, false);
			newTexture.Apply();
			var colorArrayCropped =
				newTexture.GetPixels(0, newTexture.height - _textureHeight, newTexture.width,
					_textureHeight);
			var croppedTex = new Texture2D(newTexture.width, _textureHeight,
				TextureFormat.ARGB32, false);
			croppedTex.SetPixels(colorArrayCropped);
			croppedTex = FillInClear(croppedTex, _cameraWithRenderTexture.backgroundColor);
			croppedTex.Apply();

			var bytes = croppedTex.EncodeToPNG();

			File.WriteAllBytes(path, bytes);
		}

		public Texture2D FillInClear(Texture2D tex2D, Color whatToFillWith) {
			for ( var i = 0; i < tex2D.width; i++ ) {
				for ( var j = 0; j < tex2D.height; j++ ) {
					if ( tex2D.GetPixel(i, j) == Color.clear ) {
						tex2D.SetPixel(i, j, whatToFillWith);
					}
				}
			}

			return tex2D;
		}
	}
}