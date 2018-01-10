using UnityEngine;
using System.Collections.Generic;
using System.Xml;
#if UNITY_WP8
using System.Xml.Linq;
#endif

public class CLanguageManager : MonoBehaviour
{

    private TextAsset textFile;
#if UNITY_WP8
	private XDocument theXMLDocument = new XDocument();
#else
    private XmlDocument theXMLDocument = new XmlDocument();
#endif

    private int _iLanguage = 0;

    //Add found text from xml into Dictionnary to avoid scanning again xml
    private Dictionary<string, string> _Dictionnary = new Dictionary<string, string>();

    public static CLanguageManager Get()
    {
        return GameObject.Find("_language_manager").GetComponent<CLanguageManager>();
    }

    ///-----------------------------------------------------------------------------------
    /// 
    ///-----------------------------------------------------------------------------------
    void Awake()
    {
        DontDestroyOnLoad(this);
        SetLanguage(Application.systemLanguage.ToString());
    }

    public string GetLanguage()
    {
        return PlayerPrefs.GetString("language");
    }

    public int GetLanguageIndex()
    {
        return _iLanguage;
    }

    public string GetLanguageCode()
    {
        return PlayerPrefs.GetString("language").ToUpper();
    }

    public void SetLanguageIndex( int a_iValue )
	{
		_iLanguage = a_iValue;

		switch( _iLanguage )
		{
		case 1 :
			PlayerPrefs.SetString("language","fr");
			break;
		case 2 :
			PlayerPrefs.SetString("language","es");
			break;
		default :
			PlayerPrefs.SetString("language","en");
			break;
		}

		textFile = Resources.Load("Texts/content_"+PlayerPrefs.GetString("language"),typeof(TextAsset)) as TextAsset;

		if( textFile == null )
		{
			textFile = Resources.Load("Texts/content_en",typeof(TextAsset)) as TextAsset;
			_iLanguage = 0;
		}

		#if UNITY_WP8
		theXMLDocument = XDocument.Parse( textFile.text);
		#else
		theXMLDocument.LoadXml(textFile.text);
		#endif
	}

	///-----------------------------------------------------------------------------------
	/// <summary> Allows to define the app language </summary>
	/// <param name="a_strLanguage"> language to set </param>
	///-----------------------------------------------------------------------------------
	public void SetLanguage( string a_strLanguage )
	{
		switch( a_strLanguage )
		{
		case "French" :
			PlayerPrefs.SetString("language","fr");
			_iLanguage = 1;
			break;
		case "Spanish" :
			PlayerPrefs.SetString("language", "es");
			_iLanguage = 2;
			break;
		default :
			PlayerPrefs.SetString("language","en");
			_iLanguage = 0;
			break;
		}

		textFile = Resources.Load("Texts/content_"+PlayerPrefs.GetString("language"),typeof(TextAsset)) as TextAsset;

		if( textFile == null )
		{
			textFile = Resources.Load("Texts/content_en",typeof(TextAsset)) as TextAsset;
			_iLanguage = 0;
		}
		#if UNITY_WP8
		theXMLDocument = XDocument.Parse( textFile.text);
		#else
		theXMLDocument.LoadXml(textFile.text);
		#endif
	}

    ///-----------------------------------------------------------------------------------
    /// <summary> Recovers a label </summary>
    /// <param name="a_strText"> the XML label </param>
    /// <returns> the string found </returns>
    ///-----------------------------------------------------------------------------------
    public string GetText(string a_strText)
    {
        if (string.IsNullOrEmpty(a_strText))
            return "";

        if (_Dictionnary.ContainsKey(a_strText))
        {
            return _Dictionnary[a_strText];
        }

#if UNITY_WP8
		XElement content;
		try
		{
			content = theXMLDocument.Root.Element ( a_strText );
		}
		catch
		{
			return " ";
		}
#else

        XmlNode content = theXMLDocument.SelectSingleNode("descendant::" + a_strText);
#endif

        // return " " if the content does not exist, for debugging
        string result = " ";


        if (content != null)
        {
#if UNITY_WP8
			result =  content.Value;
#else
            result = content.InnerText;
#endif
        }

        _Dictionnary.Add(a_strText, result);
        return result;
    }
}