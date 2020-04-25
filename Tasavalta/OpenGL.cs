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
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		static extern int GetSystemMetrics(int numero);

		[DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

		[DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
		static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)]string lpFileName);

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool FreeLibrary(IntPtr hModule);

		//		[DllImport("user32.dll")]
		//		static extern IntPtr GetDCEx(IntPtr hWnd, IntPtr hgrnClip, int flags);

		[DllImport("user32.dll")]
		static extern IntPtr GetWindowDC(IntPtr hWnd);

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

		private readonly int SM_CXSCREEN = 0;
		bool mOpenglOnkoTuettu = false;
		bool mValmis = false;
		bool mVieTiedot = false;
		//		bool[] mValintaListaB1 = new bool[1000];
		//		bool[] mValintaListaB2 = new bool[1000];
		IntPtr mValintaListaB1;
		IntPtr mValintaListaB2;
		IntPtr[] mValintaListaC1 = new IntPtr[1000];
		IntPtr[] mValintaListaC2 = new IntPtr[1000];
		int mValintoja1 = 0;
		int mValintoja2 = 0;
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

		public bool mSaakoTarkastaaLayerit { get; private set; } = true;

		//Vaikka C# on manageroitu, eli siinä on automaattinen roskienkerääjä, tämä luokka
		//sisältää manageroimattomia kenttiä, jotka pitää tuhota manuaalisesti deletorissa
		~OpenGL()
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

			//tuhotaan taulukot, joista roskienkerääjä ei huolehdi
			for (int i = 0; i < mValintaListaC1.Length; i++)
			{
				Marshal.FreeHGlobal(mValintaListaC1[i]);
				Marshal.FreeHGlobal(mValintaListaC2[i]);
			}
			Marshal.FreeHGlobal(mValintaListaB1);
			Marshal.FreeHGlobal(mValintaListaB2);
		}

		//OpenGLIkkunaa suljettaessa on pakotettava ikkunan tuhoaminen sekä osoittimen määrittäminen
		//null arvoiseksi, muuten ikkunan uudelleen avaaminen ei onnistu, kummallista kyllä
		protected override void OnClosing(CancelEventArgs e)
		{

			//lopetetaan OpenGLIkkuna
			this.Dispose();
			base.OnClosing(e);
			mSing.mValikko.mOpenGLIkkuna = null;
		}


		/*
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
		*/
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

			//testausta...
			byte[] lahde = new byte[1000];
			Marshal.Copy(lahde, 0, mValintaListaB2, 1000);

			//Säädetään ikkunan sijainti ja koko oikeaksi
			this.StartPosition = FormStartPosition.Manual;
			float leveysSuhde = ((float)GetSystemMetrics(SM_CXSCREEN) - 100.0f) / 1500.0f;
			int AWidth = (int)(1000 * leveysSuhde);
			int AHeight = (int)(700 * leveysSuhde);
			int ALeft = 50;
			int ATop = 100;
			this.SetBounds(ALeft, ATop, AWidth, AHeight);

			//OpenGL ikkunaa luotaessa osa komponenteista pitää luoda dynaamisesti.
			//CheckBoxComboBox:
			layeriLista = new CheckBoxComboBox();
			layeriLista.Dock = DockStyle.Top;
			layeriLista.Width = (int)(400 * leveysSuhde);
			layeriLista.AllowDrop = false;
			layeriLista.AutoCompleteMode = AutoCompleteMode.None;
//			layeriLista.DropDownStyle = ComboBoxStyle.DropDownList;
			flowLayoutPanel1.Controls.Add(layeriLista);

			//Toinen ToolStrip CheckBoxComboBoxin oikealle puolelle
			ToolStrip ts = new ToolStrip();
			ts.Dock = DockStyle.Top;
			flowLayoutPanel1.Controls.Add(ts);

			//Play nappi:
			kaynnista = new ToolStripButton();
			kaynnista.Name = "kaynnista";
			kaynnista.Text = "Play";
			kaynnista.Dock = DockStyle.Top;
			ts.Items.Add(kaynnista);

			//Stop/Rewind nappi:
			pysayta = new ToolStripButton();
			pysayta.Name = "pysayta";
			pysayta.Text = "Stop/Rewind";
			pysayta.Dock = DockStyle.Top;
			ts.Items.Add(pysayta);

			//luodaan timeri OpenGL ikkunalle
			mTimeri = new System.Windows.Forms.Timer();
			mTimeri.Enabled = true;
			mTimeri.Interval = 50;
			mTimeri.Tick += new System.EventHandler(Timeri_Tick);
		}

		//Tämä funktio kellottaa OpenGL-näkymän päivitystä
		private void Timeri_Tick(object sender, EventArgs e)
		{
//			paneeli.Focus();

			//Käsketään OpenGL:ää päivittämään näkymä
			if (mValmis) paivita();

			//jos käyttäjä on klikannut layerilistassa jotain vaihtoehtoa, 
			//tieto välitetään RunOpenGL.dll kirjastoon 0,2 sekunnin viiveellä
			if (mVieTiedot)
			{
				mVieTiedot = false;
				var timer = new DispatcherTimer();
				timer.Tick += delegate
				{
					VieValinnat();
					timer.Stop();
				};
				timer.Interval = TimeSpan.FromSeconds(2);
				timer.Start();
			}
		}

		//Layerilistan otsikkona tulee aina olla käytössä oleva ulottuvuus sekä
		//ajantasainen tieto auki olevan CAD-tiedoston solideista ja listoista
		private void TuoValinnat()
		{

			//ensiksi estetään layereiden tarkastus
			mSaakoTarkastaaLayerit = false;

			//tässä luetaan manageroimattoman bool-taulukon dataa kopiomalla
			//se byte taulukkoon. Kukin 8 bitin byte vastaa yhtä bool arvoa
			byte[] siirto = new byte[1000];
			Marshal.Copy(mValintaListaB1, siirto, 0, mValintoja1);

			//Pitää erikseen täyttää layeriListan otsikko
			//nolla=false, muut arvot=true 
			if (siirto[0] != 0)
			{
				layeriLista.Otsikko = "3D View";
			}
			else
			{
				if (siirto[1] != 0) layeriLista.Otsikko = "2D X-projection";
				if (siirto[2] != 0) layeriLista.Otsikko = "2D Y-projection";
				if (siirto[3] != 0) layeriLista.Otsikko = "2D Z-projection";
			}

			//sitten pitää kopioida kaikki CAD-tiedoston listanimet
			//sekä niiden valittuisuus manageroimattomasta muistista layeriListaan
			layeriLista.Clear();
			siirto = new byte[1000];
			Marshal.Copy(mValintaListaB2, siirto, 0, mValintoja2);
			for (int i = 0; i < mValintoja2; i++)
			{
				char[] siirto1 = new char[30];
				Marshal.Copy(mValintaListaC2[i], siirto1, 0, 30);
				string siirto2 = new string(siirto1);
				int loppu = siirto2.IndexOf('\0');
				siirto2 = siirto2.Remove(loppu);
				layeriLista.Items.Add(siirto2);
				if (siirto[i] != 0)
				{
					layeriLista.CheckBoxItems[i].Checked = true;
				}
				else
				{
					layeriLista.CheckBoxItems[i].Checked = false;
				}
			}

			//Lopuksi kopioidaan kaikki CAD-tiedoston solidiNimet sekä
			//niiden valittuisuus manageroimattomasta muistista layerilistaan
			siirto = new byte[1000];
			Marshal.Copy(mValintaListaB1, siirto, 0, mValintoja1);
			for (int i = 0; i < mValintoja1; i++)
			{
				char[] siirto1 = new char[30];
				Marshal.Copy(mValintaListaC1[i], siirto1, 0, 30);
				string siirto2 = new string(siirto1, 0, siirto1.Length);
				int loppu = siirto2.IndexOf('\0');
				siirto2 = siirto2.Remove(loppu);
				layeriLista.Items.Add(siirto2);
				if (siirto[i] != 0)
				{
					layeriLista.CheckBoxItems[i].Checked = true;
				}
				else
				{
					layeriLista.CheckBoxItems[i].Checked = false;
				}
			}

			//lopuksi sallitaan layereiden tarkastus
			mSaakoTarkastaaLayerit = true;
		}

		private void VieValinnat()
		{

			//ensiksi estetään layereiden tarkastus
			mSaakoTarkastaaLayerit = false;

			//sitten lähetetään tiedot RunOpenGL.dll:lle
			vieValinnat(mValintaListaB1, mValintaListaC1,
				mValintaListaB2, mValintaListaC2,
				ref mValintoja1, ref mValintoja2);

			//koska lähetetyissä tiedoissa saattaa olla ristiriitoja, jotka korjataan vasta
			//RunOpenGL.dll:ssä, meidän pitää tuoda valinnat takaisin
			TuoValinnat();

			//lopuksi sallitaan layereiden tarkastus
			mSaakoTarkastaaLayerit = true;
		}
		public void TarkastaLayerit()
		{

			//kopioidaan kaikki valinnat manageroimattomaan muistiin.
			//Ensin solidit
			byte[] siirto = new byte[1000];
			for (int i = 0; i < mValintoja1; i++)
			{
				if (layeriLista.CheckBoxItems[i].Checked == true)
				{
					siirto[i] = 1;
				}
				else
				{
					siirto[i] = 0;
				}
				Marshal.Copy(siirto, 0, mValintaListaB1, mValintoja1);
			}

			//sitten listat
			for (int i = 0; i < mValintoja2; i++)
			{
				if (layeriLista.CheckBoxItems[i+mValintoja1-1].Checked == true)
				{
					siirto[i] = 1;
				}
				else
				{
					siirto[i] = 0;
				}
				Marshal.Copy(siirto, 0, mValintaListaB2, mValintoja2);
			}

			//lopuksi mahdollistetaan tietojen vienti RunOpenGL.dll:n
			mVieTiedot = true;
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
			}
			/*
						//avataan Form1 ikkuna
						if (Form1)
						{
							Form1->Show();
						}
						else
						{
							Form1 = new TForm1(this);
							Form1->Show();

							//Otetaan talteen Form1:n HWND ja HDC
							int flags = (int)(DeviceContextValues.Window | DeviceContextValues.Cache | DeviceContextValues.LockWindowUpdate);
							hwnd = Form1->Handle;
							ikkunaKahva =::GetDC(Form1->Handle);
						}
			*/

			mHwnd = paneeli.Handle;
//			IntPtr hrgn = IntPtr.Zero;
//			int flags = (int)(DeviceContextValues.ClipChildren);
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
					/*
					//jos OpenGL laajennuksia on saatavilla, niiden käyttöönotto edellyttää "alusta alkamista"
					if (mOpenglOnkoTuettu == true)
					{

						//hävitetään ikkunaKahva
						ReleaseDC(hwnd, ikkunaKahva);

						//tuhotaan ikkuna Form1
						delete Form1;

						//luodaan ikkuna Form1
						Form1 = new TForm1(this);

						//Otetaan talteen Form1:n HWND ja HDC
						hwnd = Form1->Handle;
						ikkunaKahva =::GetDC(Form1->Handle);

						//avataan TForm1 ikkuna
						Form1->Show();
					}
					else
					*/
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
				case (char)163:
					pyoraAlas(); pyoraAlas();
					break;
				case (char)87:
					pyoraAlas(); pyoraAlas();
					break;
				case (char)119:
					pyoraAlas(); pyoraAlas();
					break;
			}
		}

		//Tämä metodi välittää RunOpenGL.dll:lle tiedon, että käyttäjä rullaa hiiren
		//pyörää alaspäin

	}
}
