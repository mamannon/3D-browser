package com.mannonen.mikael.example.tasavalta;


import android.content.Intent;
import android.content.res.Configuration;
import android.content.res.Resources;
import android.graphics.Bitmap;
import android.graphics.Rect;
import android.net.Uri;
import android.os.Bundle;
import android.os.Handler;
import androidx.fragment.app.Fragment;
import androidx.fragment.app.FragmentTransaction;
import androidx.core.content.FileProvider;
import android.util.DisplayMetrics;
import android.util.TypedValue;
import android.view.LayoutInflater;
import android.view.MenuItem;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.FrameLayout;
import android.widget.Toast;

import java.io.File;
import java.io.FileOutputStream;
import java.util.ArrayList;

import static com.mannonen.mikael.example.tasavalta.Singleton.annaIlmentyma;

public class CADRuutu extends Fragment {

    private Handler mHandler;
    private View mNakyma;
    private Singleton mSing;
    private FrameLayout mKehys;
    private int mLeveys, mKorkeus;
    //    private ValintaLista mValintaLista;
    private Integer mJuoksevaNumero=1;
    private boolean eiValaisua;
    private boolean siirto1, siirto2;
    private int siirto3, siirto4, siirto5;
    private Bitmap bitmap;
    private int valintoja1;
    private int valintoja2;
    private boolean ajetaan;
    private int animaatioTila;
    private boolean eteen, taakse;
    private String polku;
    private String otsikko;
    public ArrayList<Boolean> valintaListaB1;
    public ArrayList<Boolean> valintaListaB2;
    public ArrayList<String> valintaListaC1;
    public ArrayList<String> valintaListaC2;

    private native boolean kaynnistaCAD(byte[] tiedosto, int leveys, int korkeus);
    private native void taakse();
    private native void eteen();
    private native void eloKuva(int valinta);
    public native void tulosta(boolean tausta);
    public native void vieValinnat();
    public native int onkoLaajennuksia(int optio);
    public native void muutaAla(int leveys, int korkeus);
    public native void hiiriLiikkuuVasen(int x1, int y1, int x2, int y2);
    public native void hiiriLiikkuuOikea(int x, int y);
    public native void hiiriLiikkuu(int x, int y);
    public native void pyoraYlos();
    public native void pyoraAlas();
    public native void hiiriPoistuu();
    public native void hiiriLiikkuuMolemmat(int x1, int y1, int x2, int y2);
    public native boolean paivita();
    public native void sammutaCAD();
    public native void annaOrientaatio1(float[] orientaatio);
    public native void asetaOrientaatio1(float[] orientaatio);
    public native void annaTilanne();

    static boolean onkoKirjasto1Ladattu=true;
    static boolean openglOnkoTuettu=false;

    class TaakseCAD implements View.OnClickListener {

        @Override
        public void onClick(View view) { taakseClick(); }
    }

    class EteenCAD implements View.OnClickListener {

        @Override
        public void onClick(View view) {
            eteenClick();
        }
    }

    class ValintaListaCAD implements View.OnClickListener {

        @Override
        public void onClick(View view) { valintaListaClick(); }
    }

    class KaynnistaCAD implements View.OnClickListener {

        @Override
        public void onClick(View view) { kaynnistaClick();}
    }

    class PysaytaCAD implements View.OnClickListener {

        @Override
        public void onClick(View view) { pysaytaClick(); }
    }

    // Used to load the 'native-lib' library on fragment startup
    static {
        try {
            System.loadLibrary("Kirjasto1");
        } catch (UnsatisfiedLinkError e) {
            onkoKirjasto1Ladattu=false;
        }
    }

    private void tilannePaivitys() {

        //ensiksi pyydetään Kirjasto2:ta antamaan ajantasaiset tiedot
        annaTilanne();

        //sitten sijoitetaan tiedot asianomaisiin muuttujiin
        View v1=mNakyma.findViewById(R.id.eteenCAD);
        v1.setEnabled(eteen);
        v1=mNakyma.findViewById(R.id.taakseCAD);
        v1.setEnabled(taakse);
        v1=mNakyma.findViewById(R.id.kaynnistaCAD);
        Button v2=mNakyma.findViewById(R.id.pysaytaCAD);
        if  (animaatioTila==0) {
            v1.setEnabled(true);
            v2.setEnabled(false);
            v2.setText(R.string.pysayta);
        } else if (animaatioTila==1) {
            v1.setEnabled(false);
            v2.setEnabled(true);
            v2.setText(R.string.pysayta);
        } else if (animaatioTila==2) {
            v1.setEnabled(true);
            v2.setEnabled(true);
            v2.setText(R.string.alkuun);
        } else {
            v1.setEnabled(false);
            v2.setEnabled(false);
            v2.setText(R.string.pysayta);
        }
    }

    @Override
    public void onConfigurationChanged(Configuration newConfig) {
        super.onConfigurationChanged(newConfig);

        //määritetään OpenGL näkymän likimääräinen koko. Tämän jälkeen saatetaan määrittää
        //täsmällinen koko
        if (newConfig.orientation==Configuration.ORIENTATION_LANDSCAPE) {
            mLeveys=mSing.mAktiviteetti1.mPoint.y/2;
            mKorkeus=mSing.mAktiviteetti1.mPoint.x - annaActionbarKorkeus()
                    - annaStatusbarKorkeus()-annaNappiRiviKorkeus();
        } else {
            mLeveys=mSing.mAktiviteetti1.mPoint.x;
            mKorkeus=mSing.mAktiviteetti1.mPoint.y - annaActionbarKorkeus()
                    - annaStatusbarKorkeus()-annaNappiRiviKorkeus();
        }
        muutaAla(mLeveys, mKorkeus);
    }

    /**
     * Use this factory method to create a new instance of
     * this fragment using the provided parameters.
     */
    public static CADRuutu newInstance() {
        CADRuutu fragment = new CADRuutu();
        fragment.setRetainInstance(true);
        return fragment;
    }

    public CADRuutu() {
        // Do nothing
    }

    @Override
    public void onCreate(Bundle savedInstanceState) {

        super.onCreate(savedInstanceState);

        //pyydetään singelton luokan ilmentymä ja ilmoittaudutaan
        mSing=annaIlmentyma();
        mSing.mAktiviteetti1.mFragmentti3=this;

        //selvitetään valaisupolitiikka
        eiValaisua=mSing.mAktiviteetti1.eiValaisua;

        //Varmistetaan, että Kirjasto1 saatiin ladattua. Jollei, ilmoitetaan
        //asiasta käyttäjälle
        if (!onkoKirjasto1Ladattu) {
            mSing.mAktiviteetti1.Kirjasto1Epaonnistui();
        }

    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup parent, Bundle savedInstanceState) {

        //tämä siltä varalta, että Kirjasto2 puuttuu laitteesta
        if (!onkoKirjasto1Ladattu) {
            mSing.mAktiviteetti1.mFragmentti3=null;
            return null;
        }

        // xml tiedostosta luettavat jutut muodostavat View-luokan ilmentymän
        return inflater.inflate(R.layout.fragment_cadruutu, parent, false);

    }

    @Override
    public void onViewCreated(View view, Bundle savedInstanceState) {

        //Kytketään menupalkin Text Viewer -MenuItem käyttöön
        MenuItem mi=mSing.mAktiviteetti1.mMenu.findItem(R.id.CADruutu);
        mi.setEnabled(true);

        //mahdollistetaan tulostaminen
        bitmap=Bitmap.createBitmap(10, 10, Bitmap.Config.ARGB_8888);
        mi = mSing.mAktiviteetti1.mMenu.findItem(R.id.tulostaCAD);
        mi.setEnabled(true);

        //Tallennetaan view-luokan jäsenmuuttuja
        mNakyma=view;

        //Otetaan talteen OpenGLView ilmentymän kehys
        mKehys=view.findViewById(R.id.CADRuutuSisalto);

        //luodaan opengl konteksti
        if (mSing.mAktiviteetti1.mSurf==null) {
            mSing.mAktiviteetti1.mSurf = new OpenGLView(getActivity());
        }
        ViewGroup par=(ViewGroup) mSing.mAktiviteetti1.mSurf.getParent();
        if (null!=par) {
            par.removeView(mSing.mAktiviteetti1.mSurf);
        }
        mKehys.addView(mSing.mAktiviteetti1.mSurf);

        //luodaan handler viestien lähettämiseen UI-threadiin OpenGL-threadista
        if (mHandler==null) {
            mHandler = new Handler();
        }

        //Määritetään OpenGL ES renderöintipinnan likimääräinen koko
        int orientaatio=mNakyma.getContext().getResources().getConfiguration().orientation;
        if (orientaatio==Configuration.ORIENTATION_LANDSCAPE) {
            mLeveys=mSing.mAktiviteetti1.mPoint.y/2;
            mKorkeus=mSing.mAktiviteetti1.mPoint.x - annaActionbarKorkeus()
                    - annaStatusbarKorkeus()-annaNappiRiviKorkeus();
        } else {
            mLeveys=mSing.mAktiviteetti1.mPoint.x;
            mKorkeus=mSing.mAktiviteetti1.mPoint.y - annaActionbarKorkeus()
                    - annaStatusbarKorkeus()-annaNappiRiviKorkeus();
        }
        mSing.mAktiviteetti1.mSurf.setPreserveEGLContextOnPause(true);

        //Asetetaan kuuntelijat
        Button siirto=view.findViewById(R.id.taakseCAD);
        siirto.setOnClickListener(new CADRuutu.TaakseCAD());
        siirto=view.findViewById(R.id.eteenCAD);
        siirto.setOnClickListener(new CADRuutu.EteenCAD());
        siirto=view.findViewById(R.id.valintaCAD);
        siirto.setOnClickListener(new CADRuutu.ValintaListaCAD());
        siirto=view.findViewById(R.id.kaynnistaCAD);
        siirto.setOnClickListener(new CADRuutu.KaynnistaCAD());
        siirto=view.findViewById(R.id.pysaytaCAD);
        siirto.setOnClickListener(new CADRuutu.PysaytaCAD());

        //alustetaan tarvittavat taulukot
        valintaListaB1=new ArrayList<>();
        valintaListaB2=new ArrayList<>();
        valintaListaC1=new ArrayList<>();
        valintaListaC2=new ArrayList<>();

        //Otetaan talteen assets kansion polku
        polku=mSing.mAktiviteetti1.getApplicationContext().getFilesDir().getAbsolutePath();

        //Selvitetään, tukeeko laite tarvittavia OpenGL ES laajennuksia, jos sitä ei ole jo tehty
        if (!openglOnkoTuettu && onkoKirjasto1Ladattu) {
            mSing.mAktiviteetti1.mSurf.valmisteleKirjasto1();
        }

        //jos tämä on ensimmäinen kerta, kun mFragmentti3 otetaan käyttöön...
        if (mSing.mAktiviteetti1.mOnkoCADAvattu==false) {

            //...merkitään CAD tiedosto avatuksi...
            mSing.mAktiviteetti1.mOnkoCADAvattu = true;

            //...ja OpenGL ES pitäisi olla käytettävissä, joten avataan CAD tiedosto
            avaaCAD();
        } else {

            //Jos joku CAD tiedosto on jo avattu, tänne jouduttiin Androidin luotua mFragmentti3
            //uudestaan, ja silloin pitää tehdä jälkitöitä...
            tilannePaivitys();
        }
    }

    public View annaNakyma() {
        return mNakyma;
    }

    public void asetaNakymanKoko(int leveys, int korkeus) {
        mLeveys=leveys;
        mKorkeus=korkeus;
    }

    public void avaaCAD() {

        //varmistetaan, että Kirjasto1 on saatavilla
        if (onkoKirjasto1Ladattu) {

            //avataan tiedosto
            mSing.mAktiviteetti1.mSurf.lataaData();

            //merkitään muistiin, että avattu CAD on suoraan saatavilla päävalikosta
            mSing.mAktiviteetti1.mFragmentti1.tuoEsille(mSing.mAktiviteetti1.mCADTiedosto,
                    'C');
        } else {
            Toast toast=Toast.makeText(getContext(), "Unable to launch OpenGL ES!",
                    Toast.LENGTH_LONG);
            toast.show();
            openglOnkoTuettu=false;
        }
    }

    public void avaaUudelleen() {

        //varmistetaan, että Kirjasto1 on saatavilla
        if (onkoKirjasto1Ladattu) {
            mSing.mAktiviteetti1.mSurf.lataaData();
        } else {
            Toast toast=Toast.makeText(getContext(), "Unable to launch OpenGL ES!",
                    Toast.LENGTH_LONG);
            toast.show();
            openglOnkoTuettu=false;
        }
    }

    public void avataan() {

        //estetään JAVA:aa antamasta renderointipyyntöjä ja samalla varmistetaan, että kirjasto
        //on valmisteltu
        while (!mSing.mAktiviteetti1.mOpenGLRenderoija.doPermitRender(false)) {
            try {
                Thread.sleep(50);
            } catch (Exception e) {

            }
        }

        //Tämä kytkee OpenGL-näkymän esille, jos se ei ole jo esillä
        if (kaynnistaCAD(mSing.mAktiviteetti1.mCADTiedosto.getBytes(), mLeveys, mKorkeus)) {

            //CADRuutu fragmentin nappuloiden tila pitää päivittää, mikä edellyttää viestiä OpenGL
            //säikeestä UI säikeeseen
            final Runnable r = new Runnable() {
                @Override
                public void run() {

                    //varmistetaan CAD-tiedoston nimi
                    mSing.mAktiviteetti1.asetaCADVarmistus(mSing.mAktiviteetti1.mCADTiedosto);

                    mSing.mAktiviteetti1.mFragmentti3.avaamisenJalkeen();

                    //tässä vaiheessa JAVA saa taas esittää renderointipyyntöjä
                    mSing.mAktiviteetti1.mOpenGLRenderoija.doPermitRender(true);
                }
            };
            mSing.mAktiviteetti1.runOnUiThread(r);
        } else {

            //jos CAD-tiedoston avaaminen epäonnistui, annetaan käyttäjälle ilmoitus UI säikeeseen
            final Runnable r = new Runnable() {
                @Override
                public void run() {
                    Toast toast = Toast.makeText(getContext(), "Couldn't load an object. " +
                            "Try using option 'use simple graphics'.", Toast.LENGTH_SHORT);
                    toast.show();

                    //epäonnistuessa pitää korvata viallinen nimi jollain, joka ei ole viallinen
                    mSing.mAktiviteetti1.mCADTiedosto=mSing.mAktiviteetti1.annaCADVarmistus();

                    mSing.mAktiviteetti1.mFragmentti3.avaamisenJalkeen();

                    //tässä vaiheessa JAVA saa taas esittää renderointipyyntöjä
                    mSing.mAktiviteetti1.mOpenGLRenderoija.doPermitRender(true);
                }
            };
            mSing.mAktiviteetti1.runOnUiThread(r);
        }
    }

    public void avaamisenJalkeen() {

        //kytketään nappulat tilanteen mukaiseen asentoon, jos mahdollista, niinkuin pitäisi olla
        View v=mNakyma.findViewById(R.id.eteenCAD);
        if (v!=null) {
            v.setEnabled(eteen);
            v = mNakyma.findViewById(R.id.taakseCAD);
            v.setEnabled(taakse);
            v = mNakyma.findViewById(R.id.kaynnistaCAD);
            if (animaatioTila == 0) {
                v.setEnabled(true);
            } else {
                v.setEnabled(false);
            }
            v = mNakyma.findViewById(R.id.pysaytaCAD);
            v.setEnabled(false);
        }

        //merkitään muistiin, että avattu CAD näkymä on suoraan saatavilla päävalikosta
        mSing.mAktiviteetti1.mFragmentti1.tuoEsille(mSing.mAktiviteetti1.mCADTiedosto, 'C');
    }

    //käyttäjä on klikannut taakse nappulaa
    private void taakseClick() {

        taakse();
        View v1=mNakyma.findViewById(R.id.eteenCAD);
        v1.setEnabled(eteen);
        v1=mNakyma.findViewById(R.id.taakseCAD);
        v1.setEnabled(taakse);
        v1=mNakyma.findViewById(R.id.kaynnistaCAD);
        Button v2=mNakyma.findViewById(R.id.pysaytaCAD);
        if  (animaatioTila==0) {
            v1.setEnabled(true);
            v2.setEnabled(false);
            v2.setText(R.string.pysayta);
        } else if (animaatioTila==1) {
            v1.setEnabled(false);
            v2.setEnabled(true);
            v2.setText(R.string.pysayta);
        } else if (animaatioTila==2) {
            v1.setEnabled(true);
            v2.setEnabled(true);
            v2.setText(R.string.alkuun);
        } else {
            v1.setEnabled(false);
            v2.setEnabled(false);
            v2.setText(R.string.pysayta);
        }
    }

    //kayttäjä on klikannut eteen nappulaa
    private void eteenClick() {

        eteen();
        View v1=mNakyma.findViewById(R.id.eteenCAD);
        v1.setEnabled(eteen);
        v1=mNakyma.findViewById(R.id.taakseCAD);
        v1.setEnabled(taakse);
        v1=mNakyma.findViewById(R.id.kaynnistaCAD);
        Button v2=mNakyma.findViewById(R.id.pysaytaCAD);
        if  (animaatioTila==0) {
            v1.setEnabled(true);
            v2.setEnabled(false);
            v2.setText(R.string.pysayta);
        } else if (animaatioTila==1) {
            v1.setEnabled(false);
            v2.setEnabled(true);
            v2.setText(R.string.pysayta);
        } else if (animaatioTila==2) {
            v1.setEnabled(true);
            v2.setEnabled(true);
            v2.setText(R.string.alkuun);
        } else {
            v1.setEnabled(false);
            v2.setEnabled(false);
            v2.setText(R.string.pysayta);
        }
    }
    /*
        private void tarkastaLayerit(int index) {

            //muutetaan valinta siinä indeksissä, jota on klikattu
            if (index<valintoja1) {
                if (valintaListaB1.get(index)==true) {
                    valintaListaB1.set(index, false);
                } else {
                    valintaListaB1.set(index, true);
                }
            } else {
                if (valintaListaB2.get(index-valintoja1)==true) {
                    valintaListaB2.set(index-valintoja1, false);
                } else {
                    valintaListaB2.set(index-valintoja1, true);
                }
            }

            //sitten lähetetään tiedot Kirjasto1:lle
            vieValinnat();

            //koska lähetetyissä tiedoissa saattaa olla ristiriitaisuuksia, jotka selvitetään
            //vasta Kirjasto2:ssa, meidän pitää tuoda valinnat takaisin
        }
    */
    private void valintaListaClick() {

        //avataan dialogifragmentti, jossa käyttäjä voi tehdä valintoja.
        FragmentTransaction ft=mSing.mAktiviteetti1.mFragmentManager.beginTransaction();
        Fragment prev=mSing.mAktiviteetti1.mFragmentManager.
                findFragmentByTag("Valintalista");
        if (prev!=null) {
            ft.remove(prev);
        }

        //Jostain syystä Tasavalta kaatuu silloin tällöin, kun alla olevat koodirivit
        //suoritetaan. Try-catch estää mahdollisen kaatumisen. Ei pitäisi kaatua enää...
        try {
            ft.commitNow();
            ValintaLista v=new ValintaLista();
            v.show(mSing.mAktiviteetti1.mFragmentManager, "Valintalista");
        } catch (Exception e) {
            Toast toast=Toast.makeText(mSing.mAktiviteetti1, "Operation failed.",
                    Toast.LENGTH_LONG);
            toast.show();
        }
    }

    public void tulostetaan(boolean tausta) {

        //välitetään käyttäjän tulostuspyyntö Kirjasto1:een
        tulosta(tausta);
    }

    private void tulostettu(String name) {

        if (name!=null) {

            //lisätään nimen perään juokseva numero
            final String nimi = name + "_" + mJuoksevaNumero.toString();
            mJuoksevaNumero++;

            //koska tätä funktiota kutsutaan OpenGL-renderöintisäikeestä, tehdään loppu
            //runnablessa, joka ajetaan UI-säikeessä
            final Runnable r=new Runnable() {
                @Override
                public void run() {

                    try {

                        // Tallennetaan tiedosto images kansioon
                        String polku = mSing.mAktiviteetti1.getApplicationContext().getFilesDir().
                                getAbsolutePath();
                        File kansio = new File(polku, "images");
                        kansio.mkdirs();
                        final File tiedosto = new File(kansio, nimi + ".jpg");

                        //muutetaan Kirjasto1:n luoma bitmap JPG tiedostoksi.
                        FileOutputStream ulos = new FileOutputStream(tiedosto);
                        bitmap.compress(Bitmap.CompressFormat.JPEG, 100, ulos);
                        ulos.flush();
                        ulos.close();

                        //jotta käyttäjä pääsisi helpommin käsiksi jpg-tiedostoon, kopioidaan
                        // se sähköpostiin
                        //liitetiedostoksi. Sitä varten tiedosto tarvitsee URI:n
                        Uri contentUri = FileProvider.getUriForFile(
                                mSing.mAktiviteetti1.getApplicationContext(),
                                BuildConfig.APPLICATION_ID + ".fileprovider", tiedosto);

                        //jos ulkoinen ohjelma epäonnistuu, käyttäjä ei saa asiasta ilmoitusta
                        shareDocument(contentUri);

                    } catch (Exception e) {

                    }
                }
            };
            mSing.mAktiviteetti1.runOnUiThread(r);

        }
    }

    private void shareDocument(Uri uri) {

        //luodaan yhteys ulkopuoliseen sähköpostiohjelmaan intent:n avulla
        Intent shareIntent = new Intent();
        shareIntent.setAction(Intent.ACTION_SEND);
        shareIntent.setType("application/jpeg");

        //olettaen, että tämä menee sähköpostiohjelmaan
        shareIntent.putExtra(Intent.EXTRA_SUBJECT, "Here is a JPEG from "+
                BuildConfig.APPLICATION_ID);

        //liitetään PDF Uri:n avulla intentiin
        shareIntent.putExtra(Intent.EXTRA_STREAM, uri);

        //käynnistetään jokin sopiva ohjelma, jollainen toivottavasti löytyy Android-laitteesta
        mSing.mAktiviteetti1.lahetaJohonkin(shareIntent);
    }

    //tällä funktiolla luodaan bitmap
    private Bitmap luoBittiKartta(int leveys, int korkeus) {
        try {
            bitmap.recycle();
            bitmap=Bitmap.createBitmap(leveys, korkeus, Bitmap.Config.ARGB_8888);
            return bitmap;
        } catch (Exception e) {
            return null;
        }
    }

    private void kaynnistaClick() {

        //käyttäjä pyytää animaation käynnistämistä. Ilmoitetaan Kirjasto1:lle
        eloKuva(0);
        ajetaan=true;
        Button v=mNakyma.findViewById(R.id.kaynnistaCAD);
        v.setEnabled(false);
        v=mNakyma.findViewById(R.id.pysaytaCAD);
        v.setText(R.string.pysayta);
        v.setEnabled(true);

    }

    private void pysaytaClick() {

        if (ajetaan) {

            //pysäytetään animaatio
            eloKuva(1);
            ajetaan=false;
            Button v=mNakyma.findViewById(R.id.kaynnistaCAD);
            v.setEnabled(true);
            v=mNakyma.findViewById(R.id.pysaytaCAD);
            v.setText(R.string.alkuun);
            v.setEnabled(true);
        } else {

            //siirretään animaatio alkuun
            eloKuva(2);
            Button v=mNakyma.findViewById(R.id.kaynnistaCAD);
            v.setEnabled(true);
            v=mNakyma.findViewById(R.id.pysaytaCAD);
            v.setText(R.string.pysayta);
            v.setEnabled(false);
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

    public void paivitaAla() {

        //määritetään OpenGL näkymän likimääräinen koko. Tämän jälkeen saatetaan määrittää täsmällinen
        //koko
        if (mSing.mAktiviteetti1.mVaakaKerroin==2) {
            mLeveys=mSing.mAktiviteetti1.mPoint.y/2;
            mKorkeus=mSing.mAktiviteetti1.mPoint.x - annaActionbarKorkeus()
                    - annaStatusbarKorkeus()-annaNappiRiviKorkeus();
        } else {
            mLeveys=mSing.mAktiviteetti1.mPoint.x;
            mKorkeus=mSing.mAktiviteetti1.mPoint.y - annaActionbarKorkeus()
                    - annaStatusbarKorkeus()-annaNappiRiviKorkeus();
        }
        muutaAla(mLeveys, mKorkeus);
    }

}
