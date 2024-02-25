package com.mannonen.mikael.example.tasavalta;

import android.os.Bundle;
import androidx.fragment.app.Fragment;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import static com.mannonen.mikael.example.tasavalta.Singleton.annaIlmentyma;

public class About extends Fragment {

    private View mNakyma;
    private Singleton mSing;

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        //ilmoittaudutaan
        mSing=annaIlmentyma();
        mSing.mAktiviteetti1.mFragmentti4=this;
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup parent, Bundle savedInstanceState) {

        // xml tiedostosta luettavat jutut muodostavat View-luokan ilmentymän
        return inflater.inflate(R.layout.fragment_about, parent, false);

    }

    @Override
    public void onViewCreated(View view, Bundle savedInstanceState) {
        mNakyma=view;
    }

    public View annaNakyma() {
        return mNakyma;
    }

}