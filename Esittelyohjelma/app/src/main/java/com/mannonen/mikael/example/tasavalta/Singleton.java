package com.mannonen.mikael.example.tasavalta;

public final class Singleton {
    private static volatile Singleton ilmentyma=null;

    //Signleton luokka on olemassa vain tätä osoitinta varten
    public Aktiviteetti1 mAktiviteetti1=null;

    //näin Singleton luokan konstruktoria ei voi käyttää
    private Singleton() {}

    //Tämä metodi toimii Singleton luokan konstruktorina, joka luo
    //vain yhden ilmentymän tästä luokasta
    public static Singleton annaIlmentyma() {
        if (ilmentyma==null) {
            synchronized (Singleton.class) {
                if (ilmentyma==null) {
                    ilmentyma=new Singleton();
                }
            }
        }
        return ilmentyma;
    }

    //Asettaa osoittimen, jos sitä ei vielä ole asetettu
    public void asetaAktiviteetti1(Aktiviteetti1 akti) {
        if (mAktiviteetti1==null) {
            mAktiviteetti1=akti;
        }
    }
}
