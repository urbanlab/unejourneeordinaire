// Only works on ARGB32, RGB24 and Alpha8 textures that are marked readable

using System.Threading;
using UnityEngine;
using System.Collections;

public static class TextureScale 
{
	public class ThreadData
	{
		public int start;
		public int end;
		public ThreadData (int s, int e) {
			start = s;
			end = e;
		}
	}
	
	private static Color32[] texColors;
	private static Color32[] newColors;
	private static int w;
	private static float ratioX;
	private static float ratioY;
	private static int w2;
	private static int finishCount;
	private static Mutex mutex;
	
	public static void Point (Texture2D a_rTexture2D, int newWidth, int newHeight)
	{
		ThreadedScale (ref a_rTexture2D, newWidth, newHeight, false);
	}
	
	public static void Bilinear (ref Texture2D a_rTexture2D, int newWidth, int newHeight)
	{
		ThreadedScale (ref a_rTexture2D, newWidth, newHeight, true);
	}
	
	private static void ThreadedScale (ref Texture2D a_rTexture2D, int newWidth, int newHeight, bool useBilinear)
	{
		texColors = a_rTexture2D.GetPixels32();
		newColors = new Color32[newWidth * newHeight];
		if (useBilinear)
		{
			ratioX = 1.0f / ((float)newWidth / (a_rTexture2D.width-1));
			ratioY = 1.0f / ((float)newHeight / (a_rTexture2D.height-1));
		}
		else {
			ratioX = ((float)a_rTexture2D.width) / newWidth;
			ratioY = ((float)a_rTexture2D.height) / newHeight;
		}
			
		w = a_rTexture2D.width;
		w2 = newWidth;

		System.GC.Collect ();

		var cores = Mathf.Min(SystemInfo.processorCount, newHeight);
		var slice = newHeight/cores;
		
		finishCount = 0;
		if (mutex == null) {
			mutex = new Mutex(false);
		}
		if (cores > 1)
		{
			int i = 0;
			ThreadData threadData;
			for (i = 0; i < cores-1; i++) {
				threadData = new ThreadData(slice * i, slice * (i + 1));
				ParameterizedThreadStart ts = useBilinear ? new ParameterizedThreadStart(BilinearScale) : new ParameterizedThreadStart(PointScale);
				Thread thread = new Thread(ts);
				thread.Start(threadData);
			}
			threadData = new ThreadData(slice*i, newHeight);
			if (useBilinear)
			{
				BilinearScale(threadData);
			}
			else
			{
				PointScale(threadData);
			}
			while (finishCount < cores)
			{
				Thread.Sleep(1);
			}
		}
		else
		{
			ThreadData threadData = new ThreadData(0, newHeight);
			if (useBilinear)
			{
				BilinearScale(threadData);
			}
			else
			{
				PointScale(threadData);
			}
		}
			
		a_rTexture2D.Resize(newWidth, newHeight);
		a_rTexture2D.SetPixels32(newColors);
		a_rTexture2D.Apply();
	}
	
	public static void BilinearScale (System.Object obj)
	{
		ThreadData threadData = (ThreadData) obj;
		for (var y = threadData.start; y < threadData.end; y++)
		{
			int yFloor = (int)Mathf.Floor(y * ratioY);
			var y1 = yFloor * w;
			var y2 = (yFloor+1) * w;
			var yw = y * w2;
			
			for (var x = 0; x < w2; x++) {
				int xFloor = (int)Mathf.Floor(x * ratioX);
				var xLerp = x * ratioX-xFloor;
				newColors[yw + x] = ColorLerpUnclamped(ColorLerpUnclamped(texColors[y1 + xFloor], texColors[y1 + xFloor+1], xLerp),
				                                       ColorLerpUnclamped(texColors[y2 + xFloor], texColors[y2 + xFloor+1], xLerp),
				                                       y*ratioY-yFloor);
			}
		}
		
		mutex.WaitOne();
		finishCount++;
		mutex.ReleaseMutex();
		System.GC.Collect();
	}
	
	public static void PointScale (System.Object obj)
	{
		ThreadData threadData = (ThreadData) obj;
		for (var y = threadData.start; y < threadData.end; y++)
		{
			var thisY = (int)(ratioY * y) * w;
			var yw = y * w2;
			for (var x = 0; x < w2; x++) {
				newColors[yw + x] = texColors[(int)(thisY + ratioX*x)];
			}
		}
		
		mutex.WaitOne();
		finishCount++;
		mutex.ReleaseMutex();
		System.GC.Collect ();
	}
	
	private static Color ColorLerpUnclamped (Color c1, Color c2, float value)
	{
		return new Color (c1.r + (c2.r - c1.r)*value, 
		                  c1.g + (c2.g - c1.g)*value, 
		                  c1.b + (c2.b - c1.b)*value, 
		                  c1.a + (c2.a - c1.a)*value);
	}
}
