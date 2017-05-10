// ===========================================================================================================
//
// Class/Library: Utility Class
//        Author: Michael Marzilli   ( http://www.linkedin.com/in/michaelmarzilli , http://www.develteam.com/Developer/Rowell/Portfolio )
//       Created: Jun 10, 2016
//	
// VERS 1.0.000 : Jun 10, 2016 : Original File Created. Released for Unity 3D.
//
// ===========================================================================================================

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;
using System.IO;

public class Util : MonoBehaviour 
{

	#region "PRIVATE CONSTANTS"

		private const string					CURRENCY_SYMBOL	= "$";

	#endregion

	#region "DATA CHECKING FUNCTIONS"

		public	static string   StringCheck(string strInput = "")
		{
			if (strInput == null)
				return "";
			string strReturn	= Regex.Replace(strInput,		@"[^\u0000-\u007F]", string.Empty);
						 strReturn	= Regex.Replace(strReturn,	@"\\", @"\\");
						 strReturn	= Regex.Replace(strReturn,	@"--", "-");
			return strReturn;
		}
		public	static bool			IsInt(			string strInput)
		{
			strInput = StringCheck(strInput);
			try
			{
				int n;
				return int.TryParse(strInput, out n) && !strInput.Contains(",");
			} catch {
				return false;
			}
		}
		public	static bool			IsLong(			string strInput)
		{
			strInput = StringCheck(strInput);
			try
			{
				long n;
				return long.TryParse(strInput, out n) && !strInput.Contains(",");
			} catch {
				return false;
			}
		}
		public	static bool			IsFloat(		string strInput)
		{
			strInput = StringCheck(strInput);
			try
			{
				float n;
				return float.TryParse(strInput, out n) && !strInput.Contains(",");
			} catch {
				return false;
			}
		}
		public	static bool			IsDecimal(	string strInput)
		{
			strInput = StringCheck(strInput);
			try
			{
				double n;
				return double.TryParse(strInput, out n) && !strInput.Contains(",");
			} catch {
				return false;
			}
		}
		public	static bool			IsDate(string strInput)
		{
			strInput = StringCheck(strInput);
			try
			{
				System.DateTime temp;
				return System.DateTime.TryParse(strInput, out temp);
			} catch {
				return false;
			}
		}

		private static long			Fix(double Number)
		{
			float f = ConvertToFloat(Number.ToString());
			if (Number >= 0)
			{
				return (long)Mathf.Floor(f);
			}
			return (long)Mathf.Ceil(f);
		}
		
	#endregion

	#region "CONVERSION FUNCTIONS"

		public enum DateInterval { Millisecond, Second, Minute, Hour, Day, Weekday, Month, Year }

		public static System.DateTime	ConvertToDate(				string	strInput)
		{
			if (IsDate(strInput))
				return System.DateTime.Parse(strInput);
			else
				return System.DateTime.Parse("01/01/1900");
		}
		public static string					PlusMinus(						int			intInput)
		{
			string st = intInput.ToString();
			if (intInput >= 0)
				st = "+" + st;
			return st;
		}
		public static int							ConvertToInt(					string	strInput)
		{
			strInput = strInput.Replace(",", "");
			return (IsInt(strInput)) ? System.Convert.ToInt32(strInput) : 0;
		}
		public static int							ConvertToInt(					bool		blnInput)
		{
			return (blnInput) ? 1 : 0;
		}
		public static int							ConvertToInt(					float		dblInput)
		{
			return (int)Mathf.Ceil(dblInput);
		}
		public static int							ConvertToInt(					double	dblInput)
		{
			return (int)System.Math.Ceiling(dblInput);
		}
		public static long						ConvertToLong(				string	strInput)
		{
			return (IsLong(strInput)) ? System.Convert.ToInt64(strInput) : 0; 
		}
		public static float						ConvertToFloat(				string	strInput)
		{
			return (IsFloat(strInput)) ? System.Convert.ToSingle(strInput) : 0;
		}
		public static float						ConvertToFloat(				int			intInput)
		{
			return System.Convert.ToSingle(intInput);
		}
		public static float						ConvertToFloat(				double	dblInput)
		{
			return (float)  float.Parse(dblInput.ToString());
		}
		public static bool						ConvertToBoolean(			string	strInput)
		{
			return (strInput.Trim().ToLower().Trim() == "true" || strInput.Trim() == "1");
		}
		public static string					ConvertToMoneyString(	float		dblInput, bool blnShowSign = false)
		{
			return ((blnShowSign || dblInput < 0) ? ((dblInput < 0) ? "-" : "+") : "") + CURRENCY_SYMBOL + Mathf.Abs(dblInput).ToString("0.00");
		}
		public static string					ConvertToFloatString(	float		dblInput, bool blnShowSign = false)
		{
			return ((blnShowSign || dblInput < 0) ? ((dblInput < 0) ? "-" : "+") : "") + Mathf.Abs(dblInput).ToString("0.00");
		}
		public static string					ConvertToFloatString(	float		dblInput, bool blnAddSign, int intPlaces)
		{
			bool blnNegative = (dblInput < 0);
			dblInput = Mathf.Abs(dblInput);
			string strTem = "#,##0";
			if (intPlaces > 0)
				strTem += "." + ("".PadLeft(intPlaces, '0'));
			string strOutput = dblInput.ToString(strTem);

			if (dblInput > 0 && blnAddSign)
			{
				if (blnNegative)
					strOutput = "-" + strOutput;
				else
					strOutput = "+" + strOutput;
			} else if (blnNegative)
				strOutput = "-" + strOutput;

			return strOutput;
		}
		public static string					ConvertToFloatString(	string	strInput, bool blnAddSign) 
		{
			int intPlaces = 2;
			return ConvertToFloatString(strInput, blnAddSign, intPlaces); 
		}
		public static string					ConvertToFloatString(	string	strInput, bool blnAddSign, int intPlaces)
		{
			return ConvertToFloatString(ConvertToFloat(strInput), blnAddSign, intPlaces);
		}

		public static int							ConvertToFullPercent(	int intA, int intB)
		{
			float dblA = ConvertToFloat(intA);
			float dblB = ConvertToFloat(intB);
			float dblC = (dblA / dblB);
						dblC = dblC * 100;
			return ConvertToInt(dblC);
		}
		public	static	string				ConvertToHex(int i)
		{
			return string.Format("{0:X6}", i);
		}

		public static long						DateDiff(DateInterval interval, System.DateTime dateStart, System.DateTime dateEnd) 
		{
			System.TimeSpan ts = dateEnd - dateStart; 
			switch (interval) 
			{ 
				case DateInterval.Year:
					if (dateEnd.Month < dateStart.Month)
						return (dateEnd.Year - dateStart.Year) - 1; 
					else					
						return dateEnd.Year - dateStart.Year; 
				case DateInterval.Month:
					return (dateEnd.Month - dateStart.Month) + (12 * (dateEnd.Year - dateStart.Year));
				case DateInterval.Weekday:
					return Fix(ts.TotalDays) / 7;
				case DateInterval.Day:
					return Fix(ts.TotalDays);
				case DateInterval.Hour:
					return Fix(ts.TotalHours);
				case DateInterval.Minute:
					return Fix(ts.TotalMinutes);
				case DateInterval.Second:
					return Fix(ts.TotalSeconds);
				case DateInterval.Millisecond:
					return Fix(ts.TotalMilliseconds);
			}
			return 0;
		} 
		
		public static Vector2					ConvertToVector2(			string strInput)
		{
			Vector2 v2 = Vector2.zero;
			strInput = strInput.Replace("(", "").Replace(")", "");
			string[] strAr = strInput.Split(',');
			v2.x = ConvertToFloat(strAr[0].Trim());
			v2.y = ConvertToFloat(strAr[1].Trim());
			return v2;
		}
		public static Vector3					ConvertToVector3(			string strInput)
		{
			Vector3 v3 = Vector3.zero;
			strInput = strInput.Replace("(", "").Replace(")", "");
			string[] strAr = strInput.Split(',');
			v3.x = ConvertToFloat(strAr[0].Trim());
			v3.y = ConvertToFloat(strAr[1].Trim());
			v3.z = ConvertToFloat(strAr[2].Trim());
			return v3;
		}
		public static Vector4					ConvertToVector4(			string strInput)
		{
			Vector4 v4 = Vector4.zero;
			strInput = strInput.Replace("(", "").Replace(")", "");
			string[] strAr = strInput.Split(',');
			v4.x = ConvertToFloat(strAr[0].Trim());
			v4.y = ConvertToFloat(strAr[1].Trim());
			v4.z = ConvertToFloat(strAr[2].Trim());
			v4.w = ConvertToFloat(strAr[3].Trim());
			return v4;
		}
		public static Quaternion			ConvertToQuaternion(	string strInput)
		{
			Quaternion q3 = Quaternion.Euler(Vector3.zero);
			try 
			{
				strInput = strInput.Replace("(", "").Replace(")", "");
				string[] strAr = strInput.Split(',');
				q3.x = ConvertToFloat(strAr[0].Trim());
				q3.y = ConvertToFloat(strAr[1].Trim());
				q3.z = ConvertToFloat(strAr[2].Trim());
				q3.w = ConvertToFloat(strAr[3].Trim()); 
			} catch { Debug.LogError("Error in ConvertToQuaternion. " + strInput); }
			return q3;
		}
		public static Color						ConvertToColor(				string strInput)
		{
			string[] str = strInput.Split(","[0]);
			Color outp = Color.black;
			for (int i = 0; i < str.Length; i++) 
			{
				outp[i] = float.Parse(str[i]);
			}
			return outp;
     }
		public static string					ConvertSecondsToTime(	float	intInput, bool blnLongDisplay = false)
		{
			string st = "";
			intInput	= ConvertToFloat(Mathf.FloorToInt(intInput));
			int hr		= Mathf.FloorToInt( intInput / 3600);
			int min		= Mathf.FloorToInt((intInput % 3600) / 60);
			int sec		= Mathf.FloorToInt( intInput % 60);

			if (blnLongDisplay)
			{
				if (hr > 0)
					st = hr.ToString() + "hr";
				if (min > 0)
				{
					if (hr > 0)
						st += ", ";
					st += min.ToString() + "min";
				}
				if (sec > 0)
				{ 
					if (hr > 0 || min > 0)
						st += ", ";
					st += sec.ToString() + "sec";
				}
			} else {
				if (hr > 0)
					st = hr.ToString().PadLeft(2, '0') + ":";
				st += min.ToString().PadLeft(2, '0') + ":";
				st += sec.ToString().PadLeft(2, '0');
			}

			return st;
		}
		public static void						ConvertToTransform(		ref		Transform t, string strInput)
		{
			try
			{
				string[] s = strInput.Split('|');
				t.position = ConvertToVector3(s[0]);
				t.rotation = ConvertToQuaternion(s[1]);
			} catch {
				t.position = Vector3.zero;
				t.rotation = Quaternion.identity;
			}
		}
		public static string					ConvertFromTransform(	Transform t)
		{
			return t.position.ToString() + "|" + t.rotation.ToString();
		}

		private static long						Fix(float Number) 
		{ 
			if (Number >= 0) 
			{ 
				return (long)Mathf.Floor(Number); 
			} 
			return (long)Mathf.Ceil(Number); 
		}
		public	static float					Normalize(float value)
		{
			if (value > 0f) return 1f;
			if (value < 0f) return -1f;
			return 0f;
		}

		public	static byte[]					GetBytes(string str)
		{
			byte[] bytes = new byte[str.Length * sizeof(char)];
			System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
			return bytes;
		}
		public	static string					GetString(byte[] bytes)
		{
			char[] chars = new char[bytes.Length / sizeof(char)];
			System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
			return new string(chars);
		}

	#endregion

	#region "TIMER CLASS"

		public class Timer
		{
			float					fStartTime;
			float					fStopTime;
			int						intStartTime;
			int						intStopTime;
			bool					blnIsRunning = false;
			public				Timer() {  }

			public	int		GetTime
			{
				get
				{
					if (blnIsRunning)
						return (int)Time.time - intStartTime;
					else
						return intStopTime - intStartTime;
				}
			}
			public	float	GetFloatTime
			{
				get
				{
					if (blnIsRunning)
						return Time.time - fStartTime;
					else
						return fStopTime - fStartTime;
				}
			}
			public	bool	IsRunning
			{
				get
				{
					return blnIsRunning;
				}
			}
			public	void	StartTimer()
			{
				fStartTime		= Time.time;
				intStartTime	= (int)Time.time;
				blnIsRunning	= true;
			}
			public	void	StopTimer()
			{
				fStopTime			= Time.time;
				intStopTime		= (int)Time.time;
				blnIsRunning	= false;
			}
		}

	#endregion

	#region "FILE FUNCTIONS"

		public	static	string				ReadTextFile( string strDirectory,	string strFileName, bool blnStripCRLF = false)
		{
			// STRIP LEADING AND TRAILING SLASHES
			if (strDirectory.StartsWith("/") || strDirectory.StartsWith("\\"))
					strDirectory = strDirectory.Substring(1);
			if (strDirectory.EndsWith("/") || strDirectory.EndsWith("\\"))
					strDirectory = strDirectory.Substring(0, strDirectory.Length - 1);
			if (strFileName.StartsWith("/") || strFileName.StartsWith("\\"))
					strFileName = strFileName.Substring(1);

			bool		blnFound		= false;
			string	strContent	= "";
			string	strPath			= (Application.persistentDataPath + "/" + strDirectory+ "/" + strFileName).Replace("//", "/");
			blnFound = File.Exists(strPath);
			if (!blnFound)
			{
				strPath		= (Application.dataPath + "/" + strDirectory + "/" + strFileName).Replace("//", "/");
				blnFound	= File.Exists(strPath);
			}
			try 
			{
				if (blnFound)
				{ 
				StreamReader src = new StreamReader(strPath);
				strContent = src.ReadToEnd();
				src.Close();

				if (blnStripCRLF)
					strContent = strContent.Replace("\n\n", "\n");
				} else
					Debug.LogError("Unable to read \"" + strPath + "\".");
			} catch {
				Debug.LogError("Unable to read \"" + strPath + "\".");
			}
			return strContent;
		}
		public	static	bool					WriteTextFile(string strDirectory, string strFilename, string strText)
		{
			try
			{
				strDirectory = Application.dataPath + "/" + strDirectory;
	
				if (!Directory.Exists(strDirectory))
				{
					try
					{  
						Directory.CreateDirectory(strDirectory);
					} catch {
						Debug.LogError("Unable to Create directory \"" + strDirectory + "\".");
					}
				}
				if (Directory.Exists(strDirectory))
				{ 
					System.IO.File.WriteAllText(strDirectory + "/" + strFilename, strText);
					return true;
				}
			} catch {
				Debug.LogError("Unable to write \"" + strFilename + "\" to \"" + strDirectory + "\"");
			}
			return false;
		}
		public	static	bool					FileExists(		string strDirectory, string strFilename)
		{
			strDirectory = Application.dataPath + "/" + strDirectory;
			if (!Directory.Exists(strDirectory))
				return false;
			else if (File.Exists(strDirectory + "/" + strFilename))
				return true;
			return false;
		}

	#endregion

	#region "MAP UTILITIES"

		private static	double	RadiansToDegrees(double rad)
		{
			return (rad / Mathf.PI * 180.0);
		}
		private static	double	DegreesToRadians(double deg)
		{
			return (deg * Mathf.PI / 180.0);
		}

	#endregion

	#region "UNITY3D CAMERA FUNCTIONS"

		public	static	Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion angle)
		{
			return angle * (point - pivot) + pivot;
		}
		public	static	void		OrbitCameraUpdate(GameObject goOrbitingObject, GameObject goOrbitTarget = null, bool blnUseCamera = false, float fOrbitDegrees = 1, float fRotationSpeed = 25)
		{

			if (goOrbitingObject == null)
				return;

			if (goOrbitTarget != null)
			{
				// THIS OBJECT ORBITS SELECTED GAMEOBJECT
				goOrbitingObject.transform.position = Util.RotatePointAroundPivot(goOrbitingObject.transform.position, goOrbitTarget.transform.position, Quaternion.Euler(0, (fOrbitDegrees * fRotationSpeed) * Time.deltaTime, 0));
				goOrbitingObject.transform.LookAt(goOrbitTarget.transform.position);
			} else if (goOrbitingObject.transform.parent != null) {
				// CHILD GAMEOBJECT ORBITS PARENT
				goOrbitingObject.transform.position = Util.RotatePointAroundPivot(goOrbitingObject.transform.position, goOrbitingObject.transform.parent.position, Quaternion.Euler(0, (fOrbitDegrees * fRotationSpeed) * Time.deltaTime, 0));
			}
		}
		public	static	void		AlwaysFaceObject(GameObject goFacee, GameObject goScale = null, GameObject goTarget = null, float fScaleModifier = 1.0f)
		{
			if (goScale == null)
					goScale = goFacee;
			if (goFacee != null)
			{
				if (goTarget != null)
					goFacee.transform.LookAt(goTarget.transform.position);
				else
				{
					goFacee.transform.rotation = Camera.main.transform.rotation;
					float fDistance = Vector3.Distance(goFacee.transform.position, Camera.main.transform.position);
					fDistance = fDistance * fScaleModifier;
					goScale.transform.localScale = new Vector3(fDistance, fDistance, fDistance);
				}
			}
		}

		public	static Vector3	SetVectorX(	Vector3 v, float x) { v.x = x; return v; }
		public	static Vector3	SetVectorY(	Vector3 v, float y) { v.y = y; return v; }
		public	static Vector3	SetVectorZ(	Vector3 v, float z) { v.z = z; return v; }
		public	static Vector3	SetVectorXZ(Vector3 v, float x, float z) { v.x = x; v.z = z; return v; }
		public	static Vector3	AddVectorX(	Vector3 v, float x) { v.x += x; return v; }
		public	static Vector3	AddVectorY(	Vector3 v, float y) { v.y += y; return v; }
		public	static Vector3	AddVectorZ(	Vector3 v, float z) { v.z += z; return v; }
		public	static Vector3	VectorXZ(		Vector3 v) { return new Vector3(v.x, 0f, v.z); }
		public	static float		Distance2D(	Vector3 v1, Vector2 v2) { v1.y = v2.y = 0; return Vector3.Distance(v1, v2); }
		public	static Vector3	MakeUniformScale(float scale) { return new Vector3(scale, scale, scale); }
		public	static void			LookAt2D(Transform transform, Vector3 point)
		{
			Vector3 e = transform.eulerAngles;
			transform.LookAt(point);
			e.y = transform.eulerAngles.y;
			transform.eulerAngles = e;
		}
		public	static float		CalculateMass(float radius)
		{
			return Mathf.Min(radius * 0.05f, 10f);
		}

	#endregion

	#region "UNITY3D GUI FUNCTIONS"

		public	static	void	AutoResize(int screenWidth, int screenHeight)
		{
			Vector2 resizeRatio = new Vector2((float)Screen.width / screenWidth, (float)Screen.height / screenHeight);
			GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(resizeRatio.x, resizeRatio.y, 1.0f));
		}
		public	static	void	CalcWidth(Image pnl, int intCur, int intMax, int intMaxWidth, int intMaxHeight)
		{
			float fCur = Util.ConvertToFloat(intCur);
			float fMax = Util.ConvertToFloat(intMax);
			pnl.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2((intMaxWidth * (fCur / fMax)), intMaxHeight);
		}

	#endregion

	#region "UNITY3D MODEL FUNCTIONS"

		public	static Vector3 GetModelCenter(GameObject go)
		{
			Vector3 v3 = go.transform.position;
			return new Vector3(v3.x, v3.y + 1.5f, v3.z);
		}

	#endregion

	#region "UNITY3D COMPONENT FUNCTIONS"

		public	static	void				CopyComponent(GameObject	go,			Component orig)
		{
			Component newComp;
			newComp = go.GetComponent(orig.GetType());
			if (newComp == null)
				newComp = go.AddComponent(orig.GetType());

			newComp.gameObject.SetActive(orig.gameObject.activeSelf);
//		newComp.active = orig.active;		// THIS WAS DEPRECATED

			foreach (FieldInfo f in orig.GetType().GetFields())
			{
				try 
				{
					f.SetValue(newComp, f.GetValue(orig)); 
				} catch { }
			}
		}
		public	static	void				CopyComponent(Component		target, Component orig)
		{
			GameObject go = target.gameObject;
			Component newComp;
			if (target.GetType() != orig.GetType())
			{
				newComp = go.GetComponent(orig.GetType());
				if (newComp == null)
					newComp = go.AddComponent(orig.GetType());
			} else
				newComp = target;

			newComp.gameObject.SetActive(orig.gameObject.activeSelf);
//		newComp.active = orig.active;		// THIS WAS DEPRECATED

			foreach (FieldInfo f in orig.GetType().GetFields())
			{
				try 
				{
					f.SetValue(newComp, f.GetValue(orig)); 
				} catch { }
			}
		}

		public	static	GameObject	GetChildGameObject(GameObject fromGameObject, string strObjectName)
		{
			Transform[] ts = fromGameObject.transform.GetComponentsInChildren<Transform>(true);
			foreach (Transform t in ts) 
			{
				if (t != null && t.gameObject != null && t.gameObject.name != null)
					if (t.gameObject.name == strObjectName) 
						return t.gameObject;
			}
			return null;
		}

	#endregion

	#region "UNITY3D MISC FUNCTIONS"

		public	static	GameObject		GetTrueParent(GameObject goChild)
		{
			GameObject goReturn = goChild;
			while (goReturn.transform.parent			!= null			&& 
						 goReturn.transform.parent.name != "Canvas" &&
						 goReturn.transform.parent.tag	!= "Game"		&&
						 goReturn.transform.parent.tag	!= "Container")
			{
				goReturn = goReturn.transform.parent.gameObject;
			}
			return goReturn;
		}
		public	static	GameObject		GetParentWithTag(GameObject goChild, string strTag)
		{
			GameObject goReturn = goChild;
			while (goReturn.transform.parent.gameObject != null)
			{
				goReturn = goReturn.transform.parent.gameObject;
				if (goReturn.transform.tag.ToLower() == strTag.ToLower())
					return goReturn;
			}
			return goReturn;
		}
		public	static	GameObject		GetClosestObjectByTag(GameObject goSource, string strTag)
		{
			GameObject[]	objectsWithTag	= GameObject.FindGameObjectsWithTag(strTag);
			GameObject		closestObject		= null;
	
			foreach (GameObject obj in objectsWithTag)
			{
				if(!closestObject)
					closestObject = obj;

				if (Vector3.Distance(goSource.transform.position, obj.transform.position) <= Vector3.Distance(goSource.transform.position, closestObject.transform.position))
					closestObject = obj;
			}
			return closestObject;
		}
		public	static	Color					HexToColor(string hex, byte opacity = 255)
		{
			hex = hex.ToUpper();
			hex = hex.Replace("#", "");
			if (hex.Length < 6)
					hex = hex.PadRight(6, 'F');
			byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
			byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
			byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
			byte a = opacity;
			if (hex.Length > 6 && opacity == 255)
					 a = byte.Parse(hex.PadRight(8, 'F').Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
			return new Color32(r, g, b, a);
		}
		public	static	string				ColorToHex(Color c, bool useHash = true)
		{
			if (useHash)
				return string.Format("#{0:X2}{1:X2}{2:X2}", ToByte(c.r), ToByte(c.g), ToByte(c.b));
			else
				return string.Format("{0:X2}{1:X2}{2:X2}", ToByte(c.r), ToByte(c.g), ToByte(c.b));
		}
		private static	byte					ToByte(float f)
		{
			f = Mathf.Clamp01(f);
			return (byte)(f * 255);
		}
		public	static	Sprite				GetSprite(string strDirectory, string strSpriteName)
		{
			if (!strDirectory.EndsWith("/"))
					strDirectory += "/";
			strSpriteName = strSpriteName.Replace("/", "");
			Sprite sp = Resources.Load<Sprite>(strDirectory + strSpriteName);
			return sp;
		}
		public	static	Vector2				MultiplyBy(Vector2 v1, Vector2 v2)
		{
			Vector2 v3 = new Vector2();
			v3.x = v1.x * v2.x;
			v3.y = v1.y * v2.y;
			return v3;
		}
		public	static	Vector3				MultiplyBy(Vector3 v1, Vector3 v2)
		{
			Vector3 v3 = new Vector3();
			v3.x = v1.x * v2.x;
			v3.y = v1.y * v2.y;
			v3.z = v1.z * v2.z;
			return v3;
		}
		public	static	Vector2				MultiplyBy(Vector2 v1, Vector3 v2)
		{
			Vector2 v3 = new Vector2();
			v3.x = v1.x * v2.x;
			v3.y = v1.y * v2.y;
			return v3;
		}
		public	static	Vector2				MultiplyBy(Vector2 v1, float f)
		{
			Vector2 v3 = new Vector2();
			v3.x = v1.x * f;
			v3.y = v1.y * f;
			return v3;
		}
		public	static	Vector3				MultiplyBy(Vector3 v1, float f)
		{
			Vector3 v3 = new Vector3();
			v3.x = v1.x * f;
			v3.y = v1.y * f;
			v3.z = v1.z * f;
			return v3;
		}
		public	static	Vector2				DivideBy(Vector2 v1, Vector2 v2)
		{
			Vector2 v3 = new Vector2();
			v3.x = v1.x / v2.x;
			v3.y = v1.y / v2.y;
			return v3;
		}
		public	static	Vector3				DivideBy(Vector3 v1, Vector3 v2)
		{
			Vector3 v3 = new Vector3();
			v3.x = v1.x / v2.x;
			v3.y = v1.y / v2.y;
			v3.z = v1.z / v2.z;
			return v3;
		}
		public	static	Vector2				DivideBy(Vector2 v1, Vector3 v2)
		{
			Vector2 v3 = new Vector2();
			v3.x = v1.x / v2.x;
			v3.y = v1.y / v2.y;
			return v3;
		}
		public	static	Vector2				DivideBy(Vector2 v1, float f)
		{
			Vector2 v3 = new Vector2();
			v3.x = v1.x / f;
			v3.y = v1.y / f;
			return v3;
		}
		public	static	Vector3				DivideBy(Vector3 v1, float f)
		{
			Vector3 v3 = new Vector3();
			v3.x = v1.x / f;
			v3.y = v1.y / f;
			v3.z = v1.z / f;
			return v3;
		}

	#endregion

	#region "COROUTINE WITH DATA (COROUTINE PASSES DATA OUT)"

		public class CoroutineWithData 
		{
			public	Coroutine		coroutine { get; private set; }
			private IEnumerator	target;
			public	object			result;

			public CoroutineWithData(MonoBehaviour owner, IEnumerator target) 
			{
				this.target			= target;
				this.coroutine	= owner.StartCoroutine(Run());
			}
			private IEnumerator Run() 
			{
				while(target.MoveNext()) 
				{
					result = target.Current;
					yield return result;
				}
			}
		}

	#endregion

	#region "SPEED LABELS"

		public	static	string		GetSpeedLabel(char ch)
		{
			switch (ch)
			{
				case 'N':
					return "kts";
				case 'M':
					return "mph";
				case 'K':
					return "kph";
				default:
					return "mph";
			}
		}

	#endregion

	#region "DISTANCE LABELS"

		public	static	string		GetDistanceLabel(char ch)
		{
			switch (ch)
			{
				case 'N':
					return "nm";
				case 'M':
					return "miles";
				case 'K':
					return "km";
				default:
					return "miles";
			}
		}

	#endregion

	#region "CLIPBOARD FUNCTIONS"

		public	static	void			CopyToClipboard(string strText)
		{
			GUIUtility.systemCopyBuffer = strText;
		}
		public	static	string		PasteFromClipboard()
		{
			return GUIUtility.systemCopyBuffer;
		}

	#endregion

}
