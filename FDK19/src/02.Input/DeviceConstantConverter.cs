﻿using System;
using System.Collections.Generic;
using System.Text;

using SlimDXKey = SlimDXKeys.Key;
using SDLKey = SDL2.SDL.SDL_Scancode;

namespace FDK
{
	public class DeviceConstantConverter
	{
		// メソッド


		/// <returns>
		/// 対応する値がなければ SlimDXKey.Unknown を返す。
		/// </returns>
		public static SlimDXKey SDLKToKey(SDLKey sdl_key)
        {
			if (_SDLKtoKey.TryGetValue(sdl_key, out var key))
				return key;
			else
				return SlimDXKey.Unknown;
		}

		/// <summary>
		///	SDLKey(SDL_Scancode) から SlimDXKey への変換表。
		/// </summary>
		private static readonly Dictionary<SDLKey, SlimDXKey> _SDLKtoKey = new Dictionary<SDLKey, SlimDXKey>()
		{
			#region [ *** ]
			{ SDLKey.SDL_SCANCODE_UNKNOWN, SlimDXKey.Unknown },
			{ SDLKey.SDL_SCANCODE_A, SlimDXKey.A },
			{ SDLKey.SDL_SCANCODE_B, SlimDXKey.B },
			{ SDLKey.SDL_SCANCODE_C, SlimDXKey.C },
			{ SDLKey.SDL_SCANCODE_D, SlimDXKey.D },
			{ SDLKey.SDL_SCANCODE_E, SlimDXKey.E },
			{ SDLKey.SDL_SCANCODE_F, SlimDXKey.F },
			{ SDLKey.SDL_SCANCODE_G, SlimDXKey.G },
			{ SDLKey.SDL_SCANCODE_H, SlimDXKey.H },
			{ SDLKey.SDL_SCANCODE_I, SlimDXKey.I },
			{ SDLKey.SDL_SCANCODE_J, SlimDXKey.J },
			{ SDLKey.SDL_SCANCODE_K, SlimDXKey.K },
			{ SDLKey.SDL_SCANCODE_L, SlimDXKey.L },
			{ SDLKey.SDL_SCANCODE_M, SlimDXKey.M },
			{ SDLKey.SDL_SCANCODE_N, SlimDXKey.N },
			{ SDLKey.SDL_SCANCODE_O, SlimDXKey.O },
			{ SDLKey.SDL_SCANCODE_P, SlimDXKey.P },
			{ SDLKey.SDL_SCANCODE_Q, SlimDXKey.Q },
			{ SDLKey.SDL_SCANCODE_R, SlimDXKey.R },
			{ SDLKey.SDL_SCANCODE_S, SlimDXKey.S },
			{ SDLKey.SDL_SCANCODE_T, SlimDXKey.T },
			{ SDLKey.SDL_SCANCODE_U, SlimDXKey.U },
			{ SDLKey.SDL_SCANCODE_V, SlimDXKey.V },
			{ SDLKey.SDL_SCANCODE_W, SlimDXKey.W },
			{ SDLKey.SDL_SCANCODE_X, SlimDXKey.X },
			{ SDLKey.SDL_SCANCODE_Y, SlimDXKey.Y },
			{ SDLKey.SDL_SCANCODE_Z, SlimDXKey.Z },
			{ SDLKey.SDL_SCANCODE_1, SlimDXKey.D1 },
			{ SDLKey.SDL_SCANCODE_2, SlimDXKey.D2 },
			{ SDLKey.SDL_SCANCODE_3, SlimDXKey.D3 },
			{ SDLKey.SDL_SCANCODE_4, SlimDXKey.D4 },
			{ SDLKey.SDL_SCANCODE_5, SlimDXKey.D5 },
			{ SDLKey.SDL_SCANCODE_6, SlimDXKey.D6 },
			{ SDLKey.SDL_SCANCODE_7, SlimDXKey.D7 },
			{ SDLKey.SDL_SCANCODE_8, SlimDXKey.D8 },
			{ SDLKey.SDL_SCANCODE_9, SlimDXKey.D9 },
			{ SDLKey.SDL_SCANCODE_0, SlimDXKey.D0 },
			{ SDLKey.SDL_SCANCODE_RETURN, SlimDXKey.Return },
			{ SDLKey.SDL_SCANCODE_ESCAPE, SlimDXKey.Escape },
			{ SDLKey.SDL_SCANCODE_BACKSPACE, SlimDXKey.Backspace },
			{ SDLKey.SDL_SCANCODE_TAB, SlimDXKey.Tab },
			{ SDLKey.SDL_SCANCODE_SPACE, SlimDXKey.Space },
			{ SDLKey.SDL_SCANCODE_MINUS, SlimDXKey.Minus },
			{ SDLKey.SDL_SCANCODE_EQUALS, SlimDXKey.Equals },
			{ SDLKey.SDL_SCANCODE_LEFTBRACKET, SlimDXKey.LeftBracket },
			{ SDLKey.SDL_SCANCODE_RIGHTBRACKET, SlimDXKey.RightBracket },
			{ SDLKey.SDL_SCANCODE_BACKSLASH, SlimDXKey.Backslash },
			{ SDLKey.SDL_SCANCODE_SEMICOLON, SlimDXKey.Semicolon },
			{ SDLKey.SDL_SCANCODE_APOSTROPHE, SlimDXKey.Apostrophe },
			{ SDLKey.SDL_SCANCODE_GRAVE, SlimDXKey.Grave },
			{ SDLKey.SDL_SCANCODE_COMMA, SlimDXKey.Comma },
			{ SDLKey.SDL_SCANCODE_PERIOD, SlimDXKey.Period },
			{ SDLKey.SDL_SCANCODE_SLASH, SlimDXKey.Slash },
			{ SDLKey.SDL_SCANCODE_CAPSLOCK, SlimDXKey.CapsLock },
			{ SDLKey.SDL_SCANCODE_F1, SlimDXKey.F1 },
			{ SDLKey.SDL_SCANCODE_F2, SlimDXKey.F2 },
			{ SDLKey.SDL_SCANCODE_F3, SlimDXKey.F3 },
			{ SDLKey.SDL_SCANCODE_F4, SlimDXKey.F4 },
			{ SDLKey.SDL_SCANCODE_F5, SlimDXKey.F5 },
			{ SDLKey.SDL_SCANCODE_F6, SlimDXKey.F6 },
			{ SDLKey.SDL_SCANCODE_F7, SlimDXKey.F7 },
			{ SDLKey.SDL_SCANCODE_F8, SlimDXKey.F8 },
			{ SDLKey.SDL_SCANCODE_F9, SlimDXKey.F9 },
			{ SDLKey.SDL_SCANCODE_F10, SlimDXKey.F10 },
			{ SDLKey.SDL_SCANCODE_F11, SlimDXKey.F11 },
			{ SDLKey.SDL_SCANCODE_F12, SlimDXKey.F12 },
			{ SDLKey.SDL_SCANCODE_PRINTSCREEN, SlimDXKey.PrintScreen },
			{ SDLKey.SDL_SCANCODE_SCROLLLOCK, SlimDXKey.ScrollLock },
			{ SDLKey.SDL_SCANCODE_PAUSE, SlimDXKey.Pause },
			{ SDLKey.SDL_SCANCODE_INSERT, SlimDXKey.Insert },
			{ SDLKey.SDL_SCANCODE_HOME, SlimDXKey.Home },
			{ SDLKey.SDL_SCANCODE_PAGEUP, SlimDXKey.PageUp },
			{ SDLKey.SDL_SCANCODE_DELETE, SlimDXKey.Delete },
			{ SDLKey.SDL_SCANCODE_END, SlimDXKey.End },
			{ SDLKey.SDL_SCANCODE_PAGEDOWN, SlimDXKey.PageDown },
			{ SDLKey.SDL_SCANCODE_RIGHT, SlimDXKey.RightArrow },
			{ SDLKey.SDL_SCANCODE_LEFT, SlimDXKey.LeftArrow },
			{ SDLKey.SDL_SCANCODE_DOWN, SlimDXKey.DownArrow },
			{ SDLKey.SDL_SCANCODE_UP, SlimDXKey.UpArrow },
			{ SDLKey.SDL_SCANCODE_NUMLOCKCLEAR, SlimDXKey.NumberLock },
			{ SDLKey.SDL_SCANCODE_KP_DIVIDE, SlimDXKey.NumberPadSlash },
			{ SDLKey.SDL_SCANCODE_KP_MULTIPLY, SlimDXKey.NumberPadStar },
			{ SDLKey.SDL_SCANCODE_KP_MINUS, SlimDXKey.NumberPadMinus },
			{ SDLKey.SDL_SCANCODE_KP_PLUS, SlimDXKey.NumberPadPlus },
			{ SDLKey.SDL_SCANCODE_KP_ENTER, SlimDXKey.NumberPadEnter },
			{ SDLKey.SDL_SCANCODE_KP_1, SlimDXKey.NumberPad1 },
			{ SDLKey.SDL_SCANCODE_KP_2, SlimDXKey.NumberPad2 },
			{ SDLKey.SDL_SCANCODE_KP_3, SlimDXKey.NumberPad3 },
			{ SDLKey.SDL_SCANCODE_KP_4, SlimDXKey.NumberPad4 },
			{ SDLKey.SDL_SCANCODE_KP_5, SlimDXKey.NumberPad5 },
			{ SDLKey.SDL_SCANCODE_KP_6, SlimDXKey.NumberPad6 },
			{ SDLKey.SDL_SCANCODE_KP_7, SlimDXKey.NumberPad7 },
			{ SDLKey.SDL_SCANCODE_KP_8, SlimDXKey.NumberPad8 },
			{ SDLKey.SDL_SCANCODE_KP_9, SlimDXKey.NumberPad9 },
			{ SDLKey.SDL_SCANCODE_KP_0, SlimDXKey.NumberPad0 },
			{ SDLKey.SDL_SCANCODE_KP_PERIOD, SlimDXKey.NumberPadPeriod },
			{ SDLKey.SDL_SCANCODE_APPLICATION, SlimDXKey.Applications },
			{ SDLKey.SDL_SCANCODE_POWER, SlimDXKey.Power },
			{ SDLKey.SDL_SCANCODE_KP_EQUALS, SlimDXKey.NumberPadEquals },
			{ SDLKey.SDL_SCANCODE_F13, SlimDXKey.F13 },
			{ SDLKey.SDL_SCANCODE_F14, SlimDXKey.F14 },
			{ SDLKey.SDL_SCANCODE_F15, SlimDXKey.F15 },
			{ SDLKey.SDL_SCANCODE_STOP, SlimDXKey.Stop },
			{ SDLKey.SDL_SCANCODE_MUTE, SlimDXKey.Mute },
			{ SDLKey.SDL_SCANCODE_VOLUMEUP, SlimDXKey.VolumeUp },
			{ SDLKey.SDL_SCANCODE_VOLUMEDOWN, SlimDXKey.VolumeDown },
			{ SDLKey.SDL_SCANCODE_KP_COMMA, SlimDXKey.NumberPadComma },
			{ SDLKey.SDL_SCANCODE_LCTRL, SlimDXKey.LeftControl },
			{ SDLKey.SDL_SCANCODE_LSHIFT, SlimDXKey.LeftShift },
			{ SDLKey.SDL_SCANCODE_LALT, SlimDXKey.LeftAlt },
			{ SDLKey.SDL_SCANCODE_LGUI, SlimDXKey.LeftWindowsKey },
			{ SDLKey.SDL_SCANCODE_RCTRL, SlimDXKey.RightControl },
			{ SDLKey.SDL_SCANCODE_RSHIFT, SlimDXKey.RightShift },
			{ SDLKey.SDL_SCANCODE_RALT, SlimDXKey.RightAlt },
			{ SDLKey.SDL_SCANCODE_RGUI, SlimDXKey.RightWindowsKey },
			{ SDLKey.SDL_SCANCODE_AUDIONEXT, SlimDXKey.NextTrack },
			{ SDLKey.SDL_SCANCODE_AUDIOPREV, SlimDXKey.PreviousTrack },
			{ SDLKey.SDL_SCANCODE_AUDIOSTOP, SlimDXKey.MediaStop },
			{ SDLKey.SDL_SCANCODE_AUDIOPLAY, SlimDXKey.PlayPause },
			{ SDLKey.SDL_SCANCODE_MEDIASELECT, SlimDXKey.MediaSelect },
			{ SDLKey.SDL_SCANCODE_MAIL, SlimDXKey.Mail },
			{ SDLKey.SDL_SCANCODE_CALCULATOR, SlimDXKey.Calculator },
			{ SDLKey.SDL_SCANCODE_COMPUTER, SlimDXKey.MyComputer },
			{ SDLKey.SDL_SCANCODE_AC_SEARCH, SlimDXKey.WebSearch },
			{ SDLKey.SDL_SCANCODE_AC_HOME, SlimDXKey.WebHome },
			{ SDLKey.SDL_SCANCODE_AC_BACK, SlimDXKey.WebBack },
			{ SDLKey.SDL_SCANCODE_AC_FORWARD, SlimDXKey.WebForward },
			{ SDLKey.SDL_SCANCODE_AC_STOP, SlimDXKey.WebStop },
			{ SDLKey.SDL_SCANCODE_AC_REFRESH, SlimDXKey.WebRefresh },
			{ SDLKey.SDL_SCANCODE_AC_BOOKMARKS, SlimDXKey.WebFavorites },
			{ SDLKey.SDL_SCANCODE_SLEEP, SlimDXKey.Sleep },
			#endregion
		};
	}
}
