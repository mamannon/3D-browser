using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

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

        //testausta...
        [DllImport("kernel32.dll")]
        static extern uint GetLastError();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void ANNAKORKEUSLEVEYS(ref int xIncrement, ref int yIncrement,
            ref int xPosition, ref int yPostition,
            ref int xRange, ref int yRange,
            ref int leveys, ref int korkeus);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        delegate bool AVAAHTML([MarshalAs(UnmanagedType.LPWStr)]string tiedosto,
            ref IntPtr hwndMain, ref IntPtr hdc, ref IntPtr hwnd, ref int xRange, ref int yRange,
            ref int xPosition, ref int yPosition, ref uint vari,
            IntPtr otsikko,
            IntPtr[] otsikkoLista, ref int listanPituus);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void SAMMUTATEKSTI();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        delegate bool ONMOUSEEVENT(ref int X, ref int Y, ref int vent, bool vasen, bool oikea,
            ref int xRange, ref int yRange, ref int xPosition, ref int yPosition, ref uint vari);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void PIIRRA();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        delegate bool HISTORYBACK(ref int yPosition, ref int yRange, ref int xPosition, ref int xRange,
            ref uint vari, IntPtr otsikko, IntPtr[] otsikkoLista, ref int listanPituus);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        delegate bool HISTORYFORWARD(ref int yPosition, ref int yRange, ref int xPosition, ref int xRange,
            ref uint vari, IntPtr otsikko, IntPtr[] otsikkoLista, ref int listanPituus);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        delegate bool HISTORYCANBACK();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
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

        Teksti.ANNAKORKEUSLEVEYS annaKorkeusLeveys;
        Teksti.AVAAHTML avaaHtml;
        Teksti.HISTORYBACK historyBack;
        Teksti.HISTORYCANBACK historyCanBack;
        Teksti.HISTORYFORWARD historyForward;
        Teksti.HISTORYCANFORWARD historyCanForward;
        Teksti.LATAAUUDESTAAN lataaUudestaan;
        Teksti.ONMOUSEEVENT onMouseEvent;
        Teksti.PAIVITAOTSIKKOLISTA paivitaOtsikkoLista;
        Teksti.PIIRRA piirra;
        Teksti.SIIRRYANKKURIIN siirryAnkkuriin;
        Teksti.TULOSTETAAN tulostetaan;
        Teksti.SAMMUTATEKSTI sammutaTeksti;
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
        Color mTaustaVari = Color.White;

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
            if (mDllKahva != IntPtr.Zero)
            {
                sammutaTeksti();
                FreeLibrary(mDllKahva);
                mDllKahva = IntPtr.Zero;
            }

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
                        SuspendLayout();
                        this.kanvaasi.Size = new Size(siirto1, siirto2);
                        this.kanvaasi.PerformLayout();
                        this.paneeli.HorizontalScroll.Value = siirto3;
                        this.paneeli.PerformLayout();
                        this.paneeli.VerticalScroll.Value = siirto4;
                        this.paneeli.PerformLayout();
                        ResumeLayout();

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
                            SuspendLayout();
                            this.kanvaasi.Size = new Size(siirto1, siirto2);
                            this.kanvaasi.PerformLayout();
                            this.paneeli.HorizontalScroll.Value = siirto3;
                            this.paneeli.PerformLayout();
                            this.paneeli.VerticalScroll.Value = siirto4;
                            this.paneeli.PerformLayout();
                            ResumeLayout();

                            //tämä suorittaa tarvittavat päivitykset, vaikka ikkunan kokoa ei olekaan muutettu
                            MuutaKokoa(this, EventArgs.Empty);
                        }

                        if ((wparam & 0xFFF0) == 61728)  //  SC_RESTORE = 61728
                        {

                            //jo ladattu sivu pitää ladata uudestaan, jotta tekstin ja kuvien kokoa voidaan muuttaa
                            lataaUudestaan(ref siirto1, ref siirto2, ref siirto3, ref siirto4, ref vasenOikea);
                            SuspendLayout();
                            this.kanvaasi.Size = new Size(siirto1, siirto2);
                            this.kanvaasi.PerformLayout();
                            this.paneeli.HorizontalScroll.Value = siirto3;
                            this.paneeli.PerformLayout();
                            this.paneeli.VerticalScroll.Value = siirto4;
                            this.paneeli.PerformLayout();
                            ResumeLayout();

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
        /*
                //.NET 4.0 ei sisällä Task.Delay metodia, joten luodaan sellainen itse
                protected static Task Delay(float millisekunteja)
                {
                    var tcs = new TaskCompletionSource<bool>();
                    System.Timers.Timer timer = new System.Timers.Timer();
                    timer.Elapsed += (object obj, ElapsedEventArgs e) =>
                    {
                        tcs.TrySetResult(true);
                    };
                    timer.Interval = millisekunteja;
                    timer.AutoReset = false;
                    timer.Start();
                    return tcs.Task;
                }

                //tämä asynkrooninen metodi tarvitaan tekstinäkymän piirtämiseen vasta sitten,
                //kun ikkuna on valmis
                protected async Task PiirraViipeella()
                {
                    Task delay = Delay(100);
                    delay.ContinueWith((Task x) => piirra());
                }
                */

        //tämä asynkrooninen metodi tarvitaan tekstinäkymän piirtämiseen vasta sitten,
        //kun ikkuna on valmis
        protected async Task PiirraViipeella()
        {
            await Task.Delay(100);
            piirra();
        }

        public Teksti()
        {
            InitializeComponent();

            //nämä pitää alustaa tässä
            mSing = Singleton.AnnaIlmentyma;
            for (int i = 0; i < mOtsikot.Length; i++)
            {
                mOtsikot[i] = Marshal.AllocHGlobal(200 * sizeof(char));
            }
            this.MouseWheel += HiiriPyoraPyorii;
            this.kanvaasi.MouseWheel += HiiriPyoraPyorii;
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
            IntPtr otsikko = Marshal.AllocHGlobal(200 * sizeof(char));
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
                siirto = GetProcAddress(mDllKahva, "_HistoryBack");
                if (siirto == IntPtr.Zero)
                {
                    siirto = GetProcAddress(mDllKahva, "HistoryBack");
                    if (siirto == IntPtr.Zero)
                    {
                        kaikkiKunnossa = false;
                        goto virhe;
                    }
                }
                historyBack = (HISTORYBACK)
                    Marshal.GetDelegateForFunctionPointer(siirto, typeof(HISTORYBACK));
                siirto = GetProcAddress(mDllKahva, "_HistoryCanBack");
                if (siirto == IntPtr.Zero)
                {
                    siirto = GetProcAddress(mDllKahva, "HistoryCanBack");
                    if (siirto == IntPtr.Zero)
                    {
                        kaikkiKunnossa = false;
                        goto virhe;
                    }
                }
                historyCanBack = (HISTORYCANBACK)
                    Marshal.GetDelegateForFunctionPointer(siirto, typeof(HISTORYCANBACK));
                siirto = GetProcAddress(mDllKahva, "_HistoryForward");
                if (siirto == IntPtr.Zero)
                {
                    siirto = GetProcAddress(mDllKahva, "HistoryForward");
                    if (siirto == IntPtr.Zero)
                    {
                        kaikkiKunnossa = false;
                        goto virhe;
                    }
                }
                historyForward = (HISTORYFORWARD)
                    Marshal.GetDelegateForFunctionPointer(siirto, typeof(HISTORYFORWARD));
                siirto = GetProcAddress(mDllKahva, "_HistoryCanForward");
                if (siirto == IntPtr.Zero)
                {
                    siirto = GetProcAddress(mDllKahva, "HistoryCanForward");
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
                siirto = GetProcAddress(mDllKahva, "_OnMouseEvent");
                if (siirto == IntPtr.Zero)
                {
                    siirto = GetProcAddress(mDllKahva, "OnMouseEvent");
                    if (siirto == IntPtr.Zero)
                    {
                        kaikkiKunnossa = false;
                        goto virhe;
                    }
                }
                onMouseEvent = (ONMOUSEEVENT)
                    Marshal.GetDelegateForFunctionPointer(siirto, typeof(ONMOUSEEVENT));
                siirto = GetProcAddress(mDllKahva, "_paivitaOtsikot");
                if (siirto == IntPtr.Zero)
                {
                    siirto = GetProcAddress(mDllKahva, "paivitaOtsikot");
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
                siirto = GetProcAddress(mDllKahva, "_sammutaTeksti");
                if (siirto == IntPtr.Zero)
                {
                    siirto = GetProcAddress(mDllKahva, "sammutaTeksti");
                    if (siirto == IntPtr.Zero)
                    {
                        kaikkiKunnossa = false;
                        goto virhe;
                    }
                }
                sammutaTeksti = (SAMMUTATEKSTI)
                    Marshal.GetDelegateForFunctionPointer(siirto, typeof(SAMMUTATEKSTI));

            virhe:

                if (kaikkiKunnossa)
                {

                    //koska VCL-muuttujien käyttö suoraan on ongelmallista, täytyy käyttää siirtoa
                    siirto1 = 1;
                    siirto2 = 1;
                    siirto3 = this.paneeli.HorizontalScroll.Value;
                    siirto4 = this.paneeli.VerticalScroll.Value;
                    siirto5 = this.Width;
                    siirto6 = this.Height;
                    siirto7 = this.Width;
                    siirto8 = this.Height;

                    //Ensiksi Teksti.dll:lle pitää kertoa ikkunan mitat
                    annaKorkeusLeveys(ref siirto1, ref siirto2, ref siirto3, ref siirto4,
                        ref siirto5, ref siirto6, ref siirto7, ref siirto8);

                    //tallennetaan takaisin mahdollisesti muutetut arvot
                    SuspendLayout();
                    this.kanvaasi.Size = new Size(siirto5, siirto6);
                    this.kanvaasi.PerformLayout();
                    ResumeLayout();

                    //Sitten ladataan haluttu HTML sivu ja samalla Teksti.dll kertoo ladattavan sivun mitat
                    taustaVari = 0xFF000000;
                    mHwnd = this.kanvaasi.Handle;
                    mHdc = GetWindowDC(mHwnd);

                    IntPtr handle = mSing.mValikko.Handle;
                    if (avaaHtml(mSing.mValikko.mTiedosto, ref handle, ref mHdc, ref mHwnd,
                        ref siirto5, ref siirto6, ref siirto3, ref siirto4,
                                 ref taustaVari, otsikko, mOtsikot, ref mListanPituus))
                    {

                        //avattu tiedosto pitää tallentaa ja merkitä, että se on suoraan saatavilla päävalikosta
                        mSing.mValikko.mTekstiTiedosto = mSing.mValikko.mTiedosto;
                        mSing.mValikko.TuoEsille(mSing.mValikko.mTiedosto, 'T');

                        SuspendLayout();
                        this.kanvaasi.Size = new Size(siirto5, siirto6);
                        this.kanvaasi.PerformLayout();
                        this.paneeli.HorizontalScroll.Value = siirto3;
                        this.paneeli.PerformLayout();
                        this.paneeli.VerticalScroll.Value = siirto4;
                        this.paneeli.PerformLayout();
                        ResumeLayout();

                        //Asetetaan haluttu taustaväri ja sivun otsikko tässä välillä, kun taustavärin ja
                        //otsikon tietokin on saatu Teksti.dll:ltä
                        if (taustaVari != 0xFF000000)
                        {
                            this.kanvaasi.BackColor = mTaustaVari = AnnaVari(taustaVari);
                        }
                        else
                        {
                            this.kanvaasi.BackColor= mTaustaVari = Color.White;
                        }
                        char[] ots = new char[200];
                        Marshal.Copy(otsikko, ots, 0, 200);
                        string otsi = new string(ots, 0, ots.Length);
                        int loppu = otsi.IndexOf('\0');
                        otsi = otsi.Remove(loppu);
                        if (otsi.Length != 0)
                        {
                            string ot = "Text view - ";
                            this.Text = ot + otsi;
                        }
                        else
                        {
                            this.Text = "Text view";
                        }

                        //otetaan otsikkolista talteen
                        TuoOtsikot();

                        //Pyydetään Teksti.dll:ää päivittämään näkymä
                        PiirraViipeella();

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

            //otsikko pitää deletoida manuaalisesti
            Marshal.FreeHGlobal(otsikko);
        }

        //Tällä funktiolla saadaan uint luvusta Color ilmentymä
        private Color AnnaVari(uint argb)
        {
            return Color.FromArgb((byte)(255),
                (byte)((argb & 0x000000ff)),
                (byte)((argb & 0x0000ff00) >> 0x08),
                (byte)(argb & 0x00ff0000) >> 0x16);
        }

        //Apumetodi otsikoiden lataamiseksi otsikkolistaan
        private void TuoOtsikot()
        {

            //alustetaan paikalliset muuttujat
            char[] siirto1 = new char[200];

            //tämän funktion tehtävänä on ladata uudet valinnat otsikkoListaan. Tyhjennetään ensin lista
            otsikkoLista.Items.Clear();

            //sitten tallennetaan kaikki tekstit otsikkoListaan
            for (int i = 0; i < mListanPituus; i++)
            {

                Marshal.Copy(mOtsikot[i], siirto1, 0, 200);
                string siirto2 = new string(siirto1, 0, siirto1.Length);
                int loppu = siirto2.IndexOf('\0');
                siirto2 = siirto2.Remove(loppu);
                otsikkoLista.Items.Add(siirto2);
            }


            //annetaan otsikkoListalle vakio-otsikko. Se on pakko lisätä listaan DropDownList-moodissa
            otsikkoLista.Items.Add("Paragraphs");
            otsikkoLista.Text = "Paragraphs";
        }



        //Käyttäjä on klikannut eteen nappulaa
        private void EteenClick(object sender, EventArgs e)
        {
            int siirto1 = 0, siirto2 = 0, siirto3 = 0, siirto4 = 0;
            IntPtr otsikko = Marshal.AllocHGlobal(200 * sizeof(char));
            uint taustaVari = 0xFF000000;

            //Taakse nappia on klikattu. Pyydetään Teksti.dll:ää avaamaan aiempi HTML näkymä
            historyForward(ref siirto1, ref siirto2, ref siirto3, ref siirto4,
                ref taustaVari, otsikko, mOtsikot, ref mListanPituus);

            //Siirretään uuden HTML sivun mittatiedot käyttöön
            SuspendLayout();
            this.kanvaasi.Size = new Size(siirto4, siirto2);
            this.kanvaasi.PerformLayout();
            this.paneeli.HorizontalScroll.Value = siirto3;
            this.paneeli.PerformLayout();
            this.paneeli.VerticalScroll.Value = siirto1;
            this.paneeli.PerformLayout();
            ResumeLayout();

            //Asetetaan haluttu taustaväri ja sivun otsikko tässä välillä, kun taustavärin ja
            //otsikon tietokin on saatu Teksti.dll:ltä
            if (taustaVari != 0xFF000000)
            {
                this.paneeli.BackColor = mTaustaVari = AnnaVari(taustaVari);
            }
            else
            {
                this.paneeli.BackColor = mTaustaVari = Color.White;
            }
            char[] ots = new char[200];
            Marshal.Copy(otsikko, ots, 0, 200);
            string otsi = new string(ots, 0, ots.Length);
            int loppu = otsi.IndexOf('\0');
            otsi = otsi.Remove(loppu);
            if (otsi.Length != 0)
            {
                string ot = "Text view - ";
                this.Text = ot + otsi;
            }
            else
            {
                this.Text = "Text view";
            }

            //otetaan otsikkolista talteen
            TuoOtsikot();

            //Pyydetään Teksti.dll:ää päivittämään näkymä
            MuutaKokoa(this, EventArgs.Empty);
            PiirraViipeella();

            //Lopuksi pitää vielä varmistaa, voiko klikata eteen tai taakse nappia
            this.taakse.Enabled = historyCanBack();
            this.eteen.Enabled = historyCanForward();

            //otsikko pitää deletoida manuaalisesti
            Marshal.FreeHGlobal(otsikko);

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
            IntPtr otsikko = Marshal.AllocHGlobal(200 * sizeof(char));
            uint taustaVari = 0xFF000000;

            //Taakse nappia on klikattu. Pyydetään Teksti.dll:ää avaamaan aiempi HTML näkymä
            historyBack(ref siirto1, ref siirto2, ref siirto3, ref siirto4,
                ref taustaVari, otsikko, mOtsikot, ref mListanPituus);


            //otetaan otsikkolista talteen
            TuoOtsikot();

            //Siirretään uuden HTML sivun mittatiedot TekstiIkkunalle
            SuspendLayout();
            this.kanvaasi.Size = new Size(siirto4, siirto2);
            this.kanvaasi.PerformLayout();
            this.paneeli.HorizontalScroll.Value = siirto3;
            this.paneeli.PerformLayout();
            this.paneeli.VerticalScroll.Value = siirto1;
            this.paneeli.PerformLayout();
            ResumeLayout();

            //Asetetaan haluttu taustaväri ja sivun otsikko tässä välillä, kun taustavärin ja
            //otsikon tietokin on saatu Teksti.dll:ltä
            if (taustaVari != 0xFF000000)
            {
                this.paneeli.BackColor = mTaustaVari = AnnaVari(taustaVari);
            }
            else
            {
                this.paneeli.BackColor= mTaustaVari = Color.White;
            }
            char[] ots = new char[200];
            Marshal.Copy(otsikko, ots, 0, 200);
            string otsi = new string(ots, 0, ots.Length);
            int loppu = otsi.IndexOf('\0');
            otsi = otsi.Remove(loppu);
            if (otsi.Length != 0)
            {
                string ot = "Text view - ";
                this.Text = ot + otsi;
            }
            else
            {
                this.Text = "Text view";
            }

            //Pyydetään Teksti.dll:ää päivittämään näkymä
            MuutaKokoa(this, EventArgs.Empty);
            PiirraViipeella();

            //Lopuksi pitää vielä varmistaa, voiko klikata eteen tai taakse nappia
            this.taakse.Enabled = historyCanBack();
            this.eteen.Enabled = historyCanForward();

            //otsikko pitää deletoida manuaalisesti
            Marshal.FreeHGlobal(otsikko);
        }

        //käyttäjä muuttaa TekstiIkkunan kokoa
        public void MuutaKokoa(object sender, EventArgs e)
        {

            //koska Ikkunaa luotaessa windows kutsuu tätä funktiota ennen kuin
            //tätä funktiota on luotu tarvitaan ehdollistus
            if (mSing != null && mSing.mValikko.mTekstiIkkuna != null)
            {

                //alustetaan paikalliset muuttujat
                int siirto1, siirto2, siirto3, siirto4, siirto5, siirto6, siirto7, siirto8;

                //koska VCL-muuttujien käyttö suoraan on onglemallista, täytyy käyttää siirtoa
                siirto1 = 1;
                siirto2 = 1;
                siirto3 = this.paneeli.HorizontalScroll.Value;
                siirto4 = this.paneeli.VerticalScroll.Value;
                siirto5 = this.kanvaasi.Width;
                siirto6 = this.kanvaasi.Height;
                siirto7 = this.Width;
                siirto8 = this.Height;

                //Ensiksi Teksti.dll:lle pitää kertoa ikkunan mitat
                annaKorkeusLeveys(ref siirto1, ref siirto2, ref siirto3, ref siirto4,
                    ref siirto5, ref siirto6, ref siirto7, ref siirto8);

                //tallennetaan takaisin mahdollisesti muutetut arvot
                SuspendLayout();
                this.kanvaasi.Size = new Size(siirto5, siirto6);
                this.kanvaasi.PerformLayout();
                this.paneeli.HorizontalScroll.Value = siirto3;
                this.paneeli.PerformLayout();
                this.paneeli.VerticalScroll.Value = siirto4;
                this.paneeli.PerformLayout();
                ResumeLayout();

                //Pyydetään Teksti.dll:ää päivittämään näkymä
                this.kanvaasi.BackColor = mTaustaVari;
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
            if (e.Delta <= 0)
            {
                this.paneeli.ScrollaaAlas();
                MuutaKokoa(sender, EventArgs.Empty);
            }
            else
            {
                this.paneeli.ScrollaaYlos();
                MuutaKokoa(sender, EventArgs.Empty);
            }
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

                //tämä suorittaa tarvittavat päivitykset, vaikka ikkunan kokoa ei olekaan muutettu
                MuutaKokoa(this, EventArgs.Empty);
            }

            //Jos vasenta hiiren nappia klikataan
            if (e.Button == MouseButtons.Left)  //saakoKlikata
            {

                int siirto = 100;    //EVT_LEFT_DOWN = 100
                siirto1 = this.kanvaasi.Width;
                siirto2 = this.kanvaasi.Height;
                siirto3 = this.paneeli.HorizontalScroll.Value;
                siirto4 = this.paneeli.VerticalScroll.Value;
                onMouseEvent(ref x, ref y, ref siirto, true, false,
                    ref siirto1, ref siirto2, ref siirto3, ref siirto4, ref taustaVari);
                SuspendLayout();
                this.kanvaasi.Size = new Size(siirto1, siirto2);
                this.kanvaasi.PerformLayout();
                this.paneeli.HorizontalScroll.Value = siirto3;
                this.paneeli.PerformLayout();
                this.paneeli.VerticalScroll.Value = siirto4;
                this.paneeli.PerformLayout();
                ResumeLayout();

                //tämä suorittaa tarvittavat päivitykset, vaikka ikkunan kokoa ei olekaan muutettu
                MuutaKokoa(this, EventArgs.Empty);
            }
        }

        //kun käyttäjä on klikannut jotain otsikkolistan otsikkoa, suoritetaan tämä funktio
        private void ValittuOtsikko(object sender, EventArgs e)
        {
            int siirto1 = -1, siirto2 = -1, siirto3 = -1, siirto4 = -1;

            var otsikko = otsikkoLista.ComboBox.GetItemText(otsikkoLista.ComboBox.SelectedItem);
            if (otsikko != "Paragraphs")
            {
                siirryAnkkuriin(otsikko, ref siirto1, ref siirto2, ref siirto3, ref siirto4);
                SuspendLayout();
                this.kanvaasi.Size = new Size(siirto1, siirto2);
                this.kanvaasi.PerformLayout();
                this.paneeli.HorizontalScroll.Value = siirto3;
                this.paneeli.PerformLayout();
                this.paneeli.VerticalScroll.Value = siirto4;
                this.paneeli.PerformLayout();
                ResumeLayout();

                //focus jää kiinni otsikkoListaan ja se pitää poistaa siitä
                this.fokusaattori.Focus();

                //tämä suorittaa tarvittavat päivitykset, vaikka ikkunan kokoa ei olekaan muutettu
                MuutaKokoa(this, EventArgs.Empty);
                PiirraViipeella();
            }
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
                        skaalaY = this.kanvaasi.Height - this.Height;
                    }
                    else
                    {
                        uusiY = this.paneeli.VerticalScroll.Value - y + startY;
                    }

                    int siirto = 100;    //  EVT_LEFT_DOWN = 100
                    siirto1 = this.kanvaasi.Width;
                    siirto2 = this.kanvaasi.Height;
                    siirto3 = this.paneeli.HorizontalScroll.Value;

                    if (uusiY > 0 && uusiY < skaalaY)
                    {
                        SuspendLayout();
                        this.paneeli.VerticalScroll.Value = uusiY;
                        this.paneeli.PerformLayout();
                        ResumeLayout();
                        siirto4 = uusiY;
                    }
                    else
                    {
                        siirto4 = this.paneeli.VerticalScroll.Value;
                    }

                    vastaus = onMouseEvent(ref x, ref y, ref siirto, true, false,
                        ref siirto1, ref siirto2, ref siirto3, ref siirto4, ref taustaVari);
                    SuspendLayout();
                    this.kanvaasi.Size = new Size(siirto1, siirto2);
                    this.kanvaasi.PerformLayout();
                    this.paneeli.HorizontalScroll.Value = siirto3;
                    this.paneeli.PerformLayout();
                    this.paneeli.VerticalScroll.Value = siirto4;
                    this.paneeli.PerformLayout();
                    ResumeLayout();

                    //tämä suorittaa tarvittavat päivitykset, vaikka ikkunan kokoa ei olekaan muutettu
                    MuutaKokoa(sender, e);
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
                this.Cursor = Cursors.Hand;
            }
            else
            {
                this.Cursor = Cursors.Arrow;
            }

            //Lopuksi pitää vielä varmistaa, voiko klikata eteen tai taakse nappia
            this.taakse.Enabled = historyCanBack();
            this.eteen.Enabled = historyCanForward();
        }

        //hiiri poistuu paneelin alueelta
        private void HiiriPoistuu(object sender, EventArgs e)
        {
            //            Cursor.Current = Cursors.Arrow;
        }

        //otsikkoListan otsikot pitää päivittää, kun käyttäjä selailee HTML-sivuja
        //päivitys tehdään aina, kun otsikkolista avataan, riippumatta siitä, onko
        //päivitykselle tarvetta vai ei
        private void PaivitaOtsikot(object sender, EventArgs e)
        {
            paivitaOtsikkoLista(mOtsikot, ref mListanPituus);
            TuoOtsikot();

            //focus jää kiinni otsikkoListaan ja se pitää poistaa siitä
            //           otsikkoLista.Enabled = false;
            //           otsikkoLista.Enabled = true;
            this.fokusaattori.Focus();
        }

        //kirjanmerkki tarvitsee muistiinsa HTML-tiedoston näkymän
        public void AnnaOrientaatio(int[] orientaatio)
        {
            orientaatio[0] = this.kanvaasi.Width;
            orientaatio[1] = this.kanvaasi.Height;
            orientaatio[2] = this.paneeli.HorizontalScroll.Value;
            orientaatio[3] = this.paneeli.VerticalScroll.Value;
        }

        //kun kirjanmerkki avataan, tämä funktio tuo muistista edellisen näkymän
        public void AsetaOrientaatio(int[] orientaatio)
        {
            bool vasenOikea = true;
            int siirto0 = orientaatio[0];
            int siirto1 = orientaatio[1];
            int siirto2 = orientaatio[2];
            int siirto3 = orientaatio[3];
            lataaUudestaan(ref siirto0, ref siirto1, ref siirto2, ref siirto3, ref vasenOikea);
            SuspendLayout();
            this.kanvaasi.Size = new Size(siirto0, siirto1);
            this.kanvaasi.PerformLayout();
            this.paneeli.HorizontalScroll.Value = siirto2;
            this.paneeli.PerformLayout();
            this.paneeli.VerticalScroll.Value = siirto3;
            this.paneeli.PerformLayout();
            ResumeLayout();
            if (!vasenOikea)
            {
                this.taakse.Enabled = false;
                this.eteen.Enabled = false;
            }
        }

        //tämä apumetodi tarvitaan poistamaan otsikkoListan otsikko valittavien listasta
        private void PoistaOtsikko(object sender, EventArgs e)
        {
            if (otsikkoLista.Items.Contains("Paragraphs"))
                otsikkoLista.Items.Remove("Paragraphs");
        }
    }


    public class Paneeli : Panel
    {
        Singleton mSing;
        int hiword, loword;
        int pos = 0;

        public Paneeli()
        {
            mSing = Singleton.AnnaIlmentyma;
        }

        //jos tätä metodia ei määritellä uudestaan, paneeli siirtyy yläasentoon aina
        //menettäessään focuksen
        protected override System.Drawing.Point ScrollToControl(System.Windows.Forms.Control ac)
        {
            return this.DisplayRectangle.Location;
        }

        //Teksti.dll haluaa ikkunaKahvan, joka pysyy samana koko ohjelman suorituksen ajan
        //(EI TARVISE ENÄÄ SAMANA PYSYVÄÄ IKKUNAKAHVAA!)
        //Lisäksi halutaan, että vierintäpalkit ovat aina näkyvissä
        protected override CreateParams CreateParams
        {
            get
            {
                Int32 CS_OWNDC = 0x20;
                var cp = base.CreateParams;
                cp.ClassStyle = cp.ClassStyle | CS_OWNDC;
                cp.Style = cp.Style | unchecked((int)WinM.WM_HSCROLL | (int)WinM.WM_VSCROLL);
                return cp;
            }
        }

        //tällä metodilla paneelia voi scrollata alas
        public void ScrollaaAlas()
        {
            if (pos < 0) pos = 0;
            pos++;
            using (Control c = new Control() { Parent = this, Height = 1, Top = this.ClientSize.Height + pos })
            {
                this.ScrollControlIntoView(c);
            }
            this.PerformLayout();
        }

        //tällä metodilla paneelia voi scrollata ylös
        public void ScrollaaYlos()
        {
            if (pos >= 0) pos = -1;
            pos--;
            using (Control c = new Control() { Parent = this, Height = 1, Top = pos })
            {
                this.ScrollControlIntoView(c);
            }
            this.PerformLayout();
        }


        //Tämä funktio käsittelee scrollboxiin osoitetut Windows viestit
        protected override void WndProc(ref Message m)
        {

            //tehdään ensin kaikki normaalisti
            base.WndProc(ref m);

            //sitten omat jutut
            switch (m.Msg)
            {
                case (int)WinM.WM_HSCROLL:
                    // horizontal scrolling 
                    loword = (int)m.WParam;

                    //otetaan win32 LOWORD
                    loword = loword & 0x0000FFFF;
                    switch (loword)
                    {
                        case 6:   //  SB_TOP = 6

                            mSing.mValikko.mTekstiIkkuna.MuutaKokoa(this, EventArgs.Empty);
                            break;

                        case 0:   //  SB_LINEUP = 0
                            mSing.mValikko.mTekstiIkkuna.MuutaKokoa(this, EventArgs.Empty);
                            break;

                        case 4:    //  SB_THUMBPOSITION = 4
                            mSing.mValikko.mTekstiIkkuna.MuutaKokoa(this, EventArgs.Empty);
                            break;

                        case 5:   //  SB_THUMBTRACK = 5
                            mSing.mValikko.mTekstiIkkuna.MuutaKokoa(this, EventArgs.Empty);
                            break;

                        case 1:    //  SB_LINEDOWN = 1
                            mSing.mValikko.mTekstiIkkuna.MuutaKokoa(this, EventArgs.Empty);
                            break;

                        case 7:   // SB_BOTTOM = 7
                            mSing.mValikko.mTekstiIkkuna.MuutaKokoa(this, EventArgs.Empty);
                            break;

                        case 8:   //  SB_ENDSCROLL = 8
                            break;
                    }
                    break;
                case (int)WinM.WM_VSCROLL:
                    // vertical scrolling 
                    loword = (int)m.WParam;

                    //otetaan win32 LOWORD
                    loword = loword & 0x0000FFFF;
                    switch (loword)
                    {
                        case 6:   //  SB_TOP = 6
                            mSing.mValikko.mTekstiIkkuna.MuutaKokoa(this, EventArgs.Empty);
                            break;

                        case 0:   //  SB_LINEUP = 0
                            mSing.mValikko.mTekstiIkkuna.MuutaKokoa(this, EventArgs.Empty);
                            break;

                        case 4:    //  SB_THUMBPOSITION = 4
                            mSing.mValikko.mTekstiIkkuna.MuutaKokoa(this, EventArgs.Empty);
                            break;

                        case 5:   //  SB_THUMBTRACK = 5
                            mSing.mValikko.mTekstiIkkuna.MuutaKokoa(this, EventArgs.Empty);
                            break;

                        case 1:    //  SB_LINEDOWN = 1
                            mSing.mValikko.mTekstiIkkuna.MuutaKokoa(this, EventArgs.Empty);
                            break;

                        case 7:   // SB_BOTTOM = 7
                            mSing.mValikko.mTekstiIkkuna.MuutaKokoa(this, EventArgs.Empty);
                            break;

                        case 8:   //  SB_ENDSCROLL = 8
                            break;
                    }
                    break;
            }
        }
    }
}

