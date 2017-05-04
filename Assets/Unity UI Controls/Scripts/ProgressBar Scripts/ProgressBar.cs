// ===========================================================================================================
//
// Class/Library: ProgressBar Control
//        Author: Michael Marzilli   ( http://www.linkedin.com/in/michaelmarzilli , http://www.develteam.com/Developer/Rowell/Portfolio )
//       Created: Aug 10, 2016
//	
// VERS 1.0.000 : Aug 10, 2016 : Original File Created. Released for Unity 3D.
//
// ===========================================================================================================

#if UNITY_EDITOR
	#define		IS_DEBUGGING
#else
	#undef		IS_DEBUGGING
#endif

using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour 
{

	#region "PRIVATE VARIABLES"

		private GameObject			_goProgressBar				= null;
		private GameObject			_goProgressContainer	= null;
		private GameObject			_goProgressText				= null;
		private GameObject			_goProgressCaption		= null;

		[SerializeField]
		private string					_strCaption						= "";
		[SerializeField]
		private float						_fCurValue						= 0.00f;
		[SerializeField]
		private float						_fMaxValue						= 100.0f;
		[SerializeField]
		private float						_fWidth								= 0.00f;
		[SerializeField]
		private Color						_colTextColor					= Color.white;
		[SerializeField]
		private Color						_colTextShadow				= Color.black;
		[SerializeField]
		private Color						_colBarColor					= Color.cyan;
		[SerializeField]
		private bool						_blnDisplayAsPercent	= true;

		private bool						_blnIsInitialized			= false;

	#endregion

	#region "PRIVATE PROPERTIES"

		private GameObject			ProgressBarLine
		{
			get
			{
				if (_goProgressBar == null)
						_goProgressBar = gameObject.transform.GetChild(1).GetChild(0).gameObject;
				return _goProgressBar;
			}
		}
		private GameObject			ProgressContainer
		{
			get
			{
				if (_goProgressContainer == null)
						_goProgressContainer = gameObject.transform.GetChild(1).gameObject;
				return _goProgressContainer;
			}
		}
		private GameObject			ProgressText
		{
			get
			{
				if (_goProgressText == null)
						_goProgressText = gameObject.transform.GetChild(2).gameObject;
				return _goProgressText;
			}
		}
		private GameObject			ProgressCaption
		{
			get
			{
				if (_goProgressCaption == null)
						_goProgressCaption = gameObject.transform.GetChild(3).gameObject;
				return _goProgressCaption;
			}
		}

		private float						ProgressWidth
		{
			get
			{
				if (_fWidth <= 0.00f)
						_fWidth = ProgressContainer.GetComponent<RectTransform>().rect.width;
				return _fWidth;
			}
		}

	#endregion

	#region "PUBLIC PROPERTIES"

		public	string					Caption
		{
			set
			{
				_strCaption = value.Trim();
				ProgressCaption.GetComponent<Text>().text = _strCaption;
			}
		}
		public	float						Progress
		{
			get
			{
				return _fCurValue;
			}
			set
			{
				_fCurValue = value;
				if (_fCurValue > _fMaxValue)
						_fCurValue = _fMaxValue;
				float		f = Mathf.Clamp((_fCurValue / _fMaxValue), 0.00f, 1.00f);
				Vector2 v = ProgressBarLine.GetComponent<RectTransform>().sizeDelta;
				v.x = ProgressWidth * f;
				ProgressBarLine.GetComponent<RectTransform>().sizeDelta = v;
				if (_blnDisplayAsPercent)
					ProgressText.GetComponent<Text>().text = (f * 100).ToString("##0") + "%";
				else
					ProgressText.GetComponent<Text>().text = _fCurValue.ToString() + " / " + _fMaxValue.ToString();
			}
		}
		public	float						CurValue
		{
			get
			{
				return Progress;
			}
			set
			{
				Progress = value;
			}
		}
		public	float						MaxValue
		{
			get
			{
				return _fMaxValue;
			}
			set
			{
				_fMaxValue = value;
				Progress = _fCurValue;
			}
		}

		public	bool						DisplayAsPercent
		{
			get
			{
				return _blnDisplayAsPercent;
			}
			set
			{
				_blnDisplayAsPercent = value;
			}
		}
		public	Color						TextColor
		{
			get
			{
				return _colTextColor;
			}
			set
			{
				_colTextColor = value;
				ProgressText.GetComponent<Text>().color = value;
			}
		}
		public	Color						TextShadow
		{
			get
			{
				return _colTextShadow;
			}
			set
			{
				_colTextShadow = value;
				ProgressText.GetComponent<Shadow>().effectColor = value;
			}
		}
		public	Color						ProgressBarColor
		{
			get
			{
				return _colBarColor;
			}
			set
			{
				_colBarColor = value;
				ProgressBarLine.GetComponent<Image>().color = value;
			}
		}

	#endregion
	
	#region "START FUNCTIONS"

		private void						Awake()
		{
			_fCurValue	= 0.00f;
			Progress		= _fCurValue;
			Caption			= "";
		}
		private void						Start()
		{
			ProgressBarColor	= _colBarColor;
			TextColor					= _colTextColor;
			TextShadow				= _colTextShadow;
			Progress					= _fCurValue;
			_blnIsInitialized = true;
		}
		private void						OnEnable()
		{
			if (!_blnIsInitialized)
				Start();
			else 
				Progress = _fCurValue;
		}

	#endregion

	#region "PUBLIC FUNCTIONS"

		public	void						Reset()
		{
			_fCurValue	= 0.00f;
			Progress		= _fCurValue;
			Caption			= "";
		}
		public	void						SetProgress(float fCurrent, float fMaximum)
		{
			_fMaxValue	= fMaximum;
			Progress		= fCurrent;
		}

	#endregion
	
}
