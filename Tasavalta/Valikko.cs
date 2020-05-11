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
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Diagnostics;



namespace Tasavalta
{

    public enum WinM : uint
    {

        WS_POPUP = 0x80000000,
        WS_BORDER = 0x00800000,
        WS_SYSMENU = 0x00010000,
        WS_CLIPCHILDREN = 0x02000000,
        WS_OVERLAPPED = 0x00000000,
        WS_CAPTION = 0x00C00000,
        WS_THICKFRAME = 0x00040000,
        WS_MINIMIZEBOX = 0x00020000,
        WS_MAXIMIZEBOX = 0x00010000,

        WM_NULL = 0x0000,
        WM_CREATE = 0x0001,
        WM_DESTROY = 0x0002,
        WM_MOVE = 0x0003,
        WM_SIZE = 0x0005,
        WM_ACTIVATE = 0x0006,
        WM_SETFOCUS = 0x0007,
        WM_KILLFOCUS = 0x0008,
        WM_ENABLE = 0x000A,
        WM_SETREDRAW = 0x000B,
        WM_SETTEXT = 0x000C,
        WM_GETTEXT = 0x000D,
        WM_GETTEXTLENGTH = 0x000E,
        WM_PAINT = 0x000F,
        WM_CLOSE = 0x0010,
        WM_QUERYENDSESSION = 0x0011,
        WM_QUERYOPEN = 0x0013,
        WM_ENDSESSION = 0x0016,
        WM_QUIT = 0x0012,
        WM_ERASEBKGND = 0x0014,
        WM_SYSCOLORCHANGE = 0x0015,
        WM_SHOWWINDOW = 0x0018,
        WM_WININICHANGE = 0x001A,
        WM_SETTINGCHANGE = WM_WININICHANGE,
        WM_DEVMODECHANGE = 0x001B,
        WM_ACTIVATEAPP = 0x001C,
        WM_FONTCHANGE = 0x001D,
        WM_TIMECHANGE = 0x001E,
        WM_CANCELMODE = 0x001F,
        WM_SETCURSOR = 0x0020,
        WM_MOUSEACTIVATE = 0x0021,
        WM_CHILDACTIVATE = 0x0022,
        WM_QUEUESYNC = 0x0023,
        WM_GETMINMAXINFO = 0x0024,
        WM_PAINTICON = 0x0026,
        WM_ICONERASEBKGND = 0x0027,
        WM_NEXTDLGCTL = 0x0028,
        WM_SPOOLERSTATUS = 0x002A,
        WM_DRAWITEM = 0x002B,
        WM_MEASUREITEM = 0x002C,
        WM_DELETEITEM = 0x002D,
        WM_VKEYTOITEM = 0x002E,
        WM_CHARTOITEM = 0x002F,
        WM_SETFONT = 0x0030,
        WM_GETFONT = 0x0031,
        WM_SETHOTKEY = 0x0032,
        WM_GETHOTKEY = 0x0033,
        WM_QUERYDRAGICON = 0x0037,
        WM_COMPAREITEM = 0x0039,
        WM_GETOBJECT = 0x003D,
        WM_COMPACTING = 0x0041,
        WM_COMMNOTIFY = 0x0044,
        WM_WINDOWPOSCHANGING = 0x0046,
        WM_WINDOWPOSCHANGED = 0x0047,
        WM_POWER = 0x0048,
        WM_COPYDATA = 0x004A,
        WM_CANCELJOURNAL = 0x004B,
        WM_NOTIFY = 0x004E,
        WM_INPUTLANGCHANGEREQUEST = 0x0050,
        WM_INPUTLANGCHANGE = 0x0051,
        WM_TCARD = 0x0052,
        WM_HELP = 0x0053,
        WM_USERCHANGED = 0x0054,
        WM_NOTIFYFORMAT = 0x0055,
        WM_CONTEXTMENU = 0x007B,
        WM_STYLECHANGING = 0x007C,
        WM_STYLECHANGED = 0x007D,
        WM_DISPLAYCHANGE = 0x007E,
        WM_GETICON = 0x007F,
        WM_SETICON = 0x0080,
        WM_NCCREATE = 0x0081,
        WM_NCDESTROY = 0x0082,
        WM_NCCALCSIZE = 0x0083,
        WM_NCHITTEST = 0x0084,
        WM_NCPAINT = 0x0085,
        WM_NCACTIVATE = 0x0086,
        WM_GETDLGCODE = 0x0087,
        WM_SYNCPAINT = 0x0088,


        WM_NCMOUSEMOVE = 0x00A0,
        WM_NCLBUTTONDOWN = 0x00A1,
        WM_NCLBUTTONUP = 0x00A2,
        WM_NCLBUTTONDBLCLK = 0x00A3,
        WM_NCRBUTTONDOWN = 0x00A4,
        WM_NCRBUTTONUP = 0x00A5,
        WM_NCRBUTTONDBLCLK = 0x00A6,
        WM_NCMBUTTONDOWN = 0x00A7,
        WM_NCMBUTTONUP = 0x00A8,
        WM_NCMBUTTONDBLCLK = 0x00A9,
        WM_NCXBUTTONDOWN = 0x00AB,
        WM_NCXBUTTONUP = 0x00AC,
        WM_NCXBUTTONDBLCLK = 0x00AD,

        WM_INPUT_DEVICE_CHANGE = 0x00FE,
        WM_INPUT = 0x00FF,

        WM_KEYFIRST = 0x0100,
        WM_KEYDOWN = 0x0100,
        WM_KEYUP = 0x0101,
        WM_CHAR = 0x0102,
        WM_DEADCHAR = 0x0103,
        WM_SYSKEYDOWN = 0x0104,
        WM_SYSKEYUP = 0x0105,
        WM_SYSCHAR = 0x0106,
        WM_SYSDEADCHAR = 0x0107,
        WM_UNICHAR = 0x0109,
        WM_KEYLAST = 0x0109,

        WM_IME_STARTCOMPOSITION = 0x010D,
        WM_IME_ENDCOMPOSITION = 0x010E,
        WM_IME_COMPOSITION = 0x010F,
        WM_IME_KEYLAST = 0x010F,

        WM_INITDIALOG = 0x0110,
        WM_COMMAND = 0x0111,
        WM_SYSCOMMAND = 0x0112,
        WM_TIMER = 0x0113,
        WM_HSCROLL = 0x0114,
        WM_VSCROLL = 0x0115,
        WM_INITMENU = 0x0116,
        WM_INITMENUPOPUP = 0x0117,
        WM_MENUSELECT = 0x011F,
        WM_MENUCHAR = 0x0120,
        WM_ENTERIDLE = 0x0121,
        WM_MENURBUTTONUP = 0x0122,
        WM_MENUDRAG = 0x0123,
        WM_MENUGETOBJECT = 0x0124,
        WM_UNINITMENUPOPUP = 0x0125,
        WM_MENUCOMMAND = 0x0126,

        WM_CHANGEUISTATE = 0x0127,
        WM_UPDATEUISTATE = 0x0128,
        WM_QUERYUISTATE = 0x0129,

        WM_CTLCOLORMSGBOX = 0x0132,
        WM_CTLCOLOREDIT = 0x0133,
        WM_CTLCOLORLISTBOX = 0x0134,
        WM_CTLCOLORBTN = 0x0135,
        WM_CTLCOLORDLG = 0x0136,
        WM_CTLCOLORSCROLLBAR = 0x0137,
        WM_CTLCOLORSTATIC = 0x0138,
        MN_GETHMENU = 0x01E1,

        WM_MOUSEFIRST = 0x0200,
        WM_MOUSEMOVE = 0x0200,
        WM_LBUTTONDOWN = 0x0201,
        WM_LBUTTONUP = 0x0202,
        WM_LBUTTONDBLCLK = 0x0203,
        WM_RBUTTONDOWN = 0x0204,
        WM_RBUTTONUP = 0x0205,
        WM_RBUTTONDBLCLK = 0x0206,
        WM_MBUTTONDOWN = 0x0207,
        WM_MBUTTONUP = 0x0208,
        WM_MBUTTONDBLCLK = 0x0209,
        WM_MOUSEWHEEL = 0x020A,
        WM_XBUTTONDOWN = 0x020B,
        WM_XBUTTONUP = 0x020C,
        WM_XBUTTONDBLCLK = 0x020D,
        WM_MOUSEHWHEEL = 0x020E,

        WM_PARENTNOTIFY = 0x0210,
        WM_ENTERMENULOOP = 0x0211,
        WM_EXITMENULOOP = 0x0212,

        WM_NEXTMENU = 0x0213,
        WM_SIZING = 0x0214,
        WM_CAPTURECHANGED = 0x0215,
        WM_MOVING = 0x0216,

        WM_POWERBROADCAST = 0x0218,

        WM_DEVICECHANGE = 0x0219,

        WM_MDICREATE = 0x0220,
        WM_MDIDESTROY = 0x0221,
        WM_MDIACTIVATE = 0x0222,
        WM_MDIRESTORE = 0x0223,
        WM_MDINEXT = 0x0224,
        WM_MDIMAXIMIZE = 0x0225,
        WM_MDITILE = 0x0226,
        WM_MDICASCADE = 0x0227,
        WM_MDIICONARRANGE = 0x0228,
        WM_MDIGETACTIVE = 0x0229,


        WM_MDISETMENU = 0x0230,
        WM_ENTERSIZEMOVE = 0x0231,
        WM_EXITSIZEMOVE = 0x0232,
        WM_DROPFILES = 0x0233,
        WM_MDIREFRESHMENU = 0x0234,

        WM_IME_SETCONTEXT = 0x0281,
        WM_IME_NOTIFY = 0x0282,
        WM_IME_CONTROL = 0x0283,
        WM_IME_COMPOSITIONFULL = 0x0284,
        WM_IME_SELECT = 0x0285,
        WM_IME_CHAR = 0x0286,
        WM_IME_REQUEST = 0x0288,
        WM_IME_KEYDOWN = 0x0290,
        WM_IME_KEYUP = 0x0291,

        WM_MOUSEHOVER = 0x02A1,
        WM_MOUSELEAVE = 0x02A3,
        WM_NCMOUSEHOVER = 0x02A0,
        WM_NCMOUSELEAVE = 0x02A2,

        WM_WTSSESSION_CHANGE = 0x02B1,

        WM_TABLET_FIRST = 0x02c0,
        WM_TABLET_LAST = 0x02df,

        WM_CUT = 0x0300,
        WM_COPY = 0x0301,
        WM_PASTE = 0x0302,
        WM_CLEAR = 0x0303,
        WM_UNDO = 0x0304,
        WM_RENDERFORMAT = 0x0305,
        WM_RENDERALLFORMATS = 0x0306,
        WM_DESTROYCLIPBOARD = 0x0307,
        WM_DRAWCLIPBOARD = 0x0308,
        WM_PAINTCLIPBOARD = 0x0309,
        WM_VSCROLLCLIPBOARD = 0x030A,
        WM_SIZECLIPBOARD = 0x030B,
        WM_ASKCBFORMATNAME = 0x030C,
        WM_CHANGECBCHAIN = 0x030D,
        WM_HSCROLLCLIPBOARD = 0x030E,
        WM_QUERYNEWPALETTE = 0x030F,
        WM_PALETTEISCHANGING = 0x0310,
        WM_PALETTECHANGED = 0x0311,
        WM_HOTKEY = 0x0312,

        WM_PRINT = 0x0317,
        WM_PRINTCLIENT = 0x0318,

        WM_APPCOMMAND = 0x0319,

        WM_THEMECHANGED = 0x031A,

        WM_CLIPBOARDUPDATE = 0x031D,

        WM_DWMCOMPOSITIONCHANGED = 0x031E,
        WM_DWMNCRENDERINGCHANGED = 0x031F,
        WM_DWMCOLORIZATIONCOLORCHANGED = 0x0320,
        WM_DWMWINDOWMAXIMIZEDCHANGE = 0x0321,

        WM_GETTITLEBARINFOEX = 0x033F,

        WM_HANDHELDFIRST = 0x0358,
        WM_HANDHELDLAST = 0x035F,

        WM_AFXFIRST = 0x0360,
        WM_AFXLAST = 0x037F,

        WM_PENWINFIRST = 0x0380,
        WM_PENWINLAST = 0x038F,

        WM_APP = 0x8000,

        WM_USER = 0x0400,

        WM_REFLECT = WM_USER + 0x1C00,

        WM_VASENALHAALLA = WM_USER + 1,
        WM_LAHETALINKKI = WM_USER + 2
    }

    public struct DPI
    {
        public float DpiX;
        public float DpiY;
    }

    public partial class Valikko : Form
    {

        [DllImport("UxTheme.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsAppThemed();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetSystemMetrics(int numero);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)]string lpFileName);

        private readonly int SM_CXSCREEN = 0;
        bool mOnkoEng = true;
        bool mOnko3D;
        bool mTekstiAvattu = true;
        bool mCADAvattu = true;
        string mMuistiTiedosto;
        string mSijaintiPolku;
        Muisti muisti;
        Singleton mSing;

        public string mTiedosto;
        public string mAnkkuri;
        public string mCADTiedosto;
        public string mTekstiTiedosto;
        public bool mEiValaisua = false;
        public bool mPaivitetaanko = true;
        public bool mOnkoUXTeemaa { get; private set; } = false;
        public IntPtr mHwnd;
        public OpenGL mOpenGLIkkuna = null;
        public Teksti mTekstiIkkuna = null;
        public AlasVetoValikkoIkkuna mAlasVetoValikkoIkkuna = null;
        public DPI Dpi = new DPI();

        //Tällä funktiolla selvitetään, voiko käyttöjärjestelmä tarjota comctl32.dll 
        //version 6, mitä tarvitaan Visual Styles tyylien käyttöön
        bool OnkoUXTeemaa()
        {
            if (IsAppThemed())
                return true;
            else
                return false;
        }

        public Valikko()
        {

            mHwnd = this.Handle;

            //pyydetään Singleton luokan ilmentymä ja annetaan Singletonille tämän luokan osoitin
            mSing = Singleton.AnnaIlmentyma;
            mSing.asetaValikko(this);

            //ohjelman alkaessa pitää tehdä seuraavat aloitustoimet:
            //(1)selvittää voimassa oleva DPI eli Device Independent Pixel 
            //(1)selvittää, voiko tietokoneessa käyttää Visual Styles tyylejä
            //(2)säätää ikkunat ruudun resoluution mukaan,
            //(3)hakea muistitiedostosta kirjanmerkit ja tiedostotiedot sekä varmistaa niiden ajanmukaisuus
            //(4)luoda sen mukaisesti valikkoon vaihtehtoja sekä asetukset kohdilleen

            //Tämä suorittaa Form1.Designer.cs tiedoston koodin
            InitializeComponent();

            //(1): selvitetään DPI
            Graphics g = this.CreateGraphics();
            try
            {
                Dpi.DpiX = g.DpiX;
                Dpi.DpiY = g.DpiY;
            }
            finally
            {
                g.Dispose();
            }

            //(2): selvitetään, onko visual styles käytössä
            mOnkoUXTeemaa = OnkoUXTeemaa();

            //(3): määritetään leveysSuhde, eli suhde koodauskoneen ruudun vaakaresoluution (1600) ja asennus-
            //koneen vaakaresoluution välillä. Ikkunamitat tulee aina kertoa leveysSuhteella
            float leveysSuhde = ((float)GetSystemMetrics(SM_CXSCREEN) - 100.0f) / 1500.0f;
            this.Width = (int)(1500.0f * leveysSuhde);
            this.Height = (int)(100.0f * leveysSuhde);

            //Ikkunan sijainti
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(50, 50);

            //(4): ladataan sitten kirjanmerkit ja tiedostotiedot binääritiedostosta.
            //Selvitetään tämän exe-tiedoston sijainti
            String puskuri = Application.ExecutablePath;

            //koska tiedostopolku sisältää myös exe-tiedoston, katkaistaan Tasavalta.exe pois
            //tallennetaan polku myöhempää käyttöä varten
            char[] pois = new char[] { 'T', 'a', 's', 'a', 'v', 'a', 'l', 't', 'a', '.', 'e', 'x', 'e' };
            mSijaintiPolku = puskuri.TrimEnd(pois);

            //muistitiedoston avaamiseen tarvitaan myös tiedostonimi
            String lisaa = "Muistitiedosto.ini";
            mMuistiTiedosto = mSijaintiPolku.Insert(mSijaintiPolku.Length, lisaa);

            //ja avataan tiedosto Muistitiedosto.ini lukemista varten
            try {
                FileStream sisaanVirta = File.OpenRead(mMuistiTiedosto);

                //luetaan tiedoston sisältö luokkaan muisti
                if (!lueJaTarkasta(sisaanVirta, ref muisti))
                {

                    //jos ei onnistunut, käytetään aloitusmuistia
                    muisti = new Muisti();
                }
                sisaanVirta.Close();
            }
            catch
            {

                //jos ei onnistunut, käytetään aloitusmuistia
                muisti = new Muisti();
            }

        //(5): pääikkunaan pitää luoda tiedostotietojen mukaiset valikot. Aloitetaan
        //3D kentistä: palstat on luotava numerojärjestyksessä numerojärjestyksen aikaansaamiseksi
        hyppy:
            bool luotu = false;
            for (int i = 0; i < muisti.AnnaTiedostojenMaara(); i++)
            {
                Muisti.TIEDOSTOTIETO tt = muisti.AnnaTiedostoTieto(i);
                if (tt.valikkoSijainti == 1 && tt.onko3D)
                {

                    //jos palstaa ei ole vielä luotu, luodaan sellainen
                    if (!luotu)
                    {
                        N3D1 = new ToolStripMenuItem("3D 1");
                        menuStrip1.Items.Add(N3D1);
                        luotu = true;
                    }

                    //luodaan sitten varsinainen valikkokohta
                    ToolStripMenuItem siirto = new ToolStripMenuItem();
                    siirto.Text = tt.nimi;
                    siirto.Name = tt.nimi;
                    siirto.ToolTipText = "Watch";
                    siirto.Click += KlikattuValikkoa;
                    siirto.Enabled = tt.onkoAsetettu;
                    siirto.Visible = tt.onkoNakyva;
                    N3D1.DropDownItems.Add(siirto);
                }
            }
            luotu = false;
            for (int i = 0; i < muisti.AnnaTiedostojenMaara(); i++)
            {
                Muisti.TIEDOSTOTIETO tt = muisti.AnnaTiedostoTieto(i);
                if (tt.valikkoSijainti == 2 && tt.onko3D)
                {

                    //jos palstaa ei ole vielä luotu, luodaan sellainen
                    if (!luotu)
                    {
                        N3D2 = new ToolStripMenuItem("3D 2");
                        menuStrip1.Items.Add(N3D2);
                        luotu = true;
                    }

                    //luodaan sitten varsinainen valikkokohta
                    ToolStripMenuItem siirto = new ToolStripMenuItem();
                    siirto.Text = tt.nimi;
                    siirto.Name = tt.nimi;
                    siirto.ToolTipText = "Watch";
                    siirto.Click += KlikattuValikkoa;
                    siirto.Enabled = tt.onkoAsetettu;
                    siirto.Visible = tt.onkoNakyva;
                    N3D2.DropDownItems.Add(siirto);
                }
            }
            luotu = false;
            for (int i = 0; i < muisti.AnnaTiedostojenMaara(); i++)
            {
                Muisti.TIEDOSTOTIETO tt = muisti.AnnaTiedostoTieto(i);
                if (tt.valikkoSijainti == 3 && tt.onko3D)
                {

                    //jos palstaa ei ole vielä luotu, luodaan sellainen
                    if (!luotu)
                    {
                        N3D3 = new ToolStripMenuItem("3D 3");
                        menuStrip1.Items.Add(N3D3);
                        luotu = true;
                    }

                    //luodaan sitten varsinainen valikkokohta
                    ToolStripMenuItem siirto = new ToolStripMenuItem();
                    siirto.Text = tt.nimi;
                    siirto.Name = tt.nimi;
                    siirto.ToolTipText = "Watch";
                    siirto.Click += KlikattuValikkoa;
                    siirto.Enabled = tt.onkoAsetettu;
                    siirto.Visible = tt.onkoNakyva;
                    N3D3.DropDownItems.Add(siirto);
                }
            }
            luotu = false;
            for (int i = 0; i < muisti.AnnaTiedostojenMaara(); i++)
            {
                Muisti.TIEDOSTOTIETO tt = muisti.AnnaTiedostoTieto(i);
                if (tt.valikkoSijainti == 4 && tt.onko3D)
                {

                    //jos palstaa ei ole vielä luotu, luodaan sellainen
                    if (!luotu)
                    {
                        N3D4 = new ToolStripMenuItem("3D 4");
                        menuStrip1.Items.Add(N3D4);
                        luotu = true;
                    }

                    //luodaan sitten varsinainen valikkokohta
                    ToolStripMenuItem siirto = new ToolStripMenuItem();
                    siirto.Text = tt.nimi;
                    siirto.Name = tt.nimi;
                    siirto.ToolTipText = "Watch";
                    siirto.Click += KlikattuValikkoa;
                    siirto.Enabled = tt.onkoAsetettu;
                    siirto.Visible = tt.onkoNakyva;
                    N3D4.DropDownItems.Add(siirto);
                }
            }
            luotu = false;
            for (int i = 0; i < muisti.AnnaTiedostojenMaara(); i++)
            {
                Muisti.TIEDOSTOTIETO tt = muisti.AnnaTiedostoTieto(i);
                if (tt.valikkoSijainti == 5 && tt.onko3D)
                {

                    //jos palstaa ei ole vielä luotu, luodaan sellainen
                    if (!luotu)
                    {
                        N3D5 = new ToolStripMenuItem("3D 5");
                        menuStrip1.Items.Add(N3D5);
                        luotu = true;
                    }

                    //luodaan sitten varsinainen valikkokohta
                    ToolStripMenuItem siirto = new ToolStripMenuItem();
                    siirto.Text = tt.nimi;
                    siirto.Name = tt.nimi;
                    siirto.ToolTipText = "Watch";
                    siirto.Click += KlikattuValikkoa;
                    siirto.Enabled = tt.onkoAsetettu;
                    siirto.Visible = tt.onkoNakyva;
                    N3D5.DropDownItems.Add(siirto);
                }
            }
            luotu = false;

            //sitten luodaan tiedostotietojen mukaiset valikot stooreille. Myös nämä pitää
            //luoda järjestyksessä
            for (int i = 0; i < muisti.AnnaTiedostojenMaara(); i++)
            {
                Muisti.TIEDOSTOTIETO tt = muisti.AnnaTiedostoTieto(i);
                if (tt.valikkoSijainti == 1 && !tt.onko3D)
                {

                    //jos palstaa ei ole vielä luotu, luodaan sellainen
                    if (!luotu)
                    {
                        Stories1 = new ToolStripMenuItem("Stories 1");
                        menuStrip1.Items.Add(Stories1);
                        luotu = true;
                    }

                    //luodaan sitten varsinainen valikkokohta
                    ToolStripMenuItem siirto = new ToolStripMenuItem();
                    siirto.Text = tt.nimi;
                    siirto.Name = tt.nimi;
                    siirto.ToolTipText = "Read";
                    siirto.Click += KlikattuValikkoa;
                    siirto.Enabled = tt.onkoAsetettu;
                    siirto.Visible = tt.onkoNakyva;
                    Stories1.DropDownItems.Add(siirto);
                }
            }
            luotu = false;
            for (int i = 0; i < muisti.AnnaTiedostojenMaara(); i++)
            {
                Muisti.TIEDOSTOTIETO tt = muisti.AnnaTiedostoTieto(i);
                if (tt.valikkoSijainti == 2 && !tt.onko3D)
                {

                    //jos palstaa ei ole vielä luotu, luodaan sellainen
                    if (!luotu)
                    {
                        Stories2 = new ToolStripMenuItem("Stories 2");
                        menuStrip1.Items.Add(Stories2);
                        luotu = true;
                    }

                    //luodaan sitten varsinainen valikkokohta
                    ToolStripMenuItem siirto = new ToolStripMenuItem();
                    siirto.Text = tt.nimi;
                    siirto.Name = tt.nimi;
                    siirto.ToolTipText = "Read";
                    siirto.Click += KlikattuValikkoa;
                    siirto.Enabled = tt.onkoAsetettu;
                    siirto.Visible = tt.onkoNakyva;
                    Stories2.DropDownItems.Add(siirto);
                }
            }
            luotu = false;
            for (int i = 0; i < muisti.AnnaTiedostojenMaara(); i++)
            {
                Muisti.TIEDOSTOTIETO tt = muisti.AnnaTiedostoTieto(i);
                if (tt.valikkoSijainti == 3 && !tt.onko3D)
                {

                    //jos palstaa ei ole vielä luotu, luodaan sellainen
                    if (!luotu)
                    {
                        Stories3 = new ToolStripMenuItem("Stories 3");
                        menuStrip1.Items.Add(Stories3);
                        luotu = true;
                    }

                    //luodaan sitten varsinainen valikkokohta
                    ToolStripMenuItem siirto = new ToolStripMenuItem();
                    siirto.Text = tt.nimi;
                    siirto.Name = tt.nimi;
                    siirto.ToolTipText = "Read";
                    siirto.Click += KlikattuValikkoa;
                    siirto.Enabled = tt.onkoAsetettu;
                    siirto.Visible = tt.onkoNakyva;
                    Stories3.DropDownItems.Add(siirto);
                }
            }
            luotu = false;
            for (int i = 0; i < muisti.AnnaTiedostojenMaara(); i++)
            {
                Muisti.TIEDOSTOTIETO tt = muisti.AnnaTiedostoTieto(i);
                if (tt.valikkoSijainti == 4 && !tt.onko3D)
                {

                    //jos palstaa ei ole vielä luotu, luodaan sellainen
                    if (!luotu)
                    {
                        Stories4 = new ToolStripMenuItem("Stories 4");
                        menuStrip1.Items.Add(Stories4);
                        luotu = true;
                    }

                    //luodaan sitten varsinainen valikkokohta
                    ToolStripMenuItem siirto = new ToolStripMenuItem();
                    siirto.Text = tt.nimi;
                    siirto.Name = tt.nimi;
                    siirto.ToolTipText = "Read";
                    siirto.Click += KlikattuValikkoa;
                    siirto.Enabled = tt.onkoAsetettu;
                    siirto.Visible = tt.onkoNakyva;
                    Stories4.DropDownItems.Add(siirto);
                }
            }
            luotu = false;
            for (int i = 0; i < muisti.AnnaTiedostojenMaara(); i++)
            {
                Muisti.TIEDOSTOTIETO tt = muisti.AnnaTiedostoTieto(i);
                if (tt.valikkoSijainti == 5 && !tt.onko3D)
                {

                    //jos palstaa ei ole vielä luotu, luodaan sellainen
                    if (!luotu)
                    {
                        Stories5 = new ToolStripMenuItem("Stories 5");
                        menuStrip1.Items.Add(Stories5);
                        luotu = true;
                    }

                    //luodaan sitten varsinainen valikkokohta
                    ToolStripMenuItem siirto = new ToolStripMenuItem();
                    siirto.Text = tt.nimi;
                    siirto.Name = tt.nimi;
                    siirto.ToolTipText = "Read";
                    siirto.Click += KlikattuValikkoa;
                    siirto.Enabled = tt.onkoAsetettu;
                    siirto.Visible = tt.onkoNakyva;
                    Stories5.DropDownItems.Add(siirto);
                }
            }

            //lopuksi luodaan Help valikko, joka sekin täytyy luoda dynaamisesti
            Apua = new ToolStripMenuItem("Help");
            menuStrip1.Items.Add(Apua);

            ToolStripMenuItem siirto1 = new ToolStripMenuItem();
            siirto1.Text = "Ohjeet";
            siirto1.Name = "Ohjeet";
            siirto1.ToolTipText = "Read";
            siirto1.Click += KlikattuValikkoa;
            siirto1.Enabled = true;
            siirto1.Visible = true;
            Apua.DropDownItems.Add(siirto1);

            //loppujen lopuksi pitää vielä asettaa kirjanmerkkien näkymättömyydet kohdilleen
            toolStripMenuItem1.Visible = muisti.OnkoKirjanMerkkiAsetettu(0);
            toolStripMenuItem2.Visible = muisti.OnkoKirjanMerkkiAsetettu(1);
            toolStripMenuItem3.Visible = muisti.OnkoKirjanMerkkiAsetettu(2);
            toolStripMenuItem4.Visible = muisti.OnkoKirjanMerkkiAsetettu(3);
            toolStripMenuItem5.Visible = muisti.OnkoKirjanMerkkiAsetettu(4);
            toolStripMenuItem6.Visible = muisti.OnkoKirjanMerkkiAsetettu(5);
            toolStripMenuItem7.Visible = muisti.OnkoKirjanMerkkiAsetettu(6);
            toolStripMenuItem8.Visible = muisti.OnkoKirjanMerkkiAsetettu(7);
            toolStripMenuItem9.Visible = muisti.OnkoKirjanMerkkiAsetettu(8);

        }

        //kun käyttäjä klikkaa jotain dynaamisesti luotua valikkokohtaa, siirtyy
        //klikkitapahtuma tämän funktion käsiteltäväksi
        private void KlikattuValikkoa(object sender, EventArgs e)
        {

            //suoritetaan tyyppimuunnos kantaluokasta ToolStripMenuItem luokkaan
            //ja siitä edelleen stringiksi
            ToolStripMenuItem siirto = (ToolStripMenuItem)sender;
            string siirto2 = siirto.Name;

            if (siirto.ToolTipText == "Watch")
            {

                //jos kyseessä on 3D-kenttä
                mOnko3D = true;
                mTiedosto = siirto2 + ".cad";

                //jos tarpeen, luodaan opengl ikkuna
                if (mOpenGLIkkuna == null)
                {
                    mOpenGLIkkuna = new OpenGL();
                    this.AddOwnedForm(mOpenGLIkkuna);
                }

                mOpenGLIkkuna.AvaaCAD();
            }
            else
            {

                //jos tarpeen, luodaan teksti ikkuna
                if (mTekstiIkkuna == null)
                {
                    mTekstiIkkuna = new Teksti();
                    this.AddOwnedForm(mTekstiIkkuna);
                }

                //kyseessä on tekstikenttä
                mOnko3D = false;
                if (mOnkoEng)
                {
                    mTiedosto = siirto2 + ".eng";
                }
                else
                {
                    mTiedosto = siirto2 + ".fin";
                }

                if (mAnkkuri != null) mAnkkuri = mAnkkuri.Remove(0);
                mTekstiIkkuna.AvaaTeksti();
            }
        }

        //tämä funktio muodostaa tiedoston sisällöstä Muisti luokan, jos se on mahdollista
        private bool lueJaTarkasta(FileStream virta, ref Muisti m)
        {

            BinaryFormatter bin = new BinaryFormatter();
            try
            {
                m = (Muisti)bin.Deserialize(virta);
                return true;
            }
            catch (SerializationException e)
            {
                System.Diagnostics.Debug.WriteLine("Failed to deserialize. Reason: " + e.Message);
                return false;
            }
        }

        //tämä funktio vaihtaa suomen ja englannin kielen välillä käyttäjän klikkauksesta
        private void ChangeLanguageClick(object sender, EventArgs e)
        {
            if (languageEnglishToolStripMenuItem.Checked == true)
            {
                mOnkoEng = false;
                languageEnglishToolStripMenuItem.Text = "Language finnish";
            }
            else
            {
                mOnkoEng = true;
                languageEnglishToolStripMenuItem.Text = "Language english";
            }
        }

        //tämä funktio vaihtaa ARB- ja OpenGL 5.1 renderoinnin välillä käyttäjän klikkauksesta
        private void UseSimpleGraphicsClick(object sender, EventArgs e)
        {

            if (useElaborateGraphicsToolStripMenuItem.Checked == true)
            {
                mEiValaisua = true;
                useElaborateGraphicsToolStripMenuItem.Text = "Use simple graphics";
            }
            else
            {
                mEiValaisua = false;
                useElaborateGraphicsToolStripMenuItem.Text = "Use elaborate graphics";
            }
            if (mOpenGLIkkuna != null) mOpenGLIkkuna.Close();
        }

        //käyttäjä haluaa sulkea Tasavalta-sovelluksen
        private void ExitClick(object sender, EventArgs e)
        {
            this.Close();
        }

        //tätä funktiota kutsutaan aina, kun Tasavalta-sovellus suljetaan
        private void lopetus(object sender, FormClosedEventArgs e)
        {

            //tuhotaan vanha muistitiedosto
            tuhoaTiedosto(mMuistiTiedosto);

            //kun ohjelma loppuu, tallennetaan tietue muisti kovalevylle
            try
            {
                FileStream ulosVirta = File.OpenWrite(mMuistiTiedosto);
                tallennaJaVarmista(ulosVirta, muisti);
                ulosVirta.Close();
            }
            catch
            {

            }
        }

        //tällä funktiolla serialisoidaan tallennetaan kovalevylle luokka muisti
        private void tallennaJaVarmista(FileStream virta, Muisti muisti)
        {
            try
            {
                BinaryFormatter bin = new BinaryFormatter(null, new StreamingContext(StreamingContextStates.File));
                bin.Serialize(virta, muisti);
            }
            catch (SerializationException e)
            {
                System.Diagnostics.Debug.WriteLine("Failed to serialize. Reason: " + e.Message);
            }
        }

        //tätä funktiota käytetään tiedoston tuhoamiseen tietokoneen kovalevyltä
        private void tuhoaTiedosto(string tiedosto)
        {
            File.Delete(tiedosto);
            int koe = 0;
        }

        //käyttäjä haluaa luoda kirjanmerkin
        private void CreateBookmarkClick(object sender, EventArgs e)
        {

            //varmistetaan, että CADruutu on olemassa
            if (mCADTiedosto.Length != 0)
            {
                float[] siirto = {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1};
                if (mOpenGLIkkuna != null) mOpenGLIkkuna.AnnaOrientaatio(siirto);
                muisti.AsetaKirjanMerkkiOrientaatioD(siirto, mCADTiedosto);
            }
            else
            {
                float[] siirto = {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1};
                muisti.AsetaKirjanMerkkiOrientaatioD(siirto, "\0");
            }

            //varmistetaan, että TekstiRuutu on olemassa
            if (mTekstiTiedosto.Length != 0)
            {
                int[] siirto = { -1, -1, -1, -1 };
                if (mTekstiIkkuna != null) mTekstiIkkuna.AnnaOrientaatio(siirto);
                muisti.AsetaKirjanMerkkiOrientaatioT(siirto, mTekstiTiedosto);
            }
            else
            {
                int[] siirto = { -1, -1, -1, -1 };
                muisti.AsetaKirjanMerkkiOrientaatioT(siirto, "\0");
            }

            //annetaan käyttäjälle ilmoitus
            MessageBox.Show("Bookmark created. It is available next time you lauch Tasavalta.");
        }

        //käyttäjä haluaa tuhota kirjanmerkit
        private void DeleteBookmarksClick(object sender, EventArgs e)
        {
            muisti.TuhoaKirjanmerkit();
            MessageBox.Show("Bookmarks deleted. They will disappear at next startup of Tasavalta.");
        }

        //käyttäjä haluaa avata kirjanmerkin 1
        private void BM1Click(object sender, EventArgs e)
        {
            AvaaTekstiTiedosto(0);
            AvaaCADTiedosto(0);
        }

        //käyttäjä haluaa avata kirjanmerkin 2
        private void BM2Click(object sender, EventArgs e)
        {
            AvaaTekstiTiedosto(1);
            AvaaCADTiedosto(1);
        }

        //käyttäjä haluaa avata kirjanmerkin 3
        private void BM3Click(object sender, EventArgs e)
        {
            AvaaTekstiTiedosto(2);
            AvaaCADTiedosto(2);
        }

        //käyttäjä haluaa avata kirjanmerkin 4
        private void BM4Click(object sender, EventArgs e)
        {
            AvaaTekstiTiedosto(3);
            AvaaCADTiedosto(3);
        }

        //käyttäjä haluaa avata kirjanmerkin 5
        private void BM5Click(object sender, EventArgs e)
        {
            AvaaTekstiTiedosto(4);
            AvaaCADTiedosto(4);
        }

        //käyttäjä haluaa avata kirjanmerkin 6
        private void BM6Click(object sender, EventArgs e)
        {
            AvaaTekstiTiedosto(5);
            AvaaCADTiedosto(5);
        }

        //käyttäjä haluaa avata kirjanmerkin 7
        private void BM7Click(object sender, EventArgs e)
        {
            AvaaTekstiTiedosto(6);
            AvaaCADTiedosto(6);
        }

        //käyttäjä haluaa avata kirjanmerkin 8
        private void BM8Click(object sender, EventArgs e)
        {
            AvaaTekstiTiedosto(7);
            AvaaCADTiedosto(7);
        }

        //käyttäjä haluaa avata kirjanmerkin 9
        private void BM9Click(object sender, EventArgs e)
        {
            AvaaTekstiTiedosto(8);
            AvaaCADTiedosto(8);
        }

        //tätä funktiota tulee kutsua silloin, jos ainoastaan muistissa oleva CAD tiedosto halutaan
        // avata
        private bool AvaaCADTiedosto(int index)
        {

            //jos tarpeen, luodaan opengl ikkuna
            if (mOpenGLIkkuna == null)
            {
                mOpenGLIkkuna = new OpenGL();
                this.AddOwnedForm(mOpenGLIkkuna);
            }

            //katsotaan, voidaanko aloittaa
            if (mTekstiAvattu == true)
            {
                mCADAvattu = false;
            }
            else
            {
                return false;
            }

            //varmistetaan, että tarjolla on tiedosto
            Muisti.KIRJANMERKKI km = muisti.AnnaKirjanMerkki(index);
            int pituus = km.tiedostoD.Length;
            if (4 < pituus && pituus < 34)
            {

                //avataan olemassa oleva CAD-tiedosto ja asetetaan orientaatio
                mTiedosto = km.tiedostoD;
                mOpenGLIkkuna.AvaaCAD();
                mOpenGLIkkuna.AsetaOrientaatio(km.orientaatioD);
                mCADAvattu = true;
            }
            else
            {
                mCADAvattu = true;
            }

            return true;
        }

        //tätä metodia tulee kutsua silloin, jos ainoastaan muistissa oleva Teksti tiedosto halutaan
        // avata
        private bool AvaaTekstiTiedosto(int index)
        {

            //jos tarpeen, luodaan teksti ikkuna
            if (mTekstiIkkuna == null)
            {
                mTekstiIkkuna = new Teksti();
                this.AddOwnedForm(mTekstiIkkuna);
            }

            //katsotaan, voidaanko aloittaa
            if (mCADAvattu == true)
            {
                mTekstiAvattu = false;
            }
            else
            {
                return false;
            }

            //varmistetaan, että tarjolla on tiedosto
            Muisti.KIRJANMERKKI km = muisti.AnnaKirjanMerkki(index);
            int pituus = km.tiedostoT.Length;
            if (4 < pituus && pituus < 34)
            {

                //Avataan olemassa oleva HTML tiedosto halutusta kohdasta
                mTiedosto = km.tiedostoT;
                mTekstiIkkuna.AvaaTeksti();
                mTekstiIkkuna.AsetaOrientaatio(km.orientaatioT);
                mTekstiAvattu = true;
            }
            else
            {
                mTekstiAvattu = true;
            }

            return true;
        }

        //Tämä metodi avaa osoitetun CAD- tai HTML-tiedoston silloin, kun käyttäjä
        //on klikannut linkkiä CAD- tai HTML-tiedostossa
        void AvaaDokumentti(string linkki)
        {
            int index;

            //jos kyseessä on CAD tiedosto...
            if (linkki[0] == 'C') 
            {

                //jos tarpeen, luodaan opengl ikkuna
                if (mOpenGLIkkuna == null)
                {
                    mOpenGLIkkuna = new OpenGL();
                    this.AddOwnedForm(mOpenGLIkkuna);
                }

                //määritetään tiedostonimi poimimalla se # merkkien erottamasta alueesta
                index = linkki.IndexOf("#");
                mAnkkuri = linkki.Substring(index+1);
                index = mAnkkuri.IndexOf("#");
                if (index != -1) mAnkkuri = mAnkkuri.Remove(index);
                mTiedosto = mAnkkuri;
                mAnkkuri = mAnkkuri.Remove(0);
                bool onkoPiste = false;

                //tiedostonimi saattaa olla tarkenteella tai sitten ilman
                if (mTiedosto.IndexOf(".") != -1) onkoPiste = true;
                if (!onkoPiste)
                {
                    mTiedosto = mTiedosto + ".cad";
                }
                mOnko3D = true;
                mOpenGLIkkuna.AvaaCAD();
            }

            //jos kyseessä on tekstitiedosto...
            if (linkki[0] == 'T') 
            {

                //jos tarpeen, luodaan teksti ikkuna
                if (mTekstiIkkuna == null)
                {
                    mTekstiIkkuna = new Teksti();
                    this.AddOwnedForm(mTekstiIkkuna);
                }

                //määritetään tiedostonimi poimimalla se # merkkien rajaamasta alueesta
                index = linkki.IndexOf("#");
                mAnkkuri = linkki.Substring(index + 1);
                index = mAnkkuri.IndexOf("#");
                if (index != -1) mAnkkuri = mAnkkuri.Remove(index);

                //poistetaan myös mahdollinen tarkenne
                index = mAnkkuri.IndexOf(".");
                if (index != -1) mAnkkuri = mAnkkuri.Remove(index);

                mTiedosto = mAnkkuri;
                mAnkkuri = mAnkkuri.Remove(0);
            }

            //lisätään tarkenne käyttäjän kielivalinnan mukaan
            if (mOnkoEng)
            {
                mTiedosto = mTiedosto + ".Eng";
            }
            else
            {
                mTiedosto = mTiedosto + ".fin";
            }
            mOnko3D = false;

            //määritetään ankkuri käyttäjän kielivalinnan mukaan
            index = linkki.IndexOf("#");
            mAnkkuri = linkki.Substring(index + 1);
            index = mAnkkuri.IndexOf("#");
            if (index != -1) mAnkkuri = mAnkkuri.Substring(index + 1);
            if (!mOnkoEng)
            {
                index = mAnkkuri.IndexOf("#");
                if (index != -1) mAnkkuri = mAnkkuri.Substring(index + 1);
            }
            else
            {
                index = mAnkkuri.IndexOf("#");
                if (index != -1) mAnkkuri = mAnkkuri.Remove(index);
            }

            //jos saatiin ankkuri...
            if (mAnkkuri.Length != 0)
            {
                mAnkkuri = "#" + mAnkkuri;

                //alaviivat sekä + merkit pitää ankkurissa muuttaa välilyönneiksi
                mAnkkuri.Replace('_', ' ');
                mAnkkuri.Replace('+', ' ');

                //jos ankkurissa on välilyönti merkitty %20 -merkinnällä, se pitää muuttaa
                //välilyönniksi
                mAnkkuri.Replace("%20", " ");

                //liitetään ankkuri tiedostonimen perään
                mTiedosto = mTiedosto + mAnkkuri;
                mAnkkuri.Remove(0);
            }
            mTekstiIkkuna.AvaaTeksti();
        }


        //tällä metodilla huolehditaan siitä, että kun käyttäjä on avannut jonkun HTML- tai
        //CAD-tiedoston, kyseinen tiedosto ilmestyy Tasavallan päävalikkoon klikattavana 
        //nappulana, jollei se jo ole sitä
        public void TuoEsille(string nimi, char tyyppi)
        {

            //nimestä pitää erotella mahdolliset risuaidat ja tiedostopäätteet pois
            int ensimmainenHashtac = nimi.IndexOf("#");
            int toinenHashtac = nimi.IndexOf("#", ensimmainenHashtac + 1);
            string tiedosto = nimi;
            if (ensimmainenHashtac != -1)
            {
                tiedosto = nimi.Substring(ensimmainenHashtac + 1);
            }
            if (toinenHashtac != -1)
            {
                tiedosto = tiedosto.Substring(toinenHashtac - ensimmainenHashtac + 1);
            }
            int piste = tiedosto.IndexOf(".");
            if (piste != -1)
            {
                tiedosto = tiedosto.Substring(0, piste);
            }

            switch (tyyppi)
            {

                case 'T':
                    {

                        //asetetaan samanniminen Tekstitiedosto valittavaksi Muisti-tietueessa
                        for (int i = 0; i < muisti.AnnaTiedostojenMaara(); i++)
                        {
                            Muisti.TIEDOSTOTIETO tt = muisti.AnnaTiedostoTieto(i);
                            string siirto = tt.nimi;
                            if (-1 != siirto.IndexOf(tiedosto) && !tt.onko3D)
                            {
                                tt.onkoAsetettu = true;
                                tt.onkoNakyva = true;
                                muisti.AsetaTiedostoTieto(i, tt);
                            }
                        }

                        break;
                    }

                case 'C':
                    {

                        //asetetaan samanniminen CADtiedosto valittavaksi Muisti-tietueessa
                        for (int i = 0; i < muisti.AnnaTiedostojenMaara(); i++)
                        {
                            Muisti.TIEDOSTOTIETO tt = muisti.AnnaTiedostoTieto(i);
                            string siirto = tt.nimi;
                            if (-1 != siirto.IndexOf(tiedosto) && tt.onko3D)
                            {
                                tt.onkoAsetettu = true;
                                tt.onkoNakyva = true;
                                muisti.AsetaTiedostoTieto(i, tt);
                            }
                        }

                        break;
                    }
            }

        }



        //Tämä metodi käsittelee Tasavaltaan osoitetut Windows viestit
        protected override void WndProc(ref Message Msg) {

            switch (Msg.Msg) {

	            case (int)WinM.WM_LAHETALINKKI: {

                    //tämä erikoistapaus tuo RunOpenGL.dll:stä tai Teksti.dll:stä käyttäjän
                    //valitseman linkin avattavaksi
	                int kirjaimia = Math.Min(200, (int)Msg.WParam);
                    char[] linkki = new char[kirjaimia];
                    Marshal.Copy(Msg.LParam, linkki, 0, kirjaimia);
	                AvaaDokumentti(new string(linkki));
	                return;
	            }
            }

            //Erikoistapausten jälkeen käsitellään muut viestit normaalisti lähettämällä eteenpäin:
            base.WndProc(ref Msg);
        }
    }
}
