// ===========================================================================================================
//
// Class/Library: ListBox Control - Line Item
//        Author: Michael Marzilli   ( http://www.linkedin.com/in/michaelmarzilli , http://www.develteam.com/Developer/Rowell/Portfolio )
//       Created: Jun 10, 2016
//	
// VERS 1.0.000 : Jun 10, 2016 :	Original File Created. Released for Unity 3D.
//			1.0.001	:	Jun 11, 2016 :	Added a SubText field/element to the ListBox Control.
//																The SubText field is a right justified field that can add additional information.
//																Such as displaying a price for an item in and item list for a shop.
//			1.0.002 : Aug 03, 2016 :	Added the ability to Double-Tap a ListItem in order to select it (fire an OnPointerClick style event).
//
// ===========================================================================================================

#if UNITY_EDITOR
#define		IS_DEBUGGING
#else
#undef		IS_DEBUGGING
#endif

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public	partial	class	ListBoxLineItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{

	#region "PRIVATE CONSTANTS"

		private const float				ELEMENT_SPACING = 3;

	#endregion

	#region "PRIVATE VARIABLES"

		private ListBoxControl		_lbControl		= null;
		private RectTransform			_rt						= null;
		private RectTransform			_rtMainTxt		= null;
		private RectTransform			_rtSubTxt			= null;
		private RectTransform			_rtImg				= null;
		private Image							_img					= null;
		private	Text							_txtMainText	= null;
		private Text							_txtSubText		= null;
		private Image							_imgIcon			= null;

		private	Color							_itemNormalColor;
		private	Color							_itemHighlightColor;
		private	Color							_itemSelectedColor;
		private Color							_itemDisabledColor;

		private	int								_intIndex				= 0;

		private	string						_strValue				= "";
		private string						_strMainText		= "";
		private string						_strSubText			= "";

		private float							_fXpos					=  2;
		private float							_fYpos					= -2;
		private float							_fWidth					= 0;
		private float							_fHeight				= 0;
		private float							_fSpace					= 2;
		private bool							_blnSelected		= false;
		private bool							_blnEnabled			= true;
		private bool							_blnShown				= true;
		private bool							_blnInitialized	= false;

	#endregion

	#region "PRIVATE PROPERTIES"

		private	ListBoxControl		LBcontrol
		{
			get
			{
				if (_lbControl == null)
				{
					if (ListBoxControlObject != null && ListBoxControlObject.GetComponent<ListBoxControl>() != null)
						_lbControl = ListBoxControlObject.GetComponent<ListBoxControl>();
				}
				return _lbControl;
			}
		}
		private Image							DisplayedIcon
		{
			get
			{
				if (_imgIcon == null)
					if (transform.GetChild(0).gameObject != null && transform.GetChild(0).GetComponent<Image>() != null)
					{ 
						_imgIcon	= transform.GetChild(0).GetComponent<Image>();
						_rtImg		= _imgIcon.GetComponent<RectTransform>();
					}
				return _imgIcon;
			}
		}
		private Text							DisplayedMainText
		{
			get
			{
				if (_txtMainText == null)
					if (transform.GetChild(1).gameObject != null && transform.GetChild(1).GetComponent<Text>() != null)
					{ 
						_txtMainText	= transform.GetChild(1).GetComponent<Text>();
						_rtMainTxt		= _txtMainText.GetComponent<RectTransform>();
					}
				return _txtMainText;
			}
		}
		private Text							DisplayedSubText
		{
			get
			{
				if (_txtSubText == null)
					if (transform.GetChild(2).gameObject != null && transform.GetChild(2).GetComponent<Text>() != null)
					{
						_txtSubText = transform.GetChild(2).GetComponent<Text>();
						_rtSubTxt	= _txtSubText.GetComponent<RectTransform>();
					}
				return _txtSubText;
			}
		}

		private RectTransform			ImageRT
		{
			get
			{
				if (_rtImg == null)
						_rtImg = DisplayedIcon.GetComponent<RectTransform>();
				return _rtImg;
			}
		}
		private RectTransform			MainTextRT
		{
			get
			{
				if (_rtMainTxt == null)
						_rtMainTxt = DisplayedMainText.GetComponent<RectTransform>();
				return _rtMainTxt;
			}
		}
		private RectTransform			SubTextRT
		{
			get
			{
				if (_rtSubTxt == null)
						_rtSubTxt = DisplayedSubText.GetComponent<RectTransform>();
				return _rtSubTxt;
			}
		}
		private bool							HasIcon
		{
			get
			{
				return DisplayedIcon.sprite != null;
			}
		}

	#endregion

	#region "PUBLIC PROPERTIES"

		// THE LISTBOX GAMEOBJECT THAT OWNS THIS LINE ITEM
		public	GameObject				ListBoxControlObject;

		public	int								Index
		{ 
			get
			{
				return _intIndex;
			}
			set
			{
				_intIndex = value;
			}
		}

		public	string						Value
		{
			get
			{
				return _strValue;
			}
			set
			{
				_strValue = value.Trim();
			}
		}
		public	int								IntValue
		{
			get
			{
				return Util.ConvertToInt(_strValue);
			}
			set
			{
				_strValue = value.ToString();
			}
		}
		public	string						Text
		{
			get
			{
				return _strMainText;
			}
			set
			{
				_strMainText = value.Trim();
				DisplayedMainText.text = _strMainText;
				this.gameObject.name = _strMainText;
				UpdateContent();
			}
		}
		public	string						SubText
		{
			get
			{
				return _strSubText;
			}
			set
			{
				_strSubText = value.Trim();
				DisplayedSubText.text	= _strSubText;
				UpdateContent();
			}
		}
		public	void							SetIcon(Sprite sprImage)
		{
			DisplayedIcon.sprite = sprImage;
			UpdateContent();
		}
		public	void							SetIcon(string strImagePath)
		{
			DisplayedIcon.sprite = (Sprite)Resources.Load<Sprite>(strImagePath);
			UpdateContent();
		}

		public	float							X
		{
			get
			{
				return _fXpos;
			}
			set
			{
				_fXpos = value;
				UpdatePosition();
			}
		}
		public	float							Y
		{
			get
			{
				return _fYpos;
			}
			set
			{
				_fYpos = value;
				UpdatePosition();
			}
		}
		public	float							Width
		{
			get
			{
				return _fWidth;
			}
			set
			{
				_fWidth = value;
				UpdatePosition();
			}
		}
		public	float							Height
		{
			get
			{
				return _fHeight;
			}
			set
			{
				_fHeight = value;
				UpdatePosition();
			}
		}
		public	float							Spacing
		{
			get
			{
				return _fSpace;
			}
			set
			{
				_fSpace = value;
			}
		}

		public	Color							ItemNormalColor
		{
			get
			{
				return _itemNormalColor;
			}
			set 
			{
				_itemNormalColor = value;
			}
		}
		public	Color							ItemHighlightColor
		{
			get
			{
				return _itemHighlightColor;
			}
			set
			{
				_itemHighlightColor = value;
			}
		}
		public	Color							ItemSelectedColor
		{
			get
			{
				return _itemSelectedColor;
			}
			set
			{
				_itemSelectedColor = value;
			}
		}
		public	Color							ItemDisabledColor
		{
			get
			{
				return _itemDisabledColor;
			}
			set
			{
				_itemDisabledColor = value;
				UpdateContent();
			}
		}

		public	bool							Selected
		{
			get
			{
				return _blnSelected;
			}
			set
			{
				_blnSelected = value;
			}
		}
		public	bool							Enabled
		{
			get
			{
				return _blnEnabled;
			}
			set
			{
				_blnEnabled = value;
				UpdateContent();
			}
		}
		public	bool							Shown
		{
			get
			{
				return _blnShown;
			}
			set
			{
				_blnShown = value;
				UpdateContent();
			}
		}

	#endregion

	#region "PRIVATE FUNCTIONS"

		private		void						Awake()
		{
			// INITIALIZE INTERNAL VARIABLES
			_rt							= this.GetComponent<RectTransform>();
			_img						= this.GetComponent<Image>();
			_fWidth					= _rt.sizeDelta.x;
			_fHeight				= _rt.sizeDelta.y;
	    _rt.localScale	= Vector3.one;
		}
		protected	void						UpdatePosition()
		{
			if (!Application.isPlaying)
				return;

			_rt.localPosition	= new Vector3(_fXpos,		_fYpos, 0);
			_rt.sizeDelta			= new Vector2(_fWidth,	_fHeight);
	    _rt.localScale		= Vector3.one;
			transform.localScale	= Vector3.one;
		}
		protected void						UpdateContent()
		{
			if (!Application.isPlaying)
				return;

			Vector2 v2 = Vector2.zero;
			if (this.HasIcon)
			{
				// SHOW THE ICON IMAGE
				DisplayedIcon.gameObject.SetActive(true);

				// SET ICON SIZE
				v2.y = _fHeight - (ELEMENT_SPACING * 2);
				v2.x = v2.y;
				ImageRT.sizeDelta = v2;

				// SET ICON POSITION
				v2.x = ELEMENT_SPACING;
				v2.y = -(_fHeight / 2);
				ImageRT.localPosition = v2;

				// SET TEXT SIZE - TAKE INTO ACCOUNT SUB TEXT
				v2.y = _fHeight - (ELEMENT_SPACING * 2);
				v2.x = _fWidth  - (ImageRT.sizeDelta.x + (ELEMENT_SPACING * 3)) - ((_strSubText == "") ? 0 : 50);
				MainTextRT.sizeDelta = v2;

				// SET TEXT POSITION
				v2.x = (ImageRT.sizeDelta.x + (ELEMENT_SPACING * 2));
				v2.y = -(_fHeight / 2);
				MainTextRT.localPosition = v2;

			} else {
				// HIDE THE ICON IMAGE
				DisplayedIcon.gameObject.SetActive(false);

				// SET TEXT SIZE - TAKE INTO ACCOUNT SUB TEXT
				v2.y = _fHeight - (ELEMENT_SPACING * 2);
				v2.x = _fWidth  - (ELEMENT_SPACING * 2) - ((_strSubText == "") ? 0 : 50);
				MainTextRT.sizeDelta = v2;

				// SET TEXT POSITION
				v2.x =  (ELEMENT_SPACING);
				v2.y = -(_fHeight / 2);
				MainTextRT.localPosition = v2;
			}

			// SET SUBTEXT SIZE
			if (_strSubText != "")
			{
				v2 = SubTextRT.sizeDelta;
				v2.y = _fHeight - (ELEMENT_SPACING * 2);
				SubTextRT.sizeDelta = v2;
			}

			// COLORING
			if (!this.Enabled)
				_img.color = ItemDisabledColor;
			else if (this.Selected)
				_img.color = ItemSelectedColor;
			else
				_img.color = ItemNormalColor;

			// EXECUTE CUSTOM DISPLAY METHOD
			Display();
		}

	#endregion

	#region "PUBLIC FUNCTIONS"

		public	void							AutoSize()
		{
			_fXpos = _fSpace;
			_fYpos = (((_fHeight + _fSpace) * _intIndex) + _fSpace) * (-1);
			UpdatePosition();
		}
		public	void							Destroy()
		{
			if (Application.isPlaying)
				DestroyObject(this.gameObject, 0.01f);
			else
				DestroyImmediate(this.gameObject);
		}
		public	void							Select()
		{
			if (this.Enabled)
			{ 
				Selected		= true;
				_img.color	= ItemSelectedColor;
			}
		}
		public	void							UnSelect()
		{
			if (this.Enabled)
			{ 
				Selected		= false;
				_img.color	= ItemNormalColor;
			}
		}
		public	virtual	void			Display()
		{
		}

	#endregion

	#region "EVENT FUNCTIONS"

		public	void							OnPointerEnter(	PointerEventData eventData)
		{
			if (this.Enabled)
				_img.color = ItemHighlightColor;
			else
				_img.color = ItemDisabledColor;
		}
		public	void							OnPointerExit(	PointerEventData eventData)
		{
			if (!this.Enabled)
				_img.color = ItemDisabledColor;
			else if (LBcontrol.IsSelectedByIndex(this.Index))
				_img.color = ItemSelectedColor;
			else
				_img.color = ItemNormalColor;
		}
		public	void							OnPointerDown(	PointerEventData eventData)
		{
			// BUG IN UNITY MANDATES THAT, IN ORDER FOR OnPointerClick TO WORK, BOTH 
			// OnPointerDown AND OnPointerUp EVENTS MUST ALSO BE PRESENT.
			// SO WE LEAVE THIS EMPTY
		}
		public	void							OnPointerUp(		PointerEventData eventData)
		{
			// BUG IN UNITY MANDATES THAT, IN ORDER FOR OnPointerClick TO WORK, BOTH 
			// OnPointerDown AND OnPointerUp EVENTS MUST ALSO BE PRESENT.
			// SO WE LEAVE THIS EMPTY
		}
		public	void							OnPointerClick(	PointerEventData eventData)
		{
			if (!this.Enabled)
				return;
			bool blnShifted = Input.GetKey(KeyCode.LeftShift)		|| Input.GetKey(KeyCode.RightShift);
			bool blnCtrled	= Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) || LBcontrol.AutoMultiSelect;
			LBcontrol.SelectByIndex(this.Index, blnShifted, blnCtrled);
			if (eventData.clickCount > 1 || Input.touchCount > 1)
			{
				// FIRE DOUBLE-CLICK EVENT
				LBcontrol.HandleDoubleClick(this.Index);
			}
		}

	#endregion

}
