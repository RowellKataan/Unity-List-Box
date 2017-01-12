// ===========================================================================================================
//
// Class/Library: ListBox Control - Main Script
//        Author: Michael Marzilli   ( http://www.linkedin.com/in/michaelmarzilli , http://www.develteam.com/Developer/Rowell/Portfolio )
//       Created: Jun 10, 2016
//	
// VERS 1.0.000 : Jun 10, 2016 :	Original File Created. Released for Unity 3D.
//			1.0.001	:	Jun 11, 2016 :	Added a SubText field/element to the ListBox Control.
//																The SubText field is a right justified field that can add additional information.
//																Such as displaying a price for an item in and item list for a shop.
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

public	delegate	void	OnListBoxSelectChanged(	GameObject go, int intSelected);
public	delegate	void	OnListBoxDoubleClick(		GameObject go, int intSelected);

[System.Serializable]
public	class						ListBoxControl : MonoBehaviour 
{

	#region "PRIVATE CONSTANTS"

		// IF DROPDOWN LIST SELECTION IS NOT BEING PROPERLY SCROLLED TO WHEN THE DROPDOWN LIST IS SELECTED,
		// TRY INCREASING THE CONSTANT BELOW UNTIL THE SELECTED ITEM SCROLLS INTO VIEW PROPERLY
		// (THIS CONSTANT IS USED IN THE SetScroll(float fValue) IENUMERATOR
		private const float			SCROLL_DELAY	= 0.002f;		// BEST DEFAULT: 0.12f ??

	#endregion

	#region "STARTING LIST ITEM CLASS"

		[System.Serializable]
		public	class StartingListItem
		{
			public	string		Value		= "";
			public	string		Text		= "";
			public	string		SubText	= "";
			public	Sprite		Icon		= null;
			public	int				Index		= -1;

			public	StartingListItem(string strValue, string strText, Sprite imgSprite = null, string strSub = "")
			{
				Value		= strValue;
				Text		= strText;
				SubText	= strSub;
				Icon		= imgSprite;
			}
		}

	#endregion

	#region "PRIVATE VARIABLES"

		// SERIALIZED FIELDS
		[SerializeField]
		protected		List<StartingListItem>	_startArray				= new List<StartingListItem>();
		[SerializeField]
		private string											_strTitle					= "";
		[SerializeField]
		private bool												_blnBestFit				= false;
		[SerializeField]
		private bool												_blnAllowDblClick	= false;
		[SerializeField]
		private bool												_blnPartOfDDL			= false;

		private ListBoxModes								_lbMode						= ListBoxModes.ListBox;
		private List<ListBoxLineItem>				_items						= new List<ListBoxLineItem>();
		private RectTransform								_rtContainer			= null;
		private RectTransform								_rtScrollRect			= null;
		private int													_intItemCount			= 0;
		private int													_intSelectedItem	= -1;
		private List<int>										_intSelectedList	= new List<int>();

		protected	bool											_blnInitialized		= false;

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

		[SerializeField]
		public		Color										ItemNormalColor;
		[SerializeField]
		public		Color										ItemHighlightColor;
		[SerializeField]
		public		Color										ItemSelectedColor;
		[SerializeField]
		public		Color										ItemDisabledColor;

		[SerializeField]
		public		bool										CanMultiSelect			= false;
		[SerializeField]
		public		float										Height							= 36;
		[SerializeField]
		public		float										Spacing							=  4;
		[SerializeField]
		public		char										SeparatorChar				= '|';

	#endregion

	#region "PUBLIC PROPERTIES"

		public		enum										ListBoxModes : int { ListBox = 0, DropDownList = 1 }
		public		ListBoxModes						ListBoxMode
		{
			get
			{
				return _lbMode;
			}
			set
			{
				_lbMode = value;
			}
		}

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
				if (ListBoxMode == ListBoxModes.ListBox && ListBoxTitle != null)
				{
					ListBoxTitle.gameObject.SetActive(_strTitle != "");
					ListBoxTitle.text = _strTitle;
				} else
					ListBoxTitle.gameObject.SetActive(false);
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
				if (ListBoxMode == ListBoxModes.ListBox && ListBoxTitle != null)
						ListBoxTitle.resizeTextForBestFit = _blnBestFit;
			}
		}
		public		bool										AllowDoubleClick
		{
			get
			{
				return _blnAllowDblClick && !_blnPartOfDDL && ListBoxMode == ListBoxModes.ListBox;
			}
			set
			{
				_blnAllowDblClick = value;
			}
		}
		public		bool										PartOfDDL
		{
			get
			{
				return _blnPartOfDDL;
			}
			set
			{
				_blnPartOfDDL = value;
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
		public		virtual	List<ListBoxLineItem>		Items
		{
			get
			{
				if (_items == null)
						_items = new List<ListBoxLineItem>();
				return _items;
			}
		}
		public		virtual	List<int>				SelectedIndexes
		{
			get
			{
				if (_intSelectedList == null)
						_intSelectedList = new List<int>();
				return _intSelectedList;
			}
		}
		public		virtual	List<string>		SelectedValues
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
		public		virtual	string					SelectedValuesString
		{
			get
			{
				List<string> st = SelectedValues;
				if (st == null || st.Count < 1)
					return "";
				string strOut = "";
				for (int i = 0; i < st.Count; i++)
				{
					if (st[i].Trim() != "")
						strOut += SeparatorChar + st[i];
				}
				if (strOut.Length > 1)
					strOut = strOut.Substring(1);
				return strOut;
			}
		}
		public		virtual	string					SelectedValue
		{
			get
			{
				if (_intSelectedItem < 0 || _intSelectedList == null || _intSelectedList.Count < 0)
					return null;
				return Items[_intSelectedList[0]].Value;
			}
		}
		public		virtual	string					SelectedArrayValue(int intIndex)
		{
			if (intIndex > Items[_intSelectedList[0]].Value.Split(SeparatorChar).Length - 1)
				return "";
			return Items[_intSelectedList[0]].Value.Split(SeparatorChar)[intIndex];
		}
		public		virtual	int							SelectedValueInt
		{
			get
			{
				if (_intSelectedItem < 0 || _intSelectedList == null || _intSelectedList.Count < 0)
					return -1;
				return Util.ConvertToInt(Items[_intSelectedList[0]].Value);
			}
		}
		public		virtual	int							SelectedArrayValueInt(int intIndex)
		{
			return Util.ConvertToInt(SelectedArrayValue(intIndex));
		}
		public		virtual	int							SelectedIndex
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
		public		virtual	string					SelectedText
		{
			get
			{
				if (_intSelectedItem < 0 || _intSelectedList == null || _intSelectedList.Count < 0)
					return "";
				return Items[_intSelectedList[0]].Text;
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

			// EXIT IF THIS IS A DROPDOWN LIST
			if (ListBoxMode == ListBoxModes.DropDownList)
				return;

			// REMOVE ANY GAMEOBJECTS IN THE CONTAINER
			if (ScrollContainerObject != null)
			{ 
				if (ScrollContainerObject.transform.childCount > 0)
				{
					for (int i = ScrollContainerObject.transform.childCount - 1; i >= 0; i--)
						Destroy(ScrollContainerObject.transform.GetChild(i).gameObject);
				}
			}
		}
		private void			Start()
		{
			if (!gameObject.activeInHierarchy)		//	&& !_blnInitialized)
				return;

			// RESIZE THE ITEM CONTAINER TO THE WIDTH OF THE SCROLL RECT
			if (ContainerRect != null)
					ContainerRect.sizeDelta = new Vector2(ScrollRect.rect.width, ScrollRect.rect.height);

			// SET SCROLLBAR SENSITIVITY
			if (ScrollRectObject != null)
					ScrollRectObject.GetComponent<ScrollRect>().scrollSensitivity = Height - Spacing;
			if (ScrollBarObject != null)
					ScrollBarObject.GetComponent<Scrollbar>().numberOfSteps = 1;

			// EXIT IF THIS IS A DROPDOWN LIST
			if (ListBoxMode == ListBoxModes.DropDownList)
				return;

			// SET TITLE
			if (ListBoxTitle != null)
				Title = _strTitle;

			// CHECK FOR LINE ITEM PREFAB
			if (ListBoxLineItemPrefabObject == null)
				Debug.LogError(gameObject.name + " is Missing the Line Item Prefab. Please add the Prefab.");
			else if (ListBoxLineItemPrefabObject.GetComponent<ListBoxLineItem>() == null)
				Debug.LogError(gameObject.name + " is Missing the Line Item Prefab. Please add the Prefab.");

			// ADD INITIAL LIST ITEMS (IF THERE ARE ANY)
			if (StartArray.Count > 0)
			{
				for (int i = 0; i < StartArray.Count; i++)
				{
					AddItem(StartArray[i].Value, StartArray[i].Text, StartArray[i].Icon);
				}
			}

			// MARK CONTROL AS INITIALIZED
			_blnInitialized		= true;
		}
		private void			OnEnable()
		{
			if (!_blnInitialized && gameObject.activeInHierarchy)
				Start();

		// MAKE SURE THAT THE LIST BOX ITEM CONTAINER IS PROPERLY SIZED (HEIGHT)
		if (ListBoxMode == ListBoxModes.ListBox)
				UpdateListBoxContainerSize();
		}

		private void			ResizeContainer()
		{
			if (!Application.isPlaying || ListBoxMode == ListBoxModes.DropDownList)
				return;

			float fScroll = 1; 
			if (ScrollBarObject != null)
				fScroll =	ScrollBarObject.GetComponent<Scrollbar>().value;

			Vector2 v2 = ContainerRect.sizeDelta;
			v2.y = ((this.Height + this.Spacing) * Items.Count) + this.Spacing;
			ContainerRect.sizeDelta = v2;
			try
			{
				if (gameObject.activeInHierarchy)
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
			if (ListBoxMode == ListBoxModes.DropDownList)
				return;

			// UNSELECT SINGLE ITEM
			if (intIndex >= 0 && intIndex == _intSelectedItem && Items[intIndex] != null)
			{ 
				Items[_intSelectedItem].UnSelect();
				int i = _intSelectedList.FindIndex(x => x == intIndex);
				_intSelectedList.RemoveAt(i);
				if (_intSelectedList.Count > 0)
					_intSelectedItem = _intSelectedList[0];
				else
					_intSelectedItem = -1;
			} else

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
			if (ListBoxMode == ListBoxModes.DropDownList)
				return;

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
			if (ListBoxMode == ListBoxModes.DropDownList)
				return;

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
			yield return new WaitForSeconds(0.001f);
			if (gameObject.activeInHierarchy && ScrollBarObject != null && ScrollBarObject.activeSelf && ListBoxMode == ListBoxModes.ListBox)
			{
				yield return new WaitForSeconds(SCROLL_DELAY);
				ScrollBarObject.GetComponent<Scrollbar>().value = 0;
//			yield return new WaitForSeconds(0.0001f);
//			ScrollBarObject.GetComponent<Scrollbar>().value = 1;
				yield return new WaitForSeconds(0.0001f);
				ScrollBarObject.GetComponent<Scrollbar>().value = fValue;
			}
		}

		private void					PrivAddItem(		string		strValue,	string strText, string strIcon = "",	string	strSub = "")
		{
				// CHECK IF LINE ITEM PREFAB EXISTS
				if (ListBoxLineItemPrefabObject == null)
				{
					Debug.LogError(gameObject.name + " is Missing the Line Item Prefab. Please add the Prefab.");
					return;
				} else if (ListBoxLineItemPrefabObject.GetComponent<ListBoxLineItem>() == null) {
					Debug.LogError(gameObject.name + " is Missing the Line Item Prefab. Please add the Prefab.");
					return;
				}

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
					Items[i].Value		= strValue;
					Items[i].Text			= strText;
					Items[i].SubText	= strSub;
					Items[i].SetIcon(sprIcon);
				} else {
					// ITEM DOES NOT EXIST -- CREATE IT
					_intItemCount++;
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
					go.GetComponent<ListBoxLineItem>().SubText							= strSub;
					go.GetComponent<ListBoxLineItem>().SetIcon(sprIcon);
					go.GetComponent<ListBoxLineItem>().AutoSize();
					Items.Add(go.GetComponent<ListBoxLineItem>());
					ResizeContainer();
				}		
		}
		private void					PrivAddItem(		string		strValue,	string strText, Sprite sprIcon,				string	strSub = "")
		{
				// CHECK IF LINE ITEM PREFAB EXISTS
				if (ListBoxLineItemPrefabObject == null)
				{
					Debug.LogError(gameObject.name + " is Missing the Line Item Prefab. Please add the Prefab.");
					return;
				} else if (ListBoxLineItemPrefabObject.GetComponent<ListBoxLineItem>() == null) {
					Debug.LogError(gameObject.name + " is Missing the Line Item Prefab. Please add the Prefab.");
					return;
				}

				int i = Items.FindIndex(x => x.Value.ToLower() == strValue.ToLower() || x.Text.ToLower() == strText.ToLower());
				if (i >= 0)
				{
					// ITEM ALREADY EXISTS -- UPDATE IT
					Items[i].Value		= strValue;
					Items[i].Text			= strText;
					Items[i].SubText	= strSub;
					Items[i].SetIcon(sprIcon);
				} else {
					// ITEM DOES NOT EXIST -- CREATE IT
					_intItemCount++;
					i = Items.Count;
					GameObject go = (GameObject)Instantiate(ListBoxLineItemPrefabObject);
					go.transform.SetParent(ScrollContainerObject.transform);
					go.GetComponent<ListBoxLineItem>().ListBoxControlObject = this.gameObject;
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
					go.GetComponent<ListBoxLineItem>().SubText							= strSub;
					go.GetComponent<ListBoxLineItem>().SetIcon(sprIcon);
					go.GetComponent<ListBoxLineItem>().AutoSize();
					Items.Add(go.GetComponent<ListBoxLineItem>());
					ResizeContainer();
				}
		}

	#endregion

	#region "PUBLIC FUNCTIONS"
	
		#region "LIST BOX STARTING ITEMS"

			// -- CLEAR STARTING LIST
			public					void			ClearStartItems()
			{
				_startArray = new List<StartingListItem>();
			}
			public					void			InitStartItems(List<StartingListItem> sli)
			{
				ClearStartItems();
				foreach (StartingListItem s in sli)
				{
					_startArray.Add(s);
				}
			}

			// -- ADD ITEM TO STARTING LIST
			public	virtual	void			AddStartItem(string strValue, string strText, Sprite sprIcon = null, string strSub = "")
			{
				int i = StartArray.FindIndex(x => x.Value.ToLower() == strValue.ToLower() || x.Text.ToLower() == strText.ToLower());
				if (i >= 0)
				{
					// OVERWRITE EXISTING ITEM
					StartArray[i].Value		= strValue;
					StartArray[i].Text		= strText;
					StartArray[i].Icon		= sprIcon;
					StartArray[i].SubText	= strSub;
					StartArray[i].Index		= i;
				} else {
					StartArray.Add(new StartingListItem(strValue, strText, sprIcon, strSub));
					StartArray[StartArray.Count - 1].Index = StartArray.Count - 1;
				}
			}

			// -- REMOVE ITEM FROM STARTING LIST
			public	virtual	void			RemoveStartItemByIndex(int		intIndex)
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
			public	virtual	void			RemoveStartItemByValue(string	strValue)
			{
				int i = StartArray.FindIndex(x => x.Value.ToLower() == strValue.ToLower());
				if (i >= 0)
					RemoveStartItemByIndex(i);
			}
			public	virtual	void			RemoveStartItemByText(	string	strText)
			{
				int i = StartArray.FindIndex(x => x.Text.ToLower() == strText.ToLower());
				if (i >= 0)
					RemoveStartItemByIndex(i);
			}

			// -- SORT ITEMS IN STARTING LIST
			public	virtual	void			SortStartByValue()
			{
				StartArray.Sort((p1, p2) => p1.Text.CompareTo(p2.Value));
				for (int i = 0; i < StartArray.Count; i++)
				{
					StartArray[i].Index = i;
				}
			}
			public	virtual	void			SortStartByText()
			{
				StartArray.Sort((p1, p2) => p1.Text.CompareTo(p2.Text));
				for (int i = 0; i < StartArray.Count; i++)
				{
					StartArray[i].Index = i;
				}
			}
			public	virtual	void			SortStartBySub()
			{
				StartArray.Sort((p1, p2) => p1.SubText.CompareTo(p2.SubText));
				for (int i = 0; i < StartArray.Count; i++)
				{
					StartArray[i].Index = i;
				}
			}

		#endregion

		#region "LIST BOX ITEMS"

			// HANDLE LISTBOX ITEMS
			public	virtual	void			Clear()
			{
				// INITIALIZE THE ITEM LIST
				_intItemCount			= 0;
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
			public	virtual	void			AddItem(string		strValue,	string strText, string strIcon = "",	string	strSub = "")
			{
				PrivAddItem(strValue, strText, strIcon, strSub);
			}
			public	virtual void			AddItem(string		strValue, string strText, Sprite sprIcon,				string	strSub = "")
			{
				PrivAddItem(strValue, strText, sprIcon, strSub);
			}

			public	virtual	void			AddItem(string		strValue,	string strText, string strIcon, int			intSub)
			{
				AddItem(strValue, strText, strIcon, intSub.ToString());
			}
			public	virtual	void			AddItem(string		strValue,	string strText, string strIcon, float		fSub)
			{
				AddItem(strValue, strText, strIcon, fSub.ToString());
			}
			public	virtual	void			AddItem(string		strValue,	string strText, Sprite sprIcon, int			intSub)
			{
				AddItem(strValue, strText, sprIcon, intSub.ToString());
			}
			public	virtual	void			AddItem(string		strValue,	string strText, Sprite sprIcon, float		fSub)
			{
				AddItem(strValue, strText, sprIcon, fSub.ToString());
			}

			public	virtual	void			AddItem(string[]	strValue,	string strText)
			{
				if (strValue != null && strValue.Length > 0 && strText.Trim() != "")
				{
					string strNewVal = "";
					for (int i = 0; i < strValue.Length; i++)
						strNewVal += SeparatorChar + strValue[i];
					strNewVal = strNewVal.Substring(1);
					AddItem(strNewVal, strText);
				}
			}
			public	virtual	void			AddItem(string[]	strValue,	string strText, string strIcon)
			{
				if (strValue != null && strValue.Length > 0 && strText.Trim() != "")
				{
					string strNewVal = "";
					for (int i = 0; i < strValue.Length; i++)
						strNewVal += SeparatorChar + strValue[i];
					strNewVal = strNewVal.Substring(1);
					AddItem(strNewVal, strText, strIcon);
				}
			}
			public	virtual	void			AddItem(string[]	strValue,	string strText, string strIcon, string	strSub)
			{
				if (strValue != null && strValue.Length > 0 && strText.Trim() != "")
				{
					string strNewVal = "";
					for (int i = 0; i < strValue.Length; i++)
						strNewVal += SeparatorChar + strValue[i];
					strNewVal = strNewVal.Substring(1);
					AddItem(strNewVal, strText, strIcon, strSub);
				}
			}
			public	virtual	void			AddItem(string[]	strValue,	string strText, string strIcon, int			intSub)
			{
				if (strValue != null && strValue.Length > 0 && strText.Trim() != "")
				{
					string strNewVal = "";
					for (int i = 0; i < strValue.Length; i++)
						strNewVal += SeparatorChar + strValue[i];
					strNewVal = strNewVal.Substring(1);
					AddItem(strNewVal, strText, strIcon, intSub.ToString());
				}
			}
			public	virtual	void			AddItem(string[]	strValue,	string strText, string strIcon, float		fSub)
			{
				if (strValue != null && strValue.Length > 0 && strText.Trim() != "")
				{
					string strNewVal = "";
					for (int i = 0; i < strValue.Length; i++)
						strNewVal += SeparatorChar + strValue[i];
					strNewVal = strNewVal.Substring(1);
					AddItem(strNewVal, strText, strIcon, fSub.ToString());
				}
			}
			public	virtual	void			AddItem(string[]	strValue,	string strText, Sprite sprIcon)
			{
				if (strValue != null && strValue.Length > 0 && strText.Trim() != "")
				{
					string strNewVal = "";
					for (int i = 0; i < strValue.Length; i++)
						strNewVal += SeparatorChar + strValue[i];
					strNewVal = strNewVal.Substring(1);
					AddItem(strNewVal, strText, sprIcon);
				}
			}
			public	virtual	void			AddItem(string[]	strValue,	string strText, Sprite sprIcon, string	strSub)
			{
				if (strValue != null && strValue.Length > 0 && strText.Trim() != "")
				{
					string strNewVal = "";
					for (int i = 0; i < strValue.Length; i++)
						strNewVal += SeparatorChar + strValue[i];
					strNewVal = strNewVal.Substring(1);
					AddItem(strNewVal, strText, sprIcon, strSub);
				}
			}
			public	virtual	void			AddItem(string[]	strValue,	string strText, Sprite sprIcon, int			intSub)
			{
				if (strValue != null && strValue.Length > 0 && strText.Trim() != "")
				{
					string strNewVal = "";
					for (int i = 0; i < strValue.Length; i++)
						strNewVal += SeparatorChar + strValue[i];
					strNewVal = strNewVal.Substring(1);
					AddItem(strNewVal, strText, sprIcon, intSub.ToString());
				}
			}
			public	virtual	void			AddItem(string[]	strValue,	string strText, Sprite sprIcon, float		fSub)
			{
				if (strValue != null && strValue.Length > 0 && strText.Trim() != "")
				{
					string strNewVal = "";
					for (int i = 0; i < strValue.Length; i++)
						strNewVal += SeparatorChar + strValue[i];
					strNewVal = strNewVal.Substring(1);
					AddItem(strNewVal, strText, sprIcon, fSub.ToString());
				}
			}

			public	virtual	void			AddItem(int				intValue,	string strText)
			{
				AddItem(intValue.ToString(), strText);
			}
			public	virtual	void			AddItem(int				intValue,	string strText, string strIcon)
			{
				AddItem(intValue.ToString(), strText, strIcon);
			}
			public	virtual	void			AddItem(int				intValue,	string strText, string strIcon, string	strSub)
			{
				AddItem(intValue.ToString(), strText, strIcon, strSub);
			}
			public	virtual	void			AddItem(int				intValue,	string strText, string strIcon, int			intSub)
			{
				AddItem(intValue.ToString(), strText, strIcon, intSub.ToString());
			}
			public	virtual	void			AddItem(int				intValue,	string strText, string strIcon, float		fSub)
			{
				AddItem(intValue.ToString(), strText, strIcon, fSub.ToString());
			}
			public	virtual	void			AddItem(int				intValue,	string strText, Sprite sprIcon)
			{
				AddItem(intValue.ToString(), strText, sprIcon);
			}
			public	virtual	void			AddItem(int				intValue,	string strText, Sprite sprIcon, string	strSub)
			{
				AddItem(intValue.ToString(), strText, sprIcon, strSub);
			}
			public	virtual	void			AddItem(int				intValue,	string strText, Sprite sprIcon, int			intSub)
			{
				AddItem(intValue.ToString(), strText, sprIcon, intSub.ToString());
			}
			public	virtual	void			AddItem(int				intValue,	string strText, Sprite sprIcon, float		fSub)
			{
				AddItem(intValue.ToString(), strText, sprIcon, fSub.ToString());
			}

			// -- REMOVE ITEM FROM LISTBOX
			public	virtual	void			RemoveItemByIndex(int			intIndex)
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

				_intItemCount--;
				_intSelectedItem = -1;
				_intSelectedList = new List<int>();
				ResizeContainer();
			}
			public	virtual	void			RemoveItemByValue(string	strValue)
			{
				int i = Items.FindIndex(x => x.Value.ToLower() == strValue.ToLower());
				if (i >= 0)
					RemoveItemByIndex(i);
			}
			public	virtual	void			RemoveItemByText(	string	strText)
			{
				int i = Items.FindIndex(x => x.Text.ToLower() == strText.ToLower());
				if (i >= 0)
					RemoveItemByIndex(i);
			}

			// -- SORT LISTBOX ITEMS
			public	virtual	void			Sort()
			{
				SortByText();
			}
			public	virtual	void			SortByText()
			{
				Items.Sort((p1, p2) => p1.Text.CompareTo(p2.Text));
				for (int i = 0; i < Items.Count; i++)
				{ 
					Items[i].Index = i;
					Items[i].AutoSize();
				}
			}
			public	virtual	void			SortByValue()
			{
				Items.Sort((p1, p2) => p1.Value.CompareTo(p2.Value));
				for (int i = 0; i < Items.Count; i++)
				{ 
					Items[i].Index = i;
					Items[i].AutoSize();
				}
			}
			public	virtual	void			SortBySubText()
			{
				Items.Sort((p1, p2) => p1.SubText.CompareTo(p2.SubText));
				for (int i = 0; i < Items.Count; i++)
				{ 
					Items[i].Index = i;
					Items[i].AutoSize();
				}
			}

			// -- SET LISTBOX SCROLLBAR POSITION
			public	virtual	void			SetToTop()
			{
				if (gameObject.activeInHierarchy)
						StartCoroutine(SetScroll(1));
			}
			public	virtual	void			SetToBottom()
			{
				if (gameObject.activeInHierarchy)
						StartCoroutine(SetScroll(0));
			}
			public	virtual	void			SetToIndex(int intIndex)
			{			
				// SET THE SCROLLBAR TO MAKE THE SELECTED INDEX (intIndex) VISIBLE IN THE SCROLL CONTAINER
				float c = ContainerRect.rect.height;											// THE HEIGHT OF THE LISTBOX CONTAINER (VISIBLE TO THE USER)
				float h = Height + Spacing;																// THE HEIGHT OF AN INDIVIDUAL LIST ITEM
				float t = ((((float) _items.Count - 2) * h) + Spacing);		// THE TOTAL HEIGHT OF CONTAINER OF ALL LIST ITEMS
				float p = (((float) intIndex) * h);												// THE Y-POS OF AN INDIVIDUAL LIST ITEM
				float f = 1.00f;
				if (p >= t - c  )
					f = 1.00f - (p / t);
				else
					f = 1.00f - (p / (t - c));

				if (intIndex < 1)
					f = 1;
				if (gameObject.activeInHierarchy)
						StartCoroutine(SetScroll(f));
			}

			// -- CHECK FOR LISTBOX ITEM WITH VALUE
			public	virtual	bool			HasItemWithValue(string strValue)
			{
				return Items.FindIndex(x => x.Value.Trim().ToLower() == strValue.Trim().ToLower()) >= 0;
			}
			public	virtual	bool			HasItemWithValue(int		intValue)
			{
				return HasItemWithValue(intValue.ToString());
			}
			public	virtual	bool			HasItemWithValue(float	fValue)
			{
				return HasItemWithValue(fValue.ToString());
			}

			// -- ENABLE ONCLICK FOR LISTBOX ITEM (ALSO ADJUSTS ITEM STYLE)
			public	virtual	void			EnableByIndex(int				intIndex)
			{
				if (intIndex >= 0 && intIndex < Items.Count)
					Items[intIndex].Enabled = true;
			}
			public	virtual	void			EnableByValue(string		strValue)
			{
				EnableByIndex(Items.FindIndex(x => x.Value.ToLower() == strValue.ToLower()));
			}
			public	virtual	void			EnableByValue(int				intValue)
			{
				EnableByIndex(Items.FindIndex(x => x.Value.ToLower() == intValue.ToString().ToLower()));
			}
			public	virtual	void			EnableByText(string			strText)
			{
				EnableByIndex(Items.FindIndex(x => x.Text.ToLower() == strText.ToLower()));
			}

			// -- DISABLE ONCLICK FOR LISTBOX ITEM (ALSO ADJUSTS ITEM STYLE)
			public	virtual	void			DisableByIndex(int			intIndex)
			{
				if (intIndex >= 0 && intIndex < Items.Count)
					Items[intIndex].Enabled = false;
			}
			public	virtual	void			DisableByValue(string		strValue)
			{
				DisableByIndex(Items.FindIndex(x => x.Value.ToLower() == strValue.ToLower()));
			}
			public	virtual	void			DisableByValue(int			intValue)
			{
				DisableByIndex(Items.FindIndex(x => x.Value.ToLower() == intValue.ToString().ToLower()));
			}
			public	virtual	void			DisableByText(string		strText)
			{
				DisableByIndex(Items.FindIndex(x => x.Text.ToLower() == strText.ToLower()));
			}

			// -- SET LISTBOX ITEM TEXT
			public	virtual	void			SetItemTextByIndex(int		intIndex, string strNewText)
			{
				Items[intIndex].Text = strNewText;
			}
			public	virtual	void			SetItemTextByValue(string strValue, string strNewText)
			{
				int i = Items.FindIndex(x => x.Value == strValue);
				if (i >= 0)
					SetItemTextByIndex(i, strNewText);
			}
			public	virtual	void			SetItemTextByValue(int		intValue, string strNewText)
			{
				SetItemTextByValue(intValue.ToString(), strNewText);
			}

			// -- SET LISTBOX ITEM SUBTEXT
			public	virtual	void			SetItemSubTextByIndex(int		intIndex, string strNewText)
			{
				Items[intIndex].SubText = strNewText;
			}
			public	virtual	void			SetItemSubTextByValue(string strValue, string strNewText)
			{
				int i = Items.FindIndex(x => x.Value == strValue);
				if (i >= 0)
					SetItemSubTextByIndex(i, strNewText);
			}
			public	virtual	void			SetItemSubTextByValue(int		intValue, string strNewText)
			{
				SetItemSubTextByValue(intValue.ToString(), strNewText);
			}

			// -- CHANGE ITEM ORDER
			public	virtual	bool			MoveItemUp(		int				intIndex)
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
			public	virtual	bool			MoveItemDown(	int				intIndex)
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

			// -- GET LISTBOX ITEM VALUE
			public	virtual	string		GetValueByText(string		strText)
			{
				int i = Items.FindIndex(x => x.Text.ToLower() == strText.Trim().ToLower());
				if (i < 0)
					return "";
				else
					return Items[i].Value;
			}
			public	virtual	string		GetValueByIndex(int			intIndex)
			{
				if (intIndex < 0 || intIndex >= Items.Count)
					return "";
				return Items[intIndex].Value;
			}
			public	virtual	int				GetIntValueByIndex(int	intIndex)
			{
				if (intIndex < 0 || intIndex >= Items.Count)
					return -1;
				return Util.ConvertToInt(Items[intIndex].Value);
			}

			// -- GET LISTBOX ITEM TEXT
			public	virtual	string		GetTextByValue(string		strvalue)
			{
				int i = Items.FindIndex(x => x.Value.ToLower() == strvalue.Trim().ToLower());
				if (i < 0)
					return "";
				else
					return Items[i].Text;
			}
			public	virtual	string		GetTextByValue(int			intValue)
			{
				return GetTextByValue(intValue.ToString());
			}
			public	virtual	string		GetTextByValue(float		fValue)
			{
				return GetTextByValue(fValue.ToString());
			}
			public	virtual	string		GetTextByIndex(int			intIndex)
			{
				if (intIndex < 0 || intIndex >= Items.Count)
					return "";
				return Items[intIndex].Text;
			}

			// -- GET LISTBOX ITEM SUBTEXT
			public	virtual	string		GetSubTextByValue(string		strvalue)
			{
				int i = Items.FindIndex(x => x.Value.ToLower() == strvalue.Trim().ToLower());
				if (i < 0)
					return "";
				else
					return Items[i].SubText;
			}
			public	virtual	string		GetSubTextByValue(int			intValue)
			{
				return GetSubTextByValue(intValue.ToString());
			}
			public	virtual	string		GetSubTextByValue(float		fValue)
			{
				return GetSubTextByValue(fValue.ToString());
			}
			public	virtual	string		GetSubTextByIndex(int			intIndex)
			{
				if (intIndex < 0 || intIndex >= Items.Count)
					return "";
				return Items[intIndex].SubText;
			}

			// -- HANDLE SELECTION (SET LISTBOX ITEM SELECTED)
			public	virtual	void			SelectByIndex(int				intIndex, bool blnShifted = false, bool blnCtrled = false)
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
				if (_intSelectedItem >= -1)
				{
					if (this.OnChange != null) 
						OnChange(this.gameObject, _intSelectedItem);
				}
			}
			public	virtual	void			SelectByValue(string		strValue)
			{
				int i = Items.FindIndex(x => x.Value.ToLower() == strValue.ToLower());
				SelectByIndex(i);
			}
			public	virtual	void			SelectByText(	string		strText)
			{
				int i = Items.FindIndex(x => x.Text.ToLower() == strText.ToLower());
				SelectByIndex(i);
			}
			public	virtual	void			Unselect()
			{
				UnSelectAllItems();
				_intSelectedItem = -1;
				_intSelectedList = new List<int>();
			}
			public	virtual	void			HandleDoubleClick(int intIndex)
			{
				// DATA INTEGRITY CHECK
				if (!AllowDoubleClick)
					return;
				if (intIndex < -1 || intIndex >= Items.Count)
					return;

				// SELECT THE ITEM
				UnSelectAllItems();
				_intSelectedItem = intIndex;
				if (_intSelectedItem >= 0 && Items[_intSelectedItem].Enabled)
				{
					Items[_intSelectedItem].Select();
					_intSelectedList.Add(intIndex);
				}

				// PASS THE DOUBLE-CLICK EVENT TO THE ONDOUBLECLICK EVeNT
				if (_intSelectedItem >= 0)
				{
					if (this.OnDoubleClick != null)
						OnDoubleClick(this.gameObject, _intSelectedItem);
				}
			}

			// -- HANDLE SELECTED INDEXES
			public	virtual	bool			IsSelectedByIndex(int intIndex)
			{
				return (_intSelectedItem == intIndex || _intSelectedList.FindIndex(x => x == intIndex) >= 0);
			}
		
			// -- RESIZE THE CONTAINER (IF NECESSARY)
			public	virtual	void			UpdateListBoxContainerSize()
			{
				Vector2 v2 = ContainerRect.sizeDelta;
				v2.y = ((this.Height + this.Spacing) * Items.Count) + this.Spacing;
				ContainerRect.sizeDelta = v2;
				ResizeContainer();
			}

			// -- SHOW/HIDE THE LISTBOX CONTROL
			public	virtual	void			Hide()
			{
				gameObject.SetActive(true);
				if (ListBoxMode == ListBoxModes.ListBox)
				{
					GetComponent<Image>().enabled = false;			
					if (ScrollBarObject != null)
							ScrollBarObject.SetActive(false);
					if (ScrollRectObject != null)
							ScrollRectObject.SetActive(false);
					if (ListBoxTitle != null)
							ListBoxTitle.gameObject.SetActive(false);
				}
			}
			public	virtual	void			Show()
			{
				gameObject.SetActive(true);
				if (ListBoxMode == ListBoxModes.ListBox)
				{
					GetComponent<Image>().enabled = true;			
					if (ScrollBarObject != null)
							ScrollBarObject.SetActive(true);
					if (ScrollRectObject != null)
							ScrollRectObject.SetActive(true);
					if (ListBoxTitle != null)
							ListBoxTitle.gameObject.SetActive(true);
				}
			}
			public	virtual	bool			IsShown
			{
				get
				{
					if (ListBoxMode == ListBoxModes.ListBox)
					{
						return GetComponent<Image>().enabled && ScrollBarObject.activeSelf && ScrollRectObject.activeSelf; 
					}
					return false;
				}
			}

		#endregion

	#endregion

	#region "EVENT FUNCTIONS"

		public	event						OnListBoxSelectChanged		OnChange;
		public	event						OnListBoxDoubleClick			OnDoubleClick;
								
	#endregion

}
