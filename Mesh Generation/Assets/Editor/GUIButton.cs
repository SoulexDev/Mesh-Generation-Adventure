using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof(World))]
public class GUIButton : Editor
{
	public override void OnInspectorGUI()
	{
		World mapGen = (World)target;

		if (DrawDefaultInspector())
		{
			if (mapGen.autoUpdate)
			{
				mapGen.StartGenerateWorld();
			}
		}

		if (GUILayout.Button("Generate"))
		{
			mapGen.StartGenerateWorld();
		}
	}
}
