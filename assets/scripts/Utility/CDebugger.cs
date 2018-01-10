using UnityEngine;
using System.Collections;
using System.Threading;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using System.Collections.Generic;
using System.IO;

public class CDebugger {

	public static bool enable = false;
	
	public static int MAX_STACK_SIZE = 1000;
	
	public static List<string> _aMessages = new List<string>();
	
	public static void PrepareDirectoryForDebug(string directory)
	{
		foreach(string file in Directory.GetFiles(directory, "*.cs", SearchOption.AllDirectories))
		{
			PrepareForDebug(file);
		}
		
	}
	
	public static void PrepareForDebug(string fileName)
	{
		if(fileName.Contains("CDebugger"))
			return;
			
		string file = File.ReadAllText(fileName);

		file = file.Replace(")\n\t{", ")\n\t{\n\t\tCDebugger.Log();");
		//file = System.Text.RegularExpressions.Regex.Replace (")[\t]{", "{\nCDebugger.Log()");
		File.WriteAllText(fileName, file);
		
	}
	
	public static void RemoveDebugForDirectory(string directory)
	{
		foreach(string file in Directory.GetFiles(directory, "*.cs", SearchOption.AllDirectories))
		{
			RemoveDebug(file);
		}
	}
	
	public static void RemoveDebug(string fileName)
	{
		if(fileName.Contains("CDebugger"))
			return;
		string file = File.ReadAllText(fileName);
		file = file.Replace(")\n\t{\n\t\tCDebugger.Log();", ")\n\t{");
		//file = System.Text.RegularExpressions.Regex.Replace (")[\t]{", "{\nCDebugger.Log()");
		File.WriteAllText(fileName, file);
		
	}
	
	public static void CheckFreeze()
	{
		if(enable)	
			timer = new Timer(TimerCallBack, null, 1000, 1000);
		
	}
	
	static float lastLogTime = 0f;
	static Timer timer;
	private static void TimerCallBack(object o)
	{
		//UnityEngine.Debug.Log ("truc");
		if(System.DateTime.Now.Ticks - lastLogTime > 20000)
		{
			PrintStackTrace();
			timer.Dispose();
		}
		else
		{
			lastLogTime = _aMessages.Count;
		}
	}
	
	public static void Log(params object[] msg)
	{
	
		if(!enable)
			return;
			
		StackFrame frame = new StackTrace(true).GetFrame(1);
		string message = 
				"[" + GetTag() + "]" +
				"[" + GetAppName() + "]" +
				GetMethodName(frame) + 
				GetLineNumber(frame) + 
				GetMsg(msg) + "\n" +
				System.Environment.StackTrace;
		
		_aMessages.Add (message);
		lastLogTime = System.DateTime.Now.Ticks;
		if(_aMessages.Count > MAX_STACK_SIZE)
		{
			_aMessages.RemoveAt(0);
		}
		
		
		
		//UnityEngine.Debug.Log ("<b><color=blue>" + message + "</color></b>");
	}
	
	//Get specific message in list
	private static List<string> GetMessagesAt(List<string> messages, params object[] args)
	{
		List<string> res = new List<string>();
		for(int i = 0; i < args.Length; i++)
		{
			res.Add (messages[(int) args[i]]);
		}
		return res;
	}
	
	//Get message containing
	private static List<string> GetMessagesContaining(List<string> messages, params object[] args)
	{
		List<string> res = new List<string>();
		foreach(string s in messages)
		{
			if(s.Contains((string) args[0]))
			{
				res.Add (s);
			}
		}
		return res;
	}
	
	//Get get message where object is
	//get message where linenumber is
	//Get message where appname is
	//Get message where product name is
	//get message where scene is
	//get message count
	
	
	
	private static string GetMsg(object[] values)
	{
		string res = "";
		if(values.Length == 0)
		{
			return res;
		}
		
		foreach(object value in values)
		{
			res += value + "; ";
		}
		return "(" + res.Substring(0, res.Length - 2) + ")";
	}
	
	private static string GetMethodName(StackFrame frame)
	{
		return frame.GetMethod().DeclaringType + "." + frame.GetMethod().Name;
	}
	
	private static string GetLineNumber(StackFrame frame)
	{
		int l = frame.GetFileLineNumber();
		return l == 0 ? "" : "->" + l;
	}
	
	private static string GetTag()
	{
		return "CDEBUGGER";
	}
	
	private static string GetAppName()
	{
		#if UNITY_5_2
		return Application.companyName + "." + Application.productName + "." + Application.loadedLevelName;
		#else
		return Application.companyName + "." + Application.productName + "." + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
		#endif
	}

	public static void PrintStackTrace()
	{
		foreach(string msg in _aMessages)
		{
			UnityEngine.Debug.Log (msg);
		}
		
	}
	
	public static void Dispose()
	{
		if(enable)
			timer.Dispose();
	}
	

	
}
