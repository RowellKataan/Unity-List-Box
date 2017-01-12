// ===========================================================================================================
//
// Class/Library: ListBox Control - Line Item Editor/Inspector
//        Author: Michael Marzilli   ( http://www.linkedin.com/in/michaelmarzilli , http://www.develteam.com/Developer/Rowell/Portfolio )
//       Created: Jun 10, 2016
//	
// VERS 1.0.000 : Jun 10, 2016 : Original File Created. Released for Unity 3D.
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

[CustomEditor(typeof(ListBoxLineItem))]
public class ListBoxLineItemEditor : Editor 
{

	public override void OnInspectorGUI()
	{
		ListBoxLineItem myTarget = null;
		try { myTarget = (ListBoxLineItem)target; } catch { }

		if (myTarget != null)
		{
			GUI.changed = false;

			EditorGUILayout.Space();
			EditorStyles.label.fontStyle = FontStyle.Bold;
			EditorGUILayout.LabelField("GAME TURN DISPLAY");
			EditorStyles.label.fontStyle = FontStyle.Normal;
			EditorGUILayout.ObjectField("Owning ListBox", myTarget.ListBoxControlObject, typeof(GameObject), true);
			EditorGUILayout.Separator();
			EditorGUILayout.Space();


			EditorStyles.label.fontStyle = FontStyle.Bold;
			EditorGUILayout.LabelField("LINE ITEM VALUES");
			EditorStyles.label.fontStyle = FontStyle.Normal;
			EditorGUILayout.LabelField("Index",			myTarget.Index.ToString());
			EditorGUILayout.LabelField("Value",			myTarget.Value);
			EditorGUILayout.LabelField("Text",			myTarget.Text);
			EditorGUILayout.LabelField("Sub Text",	myTarget.SubText);
			if (Application.isPlaying && myTarget.ListBoxControlObject != null)
				EditorGUILayout.LabelField("Selected", (myTarget.ListBoxControlObject.GetComponent<ListBoxControl>().IsSelectedByIndex(myTarget.Index)) ? "YES" : "no");
			EditorGUILayout.Separator();
			EditorGUILayout.Space();


			EditorStyles.label.fontStyle = FontStyle.Bold;
			EditorGUILayout.LabelField("LINE ITEM PROPERTIES");
			EditorStyles.label.fontStyle = FontStyle.Normal;
			EditorGUILayout.LabelField("Width",		myTarget.Width.ToString());
			EditorGUILayout.LabelField("Height",	myTarget.Height.ToString());
			EditorGUILayout.LabelField("X-Pos",		myTarget.X.ToString());
			EditorGUILayout.LabelField("Y-Pos",		myTarget.Y.ToString());
			EditorGUILayout.LabelField("Spacing", myTarget.Spacing.ToString());
			EditorGUILayout.ColorField("Normal Color",		myTarget.ItemNormalColor);
			EditorGUILayout.ColorField("Highlight Color", myTarget.ItemHighlightColor);
			EditorGUILayout.ColorField("Selected Color",	myTarget.ItemSelectedColor);
			EditorGUILayout.ColorField("Disabled Color",	myTarget.ItemDisabledColor);
			EditorGUILayout.Separator();
			EditorGUILayout.Space();

			if (GUI.changed)
			{
				EditorUtility.SetDirty(myTarget);
				if (!Application.isPlaying)
					EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
			}
		}
	}

}
