using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace FlyingCrow.Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        private Dialogue selectedDialogue = null;
        private GUIStyle nodeStyle;

        [MenuItem("Window/Dialogue Editor")]
        public static void ShowWindow() 
        {
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }

        [OnOpenAssetAttribute(1)]
        public static bool OnOpenAssets(int instanceID, int line)
        {
            bool isDialogue = false;
            Dialogue dialogue = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;
            if (dialogue)
            {
                DialogueEditor.ShowWindow();
                isDialogue = true;
            }
            return isDialogue;
        }

        private void OnEnable()
        {
            Selection.selectionChanged += ChangeSelection;

            nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;
            nodeStyle.padding = new RectOffset(20, 20, 20, 20);
            nodeStyle.border = new RectOffset(12, 12, 12, 12);

        }

        private void ChangeSelection()
        {
            Dialogue dialogue = Selection.activeObject as Dialogue;
            if (dialogue)
            {
                selectedDialogue = dialogue;
                Repaint();
            }
        }

        private void OnGUI()
        {
            if (!selectedDialogue)
            {
                EditorGUILayout.LabelField("No Dialogue selected");
            }
            else
            {
                EditorGUILayout.LabelField(selectedDialogue.name);
                foreach (DialogueNode node in selectedDialogue.GetAllNodes())
                {
                    CreateNodeLayout(node);
                }
            }
            
        }

        private void CreateNodeLayout(DialogueNode node)
        {
            GUILayout.BeginArea(node.GetPosition(), nodeStyle);
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField(node.GetUniqueID(), EditorStyles.boldLabel);

            string id = EditorGUILayout.TextField("Unique ID", node.GetUniqueID());
            string text = EditorGUILayout.TextField("Text", node.GetText());

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(selectedDialogue, "Dialogue Content");
                node.SetUniqueID(id);
                node.SetText(text);
            }

            GUILayout.EndArea();
        }
    }
}
