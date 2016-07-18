using UnityEngine;
using UnityEngine.UI;

using System.Collections;

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
		private float						_fValue								= 0.00f;
		[SerializeField]
		private float						_fWidth								= 0.00f;
		[SerializeField]
		private Color						_colTextColor					= Color.white;
		[SerializeField]
		private Color						_colTextShadow				= Color.black;
		[SerializeField]
		private Color						_colBarColor;

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
				return _fValue;
			}
			set
			{
				_fValue = value;
				if (_fValue > 1.00f)
						_fValue = Mathf.Clamp((_fValue / 100.00f), 0.00f, 1.00f);
				Vector2 v = ProgressBarLine.GetComponent<RectTransform>().sizeDelta;
				v.x = ProgressWidth * _fValue;
				ProgressBarLine.GetComponent<RectTransform>().sizeDelta = v;
				ProgressText.GetComponent<Text>().text = (_fValue * 100).ToString("##0") + "%";
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
			_fValue		= 0.00f;
			Progress	= _fValue;
			Caption		= "";
		}

	#endregion

	#region "PUBLIC FUNCTIONS"

		public	void						Reset()
		{
			_fValue		= 0.00f;
			Progress	= _fValue;
			Caption		= "";
		}
		public	void						SetProgress(float fCurrent, float fMaximum)
		{
			_fValue		= Mathf.Clamp(fCurrent / fMaximum, 0f, 1f);
			Progress	= _fValue;
		}

	#endregion
	
}
