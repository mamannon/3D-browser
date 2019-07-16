package com.mannonen.mikael.example.tasavalta;

import android.app.AlertDialog;
import android.app.Dialog;
import android.content.Context;
import android.content.DialogInterface;
import android.os.Bundle;
import android.support.annotation.NonNull;
import android.support.v4.app.DialogFragment;
import android.support.v7.widget.LinearLayoutManager;
import android.support.v7.widget.RecyclerView;
import android.view.LayoutInflater;
import android.view.View;

import java.util.ArrayList;
import static com.mannonen.mikael.example.tasavalta.Singleton.annaIlmentyma;

public class ValintaLista extends DialogFragment {

    private Context mKonteksti;
    private LayoutInflater mInflater;
    private ArrayList<String> mValintaListaC1;
    private ArrayList<Boolean> mValintaListaB1;
    private ArrayList<String> mValintaListaC2;
    private ArrayList<Boolean> mValintaListaB2;
    private Singleton mSing;

    public ValintaLista() {
        mSing=annaIlmentyma();
        mInflater=mSing.mAktiviteetti1.getLayoutInflater();
        mValintaListaC1=mSing.mAktiviteetti1.mFragmentti3.valintaListaC1;
        mValintaListaB1=mSing.mAktiviteetti1.mFragmentti3.valintaListaB1;
        mValintaListaC2=mSing.mAktiviteetti1.mFragmentti3.valintaListaC2;
        mValintaListaB2=mSing.mAktiviteetti1.mFragmentti3.valintaListaB2;
        mKonteksti=mSing.mAktiviteetti1.getApplicationContext();
    }

    @NonNull
    @Override
    public Dialog onCreateDialog(Bundle savedInstanceState) {

        //luodaan dialogi käyttäen builder apuluokkaa
        AlertDialog.Builder builder=new AlertDialog.Builder(getContext());

        //ValintaLista dialogin otsikkoteksti
        builder.setTitle(R.string.valinta_lista_otsikko);

        //nappula, jolla käyttäjä vahvistaa tekemänsä muutokset
        DialogInterface.OnClickListener kuuntelija1=new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialogInterface, int i) {

                //Pyydetään Kirjasto1:ä päivittämään näkymä valintojen mukaiseksi
                mSing.mAktiviteetti1.mFragmentti3.vieValinnat();
            }
        };
        builder.setPositiveButton(R.string.OK, kuuntelija1);

        //luodaan valintalistat (2 kappaletta) dialogiin
        View view=(View) mInflater.inflate(R.layout.fragment_valintalista, null);
        builder.setView(view);
        RecyclerView rv1=(RecyclerView) view.findViewById(R.id.valintaLista1);
        rv1.setNestedScrollingEnabled(false);
        rv1.setLayoutManager(new LinearLayoutManager(mKonteksti));
        RecyclerViewAdapter rva1=new RecyclerViewAdapter(mValintaListaC1, mValintaListaB1,
                false);
        rv1.setAdapter(rva1);
        RecyclerView rv2=(RecyclerView) view.findViewById(R.id.valintaLista2);
        rv2.setNestedScrollingEnabled(false);
        rv2.setLayoutManager(new LinearLayoutManager(mKonteksti));
        RecyclerViewAdapter rva2=new RecyclerViewAdapter(mValintaListaC2, mValintaListaB2,
                true);
        rv2.setAdapter(rva2);

        //lopuksi dialogi luodaan ja palautetaan
        return builder.create();
    }

}
