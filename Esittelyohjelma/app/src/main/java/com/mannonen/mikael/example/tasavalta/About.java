package com.mannonen.mikael.example.tasavalta;

import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

public class About extends Fragment {

    private View mNakyma;

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
