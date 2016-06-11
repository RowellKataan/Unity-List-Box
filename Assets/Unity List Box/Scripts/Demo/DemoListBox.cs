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
				ResultTextObject.text = "<b><size=16>SELECTED ITEM(S):</size></b>\n<b>Index : Value - Text</b>\n" + value.Trim();
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
						if (i == 4)
							MyListBox.AddItem(i, "Item #" + i.ToString(), "", (i * 3 * 100));																				// SET THE SUBTEXT FIELD TO AN INTEGER
						else if (i == 5)
							MyListBox.AddItem(i, "Item #" + i.ToString(), "", "$200.00");																						// SET THE SUBTEXT FIELD TO A STRING
						else if (i == 6)
							MyListBox.AddItem("Bob", "Item #" + i.ToString());																											// USE A STRING AS THE VALUE
						else if (i == 7)
							MyListBox.AddItem(new string[] { "This", "is", "an", "Array" }, "Item #" + i.ToString());								// USE A STRING ARRAY AS THE VALUE
						else if (i == 8)
						{
							MyListBox.AddItem(i, "Item #" + i.ToString() + " (disabled)");
							MyListBox.DisableByIndex(i - 1);																																				// DISABLE THIS LIST ITEM
						} 
						else if (i == 9)
							MyListBox.AddItem(i, "Item #" + i.ToString(), Resources.Load<Sprite>("Images/Status-Green-DOT-UI"));		// ADD ITEM WITH ICON SET BY SPRITE OBJECT
						else if (i == 10)
							MyListBox.AddItem(i, "Item #" + i.ToString(), "Images/Status-Green-DOT-UI");														// ADD ITEM WITH ICON SET BY PATH TO SPRITE OBJECT
						else
							MyListBox.AddItem(i, "Item #" + i.ToString());
					}
					blnDone = true;
				}
			}
			MyListBox.SetToTop();		// SET THE SCROLLBAR TO THE TOP OF THE LIST
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
