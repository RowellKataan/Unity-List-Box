// ===========================================================================================================
//
// Class/Library: ListBox Control - Demo Application
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
using System.Collections;

public class DemoListBox : MonoBehaviour 
{

	#region "PUBLIC EDITOR PROPERTIES"

		public Text ResultTextObject;

	#endregion

	#region "PRIVATE PROPERTIES"

		private ListBoxControl			_listBox				= null;
		private ListBoxControl			MyListBox
		{
			get
			{
				if (_listBox == null)
						_listBox = (ListBoxControl)GameObject.FindObjectOfType(typeof(ListBoxControl));
				return _listBox;
			}
		}

		private string							ResultText
		{
			set
			{
				ResultTextObject.text = "<b>SELECTED ITEM(S):</b>\n" + value.Trim();
			}
		}

	#endregion

	#region "PRIVATE FUNCTIONS"

		private void								Start () 
		{
			StartCoroutine(StartEnum());
		}
		/// <summary>
		/// Wait for the ListBoxControl object to finish Initializing before performing actions on it.
		/// </summary>
		/// <returns></returns>
		private IEnumerator					StartEnum()
		{
			bool blnDone = false;
			while (!blnDone)
			{
				yield return null;
				if (MyListBox.IsInitialized)
				{
					MyListBox.OnChange += OnChange;
					for (int i = 4; i < 11; i++)
					{ 
						MyListBox.AddItem(i, "Item #" + i.ToString());
					}
					blnDone = true;
				}
			}
		}

		/// <summary>
		/// Display the Selected Items (Index, Value and Text) in a Text UI field.
		/// </summary>
		private void								DisplaySelection()
		{
			string st = "";
			for (int i = 0; i < MyListBox.SelectedIndexes.Count; i++)
				st += MyListBox.SelectedIndexes[i].ToString() + ": \"" + MyListBox.SelectedValues[i].ToString() + "\" - \"" + MyListBox.GetTextByIndex(MyListBox.SelectedIndexes[i]) + "\"\n";
			ResultText = st;
		}

	#endregion

	#region "EVENT FUNCTIONS"

		/// <summary>
		/// When a ListBoxItem is clicked, we want to update the display of selected items to our Results Text box.
		/// </summary>
		/// <param name="go"></param>
		/// <param name="intSelected"></param>
		public	void	OnChange(GameObject go, int intSelected)
		{
			if (go != MyListBox.gameObject)
				return;
			DisplaySelection();
		}

	#endregion

}
