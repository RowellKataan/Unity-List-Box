// ===========================================================================================================
//
// Class/Library: Progress Bar Control - Demo Application
//        Author: Michael Marzilli   ( http://www.linkedin.com/in/michaelmarzilli , http://www.develteam.com/Developer/Rowell/Portfolio )
//       Created: Jul 18, 2016
//	
// VERS 1.0.000 : Jul 18, 2016 : Original File Created. Released for Unity 3D.
//
// ===========================================================================================================

#if UNITY_EDITOR
	#define		IS_DEBUGGING
#else
	#undef		IS_DEBUGGING
#endif

using UnityEngine;

public class DemoProgressBar : MonoBehaviour 
{

	#region "PRIVATE VARIABLES"

		private float				fPercent	= 0.00f;
		private float				fTotal		= 0.00f;
		private int					intDir		= 1;
		private ProgressBar	pb				= null;

	#endregion

	#region "PRIVATE FUNCTIONS"

		private void				Start () 
		{
			pb = GetComponent<ProgressBar>();
			pb.SetProgress(0, 100.00f);
		}
		private void				FixedUpdate()
		{
			float f = Random.Range(0.00f, 1.00f);
			f = f * intDir;
			fTotal += f;
			if (fTotal >= 100.00f)
			{
				fTotal = 100.00f;
				intDir = -1;
			} else if (fTotal <= 0) {
				fTotal = 0.00f;
				intDir = 1;
			}
			pb.SetProgress(fTotal, 100.00f);
		}

	#endregion

}
