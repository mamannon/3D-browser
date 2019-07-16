package com.mannonen.mikael.example.tasavalta;

import android.content.pm.PackageManager;
import android.content.res.Configuration;
import android.os.Handler;
import android.support.v4.app.*;
import android.os.Bundle;
import android.view.*;
import android.widget.*;

import java.io.*;

import static com.mannonen.mikael.example.tasavalta.Singleton.annaIlmentyma;

public class PaaValikko extends Fragment {

    public Muisti mMuisti;

    private LinearLayout[] mPalstat;
    private LinearLayout mJuuri;
    private FrameLayout mKehys;
    private TextView mBookmarks;
    private ToggleButton mLanguage;
    public ToggleButton mGraphics;
//    private Button mCreateBookmark;
//    private Button mHelp;
    private Button mDelete;
    private Button mBookmark1;
    private Button mBookmark2;
    private Button mBookmark3;
    private Button mBookmark4;
    private Button mBookmark5;
    private Button mBookmark6;
    private Button mBookmark7;
    private Button mBookmark8;
    private Button mBookmark9;
    private Button[] mTiedostot;
    private TextView m3D1;
    private TextView m3D2;
    private TextView m3D3;
    private TextView m3D4;
    private TextView m3D5;
    private TextView mStories1;
    private TextView mStories2;
    private TextView mStories3;
    private TextView mStories4;
    private TextView mStories5;
    private String[] mTiedostoNimet;
    private int mNimiLaskuri=0;
    private Singleton mSing;
    private View mNakyma;
    private boolean mMuutoksia=false;

    //Tämä luokka käsittelee päävalikon vakionappien sekä
    //mahdollisten kirjanmerkkien onClick tapahtumat
    class VakioNappi implements View.OnClickListener {

        @Override
        public void onClick(View v) {

            int tag=(int) v.getTag();
            switch (tag)
            {
                case 2001:
                    v.setBackgroundColor(0xffff0000);
                    break;
                case 2002:
                    ToggleButton siirto=(ToggleButton) v;
                    if (siirto.isChecked()) {

                        //käyttäjä pyytää hienostunutta grafiikkaa
                        mSing.mAktiviteetti1.eiValaisua=false;
                        if (mSing.mAktiviteetti1.mSurf!=null) {
                            mSing.mAktiviteetti1.mFragmentti3.avaaUudelleen();
                        }
                    } else {

                        //käyttäjä pyytää yksinkertaista grafiikkaa
                        mSing.mAktiviteetti1.eiValaisua=true;
                        if (mSing.mAktiviteetti1.mSurf!=null) {
                            mSing.mAktiviteetti1.mFragmentti3.avaaUudelleen();
                        }
                    }
                    break;
                    /*
                case 2003:
                    v.setBackgroundColor(0xffff0000);
                    break;
                case 2004:
                    v.setBackgroundColor(0xffff0000);
                    break;
                    */
                case 2005:
                    mMuisti.tuhoaKirjanMerkit();
                    break;
                case 2006:
                    mMuisti.lataaKirjanMerkkiAlku(0);
                    break;
                case 2007:
                    mMuisti.lataaKirjanMerkkiAlku(1);
                    break;
                case 2008:
                    mMuisti.lataaKirjanMerkkiAlku(2);
                    break;
                case 2009:
                    mMuisti.lataaKirjanMerkkiAlku(3);
                    break;
                case 2010:
                    mMuisti.lataaKirjanMerkkiAlku(4);
                    break;
                case 2011:
                    mMuisti.lataaKirjanMerkkiAlku(5);
                    break;
                case 2012:
                    mMuisti.lataaKirjanMerkkiAlku(6);
                    break;
                case 2013:
                    mMuisti.lataaKirjanMerkkiAlku(7);
                    break;
                case 2014:
                    mMuisti.lataaKirjanMerkkiAlku(8);
                    break;
            }
        }

    }

    //Tämä luokka käsittelee päävalikon CAD -nappuloiden onClick tapahtumat
    class CADNappi implements View.OnClickListener {

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

        @Override
        public void onClick(View v) {
            int tag=(Integer) v.getTag();
            mSing.mAktiviteetti1.mCADTiedosto=mTiedostoNimet[tag]+".cad";

            //Käyttäjä valitsi CADruudun esille.
            CADRuutu siirto=mSing.mAktiviteetti1.mFragmentti3;

            //jos orientaationa on landscape
            if (mSing.mAktiviteetti1.mVaakaKerroin==2) {
                asetaLandscape();
            }

            //Varmistetaan, että CADruutu on olemassa
            if (siirto != null) {

                //CADruutu on joko valmiiksi esillä, jolloin vain avataan CAD-tiedosto tai sitten
                //CADruutu pitää siirtää esille, joka samalla avaa CAD-tiedoston
                if (!siirto.isVisible()) {
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
    }

    //Tämä luokka käsittelee päävalikon teksti -nappuloiden onClick tapahtumat
    class TekstiNappi implements View.OnClickListener {

        private void asetaLandscape() {

            //ensiksi meidän pitää antaa puolet näyttöruudusta oikeanpuoleiselle fragmentille
            View siirto=mSing.mAktiviteetti1.mSisaltoOikea;
            LinearLayout.LayoutParams parametrit=
                    (LinearLayout.LayoutParams) siirto.getLayoutParams();
            parametrit.width=0;
            parametrit.weight=1;
            siirto.setLayoutParams(parametrit);

            //sitten siirrämme CADRuutu tai PaaValikko fragmentin oikealle
            if (mSing.mAktiviteetti1.mFragmentti3!=null) {
                siirto=mSing.mAktiviteetti1.mFragmentti3.annaNakyma();
            } else {
                siirto = mSing.mAktiviteetti1.mFragmentti1.annaNakyma();
            }
            ViewGroup parent1 = (ViewGroup) siirto.getParent();
            ViewGroup parent2=null;
            if (mSing.mAktiviteetti1.mFragmentti2!=null) {
                parent2=(ViewGroup) mSing.mAktiviteetti1.mFragmentti2.annaNakyma().getParent();
            }
            if (parent1!=null && parent2==null) {

                //Ensiksi CADruudun pitää mahdollisesti järjestää asiansa
                if (mSing.mAktiviteetti1.mFragmentti3!=null
                        && parent1.equals(
                                mSing.mAktiviteetti1.mFragmentti3.annaNakyma().getParent())) {
                    mSing.mAktiviteetti1.mSurf.sailytaSaie();
                }

                parent1.removeView(siirto);
                parent2=mSing.mAktiviteetti1.mSisaltoOikea;
                parent2.removeAllViews();
                parent2.addView(siirto);
            }
        }

        @Override
        public void onClick(View v) {
            int tag=(Integer) v.getTag();
            mSing.mAktiviteetti1.mTekstiTiedosto=mTiedostoNimet[tag]+".html";

            //Valittu teksti pitää avata alkupäästään
            mSing.mAktiviteetti1.mOnkoTekstiAvattu=false;

            //Käyttäjä valitsi tekstiruudun esille.
            TekstiRuutu siirto=mSing.mAktiviteetti1.mFragmentti2;

            //jos orientaationa on landscape
            if (mSing.mAktiviteetti1.mVaakaKerroin==2) {
                asetaLandscape();
            }

            //Varmistetaan, että tekstiruutu on olemassa
            if (siirto != null) {

                //Tekstiruutu on joko valmiiksi esillä, jolloin vain avataan HTML-sivu tai sitten
                //tekstiruutu pitää siirtää esille, jolloin HTML-sivu avautuu toista kautta
                if (null==siirto.annaNakyma().getParent()) {

                    //Ensiksi CADruudun pitää mahdollisesti järjestää asiansa
                    if (mSing.mAktiviteetti1.mFragmentti3!=null
                            && null!=mSing.mAktiviteetti1.mFragmentti3.annaNakyma().getParent()) {
                        mSing.mAktiviteetti1.mSurf.sailytaSaie();
                    }

                    mSing.mAktiviteetti1.mSisaltoVasen.removeAllViews();
                    mSing.mAktiviteetti1.mSisaltoVasen.addView(siirto.annaNakyma());

                    //Sitten avataan uusi HTML-sivu
                    siirto.avaaTeksti();
                } else {

                    //Sitten avataan uusi HTML-sivu
                    siirto.avaaTeksti();
                }
            } else {

                //Ensiksi CADruudun pitää mahdollisesti järjestää asiansa
                if (mSing.mAktiviteetti1.mFragmentti3!=null
                        && null!=mSing.mAktiviteetti1.mFragmentti3.annaNakyma().getParent()) {
                    mSing.mAktiviteetti1.mSurf.sailytaSaie();
                }

                mSing.mAktiviteetti1.mSisaltoVasen.removeAllViews();
                FragmentTransaction fragmentTransaction =
                        mSing.mAktiviteetti1.mFragmentManager.beginTransaction();
                mSing.mAktiviteetti1.mFragmentti2 = new TekstiRuutu();
                fragmentTransaction.add(R.id.sisaltoVasen, mSing.mAktiviteetti1.mFragmentti2,
                        "fragmentti2");
                fragmentTransaction.disallowAddToBackStack();
                fragmentTransaction.commit();
            }
        }
    }

    //Tätä metodia kutsutaan, kun PääValikko luokka luodaan, ja Tasavallan
    //suorituksen aikana sen pitäisi tapahtua vain kerran
    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        //pyydetään Singleton luokan ilmentymä ja annetaan Singletonille tämän luokan osoitin
        mSing=annaIlmentyma();
    }

    //Tätä metodia kutsutaan joka kerta, kun fragmentti luodaan näkyviin,
    //joko dynaamisesti tai xml-tiedostosta
    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup parent, Bundle savedInstanceState) {

        ObjectInputStream sisaan=null;

        //Aktivoidaan tiedot kovalevyltä, jos jotain on olemassa
        String polku = parent.getContext().getFilesDir().getAbsolutePath();
        File file=new File(polku, "/Muisti.txt");
        try {
            sisaan=new ObjectInputStream(
                    new FileInputStream(file));
            mMuisti=(Muisti)sisaan.readObject();
            sisaan.close();
        } catch (Exception e) {
            mMuisti=new Muisti(parent.getContext());
        }

        //kytketään Muisti luokkaan kuuntelija
        mMuisti.asetaMuistiKuuntelija(new Muisti.MuistiKuuntelija() {

            @Override
            public void onTekstiAvattu() {
                mMuisti.lataaKirjanMerkkiLoppu();
            }
        });

        // varmistetaan, että Andoroid laitteessa oleva Tasavallan versio on sama mikä Muisti
        // ilmentymässä. Muuten luodaan uusi muisti ilmentymä
        int versio;
        try {
            String packageVersion = parent.getContext().getPackageName();
            versio = parent.getContext().getPackageManager().getPackageInfo(packageVersion, 0).
                    versionCode;
        } catch (PackageManager.NameNotFoundException e) {
            versio = -1;
        }
        if ((mMuisti.annaVersio()!=versio && mMuisti.annaVersio()!=-1 && versio!=-1) ||
                (mMuisti.annaVersio()==-1 && versio!=-1)) {
            mMuisti=new Muisti(parent.getContext());
        }

        // xml tiedostosta luettavat jutut muodostavat sisällön dynaamiseen kehykseen
        mKehys=new FrameLayout(mSing.mAktiviteetti1.getApplicationContext());
        return inflater.inflate(R.layout.fragment_paavalikko, mKehys, false);
    }

    @Override
    public void onStop() {

        //Kun fragmentti1 otetaan väliaikaisesti pois käytöstä, tallennetaan Muisti-luokan
        // ilmentymä levylle.
        tallennaMuistiin();

        super.onStop();
    }

    @Override
    public void onConfigurationChanged(Configuration config) {

        //jos käyttäjä on avannut uusia kenttiä, laitetaan ne tarjolle päävalikkoon
        if (mMuutoksia) {
            mMuutoksia=false;
            mKehys.removeAllViews();

            //ennen näkymän uudelleen inflatoimista pitää kytkeä teema käyttöön
            mSing.mAktiviteetti1.getApplicationContext().getTheme().applyStyle(R.style.AppTheme,
                    true);

            LayoutInflater inflater=
                    LayoutInflater.from(mSing.mAktiviteetti1.getApplicationContext());
            View view=inflater.inflate(R.layout.fragment_paavalikko, mKehys, false);
            Bundle bundle=Bundle.EMPTY;

            onViewCreated(view, bundle);
        }

        super.onConfigurationChanged(config);
    }

    //Tätä kutsutaan pian onCreateView metodin jälkeen. Kaikkien asetusten
    //määrittäminen laitetaan tänne
    @Override
    public void onViewCreated(View view, Bundle savedInstanceState) {

        //Otetaan käyttöön xml-tiedostossa annettuja juttuja
        mJuuri=(LinearLayout) view.findViewById(R.id.paavalikkoSisalto);
        mNakyma=view;

        //Sitten horisontaaliseen juureen lisätään tarvittavat palstat
        mPalstat=new LinearLayout[mMuisti.annaPalstat()+1];
        for (int i=0; i<mMuisti.annaPalstat()+1; i++) {
            mPalstat[i]=new LinearLayout(view.getContext());
            mPalstat[i].setOrientation(LinearLayout.VERTICAL);
            mJuuri.addView(mPalstat[i]);
        }

        //Palstoihin lisätään vakiosisältö, eli otsikot ja painonapit.
        //Aloitetaan vasemmasta palstasta ja vakiosisältö ensin
        mBookmarks=new TextView(view.getContext());
        mBookmarks.setText(getResources().getString(R.string.vakio_kirja));
        mBookmarks.setTextSize(25);
        mPalstat[0].addView(mBookmarks);
        mLanguage=new ToggleButton(view.getContext());
        mLanguage.setTextOff(getResources().getString(R.string.vakio_kielis));
        mLanguage.setTextOn(getResources().getString(R.string.vakio_kielie));
        mLanguage.setChecked(true);
        mLanguage.setTag(2001);
        mLanguage.setOnClickListener(new VakioNappi());
        mPalstat[0].addView(mLanguage);
        mGraphics=new ToggleButton(view.getContext());
        mGraphics.setTextOff(getResources().getString(R.string.vakio_grafs));
        mGraphics.setTextOn(getResources().getString(R.string.vakio_grafe));
        mGraphics.setChecked(true);
        mGraphics.setTag(2002);
        mGraphics.setOnClickListener(new VakioNappi());
        mPalstat[0].addView(mGraphics);
        /*
        mCreateBookmark=new Button(view.getContext());
        mCreateBookmark.setText(getResources().getString(R.string.vakio_luo));
        mCreateBookmark.setTag(2003);
        mCreateBookmark.setOnClickListener(new VakioNappi());
        mPalstat[0].addView(mCreateBookmark);
        mHelp=new Button(view.getContext());
        mHelp.setText(getResources().getString(R.string.vakio_apua));
        mHelp.setTag(2004);
        mHelp.setOnClickListener(new VakioNappi());
        mPalstat[0].addView(mHelp);
        */
        mDelete=new Button(view.getContext());
        mDelete.setText(getResources().getString(R.string.vakio_tuhoa));
        mDelete.setTag(2005);
        mDelete.setOnClickListener(new VakioNappi());
        mPalstat[0].addView(mDelete);

        //Sitten lisätään sisältöä muisti-luokan mukaisesti. Kirjanmerkit:
        if (mMuisti.onkoKirjanMerkkia(0)) {
            mBookmark1=new Button(view.getContext());
            mBookmark1.setText(getResources().getString(R.string.vakio_kirja1));
            mBookmark1.setTag(2006);
            mBookmark1.setOnClickListener(new VakioNappi());
            mPalstat[0].addView(mBookmark1);
        }
        if (mMuisti.onkoKirjanMerkkia(1)) {
            mBookmark2=new Button(view.getContext());
            mBookmark2.setText(getResources().getString(R.string.vakio_kirja2));
            mBookmark2.setTag(2007);
            mBookmark2.setOnClickListener(new VakioNappi());
            mPalstat[0].addView(mBookmark2);
        }
        if (mMuisti.onkoKirjanMerkkia(2)) {
            mBookmark3=new Button(view.getContext());
            mBookmark3.setText(getResources().getString(R.string.vakio_kirja3));
            mBookmark3.setTag(2008);
            mBookmark3.setOnClickListener(new VakioNappi());
            mPalstat[0].addView(mBookmark3);
        }
        if (mMuisti.onkoKirjanMerkkia(3)) {
            mBookmark4=new Button(view.getContext());
            mBookmark4.setText(getResources().getString(R.string.vakio_kirja4));
            mBookmark4.setTag(2009);
            mBookmark4.setOnClickListener(new VakioNappi());
            mPalstat[0].addView(mBookmark4);
        }
        if (mMuisti.onkoKirjanMerkkia(4)) {
            mBookmark5=new Button(view.getContext());
            mBookmark5.setText(getResources().getString(R.string.vakio_kirja5));
            mBookmark5.setTag(2010);
            mBookmark5.setOnClickListener(new VakioNappi());
            mPalstat[0].addView(mBookmark5);
        }
        if (mMuisti.onkoKirjanMerkkia(5)) {
            mBookmark6=new Button(view.getContext());
            mBookmark6.setText(getResources().getString(R.string.vakio_kirja6));
            mBookmark6.setTag(2011);
            mBookmark6.setOnClickListener(new VakioNappi());
            mPalstat[0].addView(mBookmark6);
        }
        if (mMuisti.onkoKirjanMerkkia(6)) {
            mBookmark7=new Button(view.getContext());
            mBookmark7.setText(getResources().getString(R.string.vakio_kirja7));
            mBookmark7.setTag(2012);
            mBookmark7.setOnClickListener(new VakioNappi());
            mPalstat[0].addView(mBookmark7);
        }
        if (mMuisti.onkoKirjanMerkkia(7)) {
            mBookmark8=new Button(view.getContext());
            mBookmark8.setText(getResources().getString(R.string.vakio_kirja8));
            mBookmark8.setTag(2013);
            mBookmark8.setOnClickListener(new VakioNappi());
            mPalstat[0].addView(mBookmark8);
        }
        if (mMuisti.onkoKirjanMerkkia(8)) {
            mBookmark9=new Button(view.getContext());
            mBookmark9.setText(getResources().getString(R.string.vakio_kirja9));
            mBookmark9.setTag(2014);
            mBookmark9.setOnClickListener(new VakioNappi());
            mPalstat[0].addView(mBookmark9);
        }

        //alustetaan mTiedostoNimet, joita voi olla enintään 2000 riippumatta siitä,
        //ovatko teksti- vai CAD-tiedostoja
        mTiedostoNimet=new String[2000];

        //Siirrytään seuraaviin palstoihin vasemmalta oikealle. 3D-palstoja
        //on enintään 5
        boolean luotu=false;
        int index1=0, index2=0;
        mTiedostot=new Button[mMuisti.annaTiedostojenMaara()];
        for (int i=0; i<mMuisti.annaTiedostojenMaara(); i++) {
            if (mMuisti.annaValikkoSijainti(i)==1 && mMuisti.annaOnko3D(i)) {

                //jos palstaan ei ole vielä lisätty, lisätään otsikko
                if (luotu!=true) {
                    index1++;
                    m3D1=new TextView(view.getContext());
                    m3D1.setText(getResources().getString(R.string.vakio_3D_1));
                    m3D1.setTextSize(25);
                    mPalstat[index1].addView(m3D1);
                    luotu=true;
                }

                //luodaan sitten varsinainen valikkokohta
                if (mMuisti.annaOnkoNakyva(i)) {
                    mTiedostot[index2]=new Button(view.getContext());
                    mTiedostot[index2].setText(mMuisti.annaTiedostonNimi(i));
                    mTiedostot[index2].setEnabled(mMuisti.annaOnkoAsetettu(i));
                    mTiedostot[index2].setTag(mNimiLaskuri);
                    mTiedostoNimet[mNimiLaskuri]=mMuisti.annaTiedostonNimi(i);
                    mNimiLaskuri++;
                    mTiedostot[index2].setOnClickListener(new CADNappi());
                    mPalstat[index1].addView(mTiedostot[index2]);
                }
                index2++;
            }
        }
        luotu=false;
        for (int i=0; i<mMuisti.annaTiedostojenMaara(); i++) {
            if (mMuisti.annaValikkoSijainti(i)==2 && mMuisti.annaOnko3D(i)) {

                //jos palstaan ei ole vielä lisätty, lisätään otsikko
                if (luotu!=true) {
                    index1++;
                    m3D2=new TextView(view.getContext());
                    m3D2.setText(getResources().getString(R.string.vakio_3D_2));
                    m3D2.setTextSize(25);
                    mPalstat[index1].addView(m3D2);
                    luotu=true;
                }

                //luodaan sitten varsinainen valikkokohta
                if (mMuisti.annaOnkoNakyva(i)) {
                    mTiedostot[index2]=new Button(view.getContext());
                    mTiedostot[index2].setText(mMuisti.annaTiedostonNimi(i));
                    mTiedostot[index2].setEnabled(mMuisti.annaOnkoAsetettu(i));
                    mTiedostot[index2].setTag(mNimiLaskuri);
                    mTiedostoNimet[mNimiLaskuri]=mMuisti.annaTiedostonNimi(i);
                    mNimiLaskuri++;
                    mTiedostot[index2].setOnClickListener(new CADNappi());
                    mPalstat[index1].addView(mTiedostot[index2]);
                }
                index2++;
            }
        }
        luotu=false;
        for (int i=0; i<mMuisti.annaTiedostojenMaara(); i++) {
            if (mMuisti.annaValikkoSijainti(i)==3 && mMuisti.annaOnko3D(i)) {

                //jos palstaan ei ole vielä lisätty, lisätään otsikko
                if (luotu!=true) {
                    index1++;
                    m3D3=new TextView(view.getContext());
                    m3D3.setText(getResources().getString(R.string.vakio_3D_3));
                    m3D3.setTextSize(25);
                    mPalstat[index1].addView(m3D3);
                    luotu=true;
                }

                //luodaan sitten varsinainen valikkokohta
                if (mMuisti.annaOnkoNakyva(i)) {
                    mTiedostot[index2]=new Button(view.getContext());
                    mTiedostot[index2].setText(mMuisti.annaTiedostonNimi(i));
                    mTiedostot[index2].setEnabled(mMuisti.annaOnkoAsetettu(i));
                    mTiedostoNimet[mNimiLaskuri]=mMuisti.annaTiedostonNimi(i);
                    mTiedostot[index2].setTag(mNimiLaskuri);
                    mNimiLaskuri++;
                    mTiedostot[index2].setOnClickListener(new CADNappi());
                    mPalstat[index1].addView(mTiedostot[index2]);
                }
                index2++;
            }
        }
        luotu=false;
        for (int i=0; i<mMuisti.annaTiedostojenMaara(); i++) {
            if (mMuisti.annaValikkoSijainti(i)==4 && mMuisti.annaOnko3D(i)) {

                //jos palstaan ei ole vielä lisätty, lisätään otsikko
                if (luotu!=true) {
                    index1++;
                    m3D4=new TextView(view.getContext());
                    m3D4.setText(getResources().getString(R.string.vakio_3D_4));
                    m3D4.setTextSize(25);
                    mPalstat[index1].addView(m3D4);
                    luotu=true;
                }

                //luodaan sitten varsinainen valikkokohta
                if (mMuisti.annaOnkoNakyva(i)) {
                    mTiedostot[index2]=new Button(view.getContext());
                    mTiedostot[index2].setText(mMuisti.annaTiedostonNimi(i));
                    mTiedostot[index2].setEnabled(mMuisti.annaOnkoAsetettu(i));
                    mTiedostot[index2].setTag(mNimiLaskuri);
                    mTiedostoNimet[mNimiLaskuri]=mMuisti.annaTiedostonNimi(i);
                    mNimiLaskuri++;
                    mTiedostot[index2].setOnClickListener(new CADNappi());
                    mPalstat[index1].addView(mTiedostot[index2]);
                }
                index2++;
            }
        }
        luotu=false;
        for (int i=0; i<mMuisti.annaTiedostojenMaara(); i++) {
            if (mMuisti.annaValikkoSijainti(i)==5 && mMuisti.annaOnko3D(i)) {

                //jos palstaan ei ole vielä lisätty, lisätään otsikko
                if (luotu!=true) {
                    index1++;
                    m3D5=new TextView(view.getContext());
                    m3D5.setText(getResources().getString(R.string.vakio_3D_5));
                    m3D5.setTextSize(25);
                    mPalstat[index1].addView(m3D5);
                    luotu=true;
                }

                //luodaan sitten varsinainen valikkokohta
                if (mMuisti.annaOnkoNakyva(i)) {
                    mTiedostot[index2]=new Button(view.getContext());
                    mTiedostot[index2].setText(mMuisti.annaTiedostonNimi(i));
                    mTiedostot[index2].setEnabled(mMuisti.annaOnkoAsetettu(i));
                    mTiedostot[index2].setTag(mNimiLaskuri);
                    mTiedostoNimet[mNimiLaskuri]=mMuisti.annaTiedostonNimi(i);
                    mNimiLaskuri++;
                    mTiedostot[index2].setOnClickListener(new CADNappi());
                    mPalstat[index1].addView(mTiedostot[index2]);
                }
                index2++;
            }
        }
        luotu=false;

        //Tekstipalstat tulevat vasemmalta oikealle 3D-palstojen jälkeen
        for (int i=0; i<mMuisti.annaTiedostojenMaara(); i++) {
            if (mMuisti.annaValikkoSijainti(i)==1 && !mMuisti.annaOnko3D(i)) {

                //jos palstaan ei ole vielä lisätty, lisätään otsikko
                if (luotu!=true) {
                    index1++;
                    mStories1=new TextView(view.getContext());
                    mStories1.setText(getResources().getString(R.string.vakio_stories1));
                    mStories1.setTextSize(25);
                    mPalstat[index1].addView(mStories1);
                    luotu=true;
                }

                //luodaan sitten varsinainen valikkokohta
                if (mMuisti.annaOnkoNakyva(i)) {
                    mTiedostot[index2]=new Button(view.getContext());
                    mTiedostot[index2].setText(mMuisti.annaTiedostonNimi(i));
                    mTiedostot[index2].setEnabled(mMuisti.annaOnkoAsetettu(i));
                    mTiedostot[index2].setTag(mNimiLaskuri);
                    mTiedostoNimet[mNimiLaskuri]=mMuisti.annaTiedostonNimi(i);
                    mNimiLaskuri++;
                    mTiedostot[index2].setOnClickListener(new TekstiNappi());
                    mPalstat[index1].addView(mTiedostot[index2]);
                }
                index2++;
            }
        }
        luotu=false;
        for (int i=0; i<mMuisti.annaTiedostojenMaara(); i++) {
            if (mMuisti.annaValikkoSijainti(i)==2 && !mMuisti.annaOnko3D(i)) {

                //jos palstaan ei ole vielä lisätty, lisätään otsikko
                if (luotu!=true) {
                    index1++;
                    mStories2=new TextView(view.getContext());
                    mStories2.setText(getResources().getString(R.string.vakio_stories2));
                    mStories2.setTextSize(25);
                    mPalstat[index1].addView(mStories2);
                    luotu=true;
                }

                //luodaan sitten varsinainen valikkokohta
                if (mMuisti.annaOnkoNakyva(i)) {
                    mTiedostot[index2]=new Button(view.getContext());
                    mTiedostot[index2].setText(mMuisti.annaTiedostonNimi(i));
                    mTiedostot[index2].setEnabled(mMuisti.annaOnkoAsetettu(i));
                    mTiedostot[index2].setTag(mNimiLaskuri);
                    mTiedostoNimet[mNimiLaskuri]=mMuisti.annaTiedostonNimi(i);
                    mNimiLaskuri++;
                    mTiedostot[index2].setOnClickListener(new TekstiNappi());
                    mPalstat[index1].addView(mTiedostot[index2]);
                }
                index2++;
            }
        }
        luotu=false;
        for (int i=0; i<mMuisti.annaTiedostojenMaara(); i++) {
            if (mMuisti.annaValikkoSijainti(i)==3 && !mMuisti.annaOnko3D(i)) {

                //jos palstaan ei ole vielä lisätty, lisätään otsikko
                if (luotu!=true) {
                    index1++;
                    mStories3=new TextView(view.getContext());
                    mStories3.setText(getResources().getString(R.string.vakio_stories3));
                    mStories3.setTextSize(25);
                    mPalstat[index1].addView(mStories3);
                    luotu=true;
                }

                //luodaan sitten varsinainen valikkokohta
                if (mMuisti.annaOnkoNakyva(i)) {
                    mTiedostot[index2]=new Button(view.getContext());
                    mTiedostot[index2].setText(mMuisti.annaTiedostonNimi(i));
                    mTiedostot[index2].setEnabled(mMuisti.annaOnkoAsetettu(i));
                    mTiedostot[index2].setTag(mNimiLaskuri);
                    mTiedostoNimet[mNimiLaskuri]=mMuisti.annaTiedostonNimi(i);
                    mNimiLaskuri++;
                    mTiedostot[index2].setOnClickListener(new TekstiNappi());
                    mPalstat[index1].addView(mTiedostot[index2]);
                }
                index2++;
            }
        }
        luotu=false;
        for (int i=0; i<mMuisti.annaTiedostojenMaara(); i++) {
            if (mMuisti.annaValikkoSijainti(i)==4 && !mMuisti.annaOnko3D(i)) {

                //jos palstaan ei ole vielä lisätty, lisätään otsikko
                if (luotu!=true) {
                    index1++;
                    mStories4=new TextView(view.getContext());
                    mStories4.setText(getResources().getString(R.string.vakio_stories4));
                    mStories4.setTextSize(25);
                    mPalstat[index1].addView(mStories4);
                    luotu=true;
                }

                //luodaan sitten varsinainen valikkokohta
                if (mMuisti.annaOnkoNakyva(i)) {
                    mTiedostot[index2]=new Button(view.getContext());
                    mTiedostot[index2].setText(mMuisti.annaTiedostonNimi(i));
                    mTiedostot[index2].setEnabled(mMuisti.annaOnkoAsetettu(i));
                    mTiedostot[index2].setTag(mNimiLaskuri);
                    mTiedostoNimet[mNimiLaskuri]=mMuisti.annaTiedostonNimi(i);
                    mNimiLaskuri++;
                    mTiedostot[index2].setOnClickListener(new TekstiNappi());
                    mPalstat[index1].addView(mTiedostot[index2]);
                }
                index2++;
            }
        }
        luotu=false;
        for (int i=0; i<mMuisti.annaTiedostojenMaara(); i++) {
            if (mMuisti.annaValikkoSijainti(i)==5 && !mMuisti.annaOnko3D(i)) {

                //jos palstaan ei ole vielä lisätty, lisätään otsikko
                if (luotu!=true) {
                    index1++;
                    mStories5=new TextView(view.getContext());
                    mStories5.setText(getResources().getString(R.string.vakio_stories5));
                    mStories5.setTextSize(25);
                    mPalstat[index1].addView(mStories5);
                    luotu=true;
                }

                //luodaan sitten varsinainen valikkokohta
                if (mMuisti.annaOnkoNakyva(i)) {
                    mTiedostot[index2]=new Button(view.getContext());
                    mTiedostot[index2].setText(mMuisti.annaTiedostonNimi(i));
                    mTiedostot[index2].setEnabled(mMuisti.annaOnkoAsetettu(i));
                    mTiedostot[index2].setTag(mNimiLaskuri);
                    mTiedostoNimet[mNimiLaskuri]=mMuisti.annaTiedostonNimi(i);
                    mNimiLaskuri++;
                    mTiedostot[index2].setOnClickListener(new TekstiNappi());
                    mPalstat[index1].addView(mTiedostot[index2]);
                }
                index2++;
            }
        }
        luotu=false;
    }

    public View annaNakyma() {
        return mNakyma;
    }

    public void tallennaMuistiin() {

        try {
            String polku =
                    mSing.mAktiviteetti1.getApplicationContext().getFilesDir().getAbsolutePath();
            File tiedosto=new File(polku + "/Muisti.txt");
            ObjectOutputStream ulos=new ObjectOutputStream(
                    new FileOutputStream(tiedosto));
            ulos.writeObject(mMuisti);
            ulos.close();
        } catch (Exception e) {
            //tyhjä
        }

    }

    public void tuoEsille(String nimi, char tyyppi) {

        //nimestä pitää erotella mahdolliset risuaidat ja tiedostopäätteet pois
        int ensimmainenHashtac=nimi.indexOf("#");
        int toinenHashtac=nimi.indexOf("#", ensimmainenHashtac+1);
        String tiedosto=nimi;
        if (ensimmainenHashtac!=-1) {
            tiedosto=nimi.substring(ensimmainenHashtac+1);
        }
        if (toinenHashtac!=-1) {
            tiedosto=tiedosto.substring(toinenHashtac-ensimmainenHashtac+1);
        }
        int piste=tiedosto.indexOf(".");
        if (piste!=-1) {
            tiedosto=tiedosto.substring(0, piste);
        }

        switch (tyyppi) {

            case 'T': {

                //asetetaan samanniminen Tekstitiedosto valittavaksi Muisti-luokassa
                for (int i=0; i<mMuisti.annaTiedostojenMaara(); i++) {
                    if (mMuisti.annaTiedostonNimi(i).equals(tiedosto) && !mMuisti.annaOnko3D(i)) {
                        mMuisti.asetaAsetetuksi(i);
                        mMuisti.asetaNakyvaksi(i);
                        mMuutoksia=true;
                    }
                }

                break;
            }

            case 'C': {

                //asetetaan samanniminen CADtiedosto valittavaksi Muisti-luokassa
                for (int i=0; i<mMuisti.annaTiedostojenMaara(); i++) {
                    if (mMuisti.annaTiedostonNimi(i).equals(tiedosto) && mMuisti.annaOnko3D(i)) {
                        mMuisti.asetaAsetetuksi(i);
                        mMuisti.asetaNakyvaksi(i);
                        mMuutoksia=true;
                    }
                }

                break;
            }

        }
    }

    public void asetaOptio(int optio) {
        if (optio==1 || optio==2) {
            mMuisti.asetaOptio(optio);
        }
    }

    public int annaOptio() {
        return mMuisti.annaOptio();
    }
}




