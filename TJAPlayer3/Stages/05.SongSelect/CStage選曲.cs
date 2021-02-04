using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using FDK;
using DiscordRPC;

using Rectangle = System.Drawing.Rectangle;

namespace TJAPlayer3
{
	internal class CStage選曲 : CStage
	{
		// プロパティ
		public int[] n確定された曲の難易度
		{
			get;
			private set;
		}
		public string str確定された曲のジャンル
		{
			get;
			private set;
		}
		public Cスコア r確定されたスコア
		{
			get;
			internal set;
		}
		public C曲リストノード r確定された曲
		{
			get;
			private set;
		}
		public int[] n現在選択中の曲の難易度
		{
			get
			{
				return this.act曲リスト.n現在選択中の曲の難易度レベル;
			}
		}

		// コンストラクタ
		public CStage選曲()
		{
			base.eStageID = CStage.EStage.SongSelect;
			base.eフェーズID = CStage.Eフェーズ.共通_通常状態;
			base.b活性化してない = true;
			base.list子Activities.Add(this.actFIFO = new CActFIFOBlack());
			base.list子Activities.Add(this.actFIfrom結果画面 = new CActFIFOBlack());
			//base.list子Activities.Add( this.actFOtoNowLoading = new CActFIFOBlack() );
			base.list子Activities.Add(this.actFOtoNowLoading = new CActFIFOStart());
			base.list子Activities.Add(this.act曲リスト = new CActSelect曲リスト());
			base.list子Activities.Add(this.act難易度選択画面 = new CActSelect難易度選択画面());
			base.list子Activities.Add(this.act演奏履歴パネル = new CActSelect演奏履歴パネル());
			base.list子Activities.Add(this.actPresound = new CActSelectPresound());
			base.list子Activities.Add(this.actSortSongs = new CActSortSongs());
			base.list子Activities.Add(this.actPlayOption = new CActSelectPlayOption());
			base.list子Activities.Add(this.actChangeSE = new CActSelectChangeSE());
		}


		// メソッド

		public void t選択曲変更通知()
		{
			this.actPresound.t選択曲が変更された();
		}

		// CStage 実装

		/// <summary>
		/// 曲リストをリセットする
		/// </summary>
		/// <param name="cs"></param>
		public void Refresh(CSongs管理 cs, bool bRemakeSongTitleBar)
		{
			this.act曲リスト.Refresh(cs, bRemakeSongTitleBar);
		}

		public override void On活性化()
		{
			Trace.TraceInformation("選曲ステージを活性化します。");
			Trace.Indent();
			try
			{
				this.n確定された曲の難易度 = new int[4];
				this.eFadeOut完了時の戻り値 = E戻り値.継続;
				this.bBGM再生済み = false;
				for (int i = 0; i < 4; i++)
					this.ctキー反復用[i] = new CCounter(0, 0, 0, TJAPlayer3.Timer);

				//this.act難易度選択画面.bIsDifficltSelect = true;
				base.On活性化();

				現在の選曲画面状況 = E選曲画面.通常;
				完全に選択済み = false;
				// Discord Presenceの更新

				TJAPlayer3.DiscordClient?.SetPresence(new RichPresence()
				{
					Details = "",
					State = "SongSelect",
					Timestamps = new Timestamps(TJAPlayer3.StartupTime),
					Assets = new Assets()
					{
						LargeImageKey = TJAPlayer3.LargeImageKey,
						LargeImageText = TJAPlayer3.LargeImageText,
					}
				});
			}
			finally
			{
				TJAPlayer3.ConfigIni.eScrollMode = EScrollMode.Normal;
				TJAPlayer3.ConfigIni.bスクロールモードを上書き = false;
				Trace.TraceInformation("選曲ステージの活性化を完了しました。");
				Trace.Unindent();
			}
		}

		public override void On非活性化()
		{
			Trace.TraceInformation( "選曲ステージを非活性化します。" );
			Trace.Indent();
			try
			{
				for( int i = 0; i < 4; i++ )
				{
					this.ctキー反復用[ i ] = null;
				}
				base.On非活性化();
			}
			finally
			{
				Trace.TraceInformation( "選曲ステージの非活性化を完了しました。" );
				Trace.Unindent();
			}
		}
		public override void OnManagedリソースの作成()
		{
			if( !base.b活性化してない )
			{
				if (TJAPlayer3.Tx.SongSelect_Background != null)
					this.ct背景スクロール用タイマー = new CCounter(0, TJAPlayer3.Tx.SongSelect_Background.szTextureSize.Width, 30, TJAPlayer3.Timer);
				this.ctカウントダウン用タイマー = new CCounter(0, 100, 1000, TJAPlayer3.Timer);
				this.ct難易度選択画面IN用タイマー = new CCounter(0, 750, 1, TJAPlayer3.Timer);
				this.ct難易度選択画面INバー拡大用タイマー = new CCounter(0, 750, 1, TJAPlayer3.Timer);
				this.ct難易度選択画面OUT用タイマー = new CCounter(0, 500, 1, TJAPlayer3.Timer);
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if( !base.b活性化してない )
			{
				base.OnManagedリソースの解放();
			}
		}
		public override int On進行描画()
		{
			if( !base.b活性化してない )
			{
			this.ct背景スクロール用タイマー?.t進行Loop();
			this.ctカウントダウン用タイマー.t進行Loop();
				#region [ 初めての進行描画 ]
				//---------------------
				if ( base.b初めての進行描画 )
				{
					this.ct登場時アニメ用共通 = new CCounter( 0, 100, 3, TJAPlayer3.Timer );
					if( TJAPlayer3.r直前のステージ == TJAPlayer3.stageResult )
					{
						this.actFIfrom結果画面.tFadeIn開始();
						base.eフェーズID = CStage.Eフェーズ.選曲_結果画面からのFadeIn;
					}
					else
					{
						this.actFIFO.tFadeIn開始();
						base.eフェーズID = CStage.Eフェーズ.共通_FadeIn;
					}
					this.t選択曲変更通知();
					base.b初めての進行描画 = false;
				}
				//---------------------
				#endregion

				this.ct登場時アニメ用共通.t進行();

				if( TJAPlayer3.Tx.SongSelect_Background != null )
					TJAPlayer3.Tx.SongSelect_Background.t2D描画( TJAPlayer3.app.Device, 0, 0 );

				if( act曲リスト.r現在選択中の曲 != null )
				{
					int nGenreBack = 0;
					if (act曲リスト.r現在選択中の曲.eNodeType == C曲リストノード.ENodeType.BOX || act曲リスト.r現在選択中の曲.eNodeType == C曲リストノード.ENodeType.SCORE)
					{
						nGenreBack = TJAPlayer3.Skin.nStrジャンルtoNum(act曲リスト.r現在選択中の曲.strジャンル);
					}
					else if (act曲リスト.r現在選択中の曲.eNodeType == C曲リストノード.ENodeType.BACKBOX) {
						nGenreBack = TJAPlayer3.Skin.nStrジャンルtoNum(act曲リスト.r現在選択中の曲.r親ノード.strジャンル);
					}
					if (TJAPlayer3.Tx.SongSelect_GenreBack[nGenreBack] != null )
					{
						for (int i = 0; i < (1280 / TJAPlayer3.Tx.SongSelect_Background.szTextureSize.Width) + 2; i++)
						{
							if ( TJAPlayer3.Tx.SongSelect_GenreBack[nGenreBack] != null&& ct背景スクロール用タイマー != null)
							{
								TJAPlayer3.Tx.SongSelect_GenreBack[nGenreBack].t2D描画(TJAPlayer3.app.Device, -ct背景スクロール用タイマー.n現在の値 + TJAPlayer3.Tx.SongSelect_Background.szTextureSize.Width * i, 0);
							}
						}
					}
				}

				if (現在の選曲画面状況 != E選曲画面.難易度選択)
				{
					this.act曲リスト.On進行描画();
					this.act難易度選択画面.裏表示 = false;
					this.act難易度選択画面.裏カウント[0] = 0;
				}

				if (現在の選曲画面状況 == E選曲画面.難易度選択In)
				{
					this.ct難易度選択画面IN用タイマー.t進行();
					if (this.ct難易度選択画面IN用タイマー.b終了値に達した)
					{
						this.ct難易度選択画面INバー拡大用タイマー.t進行();
						if (this.ct難易度選択画面INバー拡大用タイマー.b終了値に達した)
						{

							現在の選曲画面状況 = E選曲画面.難易度選択;
						}
					}
					else
					{
						this.ct難易度選択画面INバー拡大用タイマー.n現在の値 = 0;
						this.ct難易度選択画面INバー拡大用タイマー.t時間Reset();
					}

					if (TJAPlayer3.Tx.Difficulty_Center_Bar != null)
					{
						//Bar_Centerの拡大アニメーション
						int width = Math.Max(Math.Min(
							TJAPlayer3.Skin.Difficulty_Bar_Center_X_WH_WH_Y_Y[1] +
							(int)((TJAPlayer3.Skin.Difficulty_Bar_Center_X_WH_WH_Y_Y[3] - TJAPlayer3.Skin.Difficulty_Bar_Center_X_WH_WH_Y_Y[1]) * (((double)ct難易度選択画面INバー拡大用タイマー.n現在の値 * 3) / ct難易度選択画面INバー拡大用タイマー.n終了値)),
							TJAPlayer3.Skin.Difficulty_Bar_Center_X_WH_WH_Y_Y[3]), TJAPlayer3.Skin.Difficulty_Bar_Center_X_WH_WH_Y_Y[1]);

						int height = Math.Max(Math.Min(
							TJAPlayer3.Skin.Difficulty_Bar_Center_X_WH_WH_Y_Y[2] + (int)((TJAPlayer3.Skin.Difficulty_Bar_Center_X_WH_WH_Y_Y[4] - TJAPlayer3.Skin.Difficulty_Bar_Center_X_WH_WH_Y_Y[2]) * (((double)ct難易度選択画面INバー拡大用タイマー.n現在の値 * 2 - ct難易度選択画面INバー拡大用タイマー.n終了値 / 2) / ct難易度選択画面INバー拡大用タイマー.n終了値)),
							TJAPlayer3.Skin.Difficulty_Bar_Center_X_WH_WH_Y_Y[4]), TJAPlayer3.Skin.Difficulty_Bar_Center_X_WH_WH_Y_Y[2]);

						int ydiff = Math.Min(Math.Max(TJAPlayer3.Skin.Difficulty_Bar_Center_X_WH_WH_Y_Y[5] + (int)((TJAPlayer3.Skin.Difficulty_Bar_Center_X_WH_WH_Y_Y[6] - TJAPlayer3.Skin.Difficulty_Bar_Center_X_WH_WH_Y_Y[5]) * (((double)ct難易度選択画面INバー拡大用タイマー.n現在の値 * 2 - ct難易度選択画面INバー拡大用タイマー.n終了値 / 2) / ct難易度選択画面INバー拡大用タイマー.n終了値)), TJAPlayer3.Skin.Difficulty_Bar_Center_X_WH_WH_Y_Y[6]), TJAPlayer3.Skin.Difficulty_Bar_Center_X_WH_WH_Y_Y[5]);

						int xdiff = TJAPlayer3.Skin.Difficulty_Bar_Center_X_WH_WH_Y_Y[0] - width / 2;

						int wh = Math.Min(TJAPlayer3.Tx.Difficulty_Center_Bar.szTextureSize.Width / 3, TJAPlayer3.Tx.Difficulty_Center_Bar.szTextureSize.Height / 3);

						for (int i = 0; i < width / wh + 1; i++)
						{
							for (int j = 0; j < height / wh + 1; j++)
							{
								if (i == 0 && j == 0)
								{
									TJAPlayer3.Tx.Difficulty_Center_Bar.t2D描画(TJAPlayer3.app.Device, i * wh + xdiff, j * wh + ydiff, new Rectangle(0, 0, wh, wh));
								}
								else if (i == 0 && j == (height / wh))
								{
									TJAPlayer3.Tx.Difficulty_Center_Bar.t2D描画(TJAPlayer3.app.Device, i * wh + xdiff, j * wh - (wh - height % wh) + ydiff, new Rectangle(0, wh*2, wh, wh));
								}
								else if (i == (width / wh) && j == 0)
								{
									TJAPlayer3.Tx.Difficulty_Center_Bar.t2D描画(TJAPlayer3.app.Device, i * wh - (wh - width % wh) + xdiff, j * wh + ydiff, new Rectangle(wh*2, 0, wh, wh));
								}
								else if (i == (width / wh) && j == (height / wh))
								{
									TJAPlayer3.Tx.Difficulty_Center_Bar.t2D描画(TJAPlayer3.app.Device, i * wh - (wh - width % wh) + xdiff, j * wh - (wh - height % wh) + ydiff, new Rectangle(wh*2, wh*2, wh, wh));
								}
								else if (i == 0)
								{
									TJAPlayer3.Tx.Difficulty_Center_Bar.t2D描画(TJAPlayer3.app.Device, i * wh + xdiff, j * wh + ydiff, new Rectangle(0, wh, wh, wh));
								}
								else if (j == 0)
								{
									TJAPlayer3.Tx.Difficulty_Center_Bar.t2D描画(TJAPlayer3.app.Device, i * wh + xdiff, j * wh + ydiff, new Rectangle(wh, 0, wh, wh));
								}
								else if (i == (width / wh))
								{
									TJAPlayer3.Tx.Difficulty_Center_Bar.t2D描画(TJAPlayer3.app.Device, i * wh - (wh - width % wh) + xdiff, j * wh + ydiff, new Rectangle(wh*2, wh, wh, wh));
								}
								else if (j == (height / wh))
								{
									TJAPlayer3.Tx.Difficulty_Center_Bar.t2D描画(TJAPlayer3.app.Device, i * wh + xdiff, j * wh - (wh - height % wh) + ydiff, new Rectangle(wh, wh*2, wh, wh));
								}
								else
								{
									TJAPlayer3.Tx.Difficulty_Center_Bar.t2D描画(TJAPlayer3.app.Device, i * wh + xdiff, j * wh + ydiff, new Rectangle(wh, wh, wh, wh));
								}
							}
						}
					}

					int xAnime = Math.Min((int)(200 * Math.Max((((double)ct難易度選択画面INバー拡大用タイマー.n現在の値 * 3) / ct難易度選択画面INバー拡大用タイマー.n終了値),0)),200);
					int yAnime = Math.Min((int)(60 * Math.Max((((double)ct難易度選択画面INバー拡大用タイマー.n現在の値 * 2 - ct難易度選択画面INバー拡大用タイマー.n終了値 / 2) / ct難易度選択画面INバー拡大用タイマー.n終了値),0)),60);

					if (this.act曲リスト.ttk選択している曲のサブタイトル != null)
					{
						this.act曲リスト.サブタイトルtmp.t2D拡大率考慮描画(TJAPlayer3.app.Device, CTexture.RefPnt.Down, 707 + (this.act曲リスト.サブタイトルtmp.szTextureSize.Width / 2) + xAnime, TJAPlayer3.Skin.SongSelect_Overall_Y + 430 - yAnime);
						if (this.act曲リスト.ttk選択している曲の曲名 != null)
						{
							this.act曲リスト.タイトルtmp.t2D描画(TJAPlayer3.app.Device, 750 + xAnime, TJAPlayer3.Skin.SongSelect_Overall_Y + 23 - yAnime);
						}
					}
					else if (this.act曲リスト.ttk選択している曲の曲名 != null)
					{
						this.act曲リスト.タイトルtmp.t2D描画(TJAPlayer3.app.Device, 750 + xAnime, TJAPlayer3.Skin.SongSelect_Overall_Y + 23 - yAnime);
					}

				}
				else
				{
					this.ct難易度選択画面IN用タイマー.n現在の値 = 0;
					this.ct難易度選択画面IN用タイマー.t時間Reset();
				}

				if (現在の選曲画面状況 == E選曲画面.難易度選択Out)
				{
					this.ct難易度選択画面OUT用タイマー.t進行();
					if (this.ct難易度選択画面OUT用タイマー.b終了値に達した)	{
						現在の選曲画面状況 = E選曲画面.通常;
					}
				}
				else
				{
					this.ct難易度選択画面OUT用タイマー.n現在の値=0;
					this.ct難易度選択画面OUT用タイマー.t時間Reset();
				}


				//this.actPreimageパネル.On進行描画();
				//	this.bIsEnumeratingSongs = !this.actPreimageパネル.bIsPlayingPremovie;				// #27060 2011.3.2 yyagi: #PREMOVIE再生中は曲検索を中断する
				if (現在の選曲画面状況 == E選曲画面.難易度選択)
				{
                    this.act難易度選択画面.On進行描画();
				}

				if( TJAPlayer3.Tx.SongSelect_Header != null )
					TJAPlayer3.Tx.SongSelect_Header.t2D描画( TJAPlayer3.app.Device, 0, 0 );

				if( TJAPlayer3.Tx.SongSelect_Footer != null )
					TJAPlayer3.Tx.SongSelect_Footer.t2D描画( TJAPlayer3.app.Device, 0, 720 - TJAPlayer3.Tx.SongSelect_Footer.szTextureSize.Height );

				#region ネームプレート
				for (int i = 0; i < TJAPlayer3.ConfigIni.nPlayerCount; i++)
				{
					if (TJAPlayer3.Tx.NamePlate[i] != null)
					{
						TJAPlayer3.Tx.NamePlate[i].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.SongSelect_NamePlate_X[i], TJAPlayer3.Skin.SongSelect_NamePlate_Y[i]);
					}
				}
				#endregion

				#region[ 下部テキスト ]
				if (TJAPlayer3.Tx.SongSelect_Auto != null)
				{
					if (TJAPlayer3.ConfigIni.b太鼓パートAutoPlay[0])
					{
						TJAPlayer3.Tx.SongSelect_Auto.t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.SongSelect_Auto_X[0], TJAPlayer3.Skin.SongSelect_Auto_Y[0]);
					}
					if (TJAPlayer3.ConfigIni.nPlayerCount > 1 && TJAPlayer3.ConfigIni.b太鼓パートAutoPlay[1])
					{
						TJAPlayer3.Tx.SongSelect_Auto.t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.SongSelect_Auto_X[1], TJAPlayer3.Skin.SongSelect_Auto_Y[1]);
					}
				}
				if (TJAPlayer3.ConfigIni.eGameMode == EGame.完走叩ききりまショー)
					TJAPlayer3.act文字コンソール.tPrint(0, 0, C文字コンソール.Eフォント種別.白, "GAME: SURVIVAL");
				if (TJAPlayer3.ConfigIni.eGameMode == EGame.完走叩ききりまショー激辛)
					TJAPlayer3.act文字コンソール.tPrint(0, 0, C文字コンソール.Eフォント種別.白, "GAME: SURVIVAL HARD");
				if (TJAPlayer3.ConfigIni.eGameMode == EGame.特訓モード)
					TJAPlayer3.act文字コンソール.tPrint(0, 0, C文字コンソール.Eフォント種別.白, "GAME: TRAINING MODE");
				if (TJAPlayer3.ConfigIni.bSuperHard)
					TJAPlayer3.act文字コンソール.tPrint(0, 16, C文字コンソール.Eフォント種別.赤, "SUPER HARD MODE : ON");
				if (TJAPlayer3.ConfigIni.eScrollMode == EScrollMode.BMSCROLL)
					TJAPlayer3.act文字コンソール.tPrint(0, 32, C文字コンソール.Eフォント種別.赤, "BMSCROLL : ON");
				else if (TJAPlayer3.ConfigIni.eScrollMode == EScrollMode.HBSCROLL)
					TJAPlayer3.act文字コンソール.tPrint(0, 32, C文字コンソール.Eフォント種別.赤, "HBSCROLL : ON");
				else if (TJAPlayer3.ConfigIni.eScrollMode == EScrollMode.REGULSPEED)
					TJAPlayer3.act文字コンソール.tPrint(0, 32, C文字コンソール.Eフォント種別.赤, "Reg.Speed : " + TJAPlayer3.ConfigIni.nRegSpeedBPM.ToString());
				#endregion

				if (TJAPlayer3.ConfigIni.bEnableCountdownTimer && TJAPlayer3.Tx.SongSelect_Counter_Back[0] != null && TJAPlayer3.Tx.SongSelect_Counter_Back[1] != null && TJAPlayer3.Tx.SongSelect_Counter_Num[0] != null && TJAPlayer3.Tx.SongSelect_Counter_Num[1] != null)
				{
					This_counter = (100 - this.ctカウントダウン用タイマー.n現在の値);
					int dotinum = 1;
					if (This_counter >= 10)
						dotinum = 0;
					TJAPlayer3.Tx.SongSelect_Counter_Back[dotinum].t2D描画(TJAPlayer3.app.Device, 880, 0);
					for (int countdig = 0; countdig < This_counter.ToString().Length; countdig++)
						TJAPlayer3.Tx.SongSelect_Counter_Num[dotinum].t2D描画(TJAPlayer3.app.Device, (int)(((countdig + (This_counter.ToString().Length - 1) / 2.0) - (This_counter.ToString().Length - 1)) * 48.0) + TJAPlayer3.Skin.SongSelect_Counter_XY[0], TJAPlayer3.Skin.SongSelect_Counter_XY[1], new Rectangle((TJAPlayer3.Tx.SongSelect_Counter_Num[dotinum].szTextureSize.Width / 10) * (This_counter / (int)Math.Pow(10, This_counter.ToString().Length - countdig - 1) % 10 ), 0, TJAPlayer3.Tx.SongSelect_Counter_Num[dotinum].szTextureSize.Width / 10, TJAPlayer3.Tx.SongSelect_Counter_Num[dotinum].szTextureSize.Height));
				}

				if (this.act曲リスト.n現在選択中の曲の難易度レベル[0] != (int)Difficulty.Dan)
					this.actPresound.On進行描画();

				this.act演奏履歴パネル.On進行描画();

				if (act曲リスト.r現在選択中の曲 != null && TJAPlayer3.Tx.SongSelect_Difficulty != null)
					TJAPlayer3.Tx.SongSelect_Difficulty.t2D描画(TJAPlayer3.app.Device, 830, 40, new Rectangle(0, 70 * this.n現在選択中の曲の難易度[0], 260, 70));

				if( !this.bBGM再生済み && ( base.eフェーズID == CStage.Eフェーズ.共通_通常状態 ) )
				{
					TJAPlayer3.Skin.bgm選曲画面.t再生する();
					this.bBGM再生済み = true;
				}

				if (現在の選曲画面状況 == E選曲画面.Dan選択) {
					if (TJAPlayer3.Tx.Difficulty_Dan_Box != null && TJAPlayer3.Tx.Difficulty_Dan_Box_Selecting != null) {
						TJAPlayer3.Tx.Difficulty_Dan_Box.t2D描画(TJAPlayer3.app.Device, 0, 0);
						TJAPlayer3.Tx.Difficulty_Dan_Box_Selecting.t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Tx.Difficulty_Dan_Box_Selecting.szTextureSize.Width / 2 * DanSelectingRow, 0,new Rectangle(TJAPlayer3.Tx.Difficulty_Dan_Box_Selecting.szTextureSize.Width / 2 * DanSelectingRow, 0, TJAPlayer3.Tx.Difficulty_Dan_Box_Selecting.szTextureSize.Width / 2, TJAPlayer3.Tx.Difficulty_Dan_Box_Selecting.szTextureSize.Height));
					}
				}

				if ((act曲リスト.r現在選択中の曲 != null) && TJAPlayer3.ConfigIni.bTCClikeStyle && act曲リスト.r現在選択中の曲.eNodeType == C曲リストノード.ENodeType.SCORE)
					this.act曲リスト.tアイテム数の描画();

				this.actChangeSE.On進行描画();
				this.actPlayOption.On進行描画();

				// キー入力
				if ( base.eフェーズID == CStage.Eフェーズ.共通_通常状態 )
                {
					if (popupbool[0])
					{
						//クイックコンフィグの呼び出し
						TJAPlayer3.Skin.sound決定音.t再生する();
						this.actPlayOption.tActivatePopupMenu(0);
						popupbool[0] = false;
						popupbool[1] = false;
					}
					else if (popupbool[1])
					{
						TJAPlayer3.Skin.sound決定音.t再生する();
						this.actPlayOption.tActivatePopupMenu(1);
						popupbool[0] = false;
						popupbool[1] = false;
					}

					#region[もし段位道場の確認状態だったら]
					if (現在の選曲画面状況 == E選曲画面.Dan選択)
					{//2020.05.25 Mr-Ojii 段位道場の確認を追加
						#region [ ESC ]
						if (TJAPlayer3.Input管理.Keyboard.bキーが押された((int)SlimDXKeys.Key.Escape) && (this.act曲リスト.r現在選択中の曲 != null))
						{
							TJAPlayer3.Skin.sound取消音.t再生する();
							現在の選曲画面状況 = E選曲画面.通常;
						}
						#endregion
						#region[Decide]
						if (((TJAPlayer3.Pad.b押された(EPad.LRed) || TJAPlayer3.Pad.b押された(EPad.RRed)) || (TJAPlayer3.Pad.b押された(EPad.LRed2P) || TJAPlayer3.Pad.b押された(EPad.RRed2P)) && TJAPlayer3.ConfigIni.nPlayerCount >= 2 ||
										(TJAPlayer3.ConfigIni.bEnterがキー割り当てのどこにも使用されていない && TJAPlayer3.Input管理.Keyboard.bキーが押された((int)SlimDXKeys.Key.Return))))
						{
							if (DanSelectingRow == 1)
							{
								if (TJAPlayer3.Skin.sound曲決定音.b読み込み成功)
									TJAPlayer3.Skin.sound曲決定音.t再生する();
								else
									TJAPlayer3.Skin.sound決定音.t再生する();
								this.t曲を選択する();
								現在の選曲画面状況 = E選曲画面.通常;
							}
							else
							{
								TJAPlayer3.Skin.sound取消音.t再生する();
								現在の選曲画面状況 = E選曲画面.通常;
							}
						}
						#endregion
						#region [ Up ]
						if (TJAPlayer3.Pad.b押された(EPad.LBlue) || TJAPlayer3.Input管理.Keyboard.bキーが押された((int)SlimDXKeys.Key.LeftArrow))
						{
							TJAPlayer3.Skin.soundカーソル移動音.t再生する();
							DanSelectingRow = 0;
						}
						#endregion
						#region [ Down ]
						if (TJAPlayer3.Pad.b押された(EPad.RBlue) || TJAPlayer3.Input管理.Keyboard.bキーが押された((int)SlimDXKeys.Key.RightArrow))
						{
							TJAPlayer3.Skin.soundカーソル移動音.t再生する();
							DanSelectingRow = 1;
						}
						#endregion
					}
					#endregion			
					#region[難易度選択画面のキー入力]
					else if (現在の選曲画面状況 == E選曲画面.難易度選択)
					{//2020.06.02 Mr-Ojii 難易度選択画面の追加
						if (!this.actSortSongs.bIsActivePopupMenu)
						{
							#region [ ESC ]
							if (TJAPlayer3.Input管理.Keyboard.bキーが押された((int)SlimDXKeys.Key.Escape) && (this.act曲リスト.r現在選択中の曲 != null))
							{
								if (this.actChangeSE.bIsActive[0])
									this.actChangeSE.tDeativateChangeSE(0);
								else if (this.actPlayOption.bIsActive[0])
									this.actPlayOption.tDeativatePopupMenu(0);
								else if (this.act難易度選択画面.選択済み[0] && TJAPlayer3.ConfigIni.nPlayerCount >= 2)
								{
									if (this.actChangeSE.bIsActive[1])
										this.actChangeSE.tDeativateChangeSE(1);
									else if (this.actPlayOption.bIsActive[1])
										this.actPlayOption.tDeativatePopupMenu(1);
									else
										難易度から選曲へ戻る();
								}
								else
									難易度から選曲へ戻る();
							}
							#endregion
							#region [ Shift-F1: CONFIG画面 ]
							if ((TJAPlayer3.Input管理.Keyboard.bキーが押されている((int)SlimDXKeys.Key.RightShift) || TJAPlayer3.Input管理.Keyboard.bキーが押されている((int)SlimDXKeys.Key.LeftShift)) &&
								TJAPlayer3.Input管理.Keyboard.bキーが押された((int)SlimDXKeys.Key.F1))
							{   // [SHIFT] + [F1] CONFIG
								this.GotoConfig();
								return 0;
							}
							#endregion
							#region [ F3 1PオートON/OFF ]
							if (TJAPlayer3.Input管理.Keyboard.bキーが押された((int)SlimDXKeys.Key.F3))
							{
								TJAPlayer3.Skin.sound変更音.t再生する();
								TJAPlayer3.ConfigIni.b太鼓パートAutoPlay[0] = !TJAPlayer3.ConfigIni.b太鼓パートAutoPlay[0];
							}
							#endregion
							#region [ F4 2PオートON/OFF ]
							if (TJAPlayer3.Input管理.Keyboard.bキーが押された((int)SlimDXKeys.Key.F4))
							{
								if (TJAPlayer3.ConfigIni.nPlayerCount > 1)
								{
									TJAPlayer3.Skin.sound変更音.t再生する();
									TJAPlayer3.ConfigIni.b太鼓パートAutoPlay[1] = !TJAPlayer3.ConfigIni.b太鼓パートAutoPlay[1];
								}
							}
							#endregion
							#region [ F5 スーパーハード ]
							if (TJAPlayer3.Input管理.Keyboard.bキーが押された((int)SlimDXKeys.Key.F5))
							{
								TJAPlayer3.Skin.sound変更音.t再生する();
								TJAPlayer3.ConfigIni.bSuperHard = !TJAPlayer3.ConfigIni.bSuperHard;
							}
							#endregion
							#region [ F6 SCROLL ]
							if (TJAPlayer3.Input管理.Keyboard.bキーが押された((int)SlimDXKeys.Key.F6))
							{
								TJAPlayer3.Skin.sound変更音.t再生する();
								TJAPlayer3.ConfigIni.bスクロールモードを上書き = true;
								switch ((int)TJAPlayer3.ConfigIni.eScrollMode)
								{
									case 0:
										TJAPlayer3.ConfigIni.eScrollMode = EScrollMode.BMSCROLL;
										break;
									case 1:
										TJAPlayer3.ConfigIni.eScrollMode = EScrollMode.HBSCROLL;
										break;
									case 2:
										TJAPlayer3.ConfigIni.eScrollMode = EScrollMode.REGULSPEED;
										break;
									case 3:
										TJAPlayer3.ConfigIni.eScrollMode = EScrollMode.Normal;
										TJAPlayer3.ConfigIni.bスクロールモードを上書き = false;
										break;
								}
							}
							#endregion
							#region[ F7 Reg.Speed DOWN ]
							this.ctキー反復用.Left.tキー反復(TJAPlayer3.Input管理.Keyboard.bキーが押されている((int)SlimDXKeys.Key.F7) && (TJAPlayer3.ConfigIni.eScrollMode == EScrollMode.REGULSPEED),
								new CCounter.DGキー処理(
								() =>
								{
									TJAPlayer3.ConfigIni.nRegSpeedBPM -= 1;
									if (TJAPlayer3.ConfigIni.nRegSpeedBPM < 1)
									{
										TJAPlayer3.ConfigIni.nRegSpeedBPM = 1;
									}
								}));
							#endregion
							#region[ F8 Reg.Speed UP ]
							this.ctキー反復用.Right.tキー反復(TJAPlayer3.Input管理.Keyboard.bキーが押されている((int)SlimDXKeys.Key.F8) && (TJAPlayer3.ConfigIni.eScrollMode == EScrollMode.REGULSPEED),
								new CCounter.DGキー処理(
								() =>
								{
									TJAPlayer3.ConfigIni.nRegSpeedBPM += 1;
									if (TJAPlayer3.ConfigIni.nRegSpeedBPM > 9999)
									{
										TJAPlayer3.ConfigIni.nRegSpeedBPM = 9999;
									}
								}));
							#endregion
							#region [ Decide ]
							if (((TJAPlayer3.Pad.b押された(EPad.LRed) || TJAPlayer3.Pad.b押された(EPad.RRed)) ||
									(TJAPlayer3.ConfigIni.bEnterがキー割り当てのどこにも使用されていない && TJAPlayer3.Input管理.Keyboard.bキーが押された((int)SlimDXKeys.Key.Return))) && !this.act難易度選択画面.選択済み[0] && !this.actChangeSE.bIsActive[0] && !this.actPlayOption.bIsActive[0])
							{
								if (this.act難易度選択画面.現在の選択行[0] == 0)
								{
									難易度から選曲へ戻る();
								}
								else if (this.act難易度選択画面.現在の選択行[0] == 1)
								{
									this.popupbool[0] = true;
								}
								else if (this.act難易度選択画面.現在の選択行[0] == 2)
								{
									TJAPlayer3.Skin.sound音色選択音?.t再生する();
									this.actChangeSE.tActivateChangeSE(0);
								}
								else
								{
									if (!(TJAPlayer3.ConfigIni.eGameMode == EGame.特訓モード && TJAPlayer3.ConfigIni.nPlayerCount >= 2))
									{
										if (this.act難易度選択画面.裏表示 && this.act難易度選択画面.現在の選択行[0] == 6)
										{
											TJAPlayer3.Skin.sound決定音.t再生する();
											this.act難易度選択画面.選択済み[0] = true;
											this.act難易度選択画面.確定された難易度[0] = (int)Difficulty.Edit;
										}
										else
										{


											if (this.act曲リスト.r現在選択中のスコア.譜面情報.b譜面が存在する[this.act難易度選択画面.現在の選択行[0] - 3])
											{
												TJAPlayer3.Skin.sound決定音.t再生する();
												this.act難易度選択画面.選択済み[0] = true;
												this.act難易度選択画面.確定された難易度[0] = this.act難易度選択画面.現在の選択行[0] - 3;
											}
										}
									}
								}
								this.難易度選択完了したか();
							}
							else if ((( TJAPlayer3.ConfigIni.bEnterがキー割り当てのどこにも使用されていない && TJAPlayer3.Input管理.Keyboard.bキーが押された((int)SlimDXKeys.Key.Return)) && this.act難易度選択画面.選択済み[0] || TJAPlayer3.Pad.b押された(EPad.LRed2P) || TJAPlayer3.Pad.b押された(EPad.RRed2P)) && TJAPlayer3.ConfigIni.nPlayerCount >= 2 && !this.act難易度選択画面.選択済み[1] && !this.actChangeSE.bIsActive[1] && !this.actPlayOption.bIsActive[1])
							{
								if (this.act難易度選択画面.現在の選択行[1] == 0)
								{
									難易度から選曲へ戻る();
								}
								else if (this.act難易度選択画面.現在の選択行[1] == 1)
								{
									this.popupbool[1] = true;
								}
								else if (this.act難易度選択画面.現在の選択行[1] == 2)
								{
									TJAPlayer3.Skin.sound音色選択音?.t再生する();
									this.actChangeSE.tActivateChangeSE(1);
								}
								else
								{
									if (!(TJAPlayer3.ConfigIni.eGameMode == EGame.特訓モード && TJAPlayer3.ConfigIni.nPlayerCount >= 2))
									{
										if (this.act難易度選択画面.裏表示 && this.act難易度選択画面.現在の選択行[1] == 6)
										{
											TJAPlayer3.Skin.sound決定音.t再生する();
											this.act難易度選択画面.選択済み[1] = true;
											this.act難易度選択画面.確定された難易度[1] = (int)Difficulty.Edit;
										}
										else
										{
											if (this.act曲リスト.r現在選択中のスコア.譜面情報.b譜面が存在する[this.act難易度選択画面.現在の選択行[1] - 3])
											{
												TJAPlayer3.Skin.sound決定音.t再生する();

												this.act難易度選択画面.選択済み[1] = true;
												this.act難易度選択画面.確定された難易度[1] = this.act難易度選択画面.現在の選択行[1] - 3;
											}
										}
									}
								}
								this.難易度選択完了したか();
							}
							#endregion
							#region [ Right ]
							if ((TJAPlayer3.Input管理.Keyboard.bキーが押された((int)SlimDXKeys.Key.RightArrow) || TJAPlayer3.Pad.b押された(EPad.RBlue)) && !this.act難易度選択画面.選択済み[0] && !this.actChangeSE.bIsActive[0] && !this.actPlayOption.bIsActive[0])
							{
								TJAPlayer3.Skin.soundカーソル移動音.t再生する();
								this.act難易度選択画面.現在の選択行[0]++;

								if (this.act難易度選択画面.現在の選択行[0] > 6)
								{
									this.act難易度選択画面.現在の選択行[0] = 6;
									this.act難易度選択画面.裏カウント[0]++;
								}
								else
								{
									this.act難易度選択画面.裏カウント[0] = 0;
									this.act難易度選択画面.ct難易度拡大用[0].n現在の値 = 0;
									this.act難易度選択画面.ct難易度拡大用[0].t時間Reset();
								}
								if (this.act難易度選択画面.裏表示 && this.act難易度選択画面.現在の選択行[0] == 6)
								{
									this.act曲リスト.n現在のアンカ難易度レベル[0] = 4;
								}
								else
								{
									if (this.act難易度選択画面.現在の選択行[0] >= 3 && this.act曲リスト.r現在選択中のスコア.譜面情報.b譜面が存在する[this.act難易度選択画面.現在の選択行[0] - 3])
										this.act曲リスト.n現在のアンカ難易度レベル[0] = this.act難易度選択画面.現在の選択行[0] - 3;
								}
							}
							if (((TJAPlayer3.Input管理.Keyboard.bキーが押された((int)SlimDXKeys.Key.RightArrow) && this.act難易度選択画面.選択済み[0]) || TJAPlayer3.Pad.b押された(EPad.RBlue2P)) && !this.act難易度選択画面.選択済み[1] && !this.actChangeSE.bIsActive[1] && !this.actPlayOption.bIsActive[1] && TJAPlayer3.ConfigIni.nPlayerCount >= 2)
							{
								TJAPlayer3.Skin.soundカーソル移動音.t再生する();
								this.act難易度選択画面.現在の選択行[1]++;

								if (this.act難易度選択画面.現在の選択行[1] > 6)
								{
									this.act難易度選択画面.現在の選択行[1] = 6;
									this.act難易度選択画面.裏カウント[1]++;
								}
								else
								{
									this.act難易度選択画面.裏カウント[1] = 0;
									this.act難易度選択画面.ct難易度拡大用[1].n現在の値 = 0;
									this.act難易度選択画面.ct難易度拡大用[1].t時間Reset();
								}
								if (this.act難易度選択画面.裏表示 && this.act難易度選択画面.現在の選択行[1] == 6)
								{
									this.act曲リスト.n現在のアンカ難易度レベル[1] = 4;
								}
								else
								{
									if (this.act難易度選択画面.現在の選択行[1] >= 3 && this.act曲リスト.r現在選択中のスコア.譜面情報.b譜面が存在する[this.act難易度選択画面.現在の選択行[1] - 3])
										this.act曲リスト.n現在のアンカ難易度レベル[1] = this.act難易度選択画面.現在の選択行[1] - 3;
								}
							}
							#endregion
							#region [ Left ]
							if ((TJAPlayer3.Input管理.Keyboard.bキーが押された((int)SlimDXKeys.Key.LeftArrow) || TJAPlayer3.Pad.b押された(EPad.LBlue)) && !this.act難易度選択画面.選択済み[0] && !this.actChangeSE.bIsActive[0] && !this.actPlayOption.bIsActive[0])
							{
								TJAPlayer3.Skin.soundカーソル移動音.t再生する();
								this.act難易度選択画面.現在の選択行[0]--;
								if (this.act難易度選択画面.現在の選択行[0] < 0)
								{
									this.act難易度選択画面.現在の選択行[0] = 0;
								}
								else
								{
									this.act難易度選択画面.ct難易度拡大用[0].n現在の値 = 0;
									this.act難易度選択画面.ct難易度拡大用[0].t時間Reset();
								}

								this.act難易度選択画面.裏カウント[0] = 0;

								if (this.act難易度選択画面.裏表示 && this.act難易度選択画面.現在の選択行[0] == 6)
								{
									this.act曲リスト.n現在のアンカ難易度レベル[0] = 4;
								}
								else
								{
									if (this.act難易度選択画面.現在の選択行[0] >= 3 && this.act曲リスト.r現在選択中のスコア.譜面情報.b譜面が存在する[this.act難易度選択画面.現在の選択行[0] - 3])
										this.act曲リスト.n現在のアンカ難易度レベル[0] = this.act難易度選択画面.現在の選択行[0] - 3;
								}
							}
							if (((TJAPlayer3.Input管理.Keyboard.bキーが押された((int)SlimDXKeys.Key.LeftArrow) && this.act難易度選択画面.選択済み[0]) || TJAPlayer3.Pad.b押された(EPad.LBlue2P)) && !this.act難易度選択画面.選択済み[1] && !this.actChangeSE.bIsActive[1] && !this.actPlayOption.bIsActive[1] && TJAPlayer3.ConfigIni.nPlayerCount >= 2)
							{
								TJAPlayer3.Skin.soundカーソル移動音.t再生する();
								this.act難易度選択画面.現在の選択行[1]--;
								if (this.act難易度選択画面.現在の選択行[1] < 0)
								{
									this.act難易度選択画面.現在の選択行[1] = 0;
								}
								else
								{
									this.act難易度選択画面.ct難易度拡大用[1].n現在の値 = 0;
									this.act難易度選択画面.ct難易度拡大用[1].t時間Reset();
								}

								this.act難易度選択画面.裏カウント[1] = 0;

								if (this.act難易度選択画面.裏表示 && this.act難易度選択画面.現在の選択行[1] == 6)
								{
									this.act曲リスト.n現在のアンカ難易度レベル[1] = 4;
								}
								else
								{
									if (this.act難易度選択画面.現在の選択行[1] >= 3 && this.act曲リスト.r現在選択中のスコア.譜面情報.b譜面が存在する[this.act難易度選択画面.現在の選択行[1] - 3])
										this.act曲リスト.n現在のアンカ難易度レベル[1] = this.act難易度選択画面.現在の選択行[1] - 3;
								}
							}
							#endregion
						}
					}
					#endregion
					#region[通常状態のキー入力]
					else if (現在の選曲画面状況 == E選曲画面.通常)
					{
						if (!this.actSortSongs.bIsActivePopupMenu)
						{
							#region [ ESC ]
							if (TJAPlayer3.Input管理.Keyboard.bキーが押された((int)SlimDXKeys.Key.Escape) && (this.act曲リスト.r現在選択中の曲 != null))
							{
								if (this.act曲リスト.r現在選択中の曲.r親ノード == null)
								{   // [ESC]
									TJAPlayer3.Skin.sound取消音.t再生する();
									this.eFadeOut完了時の戻り値 = E戻り値.タイトルに戻る;
									this.actFIFO.tFadeOut開始();
									base.eフェーズID = CStage.Eフェーズ.共通_FadeOut;
									return 0;
								}
								else
								{
									TJAPlayer3.Skin.sound取消音.t再生する();
									bool bNeedChangeSkin = this.act曲リスト.tBOXを出る();
								}
								this.actPresound.tサウンドの停止MT();
							}

							#endregion
							#region [ Shift-F1: CONFIG画面 ]
							if ((TJAPlayer3.Input管理.Keyboard.bキーが押されている((int)SlimDXKeys.Key.RightShift) || TJAPlayer3.Input管理.Keyboard.bキーが押されている((int)SlimDXKeys.Key.LeftShift)) &&
								TJAPlayer3.Input管理.Keyboard.bキーが押された((int)SlimDXKeys.Key.F1))
							{   // [SHIFT] + [F1] CONFIG
								this.GotoConfig();
								return 0;
							}
							#endregion
							#region [ F3 1PオートON/OFF ]
							if (TJAPlayer3.Input管理.Keyboard.bキーが押された((int)SlimDXKeys.Key.F3))
							{
								TJAPlayer3.Skin.sound変更音.t再生する();
								TJAPlayer3.ConfigIni.b太鼓パートAutoPlay[0] = !TJAPlayer3.ConfigIni.b太鼓パートAutoPlay[0];
							}
							#endregion
							#region [ F4 2PオートON/OFF ]
							if (TJAPlayer3.Input管理.Keyboard.bキーが押された((int)SlimDXKeys.Key.F4))
							{
								if (TJAPlayer3.ConfigIni.nPlayerCount > 1)
								{
									TJAPlayer3.Skin.sound変更音.t再生する();
									TJAPlayer3.ConfigIni.b太鼓パートAutoPlay[1] = !TJAPlayer3.ConfigIni.b太鼓パートAutoPlay[1];
								}
							}
							#endregion
							#region [ F5 スーパーハード ]
							if (TJAPlayer3.Input管理.Keyboard.bキーが押された((int)SlimDXKeys.Key.F5))
							{
								TJAPlayer3.Skin.sound変更音.t再生する();
								TJAPlayer3.ConfigIni.bSuperHard = !TJAPlayer3.ConfigIni.bSuperHard;
							}
							#endregion
							#region [ F6 SCROLL ]
							if (TJAPlayer3.Input管理.Keyboard.bキーが押された((int)SlimDXKeys.Key.F6))
							{
								TJAPlayer3.Skin.sound変更音.t再生する();
								TJAPlayer3.ConfigIni.bスクロールモードを上書き = true;
								switch ((int)TJAPlayer3.ConfigIni.eScrollMode)
								{
									case 0:
										TJAPlayer3.ConfigIni.eScrollMode = EScrollMode.BMSCROLL;
										break;
									case 1:
										TJAPlayer3.ConfigIni.eScrollMode = EScrollMode.HBSCROLL;
										break;
									case 2:
										TJAPlayer3.ConfigIni.eScrollMode = EScrollMode.REGULSPEED;
										break;
									case 3:
										TJAPlayer3.ConfigIni.eScrollMode = EScrollMode.Normal;
										TJAPlayer3.ConfigIni.bスクロールモードを上書き = false;
										break;
								}
							}
							#endregion
							#region[ F7 Reg.Speed DOWN ]
							this.ctキー反復用.Left.tキー反復(TJAPlayer3.Input管理.Keyboard.bキーが押されている((int)SlimDXKeys.Key.F7) && (TJAPlayer3.ConfigIni.eScrollMode == EScrollMode.REGULSPEED),
								new CCounter.DGキー処理(
								() =>
								{
									TJAPlayer3.ConfigIni.nRegSpeedBPM -= 1;
									if (TJAPlayer3.ConfigIni.nRegSpeedBPM < 1)
									{
										TJAPlayer3.ConfigIni.nRegSpeedBPM = 1;
									}
								}));
							#endregion
							#region[ F8 Reg.Speed UP ]
							this.ctキー反復用.Right.tキー反復(TJAPlayer3.Input管理.Keyboard.bキーが押されている((int)SlimDXKeys.Key.F8) && (TJAPlayer3.ConfigIni.eScrollMode == EScrollMode.REGULSPEED),
								new CCounter.DGキー処理(
								() =>
								{
									TJAPlayer3.ConfigIni.nRegSpeedBPM += 1;
									if (TJAPlayer3.ConfigIni.nRegSpeedBPM > 9999)
									{
										TJAPlayer3.ConfigIni.nRegSpeedBPM = 9999;
									}
								}));
							#endregion
							if (this.act曲リスト.r現在選択中の曲 != null)
							{
								#region [ Decide ]
								if (((TJAPlayer3.Pad.b押された(EPad.LRed) || TJAPlayer3.Pad.b押された(EPad.RRed)) || (TJAPlayer3.Pad.b押された(EPad.LRed2P) || TJAPlayer3.Pad.b押された(EPad.RRed2P)) && TJAPlayer3.ConfigIni.nPlayerCount >= 2 ||
										(TJAPlayer3.ConfigIni.bEnterがキー割り当てのどこにも使用されていない && TJAPlayer3.Input管理.Keyboard.bキーが押された((int)SlimDXKeys.Key.Return))))
								{
									if (this.act曲リスト.r現在選択中の曲 != null)
									{
										switch (this.act曲リスト.r現在選択中の曲.eNodeType)
										{
											case C曲リストノード.ENodeType.SCORE:
												if (!((this.n現在選択中の曲の難易度[0] == (int)Difficulty.Dan || this.n現在選択中の曲の難易度[0] == (int)Difficulty.Tower) && (TJAPlayer3.ConfigIni.nPlayerCount >= 2 || TJAPlayer3.ConfigIni.eGameMode == EGame.特訓モード)))
												{
													if (this.n現在選択中の曲の難易度[0] == (int)Difficulty.Dan && TJAPlayer3.Tx.Difficulty_Dan_Box != null && TJAPlayer3.Tx.Difficulty_Dan_Box_Selecting != null)
													{
														if (TJAPlayer3.Skin.soundDanするカッ.b読み込み成功)
															TJAPlayer3.Skin.soundDanするカッ.t再生する();
														else
															TJAPlayer3.Skin.sound決定音.t再生する();
														DanSelectingRow = 0;
														現在の選曲画面状況 = E選曲画面.Dan選択;
													}
													else if (this.n現在選択中の曲の難易度[0] == (int)Difficulty.Tower || this.n現在選択中の曲の難易度[0] == (int)Difficulty.Dan)
													{
														if (TJAPlayer3.Skin.sound曲決定音.b読み込み成功)
															TJAPlayer3.Skin.sound曲決定音.t再生する();
														else
															TJAPlayer3.Skin.sound決定音.t再生する();
														this.t曲を選択する();
													}
													else
													{
														if (TJAPlayer3.Input管理.Keyboard.bキーが押されている((int)SlimDXKeys.Key.Tab) && !(TJAPlayer3.ConfigIni.nPlayerCount >= 2 && TJAPlayer3.ConfigIni.eGameMode == EGame.特訓モード))
														{
															if (TJAPlayer3.Skin.sound曲決定音.b読み込み成功)
															{
																TJAPlayer3.Skin.sound決定音.t再生する();
																TJAPlayer3.Skin.sound曲決定音.t再生する();
															}
															else
																TJAPlayer3.Skin.sound決定音.t再生する();
															this.t曲を選択する();
														}
														else
														{
															TJAPlayer3.Skin.sound決定音.t再生する();
															現在の選曲画面状況 = E選曲画面.難易度選択In;
														}
													}
												}
												break;
											case C曲リストノード.ENodeType.BOX:
												{
													TJAPlayer3.Skin.sound決定音.t再生する();
													bool bNeedChangeSkin = this.act曲リスト.tBOXに入る();
													if (bNeedChangeSkin)
													{
														this.eFadeOut完了時の戻り値 = E戻り値.スキン変更;
														base.eフェーズID = Eフェーズ.選曲_NowLoading画面へのFadeOut;
													}
												}
												break;
											case C曲リストノード.ENodeType.BACKBOX:
												{
													TJAPlayer3.Skin.sound取消音.t再生する();
													bool bNeedChangeSkin = this.act曲リスト.tBOXを出る();
													if (bNeedChangeSkin)
													{
														this.eFadeOut完了時の戻り値 = E戻り値.スキン変更;
														base.eフェーズID = Eフェーズ.選曲_NowLoading画面へのFadeOut;
													}
												}
												break;
											case C曲リストノード.ENodeType.RANDOM:
												TJAPlayer3.Skin.sound決定音.t再生する();
												this.t曲をランダム選択する();
												break;
										}
									}
								}
								#endregion
								#region [ Up ]
								this.ctキー反復用.Up.tキー反復(TJAPlayer3.Input管理.Keyboard.bキーが押されている((int)SlimDXKeys.Key.LeftArrow), new CCounter.DGキー処理(this.tカーソルを上へ移動する));
								if (TJAPlayer3.Pad.b押された(EPad.LBlue) || TJAPlayer3.Pad.b押された(EPad.LBlue2P) && TJAPlayer3.ConfigIni.nPlayerCount >= 2)
								{
									this.tカーソルを上へ移動する();
								}
								if (TJAPlayer3.Input管理.Keyboard.bキーが押された((int)SlimDXKeys.Key.PageDown))
								{
									this.tカーソルを上へスキップする();
								}
								if (TJAPlayer3.Input管理.Keyboard.bキーが押された((int)SlimDXKeys.Key.Home))
								{
									this.tカーソルをフォルダのはじめへスキップする();
								}
								#endregion
								#region [ Down ]
								this.ctキー反復用.Down.tキー反復(TJAPlayer3.Input管理.Keyboard.bキーが押されている((int)SlimDXKeys.Key.RightArrow), new CCounter.DGキー処理(this.tカーソルを下へ移動する));
								if (TJAPlayer3.Pad.b押された(EPad.RBlue) || TJAPlayer3.Pad.b押された(EPad.RBlue2P) && TJAPlayer3.ConfigIni.nPlayerCount >= 2)
								{
									this.tカーソルを下へ移動する();
								}
								if (TJAPlayer3.Input管理.Keyboard.bキーが押された((int)SlimDXKeys.Key.PageUp))
								{
									this.tカーソルを下へスキップする();
								}
								if (TJAPlayer3.Input管理.Keyboard.bキーが押された((int)SlimDXKeys.Key.End))
								{
									this.tカーソルをフォルダの最後へスキップする();
								}
								#endregion
								#region [ Sort ]
								if (TJAPlayer3.Input管理.Keyboard.bキーが押された((int)SlimDXKeys.Key.Space))
								{
									TJAPlayer3.Skin.sound変更音.t再生する();
									this.actSortSongs.tActivatePopupMenu(ref this.act曲リスト);
								}
								#endregion
								#region [ 上: 難易度変更(上) ]
								if (TJAPlayer3.Input管理.Keyboard.bキーが押された((int)SlimDXKeys.Key.UpArrow))
								{
									Debug.WriteLine("ドラムス難易度変更");
									if (TJAPlayer3.Input管理.Keyboard.bキーが押されている((int)SlimDXKeys.Key.LeftControl) || TJAPlayer3.Input管理.Keyboard.bキーが押されている((int)SlimDXKeys.Key.RightControl))
										this.act曲リスト.t難易度レベルをひとつ進める(1);
									else
										this.act曲リスト.t難易度レベルをひとつ進める(0);
									TJAPlayer3.Skin.sound変更音.t再生する();
								}
								#endregion
								#region [ 下: 難易度変更(下) ]
								if (TJAPlayer3.Input管理.Keyboard.bキーが押された((int)SlimDXKeys.Key.DownArrow))
								{
									Debug.WriteLine("ドラムス難易度変更");
									if (TJAPlayer3.Input管理.Keyboard.bキーが押されている((int)SlimDXKeys.Key.LeftControl) || TJAPlayer3.Input管理.Keyboard.bキーが押されている((int)SlimDXKeys.Key.RightControl))
										this.act曲リスト.t難易度レベルをひとつ戻す(1);
									else
										this.act曲リスト.t難易度レベルをひとつ戻す(0);
									TJAPlayer3.Skin.sound変更音.t再生する();
								}
								#endregion
							}
						}
					}
					#endregion

                    #region [ Minus & Equals Sound Group Level ]
                    KeyboardSoundGroupLevelControlHandler.Handle(
						TJAPlayer3.Input管理.Keyboard, TJAPlayer3.SoundGroupLevelController, TJAPlayer3.Skin, true);
					#endregion

					this.actSortSongs.t進行描画();
				}
				switch ( base.eフェーズID )
				{
					case CStage.Eフェーズ.共通_FadeIn:
						if( this.actFIFO.On進行描画() != 0 )
						{
							base.eフェーズID = CStage.Eフェーズ.共通_通常状態;
						}
						break;

					case CStage.Eフェーズ.共通_FadeOut:
						if( this.actFIFO.On進行描画() == 0 )
						{
							break;
						}
						return (int) this.eFadeOut完了時の戻り値;

					case CStage.Eフェーズ.選曲_結果画面からのFadeIn:
						if( this.actFIfrom結果画面.On進行描画() != 0 )
						{
							base.eフェーズID = CStage.Eフェーズ.共通_通常状態;
						}
						break;

					case CStage.Eフェーズ.選曲_NowLoading画面へのFadeOut:
						if( this.actFOtoNowLoading.On進行描画() == 0 )
						{
							break;
						}
						return (int) this.eFadeOut完了時の戻り値;
				}
			}
			return 0;
		}

		public enum E戻り値 : int
		{
			継続,
			タイトルに戻る,
			選曲した,
			オプション呼び出し,
			コンフィグ呼び出し,
			スキン変更
		}
		public enum E選曲画面 : int
		{ 
			通常,
			Dan選択,//2020.05.25 Mr-Ojii Danの選択用
			難易度選択In,//2020.05.25 Mr-Ojii 難易度選択画面を追加したとき用
			難易度選択,
			難易度選択Out
		}
		// その他

		#region [ private ]
		//-----------------
		internal E選曲画面 現在の選曲画面状況 = E選曲画面.通常;
		private int DanSelectingRow = 0;

		private void GotoConfig()
		{
			actChangeSE.tDeativateChangeSE(0);
			actChangeSE.tDeativateChangeSE(1);
			actPlayOption.tDeativatePopupMenu(0);
			actPlayOption.tDeativatePopupMenu(1);
			this.actPresound.tサウンドの停止MT();
			this.eFadeOut完了時の戻り値 = E戻り値.コンフィグ呼び出し;  // #24525 2011.3.16 yyagi: [SHIFT]-[F1]でCONFIG呼び出し
			this.actFIFO.tFadeOut開始();
			base.eフェーズID = CStage.Eフェーズ.共通_FadeOut;
			TJAPlayer3.Skin.sound取消音.t再生する();
		}

		private void 難易度選択完了したか() {
			if (!完全に選択済み)
			{
				if (TJAPlayer3.ConfigIni.nPlayerCount >= 2)
				{
					if (this.act難易度選択画面.選択済み[0] && this.act難易度選択画面.選択済み[1])
					{
						if (TJAPlayer3.Skin.sound曲決定音.b読み込み成功)
							TJAPlayer3.Skin.sound曲決定音.t再生する();
						else
							TJAPlayer3.Skin.sound決定音.t再生する();
						this.t曲を選択する(this.act難易度選択画面.確定された難易度[0], this.act難易度選択画面.確定された難易度[1]);
						完全に選択済み = true;
					}
				}
				else
				{
					if (this.act難易度選択画面.選択済み[0])
					{
						if (TJAPlayer3.Skin.sound曲決定音.b読み込み成功)
							TJAPlayer3.Skin.sound曲決定音.t再生する();
						else
							TJAPlayer3.Skin.sound決定音.t再生する();
						this.t曲を選択する(this.act難易度選択画面.確定された難易度[0]);
						完全に選択済み = true;
					}
				}
			}
		}

		private void 難易度から選曲へ戻る()
		{
			if (!this.actChangeSE.bIsActive[0] && !this.actChangeSE.bIsActive[1] && !this.actPlayOption.bIsActive[0] && !this.actPlayOption.bIsActive[1])
			{
				TJAPlayer3.Skin.sound取消音.t再生する();
				this.act難易度選択画面.選択済み[0] = false;
				this.act難易度選択画面.選択済み[1] = false;
				this.act難易度選択画面.b開いた直後 = true;
				現在の選曲画面状況 = E選曲画面.難易度選択Out;
			}
		}

		[StructLayout( LayoutKind.Sequential )]
		private struct STキー反復用カウンタ
		{
			public CCounter Up;
			public CCounter Down;
			public CCounter Left;
			public CCounter Right;
			public CCounter this[ int index ]
			{
				get
				{
					switch( index )
					{
						case 0:
							return this.Up;

						case 1:
							return this.Down;

						case 2:
							return this.Left;

						case 3:
							return this.Right;
					}
					throw new IndexOutOfRangeException();
				}
				set
				{
					switch( index )
					{
						case 0:
							this.Up = value;
							return;

						case 1:
							this.Down = value;
							return;

						case 2:
							this.Left = value;
							return;

						case 3:
							this.Right = value;
							return;
					}
					throw new IndexOutOfRangeException();
				}
			}
		}
		internal CActFIFOBlack actFIFO;
		private CActFIFOBlack actFIfrom結果画面;
		//private CActFIFOBlack actFOtoNowLoading;
		private CActFIFOStart actFOtoNowLoading;
		private CActSelectPresound actPresound;
		public CActSelect演奏履歴パネル act演奏履歴パネル;
		public CActSelect曲リスト act曲リスト;
		internal CActSelect難易度選択画面 act難易度選択画面;
		private bool 完全に選択済み = false;

		private CActSortSongs actSortSongs;
		private CActSelectPlayOption actPlayOption;
		internal CActSelectChangeSE actChangeSE;

		private bool bBGM再生済み;
		private STキー反復用カウンタ ctキー反復用;
		public CCounter ct登場時アニメ用共通;
		private CCounter ct背景スクロール用タイマー;
		private CCounter ctカウントダウン用タイマー;
		internal CCounter ct難易度選択画面IN用タイマー;
		internal CCounter ct難易度選択画面INバー拡大用タイマー;
		internal CCounter ct難易度選択画面OUT用タイマー;
		internal E戻り値 eFadeOut完了時の戻り値;

		public void MouseWheel(int i)
		{
			if (this.現在の選曲画面状況 == E選曲画面.通常) 			
			{
				if (i < 0) 
					this.tカーソルを下へ移動する();
				else
					this.tカーソルを上へ移動する();
			}
		}
		private void tカーソルを下へ移動する()
		{
			TJAPlayer3.Skin.soundカーソル移動音.t再生する();
			this.act曲リスト.t次に移動();
		}
		private void tカーソルを上へ移動する()
		{
			TJAPlayer3.Skin.soundカーソル移動音.t再生する();
			this.act曲リスト.t前に移動();

		}
		private void tカーソルを下へスキップする()
		{
			TJAPlayer3.Skin.sound選曲スキップ音.t再生する();
			this.act曲リスト.tかなり次に移動();
		}
		private void tカーソルを上へスキップする()
		{
			TJAPlayer3.Skin.sound選曲スキップ音.t再生する();
			this.act曲リスト.tかなり前に移動();
		}
		private void tカーソルをフォルダのはじめへスキップする()
		{
			TJAPlayer3.Skin.sound選曲スキップ音.t再生する();
			this.act曲リスト.tフォルダのはじめに移動();
		}
		private void tカーソルをフォルダの最後へスキップする()
		{
			TJAPlayer3.Skin.sound選曲スキップ音.t再生する();
			this.act曲リスト.tフォルダの最後に移動();
		}
		private void t曲をランダム選択する()
		{
			C曲リストノード song = this.act曲リスト.r現在選択中の曲;
			if( ( song.stackランダム演奏番号.Count == 0 ) || ( song.listランダム用ノードリスト == null ) )
			{
				if( song.listランダム用ノードリスト == null )
				{
					song.listランダム用ノードリスト = this.t指定された曲が存在する場所の曲を列挙する_子リスト含む( song );
				}
				int count = song.listランダム用ノードリスト.Count;
				if( count == 0 )
				{
					return;
				}
				int[] numArray = new int[ count ];
				for( int i = 0; i < count; i++ )
				{
					numArray[ i ] = i;
				}
				for( int j = 0; j < ( count * 1.5 ); j++ )
				{
					int index = TJAPlayer3.Random.Next( count );
					int num5 = TJAPlayer3.Random.Next( count );
					int num6 = numArray[ num5 ];
					numArray[ num5 ] = numArray[ index ];
					numArray[ index ] = num6;
				}
				for( int k = 0; k < count; k++ )
				{
					song.stackランダム演奏番号.Push( numArray[ k ] );
				}
				if( TJAPlayer3.ConfigIni.bLogDTX詳細ログ出力 )
				{
					StringBuilder builder = new StringBuilder( 0x400 );
					builder.Append( string.Format( "ランダムインデックスリストを作成しました: {0}曲: ", song.stackランダム演奏番号.Count ) );
					for( int m = 0; m < count; m++ )
					{
						builder.Append( string.Format( "{0} ", numArray[ m ] ) );
					}
					Trace.TraceInformation( builder.ToString() );
				}
			}
			this.act曲リスト.RandomSelect(song.listランダム用ノードリスト[song.stackランダム演奏番号.Pop()]);

		}
		private void t曲を選択する()
		{
			this.r確定された曲 = this.act曲リスト.r現在選択中の曲;
			this.r確定されたスコア = this.act曲リスト.r現在選択中のスコア;
			this.n確定された曲の難易度[0] = this.act曲リスト.n現在選択中の曲の難易度レベル[0];
			this.n確定された曲の難易度[1] = this.act曲リスト.n現在選択中の曲の難易度レベル[1];
			this.str確定された曲のジャンル = this.r確定された曲.strジャンル;
			if ( ( this.r確定された曲 != null ) && ( this.r確定されたスコア != null ) )
			{
				this.eFadeOut完了時の戻り値 = E戻り値.選曲した;
				this.actFOtoNowLoading.tFadeOut開始();				// #27787 2012.3.10 yyagi 曲決定時の画面FadeOutの省略
				base.eフェーズID = CStage.Eフェーズ.選曲_NowLoading画面へのFadeOut;
			}
			TJAPlayer3.Skin.bgm選曲画面.t停止する();
		}
		public void t曲を選択する( int nCurrentLevel )
		{
			this.r確定された曲 = this.act曲リスト.r現在選択中の曲;
			this.r確定されたスコア = this.act曲リスト.r現在選択中のスコア;
			this.n確定された曲の難易度[0] = nCurrentLevel;
			this.n確定された曲の難易度[1] = nCurrentLevel;
			this.str確定された曲のジャンル = this.r確定された曲.strジャンル;
			if ( ( this.r確定された曲 != null ) && ( this.r確定されたスコア != null ) )
			{
				this.eFadeOut完了時の戻り値 = E戻り値.選曲した;
				this.actFOtoNowLoading.tFadeOut開始();				// #27787 2012.3.10 yyagi 曲決定時の画面FadeOutの省略
				base.eフェーズID = CStage.Eフェーズ.選曲_NowLoading画面へのFadeOut;
			}

			TJAPlayer3.Skin.bgm選曲画面.t停止する();
		}
		public void t曲を選択する(int nCurrentLevel,int nCurrentLevel2)
		{
			this.r確定された曲 = this.act曲リスト.r現在選択中の曲;
			this.r確定されたスコア = this.act曲リスト.r現在選択中のスコア;
			this.n確定された曲の難易度[0] = nCurrentLevel;
			this.n確定された曲の難易度[1] = nCurrentLevel2;
			this.str確定された曲のジャンル = this.r確定された曲.strジャンル;
			if ((this.r確定された曲 != null) && (this.r確定されたスコア != null))
			{
				this.eFadeOut完了時の戻り値 = E戻り値.選曲した;
				this.actFOtoNowLoading.tFadeOut開始();                // #27787 2012.3.10 yyagi 曲決定時の画面FadeOutの省略
				base.eフェーズID = CStage.Eフェーズ.選曲_NowLoading画面へのFadeOut;
			}

			TJAPlayer3.Skin.bgm選曲画面.t停止する();
		}
		private List<C曲リストノード> t指定された曲が存在する場所の曲を列挙する_子リスト含む( C曲リストノード song )
		{
			List<C曲リストノード> list = new List<C曲リストノード>();
			song = song.r親ノード;
			if( ( song == null ) && ( TJAPlayer3.Songs管理.list曲ルート.Count > 0 ) )
			{
				foreach( C曲リストノード c曲リストノード in TJAPlayer3.Songs管理.list曲ルート )
				{
					if( ( c曲リストノード.eNodeType == C曲リストノード.ENodeType.SCORE ))
					{
						list.Add( c曲リストノード );
					}
					if( ( c曲リストノード.list子リスト != null ) && TJAPlayer3.ConfigIni.bランダムセレクトで子BOXを検索対象とする )
					{
						this.t指定された曲の子リストの曲を列挙する_孫リスト含む( c曲リストノード, ref list );
					}
				}
				return list;
			}
			this.t指定された曲の子リストの曲を列挙する_孫リスト含む( song, ref list );
			return list;
		}
		private void t指定された曲の子リストの曲を列挙する_孫リスト含む( C曲リストノード r親, ref List<C曲リストノード> list )
		{
			if( ( r親 != null ) && ( r親.list子リスト != null ) )
			{
				foreach( C曲リストノード c曲リストノード in r親.list子リスト )
				{
					if( ( c曲リストノード.eNodeType == C曲リストノード.ENodeType.SCORE ))
					{
						list.Add( c曲リストノード );
					}
					if( ( c曲リストノード.list子リスト != null ) && TJAPlayer3.ConfigIni.bランダムセレクトで子BOXを検索対象とする )
					{
						this.t指定された曲の子リストの曲を列挙する_孫リスト含む( c曲リストノード, ref list );
					}
				}
			}
		}

		private int This_counter;

		private bool[] popupbool = { false, false };

		//-----------------
		#endregion
	}
}
