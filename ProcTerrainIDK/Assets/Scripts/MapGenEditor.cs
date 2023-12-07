using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NoiseFinalizer))]
public class NewBehaviourScript : Editor
{
    public override void OnInspectorGUI()
    {
        NoiseFinalizer finalizer = (NoiseFinalizer)target;

        if (DrawDefaultInspector())
        { 
            if(finalizer.autoUpdate)
            {
                finalizer.GenerateMap();
            }
        }

        if (GUILayout.Button("Generate"))
        { 
            finalizer.GenerateMap();
        }
    }
}
