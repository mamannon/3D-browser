package com.mannonen.mikael.example.tasavalta;

import android.app.AlertDialog;
import android.app.Dialog;
import android.content.DialogInterface;
import android.content.Intent;
import android.os.Bundle;
import android.support.annotation.NonNull;
import android.support.v4.app.DialogFragment;
import android.view.LayoutInflater;
import android.view.View;
import android.widget.RadioButton;

import static com.mannonen.mikael.example.tasavalta.Singleton.annaIlmentyma;

public class ResoluutioLista extends DialogFragment {

    private Singleton mSing;
    private LayoutInflater mInflater;

    public ResoluutioLista() {
        mSing=annaIlmentyma();
        mInflater=mSing.mAktiviteetti1.getLayoutInflater();
        mSing.mAktiviteetti1.mTulostusResoluutio=100;
    }

    @NonNull
    @Override
    public Dialog onCreateDialog(Bundle bundle) {

        //luodaan dialogi käyttäen builder apuluokkaa
        AlertDialog.Builder builder=new AlertDialog.Builder(getContext());

        //ValintaLista dialogin otsikkoteksti
        builder.setTitle(R.string.resoluutio_lista_otsikko);

        //nappula, jolla käyttäjä vahvistaa resoluutiovalintansa
        DialogInterface.OnClickListener kuuntelija1=new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialogInterface, int i) {

                //tehdään tulostus erillisessä intentissä, jotta tulostaminen ei hyydytä näyttöä.
                mSing.mAktiviteetti1.startService(
                        new Intent(mSing.mAktiviteetti1.getBaseContext(), TaustaTulostus.class));
            }
        };
        builder.setPositiveButton(R.string.OK, kuuntelija1);

        //nappula, jolla käyttäjä peruuttaa halunsa tulostaa
        DialogInterface.OnClickListener kuuntelija2=new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialogInterface, int i) {

                //Älä tee mitään
            }
        };
        builder.setNegativeButton(R.string.peruuta, kuuntelija2);

        //luodaan valintalista dialogiin
        View view=(View) mInflater.inflate(R.layout.fragment_resoluutiolista, null);
        builder.setView(view);

        //laitetaan alin resoluutio valmiiksi valituksi
        RadioButton button=view.findViewById(R.id.dpi75);
        button.setChecked(true);

        //lopuksi dialogi luodaan
        return builder.create();
    }
}
