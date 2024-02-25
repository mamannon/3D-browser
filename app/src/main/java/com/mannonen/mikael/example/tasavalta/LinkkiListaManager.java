package com.mannonen.mikael.example.tasavalta;

import android.app.Dialog;

import java.util.ArrayList;

import static com.mannonen.mikael.example.tasavalta.Singleton.annaIlmentyma;

public class LinkkiListaManager {

    public ArrayList<String> mLinkit;
    public Dialog mDialog;
    private Singleton mSing;

    public LinkkiListaManager() {

        //Ilmoittaudutaan Aktiviteetti1:een
        mSing=annaIlmentyma();
        mSing.mAktiviteetti1.mLinkkiListaManager=this;

        //alustetaan taulukko
        mLinkit=new ArrayList<>();

    }

    //Tätä funktiota kutsutaan Kirjasto1:stä käsin
    private void avataanLinkkiLista() {

        //Linkkilista on avattava UI säikeessä
        final Runnable r=new Runnable() {

            @Override
            public void run() {
                if (mLinkit!=null) {
                    LinkkiLista l=new LinkkiLista();
                    l.show(mSing.mAktiviteetti1.mFragmentManager, "LinkkiLista");
                }
            }

        };
        mSing.mAktiviteetti1.runOnUiThread(r);

    }

}