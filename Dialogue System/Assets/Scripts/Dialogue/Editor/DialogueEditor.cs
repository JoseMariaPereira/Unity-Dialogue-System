using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace FlyingCrow.Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        [MenuItem("Window/Dialogue Editor")]
        public static void ShowWindow() 
        {
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }
    }
}
