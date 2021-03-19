using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Line))]
public class LineInspector : Editor
{

    private void OnSceneGUI()
    {
        var line = target as Line;

        Transform handleTransform = line.transform;
        Quaternion rotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;
        var p0 = handleTransform.TransformPoint(line.p0);
        var p1 = handleTransform.TransformPoint(line.p1);

        Handles.color = Color.blue;
        Handles.DrawLine(p0, p1);
        Handles.DoPositionHandle(p0, rotation);
        Handles.DoPositionHandle(p1, rotation);
    }
}
