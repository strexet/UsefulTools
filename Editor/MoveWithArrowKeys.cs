using UnityEditor;
using UnityEngine;

namespace UsefulTools.Editor
{
    [ExecuteInEditMode]
    public class MoveWithArrowKeys : EditorWindow
    {
        public enum Directions
        {
            NegativeZ_2D = 0,
            Z = 10,
            Y = 20,
            X = 30
        }

        private static float staticAmountToMove = 1;
        private static bool stepFrameOnMove;

        private static Vector3 rightDirection;
        private static Vector3 leftDirection;
        private static Vector3 upDirection;
        private static Vector3 downDirection;

        public Directions FacingFromDirection = Directions.NegativeZ_2D;

        [MenuItem("Tools/UsefulTools/Move With ArrowKeys/Move With ArrowKeys Window %#UP")]
        private static void Init()
        {
            var window = GetWindow<MoveWithArrowKeys>("Move With Arrow Keys");

            window.minSize = new Vector2(128, 192);

            staticAmountToMove = 1;
            stepFrameOnMove = false;

            window.Show();
        }

        private void OnGUI()
        {
//	        Debug.Log("Window width: " + position.width);
//	        Debug.Log("Window height: " + position.height);

            EditorGUI.BeginChangeCheck();

            GUILayout.Label("Facing From Direction");
            FacingFromDirection = (Directions)EditorGUILayout.EnumPopup(FacingFromDirection);
            stepFrameOnMove = EditorGUILayout.Toggle("Step frame on move: ", stepFrameOnMove);
            ChangeFacingFromDirection();

            GUILayout.Space(10);

            GUILayout.Label("Amount To Move");
            staticAmountToMove = EditorGUILayout.FloatField(staticAmountToMove);

            if (GUILayout.Button("Set 1"))
            {
                staticAmountToMove = 1f;
            }

            if (GUILayout.Button("Set 0.1"))
            {
                staticAmountToMove = 0.1f;
            }

            if (GUILayout.Button("Set 0.01"))
            {
                staticAmountToMove = 0.01f;
            }

            if (GUILayout.Button("Set 0.001"))
            {
                staticAmountToMove = 0.001f;
            }

            if (GUILayout.Button("Set 0.0001"))
            {
                staticAmountToMove = 0.0001f;
            }
        }

        private void OnDestroy() => staticAmountToMove = 0;

        private void ChangeFacingFromDirection()
        {
            switch (FacingFromDirection)
            {
                case Directions.NegativeZ_2D:
                    rightDirection = new Vector3(1, 0, 0);
                    leftDirection = new Vector3(-1, 0, 0);
                    upDirection = new Vector3(0, 1, 0);
                    downDirection = new Vector3(0, -1, 0);
                    break;

                case Directions.Z:
                    rightDirection = new Vector3(-1, 0, 0);
                    leftDirection = new Vector3(1, 0, 0);
                    upDirection = new Vector3(0, 1, 0);
                    downDirection = new Vector3(0, -1, 0);
                    break;

                case Directions.Y:
                    rightDirection = new Vector3(1, 0, 0);
                    leftDirection = new Vector3(-1, 0, 0);
                    upDirection = new Vector3(0, 0, 1);
                    downDirection = new Vector3(0, 0, -1);
                    break;

                case Directions.X:
                    rightDirection = new Vector3(0, 0, 1);
                    leftDirection = new Vector3(0, 0, -1);
                    upDirection = new Vector3(0, 1, 0);
                    downDirection = new Vector3(0, -1, 0);
                    break;
            }
        }

        private static void MoveSelected(Vector3 direction)
        {
            var selectedTransforms = Selection.GetTransforms(SelectionMode.Unfiltered);

            if (selectedTransforms != null && selectedTransforms.Length > 0 && staticAmountToMove > 0)
            {
                Undo.RegisterCompleteObjectUndo(selectedTransforms, "Move with arrow keys");

                foreach (var movedTransform in selectedTransforms)
                {
                    var currentRotationQuaternion = movedTransform.rotation;

                    movedTransform.rotation = Quaternion.identity;
                    movedTransform.Translate(direction * staticAmountToMove);
                    movedTransform.rotation = currentRotationQuaternion;

                    EditorUtility.SetDirty(movedTransform);
                }

                if (stepFrameOnMove && EditorApplication.isPaused)
                {
                    EditorApplication.Step();
                }
            }
        }

        [MenuItem("Tools/UsefulTools/Move With ArrowKeys/Move/Right %RIGHT")]
        private static void MoveRight() => MoveSelected(rightDirection);

        [MenuItem("Tools/UsefulTools/Move With ArrowKeys/Move/Left %LEFT")]
        private static void MoveLeft() => MoveSelected(leftDirection);

        [MenuItem("Tools/UsefulTools/Move With ArrowKeys/Move/Up %UP")]
        private static void MoveUp() => MoveSelected(upDirection);

        [MenuItem("Tools/UsefulTools/Move With ArrowKeys/Move/Down %DOWN")]
        private static void MoveDown() => MoveSelected(downDirection);
    }
}