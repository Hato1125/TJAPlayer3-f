using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using SlimDX;
using FDK;

namespace DTXMania
{
	internal class FlyingNotes : CActivity
	{
		// コンストラクタ

		public FlyingNotes()
		{
            base.b活性化してない = true;
        }
		
		
		// メソッド
        public virtual void Start( int nLane, int nPlayer )
		{
            if (CDTXMania.Tx.Notes != null)
            {
                for (int i = 0; i < 128; i++)
                {
                    if(!Flying[i].IsUsing)
                    {
                        // 初期化
                        Flying[i].IsUsing = true;
                        Flying[i].Lane = nLane;
                        Flying[i].Player = nPlayer;
                        Flying[i].X = CDTXMania.Skin.Game_Effect_FlyingNotes_StartPoint_X[nPlayer];
                        Flying[i].Y = CDTXMania.Skin.Game_Effect_FlyingNotes_StartPoint_Y[nPlayer];
                        Flying[i].OldValue = 0;
                        // 角度の決定
                        Flying[i].Height = Math.Abs(CDTXMania.Skin.Game_Effect_FlyingNotes_EndPoint_Y[nPlayer] - CDTXMania.Skin.Game_Effect_FlyingNotes_StartPoint_Y[nPlayer]);
                        Flying[i].Width = Math.Abs((CDTXMania.Skin.Game_Effect_FlyingNotes_EndPoint_X[nPlayer] - CDTXMania.Skin.Game_Effect_FlyingNotes_StartPoint_X[nPlayer])) / 2;
                        //Console.WriteLine("{0}, {1}", width2P, height2P);
                        var theta = ((Math.Atan2(Flying[i].Height, Flying[i].Width) * 180.0) / Math.PI);
                        Flying[i].Counter = new CCounter(0, (int)(180 - theta), CDTXMania.Skin.Game_Effect_FlyingNotes_Timer, CDTXMania.Timer);

                        Flying[i].Increase = ((Flying[i].Width * 2) / (180 - theta));
                        break;
                    }
                }
            }
        }

		// CActivity 実装

		public override void On活性化()
		{
            for (int i = 0; i < 128; i++)
            {
                Flying[i] = new Status();
                Flying[i].IsUsing = false;
                Flying[i].Counter = new CCounter();
            }
            base.On活性化();
		}
		public override void On非活性化()
		{
            for (int i = 0; i < 128; i++)
            {
                Flying[i].Counter = null;
            }
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
				base.OnManagedリソースの解放();
			}
		}
		public override int On進行描画()
		{
			if( !base.b活性化してない )
			{
                for (int i = 0; i < 128; i++)
                {
                    //if( CDTXMania.Skin.nScrollFieldX[0] > 414 + 4 )
                    //    break;
                    //if( CDTXMania.Skin.nScrollFieldX[0] < 414 - 4 )
                    //    break;

                    if (Flying[i].IsUsing)
                    {
                        Flying[i].OldValue = Flying[i].Counter.n現在の値;
                        Flying[i].Counter.t進行();
                        if (Flying[i].Counter.b終了値に達した)
                        {
                            Flying[i].Counter.t停止();
                            Flying[i].IsUsing = false;
                            CDTXMania.stage演奏ドラム画面.actGauge.Start(Flying[i].Lane, E判定.Perfect, Flying[i].
                                Player);
                            CDTXMania.stage演奏ドラム画面.actChipEffects.Start(Flying[i].Player, Flying[i].Lane);
                        }
                        for (int n = Flying[i].OldValue; n < Flying[i].Counter.n現在の値; n++)
                        {
                            Flying[i].X += Flying[i].Increase;
                            Flying[i].Y = (CDTXMania.Skin.Game_Effect_FlyingNotes_StartPoint_Y[Flying[i].Player] + (Math.Sin(Flying[i].Counter.n現在の値 * (Math.PI / 180)) * -CDTXMania.Skin.Game_Effect_FlyingNotes_Sine));
                        }
                        Flying[i].OldValue = Flying[i].Counter.n現在の値;

                        if (Flying[i].Player == 0)
                        {
                            //
                            CDTXMania.Tx.Notes?.t2D中心基準描画(CDTXMania.app.Device, (int)Flying[i].X, (int)Flying[i].Y, new Rectangle(Flying[i].Lane * 130, 0, 130, 130));
                        }
                        else if (Flying[i].Player == 1)
                        {
                            //

                        }
                    }
                }
			}
			return 0;
		}
		

		#region [ private ]
		//-----------------

        [StructLayout(LayoutKind.Sequential)]
        private struct Status
        {
            public int Lane;
            public int Player;
            public bool IsUsing;
            public CCounter Counter;
            public int OldValue;
            public double X;
            public double Y;
            public int Height;
            public int Width;
            public double Increase;
        }

        private Status[] Flying = new Status[128];
        private int[] Theta = new int[2];

        //-----------------
        #endregion
    }
}
