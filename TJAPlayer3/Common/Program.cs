﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using FDK;
using System.Reflection;

namespace TJAPlayer3
{
	internal class Program
	{
		#region [ 二重起動チェック、DLL存在チェック ]
		//-----------------------------
		private static Mutex mutex二重起動防止用;

		private static bool tDLLの存在チェック( string strDll名, string str存在しないときに表示するエラー文字列jp, string str存在しないときに表示するエラー文字列en, bool bLoadDllCheck )
		{
			string str存在しないときに表示するエラー文字列 = ( CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ja" ) ?
				str存在しないときに表示するエラー文字列jp : str存在しないときに表示するエラー文字列en;
			if ( bLoadDllCheck )
			{
				IntPtr hModule = LoadLibrary( strDll名 );		// 実際にLoadDll()してチェックする
				if ( hModule == IntPtr.Zero )
				{
					MessageBox.Show( str存在しないときに表示するエラー文字列, "DTXMania runtime error", MessageBoxButtons.OK, MessageBoxIcon.Hand );
					return false;
				}
				FreeLibrary( hModule );
			}
			else
			{													// 単純にファイルの存在有無をチェックするだけ (プロジェクトで「参照」していたり、アンマネージドなDLLが暗黙リンクされるものはこちら)
				string path = Path.Combine( System.IO.Directory.GetCurrentDirectory(), strDll名 );
				if ( !File.Exists( path ) )
				{
					MessageBox.Show( str存在しないときに表示するエラー文字列, "DTXMania runtime error", MessageBoxButtons.OK, MessageBoxIcon.Hand );
					return false;
				}
			}
			return true;
		}
		private static bool tDLLの存在チェック( string strDll名, string str存在しないときに表示するエラー文字列jp, string str存在しないときに表示するエラー文字列en )
		{
			return true;
			//return tDLLの存在チェック( strDll名, str存在しないときに表示するエラー文字列jp, str存在しないときに表示するエラー文字列en, false );
		}

		#region [DllImport]
		[DllImport( "kernel32", CharSet = CharSet.Unicode, SetLastError = true )]
		internal static extern void FreeLibrary( IntPtr hModule );

		[DllImport( "kernel32", CharSet = CharSet.Unicode, SetLastError = true )]
		internal static extern IntPtr LoadLibrary( string lpFileName );
		#endregion
		//-----------------------------
		#endregion

		[STAThread] 
		private static void Main()
		{
			mutex二重起動防止用 = new Mutex( false, "TJAPlayer3-f" );

			if ( mutex二重起動防止用.WaitOne( 0, false ) )
			{
				string newLine = Environment.NewLine;
				bool bDLLnotfound = false;

				Trace.WriteLine( "Current Directory: " + Environment.CurrentDirectory );
				Trace.WriteLine( "EXEのあるフォルダ: " + Path.GetDirectoryName( Application.ExecutablePath ) );

				Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;


				#region [DLLの存在チェック]
				if ( !tDLLの存在チェック( "dll\\FDK.dll",
					"FDK.dll またはその依存するdllが存在しません。" + newLine + "DTXManiaをダウンロードしなおしてください。",
					"FDK.dll, or its depended DLL, is not found." + newLine + "Please download DTXMania again."
					) ) bDLLnotfound = true;
				if ( !tDLLの存在チェック( "dll\\SoundDecoder.dll",
					"SoundDecoder.dll またはその依存するdllが存在しません。" + newLine + "DTXManiaをダウンロードしなおしてください。",
					"SoundDecoder.dll, or its depended DLL, is not found." + newLine + "Please download DTXMania again."
					) ) bDLLnotfound = true;
				if ( !tDLLの存在チェック( TJAPlayer3.D3DXDLL,
					TJAPlayer3.D3DXDLL + " が存在しません。" + newLine + "DirectX Redist フォルダの DXSETUP.exe を実行し、" + newLine + "必要な DirectX ランタイムをインストールしてください。",
					TJAPlayer3.D3DXDLL + " is not found." + newLine + "Please execute DXSETUP.exe in \"DirectX Redist\" folder, to install DirectX runtimes required for DTXMania.",
					true
					) ) bDLLnotfound = true;
				if ( !tDLLの存在チェック( "dll\\bass.dll",
					"bass.dll が存在しません。" + newLine + "DTXManiaをダウンロードしなおしてください。",
					"baas.dll is not found." + newLine + "Please download DTXMania again."
					) ) bDLLnotfound = true;
				if ( !tDLLの存在チェック( "dll\\Bass.Net.dll",
					"Bass.Net.dll が存在しません。" + newLine + "DTXManiaをダウンロードしなおしてください。",
					"Bass.Net.dll is not found." + newLine + "Please download DTXMania again."
					) ) bDLLnotfound = true;
				if ( !tDLLの存在チェック( "dll\\bassmix.dll",
					"bassmix.dll を読み込めません。bassmix.dll か bass.dll が存在しません。" + newLine + "DTXManiaをダウンロードしなおしてください。",
					"bassmix.dll is not loaded. bassmix.dll or bass.dll must not exist." + newLine + "Please download DTXMania again."
					) ) bDLLnotfound = true;
				if ( !tDLLの存在チェック( "dll\\bassasio.dll",
					"bassasio.dll を読み込めません。bassasio.dll か bass.dll が存在しません。" + newLine + "DTXManiaをダウンロードしなおしてください。",
					"bassasio.dll is not loaded. bassasio.dll or bass.dll must not exist." + newLine + "Please download DTXMania again."
					) ) bDLLnotfound = true;
				if ( !tDLLの存在チェック( "dll\\basswasapi.dll",
					"basswasapi.dll を読み込めません。basswasapi.dll か bass.dll が存在しません。" + newLine + "DTXManiaをダウンロードしなおしてください。",
					"basswasapi.dll is not loaded. basswasapi.dll or bass.dll must not exist." + newLine + "Please download DTXMania again."
					) ) bDLLnotfound = true;
				if ( !tDLLの存在チェック( "dll\\bass_fx.dll",
					"bass_fx.dll を読み込めません。bass_fx.dll か bass.dll が存在しません。" + newLine + "DTXManiaをダウンロードしなおしてください。",
					"bass_fx.dll is not loaded. bass_fx.dll or bass.dll must not exist." + newLine + "Please download DTXMania again."
					) ) bDLLnotfound = true;
				if ( !tDLLの存在チェック( "dll\\DirectShowLib-2005.dll",
					"DirectShowLib-2005.dll が存在しません。" + newLine + "DTXManiaをダウンロードしなおしてください。",
					"DirectShowLib-2005.dll is not found." + newLine + "Please download DTXMania again."
					) ) bDLLnotfound = true;
				#endregion
				if ( !bDLLnotfound )
				{
#if DEBUG && TEST_ENGLISH
					Thread.CurrentThread.CurrentCulture = new CultureInfo( "en-US" );
#endif

					DWM.EnableComposition( false );	// Disable AeroGrass temporally

					// BEGIN #23670 2010.11.13 from: キャッチされない例外は放出せずに、ログに詳細を出力する。
					// BEGIM #24606 2011.03.08 from: DEBUG 時は例外発生箇所を直接デバッグできるようにするため、例外をキャッチしないようにする。
					//2020.04.15 Mr-Ojii DEBUG 時も例外をキャッチするようにした。
					try
					{
						using ( var mania = new TJAPlayer3() )
							mania.Run();

						Trace.WriteLine( "" );
						Trace.WriteLine( "遊んでくれてありがとう！" );
					}
					catch( Exception e )
					{
						Trace.WriteLine( "" );
						Trace.Write( e.ToString() );
						Trace.WriteLine( "" );
						Trace.WriteLine( "エラーだゴメン！（涙" );
						AssemblyName asmApp = Assembly.GetExecutingAssembly().GetName();
						MessageBox.Show( "エラーが発生しました。\n" +
							"原因がわからない場合は、以下のエラー文を添えて、エラー送信フォームに送信してください。\n" + 
							e.ToString(), asmApp.Name + " Ver." + asmApp.Version.ToString().Substring(0, asmApp.Version.ToString().Length - 2) + " Error", MessageBoxButtons.OK, MessageBoxIcon.Error );    // #23670 2011.2.28 yyagi to show error dialog
						DialogResult result = MessageBox.Show("エラー送信フォームを開きますか?(ブラウザが起動します)\n",
							asmApp.Name + " Ver." + asmApp.Version.ToString().Substring(0, asmApp.Version.ToString().Length - 2),
							MessageBoxButtons.YesNo,
							MessageBoxIcon.Asterisk);
						if (result == DialogResult.Yes)
						{
							DialogResult result2 = MessageBox.Show("GitHubのエラー送信フォームを開きますか?※GitHubアカウントが必要です。\n\nGoogleのエラー送信フォームを開きますか?※アカウントの必要なし\n\nGitHubのからのエラー報告のほうが「Mr.おじい」が早くエラーの存在に気づけます。\n(Y:GitHub / N:Google)",
								asmApp.Name + " Ver." + asmApp.Version.ToString().Substring(0, asmApp.Version.ToString().Length - 2),
								MessageBoxButtons.YesNo,
								MessageBoxIcon.Asterisk);

							if (result2 == DialogResult.Yes)
							{
								Process.Start("https://github.com/Mr-Ojii/TJAPlayer3-f/issues/new?body=エラー文(TJAPlayer3-fから開いた場合は自動入力されます)%0D%0A" +
									System.Web.HttpUtility.UrlEncode(e.ToString()) +
									"%0D%0A" +
									"%0D%0A" +
									"使用しているスキン名・バージョン%0D%0A" +
									"%0D%0A" +
									"%0D%0A" +
									"バグを引き起こすまでの手順を書いてください%0D%0A" +
									"%0D%0A" +
									"%0D%0A" +
									"再生していた譜面(.tja)または画面%0D%0A" +
									"%0D%0A" +
									"%0D%0A" +
									"使用しているOS%0D%0A" +
									"%0D%0A" +
									"%0D%0A" +
									"不具合の内容%0D%0A" +
									"%0D%0A" +
									"%0D%0A" +
									"(追加情報を自由に書いてください(任意))%0D%0A");
							}
							else {
								Process.Start("https://docs.google.com/forms/d/e/1FAIpQLSffkhp-3kDJIZH23xMoweik5sAgy2UyaIkEQd1khn9DuR_RWg/viewform?entry.1025217940=" +
									System.Web.HttpUtility.UrlEncode(e.ToString()));
							}
						}
					}
					// END #24606 2011.03.08 from
					// END #23670 2010.11.13 from

					if ( Trace.Listeners.Count > 1 )
						Trace.Listeners.RemoveAt( 1 );
				}

				// BEGIN #24615 2011.03.09 from: Mutex.WaitOne() が true を返した場合は、Mutex のリリースが必要である。

				mutex二重起動防止用.ReleaseMutex();
				mutex二重起動防止用 = null;

				// END #24615 2011.03.09 from
			}
			else		// DTXManiaが既に起動中
			{
				//多重起動の回数の記録のためだけにレジストリを使っています。2020.04.07 Mr-Ojii
				Microsoft.Win32.RegistryKey regkey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"Software\tjaplayer3f");
				int times = (int)regkey.GetValue("Times", 0);
				times++;
				if (times >= 20) {
					times = 0;
				}
				regkey.SetValue("Times", times);
				string hyoujimoji = "一応の文字";
				if (times >= 10)
				{
					hyoujimoji = "何回も言わせるんじゃないよ。\nこのスカポンタン。\n多重起動はできないって言ってるでしょ。";
				}
				else if(times >= 5)
				{
					hyoujimoji = "何回言えばいいんでしょうか。\n多重起動はできませんよ。";
				}
				else
				{
					hyoujimoji = "多重起動はできないよ。";
				}
				MessageBox.Show(hyoujimoji, "注意", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}
	}
}
