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

[CustomEditor(typeof(ListBoxControl))]
public class ListBoxControlEditor : Editor 
{
	private string	strValue		= "";
	private string	strText			= "";
	private Sprite	sprIcon			= null;
	private bool		blnObjects	= false;

	public override void OnInspectorGUI()
	{
		ListBoxControl myTarget = null;
		try { myTarget = (ListBoxControl)target; } catch { }

		if (myTarget != null)
		{
			GUI.changed = false;
			EditorGUILayout.Space();

			if (myTarget.ListBoxMode == ListBoxControl.ListBoxModes.ListBox && !myTarget.PartOfDDL)
			{ 
				EditorStyles.foldout.fontStyle = FontStyle.Bold;
				blnObjects = EditorGUILayout.Foldout(blnObjects, "LISTBOX GAMEOBJECT COMPONENTS");
				if (blnObjects)
				{
					myTarget.ScrollBarObject							= (GameObject)EditorGUILayout.ObjectField("ScrollBar",					myTarget.ScrollBarObject,							typeof(GameObject), true);
					myTarget.ScrollRectObject							= (GameObject)EditorGUILayout.ObjectField("ScrollRect",					myTarget.ScrollRectObject,						typeof(GameObject), true);
					myTarget.ScrollContainerObject				= (GameObject)EditorGUILayout.ObjectField("Item Container",			myTarget.ScrollContainerObject,				typeof(GameObject), true);
					myTarget.ListBoxTitle									= (Text)			EditorGUILayout.ObjectField("Title Text Object",	myTarget.ListBoxTitle,								typeof(Text),				true);
					myTarget.ListBoxLineItemPrefabObject	= (GameObject)EditorGUILayout.ObjectField("Line Item Prefab",		myTarget.ListBoxLineItemPrefabObject,	typeof(GameObject), true);
					EditorGUILayout.Separator();
					EditorGUILayout.Space();
				}

				EditorStyles.label.fontStyle	= FontStyle.Bold;
				EditorGUILayout.LabelField("LISTBOX SETTINGS");
				EditorStyles.label.fontStyle	= FontStyle.Normal;
				myTarget.Title								= EditorGUILayout.TextField(	"ListBox Title",		myTarget.Title);
				myTarget.TitleBestFit					=	EditorGUILayout.Toggle(			"Best Fit Title",		myTarget.TitleBestFit);
				myTarget.CanMultiSelect				=	EditorGUILayout.Toggle(			"Can Multi-Select", myTarget.CanMultiSelect);
				myTarget.AllowDoubleClick			=	EditorGUILayout.Toggle(			"Can Double-Click", myTarget.AllowDoubleClick);
				string	strChar								= EditorGUILayout.TextField(	"Separator Char",		myTarget.SeparatorChar.ToString()).Trim();
				if (strChar.Length > 0 && strChar != myTarget.SeparatorChar.ToString())
					myTarget.SeparatorChar			= char.Parse(strChar.Substring(0, 1));
				EditorGUILayout.Separator();
				EditorGUILayout.Space();


				EditorStyles.label.fontStyle	= FontStyle.Bold;
				EditorGUILayout.LabelField("LINE ITEM SETTINGS");
				EditorStyles.label.fontStyle	= FontStyle.Normal;
				myTarget.Height								=	EditorGUILayout.FloatField("Line Item Height",	myTarget.Height);
				myTarget.Spacing							=	EditorGUILayout.FloatField("Line Item Spacing",	myTarget.Spacing);
				myTarget.ItemNormalColor			= EditorGUILayout.ColorField("Normal Color",			myTarget.ItemNormalColor);
				myTarget.ItemHighlightColor		= EditorGUILayout.ColorField("Highlight Color",		myTarget.ItemHighlightColor);
				myTarget.ItemSelectedColor		= EditorGUILayout.ColorField("Selected Color",		myTarget.ItemSelectedColor);
				myTarget.ItemDisabledColor		=	EditorGUILayout.ColorField("Disabled Color",		myTarget.ItemDisabledColor);
				EditorGUILayout.Separator();
				EditorGUILayout.Space();


				if (Application.isPlaying)
				{
					// DISPLAY THE CURRENTLY SELECTED ITEM(S) IN THE LISTBOX
					EditorStyles.label.fontStyle = FontStyle.Bold;
					EditorGUILayout.LabelField("SELECTED INDEXES & VALUES");
					EditorStyles.label.fontStyle = FontStyle.Normal;
					string st = "";
					List<string> sl = myTarget.SelectedValues;
					if (sl != null)
					{ 
						for (int i = 0; i < sl.Count; i++)
							st += myTarget.SelectedIndexes[i].ToString() + ": \"" + sl[i].ToString() + "\"\n";
						EditorStyles.label.fixedHeight = sl.Count * 14;
						EditorGUILayout.LabelField("Selected", st);
						EditorStyles.label.fixedHeight = 0;
						GUILayout.Space(sl.Count * 14);
					} else
						EditorGUILayout.LabelField("Selected", "None");
					EditorGUILayout.Separator();
					EditorGUILayout.Space();
				} else {
					EditorStyles.label.fontStyle = FontStyle.Bold;
					EditorGUILayout.LabelField("INITIAL LINE ITEMS");
					EditorStyles.label.fontStyle = FontStyle.Normal;

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

			} else {
				// REFER USER TO THE DROPDOWN LIST PROPERTIES
				EditorStyles.label.fontStyle = FontStyle.Bold;
				EditorGUILayout.LabelField("UNAVAILABLE -- (Modify the DropDown List Properties)");
				EditorStyles.label.fontStyle = FontStyle.Normal;
			}
		}
	}
}
