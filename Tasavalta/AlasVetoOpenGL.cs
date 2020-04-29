using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Tasavalta
{
    public partial class AlasVetoValikkoIkkuna : Form
    {

		[StructLayout(LayoutKind.Sequential)]
		struct TEXTMETRICW
		{
			public int tmHeight;
			public int tmAscent;
			public int tmDescent;
			public int tmInternalLeading;
			public int tmExternalLeading;
			public int tmAveCharWidth;
			public int tmMaxCharWidth;
			public int tmWeight;
			public int tmOverhang;
			public int tmDigitizeAspectX;
			public int tmDigitizeAspectY;
			public ushort tmFirstChar;
			public ushort tmLastChar;
			public ushort tmDefaultChar;
			public ushort tmBreakChar;
			public byte tmItalic;
			public byte tmUnderlined;
			public byte tmStruckOut;
			public byte tmPitchAndFamily;
			public byte tmCharSet;
		}

		[StructLayout(LayoutKind.Sequential)]
		struct RECT
		{
			public int left;
			public int top;
			public int right;
			public int bottom;
		}

		[StructLayout(LayoutKind.Sequential)]
		struct PAINTSTRUCT
		{
			public IntPtr hdc;
			public int fErase;
			public RECT rcPaint;
			public int fRestore;
			public int fIncUpdate;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
			public byte[] rgbReserved;
		}

		[StructLayout(LayoutKind.Sequential)]
		struct POINT
		{
			public int X;
			public int Y;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		class LOGFONT
		{
			public int lfHeight = 0;
			public int lfWidth = 0;
			public int lfEscapement = 0;
			public int lfOrientation = 0;
			public int lfWeight = 0;
			public byte lfItalic = 0;
			public byte lfUnderline = 0;
			public byte lfStrikeOut = 0;
			public byte lfCharSet = 0;
			public byte lfOutPrecision = 0;
			public byte lfClipPrecision = 0;
			public byte lfQuality = 0;
			public byte lfPitchAndFamily = 0;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string lfFaceName = string.Empty;
		}

		[StructLayout(LayoutKind.Sequential)]
		struct LOGBRUSH
		{
			public uint lbStyle;
			public uint lbColor;
			public uint lbHatch;
		}

		[StructLayout(LayoutKind.Sequential)]
		struct LOGPEN
		{
			public uint lopnStyle;
			public POINT lopnWidth;
			public uint lopnColor;
		}

		[DllImport("gdi32.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool GetTextMetrics(IntPtr hdc, out TEXTMETRICW lptm);

		[DllImport("user32.dll")]
		static extern IntPtr GetWindowDC(IntPtr hWnd);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool ReleaseDC(IntPtr hWnd, IntPtr hdc);

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

		[DllImport("gdi32.dll", CharSet = CharSet.Auto)]
		static extern IntPtr CreateFontIndirect([In, MarshalAs(UnmanagedType.LPStruct)] LOGFONT lplf);

		[DllImport("gdi32.dll")]
		static extern IntPtr CreateBrushIndirect([In] ref LOGBRUSH lplb);

		[DllImport("gdi32.dll")]
		static extern IntPtr CreatePenIndirect([In] ref LOGPEN lppn);

		[DllImport("user32.dll")]
		static extern IntPtr BeginPaint(IntPtr hWnd, out PAINTSTRUCT lpPaint);

		[DllImport("gdi32.dll")]
		static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool DrawFrameControl(IntPtr hdc, in RECT lpRect, uint uType, uint uState);

		[DllImport("gdi32.dll")]
		static extern uint SetBkColor(IntPtr hdc, uint crColor);

		[DllImport("gdi32.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool TextOut(IntPtr hdc, int nXStart, int nYStart, string lpString, int cbString);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool EndPaint(IntPtr hWnd, [In] ref PAINTSTRUCT lpPaint);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

		[DllImport("gdi32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool Rectangle(IntPtr hdc, int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);



		[DllImport("user32.dll", SetLastError = true)]
//		[return: MarshalAs(UnmanagedType.Bool)]
		static extern int SystemParametersInfo(int uiAction, uint uiParam, ref uint pvParam, int fWinIni);

		Singleton mSing;
		int kursoriRivilla = -1;
		int cyChar;
		uint cyClient;
		int iVscrollMax;
		int iDeltaPerLine, iAccumDelta;
		LOGFONT lf;
		LOGBRUSH br;
		LOGPEN pen;
		IntPtr sininen;
		IntPtr fontti;
		IntPtr reuna;

		protected override CreateParams CreateParams
		{

			//määritetään ikkuna, jossa ei ole kehyksiä
			get
			{
				CreateParams cp = base.CreateParams;
				cp.Style = unchecked((int)WinM.WS_POPUP | (int)WinM.WS_BORDER | (int)WinM.WS_SYSMENU);
				return cp;
			}
		}

		public AlasVetoValikkoIkkuna()
        {
            InitializeComponent();

			//ilmoittaudutaan valikkoon
			mSing = Singleton.AnnaIlmentyma;
			mSing.mValikko.mAlasVetoValikkoIkkuna = this;

			//Ikkunan sijainti pitää voida määritellä, se ei saa olla missä vain
			this.StartPosition = FormStartPosition.Manual;

		}

		protected override void WndProc(ref Message Msg)
		{

			RECT rect;
			PAINTSTRUCT ps;
			TEXTMETRICW tm;
			int ii, yy, iVertPos, iPaintBeg, iPaintEnd;
			IntPtr vanhaFontti = IntPtr.Zero;
			IntPtr vanhaVari = IntPtr.Zero;
			IntPtr vanhaKyna = IntPtr.Zero;
			uint vanhaTausta;
			IntPtr hdc = IntPtr.Zero;
			POINT point;
			uint ulScrollLines = 0;
			int SPI_GETWHEELSCROLLLINES = 104;
			int WHEEL_DELTA = 120;

			switch (Msg.Msg)
			{

				//tämä on oma windows viesti. Ilman tätä ei alasVetoValikkoIkkunaa voisi sulkea
				case (int)WinM.WM_VASENALHAALLA:
					{
						var xPos = (int) Msg.LParam;

						//otetaan win32 LOWORD
						xPos = xPos & 0x0000FFFF;
						int yPos = (int) Msg.LParam;

						//otetaan win32 HIWORD
						yPos = yPos >> 16;
						if ((int) Msg.WParam == 1)
						{
							GetWindowRect(this.Handle, out rect);
							if (rect.left <= xPos && xPos <= rect.right && rect.top <= yPos && yPos <= rect.bottom)
							{

								//ei koodia täällä
							}
							else
							{

								//asetetaan veirintäpalkki alkuun seuraavaa käyttökertaa odottamaan
								this.vScrollBar1.Value = 0;

								//sitten suljetaan ikkuna
								this.Hide();
								mSing.mValikko.mOpenGLIkkuna.mOnkoAlhaalla = false;
								mSing.mValikko.mOpenGLIkkuna.mSaakoKlikata2 = false;
							}
						}
						else
						{
							GetWindowRect(this.Handle, out rect);
							if (rect.left <= xPos && xPos <= rect.right - 16 && rect.top <= yPos && yPos <= rect.bottom)
							{

								//ei koodia täällä
							}
							else
							{

								//tämä tarvitaan sinisen taustavärin poistamiseen
								if (kursoriRivilla != -1)
								{
									kursoriRivilla = -1;
									this.Invalidate();
								}
							}
						}
						return;
					}

				//kun valintaIkkuna luodaan, luodaan myös Tahoma fontti, sininen brush ja sininen kynä
				case (int)WinM.WM_CREATE:
					{
						hdc = GetWindowDC(this.Handle);
						br.lbStyle = 0;   // BS_SOLID=0
						br.lbColor = 0x000000FF;
						sininen = CreateBrushIndirect(ref br);
						pen.lopnStyle = 0; // BS_SOLID=0
						pen.lopnColor = 0x000000FF;
						reuna = CreatePenIndirect(ref pen);
						GetTextMetrics(hdc, out tm);
						cyChar = tm.tmHeight + tm.tmExternalLeading;
						LOGFONT lf = new LOGFONT();
						lf.lfHeight = tm.tmHeight;
//						lf.lfFaceName = "Tahoma";
						lf.lfFaceName = "Arial";
						fontti = CreateFontIndirect(lf);
						ReleaseDC(this.Handle, hdc);

						//tämä on hiiren keskipyörää varten
						SystemParametersInfo(SPI_GETWHEELSCROLLLINES, 0, ref ulScrollLines, 0);
						if (ulScrollLines != 0)
						{
							iDeltaPerLine = WHEEL_DELTA / (int)ulScrollLines;
						}
						else
						{
							iDeltaPerLine = 0;
						}

						break;
					}

				case (int)WinM.WM_PAINT:
					{
						hdc =BeginPaint(this.Handle, out ps);

						//selvitetään vierityspalkin asento
						iVertPos = vScrollBar1.Value;

						//valitaan Tahoma fontti
						if (fontti != IntPtr.Zero)
						{
							vanhaFontti = SelectObject(hdc, fontti);
						}

						//piirretään vain se alue, joka on näkyvissä ikkunassa
						iPaintBeg = (int)Math.Max(0, iVertPos + (int)ps.rcPaint.top / cyChar);
						iPaintEnd = (int)Math.Min(mSing.mValikko.mOpenGLIkkuna.Valintoja1 + 
							mSing.mValikko.mOpenGLIkkuna.Valintoja2,
												iVertPos + (int)ps.rcPaint.bottom / cyChar);

						//piirretään listan 1 valinnat, jos niitä on näkyvissä
						if (iPaintBeg <= mSing.mValikko.mOpenGLIkkuna.Valintoja1)
						{
							for (int i = iPaintBeg; i < Math.Min(mSing.mValikko.mOpenGLIkkuna.Valintoja1, iPaintEnd); i++)
							{
								int pituus = mSing.mValikko.mOpenGLIkkuna.ValintaListaC1(i).Length;
								uint tila;
								if (i - iVertPos == kursoriRivilla)
								{
									vanhaVari =SelectObject(hdc, sininen);
									vanhaKyna =SelectObject(hdc, reuna);
									Rectangle(hdc, 0, (i - iVertPos) * cyChar, 254, 16 + (i - iVertPos) * cyChar);
									SelectObject(hdc, vanhaVari);
									SelectObject(hdc, vanhaKyna);
								}
								if (mSing.mValikko.mOpenGLIkkuna.ValintaListaB1(i) == true)
								{
									tila = 0 | 1024;   //DFCS_BUTTONCHECK=0   DFCS_CHECKED=1024
								}
								else
								{
									tila = 0;    //DFCS_BUTTONCHECK=0
								}
								rect.left = 0;
								rect.top = (i - iVertPos) * cyChar;
								rect.right = 16;
								rect.bottom = 16 + (i - iVertPos) * cyChar;
//								DrawFrameControl(hdc, in rect, 4, tila);     //DFC_BUTTON = 4
								if (i - iVertPos == kursoriRivilla)
								{
									vanhaTausta = SetBkColor(hdc, 0x000000FF);
									TextOut(hdc, 20, (i - iVertPos) * cyChar, mSing.mValikko.mOpenGLIkkuna.ValintaListaC1(i), pituus);
									SetBkColor(hdc, vanhaTausta);
								}
								else
								{
									TextOut(hdc, 20, (i - iVertPos) * cyChar, mSing.mValikko.mOpenGLIkkuna.ValintaListaC1(i), pituus);
								}
							}
						}

						//piirretään listan 2 valinnat, jos niitä on näkyvissä
						if (iPaintEnd >= mSing.mValikko.mOpenGLIkkuna.Valintoja1)
						{

							for (int i = mSing.mValikko.mOpenGLIkkuna.Valintoja1; i < iPaintEnd; i++)
							{
								int pituus = mSing.mValikko.mOpenGLIkkuna.ValintaListaC2(i - 
									mSing.mValikko.mOpenGLIkkuna.Valintoja1).Length + 1;
								string siirto = "*" + mSing.mValikko.mOpenGLIkkuna.ValintaListaC1(i -
									mSing.mValikko.mOpenGLIkkuna.Valintoja1);
								uint tila;
								if (i - iVertPos == kursoriRivilla)
								{
									vanhaVari = SelectObject(hdc, sininen);
									vanhaKyna = SelectObject(hdc, reuna);
									Rectangle(hdc, 0, (i - iVertPos) * cyChar, 300, 16 + (i - iVertPos) * cyChar);
									SelectObject(hdc, vanhaVari);
									SelectObject(hdc, vanhaKyna);
								}
								if (mSing.mValikko.mOpenGLIkkuna.ValintaListaB2(i - 
									mSing.mValikko.mOpenGLIkkuna.Valintoja1) == true)
								{
									tila = 0 | 1024;  // DFCS_BUTTONCHECK=0 DFCS_CHECKED=1024
								}
								else
								{
									tila = 0;   //DFCS_BUTTONCHECK=0
								}
								rect.left = 0;
								rect.top = (i - iVertPos) * cyChar;
								rect.right = 16;
								rect.bottom = 16 + (i - iVertPos) * cyChar;
//								DrawFrameControl(hdc, in rect, 4, tila);  //DFC_BUTTON=4
								if (i - iVertPos == kursoriRivilla)
								{
									vanhaTausta = SetBkColor(hdc, 0x000000FF);
									TextOut(hdc, 20, (i - iVertPos) * cyChar, siirto, pituus);
									SetBkColor(hdc, vanhaTausta);
								}
								else
								{
									TextOut(hdc, 20, (i - iVertPos) * cyChar, siirto, pituus);
								}
							}
						}

						//palautetaan vanha fontti
						if (fontti != IntPtr.Zero)
						{
							SelectObject(hdc, vanhaFontti);
						}
						EndPaint(this.Handle, ref ps);
						break;
					}

				case (int)WinM.WM_SIZE:
					{

						//asetetaan vierintäpalkin alue ja sivukoko
						hdc =GetWindowDC(this.Handle);
						GetTextMetrics(hdc, out tm);
						cyChar = tm.tmHeight + tm.tmExternalLeading;
						cyClient = (uint)Msg.LParam;

						//otetaan win32 HIWORD
						cyClient = cyClient >> 16;
						this.vScrollBar1.Minimum = 0;
						this.vScrollBar1.Maximum = Math.Max((mSing.mValikko.mOpenGLIkkuna.Valintoja1 +
							mSing.mValikko.mOpenGLIkkuna.Valintoja2) * cyChar - (int)cyClient, 0);
						this.vScrollBar1.Maximum += this.vScrollBar1.LargeChange;
						break;
					}

				case (int)WinM.WM_MOUSEWHEEL:
					{
						if (iDeltaPerLine == 0) break;
						int siirto = (int)Msg.WParam;

						//Win32 HIWORD
						siirto = siirto >> 16;
						iAccumDelta = iAccumDelta + siirto; // 120 or -120
						while (iAccumDelta >= iDeltaPerLine)
						{
							SendMessage(this.Handle, 277, 0, string.Empty);  //WM_VSCROLL=277  SB_LINEUP=0
							iAccumDelta = iAccumDelta - iDeltaPerLine;
						}
						while (iAccumDelta <= -iDeltaPerLine)
						{
							SendMessage(this.Handle, 277, 1, string.Empty);  //WM_VSCROLL=277  SB_LINEDOWN=1
							iAccumDelta = iAccumDelta + iDeltaPerLine;
						}
						return;
					}

				case (int)WinM.WM_VSCROLL:
					{

						iVertPos = this.vScrollBar1.Value;

						//Win32 LOWORD
						int siirto = (int)Msg.WParam;
						siirto = siirto & 0x0000FFFF;
						switch (siirto)
						{
							case 0:     //SB_LINEUP=0
								this.vScrollBar1.Value = this.vScrollBar1.Value - 1;
								break;
							case 1:  //SB_LINEDOWN=1
								this.vScrollBar1.Value = this.vScrollBar1.Value + 1;
								break;
							case 6:  //SB_TOP=6
								this.vScrollBar1.Value = this.vScrollBar1.Minimum;
								break;
							case 7:  //SB_BOTTOM=7
								this.vScrollBar1.Value = this.vScrollBar1.Maximum;
								break;
							case 2:   //SB_PAGEUP=2
								this.vScrollBar1.Value = this.vScrollBar1.Value - (int)cyClient;
								break;
							case 3:    //SB_PAGEDOWN=3
								this.vScrollBar1.Value = this.vScrollBar1.Value + (int)cyClient;
								break;
							case 4:  //SB_THUMBPOSITION=4
								siirto = (int)Msg.WParam;

								//Win32 HIWORD
								siirto = siirto >> 16;
								this.vScrollBar1.Value = siirto;
								break;
							case 8:  //SB_ENDSCROLL=8
								break;
							default:
								break;
						}

						if (this.vScrollBar1.Value != iVertPos)
						{
							Invalidate();
						}
						return;
					}

				case (int)WinM.WM_MOUSEMOVE:
					{
						int siirto = (int)Msg.LParam;

						//Win32 HIWORD
						siirto = siirto >> 16;
						siirto = siirto / cyChar;
						if (kursoriRivilla != siirto)
						{
							kursoriRivilla = siirto;
							Invalidate();
						}
						return;
					}

				case (int)WinM.WM_LBUTTONUP:
					{
						mSing.mValikko.mOpenGLIkkuna.TarkastaLayerit(kursoriRivilla + this.vScrollBar1.Value);
						Invalidate();
						break;
					}

			}

			base.WndProc(ref Msg);
		}
	}
}
