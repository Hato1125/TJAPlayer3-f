using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using SlimDX;
using FDK;

namespace DTXMania
{
	internal class CAct演奏パネル文字列 : CActivity
	{

		// コンストラクタ

		public CAct演奏パネル文字列()
		{
			base.b活性化してない = true;
			this.Start();
		}


        // メソッド

        public void SetPanelString(string songName, string genreName, string stageText = null)
		{
			if( base.b活性化してる )
			{
				CDTXMania.tテクスチャの解放( ref this.txPanel );
				if( (songName != null ) && (songName.Length > 0 ) )
				{
					try
					{
					    using (var bmpSongTitle = pfMusicName.DrawPrivateFont(songName, Color.White, Color.Black ))
					    {
					        this.txMusicName = CDTXMania.tテクスチャの生成( bmpSongTitle, false );
					    }

                        Bitmap bmpDiff;
                        string strDiff = "";
                        if (CDTXMania.Skin.eDiffDispMode == E難易度表示タイプ.n曲目に表示)
                        {
                            switch (CDTXMania.stage選曲.n確定された曲の難易度)
                            {
                                case 0:
                                    strDiff = "かんたん ";
                                    break;
                                case 1:
                                    strDiff = "ふつう ";
                                    break;
                                case 2:
                                    strDiff = "むずかしい ";
                                    break;
                                case 3:
                                    strDiff = "おに ";
                                    break;
                                case 4:
                                    strDiff = "えでぃと ";
                                    break;
                                default:
                                    strDiff = "おに ";
                                    break;
                            }
                            bmpDiff = pfMusicName.DrawPrivateFont(strDiff + stageText != null ? CDTXMania.Skin.Game_StageText : stageText, Color.White, Color.Black);
                        }
                        else
                        {
                            if(stageText != null)
                            {
                                bmpDiff = pfMusicName.DrawPrivateFont(stageText, Color.White, Color.Black);
                            }
                            else
                            {
                                if (CDTXMania.Skin.Game_StageText_IsRed)
                                {
                                    bmpDiff = pfMusicName.DrawPrivateFont(CDTXMania.Skin.Game_StageText, Color.White, Color.Red);
                                }
                                else
                                {
                                    bmpDiff = pfMusicName.DrawPrivateFont(CDTXMania.Skin.Game_StageText, Color.White, Color.Black);
                                }

                            }
                        }

					    using (bmpDiff)
					    {
					        this.tx難易度とステージ数 = CDTXMania.tテクスチャの生成( bmpDiff, false );
					    }
					}
					catch( CTextureCreateFailedException e )
					{
						Trace.TraceError( e.ToString() );
						Trace.TraceError( "パネル文字列テクスチャの生成に失敗しました。" );
						this.txPanel = null;
					}
				}
                if( !string.IsNullOrEmpty(genreName) )
                {
                    if(genreName.Equals( "アニメ" ) )
                    {
                        this.txGENRE = CDTXMania.Tx.TxCGen("Anime");
                    }
                    else if(genreName.Equals( "J-POP" ) )
                    {
                        this.txGENRE = CDTXMania.Tx.TxCGen("J-POP");
                    }
                    else if(genreName.Equals( "ゲームミュージック" ) )
                    {
                        this.txGENRE = CDTXMania.Tx.TxCGen("Game");
                    }
                    else if(genreName.Equals( "ナムコオリジナル" ) )
                    {
                        this.txGENRE = CDTXMania.Tx.TxCGen("Namco");
                    }
                    else if(genreName.Equals( "クラシック" ) )
                    {
                        this.txGENRE = CDTXMania.Tx.TxCGen("Classic");
                    }
                    else if(genreName.Equals( "どうよう" ) )
                    {
                        this.txGENRE = CDTXMania.Tx.TxCGen("Child");
                    }
                    else if(genreName.Equals( "バラエティ" ) )
                    {
                        this.txGENRE = CDTXMania.Tx.TxCGen("Variety");
                    }
                    else if(genreName.Equals( "ボーカロイド" ) || genreName.Equals( "VOCALOID" ) )
                    {
                        this.txGENRE = CDTXMania.Tx.TxCGen("Vocaloid");
                    }
                    else
                    {
                        using (var bmpDummy = new Bitmap( 1, 1 ))
                        {
                            this.txGENRE = CDTXMania.tテクスチャの生成( bmpDummy, true );
                        }
                    }
                }

			    this.ct進行用 = new CCounter( 0, 2000, 2, CDTXMania.Timer );
				this.Start();



			}
		}

        public void t歌詞テクスチャを生成する( string str歌詞 )
        {
            using (var bmpleric = this.pf歌詞フォント.DrawPrivateFont( str歌詞, Color.White, Color.Blue))
            {
                this.tx歌詞テクスチャ = CDTXMania.tテクスチャの生成( bmpleric, false );
            }
        }

        /// <summary>
        /// レイヤー管理のため、On進行描画から分離。
        /// </summary>
        public void t歌詞テクスチャを描画する()
        {
            if( this.tx歌詞テクスチャ != null )
            {
                this.tx歌詞テクスチャ.t2D描画( CDTXMania.app.Device, 640 - ( this.tx歌詞テクスチャ.szテクスチャサイズ.Width / 2 ), 630 );
            }
        }

		public void Stop()
		{
			this.bMute = true;
		}
		public void Start()
		{
			this.bMute = false;
		}


		// CActivity 実装

		public override void On活性化()
		{
            if( !string.IsNullOrEmpty( CDTXMania.ConfigIni.FontName ) )
            {
                this.pfMusicName = new CPrivateFastFont( new FontFamily( CDTXMania.ConfigIni.FontName), 30 );
                //this.pf縦書きテスト = new CPrivateFastFont( new FontFamily( CDTXMania.ConfigIni.strPrivateFontで使うフォント名 ), 22 );
            }
            else
                this.pfMusicName = new CPrivateFastFont( new FontFamily("MS UI Gothic"), 30 );

            this.pf歌詞フォント = new CPrivateFastFont( new FontFamily("MS UI Gothic"), 38 );

			this.txPanel = null;
			this.ct進行用 = new CCounter();
			this.Start();
            this.bFirst = true;
			base.On活性化();
		}
		public override void On非活性化()
		{
			this.ct進行用 = null;
			base.On非活性化();
		}
		public override void OnManagedリソースの作成()
		{
			if( !base.b活性化してない )
			{
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if( !base.b活性化してない )
			{
				CDTXMania.tテクスチャの解放( ref this.txPanel );
				CDTXMania.tテクスチャの解放( ref this.txMusicName );
                CDTXMania.tテクスチャの解放( ref this.txGENRE );
                CDTXMania.tテクスチャの解放(ref this.txPanel);
                CDTXMania.tテクスチャの解放(ref this.tx歌詞テクスチャ);
                CDTXMania.t安全にDisposeする(ref this.pfMusicName);
                CDTXMania.t安全にDisposeする(ref this.pf歌詞フォント);
                base.OnManagedリソースの解放();
			}
		}
		public override int On進行描画()
		{
			throw new InvalidOperationException( "t進行描画(x,y)のほうを使用してください。" );
		}
		public int t進行描画( int x, int y )
		{
            if (CDTXMania.stage演奏ドラム画面.actDan.IsAnimating) return 0;
			if( !base.b活性化してない && !this.bMute )
			{
				this.ct進行用.t進行Loop();
                if( this.bFirst )
                {
                    this.ct進行用.n現在の値 = 300;
                }
                if( this.txGENRE != null )
                    this.txGENRE.t2D描画( CDTXMania.app.Device, 1114, 74);

                if( CDTXMania.Skin.b現在のステージ数を表示しない )
                {
                    if( this.txMusicName != null )
                    {
                        float fRate = 660.0f / this.txMusicName.szテクスチャサイズ.Width;
                        if (this.txMusicName.szテクスチャサイズ.Width <= 660.0f)
                            fRate = 1.0f;
                        this.txMusicName.vc拡大縮小倍率.X = fRate;
                        this.txMusicName.t2D描画( CDTXMania.app.Device, 1254 - ( this.txMusicName.szテクスチャサイズ.Width * fRate ), 14 );
                    }
                }
                else
                {
                    #region[ 透明度制御 ]

                    if( this.ct進行用.n現在の値 < 745 )
                    {
                        this.bFirst = false;
                        this.txMusicName.n透明度 = 255;
                        if( this.txGENRE != null )
                            this.txGENRE.n透明度 = 255;
                        this.tx難易度とステージ数.n透明度 = 0;
                    }
                    else if( this.ct進行用.n現在の値 >= 745 && this.ct進行用.n現在の値 < 1000 )
                    {
                        this.txMusicName.n透明度 = 255 - ( this.ct進行用.n現在の値 - 745 );
                        if( this.txGENRE != null )
                            this.txGENRE.n透明度 = 255 - ( this.ct進行用.n現在の値 - 745 );
                        this.tx難易度とステージ数.n透明度 = this.ct進行用.n現在の値 - 745;
                    }
                    else if( this.ct進行用.n現在の値 >= 1000 && this.ct進行用.n現在の値 <= 1745 )
                    {
                        this.txMusicName.n透明度 = 0;
                        if( this.txGENRE != null )
                            this.txGENRE.n透明度 = 0;
                        this.tx難易度とステージ数.n透明度 = 255;
                    }
                    else if( this.ct進行用.n現在の値 >= 1745 )
                    {
                        this.txMusicName.n透明度 = this.ct進行用.n現在の値 - 1745;
                        if( this.txGENRE != null )
                            this.txGENRE.n透明度 = this.ct進行用.n現在の値 - 1745;
                        this.tx難易度とステージ数.n透明度 = 255 - ( this.ct進行用.n現在の値 - 1745 );
                    }
                    #endregion
                    if( this.txMusicName != null )
                    {
                        if(this.b初めての進行描画)
                        {
                            this.txMusicName.vc拡大縮小倍率.X = CDTXMania.GetSongNameXScaling(ref txMusicName);
                            b初めての進行描画 = false;
                        }
                        this.txMusicName.t2D描画( CDTXMania.app.Device, 1254 - ( this.txMusicName.szテクスチャサイズ.Width * txMusicName.vc拡大縮小倍率.X), 14 );
                    }
                    if( this.tx難易度とステージ数 != null )
	    			    this.tx難易度とステージ数.t2D描画( CDTXMania.app.Device, 1254 - this.tx難易度とステージ数.szテクスチャサイズ.Width, 14 );
                }

                //CDTXMania.act文字コンソール.tPrint( 0, 0, C文字コンソール.Eフォント種別.白, this.ct進行用.n現在の値.ToString() );

				//this.txMusicName.t2D描画( CDTXMania.app.Device, 1250 - this.txMusicName.szテクスチャサイズ.Width, 14 );
			}
			return 0;
		}


		// その他

		#region [ private ]
		//-----------------
		private CCounter ct進行用;

		private CTexture txPanel;
		private bool bMute;
        private bool bFirst;

        private CTexture txMusicName;
        private CTexture tx難易度とステージ数;
        private CTexture txGENRE;
        private CTexture tx歌詞テクスチャ;
        private CPrivateFastFont pfMusicName;
        private CPrivateFastFont pf歌詞フォント;
		//-----------------
		#endregion
	}
}
　
