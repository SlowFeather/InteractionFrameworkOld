using UnityEngine;
using System.Collections;

public class ForegroundToImage : MonoBehaviour 
{

	void Update () 
	{
		BackgroundRemovalManager backManager = BackgroundRemovalManager.Instance;

		if(backManager && backManager.IsBackgroundRemovalInitialized())
		{
			UnityEngine.UI.RawImage guiTexture = GetComponent<UnityEngine.UI.RawImage>();

			if(guiTexture && guiTexture.texture == null)
			{
				guiTexture.texture = backManager.GetForegroundTex();
			}
		}
	}

}
