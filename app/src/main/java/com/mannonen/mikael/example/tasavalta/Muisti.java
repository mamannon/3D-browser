/*
 * 2018 Mikael Mannonen
 *
 * Luokka Muisti säilyttää ohjelman toiminnan kannalta välttämättömiä
 * käyttäjän tapahtumahistorian tapahtumia. Lopetettaessa ohjelma luokan
 * ilmentymä tulee tallentaa kovalevylle, josta se ohjelmaa käynnistettäessä
 * luetaan.
 */


package com.mannonen.mikael.example.tasavalta;

import java.io.Serializable;
import android.content.Context;
import android.content.pm.PackageManager;
import android.os.Handler;
import android.os.Process;
import android.widget.Toast;

import static com.mannonen.mikael.example.tasavalta.Singleton.annaIlmentyma;

public class Muisti implements Serializable {

    private transient Singleton mSing;
    private transient Handler mHandler;
    private transient MuistiKuuntelija mKuuntelija;
    private transient int mIndeksi;
    private int mIndex=-1;
    private boolean mCADAvattu=true;
    private boolean mTekstiAvattu=true;
    private boolean mCallBack=false;

    //Tasavallan versionumero
    private int versio;

    //Android laitteen OpenGL kyvyt
    private int optio;

    //Epäonnistuiko viime kerralla tulostus ja siihen liittyvä muuttuja
    private int mViimeinen=1;

    //Alla olevat ovat kirjanmerkkejä varten
    private float[][] kirjanMerkkiOrientaatioD=new float[9][65];
    private String[] kirjanMerkkiTiedostoD=new String[9];
    private int[][] kirjanMerkkiOrientaatioT=new int[9][4];
    private String[] kirjanMerkkiTiedostoT=new String[9];
    private boolean[] kirjanMerkkiOnkoAsetettuD= {false, false, false, false,
            false, false, false, false, false};
    private boolean[] kirjanMerkkiOnkoAsetettuT= {false, false, false, false,
            false, false, false, false, false};
//	private int kirjanMerkkiKirjanMerkkeja=0;

    //Alla olevat ovat valikkomäärittelyitä varten
    private String[] tiedostoTietoNimi;
    private int[] tiedostoTietoValikkoSijainti;
    private int[] tiedostoTietoOnkoNakyva;
    private int[] tiedostoTietoOnkoAsetettu;
    private int[] tiedostoTietoOnko3D;
    private int tiedostoTietoTiedostoTietoja;
    private int palstojenLukumaara;

    public interface MuistiKuuntelija {
        public void onTekstiAvattu();
    }

    public void asetaMuistiKuuntelija(MuistiKuuntelija kuuntelija) {
        this.mKuuntelija=kuuntelija;
    }

    public Muisti(Context konteksti) {

        //pyydetään Singleton luokan ilmentymä
        mSing=annaIlmentyma();

//        kirjanMerkkiOrientaatioD=new float[9][65];
//        kirjanMerkkiTiedostoD=new String[9];
//        kirjanMerkkiOrientaatioT=new int[9][4];
//        kirjanMerkkiTiedostoT=new String[9];
        optio=0;
        tiedostoTietoTiedostoTietoja=konteksti.getResources().getInteger(R.integer.tiedostotietotiedostotietoja);
        tiedostoTietoNimi=konteksti.getResources().getStringArray(R.array.tiedostotietonimi);
        tiedostoTietoValikkoSijainti=konteksti.getResources().
                getIntArray(R.array.tiedostotietovalikkosijainti);
        tiedostoTietoOnkoNakyva= konteksti.getResources().
                getIntArray(R.array.tiedostotietoonkonakyva);
        tiedostoTietoOnkoAsetettu=konteksti.getResources().
                getIntArray(R.array.tiedostotietoonkoasetettu);
        tiedostoTietoOnko3D=konteksti.getResources().getIntArray(R.array.tiedostotietoonko3d);
        palstojenLukumaara=konteksti.getResources().getInteger(R.integer.palstojenlukumaara);
        try {
            String packageVersion=konteksti.getPackageName();
            versio = konteksti.getPackageManager().
                    getPackageInfo(packageVersion, 0).versionCode;
        } catch (PackageManager.NameNotFoundException e) {
            versio = -1;
        }
    }
    public boolean onkoKirjanMerkkia(int num) {return kirjanMerkkiOnkoAsetettuD[num]
            || kirjanMerkkiOnkoAsetettuT[num];}
    public int annaTiedostojenMaara() {return tiedostoTietoTiedostoTietoja;}
    public int annaValikkoSijainti(int num) {return tiedostoTietoValikkoSijainti[num];}
    public boolean annaOnko3D(int num) {return tiedostoTietoOnko3D[num]>0 ? true:false;}
    public String annaTiedostonNimi(int num) {return tiedostoTietoNimi[num];}
    public boolean annaOnkoAsetettu(int num) {return tiedostoTietoOnkoAsetettu[num]>0 ? true:false;}
    public boolean annaOnkoNakyva(int num) {return tiedostoTietoOnkoNakyva[num]>0 ? true:false;}
    public int annaPalstat() {return palstojenLukumaara;}
    public int annaVersio() {return versio;}
    public void asetaNakyvaksi(int num) {tiedostoTietoOnkoNakyva[num]=1;}
    public void asetaAsetetuksi(int num) {tiedostoTietoOnkoAsetettu[num]=1;}
    public void asetaOptio(int num) {optio=num;}
    public int annaOptio() {return optio;}
    public void asetaViimeinen(int num) {mViimeinen=num;}
//    public int annaViimeinen() {return mViimeinen;}

    public void asetaKirjanMerkkiOrientaatioD(float[] orientaatio, String tiedosto) {
        boolean alusta=false;

        //jos tämä metodi on ensimmäinen, etsitään ensimmäinen vapaa paikka
        if (mIndex==-1) {
            for (int i = 0; i < 9; i++) {
                if (!(kirjanMerkkiOnkoAsetettuD[i] || kirjanMerkkiOnkoAsetettuT[i])) {
                    mIndex = i;
                    break;
                }
            }
        } else {
            alusta=true;
        }

        //varmistetaan, että meillä on jotain
        if (tiedosto==null) {
            if (alusta) mIndex=-1;
            return;
        }

        if (mIndex != -1) {

            //täytetään ensimminen vapaa paikka
            for (int i=0; i<65; i++) {
                kirjanMerkkiOrientaatioD[mIndex][i]=orientaatio[i];
            }
            kirjanMerkkiTiedostoD[mIndex]=tiedosto;
            kirjanMerkkiOnkoAsetettuD[mIndex]=true;
        } else {

            //jos kaikki paikat ovat varatut, hylätään ensimmäinen ja siirretään muita niin,
            //että voimme täyttää viimeisen paikan
            kirjanMerkkiOrientaatioD[0]=kirjanMerkkiOrientaatioD[1];
            kirjanMerkkiOrientaatioD[1]=kirjanMerkkiOrientaatioD[2];
            kirjanMerkkiOrientaatioD[2]=kirjanMerkkiOrientaatioD[3];
            kirjanMerkkiOrientaatioD[3]=kirjanMerkkiOrientaatioD[4];
            kirjanMerkkiOrientaatioD[4]=kirjanMerkkiOrientaatioD[5];
            kirjanMerkkiOrientaatioD[5]=kirjanMerkkiOrientaatioD[6];
            kirjanMerkkiOrientaatioD[6]=kirjanMerkkiOrientaatioD[7];
            kirjanMerkkiOrientaatioD[7]=kirjanMerkkiOrientaatioD[8];
            kirjanMerkkiTiedostoD[0]=kirjanMerkkiTiedostoD[1];
            kirjanMerkkiTiedostoD[1]=kirjanMerkkiTiedostoD[2];
            kirjanMerkkiTiedostoD[2]=kirjanMerkkiTiedostoD[3];
            kirjanMerkkiTiedostoD[3]=kirjanMerkkiTiedostoD[4];
            kirjanMerkkiTiedostoD[4]=kirjanMerkkiTiedostoD[5];
            kirjanMerkkiTiedostoD[5]=kirjanMerkkiTiedostoD[6];
            kirjanMerkkiTiedostoD[6]=kirjanMerkkiTiedostoD[7];
            kirjanMerkkiTiedostoD[7]=kirjanMerkkiTiedostoD[8];

            for (int i=0; i<65; i++) {
                kirjanMerkkiOrientaatioD[8][i]=orientaatio[i];
            }
            kirjanMerkkiTiedostoD[8]=tiedosto;
            mIndex=8;
        }

        if (alusta) mIndex=-1;
    }

    public void asetaKirjanMerkkiOrientaatioT(int[] orientaatio, String tiedosto) {
        boolean alusta=false;

        //jos tämä metodi on ensimmäinen, etsitään ensimmäinen vapaa paikka
        if (mIndex==-1) {
            for (int i = 0; i < 9; i++) {
                if (!(kirjanMerkkiOnkoAsetettuD[i] || kirjanMerkkiOnkoAsetettuT[i])) {
                    mIndex = i;
                    break;
                }
            }
        } else {
            alusta=true;
        }

        //varmistetaan, että meillä on jotain
        if (tiedosto==null) {
            if (alusta) mIndex=-1;
            return;
        }

        if (mIndex != -1) {

            //täytetään ensimminen vapaa paikka
            kirjanMerkkiOrientaatioT[mIndex][0]=orientaatio[0];
            kirjanMerkkiOrientaatioT[mIndex][1]=orientaatio[1];
            kirjanMerkkiOrientaatioT[mIndex][2]=orientaatio[2];
            kirjanMerkkiOrientaatioT[mIndex][3]=orientaatio[3];
            kirjanMerkkiTiedostoT[mIndex]=tiedosto;
            kirjanMerkkiOnkoAsetettuT[mIndex]=true;
        } else {

            //jos kaikki paikat ovat varatut, hylätään ensimmäinen ja siirretään muita niin,
            //että voimme täyttää viimeisen paikan
            kirjanMerkkiOrientaatioT[0]=kirjanMerkkiOrientaatioT[1];
            kirjanMerkkiOrientaatioT[1]=kirjanMerkkiOrientaatioT[2];
            kirjanMerkkiOrientaatioT[2]=kirjanMerkkiOrientaatioT[3];
            kirjanMerkkiOrientaatioT[3]=kirjanMerkkiOrientaatioT[4];
            kirjanMerkkiOrientaatioT[4]=kirjanMerkkiOrientaatioT[5];
            kirjanMerkkiOrientaatioT[5]=kirjanMerkkiOrientaatioT[6];
            kirjanMerkkiOrientaatioT[6]=kirjanMerkkiOrientaatioT[7];
            kirjanMerkkiOrientaatioT[7]=kirjanMerkkiOrientaatioT[8];
            kirjanMerkkiTiedostoT[0]=kirjanMerkkiTiedostoT[1];
            kirjanMerkkiTiedostoT[1]=kirjanMerkkiTiedostoT[2];
            kirjanMerkkiTiedostoT[2]=kirjanMerkkiTiedostoT[3];
            kirjanMerkkiTiedostoT[3]=kirjanMerkkiTiedostoT[4];
            kirjanMerkkiTiedostoT[4]=kirjanMerkkiTiedostoT[5];
            kirjanMerkkiTiedostoT[5]=kirjanMerkkiTiedostoT[6];
            kirjanMerkkiTiedostoT[6]=kirjanMerkkiTiedostoT[7];
            kirjanMerkkiTiedostoT[7]=kirjanMerkkiTiedostoT[8];

            kirjanMerkkiOrientaatioT[8][0]=orientaatio[0];
            kirjanMerkkiOrientaatioT[8][1]=orientaatio[1];
            kirjanMerkkiOrientaatioT[8][2]=orientaatio[2];
            kirjanMerkkiOrientaatioT[8][3]=orientaatio[3];
            kirjanMerkkiTiedostoT[8]=tiedosto;
        }

        if (alusta) mIndex=-1;
    }

    public void tuhoaKirjanMerkit() {
        kirjanMerkkiOnkoAsetettuD[0]=false;
        kirjanMerkkiOnkoAsetettuD[1]=false;
        kirjanMerkkiOnkoAsetettuD[2]=false;
        kirjanMerkkiOnkoAsetettuD[3]=false;
        kirjanMerkkiOnkoAsetettuD[4]=false;
        kirjanMerkkiOnkoAsetettuD[5]=false;
        kirjanMerkkiOnkoAsetettuD[6]=false;
        kirjanMerkkiOnkoAsetettuD[7]=false;
        kirjanMerkkiOnkoAsetettuD[8]=false;
        kirjanMerkkiOnkoAsetettuT[0]=false;
        kirjanMerkkiOnkoAsetettuT[1]=false;
        kirjanMerkkiOnkoAsetettuT[2]=false;
        kirjanMerkkiOnkoAsetettuT[3]=false;
        kirjanMerkkiOnkoAsetettuT[4]=false;
        kirjanMerkkiOnkoAsetettuT[5]=false;
        kirjanMerkkiOnkoAsetettuT[6]=false;
        kirjanMerkkiOnkoAsetettuT[7]=false;
        kirjanMerkkiOnkoAsetettuT[8]=false;
        mIndex=-1;
        mSing=annaIlmentyma();
        Toast toast=Toast.makeText(mSing.mAktiviteetti1.getApplicationContext(),
                "Bookmarks deleted. They will disappear next time you switch to Main menu.",
                Toast.LENGTH_LONG);
        toast.show();
    }

    //tätä metodia tulee kutsua silloin, jos muistissa olevat sekä CAD että teksti tiedosto
    //molemmat halutaan avata
    public void lataaKirjanMerkkiAlku(int index) {
        mCADAvattu=true;
        mTekstiAvattu=true;
        mCallBack=true;
        mIndeksi=index;

        //Tämä if on mahdollisen epäonnistuneen tulostamisen takia (mikä ei enään ole ongelma...)
        if (mViimeinen==1) {

            //jos muistissa on tekstitiedosto...
            if (kirjanMerkkiOnkoAsetettuT[index])
                avaaTekstiTiedosto(index);
            else
                mKuuntelija.onTekstiAvattu();
        } else {

            //jos muistissa on CAD tiedosto...
            if (kirjanMerkkiOnkoAsetettuD[index])
                avaaCADTiedosto(index);
            else
                mKuuntelija.onTekstiAvattu();
        }
    }

    //tätä metodia ei koskaan pidä kutsua manuaalisesti! Kutsu sen sijaan lataa kirjanmerkkialku
    public void lataaKirjanMerkkiLoppu() {

        //tämä if on mahdollisen epäonnistuneen tulostamisen takia
        if (mViimeinen==1) {
            if (mCallBack == true) {

                //jos muistissa on CAD tiedosto...
                if (kirjanMerkkiOnkoAsetettuD[mIndeksi])
                    avaaCADTiedosto(mIndeksi);
            }
        } else {
            if (mCallBack == true) {

                //jos muistissa on tekstitiedosto...
                if (kirjanMerkkiOnkoAsetettuT[mIndeksi]) avaaTekstiTiedosto(mIndeksi);
            }
        }
        mCallBack=false;
    }

    //tätä metodia tulee kutsua silloin, jos ainoastaan muistissa oleva CAD tiedosto halutaan
    // avata
    public boolean avaaCADTiedosto(int index) {

        //katsotaan, voidaanko aloittaa
        if (mViimeinen==1) {
            if (mTekstiAvattu == true) {
                mCADAvattu = false;
            } else {
                return false;
            }
        } else {
            if (mCADAvattu == true) {
                mCADAvattu = false;
            } else {
                return false;
            }
        }

        //varmistetaan, että singleton ja handler ovat käytössä
        mSing=annaIlmentyma();
        if (mHandler==null) {
            mHandler = new Handler();
        }

        //varmistetaan, että tarjolla on tiedosto
        if (kirjanMerkkiTiedostoD[index]!=null) {
            mSing.mAktiviteetti1.mCADTiedosto=kirjanMerkkiTiedostoD[index];

            //varmistetaan, että CAD kirjasto on käynnistetty ja näkyvillä
            boolean siirto=mSing.mAktiviteetti1.mFragmentti3==null;
            if (siirto || !mSing.mAktiviteetti1.mFragmentti3.isVisible()) {
                mSing.mAktiviteetti1.esitaCADRuutu();
            }
            if (!siirto) mSing.mAktiviteetti1.mFragmentti3.avaaCAD();

            //uusi säie tarvitaan siksi, että CADRuutu saadaan ensin luotua
            final int i=index;
            final Runnable r=new Runnable() {
                @Override
                public void run() {

                    //odotetaan, että Kirjasto1 on käytössä
                    while (mSing.mAktiviteetti1.mOnkoCADAvattu!=true) {
                        try {
                            Thread.sleep(5);
                        } catch (Exception e) {

                        }
                    }

                    //lopuksi asetetaan orientaatio
                    mSing.mAktiviteetti1.mFragmentti3.asetaOrientaatio1(kirjanMerkkiOrientaatioD[i]);
                    mCADAvattu=true;

                    //loppujen lopuksi kutsutaan CallBack, jos ensiksi tuli avata CADruutu
                    if (mViimeinen==2) {
                        mKuuntelija.onTekstiAvattu();
                    }
                }
            };
            mHandler.postDelayed(r, 50);
        } else {
            mCADAvattu=true;
        }
        return true;
    }

    //tätä metodia tulee kutsua silloin, jos ainoastaan muistissa oleva Teksti tiedosto halutaan
    // avata
    public boolean avaaTekstiTiedosto(int index) {

        //katsotaan, voidaanko aloittaa
        if (mViimeinen==1) {
            if (mCADAvattu == true) {
                mTekstiAvattu = false;
            } else {
                return false;
            }
        } else {
            if (mTekstiAvattu == true) {
                mTekstiAvattu = false;
            } else {
                return false;
            }
        }

        //varmistetaan, että singleton ja handler ovat käytössä
        mSing=annaIlmentyma();
        if (mHandler==null) {
            mHandler = new Handler();
        }

        //varmistetaan, että tarjolla on tiedosto
        if (kirjanMerkkiTiedostoT[index]!=null) {
            mSing.mAktiviteetti1.mTekstiTiedosto=kirjanMerkkiTiedostoT[index];

            //varmistetaan, että Teksti kirjasto on käynnistetty ja näkyvillä
            boolean siirto=mSing.mAktiviteetti1.mFragmentti2==null;
            if (siirto || !mSing.mAktiviteetti1.mFragmentti2.isVisible()) {
                mSing.mAktiviteetti1.esitaTekstiRuutu();
            }
            if (!siirto) mSing.mAktiviteetti1.mFragmentti2.avaaTeksti();

            //uusi säie tarvitaan siksi, että TekstiRuutu saadaan ensin luotua
            final int i=index;
            final Runnable r=new Runnable() {
                @Override
                public void run() {

                    //odotetaan, että tekstinäkymä on luotu
                    while (mSing.mAktiviteetti1.mOnkoTekstiAvattu!=true) {
                        try {
                            Thread.sleep(5);
                        } catch (Exception e) {

                        }
                    }

                    //lopuksi asetetaan orientaatio
                    mSing.mAktiviteetti1.mFragmentti2.siirto3=kirjanMerkkiOrientaatioT[i][0];
                    mSing.mAktiviteetti1.mFragmentti2.siirto4=kirjanMerkkiOrientaatioT[i][1];
                    mSing.mAktiviteetti1.mFragmentti2.siirto5=kirjanMerkkiOrientaatioT[i][2];
                    mSing.mAktiviteetti1.mFragmentti2.siirto6=kirjanMerkkiOrientaatioT[i][3];
                    mSing.mAktiviteetti1.mFragmentti2.avaaUudelleen();
                    mTekstiAvattu=true;

                    //loppujen lopuksi kutsutaan CallBack, jos ensiksi tuli avata tekstiruutu
                    if (mViimeinen==1) {
                        mKuuntelija.onTekstiAvattu();
                    }
                }
            };
            mHandler.postDelayed(r, 50);
        } else {
            mTekstiAvattu=true;
            mKuuntelija.onTekstiAvattu();
        }
        return true;
    }
}

