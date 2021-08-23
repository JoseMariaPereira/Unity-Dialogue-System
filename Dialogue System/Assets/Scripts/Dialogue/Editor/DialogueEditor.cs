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
        [NonSerialized] private GUIStyle nodeStyle;
        [NonSerialized] private DialogueNode draggingNode = null;
        [NonSerialized] private Vector2 dragNodeOffSet = Vector2.zero;
        [NonSerialized] private float lineSize = 3f;
        [NonSerialized] private DialogueNode creatingNode = null;
        [NonSerialized] private DialogueNode deletingNode = null;
        [NonSerialized] private DialogueNode linkingParentNode = null;
        private Vector2 scrollPosition;
        [NonSerialized] private float maxNodeSpaceHeight = 0;
        [NonSerialized] private float maxNodeSpaceWidth = 0;
        [NonSerialized] private bool draggingCanvas = false;
        [NonSerialized] private Vector2 draggingCanvasOffset;

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

                //ScrollView
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

                foreach (DialogueNode node in selectedDialogue.GetAllNodes())
                {
                    DrawConnections(node);
                }
                foreach (DialogueNode node in selectedDialogue.GetAllNodes())
                {
                    DrawNodeLayout(node);
                    SetNodeSpace(node);
                }

                //max height and width
                GUILayoutUtility.GetRect(maxNodeSpaceWidth, maxNodeSpaceHeight);

                EditorGUILayout.EndScrollView();

                if (creatingNode != null)
                {
                    Undo.RecordObject(selectedDialogue, "Add Dialogue Node");
                    selectedDialogue.CreateNode(creatingNode);
                    creatingNode = null;
                }
                if (deletingNode != null)
                {
                    Undo.RecordObject(selectedDialogue, "Remove Dialogue Node");
                    selectedDialogue.DeleteNode(deletingNode);
                    deletingNode = null;
                }
            }
        }

        private void CatchEvents()
        {
            if (Event.current.type.Equals(EventType.MouseDown) && draggingNode == null)
            {
                draggingNode = GetNodeAtPoint(Event.current.mousePosition + scrollPosition);
                if (draggingNode != null)
                {
                    dragNodeOffSet = draggingNode.GetRect().position - Event.current.mousePosition;
                }
                else
                {
                    draggingCanvas = true;
                    draggingCanvasOffset = Event.current.mousePosition + scrollPosition;
                }
            }
            else if (Event.current.type.Equals(EventType.MouseDrag) && draggingNode != null)
            {
                Undo.RecordObject(selectedDialogue, "Move Dialogue Node");
                draggingNode.SetRectPosition(Event.current.mousePosition + dragNodeOffSet); 
                GUI.changed = true;
            }
            else if (Event.current.type.Equals(EventType.MouseDrag) && draggingCanvas)
            {
                scrollPosition = draggingCanvasOffset - Event.current.mousePosition;
                GUI.changed = true;
            }
            else if (Event.current.type.Equals(EventType.MouseUp) && draggingNode != null)
            {
                draggingNode = null;
            }
            else if (Event.current.type.Equals(EventType.MouseUp) && draggingCanvas)
            {
                draggingCanvas = false;
            }
        }

        private void DrawNodeLayout(DialogueNode node)
        {
            GUILayout.BeginArea(node.GetRect(), nodeStyle);
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField(node.GetUniqueID(), EditorStyles.boldLabel);

            string text = EditorGUILayout.TextField("Text", node.GetText());

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(selectedDialogue, "Dialogue Content");
                node.SetText(text);
            }

            //Horizontal align buttons
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("x"))
            {
                deletingNode = node;
            }
            LinkButtons(node);
            if (GUILayout.Button("+"))
            {
                creatingNode = node;
            }
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        private void LinkButtons(DialogueNode node)
        {
            if (linkingParentNode == null)
            {
                if (GUILayout.Button("link"))
                {
                    linkingParentNode = node;
                }
            }
            else if (linkingParentNode == node)
            {
                if (GUILayout.Button("cancel"))
                {
                    linkingParentNode = null;
                }
            }
            else if (linkingParentNode.ContainsChild(node))
            {
                if (GUILayout.Button("unlink"))
                {
                    Undo.RecordObject(selectedDialogue, "Remove Dialogue Link");
                    linkingParentNode.RemoveChild(node);
                    linkingParentNode = null;
                }
            }
            else
            {
                if (GUILayout.Button("child"))
                {
                    Undo.RecordObject(selectedDialogue, "Add Dialogue Link");
                    linkingParentNode.AddChild(node);
                    linkingParentNode = null;
                }
            }
        }

        private void DrawConnections(DialogueNode node)
        {
            foreach (DialogueNode child in selectedDialogue.GetAllChildren(node))
            {
                Vector3 startPosition;
                Vector3 endPosition;
                Vector3 offSet;
                if (Mathf.Abs(node.GetRect().position.x - child.GetRect().position.x) * node.GetRect().height 
                    <= Mathf.Abs(node.GetRect().position.y - child.GetRect().position.y) * node.GetRect().width)
                {
                    startPosition = new Vector2(node.GetRect().center.x, node.GetRect().yMax);
                    endPosition = new Vector2(child.GetRect().center.x, child.GetRect().yMin);
                    offSet = endPosition - startPosition;
                    offSet.y = Mathf.Abs(offSet.y * 0.9f);
                    offSet.x = 0;
                }
                else
                {
                    startPosition = new Vector2(node.GetRect().xMax, node.GetRect().center.y);
                    endPosition = new Vector2(child.GetRect().xMin, child.GetRect().center.y);
                    offSet = endPosition - startPosition;
                    offSet.y = 0;
                    offSet.x = Mathf.Abs(offSet.x * 0.9f); 
                }
                Handles.DrawBezier(
                    startPosition, endPosition, 
                    startPosition + offSet, 
                    endPosition - offSet, 
                    Color.white, null, lineSize);
            }
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

        private void SetNodeSpace(DialogueNode node)
        {
            if (node.GetRect().xMax > maxNodeSpaceWidth)
            {
                maxNodeSpaceWidth = node.GetRect().xMax;
            }
            if (node.GetRect().yMax > maxNodeSpaceHeight)
            {
                maxNodeSpaceHeight = node.GetRect().yMax;
            }
        }
    }
}
