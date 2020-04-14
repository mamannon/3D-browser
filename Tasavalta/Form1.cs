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



namespace Tasavalta
{

    public partial class Form1 : Form
    {

        [DllImport("user32.dll")]
        static extern int GetSystemMetrics(int numero);
/*
        [DllImport("kernel32.dll")]
        static extern uint GetModuleFileName
            (
                [In]
                IntPtr nulli,

                [Out]
                StringBuilder puskuri,

                [In]
                [MarshalAs(UnmanagedType.U4)]
                int pituus
            );
*/
        private readonly int SM_CXSCREEN = 0;
        bool mOnkoEng = true;
        bool mOnko3D;
        string mTiedosto;
        string mMuistiTiedosto;
        string mSijaintiPolku;
        Muisti muisti;

        public bool mEiValaisua = false;
        public OpenGL mOpenGLIkkuna = new OpenGL();
        public Teksti mTekstiIkkuna = new Teksti();

        public Form1()
        {

            //ohjelman alkaessa pitää tehdä seuraavat aloitustoimet:
            //(1)säätää ikkunat ruudun resoluution mukaan,
            //(2)hakea muistitiedostosta kirjanmerkit ja tiedostotiedot sekä varmistaa niiden ajanmukaisuus
            //(3)luoda sen mukaisesti valikkoon vaihtehtoja sekä asetukset kohdilleen

            //Tämä suorittaa Form1.Designer.cs tiedoston koodin
            InitializeComponent();

            //(1): määritetään leveysSuhde, eli suhde koodauskoneen ruudun vaakaresoluution (1600) ja asennus-
            //koneen vaakaresoluution välillä. Ikkunamitat tulee aina kertoa leveysSuhteella
            float leveysSuhde = ((float)GetSystemMetrics(SM_CXSCREEN) - 100.0f) / 1500.0f;
            this.Width = (int)(1500.0f * leveysSuhde);
            this.Height = (int)(100.0f * leveysSuhde);

            //Ikkunan sijainti
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(50, 50);

            //(2): ladataan sitten kirjanmerkit ja tiedostotiedot binääritiedostosta.
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

            //(3): pääikkunaan pitää luoda tiedostotietojen mukaiset valikot. Aloitetaan
            //3D kentistä: palstat on luotava numerojärjestyksessä numerojärjestyksen aikaansaamiseksi
            hyppy:
            bool luotu = false;
            for (int i = 0; i < muisti.annaTiedostojenMaara(); i++)
            {
                Muisti.TIEDOSTOTIETO tt = muisti.annaTiedostoTieto(i);
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
                    siirto.Click += klikattuValikkoa;
                    siirto.Enabled = tt.onkoAsetettu;
                    siirto.Visible = tt.onkoNakyva;
                    N3D1.DropDownItems.Add(siirto);
                }
            }
            luotu = false;
            for (int i = 0; i < muisti.annaTiedostojenMaara(); i++)
            {
                Muisti.TIEDOSTOTIETO tt = muisti.annaTiedostoTieto(i);
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
                    siirto.Click += klikattuValikkoa;
                    siirto.Enabled = tt.onkoAsetettu;
                    siirto.Visible = tt.onkoNakyva;
                    N3D2.DropDownItems.Add(siirto);
                }
            }
            luotu = false;
            for (int i = 0; i < muisti.annaTiedostojenMaara(); i++)
            {
                Muisti.TIEDOSTOTIETO tt = muisti.annaTiedostoTieto(i);
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
                    siirto.Click += klikattuValikkoa;
                    siirto.Enabled = tt.onkoAsetettu;
                    siirto.Visible = tt.onkoNakyva;
                    N3D3.DropDownItems.Add(siirto);
                }
            }
            luotu = false;
            for (int i = 0; i < muisti.annaTiedostojenMaara(); i++)
            {
                Muisti.TIEDOSTOTIETO tt = muisti.annaTiedostoTieto(i);
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
                    siirto.Click += klikattuValikkoa;
                    siirto.Enabled = tt.onkoAsetettu;
                    siirto.Visible = tt.onkoNakyva;
                    N3D4.DropDownItems.Add(siirto);
                }
            }
            luotu = false;
            for (int i = 0; i < muisti.annaTiedostojenMaara(); i++)
            {
                Muisti.TIEDOSTOTIETO tt = muisti.annaTiedostoTieto(i);
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
                    siirto.Click += klikattuValikkoa;
                    siirto.Enabled = tt.onkoAsetettu;
                    siirto.Visible = tt.onkoNakyva;
                    N3D5.DropDownItems.Add(siirto);
                }
            }
            luotu = false;

            //sitten luodaan tiedostotietojen mukaiset valikot stooreille. Myös nämä pitää
            //luoda järjestyksessä
            for (int i = 0; i < muisti.annaTiedostojenMaara(); i++)
            {
                Muisti.TIEDOSTOTIETO tt = muisti.annaTiedostoTieto(i);
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
                    siirto.Click += klikattuValikkoa;
                    siirto.Enabled = tt.onkoAsetettu;
                    siirto.Visible = tt.onkoNakyva;
                    Stories1.DropDownItems.Add(siirto);
                }
            }
            luotu = false;
            for (int i = 0; i < muisti.annaTiedostojenMaara(); i++)
            {
                Muisti.TIEDOSTOTIETO tt = muisti.annaTiedostoTieto(i);
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
                    siirto.Click += klikattuValikkoa;
                    siirto.Enabled = tt.onkoAsetettu;
                    siirto.Visible = tt.onkoNakyva;
                    Stories2.DropDownItems.Add(siirto);
                }
            }
            luotu = false;
            for (int i = 0; i < muisti.annaTiedostojenMaara(); i++)
            {
                Muisti.TIEDOSTOTIETO tt = muisti.annaTiedostoTieto(i);
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
                    siirto.Click += klikattuValikkoa;
                    siirto.Enabled = tt.onkoAsetettu;
                    siirto.Visible = tt.onkoNakyva;
                    Stories3.DropDownItems.Add(siirto);
                }
            }
            luotu = false;
            for (int i = 0; i < muisti.annaTiedostojenMaara(); i++)
            {
                Muisti.TIEDOSTOTIETO tt = muisti.annaTiedostoTieto(i);
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
                    siirto.Click += klikattuValikkoa;
                    siirto.Enabled = tt.onkoAsetettu;
                    siirto.Visible = tt.onkoNakyva;
                    Stories4.DropDownItems.Add(siirto);
                }
            }
            luotu = false;
            for (int i = 0; i < muisti.annaTiedostojenMaara(); i++)
            {
                Muisti.TIEDOSTOTIETO tt = muisti.annaTiedostoTieto(i);
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
                    siirto.Click += klikattuValikkoa;
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
            siirto1.Click += klikattuValikkoa;
            siirto1.Enabled = true;
            siirto1.Visible = true;
            Apua.DropDownItems.Add(siirto1);

            //loppujen lopuksi pitää vielä asettaa kirjanmerkkien näkymättömyydet kohdilleen
            toolStripMenuItem1.Visible = muisti.onkoKirjanMerkkiAsetettu(0);
            toolStripMenuItem2.Visible = muisti.onkoKirjanMerkkiAsetettu(1);
            toolStripMenuItem3.Visible = muisti.onkoKirjanMerkkiAsetettu(2);
            toolStripMenuItem4.Visible = muisti.onkoKirjanMerkkiAsetettu(3);
            toolStripMenuItem5.Visible = muisti.onkoKirjanMerkkiAsetettu(4);
            toolStripMenuItem6.Visible = muisti.onkoKirjanMerkkiAsetettu(5);
            toolStripMenuItem7.Visible = muisti.onkoKirjanMerkkiAsetettu(6);
            toolStripMenuItem8.Visible = muisti.onkoKirjanMerkkiAsetettu(7);
            toolStripMenuItem9.Visible = muisti.onkoKirjanMerkkiAsetettu(8);

        }

        //kun käyttäjä klikkaa jotain dynaamisesti luotua valikkokohtaa, siirtyy
        //klikkitapahtuma tämän funktion käsiteltäväksi
        private void klikattuValikkoa(object sender, EventArgs e)
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
//                mOpenGLIkkuna.avaaCAD(this);
            }
            else
            {

                //muuten kyseessä on tekstikenttä
                mOnko3D = false;
                if (mOnkoEng)
                {
                    mTiedosto = siirto2 + ".eng";
                }
                else
                {
                    mTiedosto = siirto2 + ".fin";
                }
 //               mTekstiIkkuna.avaaTeksti(this);
            }
        }

        //tämä funktio muodostaa tiedoston sisällöstä Muisti luokan, jos se on mahdollista
        bool lueJaTarkasta(FileStream virta, ref Muisti m)
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
            //TODO
 //           openGLIkkuna->Close();
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
            /*
            //varmistetaan, että CADruutu on olemassa
            if (!CADTiedosto.empty())
            {
                float[] siirto = {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1};
                openGLIkkuna->annaOrientaatio(siirto);
                asetaKirjanMerkkiOrientaatioD(siirto, CADTiedosto);
            }
            else
            {
                float[] siirto = {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1};
                asetaKirjanMerkkiOrientaatioD(siirto, L"\0");
            }

            //varmistetaan, että TekstiRuutu on olemassa
            if (!tekstiTiedosto.empty())
            {
                int[] siirto = { -1, -1, -1, -1 };
                tekstiIkkuna->annaOrientaatio(siirto);
                asetaKirjanMerkkiOrientaatioT(siirto, tekstiTiedosto);
            }
            else
            {
                int[] siirto = { -1, -1, -1, -1 };
                asetaKirjanMerkkiOrientaatioT(siirto, L"\0");
            }
            */
            //annetaan käyttäjälle ilmoitus
            MessageBox.Show("Bookmark created. It is available next time you lauch Tasavalta.");
        }
        
        //käyttäjä haluaa tuhota kirjanmerkit
        private void DeleteBookmarksClick(object sender, EventArgs e)
        {
            /*
            muisti.kirjanMerkki[0].onkoAsetettu = false;
            muisti.kirjanMerkki[1].onkoAsetettu = false;
            muisti.kirjanMerkki[2].onkoAsetettu = false;
            muisti.kirjanMerkki[3].onkoAsetettu = false;
            muisti.kirjanMerkki[4].onkoAsetettu = false;
            muisti.kirjanMerkki[5].onkoAsetettu = false;
            muisti.kirjanMerkki[6].onkoAsetettu = false;
            muisti.kirjanMerkki[7].onkoAsetettu = false;
            muisti.kirjanMerkki[8].onkoAsetettu = false;
            mIndex = -1;
            */
            MessageBox.Show("Bookmarks deleted. They will disappear at next startup of Tasavalta.");
        }

        //käyttäjä haluaa avata kirjanmerkin 1
        private void BM1Click(object sender, EventArgs e)
        {
            avaaTekstiTiedosto(0);
            avaaCADTiedosto(0);
        }

        //käyttäjä haluaa avata kirjanmerkin 2
        private void BM2Click(object sender, EventArgs e)
        {
            avaaTekstiTiedosto(1);
            avaaCADTiedosto(1);
        }

        //käyttäjä haluaa avata kirjanmerkin 3
        private void BM3Click(object sender, EventArgs e)
        {
            avaaTekstiTiedosto(2);
            avaaCADTiedosto(2);
        }

        //käyttäjä haluaa avata kirjanmerkin 4
        private void BM4Click(object sender, EventArgs e)
        {
            avaaTekstiTiedosto(3);
            avaaCADTiedosto(3);
        }

        //käyttäjä haluaa avata kirjanmerkin 5
        private void BM5Click(object sender, EventArgs e)
        {
            avaaTekstiTiedosto(4);
            avaaCADTiedosto(4);
        }

        //käyttäjä haluaa avata kirjanmerkin 6
        private void BM6Click(object sender, EventArgs e)
        {
            avaaTekstiTiedosto(5);
            avaaCADTiedosto(5);
        }

        //käyttäjä haluaa avata kirjanmerkin 7
        private void BM7Click(object sender, EventArgs e)
        {
            avaaTekstiTiedosto(6);
            avaaCADTiedosto(6);
        }

        //käyttäjä haluaa avata kirjanmerkin 8
        private void BM8Click(object sender, EventArgs e)
        {
            avaaTekstiTiedosto(7);
            avaaCADTiedosto(7);
        }

        //käyttäjä haluaa avata kirjanmerkin 9
        private void BM9Click(object sender, EventArgs e)
        {
            avaaTekstiTiedosto(8);
            avaaCADTiedosto(8);
        }

        //tätä funktiota tulee kutsua silloin, jos ainoastaan muistissa oleva CAD tiedosto halutaan
        // avata
        bool avaaCADTiedosto(int index)
        {
/*
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
            int pituus = wcslen(muisti.kirjanMerkki[index].tiedostoD);
            if (4 < pituus && pituus < 34)
            {
                wcscpy(tiedosto, muisti.kirjanMerkki[index].tiedostoD);
                openGLIkkuna->avaaCad(this);
                openGLIkkuna->asetaOrientaatio(muisti.kirjanMerkki[index].orientaatioD);
                mCADAvattu = true;
            }
            else
            {
                mCADAvattu = true;
            }
            */
            return true;
        }

        //tätä metodia tulee kutsua silloin, jos ainoastaan muistissa oleva Teksti tiedosto halutaan
        // avata
        bool avaaTekstiTiedosto(int index)
        {
            /*
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
            int pituus = wcslen(muisti.kirjanMerkki[index].tiedostoT);
            if (4 < pituus && pituus < 34)
            {
                wcscpy(tiedosto, muisti.kirjanMerkki[index].tiedostoT);
                tekstiIkkuna->avaaTeksti(this);
                tekstiIkkuna->asetaOrientaatio(muisti.kirjanMerkki[index].orientaatioT);
                mTekstiAvattu = true;
            }
            else
            {
                mTekstiAvattu = true;
            }
            */
            return true;
        }

    }
}
