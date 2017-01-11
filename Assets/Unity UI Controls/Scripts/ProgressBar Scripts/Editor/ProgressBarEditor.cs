// ===========================================================================================================
//
// Class/Library: ProgressBar Control
//        Author: Michael Marzilli   ( http://www.linkedin.com/in/michaelmarzilli , http://www.develteam.com/Developer/Rowell/Portfolio )
//       Created: Jul 12, 2016
//	
// VERS 1.0.000 : Jul 12, 2016 : Original File Created. Released for Unity 3D.
//
// ===========================================================================================================

#if UNITY_EDITOR
	#define		IS_DEBUGGING
#else
	#undef		IS_DEBUGGING
#endif

using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(ProgressBar))]
public class ProgressBarEditor : Editor 
{

	public override void OnInspectorGUI()
	{
		ProgressBar myTarget = null;
		try { myTarget = (ProgressBar)target; } catch { }

		if (myTarget != null)
		{
			GUI.changed = false;

			EditorStyles.label.fontStyle = FontStyle.Bold;
			EditorGUILayout.LabelField("COLOR SETTINGS");
			EditorStyles.label.fontStyle = FontStyle.Normal;
			myTarget.TextColor				= EditorGUILayout.ColorField("Text Color",					myTarget.TextColor);
			myTarget.TextShadow				= EditorGUILayout.ColorField("Text Shadow Color",		myTarget.TextShadow);
			myTarget.ProgressBarColor	= EditorGUILayout.ColorField("Progress Bar Color",	myTarget.ProgressBarColor);

			if (GUI.changed)
			{
				EditorUtility.SetDirty(myTarget);
				EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
			}
		}
	}
}
