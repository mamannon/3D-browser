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

public class OtsikkoLista extends DialogFragment {

    private Singleton mSing;
    private LayoutInflater mInflater;
    private Context mKonteksti;
    private ArrayList<String> mOtsikkoLista;

    public OtsikkoLista() {
        mSing=annaIlmentyma();
        mInflater=mSing.mAktiviteetti1.getLayoutInflater();
        mKonteksti=mSing.mAktiviteetti1.getApplicationContext();
        mOtsikkoLista=mSing.mAktiviteetti1.mRenki.mOtsikkoLista;
    }

    @NonNull
    @Override
    public Dialog onCreateDialog(Bundle savedInstanceState) {

        //luodaan dialogi käyttäen builder apuluokkaa
        AlertDialog.Builder builder=new AlertDialog.Builder(getContext());

        //ValintaLista dialogin otsikkoteksti
        builder.setTitle(R.string.otsikko_lista_otsikko);

        //nappula, jolla käyttäjä peruuttaa halunsa siirtyä uuteen väliotsikkoon
        DialogInterface.OnClickListener kuuntelija=new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialogInterface, int i) {

                //Älä tee mitään
            }
        };
        builder.setNegativeButton(R.string.peruuta, kuuntelija);

        //luodaan valintalista dialogiin
        View view=(View) mInflater.inflate(R.layout.fragment_linkkilista, null);
        builder.setView(view);
        RecyclerView rv=(RecyclerView) view.findViewById(R.id.linkkilista);
        rv.setLayoutManager(new LinearLayoutManager(mKonteksti));
        RecyclerViewAdapter3 rva=new RecyclerViewAdapter3(mOtsikkoLista);
        rv.setAdapter(rva);

        //lopuksi dialogi luodaan
        Dialog siirto= builder.create();
        mSing.mAktiviteetti1.mRenki.mDialog=siirto;
        return siirto;
    }
}
