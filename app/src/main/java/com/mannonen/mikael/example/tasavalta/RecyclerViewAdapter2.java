package com.mannonen.mikael.example.tasavalta;

import android.os.Build;
import android.support.annotation.NonNull;
import android.support.v4.app.FragmentTransaction;
import android.support.v7.widget.RecyclerView;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.LinearLayout;
import android.widget.TextView;

import java.nio.charset.StandardCharsets;
import java.util.ArrayList;

import static com.mannonen.mikael.example.tasavalta.Singleton.annaIlmentyma;

public class RecyclerViewAdapter2 extends RecyclerView.Adapter
        <RecyclerViewAdapter2.RecyclerViewHolder> {

    static class RecyclerViewHolder extends RecyclerView.ViewHolder {

        private TextView mLabel;

        RecyclerViewHolder(View view) {
            super(view);
            mLabel=view.findViewById(R.id.linkki);
        }

    }

    private ArrayList<String> mArrayList;
    private Singleton mSing;

    public RecyclerViewAdapter2(ArrayList<String> linkit) {
        mArrayList=linkit;
        mSing=annaIlmentyma();
    }

    @NonNull
    @Override
    public RecyclerViewHolder onCreateViewHolder(@NonNull ViewGroup viewGroup, int i) {
        View view= LayoutInflater.from(viewGroup.getContext()).inflate(
                R.layout.linkki_lista_rivi, viewGroup, false);
        return new RecyclerViewHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull RecyclerViewHolder recyclerViewHolder, final int i) {
        recyclerViewHolder.mLabel.setText(mArrayList.get(i));

        //ensimmäinen rivi, jossa on kyseisen solidin nimi, ei saa olla klikattava
        if (i!=0) {

            //muut rivit tarvitsevat tapahtumakuuntelijan
            View.OnClickListener siirto = new View.OnClickListener() {
                @Override
                public void onClick(View view) {

                    //ensin on selvitettävä, onko klikattu tekstilinkkiä vai CAD linkkiä
                    String linkki = mArrayList.get(i);
                    if (linkki.charAt(0) == 'C') {
                        avaaCad(linkki);
                    }
                    if (linkki.charAt(0) == 'T') {
                        avaaTeksti(linkki);
                    }

                    //Lopuksi LinkkiLista ikkuna pitää sulkea
                    mSing.mAktiviteetti1.mLinkkiListaManager.mDialog.dismiss();
                }
            };
            recyclerViewHolder.mLabel.setOnClickListener(siirto);
        }
    }

    @Override
    public int getItemCount() {
        return mArrayList==null ? 0 : mArrayList.size();
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

        //tehdään muunnos UTF8 -> ISO_8859_1
        tiedosto=UTF8toISO(tiedosto);

        //CADruutu on varmasti olemassa ja esillä, joten uuden CAD tiedoston avaaminen on
        // yksinkertaista
        mSing.mAktiviteetti1.mCADTiedosto=tiedosto;
        mSing.mAktiviteetti1.mFragmentti3.avaaCAD();
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

        //poistetaan mahdollinen tarkenne
        for (int i=0; i<tiedosto.length(); i++) {
            if (tiedosto.charAt(i) == '.') {
                tiedosto = tiedosto.substring(0, i);
            }
        }

        //valitaan englanti tai suomi eli lisätään tarkenne
        if (mSing.mAktiviteetti1.mEnglanti==true) {
            tiedosto = tiedosto + ".eng";
        } else {
            tiedosto = tiedosto + ".fin";
        }

        if (toinenHashtac!=-1) {
            String ankkuri;

            //määritetään mahdollinen ankkuri käyttäjän kielivalinnan mukaan
            if (mSing.mAktiviteetti1.mEnglanti==true) {
                ankkuri = linkki.substring(toinenHashtac, kolmasHashtac);
            } else {
                ankkuri = linkki.substring(kolmasHashtac);
            }

            //alaviivat pitää ankkurissa muuttaa välilyönneiksi
            ankkuri = ankkuri.replaceAll("_", " ");

            //liitetään ankkuri tiedostonimen perään
            if (ankkuri.length() > 0) tiedosto = tiedosto + ankkuri;
        }

        //tehdään muunnos UTF8 -> ISO_8859_1
        tiedosto=UTF8toISO(tiedosto);

        //Tekstiruutu voi olla olemassa ja esillä, tai sitten ei, joten koodia tarvitaan enemmän
        mSing.mAktiviteetti1.mTekstiTiedosto=tiedosto;

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
            //tekstiruutu pitää ensin siirtää esille
            if (null==siirto.annaNakyma().getParent()) {

                //Ensiksi CADruudun pitää mahdollisesti järjestää asiansa
                if (mSing.mAktiviteetti1.mFragmentti3!=null
                        && null!=mSing.mAktiviteetti1.mFragmentti3.annaNakyma().getParent()) {
                    mSing.mAktiviteetti1.mSurf.sailytaSaie();
                }

                if (!mSing.mAktiviteetti1.mSisaltoVasen.equals(
                        mSing.mAktiviteetti1.mFragmentti2.annaNakyma().getParent())) {
                    FragmentTransaction fragmentTransaction =
                            mSing.mAktiviteetti1.mFragmentManager.beginTransaction();
                    fragmentTransaction.remove(mSing.mAktiviteetti1.mFragmentti2);
                    fragmentTransaction.commit();
                    mSing.mAktiviteetti1.mFragmentManager.executePendingTransactions();
//                            mFragmentti3=(CADRuutu) luoUudestaan(mFragmentti3);
                    fragmentTransaction = mSing.mAktiviteetti1.mFragmentManager.beginTransaction();
                    fragmentTransaction.replace(R.id.sisaltoVasen,
                            mSing.mAktiviteetti1.mFragmentti2, "fragmentti2");
                    fragmentTransaction.disallowAddToBackStack();
                    fragmentTransaction.commit();
                }

                //Sitten avataan uusi HTML-sivu
                siirto.avaaTeksti();
            } else {

                //Sitten avataan uusi HTML-sivu
                siirto.avaaTeksti();
            }
        } else {

            //muuten tekstiruutu täytyy luoda, joka samalla avaa HTML-sivun
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

    private void asetaLandscape() {

        //ensiksi meidän pitää antaa puolet näyttöruudusta oikeanpuoleiselle fragmentille
        View siirto=mSing.mAktiviteetti1.mSisaltoOikea;
        LinearLayout.LayoutParams parametrit=(LinearLayout.LayoutParams) siirto.getLayoutParams();
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
                    && parent1.equals(mSing.mAktiviteetti1.mFragmentti3.annaNakyma().getParent())) {
                mSing.mAktiviteetti1.mSurf.sailytaSaie();
            }

            parent1.removeView(siirto);
            parent2=mSing.mAktiviteetti1.mSisaltoOikea;
            ViewGroup view=(ViewGroup) parent2.getChildAt(0);
            view.removeAllViews();
            parent2.addView(siirto);
        }
    }

    //tämä funktio muuttaa UTF8 merkkijonon ANSI merkkijonoksi, joka kelpaa Kirjasto1:lle ja
    // Kirjasto2:lle
    private String UTF8toISO(String s1) {
        if (s1==null) {
            return null;
        }
        if (Build.VERSION.SDK_INT >= 19) {
            String s2 = new String(s1.getBytes(StandardCharsets.UTF_8));
            byte[] b1 = s2.getBytes(StandardCharsets.ISO_8859_1);
            return new String(b1, StandardCharsets.ISO_8859_1);
        }
        return s1;
    }

}
