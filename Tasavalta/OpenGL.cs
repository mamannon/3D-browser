using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows.Threading;



namespace Tasavalta
{

	/// <summary>Values to pass to the GetDCEx method.</summary>
//	[Flags()]
	public enum DeviceContextValues : uint
	{
		/// <summary>DCX_WINDOW: Returns a DC that corresponds to the window rectangle rather
		/// than the client rectangle.</summary>
		Window = 0x00000001,
		/// <summary>DCX_CACHE: Returns a DC from the cache, rather than the OWNDC or CLASSDC
		/// window. Essentially overrides CS_OWNDC and CS_CLASSDC.</summary>
		Cache = 0x00000002,
		/// <summary>DCX_NORESETATTRS: Does not reset the attributes of this DC to the
		/// default attributes when this DC is released.</summary>
		NoResetAttrs = 0x00000004,
		/// <summary>DCX_CLIPCHILDREN: Excludes the visible regions of all child windows
		/// below the window identified by hWnd.</summary>
		ClipChildren = 0x00000008,
		/// <summary>DCX_CLIPSIBLINGS: Excludes the visible regions of all sibling windows
		/// above the window identified by hWnd.</summary>
		ClipSiblings = 0x00000010,
		/// <summary>DCX_PARENTCLIP: Uses the visible region of the parent window. The
		/// parent's WS_CLIPCHILDREN and CS_PARENTDC style bits are ignored. The origin is
		/// set to the upper-left corner of the window identified by hWnd.</summary>
		ParentClip = 0x00000020,
		/// <summary>DCX_EXCLUDERGN: The clipping region identified by hrgnClip is excluded
		/// from the visible region of the returned DC.</summary>
		ExcludeRgn = 0x00000040,
		/// <summary>DCX_INTERSECTRGN: The clipping region identified by hrgnClip is
		/// intersected with the visible region of the returned DC.</summary>
		IntersectRgn = 0x00000080,
		/// <summary>DCX_EXCLUDEUPDATE: Unknown...Undocumented</summary>
		ExcludeUpdate = 0x00000100,
		/// <summary>DCX_INTERSECTUPDATE: Unknown...Undocumented</summary>
		IntersectUpdate = 0x00000200,
		/// <summary>DCX_LOCKWINDOWUPDATE: Allows drawing even if there is a LockWindowUpdate
		/// call in effect that would otherwise exclude this window. Used for drawing during
		/// tracking.</summary>
		LockWindowUpdate = 0x00000400,
		/// <summary>DCX_USESTYLE: Undocumented, something related to WM_NCPAINT message.</summary>
		UseStyle = 0x00010000,
		/// <summary>DCX_VALIDATE When specified with DCX_INTERSECTUPDATE, causes the DC to
		/// be completely validated. Using this function with both DCX_INTERSECTUPDATE and
		/// DCX_VALIDATE is identical to using the BeginPaint function.</summary>
		Validate = 0x00200000,
	}

	public enum Anim :int
	{
		aja,
		seis,
		alkuun,
		eiKaytossa
	}

	public partial class OpenGL : Form
	{

		[StructLayout(LayoutKind.Sequential)]
		struct POINT
		{
			public int x;
			public int y;
		}

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		static extern int GetSystemMetrics(int numero);

		[DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

		[DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
		static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)]string lpFileName);

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool FreeLibrary(IntPtr hModule);

		[DllImport("user32.dll")]
		static extern short GetKeyState(int nVirtKey);

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool GetCursorPos(out POINT lpPoint);

		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll")]
		static extern IntPtr GetWindowDC(IntPtr hWnd);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool ReleaseDC(IntPtr hWnd, IntPtr hdc);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		delegate bool ONKOLAAJENNUKSIA(ref IntPtr ikkunaKahva, ref IntPtr hwndMain, ref IntPtr hwnd, ref bool eiValaisua);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		delegate bool ETEEN(
			[MarshalAs(UnmanagedType.LPWStr)]string tiedosto,
			ref bool eteen, ref bool taakse, ref int animaatioTila,
			IntPtr valintaListaB1,
			IntPtr[] valintaListaC1,
			IntPtr valintaListaB2,
			IntPtr[] valintaListaC2,
			ref int valintoja1, ref int valintoja2);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		delegate bool TAAKSE(
			[MarshalAs(UnmanagedType.LPWStr)]string tiedosto,
			ref bool eteen, ref bool taakse, ref int animaatioTila,
			IntPtr valintaListaB1,
			IntPtr[] valintaListaC1,
			IntPtr valintaListaB2,
			IntPtr[] valintaListaC2,
			ref int valintoja1, ref int valintoja2);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		delegate void TULOSTA();

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		delegate bool KAYNNISTACAD(
			[MarshalAs(UnmanagedType.LPWStr)]string tiedosto,
			 ref IntPtr ikkunaKahva, ref int leveys, ref int korkeus,
			 ref bool eteen, ref bool taakse, ref int animaatioTila,
			IntPtr valintaListaB1,
			IntPtr[] valintaListaC1,
			IntPtr valintaListaB2,
			IntPtr[] valintaListaC2,
			ref int valintoja1, ref int valintoja2);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		delegate void SAMMUTACAD();

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		delegate void MUUTAALA(int leveys, int korkeus);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		delegate void PYORAALAS();

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		delegate void PYORAYLOS();

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		delegate void VIEVALINNAT(
			IntPtr valintaListaB1,
			IntPtr[] valintaListaC1,
			IntPtr valintaListaB2,
			IntPtr[] valintaListaC2,
			ref int valintoja1, ref int valintoja2);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		delegate void ELOKUVA(int kytkimet);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		delegate void ANNAORIENTAATIO1(out float[] orientaatio);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		delegate void ASETAORIENTAATIO1(float[] orientaatio);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		delegate void PAIVITA();

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		delegate void HIIRILIIKKUU(ref int X, ref int Y);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		delegate void HIIRILIIKKUUVASEN(ref int X, ref int Y, ref int vanhaHiiriX, ref int vanhaHiiriY);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		delegate void HIIRILIIKKUUOIKEA(ref int X, ref int Y);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		delegate void HIIRILIIKKUUMOLEMMAT(ref int X, ref int Y, ref int vanhaHiiriX, ref int vanhaHiiriY);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		delegate void HIIRIPOISTUU();

		private readonly int SM_CXSCREEN = 0;
		bool mOpenglOnkoTuettu = false;
		bool mValmis = false;
		bool mAjetaan = false;
//		bool mVieTiedot = false;
		//		bool[] mValintaListaB1 = new bool[1000];
		//		bool[] mValintaListaB2 = new bool[1000];
		IntPtr mValintaListaB1;
		IntPtr mValintaListaB2;
		IntPtr[] mValintaListaC1 = new IntPtr[1000];
		IntPtr[] mValintaListaC2 = new IntPtr[1000];
		int mValintoja1 = 0;
		int mValintoja2 = 0;
		int vanhaHiiriX = 0;
		int vanhaHiiriY = 0;
		Timer mTimeri;
		IntPtr mDllKahva;
		IntPtr mIkkunaKahva;
		IntPtr mHwnd;
		Singleton mSing;
		OpenGL.ONKOLAAJENNUKSIA onkoLaajennuksia;
		OpenGL.ETEEN eteenI;
		OpenGL.TAAKSE taakseI;
		OpenGL.TULOSTA tulostaI;
		OpenGL.KAYNNISTACAD kaynnistaCad;
		OpenGL.SAMMUTACAD sammutaCad;
		OpenGL.MUUTAALA muutaAla;
		OpenGL.PYORAALAS pyoraAlas;
		OpenGL.PYORAYLOS pyoraYlos;
		OpenGL.VIEVALINNAT vieValinnat;
		OpenGL.ELOKUVA eloKuva;
		OpenGL.ANNAORIENTAATIO1 annaOrientaatio1;
		OpenGL.ASETAORIENTAATIO1 asetaOrientaatio1;
		OpenGL.PAIVITA paivita;
		OpenGL.HIIRILIIKKUU hiiriLiikkuu;
		OpenGL.HIIRILIIKKUUVASEN hiiriLiikkuuVasen;
		OpenGL.HIIRILIIKKUUOIKEA hiiriLiikkuuOikea;
		OpenGL.HIIRILIIKKUUMOLEMMAT hiiriLiikkuuMolemmat;
		OpenGL.HIIRIPOISTUU hiiriPoistuu;

//		public bool mSaakoTarkastaaLayerit { get; private set; } = true;
		public bool mOnkoAlhaalla { get; set; } = false;
		public bool mSaakoKlikata2 { get; set; } = false;

		//Getterit
		public int Valintoja1
		{
			get { return mValintoja1; }
		}

		public int Valintoja2
		{
			get { return mValintoja2; }
		}

		public bool ValintaListaB1(int index)
		{
			byte[] siirto = new byte[mValintoja1];
			Marshal.Copy(mValintaListaB1, siirto, 0, mValintoja1);
			if (siirto[index] != 0)
			{
				return true;
			}
			else
			{
				return false;
			}

		}

		public bool ValintaListaB2(int index)
		{
			byte[] siirto = new byte[mValintoja2];
			Marshal.Copy(mValintaListaB2, siirto, 0, mValintoja2);
			if (siirto[index] != 0)
			{
				return true;
			}
			else
			{
				return false;
			}

		}

		public string ValintaListaC1(int index)
		{
			char[] siirto1 = new char[30];
			Marshal.Copy(mValintaListaC1[index], siirto1, 0, 30);
			string siirto2 = new string(siirto1, 0, siirto1.Length);
			int loppu = siirto2.IndexOf('\0');
			siirto2 = siirto2.Remove(loppu);
			return siirto2;
		}

		public string ValintaListaC2(int index)
		{
			char[] siirto1 = new char[30];
			Marshal.Copy(mValintaListaC2[index], siirto1, 0, 30);
			string siirto2 = new string(siirto1, 0, siirto1.Length);
			int loppu = siirto2.IndexOf('\0');
			siirto2 = siirto2.Remove(loppu);
			return siirto2;
		}
/*
		//Vaikka C# on manageroitu, eli siinä on automaattinen roskienkerääjä, tämä luokka
		//sisältää manageroimattomia kenttiä, jotka pitää tuhota manuaalisesti deletorissa
		~OpenGL()
		{

			//tuhotaan taulukot, joista roskienkerääjä ei huolehdi
			for (int i = 0; i < mValintaListaC1.Length; i++)
			{
				Marshal.FreeHGlobal(mValintaListaC1[i]);
				Marshal.FreeHGlobal(mValintaListaC2[i]);
			}
			Marshal.FreeHGlobal(mValintaListaB1);
			Marshal.FreeHGlobal(mValintaListaB2);
		}
*/
		//OpenGLIkkunaa suljettaessa on pakotettava ikkunan tuhoaminen sekä osoittimen määrittäminen
		//null arvoiseksi, muuten ikkunan uudelleen avaaminen ei onnistu, kummallista kyllä
		protected override void OnClosing(CancelEventArgs e)
		{

			//ennen ikkunan sulkemista RunOpenGL.dll:n pitää tehdä lopetustoimet
			//estetään ensin päivitys...
			mValmis = false;

			//...ja sitten lähetetään pyyntö säikeiden lopettamiseksi ja muistin 
			//vapauttamiseksi...
			sammutaCad();

			//...ja lopuksi suljetaan RunOpenGL.dll
			FreeLibrary(mDllKahva);
			mDllKahva = IntPtr.Zero;

			//tuhotaan manageroimattomat taulukot, joista roskienkerääjä ei huolehdi
			for (int i = 0; i < mValintaListaC1.Length; i++)
			{
				Marshal.FreeHGlobal(mValintaListaC1[i]);
				Marshal.FreeHGlobal(mValintaListaC2[i]);
			}
			Marshal.FreeHGlobal(mValintaListaB1);
			Marshal.FreeHGlobal(mValintaListaB2);

			//paneelin device context pitää vapauttaa
			ReleaseDC(mHwnd, mIkkunaKahva);

			//lopetetaan OpenGLIkkuna pakottamalla ikkunan tuhoaminen ja osoitin null arvoiseksi
			this.Dispose();
			base.OnClosing(e);
			mSing.mValikko.mOpenGLIkkuna = null;
		}


		//OpenGL haluaa ikkunaKahvan, joka pysyy samana koko ohjelman suorituksen ajan
		protected override CreateParams CreateParams
		{
			get
			{
				Int32 CS_OWNDC = 0x20;
				CreateParams cp = base.CreateParams;
				cp.ClassStyle = cp.ClassStyle | CS_OWNDC;
				return cp;
			}
		}

		public OpenGL()
		{
			InitializeComponent();

			//Nämä pitää alustaa tässä
			mSing = Singleton.AnnaIlmentyma;
			for (int i = 0; i < mValintaListaC1.Length; i++)
			{
				mValintaListaC1[i] = Marshal.AllocHGlobal(60 * sizeof(byte));
				mValintaListaC2[i] = Marshal.AllocHGlobal(60 * sizeof(byte));
			}
			mValintaListaB1 = Marshal.AllocHGlobal(1000 * sizeof(byte));
			mValintaListaB2 = Marshal.AllocHGlobal(1000 * sizeof(byte));
			this.MouseWheel += HiiriPyoraPyorii;
			this.fokusaattori.Focus();

			//Säädetään ikkunan sijainti ja koko oikeaksi
			this.StartPosition = FormStartPosition.Manual;
			float leveysSuhde = ((float)GetSystemMetrics(SM_CXSCREEN) - 100.0f) / 1500.0f;
			int AWidth = (int)(1000 * leveysSuhde);
			int AHeight = (int)(700 * leveysSuhde);
			int ALeft = 50;
			int ATop = 100;
			this.SetBounds(ALeft, ATop, AWidth, AHeight);

			//OpenGL ikkunaa luotaessa osa komponenteista pitää luoda dynaamisesti.
			//OmaComboBox:
			layeriLista = new OmaComboBox();
			layeriLista.Dock = DockStyle.Top;
			layeriLista.Width = (int)(400 * leveysSuhde);
			layeriLista.AllowDrop = false;
			layeriLista.AutoCompleteMode = AutoCompleteMode.None;
			layeriLista.Items.Add("3D View");
			layeriLista.Items.Add("2D X-projection");
			layeriLista.Items.Add("2D Y-projection");
			layeriLista.Items.Add("2D Z-projection");
			layeriLista.DropDownStyle = ComboBoxStyle.DropDownList;
			flowLayoutPanel1.Controls.Add(layeriLista);

			//AlasVetoValikkoIkkuna:
			mSing.mValikko.mAlasVetoValikkoIkkuna = new AlasVetoValikkoIkkuna();
			this.AddOwnedForm(mSing.mValikko.mAlasVetoValikkoIkkuna);

			//Toinen ToolStrip CheckBoxComboBoxin oikealle puolelle
			ToolStrip ts = new ToolStrip();
			ts.Dock = DockStyle.Top;
			flowLayoutPanel1.Controls.Add(ts);

			//Play nappi:
			kaynnista = new ToolStripButton();
			kaynnista.Name = "kaynnista";
			kaynnista.Text = "Play";
			kaynnista.Dock = DockStyle.Top;
			kaynnista.Click += new EventHandler(this.KaynnistaClick);
			ts.Items.Add(kaynnista);

			//Stop/Rewind nappi:
			pysayta = new ToolStripButton();
			pysayta.Name = "pysayta";
			pysayta.Text = "Stop/Rewind";
			pysayta.Dock = DockStyle.Top;
			pysayta.Click += new EventHandler(this.PysaytaClick);
			ts.Items.Add(pysayta);

			//luodaan timeri OpenGL ikkunalle
			mTimeri = new Timer();
			mTimeri.Enabled = true;
			mTimeri.Interval = 50;
			mTimeri.Tick += new EventHandler(Timeri_Tick);
		}

		//Tämä funktio kellottaa OpenGL-näkymän päivitystä
		private void Timeri_Tick(object sender, EventArgs e)
		{ 

			//toimintoja saa tehdä vain, jos OpenGLIkkuna on valmis
			if (mValmis)
			{
				//Käsketään OpenGL:ää päivittämään näkymä
				paivita();

				//layeriListalle pitää mahdollisesti lähettää viesti
				int siirto = (short)GetKeyState(1);    //VK_LBUTTON=1
				if (Convert.ToBoolean(siirto & 0x8000) && mOnkoAlhaalla && mSaakoKlikata2)    //KEY_PRESSED=0x8000   //ollut alunperin 0xF0000000
				{

					//Täällä ollaan, jos käyttäjä on klikannut hiiren vasemalla napilla OpenGL ikkunaa ja
					//layeriLista on esillä. Lähetetään layeriListalle viesti, että sen pitää
					//sulkeutua.
					POINT point;
					GetCursorPos(out point);
					short x = (short)point.x;
					short y = (short)point.y;
					siirto = (point.y << 16) | (point.x);
					IntPtr lParam = (IntPtr)siirto;
					IntPtr wParam = (IntPtr)1;
					uint viesti = (uint)WinM.WM_VASENALHAALLA;
					if (mSing.mValikko.mAlasVetoValikkoIkkuna != null)
					SendMessage(mSing.mValikko.mAlasVetoValikkoIkkuna.Handle, viesti, wParam, lParam);

					//Jotta layeriListan näkymät tekstikentässä eivät vaihdu hiiren keskipyörää rullatessa,
					//vaan keskipyörää käyttämällä OpenGL näkymässä liikutaan, asetetaan focus
					this.fokusaattori.Focus();
				}
				/*
				else if (mOnkoAlhaalla)
				{

					//Täällä ollaan aina silloin, kun layeriLista on alhaalla. Täältä lähetetään viesti
					//layeriListalle, jotta layeriLista voisi päivittää näkymänsä. Tämä toiminto
					//on itse asiassa turha, sillä layeriListan näkymää ei tarvitse jatkuvasti päivittää
					//niin kuin OpenGL näkymää pitää
					//			    ::SetWindowPos(alasVetoValikkoIkkuna->Handle, HWND_TOP, 0, 0, 0, 0, SWP_NOACTIVATE | SWP_NOMOVE | SWP_NOSIZE);
					POINT point;
					GetCursorPos(out point);
					short x = (short)point.x;
					short y = (short)point.y;
					siirto = (point.y << 16) | (point.x);
					IntPtr lParam = (IntPtr)siirto;
					IntPtr wParam = (IntPtr)0;
					uint viesti = (uint)WinM.WM_VASENALHAALLA;
					if (mSing.mValikko.mAlasVetoValikkoIkkuna != null)
					SendMessage(mSing.mValikko.mAlasVetoValikkoIkkuna.Handle, viesti, wParam, lParam);
				}
				*/
			}

		}

		//Layerilistan otsikkona tulee aina olla käytössä oleva ulottuvuus sekä
		//ajantasainen tieto auki olevan CAD-tiedoston solideista ja listoista
		private void TuoValinnat()
		{

			//tässä luetaan manageroimattoman bool-taulukon dataa kopiomalla
			//se byte taulukkoon. Kukin 8 bitin byte vastaa yhtä bool arvoa
			byte[] siirto = new byte[1000];
			Marshal.Copy(mValintaListaB1, siirto, 0, mValintoja1);

			//Pitää erikseen täyttää layeriListan otsikko
			//nolla=false, muut arvot=true 
			if (siirto[0] != 0)
			{
				layeriLista.Text = "3D View";
			}
			else
			{
				if (siirto[1] != 0) layeriLista.Text = "2D X-projection";
				if (siirto[2] != 0) layeriLista.Text = "2D Y-projection";
				if (siirto[3] != 0) layeriLista.Text = "2D Z-projection";
			}
		}

		private void VieValinnat()
		{

			//sitten lähetetään tiedot RunOpenGL.dll:lle
			vieValinnat(mValintaListaB1, mValintaListaC1,
				mValintaListaB2, mValintaListaC2,
				ref mValintoja1, ref mValintoja2);

			//koska lähetetyissä tiedoissa saattaa olla ristiriitoja, jotka korjataan vasta
			//RunOpenGL.dll:ssä, meidän pitää tuoda valinnat takaisin
			TuoValinnat();

		}

		public void TarkastaLayerit(int index)
		{

			//kopioidaan kaikki valinnat manageroimattomasta muistista
			//byte taulukkoon, vaihdetaan arvo indeksin kohdalla ja 
			//kopiodaan kaikki valinnat takaisin manageroimattomaan muistiin.
			//Ensin katsotaan osuuko indeksi solideihin
			index = Math.Max(index, 0);
			byte[] siirto = new byte[1000];
			if (index < mValintoja1)
			{
				Marshal.Copy(mValintaListaB1, siirto, 0, mValintoja1);
				if (siirto[index] == 0)
				{
					siirto[index] = 1;
				}
				else
				{
					siirto[index] = 0;
				}
				Marshal.Copy(siirto, 0, mValintaListaB1, mValintoja1);
			}
			

			//sitten katsotaan osuiko indeksi sittenkin listoihin
			if (index >= mValintoja1)
			{
				Marshal.Copy(mValintaListaB2, siirto, 0, mValintoja2);
				if (siirto[index - mValintoja1] == 0)
				{
					siirto[index - mValintoja1] = 1;
				}
				else
				{
					siirto[index - mValintoja1] = 0;
				}
				Marshal.Copy(siirto, 0, mValintaListaB2, mValintoja2);
			}

			//sitten lähetetään tiedot RunOpenGL.dll:lle
			vieValinnat(mValintaListaB1, mValintaListaC1,
				mValintaListaB2, mValintaListaC2,
				ref mValintoja1, ref mValintoja2);

			//koska lähetetyissä tiedoissa saattaa olla ristiriitoja, jotka korjataan vasta
			//RunOpenGL.dll:ssä, meidän pitää tuoda valinnat takaisin
			TuoValinnat();
		}

		public bool AvaaCAD()
		{
			bool siirto1 = false, siirto2 = false, kaikkiKunnossa = true;
			int siirto3, siirto4, siirto5;

			//openGLIkkuna pitää tuoda näkyviin
			this.Show();

			//jos olemme ensimmäistä kertaa avaamassa CAD näkymää...
			if (mDllKahva == IntPtr.Zero)
			{

				//tässä funktiossa otetaan käyttöön dynaaminen linkkikirjasto RunOpenGL.dll
				string lib = "RunOpenGL.dll";
				mDllKahva = LoadLibrary(lib);
				if (mDllKahva == IntPtr.Zero)
				{
					kaikkiKunnossa = false;
					goto virhe;
				}

				//ladataan kaikki dll-funktiot
				IntPtr siirto = GetProcAddress(mDllKahva, "onkoLaajennuksia");
				if (siirto == IntPtr.Zero)
				{
					siirto = GetProcAddress(mDllKahva, "_onkoLaajennuksia");
					if (siirto == IntPtr.Zero)
					{
						kaikkiKunnossa = false;
						goto virhe;
					}
				}
				onkoLaajennuksia = (ONKOLAAJENNUKSIA)
					Marshal.GetDelegateForFunctionPointer(siirto, typeof(ONKOLAAJENNUKSIA));
				siirto = GetProcAddress(mDllKahva, "kaynnistaCad");
				if (siirto == IntPtr.Zero)
				{
					siirto = GetProcAddress(mDllKahva, "_kaynnistaCad");
					if (siirto == IntPtr.Zero)
					{
						kaikkiKunnossa = false;
						goto virhe;
					}
				}
				kaynnistaCad = (KAYNNISTACAD)
					Marshal.GetDelegateForFunctionPointer(siirto, typeof(KAYNNISTACAD));
				siirto = GetProcAddress(mDllKahva, "taakse");
				if (siirto == IntPtr.Zero)
				{
					siirto = GetProcAddress(mDllKahva, "_taakse");
					if (siirto == IntPtr.Zero)
					{
						kaikkiKunnossa = false;
						goto virhe;
					}
				}
				taakseI = (TAAKSE)
					Marshal.GetDelegateForFunctionPointer(siirto, typeof(TAAKSE));
				siirto = GetProcAddress(mDllKahva, "eteen");
				if (siirto == IntPtr.Zero)
				{
					siirto = GetProcAddress(mDllKahva, "_eteen");
					if (siirto == IntPtr.Zero)
					{
						kaikkiKunnossa = false;
						goto virhe;
					}
				}
				eteenI = (ETEEN)
					Marshal.GetDelegateForFunctionPointer(siirto, typeof(ETEEN));
				siirto = GetProcAddress(mDllKahva, "muutaAla");
				if (siirto == IntPtr.Zero)
				{
					siirto = GetProcAddress(mDllKahva, "_muutaAla");
					if (siirto == IntPtr.Zero)
					{
						kaikkiKunnossa = false;
						goto virhe;
					}
				}
				muutaAla = (MUUTAALA)
					Marshal.GetDelegateForFunctionPointer(siirto, typeof(MUUTAALA));
				siirto = GetProcAddress(mDllKahva, "vieValinnat");
				if (siirto == IntPtr.Zero)
				{
					siirto = GetProcAddress(mDllKahva, "_vieValinnat");
					if (siirto == IntPtr.Zero)
					{
						kaikkiKunnossa = false;
						goto virhe;
					}
				}
				vieValinnat = (VIEVALINNAT)
					Marshal.GetDelegateForFunctionPointer(siirto, typeof(VIEVALINNAT));
				siirto = GetProcAddress(mDllKahva, "pyoraAlas");
				if (siirto == IntPtr.Zero)
				{
					siirto = GetProcAddress(mDllKahva, "_pyoraAlas");
					if (siirto == IntPtr.Zero)
					{
						kaikkiKunnossa = false;
						goto virhe;
					}
				}
				pyoraAlas = (PYORAALAS)
					Marshal.GetDelegateForFunctionPointer(siirto, typeof(PYORAALAS));
				siirto = GetProcAddress(mDllKahva, "pyoraYlos");
				if (siirto == IntPtr.Zero)
				{
					siirto = GetProcAddress(mDllKahva, "_pyoraYlos");
					if (siirto == IntPtr.Zero)
					{
						kaikkiKunnossa = false;
						goto virhe;
					}
				}
				pyoraYlos = (PYORAYLOS)
					Marshal.GetDelegateForFunctionPointer(siirto, typeof(PYORAYLOS));
				siirto = GetProcAddress(mDllKahva, "tulosta");
				if (siirto == IntPtr.Zero)
				{
					siirto = GetProcAddress(mDllKahva, "_tulosta");
					if (siirto == IntPtr.Zero)
					{
						kaikkiKunnossa = false;
						goto virhe;
					}
				}
				tulostaI = (TULOSTA)
					Marshal.GetDelegateForFunctionPointer(siirto, typeof(TULOSTA));
				siirto = GetProcAddress(mDllKahva, "eloKuva");
				if (siirto == IntPtr.Zero)
				{
					siirto = GetProcAddress(mDllKahva, "_eloKuva");
					if (siirto == IntPtr.Zero)
					{
						kaikkiKunnossa = false;
						goto virhe;
					}
				}
				eloKuva = (ELOKUVA)
					Marshal.GetDelegateForFunctionPointer(siirto, typeof(ELOKUVA));
				siirto = GetProcAddress(mDllKahva, "sammutaCad");
				if (siirto == IntPtr.Zero)
				{
					siirto = GetProcAddress(mDllKahva, "_sammutaCad");
					if (siirto == IntPtr.Zero)
					{
						kaikkiKunnossa = false;
						goto virhe;
					}
				}
				sammutaCad = (SAMMUTACAD)
					Marshal.GetDelegateForFunctionPointer(siirto, typeof(SAMMUTACAD));
				siirto = GetProcAddress(mDllKahva, "annaOrientaatio1");
				if (siirto == IntPtr.Zero)
				{
					siirto = GetProcAddress(mDllKahva, "_annaOrientaatio1");
					if (siirto == IntPtr.Zero)
					{
						kaikkiKunnossa = false;
						goto virhe;
					}
				}
				annaOrientaatio1 = (ANNAORIENTAATIO1)
					Marshal.GetDelegateForFunctionPointer(siirto, typeof(ANNAORIENTAATIO1));
				siirto = GetProcAddress(mDllKahva, "asetaOrientaatio1");
				if (siirto == IntPtr.Zero)
				{
					siirto = GetProcAddress(mDllKahva, "_asetaOrientaatio1");
					if (siirto == IntPtr.Zero)
					{
						kaikkiKunnossa = false;
						goto virhe;
					}
				}
				asetaOrientaatio1 = (ASETAORIENTAATIO1)
					Marshal.GetDelegateForFunctionPointer(siirto, typeof(ASETAORIENTAATIO1));
				siirto = GetProcAddress(mDllKahva, "paivita");
				if (siirto == IntPtr.Zero)
				{
					siirto = GetProcAddress(mDllKahva, "_paivita");
					if (siirto == IntPtr.Zero)
					{
						kaikkiKunnossa = false;
						goto virhe;
					}
				}
				paivita = (PAIVITA)
					Marshal.GetDelegateForFunctionPointer(siirto, typeof(PAIVITA));
				siirto = GetProcAddress(mDllKahva, "hiiriLiikkuu");
				if (siirto == IntPtr.Zero)
				{
					siirto = GetProcAddress(mDllKahva, "_hiiriLiikkuu");
					if (siirto == IntPtr.Zero)
					{
						kaikkiKunnossa = false;
						goto virhe;
					}
				}
				hiiriLiikkuu = (HIIRILIIKKUU)
					Marshal.GetDelegateForFunctionPointer(siirto, typeof(HIIRILIIKKUU));
				siirto = GetProcAddress(mDllKahva, "hiiriLiikkuuVasen");
				if (siirto == IntPtr.Zero)
				{
					siirto = GetProcAddress(mDllKahva, "_hiiriLiikkuuVasen");
					if (siirto == IntPtr.Zero)
					{
						kaikkiKunnossa = false;
						goto virhe;
					}
				}
				hiiriLiikkuuVasen = (HIIRILIIKKUUVASEN)
					Marshal.GetDelegateForFunctionPointer(siirto, typeof(HIIRILIIKKUUVASEN));
				siirto = GetProcAddress(mDllKahva, "hiiriLiikkuuOikea");
				if (siirto == IntPtr.Zero)
				{
					siirto = GetProcAddress(mDllKahva, "_hiiriLiikkuuOikea");
					if (siirto == IntPtr.Zero)
					{
						kaikkiKunnossa = false;
						goto virhe;
					}
				}
				hiiriLiikkuuOikea = (HIIRILIIKKUUOIKEA)
					Marshal.GetDelegateForFunctionPointer(siirto, typeof(HIIRILIIKKUUOIKEA));
				siirto = GetProcAddress(mDllKahva, "hiiriLiikkuuMolemmat");
				if (siirto == IntPtr.Zero)
				{
					siirto = GetProcAddress(mDllKahva, "_hiiriLiikkuuMolemmat");
					if (siirto == IntPtr.Zero)
					{
						kaikkiKunnossa = false;
						goto virhe;
					}
				}
				hiiriLiikkuuMolemmat = (HIIRILIIKKUUMOLEMMAT)
					Marshal.GetDelegateForFunctionPointer(siirto, typeof(HIIRILIIKKUUMOLEMMAT));
				siirto = GetProcAddress(mDllKahva, "hiiriPoistuu");
				if (siirto == IntPtr.Zero)
				{
					siirto = GetProcAddress(mDllKahva, "_hiiriPoistuu");
					if (siirto == IntPtr.Zero)
					{
						kaikkiKunnossa = false;
						goto virhe;
					}
				}
				hiiriPoistuu = (HIIRIPOISTUU)
					Marshal.GetDelegateForFunctionPointer(siirto, typeof(HIIRIPOISTUU));
			}


			mHwnd = paneeli.Handle;
			mIkkunaKahva = GetWindowDC(mHwnd);

		virhe:

			if (mDllKahva != IntPtr.Zero && mTimeri.Enabled && kaikkiKunnossa)
			{

				//kun RunOpenGL.dll on käytössä, selvitetään, tukeeko tietokone OpenGL laajennuksia,
				//jos sitä ei ole jo tehty
				if (mOpenglOnkoTuettu == false)
				{
					bool siirto = mSing.mValikko.mEiValaisua;
					mOpenglOnkoTuettu = onkoLaajennuksia(ref mIkkunaKahva, ref mSing.mValikko.mHwnd, ref mHwnd, ref siirto);

					if (!mOpenglOnkoTuettu)
					{

						//minkäänlaista OpenGL:ää ei ole saatavilla
						MessageBox.Show("Unable to lauch OpenGL. Please check following:\n\r" +

									"Ensure that you have a graphics driver installed on a computer.\n\r"
 +
									"Ensure that your computer uses a separate graphics card.\n\r" +

									"Especially laptops may have two GPU's: one integrated into\n\r" +

									"motherboard and another separate card. In this case the\n\r" +

									"integrated GPU unfortunately may be in use by default. However, if the\n\r" +

									"integrated GPU is the only one, Tasavalta should run correctly.");
						FreeLibrary(mDllKahva);
						mDllKahva = IntPtr.Zero;
						this.Close();
						return false;
					}
					else
					{

						//Meidän on pakko saada uusi device context (hdc), jotta OpenGL näkymään saadaan 
						//optimaaliset asetukset. Uusi hdc saadaan varmuudella vain luomalla paneeli uudestaan:
						ReleaseDC(mHwnd, mIkkunaKahva);
						paneeli.Dispose();
						paneeli = new Panel();
						this.paneeli.AutoSize = true;
						this.paneeli.Dock = System.Windows.Forms.DockStyle.Fill;
						this.paneeli.Location = new System.Drawing.Point(0, flowLayoutPanel1.Height);
						this.paneeli.Margin = new System.Windows.Forms.Padding(2);
						this.paneeli.Name = "paneeli";
						this.paneeli.TabIndex = 1;
						this.paneeli.MouseClick += new System.Windows.Forms.MouseEventHandler(this.paneeliClick);
						this.paneeli.MouseLeave += new System.EventHandler(this.HiiriPoistuu);
						this.paneeli.MouseMove += new System.Windows.Forms.MouseEventHandler(this.HiiriLiikkuu);
						this.Controls.Add(paneeli);
						mHwnd = paneeli.Handle;
						mIkkunaKahva = GetWindowDC(mHwnd);
					}
				}

				//sitten voimme viimeinkin ottaa OpenGL:n käyttöön, joko laajennuksilla tai ilman
				siirto3 = paneeli.Width;
				siirto4 = paneeli.Height;
				siirto5 = 0;
				if (!kaynnistaCad(mSing.mValikko.mTiedosto, ref mIkkunaKahva,
								ref siirto3, ref siirto4, ref siirto1, ref siirto2, ref siirto5,
								mValintaListaB1, mValintaListaC1, mValintaListaB2, mValintaListaC2,
								ref mValintoja1, ref mValintoja2))
				{
					FreeLibrary(mDllKahva);
					mDllKahva = IntPtr.Zero;
					this.Close();
					return false;
				}

				//nyt voimme mahdollistaa OpenGL piirtämisen
				mValmis = true;

				//avattu tiedosto pitää tallentaa, esittää otsikkopalkissa ja merkitä, että se
				//on suoraan saatavilla päävalikosta
				mSing.mValikko.mCADTiedosto = mSing.mValikko.mTiedosto;
				if (mSing.mValikko.mTiedosto.Length != 0)
				{
					string siirto = "CAD view - ";
					this.Text = siirto + mSing.mValikko.mTiedosto;
				}
				else
				{
					this.Text = "CAD view";
				}
				mSing.mValikko.TuoEsille(mSing.mValikko.mTiedosto, 'C');

				taakse.Enabled = siirto2;
				eteen.Enabled = siirto1;
				if (siirto5 == (int)Anim.aja)
				{
					kaynnista.Enabled = true;
				}
				else
				{
					kaynnista.Enabled = false;
				}
				pysayta.Enabled = false;
				TuoValinnat();
				mSing.mValikko.mPaivitetaanko = true;
				return true;
			}
			else
			{

				//jos kirjastoa RunOpenGL.dll ei pystytä lataamaan, ilmoitetaan siitä käyttäjälle
				MessageBox.Show("Unable to load RunOpenGL.dll, cudart32_50_35.dll\n\r" +

							"or some other dependency library.");
				this.Close();
				return false;
			}
		}

		//Tällä funktiolla kerrotaan RunOpenGL.dll:lle mistä kuvakulmasta avoinna olevaa 
		//CAD tiedostoa halutaan katsoa
		public void AsetaOrientaatio(float[] orientaatio)
		{
			asetaOrientaatio1(orientaatio);
		}

		//Tällä funktiolla RunOpenGL.dll kertoo avoinna olevan CAD tiedoston kuvakulman
		public void AnnaOrientaatio(float[] orientaatio)
		{
			annaOrientaatio1(out orientaatio);
		}

		//Tämä metodi ottaa käyttäjän näppäimistön w tai s kirjaimen painalluksen
		private void NappaimistoClick(object sender, KeyPressEventArgs e)
		{
			switch (e.KeyChar)
			{
				case (char)83:
					pyoraAlas(); pyoraAlas();
					break;
				case (char)115:
					pyoraAlas(); pyoraAlas();
					break;
				case (char)87:
					pyoraYlos(); pyoraYlos();
					break;
				case (char)119:
					pyoraYlos(); pyoraYlos();
					break;
			}
		}

		//Käyttäjä on klikannut OpenGLIkkunan back-nappulaa
		private void TaakseClick(object sender, EventArgs e)
		{
			bool siirto1 = false, siirto2 = false;
			int siirto3 = 0;
			string tiedosto = string.Empty;

			//käyttäjä on klikannut taakse nappia
			if (mDllKahva != IntPtr.Zero)
			{

				taakseI(tiedosto, ref siirto1, ref siirto2, ref siirto3, mValintaListaB1, mValintaListaC1,
							mValintaListaB2, mValintaListaC2, ref mValintoja1, ref mValintoja2);
				this.eteen.Enabled = siirto1;
				this.taakse.Enabled = siirto2;
				if (tiedosto.Length != 0)
				{
					string siirto = "CAD view - ";
					this.Text = siirto + tiedosto;
				}
				else
				{
					this.Text = "CAD view";
				}
				if (siirto3 == (int)Anim.aja)
				{
					this.kaynnista.Enabled = true;
					this.pysayta.Enabled = false;
				}
				else if (siirto3 == (int)Anim.seis)
				{
					this.kaynnista.Enabled = false;
					this.pysayta.Enabled = true;
				}
				else if (siirto3 == (int)Anim.alkuun)
				{
					this.kaynnista.Enabled = true;
					this.pysayta.Enabled = true;
				}
				else
				{
					this.kaynnista.Enabled = false;
					this.pysayta.Enabled = false;
				}
				TuoValinnat();
					
				//alasVetoValikkoIkkuna pitää sulkea
				if (mSing != null && mSing.mValikko.mAlasVetoValikkoIkkuna != null)
				mSing.mValikko.mAlasVetoValikkoIkkuna.Hide();
				mOnkoAlhaalla = false;
			}
		}

		private void EteenClick(object sender, EventArgs e)
		{
			bool siirto1 = false, siirto2 = false;
			int siirto3 = 0;
			string tiedosto = string.Empty;

			//käyttäjä on klikannut eteen nappia
			if (mDllKahva != IntPtr.Zero)
			{

				eteenI(tiedosto, ref siirto1, ref siirto2, ref siirto3, mValintaListaB1, mValintaListaC1,
						   mValintaListaB2, mValintaListaC2, ref mValintoja1, ref mValintoja2);
					this.eteen.Enabled = siirto1;
					this.taakse.Enabled = siirto2;
					if (tiedosto.Length != 0)
					{
						string siirto = "CAD view - ";
						this.Text = siirto + tiedosto;
					}
					else
					{
						this.Text = "CAD view";
					}
					if (siirto3 == (int)Anim.aja)
					{
						this.kaynnista.Enabled = true;
						this.pysayta.Enabled = false;
					}
					else if (siirto3 == (int)Anim.seis)
					{
						this.kaynnista.Enabled = false;
						this.pysayta.Enabled = true;
					}
					else if (siirto3 == (int)Anim.alkuun)
					{
						this.kaynnista.Enabled = true;
						this.pysayta.Enabled = true;
					}
					else
					{
						this.kaynnista.Enabled = false;
						this.pysayta.Enabled = false;
					}
					TuoValinnat();

					//alasVetoValikkoIkkuna pitää sulkea
					if (mSing.mValikko.mAlasVetoValikkoIkkuna != null)
					mSing.mValikko.mAlasVetoValikkoIkkuna.Hide();
					mOnkoAlhaalla = false;
			}
		}

		//tämä metodi välittää käyttäjän tulostuspyynnön RunOpenGL.dll kirjastoon
		private void Tulostetaan(object sender, EventArgs e)
		{
			tulostaI();
		}


		//Tätä metodia kutsutaan, jos käyttäjä muuttaa OpenGLIkkunan kokoa
		private void MuutaKokoa(object sender, EventArgs e)
		{

			//jos ikkunan kokoa muutetaan, pitää alasvetovalikkoikkuna sulkea
			if (mSing != null && mSing.mValikko.mAlasVetoValikkoIkkuna != null)
				mSing.mValikko.mAlasVetoValikkoIkkuna.Hide();
			mOnkoAlhaalla = false;

			//ilmoitetaan OpenGLIkkunan koon muutoksesta RunOpenGL.dll:lle
			if (muutaAla != null) muutaAla(this.Width, this.Height);
		}

		//tämä metodi toimii vain, jos paneelilla tai sillä kontrollilla, jolle tämä metodi
		//on osoitettu, on focus. Hiiren keskipyorän toiminta edellyttää focusta, toisin kuin 
		//hiiren nappien käyttö eli klikkaukset
		private void HiiriPyoraPyorii(object sender, MouseEventArgs e)
		{

			if (e.Delta >= 0)
			{

				//loitontaa näkymää
				pyoraAlas();
				pyoraAlas();
			}
			else
			{

				//lähentää näkymää
				pyoraYlos();
				pyoraYlos();
			}
		}

		//käyttäjä haluaa käynnistää animaation CAD-näkymässä
		private void KaynnistaClick(object sender, EventArgs e)
		{
			eloKuva((int)Anim.aja);
			mAjetaan = true;
			kaynnista.Enabled = false;
			pysayta.Enabled = true;
		}

		//Käyttäjä haluaa pysäyttää animaation tai jos se on pysäytetty,
		//asettaa animaation alkutilanteeseen
		private void PysaytaClick(object sender, EventArgs e)
		{
			if (mAjetaan)
			{
				eloKuva((int)Anim.seis);
				mAjetaan = false;
				kaynnista.Enabled = true;
			}
			else
			{
				eloKuva((int)Anim.alkuun);
				pysayta.Enabled = false;
				kaynnista.Enabled = true;
			}
		}

		//käyttäjän klikatessa paikallaan olevan hiiren nappulaa paneelin alueella tämä
		//metodi aktivoituu
		private void paneeliClick(object sender, MouseEventArgs e)
		{
			int X = e.X;
			int Y = e.Y;

			//tämä tarvitaan layeriListan alasvetovalikkoa suljettaessa. Tällöin
			//ei tehdä muuta
			if (mOnkoAlhaalla && mSaakoKlikata2)
			{
				vanhaHiiriX = X;
				vanhaHiiriY = Y;
				return;
			}

			//selvitetään, kumpaa hiiren nappia käyttäjä on klikannut
			//vasen
			if (e.Button == MouseButtons.Left)
			{
				hiiriLiikkuuVasen(ref X, ref Y, ref vanhaHiiriX, ref vanhaHiiriY);
				vanhaHiiriX = X;
				vanhaHiiriY = Y;
				return;
			}

			//oikea
			if (e.Button == MouseButtons.Right)
			{
				hiiriLiikkuuOikea(ref X, ref Y);
				vanhaHiiriX = X;
				vanhaHiiriY = Y;
				return;
			}
		}

		//käyttäjän liikuttaessa hiirtä paneelin alueella tämä
		//metodi aktivoituu
		private void HiiriLiikkuu(object sender, MouseEventArgs e)
		{
			bool vasen = e.Button == MouseButtons.Left ? true : false;
			bool oikea = e.Button == MouseButtons.Right ? true : false;
			int X = e.X;
			int Y = e.Y;

			//tämä tarvitaan layeriListan alasvetovalikkoa suljettaessa. Tällöin
			//ei tehdä muuta
			if (mOnkoAlhaalla && mSaakoKlikata2)
			{
				vanhaHiiriX = X;
				vanhaHiiriY = Y;
				return;
			}

			//lisäksi tähän funktioon sisältyy neljä eri RunOpenGL.dll kutsua:
			//hiiriLiikkuu
			if (!vasen && !oikea)
			{
				hiiriLiikkuu(ref X, ref Y);
				vanhaHiiriX = X;
				vanhaHiiriY = Y;
				return;
			}

			//hiiriLiikkuuVasen
			if (vasen && !oikea)
			{
				hiiriLiikkuuVasen(ref X, ref Y, ref vanhaHiiriX, ref vanhaHiiriY);
				vanhaHiiriX = X;
				vanhaHiiriY = Y;
				return;
			}

			//hiiriLiikkuuOikea
			if (!vasen && oikea)
			{
				hiiriLiikkuuOikea(ref X, ref Y);
				vanhaHiiriX = X;
				vanhaHiiriY = Y;
				return;
			}

			//hiiri liikkuu vasen ja oikea pohjassa
			if (vasen && oikea)
			{
				hiiriLiikkuuMolemmat(ref X, ref Y, ref vanhaHiiriX, ref vanhaHiiriY);
				vanhaHiiriX = X;
				vanhaHiiriY = Y;
				return;
			}
		}

		//tätä metodia kutsutaan silloin, kun hiiri poistuu OpenGL-näkymän eli paneelin
		//alueelta
		private void HiiriPoistuu(object sender, EventArgs e)
		{
			hiiriPoistuu();
		}
	}


	public class OmaComboBox : ComboBox
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

		[DllImport("gdi32.dll", CharSet = CharSet.Auto)]
		static extern bool GetTextMetrics(IntPtr hdc, out TEXTMETRICW lptm);

		[DllImport("user32.dll")]
		static extern IntPtr GetWindowDC(IntPtr hWnd);

		[DllImport("user32.dll")]
		static extern bool ReleaseDC(IntPtr hWnd, IntPtr hdc);

		[DllImport("user32.dll", SetLastError = true)]
		static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

		private IntPtr mAlasVetoValikkoHWND;
		Singleton mSing;
		public OmaComboBox()
		{
			mSing = Singleton.AnnaIlmentyma;
		}

		protected override void WndProc(ref Message Msg)
		{
			if (Msg.Msg == (int)WinM.WM_CTLCOLORLISTBOX)
			{

				//jos meillä ei vielä ole layeriListan alasvetovalikkoa avattu...
				if (mSing.mValikko.mOpenGLIkkuna.mOnkoAlhaalla != true)
				{
					IntPtr hWnd = (IntPtr)Msg.LParam;
					if (hWnd != IntPtr.Zero && hWnd != this.Handle)
					{

						//...niin nyt avaamme alasvetovalikkoikkunan halutun kokoiseksi
						mAlasVetoValikkoHWND = hWnd;
						RECT rect;
						TEXTMETRICW tm;
						IntPtr hdc = GetWindowDC(mAlasVetoValikkoHWND);
						GetTextMetrics(hdc, out tm);
						int cyChar = tm.tmHeight + tm.tmExternalLeading;
						ReleaseDC(mAlasVetoValikkoHWND, hdc);
						GetWindowRect(mAlasVetoValikkoHWND, out rect);
						if (mSing.mValikko.mOpenGLIkkuna.Valintoja1 +
							mSing.mValikko.mOpenGLIkkuna.Valintoja2 < 30)
						{
							rect.bottom = cyChar * (mSing.mValikko.mOpenGLIkkuna.Valintoja1 +
								mSing.mValikko.mOpenGLIkkuna.Valintoja2);
						}
						else
						{
							rect.bottom = cyChar * 6;
						}
						this.Enabled = false;
						this.Enabled = true;
						mSing.mValikko.mOpenGLIkkuna.mOnkoAlhaalla = true;
						mSing.mValikko.mOpenGLIkkuna.mSaakoKlikata2 = true;
						//						Form1->saakoKlikata1 = false;
						rect.right = rect.right - rect.left;
						mSing.mValikko.mAlasVetoValikkoIkkuna.Left = rect.left;
						mSing.mValikko.mAlasVetoValikkoIkkuna.Top = rect.top;
						mSing.mValikko.mAlasVetoValikkoIkkuna.Width = rect.right;
						mSing.mValikko.mAlasVetoValikkoIkkuna.Height = rect.bottom;
						mSing.mValikko.mAlasVetoValikkoIkkuna.Show();
						return;
					}
				}
			}
/*
			if (Msg.Msg == (int)WinM.WM_LBUTTONDOWN && mSing.mValikko.mOpenGLIkkuna.mSaakoKlikata2)
			{

				//jos meillä on alasvetovalikkoikkuna jo avattuna, ei uutta avaamista pidä sallia.
				return;
			}
*/
			base.WndProc(ref Msg);
		}
	}


}
