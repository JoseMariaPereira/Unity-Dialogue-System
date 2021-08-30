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
        [NonSerialized] private GUIStyle playerStyle;
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
        const float bgSize = 50;
        const float maxCanvasSize = 5000;

        [MenuItem("Window/Dialogue Editor")]
        public static void ShowWindow() 
        {
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }

        [OnOpenAsset(1)]
        public static bool OnOpenAssets(int instanceID, int line)
        {
            bool isDialogue = false;
            Dialogue dialogue = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;
            if (dialogue)
            {
                ShowWindow();
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

            playerStyle = new GUIStyle();
            playerStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
            playerStyle.padding = new RectOffset(20, 20, 20, 20);
            playerStyle.border = new RectOffset(12, 12, 12, 12);

        }

        private void ChangeSelection()
        {
            Dialogue dialogue = Selection.activeObject as Dialogue;
            if (dialogue != null)
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

                SetCanvasBG();

                foreach (DialogueNode node in selectedDialogue.GetAllNodes())
                {
                    DrawConnections(node);
                }
                foreach (DialogueNode node in selectedDialogue.GetAllNodes())
                {
                    DrawNodeLayout(node);
                    SetNodeSpace(node);
                }

                GUILayoutUtility.GetRect(maxNodeSpaceWidth, maxNodeSpaceHeight);

                EditorGUILayout.EndScrollView();

                if (creatingNode != null)
                {
                    selectedDialogue.CreateNode(creatingNode);
                    creatingNode = null;
                }
                if (deletingNode != null)
                {
                    selectedDialogue.DeleteNode(deletingNode);
                    deletingNode = null;
                }
            }
        }

        private static void SetCanvasBG()
        {
            Texture2D bgText = Resources.Load("background") as Texture2D;
            GUI.DrawTextureWithTexCoords(
                new Rect(0, 0, maxCanvasSize, maxCanvasSize),
                bgText,
                new Rect(0, 0, maxCanvasSize / bgSize, maxCanvasSize / bgSize)
                );
        }

        private void CatchEvents()
        {
            if (Event.current.type.Equals(EventType.MouseDown) && draggingNode == null)
            {
                draggingNode = GetNodeAtPoint(Event.current.mousePosition + scrollPosition);
                if (draggingNode != null)
                {
                    dragNodeOffSet = draggingNode.GetRect().position - Event.current.mousePosition;
                    Selection.activeObject = draggingNode;
                }
                else
                {
                    draggingCanvas = true;
                    draggingCanvasOffset = Event.current.mousePosition + scrollPosition;
                    Selection.activeObject = selectedDialogue;
                }
            }
            else if (Event.current.type.Equals(EventType.MouseDrag) && draggingNode != null)
            {
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

            GUILayout.BeginArea(node.GetRect(), node.IsPlayer() ? playerStyle : nodeStyle);

            //EditorGUILayout.LabelField(node.name, EditorStyles.boldLabel);
            //EditorGUILayout.LabelField("");

            //node.SetText(EditorGUILayout.TextField("Text", node.GetText()));
            node.SetText(EditorGUILayout.TextArea(node.GetText(), GUILayout.Height(100)));

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
                    linkingParentNode.RemoveChild(node);
                    linkingParentNode = null;
                }
            }
            else
            {
                if (GUILayout.Button("child"))
                {
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
                Texture2D aTexture;
                Vector2 texSize;
                if (VerticalLineCondition(node, child))
                {
                    startPosition = new Vector2(node.GetRect().center.x, node.GetRect().yMax);
                    endPosition = new Vector2(child.GetRect().center.x, child.GetRect().yMin);
                    offSet = endPosition - startPosition;
                    offSet.y = Mathf.Abs(offSet.y * 0.9f);
                    offSet.x = 0;
                    aTexture = Resources.Load("Down") as Texture2D;
                    texSize = new Vector2(10, 5);
                }
                else
                {
                    startPosition = new Vector2(node.GetRect().xMax, node.GetRect().center.y);
                    endPosition = new Vector2(child.GetRect().xMin, child.GetRect().center.y);
                    offSet = endPosition - startPosition;
                    offSet.y = 0;
                    offSet.x = Mathf.Abs(offSet.x * 0.9f);
                    aTexture = Resources.Load("Right") as Texture2D;
                    texSize = new Vector2(5, 10);
                }
                Handles.DrawBezier(
                    startPosition, endPosition, 
                    startPosition + offSet, 
                    endPosition - offSet, 
                    Color.white, null, lineSize);
                GUI.DrawTexture(new Rect(endPosition.x - texSize.x / 2, endPosition.y - texSize.y / 2, texSize.x, texSize.y), aTexture, ScaleMode.StretchToFill);
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
                maxNodeSpaceWidth = Mathf.Min(node.GetRect().xMax, maxCanvasSize);
            }
            if (node.GetRect().yMax > maxNodeSpaceHeight)
            {
                maxNodeSpaceHeight = Mathf.Min(node.GetRect().yMax, maxCanvasSize);
            }
        }

        private bool VerticalLineCondition(DialogueNode parent, DialogueNode child)
        {
            bool makeHorizontal;
            makeHorizontal = (parent.GetRect().position.x - child.GetRect().position.x) * parent.GetRect().height
                    > (parent.GetRect().position.y - child.GetRect().position.y) * parent.GetRect().width;
            return makeHorizontal;
        }
    }
}
