package com.mannonen.mikael.example.tasavalta;

import android.Manifest;
import android.app.ActivityManager;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.pm.PackageManager;
import android.content.res.AssetManager;
import android.content.res.Configuration;
import android.content.res.Resources;
import android.graphics.Point;
import android.os.Build;
import android.os.Bundle;
import android.support.v7.app.AppCompatActivity;
import android.util.DisplayMetrics;
import android.view.Display;
import android.view.Menu;
import android.view.MenuItem;
import android.support.v4.app.*;
import android.support.v7.widget.*;
import android.view.View;
import android.view.ViewGroup;
import android.view.Window;
import android.view.WindowManager;
import android.widget.LinearLayout;
import android.widget.RadioButton;
import android.widget.Toast;

import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.Map;

import static com.mannonen.mikael.example.tasavalta.Singleton.annaIlmentyma;

public class Aktiviteetti1 extends AppCompatActivity {

    public PaaValikko mFragmentti1;
    public TekstiRuutu mFragmentti2;
    public CADRuutu mFragmentti3;
    public About mFragmentti4;
    public Menu mMenu;
    public JNIRenki mRenki;
    public LinkkiListaManager mLinkkiListaManager;
    public OpenGLRenderoija mOpenGLRenderoija;

    public Toolbar mMenuPalkki;
    public ViewGroup mSisaltoVasen;
    public ViewGroup mSisaltoOikea;
    public Resources.Theme mTeema;
    public String mTekstiTiedosto;
    public String mCADTiedosto;
    public OpenGLView mSurf;
    public FragmentManager mFragmentManager;

    public Point mPoint;
    public WindowManager mWm;
    public DisplayMetrics mDm;
    public Window mWin;
    public boolean mOnkoTekstiAvattu=false;
    public boolean mOnkoCADAvattu=false;
    public int mVaakaKerroin;
    public int mNappiKorkeus;
    public float mTulostusResoluutio;
    public boolean eiValaisua=false;
    public int mViimeinen=0;
    public boolean mLowMemory=false;

    private Singleton mSing;
    private TaustaKysely mTaustaKysely=null;
    private ResoluutioLista mResoluutioLista=null;
    final private int REQUEST_CODE_ASK_MULTIPLE_PERMISSIONS = 124;

    private void tallennaAsetukset() {
        SharedPreferences sp=getPreferences(MODE_PRIVATE);
        SharedPreferences.Editor editor=sp.edit();
        editor.putString("TekstiTiedosto", mTekstiTiedosto);
        editor.putString("CADTiedosto", mCADTiedosto);
        editor.putBoolean("menu0", mMenu.getItem(0).isEnabled());
        editor.putBoolean("menu1", mMenu.getItem(1).isEnabled());
        editor.putBoolean("menu2", mMenu.getItem(2).isEnabled());
        editor.putBoolean("menu3", mMenu.getItem(3).isEnabled());
        editor.putBoolean("menu4", mMenu.getItem(4).isEnabled());
        editor.putBoolean("menu5", mMenu.getItem(5).isEnabled());
        editor.commit();
    }

    private void lataaAsetukset() {
        SharedPreferences sp=getPreferences(MODE_PRIVATE);
        mTekstiTiedosto=sp.getString("TekstiTiedosto", null);
        mCADTiedosto=sp.getString("CADTiedosto", null);
        mMenu.getItem(0).setEnabled(sp.getBoolean("menu0", true));
        if (mTekstiTiedosto!=null && !mTekstiTiedosto.isEmpty()) {
            mMenu.getItem(1).setEnabled(sp.getBoolean("menu1", false));
            mMenu.getItem(4).setEnabled(sp.getBoolean("menu4", false));
        } else {
            mMenu.getItem(1).setEnabled(false);
            mMenu.getItem(4).setEnabled(false);
        }
        if (mCADTiedosto!=null && !mCADTiedosto.isEmpty()) {
            mMenu.getItem(2).setEnabled(sp.getBoolean("menu2", false));
            mMenu.getItem(5).setEnabled(sp.getBoolean("menu5", false));
        } else {
            mMenu.getItem(2).setEnabled(false);
            mMenu.getItem(5).setEnabled(false);
        }
        mMenu.getItem(3).setEnabled(sp.getBoolean("menu3", true));
    }

    //tällä funktiolla kopioidaan assets-kansion sisältö haluttuun sijaintiin
    private void kopioiTiedostotJaKansiot(String polku, String sijainti) {
        AssetManager assetManager = getApplicationContext().getAssets();
        String assets[] = null;

        try {
            assets = assetManager.list(polku);
            if (assets.length == 0) {

                //kyseessä täytyy olla tiedosto, joka pitää kopioida. Vaihtoehtoisesti
                //kyseessä voi myöskin olla kelvoton tiedostopolku
                kopioiTiedosto(polku, sijainti);
            } else {

                //kyseessä on kansio, jonka sisältö täytyy yksitellen kopioida.
                File kansio=new File(sijainti);

                //selvitetään, onko kyseinen kansio luotu aiemmin, tai onko se pelkkä
                //sijainti, jolloin ei tehdä mitään
                if (!kansio.exists() || sijainti.endsWith("com.mannonen.mikael.example.tasavalta/files")) {
                    kansio.mkdir();

                        //jos kyseisen kansion luonti onnistui, kopioidaan sinne kansion sisältö...
                        for (int i=0; i<assets.length; i++) {

                            //...ilman näitä kolmea kansiota/tiedostoa
                            if (!assets[i].startsWith("images") && !assets[i].startsWith("sounds")
                                    && !assets[i].startsWith("webkit")) {
                                if (sijainti.length()>0 &&
                                        sijainti.charAt(sijainti.length()-1)!='/') {
                                    sijainti=sijainti + "/";
                                }
                                if (polku.length()>0 && polku.charAt(polku.length()-1)!='/') {
                                    polku=polku + "/";
                                }
                                kopioiTiedostotJaKansiot(polku + assets[i],
                                        sijainti + assets[i]);
                            }
                        }



                }
            }
        } catch (IOException ex) {
            Toast toast=Toast.makeText(getApplicationContext(),
                    "Loading the contents of Tasavalta failed.", Toast.LENGTH_LONG);
            toast.show();
        }
    }

    //tämä funktio kopioi yhden tiedoston täältä (assets) tanne (vapaasti valittava)
    private void kopioiTiedosto(String taalta, String tanne) {
        AssetManager assetManager = getApplicationContext().getAssets();

        InputStream in = null;
        OutputStream out = null;
        try {
            in = assetManager.open(taalta);
            out = new FileOutputStream(tanne);

            byte[] buffer = new byte[1024];
            int read;
            while ((read = in.read(buffer)) != -1) {
                out.write(buffer, 0, read);
            }
            in.close();
            in = null;
            out.flush();
            out.close();
            out = null;
        } catch (Exception e) {

            //tämä virhe voi johtua tyhjästä kansiosta
            Toast toast=Toast.makeText(getApplicationContext(),
                    "Couldn't create a file.", Toast.LENGTH_LONG);
            toast.show();
        }
    }

    //tämä funktio ilmoittaa käyttäjälle Kirjasto2:n käyttöönoton epäonnistumisesta.
    //Asia kuuluu TekstiRuutu luokalle, mutta koska konstruktori ei pysty näyttämään
    //toastia, on alla oleva funktio välttämätön
    public void Kirjasto2Epaonnistui() {
        Toast toast=Toast.makeText(getApplicationContext(), "Failed to load library Kirjasto2.",
                Toast.LENGTH_LONG);
        toast.show();
    }

    //sama kuin edellä Kirjasto1:lle
    public void Kirjasto1Epaonnistui() {
        Toast toast=Toast.makeText(getApplicationContext(), "Failed to load library Kirjasto1.",
                Toast.LENGTH_LONG);
        toast.show();
    }

    @Override
    public void onPause() {

        //Tallennetaan aktiviteetin asetuksia helpottamaan aktiviteetin uudelleen
        //käynnistämistä silloin, kun esimerkiksi ohjelmasta poistutuaan back painiketta painamalla
        tallennaAsetukset();
        super.onPause();
        if (mFragmentti3!=null) {
            mSurf.onPause();
        }
    }

    @Override
    public void onResume() {
        super.onResume();
        if (mFragmentti3!=null) {
            mSurf.onResume();
        }
    }

    @Override
    public void onSaveInstanceState(Bundle bundle) {

        //Tallennetaan aktiviteetin asetuksia helpottamaan aktiviteetin uudelleen
        //käynnistämistä silloin, kun esimerkiksi näyttöä kierretään
        bundle.putString("TekstiTiedosto", mTekstiTiedosto);
        bundle.putString("CADTiedosto", mCADTiedosto);
        bundle.putByte("menu0", (byte) (mMenu.getItem(0).isEnabled() ? 1 : 0));
        bundle.putByte("menu1", (byte) (mMenu.getItem(1).isEnabled() ? 1 : 0));
        bundle.putByte("menu2", (byte) (mMenu.getItem(2).isEnabled() ? 1 : 0));
        bundle.putByte("menu3", (byte) (mMenu.getItem(3).isEnabled() ? 1 : 0));
        bundle.putByte("menu4", (byte) (mMenu.getItem(4).isEnabled() ? 1 : 0));
        bundle.putByte("menu5", (byte) (mMenu.getItem(5).isEnabled() ? 1 : 0));
        super.onSaveInstanceState(bundle);
    }

    //aktiviteetin konstruktori
    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_aktiviteetti1);

        //orientaatio pitää merkitä muistiin ensimmäisenä, ja siksi se
        //on tehtävä aktiviteetissa, josta fragmentit voivat hakea tiedon
        int orientaatio=getResources().getConfiguration().orientation;
        if (orientaatio == Configuration.ORIENTATION_LANDSCAPE) {
            mVaakaKerroin = 2;
        } else {
            mVaakaKerroin = 1;
        }

        //otetaan myöhempää käyttöä varten talteen sisältöVasen, sisältöOikea ja teema
        mSisaltoOikea=findViewById(R.id.sisaltoOikea);
        mSisaltoVasen=findViewById(R.id.sisaltoVasen);
        mTeema=getTheme();

        //pyydetään Singleton luokan ilmentymä ja annetaan Singletonille tämän luokan osoitin
        mSing=annaIlmentyma();
        mSing.asetaAktiviteetti1(this);

        //lisätään menupalkki, jollei sitä ole jo lisätty
        mMenuPalkki = (Toolbar) findViewById(R.id.menuPalkki);
        if (mMenuPalkki!=null) {
            setSupportActionBar(mMenuPalkki);
        }

        //Ladataan Tasavalta-sovelluksen APK-paketin assets-kansion tiedostot ohjelman
        //käyttämään sijaintiin, eli /data/data/com.mannonen.tasavalta/files/
        String lahtoPolku="";
        String saapumisPolku=getApplicationContext().getFilesDir().getAbsolutePath();
        kopioiTiedostotJaKansiot(lahtoPolku, saapumisPolku);

        //päävalikko on fragmentissa, joka pitää lisätä aktiviteettiin heti,
        //jollei sitä ole jo lisätty (päävalikon luonnin pitäisi tapahtua vain kerran
        //ohjelmaa käynnistettäessä, muulloin pitäisi olla käytössä onConfigurationChanged)
        if (savedInstanceState==null) {
            mFragmentManager = getSupportFragmentManager();
            FragmentTransaction fragmentTransaction = mFragmentManager.beginTransaction();

            mFragmentti1 = new PaaValikko();
            fragmentTransaction.add(R.id.sisaltoVasen, mFragmentti1, "fragmentti1");
            fragmentTransaction.commit();
        } else {

            //Palautetaan mahdolliset aktiviteetin aiemmat asetukset
            mTekstiTiedosto = savedInstanceState.getString("TekstiTiedosto");
            mCADTiedosto = savedInstanceState.getString("CADTiedosto");
            mMenu.getItem(0).setEnabled(1 == savedInstanceState.getByte("menu0"));
            if (mTekstiTiedosto!=null && !mTekstiTiedosto.isEmpty()) {
                mMenu.getItem(1).setEnabled(1 == savedInstanceState.getByte("menu1"));
                mMenu.getItem(4).setEnabled(1 == savedInstanceState.getByte("menu4"));
            } else {
                mMenu.getItem(1).setEnabled(false);
                mMenu.getItem(4).setEnabled(false);
            }
            if (mCADTiedosto!=null && !mCADTiedosto.isEmpty()) {
                mMenu.getItem(2).setEnabled(1 == savedInstanceState.getByte("menu2"));
                mMenu.getItem(5).setEnabled(1 == savedInstanceState.getByte("menu5"));
            } else {
                mMenu.getItem(2).setEnabled(false);
                mMenu.getItem(5).setEnabled(false);
            }
            mMenu.getItem(3).setEnabled(1 == savedInstanceState.getByte("menu3"));
        }

        //Näytön koon selvittäminen pikseleissä orientaatio huomioon ottaen
        mWm=(WindowManager) getApplicationContext().getSystemService(Context.WINDOW_SERVICE);
        Display display=mWm.getDefaultDisplay();
        mPoint=new Point();
        display.getSize(mPoint);
        if (mVaakaKerroin==2) {
            int siirto=mPoint.x;
            mPoint.x=mPoint.y;
            mPoint.y=siirto;
        }
/*
        //nämä luvat pitää pyytää tässä, jotta OpenGL toimisi varmasti
        if (Build.VERSION.SDK_INT >= 23) {
            callMultiplePermissions();
        }
*/
        //tiedustellaan, paljonko Android-laite suostuu antamaan muistia käyttöömme
        ActivityManager.MemoryInfo info=annaMuistiTiedot();
        if (info.lowMemory) {
            mLowMemory=true;
        }

        //Näytön muiden tietojen selvitys
        mDm=getResources().getDisplayMetrics();

        //Tasavallan ohjelmaikkunan tietojen selvitys
        mWin=this.getWindow();

        //Valikkopalkin korkeuden määritys
        mNappiKorkeus=(int) getResources().getDimension(R.dimen.nappirivi_korkeus);

        //muita alkuasetuksia
        mOnkoTekstiAvattu=false;
        mOnkoCADAvattu=false;
        mTaustaKysely = new TaustaKysely();
        mResoluutioLista = new ResoluutioLista();
    }

    @Override
    public void onBackPressed() {
        moveTaskToBack(true);
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {

        //otetaan menu instanssi talteen
        mMenu=menu;

        // Inflate the menu; this adds items to the action bar if it is present.
        getMenuInflater().inflate(R.menu.menu_aktiviteetti1, menu);

        //Tämä lataa mahdolliset aktiviteetin aiemmat asetukset, jos savedInstanceState oli null
        lataaAsetukset();

        return true;
    }

    //Tällä funktiolla käsitellään näytön orientaatiomuutosten vaatimat toimenpiteet
    @Override
    public void onConfigurationChanged(Configuration newConfig) {

        //konfiguraatiomuutoksessa on hyvä tallentaa tämänhetkinen tilanne muistiin
        mFragmentti1.tallennaMuistiin();

        //orientaatiomuutos pitää merkitä muistiin ensimmäisenä, ja siksi se
        //on tehtävä aktiviteetissa, josta fragmentit voivat hakea tiedon
        if (newConfig.orientation == Configuration.ORIENTATION_LANDSCAPE) {
            mVaakaKerroin = 2;
        } else {
            mVaakaKerroin = 1;
        }

        super.onConfigurationChanged(newConfig);

        View siirto;
        ViewGroup parent;
        ViewGroup vasen=mSisaltoVasen;
        ViewGroup oikea=mSisaltoOikea;

        //TekstiRuudun pitää tallentaa datansa. Jotta se tapahtuisi aina konfiguraatio-
        //muutoksessa, pitää funktiota kutsua täältä käsin
        if (mFragmentti2!=null) {
            mFragmentti2.tallennaAsetukset();
        }

        //myös CADruudun pitää järjestää asiansa
        if (mFragmentti3!=null) {
            mSurf.sailytaSaie();
        }

        //jos orientaatioksi vaihtuu landscape, kaksi fragmenttia on pääsääntöisesti rinnakkain
        if (newConfig.orientation == Configuration.ORIENTATION_LANDSCAPE) {
            boolean[] lista=new boolean[4];
            int nakyva=0;

            //selvitetään, mitkä fragmentit ovat tällä hetkellä luotuina ja
            //mikä niistä on näkyvä
            if (mFragmentti1!=null) {
                lista[0]=true;
                parent=(ViewGroup) mFragmentti1.annaNakyma().getParent();
                if (parent!=null && parent.equals(vasen)) nakyva=1;
            }
            if (mFragmentti2!=null) {
                lista[1]=true;
                parent=(ViewGroup) mFragmentti2.annaNakyma().getParent();
                if (parent!=null && parent.equals(vasen)) nakyva=2;
            }
            if (mFragmentti3!=null) {
                lista[2]=true;
                parent=(ViewGroup) mFragmentti3.annaNakyma().getParent();
                if (parent!=null && parent.equals(vasen)) nakyva=3;
            }
            if (mFragmentti4!=null) {
                lista[3]=true;
                parent=(ViewGroup) mFragmentti4.annaNakyma().getParent();
                if (parent!=null && parent.equals(vasen)) nakyva=4;
            }

            //sitten siirretään kaksi fragmenttia horizontal layoutin sisälle
            if (lista[0] && nakyva!=1) {

                //ensiksi meidän pitää antaa puolet näyttöruudusta oikeanpuoleiselle fragmentille
                LinearLayout.LayoutParams parametrit=
                        (LinearLayout.LayoutParams) oikea.getLayoutParams();
                parametrit.width=0;
                parametrit.weight=1;
                oikea.setLayoutParams(parametrit);

                //sitten siirrämme vasemalla näkyvän fragmentin oikealle
                switch (nakyva) {
                    case 2:
                        siirto=mFragmentti2.annaNakyma();
                        parent=(ViewGroup) siirto.getParent();
                        if (parent!=null && parent.equals(vasen)) {
                            parent.removeView(siirto);
                            oikea.removeAllViews();
                            oikea.addView(siirto);
                        }
                        break;
                    case 3:
                        siirto=mFragmentti3.annaNakyma();
                        parent=(ViewGroup) siirto.getParent();
                        if (parent!=null && parent.equals(vasen)) {
                            parent.removeView(siirto);
                            oikea.removeAllViews();
                            oikea.addView(siirto);
                        }
                        break;
                    case 4:
                        siirto=mFragmentti4.annaNakyma();
                        parent=(ViewGroup) siirto.getParent();
                        if (parent!=null && parent.equals(vasen)) {
                            parent.removeView(siirto);
                            oikea.removeAllViews();
                            oikea.addView(siirto);
                        }
                        break;
                }

                //lopuksi vasemmalle puolelle otetaan backstacista aiemmin esillä ollut fragmentti
                siirto=mFragmentti1.annaNakyma();
                parent = (ViewGroup) siirto.getParent();
                if (parent!=null) parent.removeView(siirto);
                vasen.removeAllViews();
                vasen.addView(siirto);

                return;
            }

            if (lista[1] && nakyva!=2) {

                //ensiksi meidän pitää antaa puolet näyttöruudusta oikeanpuoleiselle fragmentille
                LinearLayout.LayoutParams parametrit=
                        (LinearLayout.LayoutParams) oikea.getLayoutParams();
                parametrit.width=0;
                parametrit.weight=1;
                oikea.setLayoutParams(parametrit);

                //sitten siirrämme vasemalla näkyvän fragmentin oikealle
                switch (nakyva) {
                    case 1:
                        siirto=mFragmentti1.annaNakyma();
                        parent=(ViewGroup) siirto.getParent();
                        if (parent!=null && parent.equals(vasen)) {
                            parent.removeView(siirto);
                            oikea.removeAllViews();
                            oikea.addView(siirto);
                        }
                        break;
                    case 3:
                        siirto=mFragmentti3.annaNakyma();
                        parent=(ViewGroup) siirto.getParent();
                        if (parent!=null && parent.equals(vasen)) {
                            parent.removeView(siirto);
                            oikea.removeAllViews();
                            oikea.addView(siirto);
                        }
                        break;
                    case 4:
                        siirto=mFragmentti4.annaNakyma();
                        parent=(ViewGroup) siirto.getParent();
                        if (parent!=null && parent.equals(vasen)) {
                            parent.removeView(siirto);
                            oikea.removeAllViews();
                            oikea.addView(siirto);
                        }
                        break;
                }

                //lopuksi vasemmalle puolelle otetaan backstacista aiemmin esillä ollut fragmentti
                siirto=mFragmentti2.annaNakyma();
                parent = (ViewGroup) siirto.getParent();
                if (parent!=null) parent.removeView(siirto);
                vasen.removeAllViews();
                vasen.addView(siirto);

                return;
            }

            if (lista[2] && nakyva!=3) {

                //ensiksi meidän pitää antaa puolet näyttöruudusta oikeanpuoleiselle fragmentille
                LinearLayout.LayoutParams parametrit=
                        (LinearLayout.LayoutParams) oikea.getLayoutParams();
                parametrit.width=0;
                parametrit.weight=1;
                oikea.setLayoutParams(parametrit);

                //sitten siirrämme vasemalla näkyvän fragmentin oikealla
                switch (nakyva) {
                    case 1:
                        siirto=mFragmentti1.annaNakyma();
                        parent=(ViewGroup) siirto.getParent();
                        if (parent!=null && parent.equals(vasen)) {
                            parent.removeView(siirto);
                            oikea.removeAllViews();
                            oikea.addView(siirto);
                        }
                        break;
                    case 2:
                        siirto=mFragmentti2.annaNakyma();
                        parent=(ViewGroup) siirto.getParent();
                        if (parent!=null && parent.equals(vasen)) {
                            parent.removeView(siirto);
                            oikea.removeAllViews();
                            oikea.addView(siirto);
                        }
                        break;
                    case 4:
                        siirto=mFragmentti4.annaNakyma();
                        parent=(ViewGroup) siirto.getParent();
                        if (parent!=null && parent.equals(vasen)) {
                            parent.removeView(siirto);
                            oikea.removeAllViews();
                            oikea.addView(siirto);
                        }
                        break;
                }

                //lopuksi vasemmalle puolelle otetaan backstacista aiemmin esillä ollut fragmentti
                siirto=mFragmentti3.annaNakyma();
                parent = (ViewGroup) siirto.getParent();
                if (parent!=null) parent.removeView(siirto);
                vasen.removeAllViews();
                vasen.addView(siirto);

                return;
            }

            if (lista[3] && nakyva!=4) {

                //ensiksi meidän pitää antaa puolet näyttöruudusta oikeanpuoleiselle fragmentille
                LinearLayout.LayoutParams parametrit=
                        (LinearLayout.LayoutParams) oikea.getLayoutParams();
                parametrit.width=0;
                parametrit.weight=1;
                oikea.setLayoutParams(parametrit);

                //sitten siirrämme vasemalla näkyvän fragmentin oikealle
                switch (nakyva) {
                    case 1:
                        siirto=mFragmentti1.annaNakyma();
                        parent=(ViewGroup) siirto.getParent();
                        if (parent!=null && parent.equals(vasen)) {
                            parent.removeView(siirto);
                            oikea.removeAllViews();
                            oikea.addView(siirto);
                        }
                        break;
                    case 2:
                        siirto=mFragmentti2.annaNakyma();
                        parent=(ViewGroup) siirto.getParent();
                        if (parent!=null && parent.equals(vasen)) {
                            parent.removeView(siirto);
                            oikea.removeAllViews();
                            oikea.addView(siirto);
                        }
                        break;
                    case 3:
                        siirto=mFragmentti3.annaNakyma();
                        parent=(ViewGroup) siirto.getParent();
                        if (parent!=null && parent.equals(vasen)) {
                            parent.removeView(siirto);
                            oikea.removeAllViews();
                            oikea.addView(siirto);
                        }
                        break;
                }

                //lopuksi vasemmalle puolelle otetaan aiemmin esillä ollut fragmentti
                siirto=mFragmentti4.annaNakyma();
                parent = (ViewGroup) siirto.getParent();
                if (parent!=null) parent.removeView(siirto);
                vasen.removeAllViews();
                vasen.addView(siirto);

                return;
            }

            //portrait orientaatiossa vaihdetaan vain yksi fragmentti näkyviin
        } else if (newConfig.orientation == Configuration.ORIENTATION_PORTRAIT){

            //jos meillä on muitakin fragmentteja olemassa kuin päävalikko
            if (mFragmentti2!=null || mFragmentti3!=null || mFragmentti4!=null) {


                //ensiksi meidän pitää irroittaa vasen fragmentti viewgroupista
                if (mFragmentti1 != null) {
                    siirto = mFragmentti1.annaNakyma();
                    parent = (ViewGroup) siirto.getParent();
                    if (parent!=null && parent.equals(vasen)) {
                        parent.removeView(siirto);
                    }
                }
                if (mFragmentti2 != null) {
                    siirto = mFragmentti2.annaNakyma();
                    parent = (ViewGroup) siirto.getParent();
                    if (parent!=null && parent.equals(vasen)) {
                        parent.removeView(siirto);
                    }
                }
                if (mFragmentti3 != null) {
                    siirto = mFragmentti3.annaNakyma();
                    parent = (ViewGroup) siirto.getParent();
                    if (parent!=null && parent.equals(vasen)) {
                        parent.removeView(siirto);
                    }
                }
                if (mFragmentti4 != null) {
                    siirto = mFragmentti4.annaNakyma();
                    parent = (ViewGroup) siirto.getParent();
                    if (parent!=null && parent.equals(vasen)) {
                        parent.removeView(siirto);
                    }
                }

                //selvitetään, mikä fragmentti on näkyvissä oikealla ja siirretään se vasemmalle
                if (mFragmentti1 != null) {
                    siirto=mFragmentti1.annaNakyma();
                        parent =(ViewGroup) siirto.getParent();
                        if (parent!=null && parent.equals(oikea)) {
                            parent.removeView(siirto);
                            vasen.removeAllViews();
                            vasen.addView(siirto);
                        }
                }
                if (mFragmentti2 != null) {
                    siirto=mFragmentti2.annaNakyma();
                    parent =(ViewGroup) siirto.getParent();
                    if (parent!=null && parent.equals(oikea)) {
                        parent.removeView(siirto);
                        vasen.removeAllViews();
                        vasen.addView(siirto);
                    }
                }
                if (mFragmentti3 != null) {
                    siirto=mFragmentti3.annaNakyma();
                    parent =(ViewGroup) siirto.getParent();
                    if (parent!=null && parent.equals(oikea)) {
                        parent.removeView(siirto);
                        vasen.removeAllViews();
                        vasen.addView(siirto);
                    }
                }
                if (mFragmentti4 != null) {
                    siirto=mFragmentti4.annaNakyma();
                    parent =(ViewGroup) siirto.getParent();
                    if (parent!=null && parent.equals(oikea)) {
                        parent.removeView(siirto);
                        vasen.removeAllViews();
                        vasen.addView(siirto);
                    }
                }

                //lopuksi otetaan oikeanpuoleiselta fragmentilta ruututila pois
                LinearLayout.LayoutParams parametrit = (LinearLayout.LayoutParams) oikea.getLayoutParams();
                parametrit.width = 0;
                parametrit.weight = 0;
                oikea.setLayoutParams(parametrit);
            }
        }
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {

        // Handle action bar item clicks here. The action bar will
        // automatically handle clicks on the Home/Up button, so long
        // as you specify a parent activity in AndroidManifest.xml.
        int id = item.getItemId();

        if (id == R.id.paavalikko) {

            //jos orientaationa on landscape
            int orientaatio=getResources().getConfiguration().orientation;
            if (orientaatio== Configuration.ORIENTATION_LANDSCAPE) {
                asetaLandscape();
            }

            //PaaValikko on joko valmiiksi esillä, jolloin ei tehdä mitään tai sitten se pitää
            //siirtää esille. Olemassa se on joka tapauksessa.
            if (null==mFragmentti1.annaNakyma().getParent()) {

                //Ensiksi CADruudun pitää mahdollisesti järjestää asiansa
                if (mFragmentti3!=null && null!=mFragmentti3.annaNakyma().getParent()) {
                    mSurf.sailytaSaie();
                }

                mSisaltoVasen.removeAllViews();
                mSisaltoVasen.addView(mFragmentti1.annaNakyma());
            }
            return true;
        }

        if (id == R.id.tekstiruutu) {

            esitaTekstiRuutu();
            return true;
        }

        if (id == R.id.CADruutu) {

            esitaCADRuutu();
            return true;

        }

        if (id==R.id.tulostaCAD) {

            //varmistetaan ensin, että CADruutu on olemassa ja näkyvillä
            if (mFragmentti3==null || !mFragmentti3.isVisible()) {
                esitaCADRuutu();
            }

            //Käyttäjä haluaa tulostaa CAD-nakymän. Kysytään käyttäjältä näkymän taustaväri.
            //Koska Tasavalta ei lataa tarvitsemiaan kirjastoja sovelluksen alussa, vaan fragmentin
            //alussa, Tasavalta ei aina kykene tulostamaan haluttua objektia
            if (mViimeinen==1) {
                mTaustaKysely.show(mSing.mAktiviteetti1.mFragmentManager, "Taustakysely");
            } else {
                Toast toast=Toast.makeText(getApplicationContext(),
                        "Print job failed. To retry printing, " +
                                "please close and restart Tasavalta first!", Toast.LENGTH_LONG);
                toast.show();
            }
            return true;
        }

        if (id==R.id.tulostaTeksti) {

            //varmistetaan ensin, että tekstiruutu on olemassa ja näkyvillä
            if (mFragmentti2==null || !mFragmentti2.isVisible()) {
                esitaTekstiRuutu();
            }

            //Käyttäjä haluaa tulostaa Tekstidokumentin. Kysytään käyttäjältä dokumentin resoluutio,
            //sillä dokumentti on Android-versiossa kuvatiedosto, joka kuluttaa paljon muistia.
            //Koska Tasavalta ei lataa tarvitsemiaan kirjastoja sovelluksen alussa, vaan fragmentin
            //alussa, Tasavalta ei aina kykene tulostamaan haluttua objektia
            if (mViimeinen==2) {
                mResoluutioLista.show(mSing.mAktiviteetti1.mFragmentManager, "ResoluutioLista");
            } else {
                Toast toast=Toast.makeText(getApplicationContext(),
                        "Print job failed. To retry printing, " +
                                "please close and restart Tasavalta first!", Toast.LENGTH_LONG);
                toast.show();
            }
            return true;
        }

        if (id == R.id.about) {

            //jos orientaationa on landscape
            int orientaatio=getResources().getConfiguration().orientation;
            if (orientaatio== Configuration.ORIENTATION_LANDSCAPE) {
                asetaLandscape();
            }

            //Varmistetaan, että aboutruutu on olemassa
            if (mFragmentti4 != null) {

                //aboutruutu on joko valmiiksi esillä, jolloin ei tehdä mitään tai sitten se pitää
                //siirtää esille.
                if (null==mFragmentti4.annaNakyma().getParent()) {

                    //Ensiksi CADruudun pitää mahdollisesti järjestää asiansa
                    if (mFragmentti3!=null && null!=mFragmentti3.annaNakyma().getParent()) {
                        mSurf.sailytaSaie();
                    }

                    mSisaltoVasen.removeAllViews();
                    mSisaltoVasen.addView(mFragmentti4.annaNakyma());
                }

                //muuten aboutruutu täytyy luoda
            } else {

                //Ensiksi CADruudun pitää mahdollisesti järjestää asiansa
                if (mFragmentti3!=null && null!=mFragmentti3.annaNakyma().getParent()) {
                    mSurf.sailytaSaie();
                }
                mSisaltoVasen.removeAllViews();

                FragmentTransaction fragmentTransaction = mFragmentManager.beginTransaction();
                mFragmentti4 = new About();
                fragmentTransaction.add(R.id.sisaltoVasen, mFragmentti4, "fragmentti4");
                fragmentTransaction.disallowAddToBackStack();
                fragmentTransaction.commit();
            }
            return true;
        }

        if (R.id.help == id) {
            mTekstiTiedosto="Ohjeet.html";
            boolean avataanko=mFragmentti2!=null && mFragmentti2.isVisible();
            esitaTekstiRuutu();
            if (avataanko) mFragmentti2.avaaTeksti();
            return true;
        }

        if (R.id.sulje == id) {

            //tallennetaan ohjelman tila muistiin
            mFragmentti1.tallennaMuistiin();

            //tyhjennetään Android laitteen muistista pdf-dokumentit
            File dirPath = new File(mSing.mAktiviteetti1.getApplicationContext().getFilesDir(),
                    "pdfs");
            tuhoaRekursiivisesti(dirPath);

            //tyhjennetään Android laitteen muistista kuvat
            dirPath = new File(mSing.mAktiviteetti1.getApplicationContext().getFilesDir(),
                    "images");
            tuhoaRekursiivisesti(dirPath);

            //suljetaan Tasavalta sovellus
            finishAffinity();
            System.exit(0);
            return true;
        }

        if (R.id.luoKirjanMerkki == id) {

            //varmistetaan, että CADruutu on olemassa
            if (mFragmentti3!=null) {
                float siirto[]={-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1};
                mFragmentti3.annaOrientaatio1(siirto);
                mFragmentti1.mMuisti.asetaKirjanMerkkiOrientaatioD(siirto, mCADTiedosto);
            } else {
                float siirto[]={-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1};
                mFragmentti1.mMuisti.asetaKirjanMerkkiOrientaatioD(siirto, null);
            }

            //varmistetaan, että TekstiRuutu on olemassa
            if (mFragmentti2!=null) {
                int siirto[]={-1, -1, -1, -1};
                siirto[0]=mFragmentti2.siirto3;
                siirto[1]=mFragmentti2.siirto4;
                siirto[2]=mFragmentti2.siirto5;
                siirto[3]=mFragmentti2.siirto6;
                mFragmentti1.mMuisti.asetaKirjanMerkkiOrientaatioT(siirto, mTekstiTiedosto);
            } else {
                int siirto[]={-1, -1, -1, -1};
                mFragmentti1.mMuisti.asetaKirjanMerkkiOrientaatioT(siirto, null);
            }

            //annetaan käyttäjälle ilmoitus
            Toast toast=Toast.makeText(getApplicationContext(),
                    "Bookmark created. It is available next time you lauch Tasavalta.",
                    Toast.LENGTH_LONG);
            toast.show();
        }

        return super.onOptionsItemSelected(item);
    }

    public void tuhoaRekursiivisesti(File file) {
        if (file.isDirectory()) {
            for (File lapsi : file.listFiles()) {
                tuhoaRekursiivisesti(lapsi);
            }
        }
        file.delete();
    }

    public void esitaCADRuutu() {

        //jos orientaationa on landscape
        int orientaatio=getResources().getConfiguration().orientation;
        if (orientaatio== Configuration.ORIENTATION_LANDSCAPE) {
            asetaLandscape();
        }

        //Varmistetaan, että cadruutu on olemassa
        if (mFragmentti3 != null) {

            //CADruutu on joko valmiiksi esillä, jolloin ei tehdä mitään tai sitten se pitää
            //siirtää esille.
            if (null==mFragmentti3.annaNakyma().getParent()) {
                mSisaltoVasen.removeAllViews();
                mSisaltoVasen.addView(mFragmentti3.annaNakyma());
                mFragmentti3.paivitaAla();
            }

            //muuten CADruutu täytyy luoda
        } else {
            mSisaltoVasen.removeAllViews();
            FragmentTransaction fragmentTransaction = mFragmentManager.beginTransaction();
            mFragmentti3 = new CADRuutu();
            fragmentTransaction.add(R.id.sisaltoVasen, mFragmentti3, "fragmentti3");
            fragmentTransaction.disallowAddToBackStack();
            fragmentTransaction.commit();
        }
    }

    public void esitaTekstiRuutu() {

        //jos orientaationa on landscape
        int orientaatio=getResources().getConfiguration().orientation;
        if (orientaatio== Configuration.ORIENTATION_LANDSCAPE) {
            asetaLandscape();
        }

        //Varmistetaan, että tekstiruutu on olemassa
        if (mFragmentti2 != null) {

            //Tekstiruutu on joko valmiiksi esillä, jolloin ei tehdä mitään tai sitten se pitää
            //siirtää esille.
            if (null==mFragmentti2.annaNakyma().getParent()) {

                //Ensiksi CADruudun pitää mahdollisesti järjestää asiansa
                if (mFragmentti3!=null && null!=mFragmentti3.annaNakyma().getParent()) {
                    mSurf.sailytaSaie();
                }

                mSisaltoVasen.removeAllViews();
                mSisaltoVasen.addView(mFragmentti2.annaNakyma());
            }

            //muuten tekstiruutu täytyy luoda
        } else {

            //Ensiksi CADruudun pitää mahdollisesti järjestää asiansa
            if (mFragmentti3!=null && null!=mFragmentti3.annaNakyma().getParent()) {
                mSurf.sailytaSaie();
            }
            mSisaltoVasen.removeAllViews();

            FragmentTransaction fragmentTransaction = mFragmentManager.beginTransaction();
            mFragmentti2 = new TekstiRuutu();
            fragmentTransaction.add(R.id.sisaltoVasen, mFragmentti2, "fragmentti2");
            fragmentTransaction.disallowAddToBackStack();
            fragmentTransaction.commit();
        }
    }

    private void asetaLandscape() {

        //ensiksi meidän pitää antaa puolet näyttöruudusta oikeanpuoleiselle fragmentille
        View siirto=mSisaltoOikea;
        LinearLayout.LayoutParams parametrit=(LinearLayout.LayoutParams) siirto.getLayoutParams();
        parametrit.width=0;
        parametrit.weight=1;
        siirto.setLayoutParams(parametrit);

        //sitten siirrämme CADRuutu, TekstiRuutu tai PaaValikko fragmentin oikealle
        if (mFragmentti3!=null) {
            siirto = mFragmentti3.annaNakyma();
        } else if (mFragmentti2!=null) {
            siirto = mFragmentti2.annaNakyma();
        } else {
            siirto = mFragmentti1.annaNakyma();
        }
        ViewGroup parent1 = (ViewGroup) siirto.getParent();
        if (parent1!=null) {

            //Ensiksi CADruudun pitää mahdollisesti järjestää asiansa
            if (mFragmentti3!=null && parent1.equals(mFragmentti3.annaNakyma().getParent())) {
                mSurf.sailytaSaie();
            }
            parent1.removeView(siirto);
            ViewGroup parent2=mSisaltoOikea;
            parent2.removeAllViews();
            parent2.addView(siirto);
        }
    }

    //Tällä funktiolla lähetetään ulkopuoliselle ohjelmalle jokin tehtävä
    public void lahetaJohonkin(Intent intent) {

        startActivity(intent);
    }

    //Tämä kuuntelija ottaa vastaan ResoluutioLista ilmentymän radionappivalinnan
    public void onRadioButtonClicked(View view) {

        boolean klikattu=((RadioButton) view).isChecked();

        switch (view.getId()) {

            case R.id.dpi75:
                if (klikattu) mTulostusResoluutio=100;
                break;

            case R.id.dpi150:
                if (klikattu) mTulostusResoluutio=150;
                break;

            case R.id.dpi300:
                if (klikattu) mTulostusResoluutio=300;
                break;

            case R.id.dpi600:
                if (klikattu) mTulostusResoluutio=600;
                break;
        }
    }

    //tällä funktiolla tehdään Adroid laitteen muistikysely
    private ActivityManager.MemoryInfo annaMuistiTiedot() {
        ActivityManager activityManager=(ActivityManager) getSystemService(ACTIVITY_SERVICE);
        ActivityManager.MemoryInfo info=new ActivityManager.MemoryInfo();
        activityManager.getMemoryInfo(info);
        return info;
    }

    //************** Multiple permissions ****************//

    /**
     * Call multiple Permissions
     */
/*
    private void callMultiplePermissions() {
        ArrayList<String> permissionsNeeded = new ArrayList<String>();

        final ArrayList<String> permissionsList = new ArrayList<String>();
        if (!addPermission(permissionsList, Manifest.permission.READ_EXTERNAL_STORAGE))
            permissionsNeeded.add("NETWORK STATE");
        if (!addPermission(permissionsList, Manifest.permission.WRITE_EXTERNAL_STORAGE))
            permissionsNeeded.add("WRITE EXTERNAL STORAGE");

        if (permissionsList.size() > 0) {
            if (permissionsNeeded.size() > 0) {
                // Need Rationale
                String message = "You need to grant access to " + permissionsNeeded.get(0);
                for (int i = 1; i < permissionsNeeded.size(); i++)
                    message = message + ", " + permissionsNeeded.get(i);

                if (Build.VERSION.SDK_INT >= 23) {
                    // Marshmallow+
                    requestPermissions(permissionsList.toArray(new String[permissionsList.size()]),
                            REQUEST_CODE_ASK_MULTIPLE_PERMISSIONS);
                } else {
                    // Pre-Marshmallow
                }

                return;
            }
            if (Build.VERSION.SDK_INT >= 23) {
                // Marshmallow+
                requestPermissions(permissionsList.toArray(new String[permissionsList.size()]),
                        REQUEST_CODE_ASK_MULTIPLE_PERMISSIONS);
            } else {
                // Pre-Marshmallow
            }

            return;
        }

    }
*/
    /**
     * add Permissions
     *
     * @param permissionsList
     * @param permission
     * @return
     */
/*
    private boolean addPermission(ArrayList<String> permissionsList, String permission) {
        if (Build.VERSION.SDK_INT >= 23) {
            // Marshmallow+
            if (checkSelfPermission(permission) != PackageManager.PERMISSION_GRANTED) {
                permissionsList.add(permission);
                // Check for Rationale Option
                if (!shouldShowRequestPermissionRationale(permission))
                    return false;
            }
        } else {
            // Pre-Marshmallow
        }

        return true;
    }
*/
    /**
     * Permissions results
     *
     * @param requestCode
     * @param permissions
     * @param grantResults
     */
/*
    @Override
    public void onRequestPermissionsResult(int requestCode,
                                           String[] permissions, int[] grantResults) {
        switch (requestCode) {
            case REQUEST_CODE_ASK_MULTIPLE_PERMISSIONS: {
                Map<String, Integer> perms = new HashMap<>();
                // Initial
                perms.put(Manifest.permission.READ_EXTERNAL_STORAGE,
                        PackageManager.PERMISSION_GRANTED);
                perms.put(Manifest.permission.WRITE_EXTERNAL_STORAGE,
                        PackageManager.PERMISSION_GRANTED);
                // Fill with results
                for (int i = 0; i < permissions.length; i++)
                    perms.put(permissions[i], grantResults[i]);
                // Check for ACCESS_FINE_LOCATION and others
                if (perms.get(Manifest.permission.READ_EXTERNAL_STORAGE) ==
                        PackageManager.PERMISSION_GRANTED
                        && perms.get(Manifest.permission.WRITE_EXTERNAL_STORAGE) ==
                        PackageManager.PERMISSION_GRANTED ) {
                    // All Permissions Granted

                } else {
                    // Permission Denied
                    Toast toast=Toast.makeText(getApplicationContext(),
                            "Some Permission is Denied", Toast.LENGTH_LONG);
                    toast.show();
                }
            }
            break;
            default:
                super.onRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
*/
}

