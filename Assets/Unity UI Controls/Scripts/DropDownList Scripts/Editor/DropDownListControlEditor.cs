// ===========================================================================================================
//
// Class/Library: ListBox Control Editor/Inspector
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
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;

[CustomEditor(typeof(DropDownListControl))]
public class DropDownListControlEditor : Editor 
{
	private string	strValue		= "";
	private string	strText			= "";
	private Sprite	sprIcon			= null;
	private bool		blnObjects	= false;

	public override void OnInspectorGUI()
	{
		DropDownListControl myTarget = null;
		try { myTarget = (DropDownListControl)target; } catch { }

		if (myTarget != null)
		{
			GUI.changed = false;
			EditorGUILayout.Space();

			EditorStyles.foldout.fontStyle = FontStyle.Bold;
			blnObjects = EditorGUILayout.Foldout(blnObjects, "DROPDOWN LIST GAMEOBJECT COMPONENTS");
			if (blnObjects)
			{
				myTarget.ParentContainer		= (GameObject)		EditorGUILayout.ObjectField("Parent Container",	myTarget.ParentContainer,			typeof(GameObject),			true);
				EditorGUILayout.Space();
				myTarget.SelectedTextObject	= (Text)					EditorGUILayout.ObjectField("Selected Text",		myTarget.SelectedTextObject,	typeof(Text),						true);
				myTarget.DdlListBox					= (ListBoxControl)EditorGUILayout.ObjectField("List Box Control",	myTarget.DdlListBox,					typeof(ListBoxControl), true);
				EditorGUILayout.Separator();
				EditorGUILayout.Space();
			}

			myTarget.DdlListBox.Title								= "";
			myTarget.DdlListBox.TitleBestFit				= false;
			myTarget.DdlListBox.CanMultiSelect			= false;
			myTarget.DdlListBox.PartOfDDL						= true;
			myTarget.DdlListBox.AllowDoubleClick		= false;

			EditorStyles.label.fontStyle = FontStyle.Bold;
			EditorGUILayout.LabelField("DROPDOWN LIST PLACEHOLDER TEXT");
			EditorStyles.label.fontStyle = FontStyle.Normal;
			myTarget.PlaceholderText								= EditorGUILayout.TextField("Placeholder Text",		myTarget.PlaceholderText);
			EditorGUILayout.Separator();
			EditorGUILayout.Space();

			EditorStyles.label.fontStyle	= FontStyle.Bold;
			EditorGUILayout.LabelField("LINE ITEM SETTINGS");
			EditorStyles.label.fontStyle	= FontStyle.Normal;
			myTarget.LineItemHeight									=	EditorGUILayout.FloatField("Line Item Height",	myTarget.LineItemHeight);
			myTarget.DdlListBox.Spacing							=	EditorGUILayout.FloatField("Line Item Spacing",	myTarget.DdlListBox.Spacing);
			myTarget.DdlListBox.ItemNormalColor			= EditorGUILayout.ColorField("Normal Color",			myTarget.DdlListBox.ItemNormalColor);
			myTarget.DdlListBox.ItemHighlightColor	= EditorGUILayout.ColorField("Highlight Color",		myTarget.DdlListBox.ItemHighlightColor);
			myTarget.DdlListBox.ItemSelectedColor		= EditorGUILayout.ColorField("Selected Color",		myTarget.DdlListBox.ItemSelectedColor);
			myTarget.DdlListBox.ItemDisabledColor		=	EditorGUILayout.ColorField("Disabled Color",		myTarget.DdlListBox.ItemDisabledColor);
			EditorGUILayout.Separator();
			EditorGUILayout.Space();


			if (Application.isPlaying)
			{
				// DISPLAY THE CURRENTLY SELECTED ITEM(S) IN THE LISTBOX
				EditorStyles.label.fontStyle = FontStyle.Bold;
				EditorGUILayout.LabelField("SELECTED INDEXES & VALUES");
				EditorStyles.label.fontStyle = FontStyle.Normal;
				string st = "";
				List<string> sl = myTarget.DdlListBox.SelectedValues;
				if (sl != null)
				{ 
					for (int i = 0; i < sl.Count; i++)
						st += myTarget.DdlListBox.SelectedIndexes[i].ToString() + ": \"" + sl[i].ToString() + "\"\n";
					EditorStyles.label.fixedHeight = sl.Count * 14;
					EditorGUILayout.LabelField("Selected", st);
					EditorStyles.label.fixedHeight = 0;
					GUILayout.Space(sl.Count * 14);
				} else
					EditorGUILayout.LabelField("Selected", "None");
				EditorGUILayout.Separator();
				EditorGUILayout.Space();
			} else {
				EditorStyles.label.fontStyle		= FontStyle.Bold;
				EditorGUILayout.LabelField("INITIAL LINE ITEMS");
				EditorStyles.label.fontStyle		= FontStyle.Normal;

				myTarget.StartingValue					= EditorGUILayout.TextField("Starting Value", myTarget.StartingValue);
				EditorGUILayout.Space();

				EditorGUILayout.BeginVertical();

				// DISPLAY LIST HEADER
				EditorGUILayout.BeginHorizontal();
				EditorStyles.label.richText			= true;
				EditorStyles.label.stretchWidth = false;
				GUILayout.Button(" ", GUILayout.Width(20));
				EditorGUILayout.LabelField("<color=yellow>Index</color>", GUILayout.Width(40));
				EditorGUILayout.LabelField("<color=yellow>Value</color>", GUILayout.Width(100));
				EditorGUILayout.LabelField("<color=yellow>Text</color>");
				EditorStyles.label.richText			= false;
				EditorStyles.label.stretchWidth = true;
				EditorGUILayout.EndHorizontal();

				// DISPLAY INITIAL LIST ITEMS
				for (int i = 0; i < myTarget.StartArray.Count; i++)
				{
					EditorGUILayout.BeginHorizontal();
					if (GUILayout.Button("x", GUILayout.Width(20)))
						myTarget.RemoveStartItemByIndex(i);
					else
					{
						EditorGUILayout.LabelField(myTarget.StartArray[i].Index.ToString(), GUILayout.Width(40));
						EditorGUILayout.LabelField(myTarget.StartArray[i].Value, GUILayout.Width(100));
						EditorGUILayout.LabelField(myTarget.StartArray[i].Text);
					}
					EditorGUILayout.EndHorizontal();
				}
				EditorGUILayout.EndVertical();
				EditorGUILayout.Space();

				// ADD NEW ITEM
				strValue	= EditorGUILayout.TextField("Value",	strValue);
				strText		= EditorGUILayout.TextField("Text",		strText);
				sprIcon		= (Sprite)EditorGUILayout.ObjectField("Icon", sprIcon, typeof(Sprite), true);
				if (GUILayout.Button("Add Item") && strValue.Trim() != "" && strText.Trim() != "")
				{
					myTarget.AddStartItem(strValue, strText, sprIcon);
					strValue	= "";
					strText		= "";
					sprIcon		= null;
				}
				EditorGUILayout.Space();

				// SORT LIST ITEMS
				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button("Sort By Text"))
					myTarget.SortStartByText();
				if (GUILayout.Button("Sort By Value"))
					myTarget.SortStartByValue();
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.Separator();
				EditorGUILayout.Space();
			}

			if (GUI.changed)
			{
				EditorUtility.SetDirty(myTarget);
				if (!Application.isPlaying)
					EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
			}
		}
	}

}
