package com.mannonen.mikael.example.tasavalta;

import android.app.IntentService;
import android.content.Intent;
import android.os.Build;

import static com.mannonen.mikael.example.tasavalta.Singleton.annaIlmentyma;

public class TaustaTulostus extends IntentService {

    private Singleton mSing;

    public TaustaTulostus() {
        super("taustatulostus");
        mSing=annaIlmentyma();
    }

    @Override
    protected void onHandleIntent(Intent intent) {

        //jos tämä Android-laite käyttää android versiota KitKat (API 19) tai uudempaa
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.KITKAT) {
            //IntentService suorittaa työnsä omassa säikeessään ja kontekstissaan, ei UI-säikeessä
            mSing.mAktiviteetti1.mRenki.asetaPrinttiKonteksti(this.getBaseContext());
            mSing.mAktiviteetti1.mFragmentti2.tulostetaan(mSing.mAktiviteetti1.mTulostusResoluutio);
        }
    }
}