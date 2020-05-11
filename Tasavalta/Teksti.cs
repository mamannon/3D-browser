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



    public partial class Teksti : Form
    {

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetSystemMetrics(int numero);

        [DllImport("user32.dll")]
        static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)]string lpFileName);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ReleaseDC(IntPtr hWnd, IntPtr hdc);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void ANNAKORKEUSLEVEYS(ref int xIncrement, ref int yIncrement,
            ref int xPosition, ref int yPostition,
            ref int xRange, ref int yRange,
            ref int leveys, ref int korkeus);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate bool AVAAHTML([MarshalAs(UnmanagedType.LPWStr)]string tiedosto,
            IntPtr hwndMain, IntPtr hdc, IntPtr hwnd, ref int xRange, ref int yRange,
            ref int xPosition, ref int yPosition, ref uint vari,
            [MarshalAs(UnmanagedType.LPWStr)]string otsikko,
            IntPtr[] otsikkoLista, ref int listanPituus);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void ASETAFONTIT(ref char[] normal_face);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate bool ONMOUSEEVENT(ref int X, ref int Y, ref int vent, bool vasen, bool oikea,
            ref int xRange, ref int yRange, ref int xPosition, ref int yPosition, ref uint vari);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void PIIRRA();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate bool HISTORYBACK(ref int yPosition, ref int yRange, ref int xPosition, ref int xRange,
            ref uint vari, [MarshalAs(UnmanagedType.LPWStr)]string otsikko, IntPtr[] otsikkoLista, ref int listanPituus);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate bool HISTORYFORWARD(ref int yPosition, ref int yRange, ref int xPosition, ref int xRange,
            ref uint vari, [MarshalAs(UnmanagedType.LPWStr)]string otsikko, IntPtr[] otsikkoLista, ref int listanPituus);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate bool HISTORYCANBACK();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate bool HISTORYCANFORWARD();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void TULOSTETAAN();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void SIIRRYANKKURIIN([MarshalAs(UnmanagedType.LPWStr)]string tiedosto,
            ref int xRange, ref int yRange, ref int xPosition, ref int yPosition);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void LATAAUUDESTAAN(ref int xRange, ref int yRange,
            ref int xPosition, ref int yPosition, ref bool vasenOikea);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void PAIVITAOTSIKKOLISTA(IntPtr[] otsikkoLista, ref int listanPituus);

        Teksti.ANNAKORKEUSLEVEYS annaKorkeusLeveys;//
        Teksti.ASETAFONTIT asetaFontit;//
        Teksti.AVAAHTML avaaHtml;//
        Teksti.HISTORYBACK historyBack;//
        Teksti.HISTORYCANBACK historyCanBack;//
        Teksti.HISTORYFORWARD historyForward;//
        Teksti.HISTORYCANFORWARD historyCanForward;//
        Teksti.LATAAUUDESTAAN lataaUudestaan;//
        Teksti.ONMOUSEEVENT onMouseEvent;//
        Teksti.PAIVITAOTSIKKOLISTA paivitaOtsikkoLista;//
        Teksti.PIIRRA piirra;//
        Teksti.SIIRRYANKKURIIN siirryAnkkuriin;//
        Teksti.TULOSTETAAN tulostetaan;
        private readonly int SM_CXSCREEN = 0;
        Singleton mSing;
        IntPtr mHwnd = IntPtr.Zero;
        IntPtr mHdc = IntPtr.Zero;
        IntPtr mDllKahva = IntPtr.Zero;
        int mMoneskoTeksti = 0;
        bool mOnkoVasenAlhaalla = false;
        bool mSaakoKlikata = true;
        IntPtr[] mOtsikot = new IntPtr[100];
        int mListanPituus;
        bool onkoVasenAlhaalla = false;
        int startY, uusiY, skaalaY;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;

                //määritetään ikkuna sellaiseksi, että se ei puutu lapsi-ikkunoiden piirtämiseen
                cp.Style = unchecked((int)WinM.WS_OVERLAPPED | (int)WinM.WS_CAPTION | (int)WinM.WS_SYSMENU |
                    (int)WinM.WS_THICKFRAME | (int)WinM.WS_MINIMIZEBOX |
                    (int)WinM.WS_MINIMIZEBOX | (int)WinM.WS_CLIPCHILDREN);
                return cp;
            }

        }

        protected override void OnClosing(CancelEventArgs e)
        {

            //tuhotaan manageroimattomat taulukot, joista roskienkerääjä ei huolehdi
            for (int i = 0; i < mOtsikot.Length; i++)
            {
                Marshal.FreeHGlobal(mOtsikot[i]);
            }

            //siltä varalta, että käyttäjä edelleen käyttää CAD näkymää
            if (mSing.mValikko.mTekstiTiedosto != null) 
                mSing.mValikko.mTekstiTiedosto.Remove(0);

            //Kun teksti ikkuna suljetaan, on Teksti.dll vapautettava
            FreeLibrary(mDllKahva);

            //Samoin ikkunakahva (hdc) pitää vapauttaa
            ReleaseDC(mHwnd, mHdc);

            //lopetetaan OpenGLIkkuna pakottamalla ikkunan tuhoaminen ja osoitin null arvoiseksi
            this.Dispose();
            base.OnClosing(e);
            mSing.mValikko.mTekstiIkkuna = null;
        }

        protected override void WndProc(ref Message m)
        {

            int siirto1 = -1, siirto2 = -1, siirto3 = -1, siirto4 = -1;
            bool vasenOikea = true;

            //Tämä funktio käsittelee tekstiIkkunaan osoitetut Windows viestit. Käsitellään
            //ensin kaikki viestit normaalisti:
            base.WndProc(ref m);

            //Jos WM_EXITSIZEMOVE tai WM_SIZE on käsiteltävä viesti, tehdään jotain extraa:
            switch (m.Msg)
            {
                case (int)WinM.WM_EXITSIZEMOVE:
                    {

                        //jo ladattu sivu pitää ladata uudestaan, jotta tekstin ja kuvien kokoa voidaan muuttaa
                        lataaUudestaan(ref siirto1, ref siirto2, ref siirto3, ref siirto4, ref vasenOikea);
                        this.paneeli.Width = siirto1;
                        this.paneeli.Height = siirto2;
                        this.paneeli.HorizontalScroll.Value = siirto3;
                        this.paneeli.VerticalScroll.Value = siirto4;

                        //tämä suorittaa tarvittavat päivitykset, vaikka ikkunan kokoa ei olekaan muutettu
                        MuutaKokoa(this, EventArgs.Empty);
                        break;

                    }
                case (int)WinM.WM_SYSCOMMAND:
                    {
                        int wparam = (int)m.WParam;
                        if ((wparam & 0xFFF0) == 61488)  //SC_MAXIMIZE = 61488
                        {

                            //jo ladattu sivu pitää ladata uudestaan, jotta tekstin ja kuvien kokoa voidaan muuttaa
                            lataaUudestaan(ref siirto1, ref siirto2, ref siirto3, ref siirto4, ref vasenOikea);
                            this.paneeli.Width = siirto1;
                            this.paneeli.Height = siirto2;
                            this.paneeli.HorizontalScroll.Value = siirto3;
                            this.paneeli.VerticalScroll.Value = siirto4;

                            //tämä suorittaa tarvittavat päivitykset, vaikka ikkunan kokoa ei olekaan muutettu
                            MuutaKokoa(this, EventArgs.Empty);
                        }

                        if ((wparam & 0xFFF0) == 61728)  //  SC_RESTORE = 61728
                        {

                            //jo ladattu sivu pitää ladata uudestaan, jotta tekstin ja kuvien kokoa voidaan muuttaa
                            lataaUudestaan(ref siirto1, ref siirto2, ref siirto3, ref siirto4, ref vasenOikea);
                            this.paneeli.Width = siirto1;
                            this.paneeli.Height = siirto2;
                            this.paneeli.HorizontalScroll.Value = siirto3;
                            this.paneeli.VerticalScroll.Value = siirto4;

                            //tämä suorittaa tarvittavat päivitykset, vaikka ikkunan kokoa ei olekaan muutettu
                            MuutaKokoa(this, EventArgs.Empty);
                        }
                        break;
                    }
            }

            //jos ikkunan kokoa on muutettu, pitää eteen ja taakse disabloida
            if (!vasenOikea)
            {
                this.taakse.Enabled = false;
                this.eteen.Enabled = false;
            }  
        }

        public Teksti()
        {
            InitializeComponent();

            //nämä pitää alustaa tässä
            mSing = Singleton.AnnaIlmentyma;
            for (int i = 0; i < mOtsikot.Length; i++)
            {
                mOtsikot[i] = Marshal.AllocHGlobal(200 * sizeof(byte));
            }
            mHwnd = this.paneeli.Handle;
            mHdc = GetWindowDC(mHwnd);
            this.MouseWheel += HiiriPyoraPyorii;
            this.fokusaattori.Focus();

            //Säädetään ikkunan sijainti ja koko oikeaksi
            this.StartPosition = FormStartPosition.Manual;
            float leveysSuhde = ((float)GetSystemMetrics(SM_CXSCREEN) - 100.0f) / 1500.0f;
            int AWidth = (int)(1000 * leveysSuhde);
            int AHeight = (int)(700 * leveysSuhde);
            int ALeft = 50 + (int)(500 * leveysSuhde);
            int ATop = 100;
            this.SetBounds(ALeft, ATop, AWidth, AHeight);

        }

        //Tämä metodi avaa HTML-tiedoston ja tarvittaessa ottaa käyttöön Teksti.dll 
        //kirjaston. 
        public void AvaaTeksti()
        {
            int siirto1, siirto2, siirto3, siirto4, siirto5, siirto6, siirto7, siirto8;
            IntPtr[] otsikot;
            string otsikko = string.Empty;
            bool kaikkiKunnossa = true;
            uint taustaVari;

            //otetaan käyttöön dynaaminen linkkikirjasto Teksti.dll, jollei se ole jo käytössä
            if (mDllKahva == IntPtr.Zero)
            {
                mDllKahva = LoadLibrary("Teksti.dll");
            }

            //tekstiIkkuna pitää avata
            this.Show();

            //avataan käyttäjän valitsema html-sivu
            if (mDllKahva != IntPtr.Zero)
            {
                IntPtr siirto = GetProcAddress(mDllKahva, "_lataaTiedosto");
                if (siirto == IntPtr.Zero)
                {
                    siirto = GetProcAddress(mDllKahva, "lataaTiedosto");
                    if (siirto == IntPtr.Zero)
                    {
                        kaikkiKunnossa = false;
                        goto virhe;
                    }
                }
                avaaHtml = (AVAAHTML)
                    Marshal.GetDelegateForFunctionPointer(siirto, typeof(AVAAHTML));
                siirto = GetProcAddress(mDllKahva, "_annaKorkeusLeveys");
                if (siirto == IntPtr.Zero)
                {
                    siirto = GetProcAddress(mDllKahva, "annaKorkeusLeveys");
                    if (siirto == IntPtr.Zero)
                    {
                        kaikkiKunnossa = false;
                        goto virhe;
                    }
                }
                annaKorkeusLeveys = (ANNAKORKEUSLEVEYS)
                    Marshal.GetDelegateForFunctionPointer(siirto, typeof(ANNAKORKEUSLEVEYS));
                siirto = GetProcAddress(mDllKahva, "_asetaFontit");
                if (siirto == IntPtr.Zero)
                {
                    siirto = GetProcAddress(mDllKahva, "asetaFontit");
                    if (siirto == IntPtr.Zero)
                    {
                        kaikkiKunnossa = false;
                        goto virhe;
                    }
                }
                asetaFontit = (ASETAFONTIT)
                    Marshal.GetDelegateForFunctionPointer(siirto, typeof(ASETAFONTIT));
                siirto = GetProcAddress(mDllKahva, "_historyBack");
                if (siirto == IntPtr.Zero)
                {
                    siirto = GetProcAddress(mDllKahva, "historyBack");
                    if (siirto == IntPtr.Zero)
                    {
                        kaikkiKunnossa = false;
                        goto virhe;
                    }
                }
                historyBack = (HISTORYBACK)
                    Marshal.GetDelegateForFunctionPointer(siirto, typeof(HISTORYBACK));
                siirto = GetProcAddress(mDllKahva, "_historyCanBack");
                if (siirto == IntPtr.Zero)
                {
                    siirto = GetProcAddress(mDllKahva, "historyCanBack");
                    if (siirto == IntPtr.Zero)
                    {
                        kaikkiKunnossa = false;
                        goto virhe;
                    }
                }
                historyCanBack = (HISTORYCANBACK)
                    Marshal.GetDelegateForFunctionPointer(siirto, typeof(HISTORYCANBACK));
                siirto = GetProcAddress(mDllKahva, "_historyForward");
                if (siirto == IntPtr.Zero)
                {
                    siirto = GetProcAddress(mDllKahva, "historyForward");
                    if (siirto == IntPtr.Zero)
                    {
                        kaikkiKunnossa = false;
                        goto virhe;
                    }
                }
                historyForward = (HISTORYFORWARD)
                    Marshal.GetDelegateForFunctionPointer(siirto, typeof(HISTORYFORWARD));
                siirto = GetProcAddress(mDllKahva, "_historyCanForward");
                if (siirto == IntPtr.Zero)
                {
                    siirto = GetProcAddress(mDllKahva, "historyCanForward");
                    if (siirto == IntPtr.Zero)
                    {
                        kaikkiKunnossa = false;
                        goto virhe;
                    }
                }
                historyCanForward = (HISTORYCANFORWARD)
                    Marshal.GetDelegateForFunctionPointer(siirto, typeof(HISTORYCANFORWARD));
                siirto = GetProcAddress(mDllKahva, "_lataaUudestaan");
                if (siirto == IntPtr.Zero)
                {
                    siirto = GetProcAddress(mDllKahva, "lataaUudestaan");
                    if (siirto == IntPtr.Zero)
                    {
                        kaikkiKunnossa = false;
                        goto virhe;
                    }
                }
                lataaUudestaan = (LATAAUUDESTAAN)
                    Marshal.GetDelegateForFunctionPointer(siirto, typeof(LATAAUUDESTAAN));
                siirto = GetProcAddress(mDllKahva, "_onMouseEvent");
                if (siirto == IntPtr.Zero)
                {
                    siirto = GetProcAddress(mDllKahva, "onMouseEvent");
                    if (siirto == IntPtr.Zero)
                    {
                        kaikkiKunnossa = false;
                        goto virhe;
                    }
                }
                onMouseEvent = (ONMOUSEEVENT)
                    Marshal.GetDelegateForFunctionPointer(siirto, typeof(ONMOUSEEVENT));
                siirto = GetProcAddress(mDllKahva, "_paivitaOtsikkoLista");
                if (siirto == IntPtr.Zero)
                {
                    siirto = GetProcAddress(mDllKahva, "paivitaOtsikkoLista");
                    if (siirto == IntPtr.Zero)
                    {
                        kaikkiKunnossa = false;
                        goto virhe;
                    }
                }
                paivitaOtsikkoLista = (PAIVITAOTSIKKOLISTA)
                    Marshal.GetDelegateForFunctionPointer(siirto, typeof(PAIVITAOTSIKKOLISTA));
                siirto = GetProcAddress(mDllKahva, "_piirra");
                if (siirto == IntPtr.Zero)
                {
                    siirto = GetProcAddress(mDllKahva, "piirra");
                    if (siirto == IntPtr.Zero)
                    {
                        kaikkiKunnossa = false;
                        goto virhe;
                    }
                }
                piirra = (PIIRRA)
                    Marshal.GetDelegateForFunctionPointer(siirto, typeof(PIIRRA));
                siirto = GetProcAddress(mDllKahva, "_siirryAnkkuriin");
                if (siirto == IntPtr.Zero)
                {
                    siirto = GetProcAddress(mDllKahva, "siirryAnkkuriin");
                    if (siirto == IntPtr.Zero)
                    {
                        kaikkiKunnossa = false;
                        goto virhe;
                    }
                }
                siirryAnkkuriin = (SIIRRYANKKURIIN)
                    Marshal.GetDelegateForFunctionPointer(siirto, typeof(SIIRRYANKKURIIN));
                siirto = GetProcAddress(mDllKahva, "_tulostetaan");
                if (siirto == IntPtr.Zero)
                {
                    siirto = GetProcAddress(mDllKahva, "tulostetaan");
                    if (siirto == IntPtr.Zero)
                    {
                        kaikkiKunnossa = false;
                        goto virhe;
                    }
                }
                tulostetaan = (TULOSTETAAN)
                    Marshal.GetDelegateForFunctionPointer(siirto, typeof(TULOSTETAAN));

            virhe:

                if (kaikkiKunnossa)
                {

                    //koska VCL-muuttujien käyttö suoraan on onglemallista, täytyy käyttää siirtoa
                    siirto1 = 1;
                    siirto2 = 1;
                    siirto3 = this.paneeli.HorizontalScroll.Value;
                    siirto4 = this.paneeli.VerticalScroll.Value;
                    siirto5 = this.paneeli.Width;
                    siirto6 = this.paneeli.Height;
                    siirto7 = this.Width;
                    siirto8 = this.Height;

                    //Ensiksi Teksti.dll:lle pitää kertoa ikkunan mitat
                    annaKorkeusLeveys(ref siirto1, ref siirto2, ref siirto3, ref siirto4,
                        ref siirto5, ref siirto6, ref siirto7, ref siirto8);

                    //tallennetaan takaisin mahdollisesti muutetut arvot
                    this.paneeli.HorizontalScroll.Value = siirto3;
                    this.paneeli.VerticalScroll.Value = siirto4;
                    this.paneeli.Width = siirto5;
                    this.paneeli.Height = siirto6;

                    //Sitten ladataan haluttu HTML sivu ja samalla Teksti.dll kertoo ladattavan sivun mitat
                    taustaVari = 0xFF000000;
                    if (avaaHtml(mSing.mValikko.mTiedosto, mSing.mValikko.Handle, mHdc, mHwnd,
                        ref siirto5, ref siirto6, ref siirto3, ref siirto4,
                                 ref taustaVari, otsikko, mOtsikot, ref mListanPituus))
                    {

                        //avattu tiedosto pitää tallentaa ja merkitä, että se on suoraan saatavilla päävalikosta
                        mSing.mValikko.mTekstiTiedosto = mSing.mValikko.mTiedosto;
                        mSing.mValikko.TuoEsille(mSing.mValikko.mTiedosto, 'T');

                        this.paneeli.Width = siirto5;
                        this.paneeli.Height = siirto6;
                        this.paneeli.HorizontalScroll.Value = siirto3;
                        this.paneeli.VerticalScroll.Value = siirto4;
                        /*
                                                //Suurennetaan tekstiKentta1:n ala ladattavan sivun kokoiseksi, jos se on tarpeen
                                                if (tekstiIkkuna->ScrollBox1->Width < tekstiIkkuna->ScrollBox1->HorzScrollBar->Range)
                                                {
                                                    tekstiKentta1->Width = tekstiIkkuna->ScrollBox1->HorzScrollBar->Range + 20;
                                                }
                                                if (tekstiIkkuna->ScrollBox1->Height < tekstiIkkuna->ScrollBox1->VertScrollBar->Range)
                                                {
                                                    tekstiKentta1->Height = tekstiIkkuna->ScrollBox1->VertScrollBar->Range;
                                                }
                        */
                        //Asetetaan haluttu taustaväri ja sivun otsikko tässä välillä, kun taustavärin ja
                        //otsikon tietokin on saatu Teksti.dll:ltä
                        if (taustaVari != 0xFF000000)
                        {
                            this.paneeli.BackColor = AnnaVari(taustaVari);
                        }
                        this.Invalidate();
                        if (otsikko.Length != 0)
                        {
                            string ot = "Text view - ";
                            this.Text = ot + otsikko;
                        }
                        else
                        {
                            this.Text = "Text view";
                        }

                        //otetaan otsikkolista talteen
                        TuoOtsikot();

                        //Pyydetään Teksti.dll:ää päivittämään näkymä
                        piirra();

                        //Lopuksi pitää vielä varmistaa, voiko klikata eteen tai taakse nappia
                        this.taakse.Enabled = historyCanBack();
                        this.eteen.Enabled = historyCanForward();
                    }
                    else
                    {
                        this.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Unable to use funktion pointers.");
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("Unable to load Teksti.dll or some other dependency library.");
                this.Close();
            }
        }

        //Tällä funktiolla saadaan uint luvusta Color ilmentymä
        private Color AnnaVari(uint argb)
        {
            return Color.FromArgb((byte)((argb & 0xff000000) >> 0x18),
                (byte)((argb & 0x00ff0000) >> 0x10),
                (byte)((argb & 0x0000ff00) >> 0x08),
                (byte)(argb & 0x000000ff));
        }

        //Apumetodi otsikoiden lataamiseksi otsikkolistaan
        private void TuoOtsikot()
        {

            //alustetaan paikalliset muuttujat
            char[] siirto1 = new char[200];

            //tämän funktion tehtävänä on ladata uudet valinnat otsikkoListaan. Tyhjennetään ensin lista
            otsikkoLista.Items.Clear();
            otsikkoLista.Text = "Paragraphs";

            //sitten tallennetaan kaikki tekstit otsikkoListaan
            for (int i = 0; i < mListanPituus; i++)
            {

                Marshal.Copy(mOtsikot[i], siirto1, 0, 200);
                string siirto2 = new string(siirto1, 0, siirto1.Length);
                int loppu = siirto2.IndexOf('\0');
                siirto2 = siirto2.Remove(loppu);
                otsikkoLista.Items.Add(siirto2);
            }
        }

        //Käyttäjä on klikannut eteen nappulaa
        private void EteenClick(object sender, EventArgs e)
        {
            int siirto1 = 0, siirto2 = 0, siirto3 = 0, siirto4 = 0;
            string otsikko = string.Empty;
            uint taustaVari = 0xFF000000;

            //Taakse nappia on klikattu. Pyydetään Teksti.dll:ää avaamaan aiempi HTML näkymä
            historyForward(ref siirto1, ref siirto2, ref siirto3, ref siirto4,
                ref taustaVari, otsikko, mOtsikot, ref mListanPituus);

            //Siirretään uuden HTML sivun mittatiedot VCL:lle
            this.paneeli.Width = siirto4;
            this.paneeli.Height = siirto2;
            this.paneeli.HorizontalScroll.Value = siirto3;
            this.paneeli.VerticalScroll.Value = siirto1;
            /*
                        //Suurennetaan tekstiKentta1:n ala ladattavan sivun kokoiseksi, jos se on tarpeen
                        if (tekstiIkkuna->ScrollBox1->Width < tekstiIkkuna->ScrollBox1->HorzScrollBar->Range)
                        {
                            tekstiKentta1->Width = tekstiIkkuna->ScrollBox1->HorzScrollBar->Range + 20;
                        }
                        if (tekstiIkkuna->ScrollBox1->Height < tekstiIkkuna->ScrollBox1->VertScrollBar->Range)
                        {
                            tekstiKentta1->Height = tekstiIkkuna->ScrollBox1->VertScrollBar->Range;
                        }
            */
            //Asetetaan haluttu taustaväri ja sivun otsikko tässä välillä, kun taustavärin ja
            //otsikon tietokin on saatu Teksti.dll:ltä
            if (taustaVari != 0xFF000000)
            {
                this.paneeli.BackColor = AnnaVari(taustaVari);
            }
            this.Invalidate();
            if (otsikko.Length != 0)
            {
                string ot = "Text view - ";
                this.Text = ot + otsikko;
            }
            else
            {
                this.Text = "Text view";
            }

            //otetaan otsikkolista talteen
            TuoOtsikot();

            //Pyydetään Teksti.dll:ää päivittämään näkymä
            piirra();

            //Lopuksi pitää vielä varmistaa, voiko klikata eteen tai taakse nappia
            this.taakse.Enabled = historyCanBack();
            this.eteen.Enabled = historyCanForward();
        }

        //käyttäjä on klikannut tulosta nappulaa
        private void TulostaClick(object sender, EventArgs e)
        {
            tulostetaan();
        }

        //käyttäjä on klikannut taakse-nappulaa
        private void TaakseClick(object sender, EventArgs e)
        {
            int siirto1 = 0, siirto2 = 0, siirto3 = 0, siirto4 = 0;
            string otsikko = string.Empty;
            uint taustaVari = 0xFF000000;

            //Taakse nappia on klikattu. Pyydetään Teksti.dll:ää avaamaan aiempi HTML näkymä
            historyBack(ref siirto1, ref siirto2, ref siirto3, ref siirto4,
                ref taustaVari, otsikko, mOtsikot, ref mListanPituus);

            //Siirretään uuden HTML sivun mittatiedot TekstiIkkunalle
            this.paneeli.Width = siirto4;
            this.paneeli.Height = siirto2;
            this.paneeli.HorizontalScroll.Value = siirto3;
            this.paneeli.VerticalScroll.Value = siirto1;
            /*
                        //Suurennetaan tekstiKentta1:n ala ladattavan sivun kokoiseksi, jos se on tarpeen
                        if (tekstiIkkuna->ScrollBox1->Width < tekstiIkkuna->ScrollBox1->HorzScrollBar->Range)
                        {
                            tekstiKentta1->Width = tekstiIkkuna->ScrollBox1->HorzScrollBar->Range + 20;
                        }
                        if (tekstiIkkuna->ScrollBox1->Height < tekstiIkkuna->ScrollBox1->VertScrollBar->Range)
                        {
                            tekstiKentta1->Height = tekstiIkkuna->ScrollBox1->VertScrollBar->Range;
                        }
            */
            //Asetetaan haluttu taustaväri ja sivun otsikko tässä välillä, kun taustavärin ja
            //otsikon tietokin on saatu Teksti.dll:ltä
            if (taustaVari != 0xFF000000)
            {
                this.paneeli.BackColor = AnnaVari(taustaVari);
            }
            this.Invalidate();
            if (otsikko.Length != 0)
            {
                string ot = "Text view - ";
                this.Text = ot + otsikko;
            }
            else
            {
                this.Text = "Text view";
            }

            //otetaan otsikkolista talteen
            TuoOtsikot();

            //Pyydetään Teksti.dll:ää päivittämään näkymä
            piirra();

            //Lopuksi pitää vielä varmistaa, voiko klikata eteen tai taakse nappia
            this.taakse.Enabled = historyCanBack();
            this.eteen.Enabled = historyCanForward();
        }

        //käyttäjä muuttaa TekstiIkkunan kokoa
        public void MuutaKokoa(object sender, EventArgs e)
        {

            //koska Ikkunaa luotaessa windows kutsuu tätä funktiota ennen kuin
            //tätä funktiota on luotu tarvitaan ehdollistus
            if (mSing.mValikko.mTekstiIkkuna != null)
            {

                //alustetaan paikalliset muuttujat
                int siirto1, siirto2, siirto3, siirto4, siirto5, siirto6, siirto7, siirto8;
                /*
                            //kun tekstiIkkunan kokoa muutetaan, pitää muuttaa myös komponenttien koot
                            ControlBar1->Width = tekstiIkkuna->ClientWidth;
                            ScrollBox1->Height = tekstiIkkuna->ClientHeight - ControlBar1->Height;
                            ScrollBox1->Width = tekstiIkkuna->ClientWidth;

                            //Ikkunan client-alueen sisältö on pyyhittävä pois uuden tieltä
                            if (tekstiKentta1)
                            {
                                tekstiKentta1->Repaint();
                            }
                */
                //koska VCL-muuttujien käyttö suoraan on onglemallista, täytyy käyttää siirtoa
                siirto1 = 1;
                siirto2 = 1;
                siirto3 = this.paneeli.HorizontalScroll.Value;
                siirto4 = this.paneeli.VerticalScroll.Value;
                siirto5 = this.paneeli.Width;
                siirto6 = this.paneeli.Height;
                siirto7 = this.Width;
                siirto8 = this.Height;

                //Ensiksi Teksti.dll:lle pitää kertoa ikkunan mitat
                annaKorkeusLeveys(ref siirto1, ref siirto2, ref siirto3, ref siirto4,
                    ref siirto5, ref siirto6, ref siirto7, ref siirto8);

                //tallennetaan takaisin mahdollisesti muutetut arvot
                this.paneeli.HorizontalScroll.Value = siirto3;
                this.paneeli.VerticalScroll.Value = siirto4;
                this.paneeli.Width = siirto5;
                this.paneeli.Height = siirto6;
                /*
                            //Sovitetaan tekstiKentta1:n ala ladattavan sivun kokoiseksi, jos se on tarpeen
                            if (tekstiIkkuna->ScrollBox1->Width > tekstiKentta1->Width)
                            {
                                tekstiKentta1->Width = tekstiIkkuna->ScrollBox1->HorzScrollBar->Range + 20;
                            }
                            if (tekstiIkkuna->ScrollBox1->Width < tekstiIkkuna->ScrollBox1->HorzScrollBar->Range)
                            {
                                tekstiKentta1->Width = tekstiIkkuna->ScrollBox1->HorzScrollBar->Range + 20;
                            }
                            if (tekstiIkkuna->ScrollBox1->Height < tekstiIkkuna->ScrollBox1->VertScrollBar->Range)
                            {
                                tekstiKentta1->Height = tekstiIkkuna->ScrollBox1->VertScrollBar->Range;
                            }
                */
                //Pyydetään Teksti.dll:ää päivittämään näkymä
                piirra();
            }
        }

        //tämä metodi toimii vain, jos paneelilla tai sillä kontrollilla, jolle tämä metodi
        //on osoitettu, on focus. Hiiren keskipyorän toiminta edellyttää focusta, toisin kuin 
        //hiiren nappien käyttö eli klikkaukset
        private void HiiriPyoraPyorii(object sender, MouseEventArgs e)
        {
            int siirto1, siirto2, siirto3, siirto4, siirto5, siirto6, siirto7, siirto8;

            //ensin selvitetään, kumpaan suuntaan käyttäjä rullaa keskipyörää
            if (e.Delta >= 0)
            {
                this.paneeli.VerticalScroll.Value += 20;
            }
            else
            {
                this.paneeli.VerticalScroll.Value -= 20;
            }

            //koska muuttujien käyttö suoraan on onglemallista, täytyy käyttää siirtoa
            siirto1 = 1;
            siirto2 = 1;
            siirto3 = this.paneeli.HorizontalScroll.Value;
            siirto4 = this.paneeli.VerticalScroll.Value;
            siirto5 = this.paneeli.Width;
            siirto6 = this.paneeli.Height;
            siirto7 = this.Width;
            siirto8 = this.Height;

            //Ensiksi Teksti.dll:lle pitää kertoa ikkunan mitat
            annaKorkeusLeveys(ref siirto1, ref siirto2, ref siirto3, ref siirto4,
                ref siirto5, ref siirto6, ref siirto7, ref siirto8);

            //tallennetaan takaisin mahdollisesti muutetut arvot
            this.paneeli.HorizontalScroll.Value = siirto3;
            this.paneeli.VerticalScroll.Value = siirto4;
            this.paneeli.Width = siirto5;
            this.paneeli.Height = siirto6;

            //Ikkunan client-alueen sisältö on pyyhittävä pois uuden tieltä
            this.Invalidate();

            //Pyydetään Teksti.dll:ää päivittämään näkymä
            piirra();
        }

        //käyttäjän klikatessa paikallaan olevan hiiren nappulaa paneelin alueella tämä
        //metodi aktivoituu
        private void PaneeliClick(object sender, MouseEventArgs e)
        {
            int siirto1 = -1, siirto2 = -1, siirto3 = -1, siirto4 = -1;
            uint taustaVari = 0xFF000000;
            int x = e.X;
            int y = e.Y;

            //kytketään pan pois päältä, sillä hiiriklikkaus on onMouseUp-tapahtuma
            onkoVasenAlhaalla = false;

            //Koska hiirtä klikataan teksti-ikkunan client-alueella, lähetetään tieto kohteeseen Teksti.dll

            //Jos oikeaa hiiren nappia klikataan
            if (e.Button == MouseButtons.Right)
            {
                int siirto = 104;  //EVT_RIGHT_DOWN = 104
                onMouseEvent(ref x, ref y, ref siirto, false, true, 
                    ref siirto1, ref siirto2, ref siirto3, ref siirto4, ref taustaVari);
            }

            //Jos vasenta hiiren nappia klikataan
            if (e.Button == MouseButtons.Left)  //saakoKlikata
            {

                int siirto = 100;    //EVT_LEFT_DOWN = 100
                siirto1 = this.paneeli.Width;
                siirto2 = this.paneeli.Height;
                siirto3 = this.paneeli.HorizontalScroll.Value;
                siirto4 = this.paneeli.VerticalScroll.Value;
                onMouseEvent(ref x, ref y, ref siirto, true, false, 
                    ref siirto1, ref siirto2, ref siirto3, ref siirto4, ref taustaVari);
                this.paneeli.Width = siirto1;
                this.paneeli.Height = siirto2;
                this.paneeli.HorizontalScroll.Value = siirto3;
                this.paneeli.VerticalScroll.Value = siirto4;
/*
                //jos klikattiin linkkiä, jäädytetään uusien linkkien avaaminen hetkeksi
                if (siirto == 1)
                {
                    saakoKlikata = false;
                    timerKahva =::SetTimer(paaIkkuna->hwndMain, TIMER2, 800, NULL);
                }
*/
            }
        }

        //kun käyttäjä on klikannut jotain otsikkolistan otsikkoa, suoritetaan tämä funktio
        private void ValittuOtsikko(object sender, EventArgs e)
        {
            int siirto1 = -1, siirto2 = -1, siirto3 = -1, siirto4 = -1;

            var otsikko = otsikkoLista.ComboBox.GetItemText(otsikkoLista.ComboBox.SelectedItem);
            siirryAnkkuriin(otsikko, ref siirto1, ref siirto2, ref siirto3, ref siirto4);
            this.paneeli.Width = siirto1;
            this.paneeli.Height = siirto2;
            this.paneeli.HorizontalScroll.Value = siirto3;
            this.paneeli.VerticalScroll.Value = siirto4;
/*
            //focus jää kiinni otsikkoListaan ja se pitää poistaa siitä
            otsikkoLista->Enabled = false;
            otsikkoLista->Enabled = true;
*/
            //tämä suorittaa tarvittavat päivitykset, vaikka ikkunan kokoa ei olekaan muutettu
            MuutaKokoa(this, EventArgs.Empty);
        }


        //käyttäjä liikuttaa hiirtä paneelin alueella ja mahdollisesti klikkaa
        private void HiiriLiikkuu(object sender, MouseEventArgs e)
        {
            int siirto1 = -1, siirto2 = -1, siirto3 = -1, siirto4 = -1;
            int x = e.X;
            int y = e.Y;
            uint taustaVari = 0xFF000000;
            bool vastaus = false;

            //Koska hiiri liikkuu teksti-ikkunan client-alueella, lähetetään tieto kohteeseen Teksti.dll

            //Jos hiiri liikkuu
            if (e.Button != MouseButtons.Left && e.Button != MouseButtons.Right)
            {
                int siirto = 106;   //  EVT_MOTION = 106
                vastaus = onMouseEvent(ref x, ref y, ref siirto, false, false, 
                    ref siirto1, ref siirto2, ref siirto3, ref siirto4, ref taustaVari);
            }

            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
            {

                //Jos hiiri liikkuu vasen painettuna
                if (e.Button == MouseButtons.Left && e.Button != MouseButtons.Right)   //saakoKlikata
                {

                    //kytketään panning toimintaan, jollei se ole jo päällä
                    if (!onkoVasenAlhaalla)
                    {
                        onkoVasenAlhaalla = true;
                        startY = y;
                        skaalaY = this.paneeli.Height - this.Height;
                    }
                    else
                    {
                        uusiY = this.paneeli.VerticalScroll.Value - y + startY;
                    }

                    int siirto = 100;    //  EVT_LEFT_DOWN = 100
                    siirto1 = this.paneeli.Width;
                    siirto2 = this.paneeli.Height;
                    siirto3 = this.paneeli.HorizontalScroll.Value;

                    if (uusiY > 0 && uusiY < skaalaY)
                    {
                        this.paneeli.VerticalScroll.Value = uusiY;
                        siirto4 = uusiY;
                    }
                    else
                    {
                        siirto4 = this.paneeli.VerticalScroll.Value;
                    }

                    vastaus = onMouseEvent(ref x, ref y, ref siirto, true, false, 
                        ref siirto1, ref siirto2, ref siirto3, ref siirto4, ref taustaVari);
                    this.paneeli.Width = siirto1;
                    this.paneeli.Height = siirto2;
                    this.paneeli.HorizontalScroll.Value = siirto3;
                    this.paneeli.VerticalScroll.Value = siirto4;

                    //tämä suorittaa tarvittavat päivitykset, vaikka ikkunan kokoa ei olekaan muutettu
                    MuutaKokoa(sender, e);
                    /*
                    //jos klikattiin linkkiä, jäädytetään uusien linkkien avaaminen hetkeksi
                    if (siirto == 1)
                    {
                        saakoKlikata = false;
                        timerKahva =::SetTimer(paaIkkuna->hwndMain, TIMER2, 800, NULL);
                    }
                    */
                }

                //Jos hiiri liikkuu oikea painettuna
                if (e.Button != MouseButtons.Left && e.Button == MouseButtons.Right)
                {
                    int siirto = 104;      //   EVT_RIGHT_DOWN 104
                    vastaus = onMouseEvent(ref x, ref y, ref siirto, false, true, 
                        ref siirto1, ref siirto2, ref siirto3, ref siirto4, ref taustaVari);
                }

            }

            //jos vastaus on true, kursori on etusormikämmen, false on nuoli
            if (vastaus)
            {
                Cursor.Current = Cursors.Hand;
            }
            else
            {
                Cursor.Current = Cursors.Arrow;
            }

            //Lopuksi pitää vielä varmistaa, voiko klikata eteen tai taakse nappia
            this.taakse.Enabled = historyCanBack();
            this.eteen.Enabled = historyCanForward();
        }

        //hiiri poistuu paneelin alueelta
        private void HiiriPoistuu(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.Arrow;
        }

        //otsikkoListan otsikot pitää päivittää, kun käyttäjä selailee HTML-sivuja
        //päivitys tehdään aina, kun otsikkolista avataan, riippumatta siitä, onko
        //päivitykselle tarvetta vai ei
        private void PaivitaOtsikot(object sender, EventArgs e)
        {
            paivitaOtsikkoLista(mOtsikot, ref mListanPituus);
            TuoOtsikot();
        }

        //kirjanmerkki tarvitsee muistiinsa HTML-tiedoston näkymän
        public void AnnaOrientaatio(int[] orientaatio)
        {
            orientaatio[0] = this.paneeli.Width;
            orientaatio[1] = this.paneeli.Height;
            orientaatio[2] = this.paneeli.HorizontalScroll.Value;
            orientaatio[3] = this.paneeli.VerticalScroll.Value;
        }

        //kun kirjanmerkki avataan, tämä funktio tuo muistista edellisen näkymän
        public void AsetaOrientaatio(in int[] orientaatio)
        {
            bool vasenOikea = true;
            int siirto0 = orientaatio[0];
            int siirto1 = orientaatio[1];
            int siirto2 = orientaatio[2];
            int siirto3 = orientaatio[3];
            lataaUudestaan(ref siirto0, ref siirto1, ref siirto2, ref siirto3, ref vasenOikea);
            this.paneeli.Width = siirto0;
            this.paneeli.Height = siirto1;
            this.paneeli.HorizontalScroll.Value = siirto2;
            this.paneeli.VerticalScroll.Value = siirto3;
            if (!vasenOikea)
            {
                    this.taakse.Enabled = false;
                    this.eteen.Enabled = false;
            }
        }

    }

    public class Paneeli : Panel
    {
        Singleton mSing;
        int hiword, loword;

        public Paneeli()
        {
            mSing = Singleton.AnnaIlmentyma;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);

        }

        protected override void WndProc(ref Message m)
        {

            //Tämä funktio käsittelee scrollboxiin osoitetut Windows viestit
            switch (m.Msg)
            {
                case (int)WinM.WM_HSCROLL:
                    /* horizontal scrolling */
                    loword = (int)m.WParam;

                    //otetaan win32 LOWORD
                    loword = loword & 0x0000FFFF;
                    switch (loword)
                    {
                        case 6:   //  SB_TOP = 6
                            this.HorizontalScroll.Value = 0;
                            mSing.mValikko.mTekstiIkkuna.MuutaKokoa(this, EventArgs.Empty);
                            break;

                        case 0:   //  SB_LINEUP = 0
                            if (this.HorizontalScroll.Value > 0)
                            {
                                this.HorizontalScroll.Value =
                                    this.HorizontalScroll.Value - 1;
                                mSing.mValikko.mTekstiIkkuna.MuutaKokoa(this, EventArgs.Empty);
                            }
                            break;

                        case 4:    //  SB_THUMBPOSITION = 4
                            hiword = (int)m.WParam;

                            //otetaan win32 HIWORD
                            hiword = hiword >> 16;
                            this.HorizontalScroll.Value = hiword;
                            mSing.mValikko.mTekstiIkkuna.MuutaKokoa(this, EventArgs.Empty);
                            break;

                        case 5:   //  SB_THUMBTRACK = 5
                            hiword = (int)m.WParam;

                            //otetaan win32 HIWORD
                            hiword = hiword >> 16;
                            this.HorizontalScroll.Value = hiword;
                            mSing.mValikko.mTekstiIkkuna.MuutaKokoa(this, EventArgs.Empty);
                            break;

                        case 1:    //  SB_LINEDOWN = 1
                            if (this.HorizontalScroll.Value <
                                this.Width - mSing.mValikko.mTekstiIkkuna.Width)
                            {
                                this.HorizontalScroll.Value =
                                    this.HorizontalScroll.Value + 1;
                                mSing.mValikko.mTekstiIkkuna.MuutaKokoa(this, EventArgs.Empty);
                            }
                            break;

                        case 7:   // SB_BOTTOM = 7
                            this.HorizontalScroll.Value = this.HorizontalScroll.Maximum;
                            mSing.mValikko.mTekstiIkkuna.MuutaKokoa(this, EventArgs.Empty);
                            break;

                        case 8:   //  SB_ENDSCROLL = 8
                            break;
                    }
                    break;
                case (int)WinM.WM_VSCROLL:
                    /* vertical scrolling */
                    loword = (int)m.WParam;

                    //otetaan win32 LOWORD
                    loword = loword & 0x0000FFFF;
                    switch (loword)
                    {
                        case 6:   //  SB_TOP = 6
                            this.VerticalScroll.Value = 0;
                            mSing.mValikko.mTekstiIkkuna.MuutaKokoa(this, EventArgs.Empty);
                            break;

                        case 0:   //  SB_LINEUP = 0
                            if (this.VerticalScroll.Value > 0)
                            {
                                this.VerticalScroll.Value =
                                    this.VerticalScroll.Value - 1;
                                mSing.mValikko.mTekstiIkkuna.MuutaKokoa(this, EventArgs.Empty);
                            }
                            break;

                        case 4:    //  SB_THUMBPOSITION = 4
                            hiword = (int)m.WParam;

                            //otetaan win32 HIWORD
                            hiword = hiword >> 16;
                            this.VerticalScroll.Value = hiword;
                            mSing.mValikko.mTekstiIkkuna.MuutaKokoa(this, EventArgs.Empty);
                            break;

                        case 5:   //  SB_THUMBTRACK = 5
                            hiword = (int)m.WParam;

                            //otetaan win32 HIWORD
                            hiword = hiword >> 16;
                            this.VerticalScroll.Value = hiword;
                            mSing.mValikko.mTekstiIkkuna.MuutaKokoa(this, EventArgs.Empty);
                            break;

                        case 1:    //  SB_LINEDOWN = 1
                            if (this.VerticalScroll.Value <
                                this.Width - mSing.mValikko.mTekstiIkkuna.Width)
                            {
                                this.VerticalScroll.Value =
                                    this.VerticalScroll.Value + 1;
                                mSing.mValikko.mTekstiIkkuna.MuutaKokoa(this, EventArgs.Empty);
                            }
                            break;

                        case 7:   // SB_BOTTOM = 7
                            this.VerticalScroll.Value = this.VerticalScroll.Maximum;
                            mSing.mValikko.mTekstiIkkuna.MuutaKokoa(this, EventArgs.Empty);
                            break;

                        case 8:   //  SB_ENDSCROLL = 8
                            break;
                    }
                    break;
                    /*
                                    //tämä on välttämätön osa ohjelmaa, se käydään läpi jokaisessa ruudunpäivityksessä
                                    case WM_DESTROY:
                                        {
                                            ScrollBox1->WindowProc = OldScrollBoxWP;
                                            if (!ScrollBox1->ComponentState.Contains(csDestroying))
                                            {
                                                OldScrollBoxWP(AMsg);
                                                OldScrollBoxWP = ScrollBox1->WindowProc;
                                                ScrollBox1->WindowProc = NewScrollBoxWP;
                                                return;
                                            }
                                        }
                                        */
            }
            //           OldScrollBoxWP(AMsg);

            base.WndProc(ref m);
        }
    }

}
