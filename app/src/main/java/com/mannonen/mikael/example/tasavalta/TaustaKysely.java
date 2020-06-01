package com.mannonen.mikael.example.tasavalta;

import android.app.AlertDialog;
import android.app.Dialog;
import android.content.DialogInterface;
import android.os.Bundle;
import android.support.annotation.NonNull;
import android.support.v4.app.DialogFragment;

import static com.mannonen.mikael.example.tasavalta.Singleton.annaIlmentyma;

public class TaustaKysely extends DialogFragment {

    private Singleton mSing;

    public TaustaKysely() {
        mSing=annaIlmentyma();
    }

    @NonNull
    @Override
    public Dialog onCreateDialog(Bundle bundle) {

        AlertDialog.Builder builder=new AlertDialog.Builder(getContext());
        builder.setTitle(R.string.tausta_kysely_otsikko);

        DialogInterface.OnClickListener kuuntelija1=new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialogInterface, int i) {
                mSing.mAktiviteetti1.mFragmentti3.tulostetaan(true);
            }
        };
        builder.setPositiveButton(R.string.Kylla, kuuntelija1);

        DialogInterface.OnClickListener kuuntelija2=new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialogInterface, int i) {
                mSing.mAktiviteetti1.mFragmentti3.tulostetaan(false);
            }
        };
        builder.setNegativeButton(R.string.Ei, kuuntelija2);

        DialogInterface.OnClickListener kuuntelija3=new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialogInterface, int i) {
                //Älä tee mitään
            }
        };
        builder.setNeutralButton(R.string.peruuta, kuuntelija3);

        return builder.create();
    }

}
