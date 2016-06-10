// ===========================================================================================================
//
// Class/Library: ListBox Control - Main Script
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
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public	delegate	void	OnListBoxSelectChanged(GameObject go, int intSelected);

public	class						ListBoxControl : MonoBehaviour 
{

	#region "STARTING LIST ITEM CLASS"

		[System.Serializable]
		public	class StartingListItem
		{
			public	string		Value		= "";
			public	string		Text		= "";
			public	Sprite		Icon		= null;
			public	int				Index		= -1;

			public	StartingListItem(string strValue, string strText, Sprite imgSprite = null)
			{
				Value = strValue;
				Text	= strText;
				Icon	= imgSprite;
			}
		}

	#endregion

	#region "PRIVATE VARIABLES"

		[SerializeField]
		private List<StartingListItem>		_startArray				= new List<StartingListItem>();
		[SerializeField]
		private string										_strTitle					= "";
		[SerializeField]
		private bool											_blnBestFit				= false;

		private List<ListBoxLineItem>			_items						= new List<ListBoxLineItem>();
		private RectTransform							_rtContainer			= null;
		private RectTransform							_rtScrollRect			= null;
		private int												_intSelectedItem	= -1;
		private List<int>									_intSelectedList	= new List<int>();
		private bool											_blnInitialized		= false;

	#endregion

	#region "PRIVATE PROPERTIES"

		private RectTransform							ContainerRect
		{
			get
			{
				if (_rtContainer == null)
					if (ScrollContainerObject != null)
						_rtContainer = ScrollContainerObject.GetComponent<RectTransform>();
				return _rtContainer;
			}
		}
		private RectTransform							ScrollRect
		{
			get
			{
				if (_rtScrollRect == null)
					if (ScrollRectObject != null)
						_rtScrollRect = ScrollRectObject.GetComponent<RectTransform>();
				return _rtScrollRect;
			}
		}

	#endregion

	#region "PUBLIC EDITOR PROPERTIES"

		public		GameObject							ScrollBarObject;
		public		GameObject							ScrollRectObject;
		public		GameObject							ScrollContainerObject;
		public		Text										ListBoxTitle;
		public		GameObject							ListBoxLineItemPrefabObject;

		public		Color										ItemNormalColor;
		public		Color										ItemHighlightColor;
		public		Color										ItemSelectedColor;
		public		Color										ItemDisabledColor;

		public		bool										CanMultiSelect			= false;
		public		float										Height							= 36;
		public		float										Spacing							=  4;

		// HANDLE LISTBOX TITLE
		public		string									Title
		{
			get
			{
				return _strTitle;
			}
			set
			{
				_strTitle = value.Trim();
				if (ListBoxTitle != null)
						ListBoxTitle.text = _strTitle;
			}
		}
		public		bool										TitleBestFit
		{
			get
			{
				return _blnBestFit;
			}
			set
			{
				_blnBestFit = value;
				if (ListBoxTitle != null)
						ListBoxTitle.resizeTextForBestFit = _blnBestFit;
			}
		}

		// HANDLE STARTING LIST ITEMS
		public	List<StartingListItem>		StartArray
		{
			get
			{
				return _startArray;
			}
		}

		// HANDLE SELECTION (GET)
		public		List<ListBoxLineItem>		Items
		{
			get
			{
				if (_items == null)
						_items = new List<ListBoxLineItem>();
				return _items;
			}
		}
		public		List<int>								SelectedIndexes
		{
			get
			{
				if (_intSelectedList == null)
						_intSelectedList = new List<int>();
				return _intSelectedList;
			}
		}
		public		List<string>						SelectedValues
		{
			get
			{
				if (_intSelectedItem < 0 || _intSelectedList == null || _intSelectedList.Count < 0)
					return null;
				List<string> st = new List<string>();
				for (int i = 0; i < _intSelectedList.Count; i++)
					st.Add(Items[_intSelectedList[i]].Value);
				return st;
			}
		}
		public		string									SelectedValue
		{
			get
			{
				if (_intSelectedItem < 0 || _intSelectedList == null || _intSelectedList.Count < 0)
					return null;
				return Items[_intSelectedList[0]].Value;
			}
		}
		public		string									SelectedArrayValue(int intIndex)
		{
			if (intIndex > Items[_intSelectedList[0]].Value.Split('|').Length - 1)
				return "";
			return Items[_intSelectedList[0]].Value.Split('|')[intIndex];
		}
		public		int											SelectedValueInt
		{
			get
			{
				if (_intSelectedItem < 0 || _intSelectedList == null || _intSelectedList.Count < 0)
					return -1;
				return Util.ConvertToInt(Items[_intSelectedList[0]].Value);
			}
		}
		public		int											SelectedArrayValueInt(int intIndex)
		{
			return Util.ConvertToInt(SelectedArrayValue(intIndex));
		}
		public		int											SelectedIndex
		{
			get
			{
				return _intSelectedItem;
			}
			set
			{
				_intSelectedItem = value;
			}
		}

		public		bool										IsInitialized
		{
			get
			{ 
				return _blnInitialized;
			}
		}

	#endregion

	#region "PRIVATE FUNCTIONS"

		private void			Awake()
		{
			// INITIALIZE THE ITEM LIST
			_intSelectedItem	= -1;
			_items						= new List<ListBoxLineItem>();
			_intSelectedList	= new List<int>();

			// REMOVE ANY GAMEOBJECTS IN THE CONTAINER
			if (ScrollContainerObject.transform.childCount > 0)
			{
				for (int i = ScrollContainerObject.transform.childCount - 1; i >= 0; i--)
					Destroy(ScrollContainerObject.transform.GetChild(i).gameObject);
			}
		}
		private void			Start()
		{
			// RESIZE THE ITEM CONTAINER TO THE WIDTH OF THE SCROLL RECT
			ContainerRect.sizeDelta = gameObject.GetComponent<RectTransform>().sizeDelta + ScrollRect.sizeDelta;
			
			// SET SCROLLBAR SENSITIVITY
			ScrollRectObject.GetComponent<ScrollRect>().scrollSensitivity = Height;

			// ADD INITIAL LIST ITEMS (IF THERE ARE ANY)
			if (StartArray.Count > 0)
			{
				for (int i = 0; i < StartArray.Count; i++)
				{
					AddItem(StartArray[i].Value, StartArray[i].Text, StartArray[i].Icon);
				}
			}

			// MARK CONTROL AS INITIALIZED
			_blnInitialized = true;
		}
		private void			OnEnable()
		{
			// MAKE SURE THAT THE LIST BOX ITEM CONTAINER IS PROPERLY SIZED (HEIGHT)
			UpdateListBoxContainerSize();
		}

		private void			ResizeContainer()
		{
			if (!Application.isPlaying)
				return;

			float fScroll = 1; 
			if (ScrollBarObject != null)
				fScroll =	ScrollBarObject.GetComponent<Scrollbar>().value;

			Vector2 v2 = ContainerRect.sizeDelta;
			v2.y = ((this.Height + this.Spacing) * Items.Count) + this.Spacing;
			ContainerRect.sizeDelta = v2;
			try
			{ 
				if (gameObject.activeSelf)
					StartCoroutine(SetScroll(fScroll));
			} catch { }
		}

		private void			SelectByRange(	int intEnd)
		{
			// SELECTS A RANGE OF ITEMS STARTING AT _intSelectedItem, EXTENDING TO intEnd
			int s = (int)Mathf.Sign(intEnd - _intSelectedItem);
			int i = _intSelectedItem;
			int e = intEnd;
			while  (e >= 0 && i >= 0 && i < Items.Count &&
						((s > 0 && i <= e) || (s < 0 && i >= e)))
			{
				if (Items[i].Enabled && Items[i].Shown)
				{ 
					Items[i].Select();
					_intSelectedList.Add(i);
				}
				i += s;
			}
		}

		private void			UnSelectItem(		int intIndex)
		{
			// UNSELECT SINGLE ITEM
			if (intIndex >= 0 && intIndex == _intSelectedItem && Items[intIndex] != null)
			{ 
				Items[_intSelectedItem].UnSelect();
				_intSelectedItem = -1;
			}

			// UNSELECT THE ITEM FROM THE LIST
			if (_intSelectedList.Count > 0)
			{
				int i = _intSelectedList.FindIndex(x => x == intIndex);
				if (i >= 0)
				{
					Items[_intSelectedList[i]].UnSelect();
					_intSelectedList.RemoveAt(i);
				}
			}
		}
		private void			UnSelectByRange(int intEnd)
		{
			int s = (int)Mathf.Sign(intEnd - _intSelectedItem);
			int i = _intSelectedItem;
			int e = intEnd;
			while (e >= 0 && i >= 0 && i < Items.Count &&
						((s > 0 && i <= e) || (s < 0 && i >= e)))
			{
				Items[_intSelectedList[i]].UnSelect();
				_intSelectedList.RemoveAt(i);
				i += s;
			}
		}
		private void			UnSelectAllItems()
		{
			// UNSELECT SINGLE ITEM
			if (_intSelectedItem >= 0 && Items[_intSelectedItem] != null)
				Items[_intSelectedItem].UnSelect();

			// UNSELECT MULTIPLY SELECTED ITEMS
			if (_intSelectedList.Count > 0)
			{
				for (int i = _intSelectedList.Count - 1; i >= 0; i--)
				{
					Items[_intSelectedList[i]].UnSelect();
					_intSelectedList.RemoveAt(i);
				}
			}
		}

		private IEnumerator		SetScroll(float fValue)
		{
			if (ScrollBarObject != null && ScrollBarObject.activeSelf)
			{
				yield return new WaitForSeconds(0.12f);
				ScrollBarObject.GetComponent<Scrollbar>().value = 0;
				yield return new WaitForSeconds(0.001f);
				ScrollBarObject.GetComponent<Scrollbar>().value = 1;
				yield return new WaitForSeconds(0.001f);
				ScrollBarObject.GetComponent<Scrollbar>().value = fValue;
			}
		}

	#endregion

	#region "PUBLIC FUNCTIONS"
	
		#region "LIST BOX STARTING ITEMS"

			// -- ADD ITEM TO STARTING LIST
			public	void			AddStartItem(string strValue, string strText, Sprite sprIcon = null)
			{
				int i = StartArray.FindIndex(x => x.Value.ToLower() == strValue.ToLower() || x.Text.ToLower() == strText.ToLower());
				if (i >= 0)
				{
					// OVERWRITE EXISTING ITEM
					StartArray[i].Value = strValue;
					StartArray[i].Text	= strText;
					StartArray[i].Icon	= sprIcon;
					StartArray[i].Index	= i;
				} else {
					StartArray.Add(new StartingListItem(strValue, strText, sprIcon));
					StartArray[StartArray.Count - 1].Index = StartArray.Count - 1;
				}
			}

			// -- REMOVE ITEM FROM STARTING LIST
			public	void			RemoveStartItemByIndex(int		intIndex)
			{
				if (intIndex < 0 || intIndex >= StartArray.Count)
					return;

				for (int i = StartArray.Count - 1; i >= intIndex; i--)
				{
					if (i > intIndex)
					{
						// MOVE THE ITEM UP ONE SLOT
						StartArray[i].Index = i - 1;
					} else {
						// REMOVE THE ITEM
						StartArray.RemoveAt(i);
					}
				}
			}
			public	void			RemoveStartItemByValue(string	strValue)
			{
				int i = StartArray.FindIndex(x => x.Value.ToLower() == strValue.ToLower());
				if (i >= 0)
					RemoveStartItemByIndex(i);
			}
			public	void			RemoveStartItemByText(	string	strText)
			{
				int i = StartArray.FindIndex(x => x.Text.ToLower() == strText.ToLower());
				if (i >= 0)
					RemoveStartItemByIndex(i);
			}

			// -- SORT ITEMS IN STARTING LIST
			public	void			SortStartByValue()
			{
				StartArray.Sort((p1, p2) => p1.Text.CompareTo(p2.Value));
				for (int i = 0; i < StartArray.Count; i++)
				{
					StartArray[i].Index = i;
				}
			}
			public	void			SortStartByText()
			{
				StartArray.Sort((p1, p2) => p1.Text.CompareTo(p2.Text));
				for (int i = 0; i < StartArray.Count; i++)
				{
					StartArray[i].Index = i;
				}
			}

		#endregion

		#region "LIST BOX ITEMS"

			// HANDLE LISTBOX ITEMS
			public	void			Clear()
			{
				// INITIALIZE THE ITEM LIST
				_intSelectedItem = -1;
				_items = new List<ListBoxLineItem>();
				_intSelectedList = new List<int>();

				// REMOVE ANY GAMEOBJECTS IN THE CONTAINER
				if (ScrollContainerObject.transform.childCount > 0)
				{
					for (int i = ScrollContainerObject.transform.childCount - 1; i >= 0; i--)
						Destroy(ScrollContainerObject.transform.GetChild(i).gameObject);
				}
			}

			// -- ADD ITEM TO LISTBOX
			public	void			AddItem(string		strValue,	string strText, string strIcon = "")
			{
				// CALCULATE ICON SPRITE
				Sprite sprIcon = null;
				if (strIcon != "")
				{
					sprIcon = Resources.Load<Sprite>(strIcon);
				}

				int i = Items.FindIndex(x => x.Value.ToLower() == strValue.ToLower() || x.Text.ToLower() == strText.ToLower());
				if (i >= 0)
				{
					// ITEM ALREADY EXISTS -- UPDATE IT
					Items[i].Value	= strValue;
					Items[i].Text		= strText;
					Items[i].SetIcon(sprIcon);
				} else {
					// ITEM DOES NOT EXIST -- CREATE IT
					i = Items.Count;
					GameObject go = (GameObject)Instantiate(ListBoxLineItemPrefabObject);
					go.transform.SetParent(ScrollContainerObject.transform);
					go.GetComponent<ListBoxLineItem>().ListBoxControlObject	= this.gameObject;
					go.GetComponent<ListBoxLineItem>().Index								= i;
					go.GetComponent<ListBoxLineItem>().Spacing							= this.Spacing;
					go.GetComponent<ListBoxLineItem>().Width								= ContainerRect.sizeDelta.x - (this.Spacing * 2);
					if (this.Height > 0)
						go.GetComponent<ListBoxLineItem>().Height							= this.Height;
					else
						this.Height = go.GetComponent<ListBoxLineItem>().Height;
					go.GetComponent<ListBoxLineItem>().ItemNormalColor			= ItemNormalColor;
					go.GetComponent<ListBoxLineItem>().ItemHighlightColor		=	ItemHighlightColor;
					go.GetComponent<ListBoxLineItem>().ItemSelectedColor		= ItemSelectedColor;
					go.GetComponent<ListBoxLineItem>().ItemDisabledColor		= ItemDisabledColor;
					go.GetComponent<ListBoxLineItem>().Value								= strValue;
					go.GetComponent<ListBoxLineItem>().Text									= strText;
					go.GetComponent<ListBoxLineItem>().SetIcon(sprIcon);
					go.GetComponent<ListBoxLineItem>().AutoSize();
					Items.Add(go.GetComponent<ListBoxLineItem>());
					ResizeContainer();
				}
			}
			public	void			AddItem(string		strValue,	string strText, Sprite sprIcon)
			{
				int i = Items.FindIndex(x => x.Value.ToLower() == strValue.ToLower() || x.Text.ToLower() == strText.ToLower());
				if (i >= 0)
				{
					// ITEM ALREADY EXISTS -- UPDATE IT
					Items[i].Value	= strValue;
					Items[i].Text		= strText;
					Items[i].SetIcon(sprIcon);
				} else {
					// ITEM DOES NOT EXIST -- CREATE IT
					i = Items.Count;
					GameObject go = (GameObject)Instantiate(ListBoxLineItemPrefabObject);
					go.transform.SetParent(ScrollContainerObject.transform);
					go.GetComponent<ListBoxLineItem>().ListBoxControlObject	= this.gameObject;
					go.GetComponent<ListBoxLineItem>().Index								= i;
					go.GetComponent<ListBoxLineItem>().Spacing							= this.Spacing;
					go.GetComponent<ListBoxLineItem>().Width								= ContainerRect.sizeDelta.x - (this.Spacing * 2);
					if (this.Height > 0)
						go.GetComponent<ListBoxLineItem>().Height							= this.Height;
					else
						this.Height = go.GetComponent<ListBoxLineItem>().Height;
					go.GetComponent<ListBoxLineItem>().ItemNormalColor			= ItemNormalColor;
					go.GetComponent<ListBoxLineItem>().ItemHighlightColor		=	ItemHighlightColor;
					go.GetComponent<ListBoxLineItem>().ItemSelectedColor		= ItemSelectedColor;
					go.GetComponent<ListBoxLineItem>().ItemDisabledColor		= ItemDisabledColor;
					go.GetComponent<ListBoxLineItem>().Value								= strValue;
					go.GetComponent<ListBoxLineItem>().Text									= strText;
					go.GetComponent<ListBoxLineItem>().SetIcon(sprIcon);
					go.GetComponent<ListBoxLineItem>().AutoSize();
					Items.Add(go.GetComponent<ListBoxLineItem>());
					ResizeContainer();
				}
			}
			public	void			AddItem(string[]	strValue,	string strText)
			{
				if (strValue != null && strValue.Length > 0 && strText.Trim() != "")
				{
					string strNewVal = "";
					for (int i = 0; i < strValue.Length; i++)
						strNewVal += "|" + strValue[i];
					strNewVal = strNewVal.Substring(1);
					AddItem(strNewVal, strText);
				}
			}
			public	void			AddItem(string[]	strValue,	string strText, string strIcon)
			{
				if (strValue != null && strValue.Length > 0 && strText.Trim() != "")
				{
					string strNewVal = "";
					for (int i = 0; i < strValue.Length; i++)
						strNewVal += "|" + strValue[i];
					strNewVal = strNewVal.Substring(1);
					AddItem(strNewVal, strText, strIcon);
				}
			}
			public	void			AddItem(string[]	strValue,	string strText, Sprite sprIcon)
			{
				if (strValue != null && strValue.Length > 0 && strText.Trim() != "")
				{
					string strNewVal = "";
					for (int i = 0; i < strValue.Length; i++)
						strNewVal += "|" + strValue[i];
					strNewVal = strNewVal.Substring(1);
					AddItem(strNewVal, strText, sprIcon);
				}
			}
			public	void			AddItem(int				intValue,	string strText)
			{
				AddItem(intValue.ToString(), strText);
			}
			public	void			AddItem(int				intValue,	string strText, string strIcon)
			{
				AddItem(intValue.ToString(), strText, strIcon);
			}
			public	void			AddItem(int				intValue,	string strText, Sprite sprIcon)
			{
				AddItem(intValue.ToString(), strText, sprIcon);
			}

			// -- REMOVE ITEM FROM LISTBOX
			public	void			RemoveItemByIndex(int			intIndex)
			{
				if (intIndex < 0 || intIndex >= Items.Count)
					return;

				for (int i = Items.Count - 1; i >= intIndex; i--)
				{
					if (i > intIndex)
					{ 
						// MOVE THE ITEM UP ONE SLOT
						Items[i].Index = i - 1;
						Items[i].AutoSize();
					} else {
						// REMOVE THE ITEM
						Items[i].Destroy();
						Items.RemoveAt(i);
					}
				}

				_intSelectedItem = -1;
				_intSelectedList = new List<int>();
				ResizeContainer();
			}
			public	void			RemoveItemByValue(string	strValue)
			{
				int i = Items.FindIndex(x => x.Value.ToLower() == strValue.ToLower());
				if (i >= 0)
					RemoveItemByIndex(i);
			}
			public	void			RemoveItemByText(	string	strText)
			{
				int i = Items.FindIndex(x => x.Text.ToLower() == strText.ToLower());
				if (i >= 0)
					RemoveItemByIndex(i);
			}

			// -- SORT LISTBOX ITEMS
			public	void			Sort()
			{
				SortByText();
			}
			public	void			SortByText()
			{
				Items.Sort((p1, p2) => p1.Text.CompareTo(p2.Text));
				for (int i = 0; i < Items.Count; i++)
				{ 
					Items[i].Index = i;
					Items[i].AutoSize();
				}
			}
			public	void			SortByValue()
			{
				Items.Sort((p1, p2) => p1.Text.CompareTo(p2.Value));
				for (int i = 0; i < Items.Count; i++)
				{ 
					Items[i].Index = i;
					Items[i].AutoSize();
				}
			}

			// -- SET LISTBOX SCROLLBAR POSITION
			public	void			SetToTop()
			{
				if (gameObject.activeSelf)
					StartCoroutine(SetScroll(1));
			}
			public	void			SetToBottom()
			{
				StartCoroutine(SetScroll(0));
			}

			// -- CHECK FOR LISTBOX ITEM WITH VALUE
			public	bool			HasItemWithValue(string strValue)
			{
				return Items.FindIndex(x => x.Value.Trim().ToLower() == strValue.Trim().ToLower()) >= 0;
			}
			public	bool			HasItemWithValue(int		intValue)
			{
				return HasItemWithValue(intValue.ToString());
			}
			public	bool			HasItemWithValue(float	fValue)
			{
				return HasItemWithValue(fValue.ToString());
			}

			// -- ENABLE ONCLICK FOR LISTBOX ITEM (ALSO ADJUSTS ITEM STYLE)
			public	void			EnableByIndex(int				intIndex)
			{
				if (intIndex >= 0)
					Items[intIndex].Enabled = true;
			}
			public	void			EnableByValue(string		strValue)
			{
				EnableByIndex(Items.FindIndex(x => x.Value.ToLower() == strValue.ToLower()));
			}
			public	void			EnableByValue(int				intValue)
			{
				EnableByIndex(Items.FindIndex(x => x.Value.ToLower() == intValue.ToString().ToLower()));
			}
			public	void			EnableByText(string			strText)
			{
				EnableByIndex(Items.FindIndex(x => x.Text.ToLower() == strText.ToLower()));
			}

			// -- DISABLE ONCLICK FOR LISTBOX ITEM (ALSO ADJUSTS ITEM STYLE)
			public	void			DisableByIndex(int			intIndex)
			{
				if (intIndex >= 0)
					Items[intIndex].Enabled = false;
			}
			public	void			DisableByValue(string		strValue)
			{
				DisableByIndex(Items.FindIndex(x => x.Value.ToLower() == strValue.ToLower()));
			}
			public	void			DisableByValue(int			intValue)
			{
				DisableByIndex(Items.FindIndex(x => x.Value.ToLower() == intValue.ToString().ToLower()));
			}
			public	void			DisableByText(string		strText)
			{
				DisableByIndex(Items.FindIndex(x => x.Text.ToLower() == strText.ToLower()));
			}

			// -- SET LISTBOX ITEM TEXT
			public	void			SetItemTextByIndex(int		intIndex, string strNewText)
			{
				Items[intIndex].Text = strNewText;
			}
			public	void			SetItemTextByValue(string strValue, string strNewText)
			{
				int i = Items.FindIndex(x => x.Value == strValue);
				if (i >= 0)
					SetItemTextByIndex(i, strNewText);
			}
			public	void			SetItemTextByValue(int		intValue, string strNewText)
			{
				SetItemTextByValue(intValue.ToString(), strNewText);
			}

			// -- CHANGE ITEM ORDER
			public	bool			MoveItemUp(		int				intIndex)
			{
				if (intIndex < 1)
					return false;

				// UNSELECT BOTH ITEMS
				bool blnOrig = IsSelectedByIndex(intIndex);
				bool blnTrgt = IsSelectedByIndex(intIndex - 1);
				UnSelectItem(intIndex);
				UnSelectItem(intIndex - 1);

					// MOVE THE ITEM
				ListBoxLineItem liOrig	= _items[intIndex];
				ListBoxLineItem liTrgt	= _items[intIndex - 1];
				liOrig.Index--;
				liTrgt.Index++;
				_items[intIndex]				= liTrgt;
				_items[intIndex - 1]		= liOrig;
				_items[intIndex].AutoSize();
				_items[intIndex - 1].AutoSize();

				// UPDATE SELECTION
				if (blnTrgt)
						SelectByIndex(intIndex);
				if (blnOrig)
						SelectByIndex(intIndex - 1);
				if (_intSelectedItem == intIndex)
						_intSelectedItem--;

				return true;
			}
			public	bool			MoveItemDown(	int				intIndex)
			{
				if (intIndex < 0 || intIndex >= _items.Count - 1)
					return false;

				// UNSELECT BOTH ITEMS
				bool blnOrig = IsSelectedByIndex(intIndex);
				bool blnTrgt = IsSelectedByIndex(intIndex + 1);
				UnSelectItem(intIndex);
				UnSelectItem(intIndex + 1);

				// MOVE THE ITEM
				ListBoxLineItem liOrig	= _items[intIndex];
				ListBoxLineItem liTrgt	= _items[intIndex + 1];
				liOrig.Index++;
				liTrgt.Index--;
				_items[intIndex]				= liTrgt;
				_items[intIndex + 1]		= liOrig;
				_items[intIndex].AutoSize();
				_items[intIndex + 1].AutoSize();

				// UPDATE SELECTION
				if (blnTrgt)
						SelectByIndex(intIndex);
				if (blnOrig)
						SelectByIndex(intIndex + 1);
				if (_intSelectedItem == intIndex)
						_intSelectedItem++;

				return true;
			}

			// -- GET LISTBOX ITEM VALUE / TEXT
			public	string		GetValueByText(string		strText)
			{
				int i = Items.FindIndex(x => x.Text.ToLower() == strText.Trim().ToLower());
				if (i < 0)
					return "";
				else
					return Items[i].Value;
			}
			public	string		GetValueByIndex(int			intIndex)
			{
				if (intIndex < 0 || intIndex >= Items.Count)
					return "";
				return Items[intIndex].Value;
			}
			public	int				GetIntValueByIndex(int	intIndex)
			{
				if (intIndex < 0 || intIndex >= Items.Count)
					return -1;
				return Util.ConvertToInt(Items[intIndex].Value);
			}
			public	string		GetTextByValue(string		strvalue)
			{
				int i = Items.FindIndex(x => x.Value.ToLower() == strvalue.Trim().ToLower());
				if (i < 0)
					return "";
				else
					return Items[i].Text;
			}
			public	string		GetTextByValue(int			intValue)
			{
				return GetTextByValue(intValue.ToString());
			}
			public	string		GetTextByValue(float		fValue)
			{
				return GetTextByValue(fValue.ToString());
			}
			public	string		GetTextByIndex(int			intIndex)
			{
				if (intIndex < 0 || intIndex >= Items.Count)
					return "";
				return Items[intIndex].Text;
			}

			// -- HANDLE SELECTION (SET LISTBOX ITEM SELECTED)
			public	void			SelectByIndex(int				intIndex, bool blnShifted = false, bool blnCtrled = false)
			{
				// DATA INTEGRITY CHECK
				if (intIndex < -1 || intIndex >= Items.Count)
					return;

				// MULTI-SELECT OVERRIDE
				blnShifted	= blnShifted	&& CanMultiSelect;
				blnCtrled		= blnCtrled		&& CanMultiSelect;

				// UNSHIFTED/UNCONTROLLED/UNSELECTED CLICK -- (CLICKING FOR THE FIRST TIME)
				if ((!blnShifted && !blnCtrled) || _intSelectedItem < 0)
				{
					UnSelectAllItems();
					_intSelectedItem = intIndex;
					if (_intSelectedItem >= 0 && Items[_intSelectedItem].Enabled)
					{ 
						Items[_intSelectedItem].Select();
						_intSelectedList.Add(intIndex);
					}
				// CONTROLLED CLICK -- (ADD ITEM TO SELECTED LIST)
				} else if (blnCtrled) {
					if (intIndex >= 0 && intIndex < Items.Count && Items[intIndex].Enabled)
					{
						if (IsSelectedByIndex(intIndex))
							UnSelectItem( intIndex);
						else
						{ 
							Items[intIndex].Select();
							_intSelectedList.Add(intIndex);
						}
					}
				// SHIFT-CLICK -- (ADD RANGE OF ITEMS TO SELECTED LIST)
				} else if (blnShifted) {
					UnSelectAllItems();
					SelectByRange(intIndex);
				}
				if (_intSelectedItem >= 0)
				{
					if (this.OnChange != null) 
						OnChange(this.gameObject, _intSelectedItem);
				}
			}
			public	void			SelectByValue(string		strValue)
			{
				int i = Items.FindIndex(x => x.Value.ToLower() == strValue.ToLower());
				SelectByIndex(i);
			}
			public	void			SelectByText(	string		strText)
			{
				int i = Items.FindIndex(x => x.Text.ToLower() == strText.ToLower());
				SelectByIndex(i);
			}
			public	void			Unselect()
			{
				UnSelectAllItems();
				_intSelectedItem = -1;
				_intSelectedList = new List<int>();
			}

			// -- HANDLE SELECTED INDEXES
			public	bool			IsSelectedByIndex(int intIndex)
			{
				return (_intSelectedItem == intIndex || _intSelectedList.FindIndex(x => x == intIndex) >= 0);
			}
		
			// -- RESIZE THE CONTAINER (IF NECESSARY)
			public	void			UpdateListBoxContainerSize()
			{
				Vector2 v2 = ContainerRect.sizeDelta;
				v2.y = ((this.Height + this.Spacing) * Items.Count) + this.Spacing;
				ContainerRect.sizeDelta = v2;
			}

		#endregion

	#endregion

	#region "EVENT FUNCTIONS"

		public	event						OnListBoxSelectChanged		OnChange;

	#endregion

}




//		LBcontrol.AddItem(new string []{"0", "None", "Backstep", "3.141562"}, "-- None --");

