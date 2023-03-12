﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using FDK;

namespace TJAPlayer3
{
	[Serializable]
	internal class Cスコア
	{
		// プロパティ

		public STScoreIniInfo ScoreIniInfo;
		[Serializable]
		[StructLayout( LayoutKind.Sequential )]
		public struct STScoreIniInfo
		{
			public DateTime LastWriteTime;
			public long FileSize;

			public STScoreIniInfo( DateTime LastWriteTime, long FileSize )
			{
				this.LastWriteTime = LastWriteTime;
				this.FileSize = FileSize;
			}
		}

		public STFileInfo FileInfo;
		[Serializable]
		[StructLayout( LayoutKind.Sequential )]
		public struct STFileInfo
		{
			public string FileAbsolutePath;
			public string DirAbsolutePath;
			public DateTime LastWriteTime;
			public long FileSize;

			public STFileInfo( string FileAbsolutePath, string DirAbsolutePath, DateTime LastWriteTime, long FileSize )
			{
				this.FileAbsolutePath = FileAbsolutePath;
				this.DirAbsolutePath = DirAbsolutePath;
				this.LastWriteTime = LastWriteTime;
				this.FileSize = FileSize;
			}
		}

		public ST譜面情報 譜面情報;
		[Serializable]
		[StructLayout( LayoutKind.Sequential )]
		public struct ST譜面情報
		{
			public string Title;
			public string Genre;
			public int 演奏回数;
			public STHISTORY 演奏履歴;
			public double Bpm;
			public int Duration;
			public string strBGMファイル名;
			public int SongVol;
			public LoudnessMetadata? SongLoudnessMetadata;
			public bool b歌詞あり;
			public int nデモBGMオフセット;
			public bool[] b譜面が存在する;
			public bool[] b譜面分岐;
			public bool[] bPapaMamaSupport;
			public int[] nハイスコア;
			public int[] nSecondScore;
			public int[] nThirdScore;
			public string[] strHiScorerName;
			public string[] strSecondScorerName;
			public string[] strThirdScorerName;
			public int[] nCrown;
			public string strSubTitle;
			public int[] nレベル;

			[Serializable]
			[StructLayout( LayoutKind.Sequential )]
			public struct STHISTORY
			{
				public string 行1;
				public string 行2;
				public string 行3;
				public string 行4;
				public string 行5;
				public string 行6;
				public string 行7;
				public string this[ int index ]
				{
					get
					{
						switch( index )
						{
							case 0:
								return this.行1;

							case 1:
								return this.行2;

							case 2:
								return this.行3;

							case 3:
								return this.行4;

							case 4:
								return this.行5;
							case 5:
								return this.行6;
							case 6:
								return this.行7;
						}
						throw new IndexOutOfRangeException();
					}
					set
					{
						switch( index )
						{
							case 0:
								this.行1 = value;
								return;

							case 1:
								this.行2 = value;
								return;

							case 2:
								this.行3 = value;
								return;

							case 3:
								this.行4 = value;
								return;

							case 4:
								this.行5 = value;
								return;
							case 5:
								this.行6 = value;
								return;
							case 6:
								this.行7 = value;
								return;
						}
						throw new IndexOutOfRangeException();
					}
				}
			}
		}

		// コンストラクタ

		public Cスコア()
		{
			this.ScoreIniInfo = new STScoreIniInfo( DateTime.MinValue, 0L );
			this.FileInfo = new STFileInfo( "", "", DateTime.MinValue, 0L );
			this.譜面情報 = new ST譜面情報();
			this.譜面情報.Title = "";
			this.譜面情報.Genre = "";
			this.譜面情報.演奏回数 = new int();
			this.譜面情報.演奏履歴 = new ST譜面情報.STHISTORY();
			this.譜面情報.演奏履歴.行1 = "";
			this.譜面情報.演奏履歴.行2 = "";
			this.譜面情報.演奏履歴.行3 = "";
			this.譜面情報.演奏履歴.行4 = "";
			this.譜面情報.演奏履歴.行5 = "";
			this.譜面情報.演奏履歴.行6 = "";
			this.譜面情報.演奏履歴.行7 = "";
			this.譜面情報.Bpm = 120.0;
			this.譜面情報.Duration = 0;
			this.譜面情報.strBGMファイル名 = "";
			this.譜面情報.SongVol = CSound.DefaultSongVol;
			this.譜面情報.SongLoudnessMetadata = null;
			this.譜面情報.nデモBGMオフセット = 0;
			this.譜面情報.b譜面が存在する = new bool[(int)Difficulty.Total];
			this.譜面情報.b譜面分岐 = new bool[(int)Difficulty.Total];
			this.譜面情報.bPapaMamaSupport = new bool[(int)Difficulty.Total];
			this.譜面情報.b歌詞あり = false;
			this.譜面情報.nハイスコア = new int[(int)Difficulty.Total];
			this.譜面情報.nSecondScore = new int[(int)Difficulty.Total];
			this.譜面情報.nThirdScore = new int[(int)Difficulty.Total];
			this.譜面情報.strHiScorerName = new string[(int)Difficulty.Total];
			this.譜面情報.strSecondScorerName = new string[(int)Difficulty.Total];
			this.譜面情報.strThirdScorerName = new string[(int)Difficulty.Total];
			this.譜面情報.nCrown = new int[(int)Difficulty.Total];
			this.譜面情報.strSubTitle = "";
			this.譜面情報.nレベル = new int[(int)Difficulty.Total] { -1, -1, -1, -1, -1, -1, -1};
		}
	}
}
