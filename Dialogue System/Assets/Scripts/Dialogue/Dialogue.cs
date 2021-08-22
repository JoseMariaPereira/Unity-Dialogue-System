using UnityEngine;
using System.Collections.Generic;
using System;

namespace FlyingCrow.Dialogue
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/New Dialogue", order = 0)]
    public class Dialogue : ScriptableObject
    {
        [SerializeField] private List<DialogueNode> nodes = new List<DialogueNode>();
        private Dictionary<string, DialogueNode> nodeDictionary = new Dictionary<string, DialogueNode>();

#if UNITY_EDITOR
        private void Awake()
        {
            if (nodes.Count == 0)
            {
                nodes.Add(new DialogueNode());
            }
            OnValidate();
        }
#endif

        private void OnValidate()
        {
            foreach(DialogueNode node in GetAllNodes())
            {
                if (nodeDictionary.ContainsValue(node))
                {
                    if (!nodeDictionary.ContainsKey(node.GetUniqueID()))
                    {
                        string keyToRemove = null;
                        foreach (string key in nodeDictionary.Keys)
                        {
                            if (nodeDictionary[key].Equals(node))
                            {
                                keyToRemove = key;
                            }
                        }
                        if (keyToRemove != null)
                        {
                            nodeDictionary.Remove(keyToRemove);
                            nodeDictionary.Add(node.GetUniqueID(), node);
                        }
                    }
                }
                else
                {
                    if (!nodeDictionary.ContainsKey(node.GetUniqueID()))
                    {
                        nodeDictionary.Add(node.GetUniqueID(), node);
                    }
                }
            }
            foreach(KeyValuePair<String, DialogueNode> pair in nodeDictionary)
            {
                Debug.Log(pair.Value.GetText() + "//" + pair.Key);
            }
        }

        public IEnumerable<DialogueNode> GetAllNodes()
        {
            return nodes;
        }

        public DialogueNode GetRootNode()
        {
            return nodes[0];
        }

        public IEnumerable<DialogueNode> GetAllChildren(DialogueNode parentNode)
        {
            foreach (string child in parentNode.GetChildren())
            {
                if (nodeDictionary.ContainsKey(child))
                {
                    yield return nodeDictionary[child];
                }
            }
        }
    }
}
