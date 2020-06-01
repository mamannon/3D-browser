package com.mannonen.mikael.example.tasavalta;

import android.support.annotation.NonNull;
import android.support.v7.widget.RecyclerView;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.CheckBox;
import android.widget.TextView;

import java.util.ArrayList;

public class RecyclerViewAdapter extends RecyclerView.Adapter
        <RecyclerViewAdapter.RecyclerViewHolder> {

    static class RecyclerViewHolder extends RecyclerView.ViewHolder {

        private TextView mLabel;
        private CheckBox mCheckBox;

        RecyclerViewHolder(View view) {
            super(view);
            mLabel = (TextView) view.findViewById(R.id.label);
            mCheckBox = (CheckBox) view.findViewById(R.id.checkbox);
        }

    }

    private ArrayList<String> mArrayListC;
    private ArrayList<Boolean> mArrayListB;
    private boolean mMerkki;

    public RecyclerViewAdapter(ArrayList<String> alc, ArrayList<Boolean> alb, boolean merkki) {
        mArrayListC=alc;
        mArrayListB=alb;
        mMerkki=merkki;
    }

    @Override
    public RecyclerViewHolder onCreateViewHolder(ViewGroup viewGroup, int i) {
        View view = LayoutInflater.from(viewGroup.getContext()).inflate(
                R.layout.valinta_lista_rivi, viewGroup, false);
        return new RecyclerViewHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull RecyclerViewHolder holder, final int i) {

        //solidit ja listat erotetaan merkillä ja siksi ne tarvitsevat erilliset adapterit
        if (mMerkki) {
            holder.mLabel.setText("*"+mArrayListC.get(i));
        } else {
            holder.mLabel.setText(mArrayListC.get(i));
        }
        holder.mCheckBox.setChecked(mArrayListB.get(i));

        View.OnClickListener siirto=new View.OnClickListener() {
            @Override
            public void onClick(View v) {

                //solidit ja listat tarvitsevat erilaisen käsittelyn
                if (mMerkki) {
                    checkCheckBox(i, !mArrayListB.get(i));
                } else {

                    //3D- ja 2D-kentät tarvitsevat erilaisen käsittelyn
                    if (mArrayListC.get(3).equals("2D Z-projection")) {

                        //3D-kentässä neljä ensimmäistä vaihtoehtoa ovat toisensa pois sulkevia
                        if (i > 3) {
                            checkCheckBox(i, !mArrayListB.get(i));
                        } else {
                            switch (i) {
                                case 0:
                                    checkCheckBox(0, true);
                                    checkCheckBox(1, false);
                                    checkCheckBox(2, false);
                                    checkCheckBox(3, false);
                                    break;

                                case 1:
                                    checkCheckBox(0, false);
                                    checkCheckBox(1, true);
                                    checkCheckBox(2, false);
                                    checkCheckBox(3, false);
                                    break;

                                case 2:
                                    checkCheckBox(0, false);
                                    checkCheckBox(1, false);
                                    checkCheckBox(2, true);
                                    checkCheckBox(3, false);
                                    break;

                                case 3:
                                    checkCheckBox(0, false);
                                    checkCheckBox(1, false);
                                    checkCheckBox(2, false);
                                    checkCheckBox(3, true);
                            }
                        }
                    } else {

                        //2D-kentässä ei ensimmäistä vaihtoehtoa saa muuttaa
                        if (i > 0) {
                            checkCheckBox(i, !mArrayListB.get(i));
                        }
                    }
                }
            }
        };
        holder.mCheckBox.setOnClickListener(siirto);

        siirto=new View.OnClickListener() {
            @Override
            public void onClick(View v) {

                //solidit ja listat tarvitsevat erilaisen käsittelyn
                if (mMerkki) {
                    checkCheckBox(i, !mArrayListB.get(i));
                } else {

                    //3D- ja 2D-kentät tarvitsevat erilaisen käsittelyn
                    if (mArrayListC.get(3).equals("2D Z-projection")) {

                        //3D-kentässä neljä ensimmäistä vaihtoehtoa ovat toisensa pois sulkevia
                        if (i > 3) {
                            checkCheckBox(i, !mArrayListB.get(i));
                        } else {
                            switch (i) {
                                case 0:
                                    checkCheckBox(0, true);
                                    checkCheckBox(1, false);
                                    checkCheckBox(2, false);
                                    checkCheckBox(3, false);
                                    break;

                                case 1:
                                    checkCheckBox(0, false);
                                    checkCheckBox(1, true);
                                    checkCheckBox(2, false);
                                    checkCheckBox(3, false);
                                    break;

                                case 2:
                                    checkCheckBox(0, false);
                                    checkCheckBox(1, false);
                                    checkCheckBox(2, true);
                                    checkCheckBox(3, false);
                                    break;

                                case 3:
                                    checkCheckBox(0, false);
                                    checkCheckBox(1, false);
                                    checkCheckBox(2, false);
                                    checkCheckBox(3, true);
                            }
                        }
                    } else {

                        //2D-kentässä ei ensimmäistä vaihtoehtoa saa muuttaa
                        if (i > 0) {
                            checkCheckBox(i, !mArrayListB.get(i));
                        }
                    }
                }
            }
        };
        holder.mLabel.setOnClickListener(siirto);

    }

    @Override
    public int getItemCount() {
        return mArrayListC==null ? 0 : mArrayListC.size();
    }

    private void checkCheckBox(int position, boolean value) {

        mArrayListB.set(position, value);
        notifyDataSetChanged();
    }

}
