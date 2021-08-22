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
                foreach(DialogueNode node in selectedDialogue.GetAllNodes())
                {
                    EditorGUILayout.LabelField(node.GetText());
                }
            }
            
        }
    }
}
