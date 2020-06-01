package com.mannonen.mikael.example.tasavalta;

import android.content.SharedPreferences;
import android.content.res.Configuration;
import android.content.res.Resources;
import android.graphics.Rect;
import android.os.Build;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentTransaction;
import android.util.DisplayMetrics;
import android.util.TypedValue;
import android.view.LayoutInflater;
import android.view.MenuItem;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

import static android.content.Context.MODE_PRIVATE;
import static com.mannonen.mikael.example.tasavalta.Singleton.annaIlmentyma;

public class TekstiRuutu extends Fragment {

    public native void annaKorkeusLeveys();
    public native boolean lataaTiedosto(byte[] tiedosto);
    public native boolean OnMouseEvent(int x, int y, boolean vasen, boolean oikea);
    public native boolean HistoryBack();
    public native boolean HistoryForward();
    public native boolean HistoryCanBack();
    public native boolean HistoryCanForward();
    public native void siirryAnkkuriin(String otsikko);
    public native void lataaUudestaan();
    public native void piirra();
    public native void tulostetaan(float dpi);
    public native void sammutaTeksti();

    public int siirto3, siirto4, siirto5, siirto6, siirto7, siirto8;
    public String otsikko;
    public int taustaVari;
    public CanvasView mCan;

    private View mNakyma;
    private Singleton mSing;
    private boolean mPalautaSijainti;
    private int mOnkoVaaka;

    static boolean onkoKirjasto2Ladattu=true;

    class TaakseTeksti implements View.OnClickListener {

        @Override
        public void onClick(View view) { taakseClick(); }
    }

    class EteenTeksti implements View.OnClickListener {

        @Override
        public void onClick(View view) {
            eteenClick();
        }
    }

    class OtsikotTeksti implements View.OnClickListener {

        @Override
        public void onClick(View view) {
            if (mSing.mAktiviteetti1.mRenki.mOtsikkoLista!=null) {

                //avataan dialogifragmentti, jossa käyttäjä voi klikata väliotsikkoa
                FragmentTransaction ft=mSing.mAktiviteetti1.mFragmentManager.beginTransaction();
                Fragment prev=mSing.mAktiviteetti1.mFragmentManager.
                        findFragmentByTag("Otsikkolista");
                if (prev!=null) {
                    ft.remove(prev);
                }

                //Jostain syystä Tasavalta kaatuu silloin tällöin, kun alla olevat koodirivit
                //suoritetaan. Try-catch estää mahdollisen kaatumisen. Ei pitäisi kaatua enää...
                try {
                    ft.commitNow();
                    OtsikkoLista l=new OtsikkoLista();
                    l.show(mSing.mAktiviteetti1.mFragmentManager, "Otsikkolista");
                } catch (Exception e) {
                    Toast toast=Toast.makeText(mSing.mAktiviteetti1, "Operation failed.",
                            Toast.LENGTH_LONG);
                    toast.show();
                }
            }
        }
    }

    // Used to load the 'native-lib' library on fragment startup.
    static {
        try {
            System.loadLibrary("Kirjasto2");
        } catch (Exception e) {
            onkoKirjasto2Ladattu=false;
        }
        int koe=0;
    }

    @Override
    public void onCreate(Bundle savedInstanceState) {

        super.onCreate(savedInstanceState);

        //pyydetään Singleton luokan ilmentymä ja ilmoittaudutaan
        mSing=annaIlmentyma();
        mSing.mAktiviteetti1.mFragmentti2=this;

        //Varmistetaan, että Kirjasto2 saatiin ladattua. Jollei, ilmoitetaan
        //asiasta käyttäjälle
        if (!onkoKirjasto2Ladattu) {
            mSing.mAktiviteetti1.Kirjasto2Epaonnistui();
        }
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup parent, Bundle savedInstanceState) {

        //tämä siltä varalta, että Kirjasto2 puuttuu laitteesta
        if (!onkoKirjasto2Ladattu) {
            mSing.mAktiviteetti1.mFragmentti2=null;
            return null;
        }

        // xml tiedostosta luettavat jutut muodostavat View-luokan ilmentymän
        return inflater.inflate(R.layout.fragment_tekstiruutu, parent, false);
    }

    @Override
    public void onViewCreated(View view, Bundle savedInstanceState) {

        //Kytketään menupalkin Text Viewer -MenuItem käyttöön
        MenuItem mi = mSing.mAktiviteetti1.mMenu.findItem(R.id.tekstiruutu);
        mi.setEnabled(true);

        //jos tämä Android-laite käyttää android versiota KitKat (API 19) tai uudempaa,
        //mahdolistetaan tulostaminen
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.KITKAT) {
            mi = mSing.mAktiviteetti1.mMenu.findItem(R.id.tulostaTeksti);
            mi.setEnabled(true);
        }

        //Otetaan talteen CanvasView ilmentymä
        mCan = view.findViewById(R.id.kanvaasiTeksti);

        //Tallennetaan view-luokan jäsenmuuttuja
        mNakyma = view;

        //siirretään orientaatiotieto Kirjasto2:n luettavaksi
        mOnkoVaaka = mSing.mAktiviteetti1.mVaakaKerroin;

        //Asetetaan kuuntelijat
        Button siirto = view.findViewById(R.id.taakseTeksti);
        siirto.setOnClickListener(new TaakseTeksti());
        siirto = view.findViewById(R.id.eteenTeksti);
        siirto.setOnClickListener(new EteenTeksti());
        siirto = view.findViewById(R.id.otsikotTeksti);
        siirto.setOnClickListener(new OtsikotTeksti());

        //onViewCreated metodiin voidaan päätyä kahdesta syystä:
        if (mSing.mAktiviteetti1.mOnkoTekstiAvattu) {

            //ladataan edellisen ilmentymän data
            lataaAsetukset();

            //joko TekstiRuutu pitää konstruoida uudelleen...
            if (mOnkoVaaka==mSing.mAktiviteetti1.mVaakaKerroin) {
                mPalautaSijainti = false;
            } else {
                mPalautaSijainti = true;
            }
            avaaUudelleen();
        } else {

            //...tai sitten TekstiRuutu avataan ensimmäistä kertaa
            mPalautaSijainti=false;
            mOnkoVaaka=mSing.mAktiviteetti1.mVaakaKerroin;
            avaaTeksti();
        }
    }

    public void tallennaAsetukset() {
        SharedPreferences sp=mSing.mAktiviteetti1.getPreferences(MODE_PRIVATE);
        SharedPreferences.Editor editor=sp.edit();
        editor.putInt("siirto3", siirto3);
        editor.putInt("siirto4", siirto4);
        editor.putInt("siirto5", siirto5);
        editor.putInt("siirto6", siirto6);
        Button taakse=mNakyma.findViewById(R.id.taakseTeksti);
        Button eteen=mNakyma.findViewById(R.id.eteenTeksti);
        editor.putBoolean("taakse", taakse.isEnabled());
        editor.putBoolean("eteen", eteen.isEnabled());
        editor.commit();
    }

    private void lataaAsetukset() {
        SharedPreferences sp=mSing.mAktiviteetti1.getPreferences(MODE_PRIVATE);
        siirto3=sp.getInt("siirto3", 0);
        siirto4=sp.getInt("siirto4", 0);
        siirto5=sp.getInt("siirto5", 0);
        siirto6=sp.getInt("siirto6", 0);
        Button taakse=mNakyma.findViewById(R.id.taakseTeksti);
        Button eteen=mNakyma.findViewById(R.id.eteenTeksti);
        taakse.setEnabled(sp.getBoolean("taakse", false));
        eteen.setEnabled(sp.getBoolean("eteen", false));
    }

    @Override
    public void onConfigurationChanged(Configuration newConfig) {
        mSing.mAktiviteetti1.mFragmentti2.mCan.mHandler.removeCallbacksAndMessages(null);
        super.onConfigurationChanged(newConfig);
        mPalautaSijainti=true;
        uudelleen(mNakyma);
    }

    public View annaNakyma() {
        return mNakyma;
    }

    public void avaaTeksti() {

        //varmistetaan, että teksti-ikkuna on esillä ja Kirjasto2 saatavilla
        if (mNakyma.isShown() && onkoKirjasto2Ladattu) {
            mSing.mAktiviteetti1.mFragmentti2.mCan.mHandler.removeCallbacksAndMessages(null);
            avataan(mNakyma);

            //merkitään muistiin, että avattu teksti on suoraan saatavilla päävalikosta
            mSing.mAktiviteetti1.mFragmentti1.tuoEsille(mSing.mAktiviteetti1.mTekstiTiedosto,
                    'T');
        }

    }

    public void avaaUudelleen() {

        //varmistetaan, että teksti-ikkuna on esillä ja Kirjasto2 saatavilla
        if (mNakyma.isShown() && onkoKirjasto2Ladattu) {
            mSing.mAktiviteetti1.mFragmentti2.mCan.mHandler.removeCallbacksAndMessages(null);
            uudelleen(mNakyma);
        }
    }

    //joskus jo avattu HTML-dokumentti pitää muotoilla uudelleen
    private void uudelleen(View view) {
        float siirto03=0;
        float siirto05=1;
        float siirto04=0;
        float siirto06=1;

        if (mPalautaSijainti) {

            //jos edellisen ilmentymän scrollien sijainti pitää palauttaa...
            siirto03=siirto3;
            siirto05=siirto5;
            siirto04=siirto4;
            siirto06=siirto6;
        }

        //näkymän leveys
        if (mSing.mAktiviteetti1.mVaakaKerroin==1){
            siirto7=mSing.mAktiviteetti1.mPoint.x/mSing.mAktiviteetti1.mVaakaKerroin;
        } else {
            siirto7=mSing.mAktiviteetti1.mPoint.y/mSing.mAktiviteetti1.mVaakaKerroin;
        }

        //näkymän korkeus
        if (mSing.mAktiviteetti1.mVaakaKerroin==1){
            siirto8 = mSing.mAktiviteetti1.mPoint.y - annaActionbarKorkeus() -
                    annaStatusbarKorkeus()-annaNappiRiviKorkeus();
        } else {
            siirto8 = mSing.mAktiviteetti1.mPoint.x - annaActionbarKorkeus() -
                    annaStatusbarKorkeus()-annaNappiRiviKorkeus();
        }

        //Ensiksi Kirjasto2:lle pitää kertoa ikkunan mitat
        annaKorkeusLeveys();

        //jos vanha sijainti pitää (likimääräisesti) palauttaa...
        if (mPalautaSijainti && mOnkoVaaka!=mSing.mAktiviteetti1.mVaakaKerroin) {
            if (mOnkoVaaka==2) {
                siirto3 = (int) (siirto03 / siirto05 * 1 * siirto5);
                siirto4 = (int) (siirto04 / siirto06 * 1 * siirto6);
            } else {
                siirto3 = (int) (siirto03 / siirto05 / 1 * siirto5);
                siirto4 = (int) (siirto04 / siirto06 / 1 * siirto6);
            }
            mOnkoVaaka=mSing.mAktiviteetti1.mVaakaKerroin;
            annaKorkeusLeveys();
        }

        //Sitten muotoillaan HTML sivu uudelleen
        lataaUudestaan();

        //Asetetaan haluttu taustaväri ja sivun otsikko
        mCan.kokoTausta(taustaVari);
        if (otsikko!=null) {
            TextView siirto=view.findViewById(R.id.otsikkoTeksti);
            siirto.setText(otsikko);
        }

        //Pyydetään Kirjasto2:ta päivittämään näkymä
        piirra();
        mSing.mAktiviteetti1.mFragmentti2.mCan.invalidate();

        //Lopuksi pitää vielä tarkistaa, voiko klikata eteen tai taakse nappia
        Button taakse=mNakyma.findViewById(R.id.taakseTeksti);
        Button eteen=mNakyma.findViewById(R.id.eteenTeksti);
        if (HistoryCanBack())
            taakse.setEnabled(true);
        else
            taakse.setEnabled(false);
        if (HistoryCanForward())
            eteen.setEnabled(true);
        else
            eteen.setEnabled(false);
    }

    //tällä funktiolla ladataan HTML-dokumentti
    private void avataan(View view) {

        //vaakaskrollin sijanti näkymän vasemmalla reunalla
        siirto3=0;

        //vaakaskrollin kokonaisliikkuma-alue on toistaiseksi sama kuin näkymän leveys
        if (mSing.mAktiviteetti1.mVaakaKerroin==1){
            siirto5=0;
            siirto7=mSing.mAktiviteetti1.mPoint.x/mSing.mAktiviteetti1.mVaakaKerroin;
        } else {
            siirto5=0;
            siirto7=mSing.mAktiviteetti1.mPoint.y/mSing.mAktiviteetti1.mVaakaKerroin;
        }

        //pystyskrollin sijainti näkymän yläreunassa
        siirto4=0;

        //pystyskrollin kokonaisliikkuma-alue on toistaiseksi sama kuin näkymän korkeus
        if (mSing.mAktiviteetti1.mVaakaKerroin==1){
            siirto6=0;
            siirto8 = mSing.mAktiviteetti1.mPoint.y - annaActionbarKorkeus()
                    - annaStatusbarKorkeus()-annaNappiRiviKorkeus();
        } else {
            siirto6=0;
            siirto8 = mSing.mAktiviteetti1.mPoint.x - annaActionbarKorkeus()
                    - annaStatusbarKorkeus()-annaNappiRiviKorkeus();
        }

        //Ensiksi Kirjasto2:lle pitää kertoa ikkunan mitat
        annaKorkeusLeveys();

        //Sitten ladataan haluttu HTML sivu ja samalla Kirjasto2 kertoo ladattavan sivun mitat
        if (lataaTiedosto(mSing.mAktiviteetti1.mTekstiTiedosto.getBytes())) {

            //Koska HTML-dokumentin avaus onnistui, merkitään se muistiin
            mSing.mAktiviteetti1.mOnkoTekstiAvattu=true;

            //varmistetaan tekstitiedoston nimi
            mSing.mAktiviteetti1.asetaTekstiVarmistus(mSing.mAktiviteetti1.mTekstiTiedosto);

            //Asetetaan haluttu taustaväri ja sivun otsikko
            mCan.kokoTausta(taustaVari);
            if (otsikko!=null) {
                TextView siirto=view.findViewById(R.id.otsikkoTeksti);
                siirto.setText(otsikko);
            }

            //Pyydetään Kirjasto2:ta päivittämään näkymä
            piirra();
            mSing.mAktiviteetti1.mFragmentti2.mCan.invalidate();

            //Lopuksi pitää vielä tarkistaa, voiko klikata eteen tai taakse nappia
            Button taakse=view.findViewById(R.id.taakseTeksti);
            Button eteen=view.findViewById(R.id.eteenTeksti);
            if (HistoryCanBack()) {
                taakse.setEnabled(true);
            } else {
                taakse.setEnabled(false);
            }
            if (HistoryCanForward()) {
                eteen.setEnabled(true);
            } else {
                eteen.setEnabled(false);
            }

        } else {

            //epäonnistuessa pitää korvata viallinen nimi jollain, joka ei ole viallinen
            mSing.mAktiviteetti1.mTekstiTiedosto=mSing.mAktiviteetti1.annaTekstiVarmistus();

            //jos HTML-sivun avaaminen epäonnistui, annetaan käyttäjälle ilmoitus
            Toast toast=Toast.makeText(getContext(), "Unable to open file.",
                    Toast.LENGTH_SHORT);
            toast.show();
        }
    }

    public void taakseClick() {

        //Mahdollinen skrollaus on pysäytettävä
        mSing.mAktiviteetti1.mFragmentti2.mCan.mHandler.removeCallbacksAndMessages(null);

        //Taakse nappia on klikattu. Pyydetään Kirjasto2:ta avaamaan aiempi HTML näkymä
        if (!HistoryBack()) {

            //jos HTML-sivun avaaminen epäonnistui, annetaan käyttäjälle ilmoitus
            Toast toast=Toast.makeText(getContext(), "Unable to go back.", Toast.LENGTH_SHORT);
            toast.show();
            return;
        }

        //Asetetaan haluttu taustaväri ja sivun otsikko
        mCan.kokoTausta(taustaVari);
        if (otsikko!=null) {
            TextView siirto=mNakyma.findViewById(R.id.otsikkoTeksti);
            siirto.setText(otsikko);
        }

        //Päivitetään näkymä varmuuden varalta
        mSing.mAktiviteetti1.mFragmentti2.mCan.invalidate();

        //Lopuksi pitää vielä tarkistaa, voiko klikata eteen tai taakse nappia
        Button taakse=mNakyma.findViewById(R.id.taakseTeksti);
        Button eteen=mNakyma.findViewById(R.id.eteenTeksti);
        if (HistoryCanBack())
            taakse.setEnabled(true);
        else
            taakse.setEnabled(false);
        if (HistoryCanForward())
            eteen.setEnabled(true);
        else
            eteen.setEnabled(false);
    }

    private void eteenClick() {

        //Mahdollinen skrollaus on pysäytettävä
        mSing.mAktiviteetti1.mFragmentti2.mCan.mHandler.removeCallbacksAndMessages(null);

        //Eteen nappia on klikattu. Pyydetään Kirjasto2:ta avaamaan aiempi HTML näkymä
        if (!HistoryForward()) {

            //jos HTML-sivun avaaminen epäonnistui, annetaan käyttäjälle ilmoitus
            Toast toast=Toast.makeText(getContext(), "Unable to go forward.",
                    Toast.LENGTH_SHORT);
            toast.show();
            return;
        }

        //Asetetaan haluttu taustaväri ja sivun otsikko
        mCan.kokoTausta(taustaVari);
        if (otsikko!=null) {
            TextView siirto=mNakyma.findViewById(R.id.otsikkoTeksti);
            siirto.setText(otsikko);
        }

        //Päivitetään näkymä varmuuden varalta
        mSing.mAktiviteetti1.mFragmentti2.mCan.invalidate();

        //Lopuksi pitää vielä tarkistaa, voiko klikata eteen tai taakse nappia
        Button taakse=mNakyma.findViewById(R.id.taakseTeksti);
        Button eteen=mNakyma.findViewById(R.id.eteenTeksti);
        if (HistoryCanBack())
            taakse.setEnabled(true);
        else
            taakse.setEnabled(false);
        if (HistoryCanForward())
            eteen.setEnabled(true);
        else
            eteen.setEnabled(false);
    }

    public void muutaKokoa(int leveys, int korkeus) {

        //näkymän leveys
        siirto7=leveys;

        //näkymän korkeus
        siirto8 = korkeus;

        //Sitten voimme kertoa Kirjasto2:lle uuden ikkunan mitat
        annaKorkeusLeveys();

        //Pyydetään Kirjasto2:ta päivittämään näkymä
        piirra();
        mSing.mAktiviteetti1.mFragmentti2.mCan.invalidate();
    }

    public void rullaus(int vaaka, int pysty) {

        boolean lopeta1=false;
        boolean lopeta2=false;

        //vaakaskrollin sijanti näkymän vasemmalla reunalla HUOM: lisätään vaakarullaus
        siirto3=siirto3+vaaka*(int) mSing.mAktiviteetti1.mDm.density;

        //näkymän leveys
        if (mSing.mAktiviteetti1.mVaakaKerroin==1){
            siirto7=mSing.mAktiviteetti1.mPoint.x/mSing.mAktiviteetti1.mVaakaKerroin;
        } else {
            siirto7=mSing.mAktiviteetti1.mPoint.y/mSing.mAktiviteetti1.mVaakaKerroin;
        }

        //varmistetaan, ettei rullata kokonaisliikkuma-alueen ulkopuolelle
        if (siirto5<siirto3+siirto7)
        {
            siirto3=siirto5-(siirto7);
            mSing.mAktiviteetti1.mFragmentti2.mCan.piirraScrolliMerkit=false;
            lopeta1=true;
        }
        if (siirto3<0)
        {
            siirto3=0;
            mSing.mAktiviteetti1.mFragmentti2.mCan.piirraScrolliMerkit=false;
            lopeta1=true;
        }

        //pystyskrollin sijainti näkymän yläreunassa HUOM! lisätään pystyrullaus
        siirto4=siirto4+pysty*(int) mSing.mAktiviteetti1.mDm.density;

        //näkymän korkeus
        if (mSing.mAktiviteetti1.mVaakaKerroin==1) {
            siirto8 = mSing.mAktiviteetti1.mPoint.y - annaActionbarKorkeus()
                    - annaStatusbarKorkeus() - annaNappiRiviKorkeus();
        } else {
            siirto8 = mSing.mAktiviteetti1.mPoint.x - annaActionbarKorkeus()
                    - annaStatusbarKorkeus() - annaNappiRiviKorkeus();
        }

        //varmistetaan, ettei rullata kokonaisliikkuma-alueen ulkopuolelle
        if (siirto6<siirto4+siirto8-mSing.mAktiviteetti1.mRenki.mHeight)
        {
            siirto4=siirto6-(siirto8-mSing.mAktiviteetti1.mRenki.mHeight);
            mSing.mAktiviteetti1.mFragmentti2.mCan.piirraScrolliMerkit=false;
            lopeta2=true;
        }
        if (siirto4<0)
        {
            siirto4=0;
            mSing.mAktiviteetti1.mFragmentti2.mCan.piirraScrolliMerkit=false;
            lopeta2=true;
        }

        //jos scrolli on jumissa joko vaaka- tai pystysuunnassa, estetään skrollaaminen jatkossa
        if (lopeta1 || lopeta2) {
            mCan.lopetaPaivitys();
        }

        //Sitten voimme kertoa Kirjasto2:lle uuden ikkunan mitat
        annaKorkeusLeveys();

        //Pyydetään Kirjasto2:ta päivittämään näkymä
        piirra();
        mSing.mAktiviteetti1.mFragmentti2.mCan.invalidate();
    }

    public void klikkaus(int X, int Y) {

        //Ilmoitetaan klikkauksesta Kirjasto2:lle
        boolean vastaus=OnMouseEvent(siirto3+X, siirto4+Y, true, false);

        TextView siirto = mNakyma.findViewById(R.id.otsikkoTeksti);
        if (vastaus && siirto!=null) {

            //Asetetaan haluttu taustaväri ja sivun otsikko
            mCan.kokoTausta(taustaVari);
            if (otsikko != null) {
                siirto.setText(otsikko);
            }

            //Pyydetään Kirjasto2:ta päivittämään näkymä
            piirra();
            mSing.mAktiviteetti1.mFragmentti2.mCan.invalidate();

            //Lopuksi pitää vielä tarkistaa, voiko klikata eteen tai taakse nappia
            Button taakse = mNakyma.findViewById(R.id.taakseTeksti);
            Button eteen = mNakyma.findViewById(R.id.eteenTeksti);
            if (HistoryCanBack())
                taakse.setEnabled(true);
            else
                taakse.setEnabled(false);
            if (HistoryCanForward())
                eteen.setEnabled(true);
            else
                eteen.setEnabled(false);
        }
    }

    private int annaActionbarKorkeus() {
        TypedValue tv=new TypedValue();
        Resources.Theme teema=mSing.mAktiviteetti1.mTeema;
        DisplayMetrics metrics=mSing.mAktiviteetti1.mDm;
        if (teema.resolveAttribute(android.R.attr.actionBarSize, tv, true)) {
            return TypedValue.complexToDimensionPixelOffset(tv.data, metrics);
        } else {
            return 0;
        }
    }

    private int annaStatusbarKorkeus() {
        Rect rect=new Rect();
        mSing.mAktiviteetti1.mWin.getDecorView().getWindowVisibleDisplayFrame(rect);
        return rect.top;
    }

    private int annaNappiRiviKorkeus() {
        return mSing.mAktiviteetti1.mNappiKorkeus;
    }

}
