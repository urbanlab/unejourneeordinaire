
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using System.Diagnostics;

public class CPlatformToggle : EditorWindow
{
	private enum enumExportType
	{
		NONE,

		GOOGLE_CARDBOARD,
		SAMSUNG_GEAR_VR,

		TABLET
	}


	private GUISkin m_guiSkin;

	private string android_3D_cam = "Prefabs/cameras/GoogleCardboard";
	private string samsung_gear_cam = "Prefabs/cameras/SamsungGearVR";
	private string standard_unity_cam = "Prefabs/cameras/StandardUnityCamera";

	private Texture2D _t2dCardboardIcon;
	private Texture2D _t2dGearIcon;
	
	enumExportType _eSelectedBuildExport = enumExportType.SAMSUNG_GEAR_VR;

	[MenuItem("IHMTEK/App Selector")]
	public static void ShowWindow()
	{
		EditorWindow.GetWindow(typeof(CPlatformToggle));
	}

	///-----------------------------------------------------------------------------------
	/// 																			
	///-----------------------------------------------------------------------------------
	void Awake()
	{
		m_guiSkin = Resources.Load( "Fonts/GUISkin") as GUISkin;

		_t2dGearIcon =  Resources.Load ("Textures/PUBLISHING/icon_android_gear") as Texture2D; 
		_t2dCardboardIcon =  Resources.Load ("Textures/PUBLISHING/icon_android_cardboard") as Texture2D;
	}

	///-----------------------------------------------------------------------------------
	/// 																			
	///-----------------------------------------------------------------------------------
	void OnGUI()
	{
		if (!EditorApplication.isPlaying)
		{
			_eSelectedBuildExport = (enumExportType) EditorGUILayout.EnumPopup("Select type of build", _eSelectedBuildExport);
		}

		if ( GUILayout.Button("Apply SetUp"))
		{
			SetUp();
		}

		if (GUILayout.Button( "Open Player Settings" ))
		{
			EditorApplication.ExecuteMenuItem("Edit/Project Setttings/Player");
		}
			

	}

	///-----------------------------------------------------------------------------------
	/// 																			
	///-----------------------------------------------------------------------------------
	void SetUp( )
	{
		switch( _eSelectedBuildExport )
		{
			case enumExportType.NONE:
				break;

		case enumExportType.GOOGLE_CARDBOARD:
			SetupGoogleCardboard();
			break;

		case enumExportType.SAMSUNG_GEAR_VR:
			SetupSamsungGearVR();
			break;

		case enumExportType.TABLET:
			SetupTablet();
			break;

		}
	}

	///-----------------------------------------------------------------------------------
	/// 																			
	///-----------------------------------------------------------------------------------
	void SetupGoogleCardboard()
	{
		//Replace camera
		DestroyCurrentCamera();
		Object instance = Instantiate(Resources.Load (android_3D_cam));
		instance.name = "Camera";
		EditorApplication.SaveScene ();

		//Change manifest
		Texture2D[] texture = new Texture2D[1];
		System.IO.File.Delete("Assets/plugins/android/AndroidManifest.xml");

		if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android) 
		{
			texture[0] = _t2dCardboardIcon;
		}
		else if(EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS )
		{
			texture[0] = Resources.Load ("Textures/PUBLISHING/icon_android_ios") as Texture2D;
		}

		string strApplicationName =  "LOGEMENT";

		PlayerSettings.productName = strApplicationName;
		PlayerSettings.SetIconsForTargetGroup (BuildTargetGroup.Unknown, texture);
		PlayerSettings.applicationIdentifier = "com.ihmtek.logementadaptecardboard";
		System.IO.File.Copy ("Assets/plugins/android/AndroidManifestAndroid.xml", "Assets/plugins/android/AndroidManifest.xml");
		PlayerSettings.virtualRealitySupported = false;

		// Activate client wifi manager
		EditorSceneManager.OpenScene ("Assets/Scenes/Start.unity");
		GameObject goClient = GameObject.Find("Wifi").transform.GetChild(0).gameObject;
		goClient.GetComponent< EasyWiFiManager >().applicationName = strApplicationName;
		goClient.SetActive( true );


		GameObject.Find("Wifi").transform.GetChild(1).gameObject.SetActive( false ); //Server
		EditorApplication.SaveScene ();
	}

	///-----------------------------------------------------------------------------------
	/// 																			
	///-----------------------------------------------------------------------------------
	void SetupSamsungGearVR( )
	{
		//Replace camera
		DestroyCurrentCamera();
		Object instance = Instantiate(Resources.Load (samsung_gear_cam));
		instance.name = "Camera";
		EditorApplication.SaveScene ();

		//Change manifest
		Texture2D[] texture = new Texture2D[1];
		System.IO.File.Delete("Assets/plugins/android/AndroidManifest.xml");

		texture[0] = _t2dGearIcon;

		string strApplicationName = "LOGEMENT";

		PlayerSettings.productName = strApplicationName;
		PlayerSettings.SetIconsForTargetGroup (BuildTargetGroup.Unknown, texture);
		PlayerSettings.applicationIdentifier = "com.ihmtek.logementadapte";
		System.IO.File.Copy ("Assets/plugins/android/AndroidManifestGear.xml", "Assets/plugins/android/AndroidManifest.xml");
		PlayerSettings.virtualRealitySupported = true;


		// Activate client wifi manager
		EditorSceneManager.OpenScene ("Assets/Scenes/Start.unity");
		GameObject goClient = GameObject.Find("Wifi").transform.GetChild(0).gameObject;
		goClient.GetComponent< EasyWiFiManager >().applicationName = strApplicationName;
		goClient.SetActive( true ); 

		GameObject.Find("Wifi").transform.GetChild(1).gameObject.SetActive( false ); //Server
		EditorApplication.SaveScene ();
	}

	///-----------------------------------------------------------------------------------
	/// 																			
	///-----------------------------------------------------------------------------------
	void SetupTablet( )
	{
		//Replace camera
		DestroyCurrentCamera();

		Object instance = Instantiate( Resources.Load ( standard_unity_cam ) );
		instance.name = "Camera";
		EditorApplication.SaveScene ();

		Texture2D[] texture = new Texture2D[1];
		System.IO.File.Delete("Assets/plugins/android/AndroidManifest.xml");

		if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android) 
		{
			texture[0] = _t2dCardboardIcon;
		}

		else if(EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS )
		{
			texture[0] = Resources.Load ("Textures/PUBLISHING/icon_android_ios") as Texture2D;
		}

		string strApplicationName = "LOGEMENT";

		PlayerSettings.productName = strApplicationName;
		PlayerSettings.SetIconsForTargetGroup (BuildTargetGroup.Unknown, texture);
		PlayerSettings.applicationIdentifier = "com.ihmtek.logementadaptetablet";

		System.IO.File.Copy ("Assets/plugins/android/AndroidManifestAndroid.xml", "Assets/plugins/android/AndroidManifest.xml");
		PlayerSettings.virtualRealitySupported = false;

		// Activate server wifi manager
		EditorSceneManager.OpenScene ("Assets/Scenes/Start.unity");
		GameObject.Find("Wifi").transform.GetChild(0).gameObject.SetActive( false );

		GameObject goServer = GameObject.Find("Wifi").transform.GetChild(1).gameObject; 
		goServer.GetComponent< EasyWiFiManager >().applicationName = strApplicationName;
		goServer.SetActive( true ); 


		EditorApplication.SaveScene ();
	}

	///-----------------------------------------------------------------------------------
	/// 																			
	///-----------------------------------------------------------------------------------
	void DestroyCurrentCamera()
	{
		if (EditorSceneManager.GetActiveScene ().name != "Main") 
		{
			EditorSceneManager.OpenScene ("Assets/Scenes/Main.unity");
		}

		GameObject.DestroyImmediate(GameObject.Find ("Camera"));
	}
		
}
