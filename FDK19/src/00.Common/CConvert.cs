﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace FDK
{
	public static class CConvert
	{
		// メソッド

		public static double DegreeToRadian( double angle )
		{
			return ( ( Math.PI * angle ) / 180.0 );
		}
		public static double RadianToDegree( double angle )
		{
			return ( angle * 180.0 / Math.PI );
		}
		public static float DegreeToRadian( float angle )
		{
			return (float) DegreeToRadian( (double) angle );
		}
		public static float RadianToDegree( float angle )
		{
			return (float) RadianToDegree( (double) angle );
		}

		/// <summary>
		/// 百分率数値を255段階数値に変換するメソッド。透明度用。
		/// </summary>
		/// <param name="num"></param>
		/// <returns></returns>
		public static int nParsentTo255(double num)
		{
			return (int)(255.0 * num);
		}

		/// <summary>
		/// 255段階数値を百分率に変換するメソッド。
		/// </summary>
		/// <param name="num"></param>
		/// <returns></returns>
		public static int n255ToParsent(int num)
		{
			return (int)(100.0 / num);
		}
	} 
}
