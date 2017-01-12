// ===========================================================================================================
//
// Class/Library: ListBox Control - Demo Application
//        Author: Michael Marzilli   ( http://www.linkedin.com/in/michaelmarzilli , http://www.develteam.com/Developer/Rowell/Portfolio )
//       Created: Jun 10, 2016
//	
// VERS 1.0.000 : Jun 10, 2016 : Original File Created. Released for Unity 3D.
//
// NOTE:	YOU MAY EXPERIENCE A SLIGHT DELAY IN THE CONTROL OPENING/CLOSING 
//				IF MORE THAN 500 (OR SO) ITEMS ARE ADDED TO ONE DROPDOWN LISTBOX.
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

	#region "PRIVATE CONSTANTS"

		private const int						MAX_DDL_ITEMS = 200;

	#endregion

	#region "PUBLIC EDITOR PROPERTIES"

		public	Text									ResultTextObject;
		public	ListBoxControl				MyListBox;
		public	DropDownListControl		MyDDL;

	#endregion

	#region "PRIVATE PROPERTIES"

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
		/// Once Initialized, fill the control with some sample data.
		/// </summary>
		/// <returns></returns>
		private IEnumerator					StartEnum()
		{
			bool blnDone = false;
			while (!blnDone)
			{
				yield return null;
				if (MyListBox != null && MyListBox.IsInitialized)
				{
					MyListBox.OnChange			+= OnDemoChange;
					MyListBox.OnDoubleClick	+= OnDemoDoubleClick;
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
					MyListBox.SetToTop();		// SET THE SCROLLBAR TO THE TOP OF THE LIST
				}
				if (MyDDL != null && MyDDL.IsInitialized) {
					MyDDL.DdlListBox.OnChange += OnDemoChange;
					for (int i = 4; i < 11; i++)
					{ 
						if (i == 4)
							MyDDL.AddItem(i, "Item #" + i.ToString(), "", (i * 3 * 100));																				// SET THE SUBTEXT FIELD TO AN INTEGER
						else if (i == 5)
							MyDDL.AddItem(i, "Item #" + i.ToString(), "", "$200.00");																						// SET THE SUBTEXT FIELD TO A STRING
						else if (i == 6)
							MyDDL.AddItem("Bob", "Item #" + i.ToString());																											// USE A STRING AS THE VALUE
						else if (i == 7)
							MyDDL.AddItem(new string[] { "This", "is", "an", "Array" }, "Item #" + i.ToString());								// USE A STRING ARRAY AS THE VALUE
						else if (i == 8)
						{
							MyDDL.AddItem(i, "Item #" + i.ToString() + " (disabled)");
							MyDDL.DisableByIndex(i - 1);																																				// DISABLE THIS LIST ITEM
						} 
						else if (i == 9)
							MyDDL.AddItem(i, "Item #" + i.ToString(), Resources.Load<Sprite>("Images/Status-Green-DOT-UI"));		// ADD ITEM WITH ICON SET BY SPRITE OBJECT
						else if (i == 10)
							MyDDL.AddItem(i, "Item #" + i.ToString(), "Images/Status-Green-DOT-UI");														// ADD ITEM WITH ICON SET BY PATH TO SPRITE OBJECT
						else
							MyDDL.AddItem(i, "Item #" + i.ToString());
					}

					yield return null;
					yield return new WaitForSeconds(0.01f);

					for (int i = 11; i <= MAX_DDL_ITEMS; i++)
					{
						MyDDL.AddItem(i, "Item #" + i.ToString());
					}

					blnDone = true;
					MyDDL.SetToTop();		// SET THE SCROLLBAR TO THE TOP OF THE LIST
				}
			}
			DisplaySelection();
		}

		/// <summary>
		/// Display the Selected Items (Index, Value and Text) in a Text UI field.
		/// </summary>
		private void								DisplaySelection()
		{
			string	st	= "";
			ResultText	= "";

			if (MyListBox != null)
				for (int i = 0; i < MyListBox.SelectedIndexes.Count; i++)
					st += MyListBox.SelectedIndexes[i].ToString() + ": \"" + MyListBox.SelectedValues[i].ToString() + "\" - \"" + MyListBox.GetTextByIndex(MyListBox.SelectedIndexes[i]) + "\"\n";
			else if (MyDDL != null)
				for (int i = 0; i < MyDDL.SelectedIndexes.Count; i++)
					st += MyDDL.SelectedIndexes[i].ToString() + ": \"" + MyDDL.SelectedValues[i].ToString() + "\" - \"" + MyDDL.GetTextByIndex(MyDDL.SelectedIndexes[i]) + "\"\n";

			ResultText = st;
		}

	#endregion

	#region "EVENT FUNCTIONS"

		/// <summary>
		/// When a ListBoxItem is clicked, we want to update the display of selected items to our Results Text box.
		/// </summary>
		/// <param name="go"></param>
		/// <param name="intSelected"></param>
		public	void	OnDemoChange(GameObject go, int intSelected)
		{
			if ((MyListBox	!= null && go == MyListBox.gameObject) || 
					(MyDDL			!= null && go == MyDDL.DdlListBox.gameObject))
				DisplaySelection();
		}
		/// <summary>
		/// When a ListBoxItem is double-clicked, we want to update the display of selected items to our Results Text box.
		/// </summary>
		/// <param name="go"></param>
		/// <param name="intSelected"></param>
		public	void	OnDemoDoubleClick(GameObject go, int intSelected)
		{
			if ((MyListBox	!= null && go == MyListBox.gameObject) || 
					(MyDDL			!= null && go == MyDDL.DdlListBox.gameObject))
			ResultText = "Double-Clicked Index " + intSelected.ToString();
		}

	#endregion

	#region "BUTTON FUNCTIONS"

		/// <summary>
		/// The Clear Selected Item from DropDown List button has been clicked.
		/// </summary>
		public	void	OnClearDDLClick()
		{
			if (MyDDL != null)
				MyDDL.SelectByIndex(-1);
			DisplaySelection();
		}

	#endregion

}
