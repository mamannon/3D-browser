package com.mannonen.mikael.example.tasavalta;

import android.support.annotation.NonNull;
import android.support.v4.app.FragmentTransaction;
import android.support.v7.widget.RecyclerView;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.LinearLayout;
import android.widget.TextView;
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
            parent2.removeAllViews();
            parent2.addView(siirto);
        }
    }
}
