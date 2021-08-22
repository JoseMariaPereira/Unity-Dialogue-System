using System;
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
        private DialogueNode draggingNode = null;
        private Vector2 dragOffSet = Vector2.zero;

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
                CatchEvents();
                EditorGUILayout.LabelField(selectedDialogue.name);
                foreach (DialogueNode node in selectedDialogue.GetAllNodes())
                {
                    CreateNodeLayout(node);
                }
            }
        }

        private void CatchEvents()
        {
            if (Event.current.type.Equals(EventType.MouseDown) && draggingNode == null)
            {
                draggingNode = GetNodeAtPoint(Event.current.mousePosition);
                if (draggingNode != null)
                {
                    dragOffSet = draggingNode.GetRect().position - Event.current.mousePosition;
                }
            }
            else if (Event.current.type.Equals(EventType.MouseDrag) && draggingNode != null)
            {
                Undo.RecordObject(selectedDialogue, "Move Dialogue Node");
                draggingNode.SetRectPosition(Event.current.mousePosition + dragOffSet); 
                GUI.changed = true;
            }
            else if (Event.current.type.Equals(EventType.MouseUp) && draggingNode != null)
            {
                draggingNode = null;
            }
        }

        private void CreateNodeLayout(DialogueNode node)
        {
            GUILayout.BeginArea(node.GetRect(), nodeStyle);
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

        private DialogueNode GetNodeAtPoint(Vector2 point)
        {
            DialogueNode lastNode = null;
            foreach(DialogueNode node in selectedDialogue.GetAllNodes())
            {
                if (node.GetRect().Contains(point))
                {
                    lastNode = node;
                }
            }
            return lastNode;
        }
    }
}
