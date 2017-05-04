// ===========================================================================================================
//
// Class/Library: DropDownList Control - Main Script
//        Author: Michael Marzilli   ( http://www.linkedin.com/in/michaelmarzilli , http://www.develteam.com/Developer/Rowell/Portfolio )
//       Created: Jun 30, 2016
//	
// VERS 1.0.000 : Jun 30, 2016 :	Original File Created. Released for Unity 3D.
//			1.0.001 : Jul 25, 2016 :	Added a check to make sure the control does not extend outside of the screen.
//																Added the ability to set the control's Parent container. The default will be the
//																object's direct parent (if the gameobject in the inspector is left null).
//																This is for future use, and currently unused.
//
// NOTE:	TRY TO KEEP THE NUMBER OF ITEMS IN A DROPDOWN LIST TO 500 ITEMS OR LESS.
//				OTHERWISE, YOU MAY EXPERIENCE A MOMENTARY PAUSE WHEN THE CONTROL IS OPENED AND COLLAPSED.
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
using System.Collections.Generic;

public delegate void OnDropDownSelectChanged(GameObject go, int intSelected);

public class DropDownListControl : ListBoxControl 
{

	#region "PRIVATE CONSTANTS"

		private const	float				CONTROL_BORDER						= 30;
		private const int					MINIMUM_DISPLAYED_CELLS		= 3;

	#endregion

	#region "PRIVATE VARIABLES"

		[SerializeField]
		private	string						_strStartingValue		= "";
		[SerializeField]
		private string						_strPlaceholder			= "Select Item...";
		[SerializeField]
		private float							_fHeight						= 36;

		private bool							_blnIamAwake				= false;
		private bool							_blnDroppedDown			= false;
		private Event							_evntClick					= null;
		private RectTransform			_trnLB							= null;
		private float							_fOffset						= -1;
		private int								_intIndexPosition		= -1;

	#endregion

	#region "PRIVATE PROPERTIES"

		private int								ControlIndex
		{
			get
			{
				if (_intIndexPosition < 0)
						_intIndexPosition = DdlListBox.transform.parent.GetSiblingIndex();
				return _intIndexPosition;
			}
		}

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
		public	string										PlaceholderText
		{
			get
			{
				return _strPlaceholder;
			}
			set
			{
				_strPlaceholder = value.Trim();
				SelectedTextObject.text = _strPlaceholder;
			}
		}
		public	float											LineItemHeight
		{
			get
			{
				return _fHeight;
			}
			set
			{
				_fHeight = value;
				if (DdlListBox != null)
						DdlListBox.Height = _fHeight;
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

		public	GameObject				ParentContainer		= null;
		public	Text							SelectedTextObject;
		public	GameObject				DropDownButton;
		public	ListBoxControl		DdlListBox;

	#endregion

	#region "PRIVATE FUNCTIONS"

		private void							Awake()
		{
			this.ListBoxMode	= ListBoxModes.DropDownList;

			if (DdlListBox != null)
			{
				DdlListBox.gameObject.SetActive(true);
				DdlListBox.PartOfDDL	= true;
				DdlListBox.Height			= _fHeight;
				DdlListBox.InitStartItems(_startArray);
				_blnIamAwake = true;
			}
			if (ParentContainer == null)
					ParentContainer =  this.transform.parent.gameObject;
		}
		private void							Start()
		{
			// INITIALIZE THE DROPDOWN LISTBOX (IF APPLICABLE)
			if (DdlListBox != null)
			{ 
				DdlListBox.gameObject.SetActive(true);
				int i = ControlIndex;
				if (!_blnIamAwake)
						Awake();
				DdlListBox.Show();
				DdlListBox.OnChange += this.OnChange;
			}
			// RE-POSITION THE DROPDOWN LISTBOX (IF APPLICABLE)
			DetermineDropDownPosition();
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
				DdlListBox.transform.parent.SetSiblingIndex(ControlIndex);
			}
		}
		private void							OnGUI()
		{
			Event e = Event.current;
			if (_blnInitialized && _blnDroppedDown)
				if (e.type == EventType.MouseUp)
				{
					// CHECKING FOR CLICK OUTSIDE OF DDL CONTROL IN A COROUTINE 
					// BECAUSE WE DON'T WANT TO MISS A CLICK ON A LIST ITEM
					_evntClick = e;
					StartCoroutine(CheckHide());
				}
		}

		private bool							IsOverflowing(GameObject go)
		{
			// DETERMINE IF THE GAMEOBJECT/CONTROL EXTENDS OFF THE SCREEN
			Rect			screenRect		= new Rect(0f, 0f, Screen.width, Screen.height);
			Vector3[]	objectCorners	= new Vector3[4];
			go.GetComponent<RectTransform>().GetWorldCorners(objectCorners);
 
			foreach (Vector3 corner in objectCorners)
			{
				if (!screenRect.Contains(corner))
					return true;
			}
			return false; 
		}
		private void							DetermineDropDownPosition()
		{
			// DETERMINE IF THE DROPDOWN LISTBOX EXTENDS BEYOND THE BOUNDARIES OF THE SCREEN
			// IF SO, MOVE LISTBOX ABOVE THE DDL CONTROL. OTHERWISE, LEAVE IT BELOW THE DDL CONTROL.
			if (DdlListBox == null)
				return;
			if (!IsOverflowing(DdlListBox.gameObject))
				return;

			RectTransform rt				= DdlListBox.GetComponent<RectTransform>();
			Vector3				v3Pos			= rt.position;
			Vector2				v2Size		= rt.sizeDelta;
			float					fThisSize	= this.GetComponent<RectTransform>().sizeDelta.y;
			float					fNewPos		= v3Pos.y + v2Size.y + fThisSize + (DdlListBox.Spacing * 2);
										v3Pos.y		= fNewPos;
			DdlListBox.GetComponent<RectTransform>().position = v3Pos;
		}

		private IEnumerator				CheckHide()
		{
			yield return new WaitForSeconds(0.1f);
			HideIfClickedOutside(DdlListBox.gameObject, _evntClick);
		}
		private bool							HideIfClickedOutside(GameObject panel, Event e)
		{
			if (panel != null && panel.activeSelf && _blnDroppedDown)
			{
				if (_trnLB == null)
						_trnLB					= DdlListBox.GetComponent<RectTransform>();
				if (_fOffset < 0)
						_fOffset				= SelectedTextObject.GetComponent<RectTransform>().rect.size.y;

				float		fCalc				=	Screen.height  - (_trnLB.position.y + _fOffset + CONTROL_BORDER);
				Vector2 mouse				= new Vector2(e.mousePosition.x,  e.mousePosition.y);
				Vector2 location		= new Vector2(_trnLB.position.x - CONTROL_BORDER,  fCalc);
				Vector2 size				= new Vector2(_trnLB.rect.size.x + (CONTROL_BORDER * 2), _trnLB.rect.size.y + _fOffset + (CONTROL_BORDER * 2));
				Rect		r						= new Rect(location, size);

				if (!r.Contains(mouse))
				{
					_evntClick = null;
					DdlListBox.Hide();
					_blnDroppedDown = false;
					DdlListBox.transform.parent.SetSiblingIndex(ControlIndex);
					return true;
				}
			}
			_evntClick = null;
			return false;
		}

	#endregion

	#region "PUBLIC FUNCTIONS"

		#region "DROPDOWN LIST ITEMS"

			// HANDLE LISTBOX ITEMS
			public	override	void			Clear()
			{
				if (DdlListBox != null)
						DdlListBox.Clear();
				if (SelectedTextObject != null)
						SelectedTextObject.text = PlaceholderText;
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
				{ 
					DdlListBox.SelectByIndex(intIndex, blnShifted, blnCtrled);
					if (DdlListBox.SelectedIndex < 0)
						SelectedTextObject.text = _strPlaceholder;
					else
						SelectedTextObject.text = DdlListBox.SelectedText;
				}
			}
			public	override	void			SelectByValue(string		strValue)
			{
				if (DdlListBox != null)
				{ 
					DdlListBox.SelectByValue(strValue);
					if (DdlListBox.SelectedIndex < 0)
						SelectedTextObject.text = _strPlaceholder;
					else
						SelectedTextObject.text = DdlListBox.SelectedText;
				}
			}
			public	override	void			SelectByText(	string		strText)
			{
				if (DdlListBox != null)
				{ 
					DdlListBox.SelectByText(strText);
					if (DdlListBox.SelectedIndex < 0)
						SelectedTextObject.text = _strPlaceholder;
					else
						SelectedTextObject.text = DdlListBox.SelectedText;
				}
			}
			public	override	void			Unselect()
			{
				if (DdlListBox != null)
					DdlListBox.Unselect();
				SelectedTextObject.text = _strPlaceholder;
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

		private void									DoShow()
		{
			if (_intIndexPosition < 0)
					_intIndexPosition = DdlListBox.transform.parent.GetSiblingIndex();
			DdlListBox.transform.parent.SetSiblingIndex(transform.parent.childCount - 1);
			UpdateListBoxContainerSize();
			DdlListBox.Show();
			DdlListBox.SetToIndex(this.SelectedIndex);
		}

	#endregion

	#region "BUTTON FUNCTIONS"

		public	void							OnDownButtonClick()
		{
			if (DdlListBox != null)
			{
				_blnDroppedDown = !_blnDroppedDown;
				if (_blnDroppedDown)
					DoShow();
				else
				{
					DdlListBox.transform.parent.SetSiblingIndex(ControlIndex);
					DdlListBox.Hide();
				}
			}
		}

	#endregion

	#region "EVENT FUNCTIONS"

		public	event	OnDropDownSelectChanged	OnSelectionChange;

		public	new void					OnChange(GameObject go, int intIndex)
		{
			if (go != DdlListBox.gameObject)
				return;

			if (SelectedTextObject != null)
				if (intIndex < 0)
					SelectedTextObject.text = PlaceholderText;
				else
					SelectedTextObject.text = DdlListBox.GetTextByIndex(intIndex);

			_blnDroppedDown = false;
			DdlListBox.transform.parent.SetSiblingIndex(ControlIndex);
			DdlListBox.Hide();

			if (SelectedIndex >= 0 && this.OnSelectionChange != null)
					OnSelectionChange(this.gameObject, SelectedIndex);
		}

	#endregion

}
