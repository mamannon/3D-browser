package com.mannonen.mikael.example.tasavalta;

import android.support.annotation.NonNull;
import android.support.v7.widget.RecyclerView;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;

import java.util.ArrayList;

import static com.mannonen.mikael.example.tasavalta.Singleton.annaIlmentyma;

public class RecyclerViewAdapter3 extends RecyclerView.Adapter
        <RecyclerViewAdapter3.RecyclerViewHolder> {

    static class RecyclerViewHolder extends RecyclerView.ViewHolder {

        private TextView mLabel;

        RecyclerViewHolder(View view) {
            super(view);
            mLabel=view.findViewById(R.id.linkki);
        }

    }

    private ArrayList<String> mArrayList;
    private Singleton mSing;

    public RecyclerViewAdapter3(ArrayList<String> linkit) {
        mArrayList=linkit;
        mSing=annaIlmentyma();
    }

    @NonNull
    @Override
    public RecyclerViewAdapter3.RecyclerViewHolder onCreateViewHolder(@NonNull ViewGroup viewGroup,
                                                                      int i) {
        View view= LayoutInflater.from(viewGroup.getContext()).inflate(
                R.layout.linkki_lista_rivi, viewGroup, false);
        return new RecyclerViewAdapter3.RecyclerViewHolder(view);
    }

    @Override
    public void onBindViewHolder(
            @NonNull RecyclerViewAdapter3.RecyclerViewHolder recyclerViewHolder, final int i) {
        recyclerViewHolder.mLabel.setText(mArrayList.get(i));

        //rivit tarvitsevat tapahtumakuuntelijat
        View.OnClickListener siirto = new View.OnClickListener() {

            @Override
            public void onClick(View view) {

                mSing.mAktiviteetti1.mFragmentti2.siirryAnkkuriin(mArrayList.get(i));
                mSing.mAktiviteetti1.mFragmentti2.piirra();
                mSing.mAktiviteetti1.mFragmentti2.mCan.invalidate();

                //Lopuksi LinkkiLista ikkuna pitää sulkea
                mSing.mAktiviteetti1.mRenki.mDialog.dismiss();
            }
        };
        recyclerViewHolder.mLabel.setOnClickListener(siirto);
    }

    @Override
    public int getItemCount() {
        return mArrayList==null ? 0 : mArrayList.size();
    }

}
