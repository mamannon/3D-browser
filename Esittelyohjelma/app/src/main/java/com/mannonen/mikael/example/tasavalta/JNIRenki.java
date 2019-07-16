package com.mannonen.mikael.example.tasavalta;

import android.annotation.TargetApi;
import android.app.Dialog;
import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.Canvas;
import android.graphics.Paint;
import android.graphics.Rect;
import android.graphics.RectF;
import android.net.Uri;
import android.print.PrintAttributes;
import android.print.pdf.PrintedPdfDocument;
import android.support.v4.app.FragmentTransaction;
import android.support.v4.content.FileProvider;
import android.util.DisplayMetrics;
import android.view.View;
import android.view.ViewGroup;
import android.widget.LinearLayout;
import android.widget.Toast;

import java.io.File;
import java.io.FileOutputStream;
import java.io.UnsupportedEncodingException;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.Map;

import static android.content.Context.PRINT_SERVICE;
import static com.mannonen.mikael.example.tasavalta.Singleton.annaIlmentyma;

public class JNIRenki {

    private Singleton mSing;

    private PrintedPdfDocument mDokumentti;
    private PrintedPdfDocument.Page mSivu;
    public CanvasView mPrintCan;
    public int mSivuNumero;

    public ArrayList<String> mOtsikkoLista;
    public Dialog mDialog;

    public int mHeight;
    public int mWidth;
    public int mWeight;
    public byte mItalic;
    public byte mUnderline;
    public byte mStrikeOut;
    public byte mPitchAndFamily;
    public String mFaceName;
    public int mAscent;
    public int mDescent;
    public int mInternalLeading;
    public int mExternalLeading;
    public int mAveCharWidth;

    public char mTyyppi;
    public int mVari;
    public Bitmap mBitmap;

    public int mTyyli1;
    public int mLeveys;
    public int mPituus;
    public int mTyyli2;
    public int mHatch;
    public int mDash1;
    public int mDash2;

    private int mDensityDpi;
    private int mHeightPixels;
    private int mWidthPixels;
    private float mDensity;
    private float mScaledDensity;
    private float mXDpi;
    private float mYDpi;
    private float mDisplaySize;

    //alla olevat muuttujat kuuluvat CanvasView luokalle, mutta JNIRenki:iin
    //sijoitettuna niitä ei tarvitse varmuuskopioda orientaatiomuutoksissa. Niitä voi
    //käyttää useampi CanvasView-ilmentymä
    public int cLeveys;
    public int cKorkeus;
    public Map<Integer, Paint> cFontit;
    public Map<Integer, Sivellin> cSiveltimet;
    public Map<Integer, Paint> cKynat;
    public Map<Integer, Bitmap> cKuvat;

    public boolean cTulostetaanko=false;

    public JNIRenki() {

        //Ilmoittaudutaan Aktiviteetti1:een
        mSing=annaIlmentyma();
        mSing.mAktiviteetti1.mRenki=this;

        cFontit=new HashMap<>();
        cSiveltimet=new HashMap<>();
        cKynat=new HashMap<>();
        mOtsikkoLista=new ArrayList<>();

    }

    @TargetApi(19)
    private void valmisteleTulostus(float lev, float kor) {

        cLeveys =(int) lev;
        cKorkeus =(int) kor;
        cTulostetaanko = true;

        //tämä toimii, vaikka getApplicationContext antaa eri kontekstin kuin mikä
        //TaustaTulostus ilmentymällä on, jonka kautta tähän funktioon tullaan
        mPrintCan = new CanvasView(mSing.mAktiviteetti1.getApplicationContext());
        cTulostetaanko = false;

        mSivuNumero=0;

    }

    @TargetApi(19)
    private boolean aloitaTulostus(float Xdpi , float Ydpi, int sivuMaara) {

        try {
            PrintAttributes printAttrs = new PrintAttributes.Builder().
                    setColorMode(PrintAttributes.COLOR_MODE_COLOR).
                    setMediaSize(PrintAttributes.MediaSize.ISO_A4).
                    setResolution(new PrintAttributes.Resolution("pdf", PRINT_SERVICE,
                            (int) Xdpi, (int) Ydpi)).
                    setMinMargins(PrintAttributes.Margins.NO_MARGINS).
                    build();
            mDokumentti = new PrintedPdfDocument(mSing.mAktiviteetti1.getApplicationContext(),
                    printAttrs);

            return true;
        } catch (Exception e) {
            return false;
        }
    }

    @TargetApi(19)
    private boolean aloitaSivu() {

        try {

            //luodaan uusi sivu ja putsataan edellisen sotkut bitmapista
            mSivu=mDokumentti.startPage(mSivuNumero);
            varitaNakyma(0xffffffff, 2);

            return true;
        } catch (Exception e) {
            return false;
        }


    }

    @TargetApi(19)
    private boolean lopetaSivu() {

        try {

            //skaalataan ja piirretään piirretty bitmap mSivun kanvaasiin
            Bitmap siirto=mPrintCan.annaBitmap();
            Canvas kanvaasi=mSivu.getCanvas();

            Rect alku=new Rect(0, 0, siirto.getWidth(), siirto.getHeight());
            int leveysK=kanvaasi.getWidth();
            int korkeusK=kanvaasi.getHeight();
            RectF loppu=new RectF(0,0,leveysK, korkeusK);

            kanvaasi.drawBitmap(siirto, alku, loppu, null);

            //prosessoidaan sivu pdf-dokumenttiin
            mDokumentti.finishPage(mSivu);
            mSivuNumero++;
            return true;
        } catch (Exception e) {
            return false;
        }
    }

    @TargetApi(19)
    private boolean lopetaTulostus(String nimi) {

        FileOutputStream os;
        File file;

        try {

            //tulostetaan luotu dokumentti pdf-tiedostoon kansioon /files/pdfs/
            File pdfDirPath = new File(mSing.mAktiviteetti1.getApplicationContext().getFilesDir(),
                    "pdfs");
            pdfDirPath.mkdirs();
            file = new File(pdfDirPath, nimi+".pdf");
            os = new FileOutputStream(file);
            mDokumentti.writeTo(os);
            mDokumentti.close();
            os.close();

            //jotta käyttäjä pääsisi helpommin käsiksi pdf-tiedostoon, kopioidaan se sähköpostiin
            //liitetiedostoksi. Sitä varten tiedosto tarvitsee URI:n
            Uri contentUri = FileProvider.getUriForFile(
                    mSing.mAktiviteetti1.getApplicationContext(),
                    BuildConfig.APPLICATION_ID + ".fileprovider", file);

            //jos ulkoinen ohjelma epäonnistuu, käyttäjä ei saa asiasta ilmoitusta
            shareDocument(contentUri);

        } catch (Exception e) {
            return false;
        }

        return true;
    }

    @TargetApi(19)
    private void shareDocument(Uri uri) {

        //luodaan yhteys ulkopuoliseen sähköpostiohjelmaan intent:n avulla
        Intent shareIntent = new Intent();
        shareIntent.setAction(Intent.ACTION_SEND);
        shareIntent.setType("application/pdf");

        //olettaen, että tämä menee sähköpostiohjelmaan
        shareIntent.putExtra(Intent.EXTRA_SUBJECT,
                "Here is a PDF from "+BuildConfig.APPLICATION_ID);

        //liitetään PDF Uri:n avulla intentiin
        shareIntent.putExtra(Intent.EXTRA_STREAM, uri);

        //käynnistetään jokin sopiva ohjelma, jollainen toivottavasti löytyy Android-laitteesta
        mSing.mAktiviteetti1.lahetaJohonkin(shareIntent);
    }

    //tätä funktiota kutsutaan Kirjasto2:sta käsin. Se antaa sovelluksen tiedostopolun
    private String annaTiedostoPolku() {

        try {
            return mSing.mAktiviteetti1.getApplicationContext().getFilesDir().getAbsolutePath();
        } catch (Exception e) {
            return null;
        }
    }

    //tätä funktiota kutsutaan Kirjasto2:sta käsin. Sillä luodaan haluttu fontti.
    private int luoFontti() { return mSing.mAktiviteetti1.mFragmentti2.mCan.luoFontti(); }

    //Tätä funktiota kustutaan Kirjasto2:sta käsin. Sillä tuhotaan kyseinen fontti.
    private boolean tuhoaFontti(int fontti) {
        return mSing.mAktiviteetti1.mFragmentti2.mCan.tuhoaFontti(fontti);}

    //tätä funktiota kutsutaan Kirjasto2:sta käsin. Sillä otetaan käyttöön haluttu fontti
    private int asetaFontti(int fontti, int hdc) {
        if (hdc<2) {
            return mSing.mAktiviteetti1.mFragmentti2.mCan.asetaFontti(fontti);
        } else {
            return mPrintCan.asetaFontti(fontti);
        }
    }

    //tätä funktiota kutsutaan kirjasto2:sta käsin. Se antaa käytössä olevan fontin mittatietoja
    private void annaTekstiMetriikka(int hdc) {
        if (hdc<2) {
            mSing.mAktiviteetti1.mFragmentti2.mCan.annaTekstiMetriikka();
        } else {
            mPrintCan.annaTekstiMetriikka();
        }
    }

    //tätä funktiota kutsutaan Kirjasto2:sta käsin. Se antaa annetun tekstipätkän fyysiset mitat
    private void annaTekstinUlottuvuus(byte[] teksti, int hdc) {
        try {
            String siirto=new String(teksti, "UTF-8");
            if (hdc<2) {
                mSing.mAktiviteetti1.mFragmentti2.mCan.annaTekstinUlottuvuus(siirto);
            } else {
                mPrintCan.annaTekstinUlottuvuus(siirto);
            }
        } catch (UnsupportedEncodingException e) {

            //tämän tarkoituksena on vain antaa tyhjä tila sanan kohdalle
            if (hdc<2) {
                mWidth = 50;
                mHeight = 1;
            } else {
                mWidth = 500;
                mHeight = 1;
            }
        }
    }

    //tätä funktiota kutsutaan Kirjasto2:sta käsin. Se piirtää kanvaasille annetun tekstin
    //haluttuun kohtaan
    private boolean piirraTeksti(int x, int y, byte[] teksti, int hdc) {
        try {
            String siirto=new String(teksti, "UTF-8");
            if (hdc<2) {
                return mSing.mAktiviteetti1.mFragmentti2.mCan.piirraTeksti(x, y, siirto);
            } else {
                return mPrintCan.piirraTeksti(x, y, siirto);
            }
        } catch (UnsupportedEncodingException e) {

            //merkistön muunnoksen epäonnistuessa tekstiä ei piirretä, vaan viivat
            if (hdc<2) {
                mSing.mAktiviteetti1.mFragmentti2.mCan.piirraTeksti(x, y, "----");
            } else {
                mPrintCan.piirraTeksti(x, y, "----");
            }
            return false;
        }
    }

    //tätä funktiota kutsutaan Kirjasto2:sta käsin. Sillä luodaan haluttu sivellin.
    private int luoSivellin() { return mSing.mAktiviteetti1.mFragmentti2.mCan.luoSivellin(); }

    //tätä funktiota kutsutaan Kirjasto2:sta käsin.
    //Sillä otetaan käyttöön haluttu sivellin
    private int asetaSivellin(int sivellin, int hdc) {
        if (hdc<2) {
            return mSing.mAktiviteetti1.mFragmentti2.mCan.asetaSivellin(sivellin);
        } else {
            return mPrintCan.asetaSivellin(sivellin);
        }
    }

    //tätä funtkiota käytetään Kirjasto2:sta käsin. Sillä poistetaan haluttu sivellin
    private boolean tuhoaSivellin(int sivellin) {
        return mSing.mAktiviteetti1.mFragmentti2.mCan.tuhoaSivellin(sivellin);
    }

        //tätä funktiota kutsutaan Kirjasto2:sta käsin. Sillä täytetään nelio värillä
    private boolean taytaNelio(int x1, int y1, int x2, int y2, int hdc) {
        if (hdc<2) {
            return mSing.mAktiviteetti1.mFragmentti2.mCan.taytaNelio(x1, y1, x2, y2);
        } else {
            return mPrintCan.taytaNelio(x1, y1, x2, y2);
        }
    }

    //tätä funktiota kutsutaan Kirjasto2:sta käsin. Sillä luodaan haluttu kynä
    private int luoKyna() {return mSing.mAktiviteetti1.mFragmentti2.mCan.luoKyna(); }

    //tätä funktiota kutsutaan Kirjasto2:sta käsin. Sillä otetaan käyttöön haluttu kynä
    private int asetaKyna(int kyna, int hdc) {
        if (hdc<2) {
            return mSing.mAktiviteetti1.mFragmentti2.mCan.asetaKyna(kyna);
        } else {
            return mPrintCan.asetaKyna(kyna);
        }
    }

    //tätä funktiota kutsutaan Kirjasto2:sta käsin. Sillä tuhotaan valittu kynä
    private boolean tuhoaKyna(int kyna) {
        return mSing.mAktiviteetti1.mFragmentti2.mCan.tuhoaKyna(kyna);
    }

    //tätä funktiota kutsutaan Kirjasto2:sta käsin. Sillä piirretään polyline
    private boolean piirraPolyline(float[] pisteet, int pisteita, int hdc) {
        if (hdc<2) {
            return mSing.mAktiviteetti1.mFragmentti2.mCan.piirraPolyline(pisteet, pisteita);
        } else {
            return mPrintCan.piirraPolyline(pisteet, pisteita);
        }
    }

    //tätä funktiota kutsutaan Kirjasto2:sta käsin. Sillä piirretään viiva
    private boolean piirraViiva(int x1, int y1, int x2, int y2, int hdc) {
        if (hdc<2) {
            return mSing.mAktiviteetti1.mFragmentti2.mCan.piirraViiva(x1, y1, x2, y2);
        } else {
            return mPrintCan.piirraViiva(x1, y1, x2, y2);
        }
    }

    //tätä funktiota kutsutaan Kirjasto2:sta käsin. Sillä piirretään neliö
    private boolean piirraNelio(int x1, int y1, int x2, int y2, int hdc) {
        if (hdc<2) {
            return mSing.mAktiviteetti1.mFragmentti2.mCan.piirraNelio(x1, y1, x2, y2);
        } else {
            return mPrintCan.piirraNelio(x1, y1, x2, y2);
        }
    }

    //tätä funktiota kutsutaan Kirjasto2:sta käsin. Sillä asetetaan taustaväri
    private int asetaTaustaVari(int vari, int hdc) {
        if (hdc<2) {
            return mSing.mAktiviteetti1.mFragmentti2.mCan.asetaTaustaVari(vari);
        } else {
            return mPrintCan.asetaTaustaVari(vari);
        }
    }

    //tätä funktioita kutsutaan Kirjasto2:sta käsin. Sillä kysytään taustaväri
    private int annaTaustaVari(int hdc) {
        if (hdc<2) {
            return mSing.mAktiviteetti1.mFragmentti2.mCan.annaTaustaVari();
        } else {
            return mPrintCan.annaTaustaVari();
        }
    }

    //tätä funktiota kutsutaan Kirjasto2:sta käsin. Sillä asetetaan tekstin väri
    private int asetaTekstinVari(int vari, int hdc) {
        if (hdc<2) {
            return mSing.mAktiviteetti1.mFragmentti2.mCan.asetaTekstinVari(vari);
        } else {
            return mPrintCan.asetaTekstinVari(vari);
        }
    }

    //tätä funktiota kutsutaan Kirjasto2:sta käsin. Sillä kysytään tekstin väri
    private int annaTekstinVari(int hdc) {
        if (hdc<2) {
            return mSing.mAktiviteetti1.mFragmentti2.mCan.annaTekstinVari();
        } else {
            return mPrintCan.annaTaustaVari();
        }
    }

    //tätä funktiota kutsutaan Kirjasto2:sta käsin. Sillä piiretään kuva
    private boolean piirraBittiKartta(int x1, int y1, int x2, int y2, Bitmap bitmap, int hdc) {
        if (hdc<2) {
            return mSing.mAktiviteetti1.mFragmentti2.mCan.piirraBittiKartta(x1, y1, x2, y2, bitmap);
        } else {
            return mPrintCan.piirraBittiKartta(x1, y1, x2, y2, bitmap);
        }
    }

    //tätä funktiota kutsutaan Kirjasto2:sta käsin. Sillä luodaan kuva
    private Bitmap luoBittiKartta(int leveys, int korkeus, int formaatti, int hdc) {
        if (hdc<2) {
            return mSing.mAktiviteetti1.mFragmentti2.mCan.luoBittiKartta(leveys, korkeus,
                    formaatti);
        } else {
            return mPrintCan.luoBittiKartta(leveys, korkeus, formaatti);
        }
    }

    //tätä funktiota kutsutaan Kirjasto2:sta käsin. Sillä täytetään ellipsi värillä
    private boolean taytaEllipsi(int x1, int y1, int x2, int y2, int hdc) {
        if (hdc<2) {
            return mSing.mAktiviteetti1.mFragmentti2.mCan.taytaEllipsi(x1, y1, x2, y2);
        } else {
            return mPrintCan.taytaEllipsi(x1, y1, x2, y2);
        }
    }

    //tätä funktiota kutsutaan Kirjasto2:sta käsin. Sillä piirretään ellipsin reunat
    private boolean piirraEllipsi(int x1, int y1, int x2, int y2, int hdc) {
        if (hdc<2) {
            return mSing.mAktiviteetti1.mFragmentti2.mCan.piirraEllipsi(x1, y1, x2, y2);
        } else {
            return mPrintCan.piirraEllipsi(x1, y1, x2, y2);
        }
    }

    //tätä funktiota kutsutaan Kirjasto2:sta käsin. Sillä piirretään pyöreäkulmaisen nelion reunat
    private boolean piirraPyoreaNelio(int x1, int y1, int x2, int y2, int ax, int ay, int hdc) {
        if (hdc<2) {
            return mSing.mAktiviteetti1.mFragmentti2.mCan.piirraPyoreaNelio(x1, y1, x2, y2, ax, ay);
        } else {
            return mPrintCan.piirraPyoreaNelio(x1, y1, x2, y2, ax, ay);
        }
    }

    //tätä funktiota kutsutaan Kirjasto2:sta käsin. Sillä täytetään pyoreäkulmainen neliö
    private boolean taytaPyoreaNelio(int x1, int y1, int x2, int y2, int ax, int ay, int hdc) {
        if (hdc<2) {
            return mSing.mAktiviteetti1.mFragmentti2.mCan.taytaPyoreaNelio(x1, y1, x2, y2, ax, ay);
        } else {
            return mPrintCan.taytaPyoreaNelio(x1, y1, x2, y2, ax, ay);
        }
    }

    //tätä funktiota kutsutaan Kirjasto2:sta käsin. Sillä väritetään yksittäinen pikseli
    private boolean setPixel(int x, int y, int vari, int hdc) {
        if (hdc<2) {
            return mSing.mAktiviteetti1.mFragmentti2.mCan.setPixel(x, y, vari);
        } else {
            return mPrintCan.setPixel(x, y, vari);
        }
    }

    //tätä funktiota kutsutaan Kijasto2:sta käsin. tämä funktio pyyhkii kanvaasista kaiken sinne
    //piirretyn annetulla värillä
    private boolean varitaNakyma(int vari, int hdc) {
        if (hdc<2) {
            return mSing.mAktiviteetti1.mFragmentti2.mCan.varitaNakyma(vari);
        } else {
            return mPrintCan.varitaNakyma(vari);
        }
    }

    //tätä funktiota kutsutaan Kirjasto2:sta käsin. Sillä selvitetään laitteen näytön tuumakoko
    private void annaNaytonTiedot() {
        mDisplaySize = mSing.mAktiviteetti1.mFragmentti2.mCan.annaNaytonTuumaKoko();
        DisplayMetrics siirto = mSing.mAktiviteetti1.mDm;
        mDensity = siirto.density;
        mScaledDensity = siirto.scaledDensity;
        mDensityDpi = siirto.densityDpi;
        mHeightPixels = siirto.heightPixels;
        mWidthPixels = siirto.widthPixels;
        mXDpi = siirto.xdpi;
        mYDpi = siirto.ydpi;
    }

    //tätä funktiota kutsutaan Kirjasto2:sta käsin. Sillä selvitetään ikkunan leveys
    private int annaIkkunanLeveys(int hdc) {
        if (hdc<2) {
            return mSing.mAktiviteetti1.mFragmentti2.mCan.annaIkkunanLeveys();
        } else {
            return mPrintCan.annaIkkunanLeveys();
        }
    }

    //tätä funktiota kutsutaan Kirjasto2:sta käsin. Sillä selvitetään ikkunan korkeus
    private int annaIkkunanKorkeus(int hdc) {
        if (hdc<2) {
            return mSing.mAktiviteetti1.mFragmentti2.mCan.annaIkkunanKorkeus();
        } else {
            return mPrintCan.annaIkkunanKorkeus();
        }
    }

    //tätä funktiota kutsutaan Kirjasto2:sta käsin. Sillä avataan tiedosto
    private void avaaLinkki(String linkki) {

        final String siirto=linkki;

        //toisin kuin yllä olevissa JNIRenki luokan metodeissa, tämän metodin käyttö saattaa
        //vaatia View-objektien luontia tai muokkausta. View-objekteja voi muokata vain UI säikeestä
        //käsin, joten täytyy luoda uusi säie liitettäväksi UI säikeeseen.
        final Runnable r=new Runnable() {
            @Override
            public void run() {

                //ensin on selvitettävä, onko klikattu tekstilinkkiä vai CAD linkkiä
                if (siirto.charAt(0) == 'C') {
                    avaaCad(siirto);
                }
                if (siirto.charAt(0) == 'T') {
                    avaaTeksti(siirto);
                }
            }
        };
        mSing.mAktiviteetti1.runOnUiThread(r);
    }

    //tätä funktiota kutsutaan Kirjasto2:sta käsin. Se antaa tulostusviestin käyttäjälle
    private void tulostusViesti(String viesti) {
        Toast toast=Toast.makeText(mSing.mAktiviteetti1.getApplicationContext(), viesti,
                Toast.LENGTH_LONG);
        toast.show();
    }

    private void avaaTeksti(String linkki) {

        //linkistä pitää erotella tiedostonimi
        int ensimmainenHashtac=linkki.indexOf("#");
        int toinenHashtac=linkki.indexOf("#", ensimmainenHashtac+1);
        int kolmasHashtac=-1;
        String tiedosto;
        if (toinenHashtac==-1) {
            tiedosto=linkki.substring(ensimmainenHashtac+1);
        } else {
            tiedosto=linkki.substring(ensimmainenHashtac+1, toinenHashtac);
            kolmasHashtac=linkki.indexOf("#", toinenHashtac+1);
        }
        if (!tiedosto.contains(".")) {
            tiedosto=tiedosto+".html";
        }

        //määritetään mahdollinen ankkuri ja muutetaan alaviivat välilyönneiksi
        if (toinenHashtac!=-1) {
            String ankkuri;
            if (kolmasHashtac == -1) {
                ankkuri = linkki.substring(toinenHashtac);
            } else {
                ankkuri = linkki.substring(toinenHashtac, kolmasHashtac);
            }
            ankkuri = ankkuri.replaceAll("_", " ");

            //liitetään ankkuri tiedostonimen perään
            if (ankkuri.length() > 0) tiedosto = tiedosto + ankkuri;
        }

        //Valittu teksti pitää avata alkupäästään
        mSing.mAktiviteetti1.mOnkoTekstiAvattu=false;

        //Tekstiruutu on varmasti olemassa ja esillä, joten uuden tekstitiedoston avaaminen on
        // yksinkertaista
        mSing.mAktiviteetti1.mTekstiTiedosto=tiedosto;
        mSing.mAktiviteetti1.mFragmentti2.avaaTeksti();
    }

    private void avaaCad(String linkki) {

        //linkistä pitää erotella tiedostonimi
        int ensimmainenHashtac=linkki.indexOf("#");
        int toinenHashtac=linkki.indexOf("#", ensimmainenHashtac+1);
        String tiedosto;
        if (toinenHashtac==-1) {
            tiedosto=linkki.substring(ensimmainenHashtac+1);
        } else {
            tiedosto=linkki.substring(ensimmainenHashtac+1, toinenHashtac);
        }
        if (!tiedosto.contains(".")) {
            tiedosto=tiedosto+".cad";
        }

        //CADRuutu voi olla olemassa ja esillä, tai sitten ei, joten koodia tarvitaan enemmän
        mSing.mAktiviteetti1.mCADTiedosto=tiedosto;

        //Käyttäjä valitsi CADRuudun esille.
        CADRuutu siirto=mSing.mAktiviteetti1.mFragmentti3;

        //jos orientaationa on landscape
        if (mSing.mAktiviteetti1.mVaakaKerroin==2) {
            asetaLandscape();
        }

        //Varmistetaan, että CADruutu on olemassa
        if (siirto != null) {

            //CADruutu on joko valmiiksi esillä, jolloin vain avataan CAD-tiedosto tai sitten
            //CADruutu pitää siirtää esille, joka samalla avaa CAD-tiedoston
            if (null==siirto.annaNakyma().getParent()) {

                mSing.mAktiviteetti1.mSisaltoVasen.removeAllViews();
                mSing.mAktiviteetti1.mSisaltoVasen.addView(siirto.annaNakyma());

                //avataan uusi CAD-tiedosto
                siirto.avaaCAD();
            } else {

                //avataan uusi CAD-tiedosto
                siirto.avaaCAD();
            }
        } else {

            //muuten CADruutu täytyy luoda ja se samalla avaa CAD-tiedoston
            mSing.mAktiviteetti1.mSisaltoVasen.removeAllViews();
            FragmentTransaction fragmentTransaction =
                    mSing.mAktiviteetti1.mFragmentManager.beginTransaction();
            mSing.mAktiviteetti1.mFragmentti3 = new CADRuutu();
            fragmentTransaction.add(R.id.sisaltoVasen, mSing.mAktiviteetti1.mFragmentti3,
                    "fragmentti3");
            fragmentTransaction.disallowAddToBackStack();
            fragmentTransaction.commit();
        }
    }

    private void asetaLandscape() {

        //ensiksi meidän pitää antaa puolet näyttöruudusta oikeanpuoleiselle fragmentille
        View siirto=mSing.mAktiviteetti1.mSisaltoOikea;
        LinearLayout.LayoutParams parametrit=(LinearLayout.LayoutParams) siirto.getLayoutParams();
        parametrit.width=0;
        parametrit.weight=1;
        siirto.setLayoutParams(parametrit);

        //sitten siirrämme TekstiRuutu tai PaaValikko fragmentin oikealle
        if (mSing.mAktiviteetti1.mFragmentti2!=null) {
            siirto = mSing.mAktiviteetti1.mFragmentti2.annaNakyma();
        } else {
            siirto = mSing.mAktiviteetti1.mFragmentti1.annaNakyma();
        }
        ViewGroup parent1 = (ViewGroup) siirto.getParent();
        ViewGroup parent2=null;
        if (mSing.mAktiviteetti1.mFragmentti3!=null) {
            parent2=(ViewGroup) mSing.mAktiviteetti1.mFragmentti3.annaNakyma().getParent();
        }
        if (parent1!=null && parent2==null) {

            parent1.removeView(siirto);
            parent2=mSing.mAktiviteetti1.mSisaltoOikea;
            parent2.removeAllViews();
            parent2.addView(siirto);
        }
    }

}
