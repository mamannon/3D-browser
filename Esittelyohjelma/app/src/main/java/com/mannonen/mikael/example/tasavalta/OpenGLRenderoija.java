package com.mannonen.mikael.example.tasavalta;

import android.graphics.Rect;
import android.view.Choreographer;
import android.widget.Toast;
import javax.microedition.khronos.egl.EGLConfig;
import javax.microedition.khronos.opengles.GL10;

import static com.mannonen.mikael.example.tasavalta.Singleton.annaIlmentyma;
import static com.mannonen.mikael.example.tasavalta.CADRuutu.openglOnkoTuettu;

public class OpenGLRenderoija implements OmaSurfaceView.Renderer, Choreographer.FrameCallback {

    private Singleton mSing;
    private boolean mDoFrameValmis=false;
    private long mAika=0;
    private boolean mOnkoAika=true;
    private boolean mSaakoRenderoida=false;
    private boolean mOnkoValmisteltu=false;

    public OpenGLRenderoija() {

        //pyydetään Singleton luokan ilmentymä
        mSing = annaIlmentyma();

        //ilmoittaudutaan aktiviteetti1:een
        mSing.mAktiviteetti1.mOpenGLRenderoija=this;

        //pyydetään Choreographerilta ensimmäinen callback
        Choreographer.getInstance().postFrameCallback(this);
    }

    //tätä funktiota Choreographer kutsuu UI-threadista
    @Override
    public void doFrame(long nanoSeconds) {

        //ensin pyydetään seuraava callback
        Choreographer.getInstance().postFrameCallback(this);

        //lasketaan aikadelta
        long diff;
        if (mOnkoAika) {
            diff = System.nanoTime() - nanoSeconds;
        } else {
            diff = System.nanoTime() - mAika;
        }
        if (diff<31000000) {
            mOnkoAika=false;
            if (mAika==0) mAika=nanoSeconds;
        } else {
            mOnkoAika=true;
            mAika=0;
        }

        //sitten piirretään uusi ruutu, jos edellinen on piirretty ja aikaa on kulunut
        //vähintään 31 millisekuntia edellisestä piirtämisestä
        if (mDoFrameValmis && mOnkoAika && mSaakoRenderoida) {
            mDoFrameValmis=false;
            final Runnable r = new Runnable() {
                @Override
                public void run() {
                    mSing.mAktiviteetti1.mSurf.requestRender();
                }
            };
            mSing.mAktiviteetti1.mSurf.queueEvent(r);
        }
    }

    public boolean doPermitRender(boolean permit) {
        if (mOnkoValmisteltu) {
            mSaakoRenderoida = permit;
            return true;
        } else {
            mSaakoRenderoida = false;
            return false;
        }
    }

    //kaikkia alla olevia funktioita kutsutaan renderthreadista
    public void onSurfaceCreated(GL10 gl, EGLConfig config) {
        mDoFrameValmis=true;
    }

    public void onDrawFrame(GL10 unused) {

        if (mSaakoRenderoida) {
            mSing.mAktiviteetti1.mFragmentti3.paivita();
        }
        mDoFrameValmis = true;

    }

    public void onSurfaceChanged(GL10 unused, int width, int height) {

        //määritetään OpenGL näkymän täsmällinen koko
        mSing.mAktiviteetti1.mSurf.getHolder().setSizeFromLayout();
        Rect rect=mSing.mAktiviteetti1.mSurf.getHolder().getSurfaceFrame();
        if (rect.width()!=0 && rect.height()!=0) {
            mSing.mAktiviteetti1.mFragmentti3.asetaNakymanKoko(rect.width(), rect.height());
            width=rect.width();
            height=rect.height();
        }
        mSing.mAktiviteetti1.mFragmentti3.muutaAla(width, height);

    }

    public void onSetup(GL10 unused) {

        //määritetään OpenGL näkymän täsmällinen koko
        mSing.mAktiviteetti1.mSurf.getHolder().setSizeFromLayout();
        Rect rect=mSing.mAktiviteetti1.mSurf.getHolder().getSurfaceFrame();
        if (rect.width()!=0 && rect.height()!=0) {
            mSing.mAktiviteetti1.mFragmentti3.asetaNakymanKoko(rect.width(), rect.height());
        }

        //suoritetaan valmistelutoimet
        int optio=mSing.mAktiviteetti1.mFragmentti1.annaOptio();
        optio = mSing.mAktiviteetti1.mFragmentti3.onkoLaajennuksia(optio);
        mSing.mAktiviteetti1.mFragmentti1.asetaOptio(optio);
        if (optio==0) {
            openglOnkoTuettu=false;
        } else {
            openglOnkoTuettu=true;
        }

        //merkitään valmistelutoimet suoritetuiksi
        mOnkoValmisteltu=true;
    }

    public void onLoadData(GL10 unused) {
        if (openglOnkoTuettu) {
            mSing.mAktiviteetti1.mFragmentti3.avataan();
        } else {
            Toast toast=Toast.makeText(mSing.mAktiviteetti1.getApplicationContext(),
                    "Unable to launch OpenGL ES!",
                    Toast.LENGTH_LONG);
            toast.show();
            openglOnkoTuettu=false;
        }
    }

}
