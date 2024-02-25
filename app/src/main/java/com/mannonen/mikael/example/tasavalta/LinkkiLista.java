package com.mannonen.mikael.example.tasavalta;

import android.app.AlertDialog;
import android.app.Dialog;
import android.content.Context;
import android.content.DialogInterface;
import android.os.Bundle;
import androidx.annotation.NonNull;
import androidx.fragment.app.DialogFragment;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;
import android.view.LayoutInflater;
import android.view.View;

import java.util.ArrayList;

import static com.mannonen.mikael.example.tasavalta.Singleton.annaIlmentyma;

public class LinkkiLista extends DialogFragment {

    private Singleton mSing;
    private LayoutInflater mInflater;
    private Context mKonteksti;
    private ArrayList<String> mLinkkiLista;

    public LinkkiLista() {
        mSing=annaIlmentyma();
        mInflater=mSing.mAktiviteetti1.getLayoutInflater();
        mKonteksti=mSing.mAktiviteetti1.getApplicationContext();
        mLinkkiLista=mSing.mAktiviteetti1.mLinkkiListaManager.mLinkit;
    }

    @NonNull
    @Override
    public Dialog onCreateDialog(Bundle savedInstanceState) {
        super.onCreateDialog(savedInstanceState);

        //luodaan dialogi käyttäen builder apuluokkaa
        AlertDialog.Builder builder=new AlertDialog.Builder(getContext());

        //ValintaLista dialogin otsikkoteksti
        builder.setTitle(R.string.linkki_lista_otsikko);

        //nappula, jolla käyttäjä peruuttaa halunsa siirtyä uuteen linkkiin
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
        RecyclerViewAdapter2 rva=new RecyclerViewAdapter2(mLinkkiLista);
        rv.setAdapter(rva);

        //lopuksi dialogi luodaan
        Dialog siirto= builder.create();
        mSing.mAktiviteetti1.mLinkkiListaManager.mDialog=siirto;
        return siirto;
    }

}
