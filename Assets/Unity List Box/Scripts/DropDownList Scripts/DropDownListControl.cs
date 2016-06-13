using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DropDownListControl : ListBoxControl 
{

	#region "PRIVATE VARIABLES"

		[SerializeField]
		private	string						_strStartingValue		= "";

		private string						_strPlaceholder			= "Select Item...";
		private bool							_blnDroppedDown			= false;

	#endregion

	#region "PUBLIC PROPERTIES"

		public	string										StartingValue
		{
			get
			{
				return _strStartingValue;
			}
			set
			{
				_strStartingValue = value.Trim();
			}
		}

		public	override	List<ListBoxLineItem>		Items
		{
			get
			{
				if (DdlListBox != null)
					return DdlListBox.Items;
				else
					return new List<ListBoxLineItem>();
			}
		}
		public	override	List<int>				SelectedIndexes
		{
			get
			{
				if (DdlListBox != null)
					return	DdlListBox.SelectedIndexes;
				else
					return new List<int>();
			}
		}
		public	override	List<string>		SelectedValues
		{
			get
			{
				if (DdlListBox != null)
					return DdlListBox.SelectedValues;
				else
					return	new List<string>();
			}
		}
		public	override	string					SelectedValue
		{
			get
			{
				if (DdlListBox != null)
					return DdlListBox.SelectedValue;
				else
					return "";
			}
		}
		public	override	string					SelectedArrayValue(int intIndex)
		{
			if (DdlListBox != null)
				return DdlListBox.SelectedArrayValue(intIndex);
			else
				return "";
		}
		public	override	int							SelectedValueInt
		{
			get
			{
				if (DdlListBox != null)
					return DdlListBox.SelectedValueInt;
				else
					return -1;
			}
		}
		public	override	int							SelectedArrayValueInt(int intIndex)
		{
				if (DdlListBox != null)
					return DdlListBox.SelectedArrayValueInt(intIndex);
				else
					return -1;
		}
		public	override	int							SelectedIndex
		{
			get
			{
				if (DdlListBox != null)
					return DdlListBox.SelectedIndex;
				else
					return -1;
			}
			set
			{
				if (DdlListBox != null)
					DdlListBox.SelectedIndex = value;
			}
		}

	#endregion

	#region "PUBLIC EDITOR PROPERTIES"

		public	Text							SelectedTextObject;
		public	GameObject				DropDownButton;
		public	ListBoxControl		DdlListBox;

	#endregion

	#region "PRIVATE FUNCTIONS"

		private void							Awake()
		{
			this.ListBoxMode = ListBoxModes.DropDownList;

			if (DdlListBox != null)
			{
				DdlListBox.PartOfDDL = true;
				DdlListBox.InitStartItems(_startArray);
			}
		}
		private void							Start()
		{
			if (DdlListBox != null)
			{ 
				DdlListBox.gameObject.SetActive(true);
				DdlListBox.Show();
				DdlListBox.OnChange += this.OnChange;
			}
		}
		private void							Update()
		{
			if (!_blnInitialized && DdlListBox.IsInitialized)
			{
				_blnInitialized = true;
				_blnDroppedDown = false;

				// SET THE STARTING VALUE
				if (StartingValue == "")
					DdlListBox.SelectedIndex = -1;
				else
					DdlListBox.SelectByValue(StartingValue);

				// SET THE SELECTED TEXT
				if (DdlListBox.SelectedIndex >= 0)
					SelectedTextObject.text = DdlListBox.GetTextByIndex(DdlListBox.SelectedIndex);
				else
					SelectedTextObject.text = _strPlaceholder;

				// HIDE THE LISTBOX
				DdlListBox.Hide();
			}
		}

	#endregion

	#region "PUBLIC FUNCTIONS"

		#region "DROPDOWN LIST ITEMS"

			// HANDLE LISTBOX ITEMS
			public	override	void			Clear()
			{
				if (DdlListBox != null)
						DdlListBox.Clear();
			}

			// -- ADD ITEM TO LISTBOX
			public	override	void			AddItem(string		strValue,	string strText, string strIcon = "",	string	strSub = "")
			{
				if (DdlListBox != null)
					DdlListBox.AddItem(strValue,	strText, strIcon, strSub);
			}
			public	override	void			AddItem(string		strValue,	string strText, Sprite sprIcon,				string	strSub = "")
			{
				if (DdlListBox != null)
					DdlListBox.AddItem(strValue,	strText, sprIcon, strSub);
			}

			public	override	void			AddItem(string		strValue,	string strText, string strIcon, int			intSub)
			{
				if (DdlListBox != null)
					DdlListBox.AddItem(strValue, strText, strIcon, intSub.ToString());
			}
			public	override	void			AddItem(string		strValue,	string strText, string strIcon, float		fSub)
			{
				if (DdlListBox != null)
					DdlListBox.AddItem(strValue, strText, strIcon, fSub.ToString());
			}
			public	override	void			AddItem(string		strValue,	string strText, Sprite sprIcon, int			intSub)
			{
				if (DdlListBox != null)
					DdlListBox.AddItem(strValue, strText, sprIcon, intSub.ToString());
			}
			public	override	void			AddItem(string		strValue,	string strText, Sprite sprIcon, float		fSub)
			{
				if (DdlListBox != null)
					DdlListBox.AddItem(strValue, strText, sprIcon, fSub.ToString());
			}

			public	override	void			AddItem(string[]	strValue,	string strText)
			{
				if (DdlListBox != null)
					DdlListBox.AddItem(strValue, strText);
			}
			public	override	void			AddItem(string[]	strValue,	string strText, string strIcon)
			{
				if (DdlListBox != null)
					DdlListBox.AddItem(strValue,	strText, strIcon);
			}
			public	override	void			AddItem(string[]	strValue,	string strText, string strIcon, string	strSub)
			{
				if (DdlListBox != null)
					DdlListBox.AddItem(strValue,	strText, strIcon, strSub);
			}
			public	override	void			AddItem(string[]	strValue,	string strText, string strIcon, int			intSub)
			{
				if (DdlListBox != null)
					DdlListBox.AddItem(strValue, strText, strIcon, intSub);
			}
			public	override	void			AddItem(string[]	strValue,	string strText, string strIcon, float		fSub)
			{
				if (DdlListBox != null)
					DdlListBox.AddItem(strValue,	strText, strIcon, fSub);
			}
			public	override	void			AddItem(string[]	strValue,	string strText, Sprite sprIcon)
			{
				if (DdlListBox != null)
					DdlListBox.AddItem(strValue, strText, sprIcon);
			}
			public	override	void			AddItem(string[]	strValue,	string strText, Sprite sprIcon, string	strSub)
			{
				if (DdlListBox != null)
					DdlListBox.AddItem(strValue,	strText, sprIcon, strSub);
			}
			public	override	void			AddItem(string[]	strValue,	string strText, Sprite sprIcon, int			intSub)
			{
				if (DdlListBox != null)
					DdlListBox.AddItem(strValue, strText, sprIcon, intSub);
			}
			public	override	void			AddItem(string[]	strValue,	string strText, Sprite sprIcon, float		fSub)
			{
				if (DdlListBox != null)
					DdlListBox.AddItem(strValue, strText, sprIcon, fSub);
			}

			public	override	void			AddItem(int				intValue,	string strText)
			{
				if (DdlListBox != null)
					DdlListBox.AddItem(intValue, strText);
			}
			public	override	void			AddItem(int				intValue,	string strText, string strIcon)
			{
				if (DdlListBox != null)
					DdlListBox.AddItem(intValue, strText, strIcon);
			}
			public	override	void			AddItem(int				intValue,	string strText, string strIcon, string	strSub)
			{
				if (DdlListBox != null)
					DdlListBox.AddItem(intValue, strText, strIcon, strSub);
			}
			public	override	void			AddItem(int				intValue,	string strText, string strIcon, int			intSub)
			{
				if (DdlListBox != null)
					DdlListBox.AddItem(intValue, strText, strIcon, intSub);
			}
			public	override	void			AddItem(int				intValue,	string strText, string strIcon, float		fSub)
			{
				if (DdlListBox != null)
					DdlListBox.AddItem(intValue, strText, strIcon, fSub);
			}
			public	override	void			AddItem(int				intValue,	string strText, Sprite sprIcon)
			{
				if (DdlListBox != null)
					DdlListBox.AddItem(intValue, strText, sprIcon);
			}
			public	override	void			AddItem(int				intValue,	string strText, Sprite sprIcon, string	strSub)
			{
				if (DdlListBox != null)
					DdlListBox.AddItem(intValue, strText, sprIcon, strSub);
			}
			public	override	void			AddItem(int				intValue,	string strText, Sprite sprIcon, int			intSub)
			{
				if (DdlListBox != null)
					DdlListBox.AddItem(intValue, strText, sprIcon, intSub);
			}
			public	override	void			AddItem(int				intValue,	string strText, Sprite sprIcon, float		fSub)
			{
				if (DdlListBox != null)
					DdlListBox.AddItem(intValue, strText, sprIcon, fSub);
			}

			// -- REMOVE ITEM FROM LISTBOX
			public	override	void			RemoveItemByIndex(int			intIndex)
			{
				if (DdlListBox != null)
					DdlListBox.RemoveItemByIndex(intIndex);
			}
			public	override	void			RemoveItemByValue(string	strValue)
			{
				if (DdlListBox != null)
					DdlListBox.RemoveItemByValue(strValue);
			}
			public	override	void			RemoveItemByText(	string	strText)
			{
				if (DdlListBox != null)
					DdlListBox.RemoveItemByText(strText);
			}

			// -- SORT LISTBOX ITEMS
			public	override	void			Sort()
			{
				if (DdlListBox != null)
					DdlListBox.SortByText();
			}
			public	override	void			SortByText()
			{
				if (DdlListBox != null)
					DdlListBox.SortByText();
			}
			public	override	void			SortByValue()
			{
				if (DdlListBox != null)
					DdlListBox.SortByValue();
			}
			public	override	void			SortBySubText()
			{
				if (DdlListBox != null)
					DdlListBox.SortBySubText();
			}

			// -- SET LISTBOX SCROLLBAR POSITION
			public	override	void			SetToTop()
			{
				if (DdlListBox != null)
					DdlListBox.SetToTop();
			}
			public	override	void			SetToBottom()
			{
				if (DdlListBox != null)
					DdlListBox.SetToBottom();
			}

			// -- CHECK FOR LISTBOX ITEM WITH VALUE
			public	override	bool			HasItemWithValue(string strValue)
			{
				if (DdlListBox != null)
					return DdlListBox.HasItemWithValue(strValue);
				else
					return false;
			}
			public	override	bool			HasItemWithValue(int		intValue)
			{
				if (DdlListBox != null)
					return DdlListBox.HasItemWithValue(intValue);
				else
					return false;
			}
			public	override	bool			HasItemWithValue(float	fValue)
			{
				if (DdlListBox != null)
					return DdlListBox.HasItemWithValue(fValue);
				else 
					return false;
			}

			// -- ENABLE ONCLICK FOR LISTBOX ITEM (ALSO ADJUSTS ITEM STYLE)
			public	override	void			EnableByIndex(int				intIndex)
			{
				if (DdlListBox != null)
					DdlListBox.EnableByIndex(intIndex);
			}
			public	override	void			EnableByValue(string		strValue)
			{
				if (DdlListBox != null)
					DdlListBox.EnableByValue(strValue);
			}
			public	override	void			EnableByValue(int				intValue)
			{
				if (DdlListBox != null)
					DdlListBox.EnableByValue(intValue);
			}
			public	override	void			EnableByText(string			strText)
			{
				if (DdlListBox != null)
					DdlListBox.EnableByText(strText);
			}

			// -- DISABLE ONCLICK FOR LISTBOX ITEM (ALSO ADJUSTS ITEM STYLE)
			public	override	void			DisableByIndex(int			intIndex)
			{
				if (DdlListBox != null)
					DdlListBox.DisableByIndex(intIndex);
			}
			public	override	void			DisableByValue(string		strValue)
			{
				if (DdlListBox != null)
					DdlListBox.DisableByValue(strValue);
			}
			public	override	void			DisableByValue(int			intValue)
			{
				if (DdlListBox != null)
					DdlListBox.DisableByValue(intValue);
			}
			public	override	void			DisableByText(string		strText)
			{
				if (DdlListBox != null)
					DdlListBox.DisableByText(strText);
			}

			// -- SET LISTBOX ITEM TEXT
			public	override	void			SetItemTextByIndex(int		intIndex, string strNewText)
			{
				if (DdlListBox != null)
					DdlListBox.SetItemTextByIndex(intIndex, strNewText);
			}
			public	override	void			SetItemTextByValue(string strValue, string strNewText)
			{
				if (DdlListBox != null)
					DdlListBox.SetItemTextByValue(strValue, strNewText);
			}
			public	override	void			SetItemTextByValue(int		intValue, string strNewText)
			{
				if (DdlListBox != null)
					DdlListBox.SetItemTextByValue(intValue.ToString(), strNewText);
			}

			// -- SET LISTBOX ITEM SUBTEXT
			public	override	void			SetItemSubTextByIndex(int		intIndex, string strNewText)
			{
				if (DdlListBox != null)
					DdlListBox.Items[intIndex].SubText = strNewText;
			}
			public	override	void			SetItemSubTextByValue(string strValue, string strNewText)
			{
				if (DdlListBox != null)
					DdlListBox.SetItemSubTextByValue(strValue, strNewText);
			}
			public	override	void			SetItemSubTextByValue(int		intValue, string strNewText)
			{
				if (DdlListBox != null)
					DdlListBox.SetItemSubTextByValue(intValue.ToString(), strNewText);
			}

			// -- CHANGE ITEM ORDER
			public	override	bool			MoveItemUp(		int				intIndex)
			{
				if (DdlListBox != null)
					return DdlListBox.MoveItemUp(intIndex);
				else
					return false;
			}
			public	override	bool			MoveItemDown(	int				intIndex)
			{
				if (DdlListBox != null)
					return  DdlListBox.MoveItemDown(intIndex);
				else
					return false;
			}

			// -- GET LISTBOX ITEM VALUE
			public	override	string		GetValueByText(string		strText)
			{
				if (DdlListBox != null)
					return DdlListBox.GetValueByText(strText);
				else
					return "";
			}
			public	override	string		GetValueByIndex(int			intIndex)
			{
				if (DdlListBox != null)
					return DdlListBox.GetValueByIndex(intIndex);
				else
					return "";
			}
			public	override	int				GetIntValueByIndex(int	intIndex)
			{
				if (DdlListBox != null)
					return DdlListBox.GetIntValueByIndex(intIndex);
				else
					return -1;
			}

			// -- GET LISTBOX ITEM TEXT
			public	override	string		GetTextByValue(string		strvalue)
			{
				if (DdlListBox != null)
					return DdlListBox.GetTextByValue(strvalue);
				else
					return "";
			}
			public	override	string		GetTextByValue(int			intValue)
			{
				if (DdlListBox != null)
					return DdlListBox.GetTextByValue(intValue);
				else 
					return "";
			}
			public	override	string		GetTextByValue(float		fValue)
			{
				if (DdlListBox != null)
					return DdlListBox.GetTextByValue(fValue);
				else
					return "";
			}
			public	override	string		GetTextByIndex(int			intIndex)
			{
				if (DdlListBox != null)
					return DdlListBox.GetTextByIndex(intIndex);
				else
					return "";
			}

			// -- GET LISTBOX ITEM SUBTEXT
			public	override	string		GetSubTextByValue(string	strvalue)
			{
				if (DdlListBox != null)
					return DdlListBox.GetSubTextByValue(strvalue);
				else
					return "";
			}
			public	override	string		GetSubTextByValue(int			intValue)
			{
				if (DdlListBox != null)
					return DdlListBox.GetSubTextByValue(intValue);
				else
					return "";
			}
			public	override	string		GetSubTextByValue(float		fValue)
			{
				if (DdlListBox != null)
					return DdlListBox.GetSubTextByValue(fValue);
				else
					return "";
			}
			public	override	string		GetSubTextByIndex(int			intIndex)
			{
				if (DdlListBox != null)
					return DdlListBox.GetSubTextByIndex(intIndex);
				else
					return "";
			}

			// -- HANDLE SELECTION (SET LISTBOX ITEM SELECTED)
			public	override	void			SelectByIndex(int				intIndex, bool blnShifted = false, bool blnCtrled = false)
			{
				if (DdlListBox != null)
					DdlListBox.SelectByIndex(intIndex, blnShifted, blnCtrled);
			}
			public	override	void			SelectByValue(string		strValue)
			{
				if (DdlListBox != null)
					DdlListBox.SelectByValue(strValue);
			}
			public	override	void			SelectByText(	string		strText)
			{
				if (DdlListBox != null)
					DdlListBox.SelectByText(strText);
			}
			public	override	void			Unselect()
			{
				if (DdlListBox != null)
					DdlListBox.Unselect();
			}

			// -- HANDLE SELECTED INDEXES
			public	override	bool			IsSelectedByIndex(int intIndex)
			{
				if (DdlListBox != null)
					return DdlListBox.IsSelectedByIndex(intIndex);
				else
					return false;
			}
		
			// -- RESIZE THE CONTAINER (IF NECESSARY)
			public	override	void			UpdateListBoxContainerSize()
			{
				if (DdlListBox != null)
					DdlListBox.UpdateListBoxContainerSize();
			}

			// -- SHOW/HIDE THE CONTROL
			public	override	void			Hide()
			{	
				_blnDroppedDown = false;
				if (DdlListBox != null)
						DdlListBox.Hide();
				GetComponent<Image>().enabled = false;
				if (SelectedTextObject != null)
						SelectedTextObject.gameObject.SetActive(false);
				if (DropDownButton != null)
						DropDownButton.SetActive(false);
			}
			public	override	void			Show()
			{
				_blnDroppedDown = false;
				GetComponent<Image>().enabled = true;
				if (SelectedTextObject != null)
						SelectedTextObject.gameObject.SetActive(true);
				if (DropDownButton != null)
						DropDownButton.SetActive(true);
			}

		#endregion

	#endregion

	#region "BUTTON FUNCTIONS"

		public	void							OnDownButtonClick()
		{
			if (DdlListBox != null)
			{
				_blnDroppedDown = !_blnDroppedDown;
				if (_blnDroppedDown)
					DdlListBox.Show();
				else
					DdlListBox.Hide();
			}
		}

	#endregion

	#region "EVENT FUNCTIONS"

		public	void							OnChange(GameObject go, int intIndex)
		{
			if (go != this.DdlListBox.gameObject)
				return;

			if (SelectedTextObject != null)
					SelectedTextObject.text = DdlListBox.GetTextByIndex(intIndex);

			_blnDroppedDown = false;
			DdlListBox.Hide();
		}

	#endregion

}
