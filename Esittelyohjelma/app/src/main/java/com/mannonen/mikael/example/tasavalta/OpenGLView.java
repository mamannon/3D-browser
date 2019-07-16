package com.mannonen.mikael.example.tasavalta;

import android.content.Context;
import android.opengl.GLES20;
import android.util.AttributeSet;
import android.view.MotionEvent;
import android.widget.Toast;

import javax.microedition.khronos.egl.EGL10;
import javax.microedition.khronos.egl.EGLConfig;
import javax.microedition.khronos.egl.EGLDisplay;

import static com.mannonen.mikael.example.tasavalta.Singleton.annaIlmentyma;
import static java.lang.Math.abs;

public class OpenGLView extends OmaSurfaceView {

    private static final float TOLERANCE=10;
    private OpenGLRenderoija mRenderoija;
    private Singleton mSing;
    private float X1, Y1, X2, Y2;
    private int mSormia=1;

    class MultisampleConfigChooser2 implements OmaSurfaceView.EGLConfigChooser {

        @Override
        public EGLConfig chooseConfig(EGL10 egl, EGLDisplay display) {

            //yritetään löytää normaali 4xMSAA multisample pikseliformaatti
            int[] siirto=new int[1];
            int[] attributes = {
                    EGL10.EGL_RED_SIZE, 5,
                    EGL10.EGL_GREEN_SIZE, 6,
                    EGL10.EGL_BLUE_SIZE, 5,
                    EGL10.EGL_DEPTH_SIZE, 16,
                    EGL10.EGL_RENDERABLE_TYPE, 4,
                    EGL10.EGL_SAMPLE_BUFFERS, 1,
                    EGL10.EGL_SAMPLES, 4,
                    EGL10.EGL_NONE
            };
            if (!egl.eglChooseConfig(display, attributes, null, 0, siirto) ||
                    siirto[0]<=0) {

                //eme saaneet 4xMSAA multisamplea. yritetään 2xMSAA
                attributes = new int[]{
                        EGL10.EGL_RED_SIZE, 5,
                        EGL10.EGL_GREEN_SIZE, 6,
                        EGL10.EGL_BLUE_SIZE, 5,
                        EGL10.EGL_DEPTH_SIZE, 16,
                        EGL10.EGL_RENDERABLE_TYPE, 4,
                        EGL10.EGL_SAMPLE_BUFFERS, 1,
                        EGL10.EGL_SAMPLES, 2,
                        EGL10.EGL_NONE
                };
                if (!egl.eglChooseConfig(display, attributes, null, 0, siirto) ||
                        siirto[0]<=0) {

                    //emme saaneet 2xMSAA multisamplea. Yritetään vielä nVidia Tegra2 multisamplea
                    final int EGL_COVERAGE_BUFFERS_NV = 0x30E0;
                    final int EGL_COVERAGE_SAMPLES_NV = 0x30E1;
                    attributes = new int[]{
                            EGL10.EGL_RED_SIZE, 5,
                            EGL10.EGL_GREEN_SIZE, 6,
                            EGL10.EGL_BLUE_SIZE, 5,
                            EGL10.EGL_DEPTH_SIZE, 16,
                            EGL10.EGL_RENDERABLE_TYPE, 4,
                            EGL_COVERAGE_BUFFERS_NV, 1,
                            EGL_COVERAGE_SAMPLES_NV, 2,
                            EGL10.EGL_NONE
                    };
                    if (!egl.eglChooseConfig(display, attributes, null, 0, siirto)
                            || siirto[0] <= 0) {

                        //emme saaneet multisamplea. Yritetään ilman multisamplea
                        attributes = new int[]{
                                EGL10.EGL_RED_SIZE, 5,
                                EGL10.EGL_GREEN_SIZE, 6,
                                EGL10.EGL_BLUE_SIZE, 5,
                                EGL10.EGL_DEPTH_SIZE, 16,
                                EGL10.EGL_RENDERABLE_TYPE, 4,
                                EGL10.EGL_NONE
                        };
                        egl.eglChooseConfig(display, attributes, null, 0, siirto);
                    }
                }
            }

            //seuraavaksi saaduista konfiguraatioista valitaan sellainen, jonka RGB-arvot ovat 5-6-5
            //eivätkä 8-8-8. Sitä varten luodaan kaikki tarjolla olevat konfiguraatiot
            int konfiguraatioita=siirto[0];
            EGLConfig[] konfiguraatiot=new EGLConfig[konfiguraatioita];
            if (!egl.eglChooseConfig(display, attributes, konfiguraatiot, konfiguraatioita,
                    siirto)) {
                throw new IllegalArgumentException("eglChooseConfig failed!");
            }

            //käydään läpi konfiguraatiot
            int index=-1;
            for (int i=0; i<konfiguraatioita; i++) {
                if (egl.eglGetConfigAttrib(display, konfiguraatiot[i],
                        EGL10.EGL_RED_SIZE, siirto)) {
                    if (siirto[0]==5) {
                        index=i;
                        break;
                    }
                }
            }

            //palutetaan sopiva konfiguraatio
            if (index==-1) {
                return konfiguraatiot[0];
            } else {
                return konfiguraatiot[index];
            }
        }
    }

    class MultisampleConfigChooser3 implements OmaSurfaceView.EGLConfigChooser {


        int[] attributes1 = {
                EGL10.EGL_RED_SIZE, 8,
                EGL10.EGL_GREEN_SIZE, 8,
                EGL10.EGL_BLUE_SIZE, 8,
                EGL10.EGL_ALPHA_SIZE, 8,
                EGL10.EGL_DEPTH_SIZE, 24,
                EGL10.EGL_STENCIL_SIZE, 8,
                EGL10.EGL_RENDERABLE_TYPE, 4,
                EGL10.EGL_SAMPLE_BUFFERS, 1,
                EGL10.EGL_SAMPLES, 4,
                EGL10.EGL_NONE
        };

        int[] attributes2 = {
                EGL10.EGL_RED_SIZE, 8,
                EGL10.EGL_GREEN_SIZE, 8,
                EGL10.EGL_BLUE_SIZE, 8,
                EGL10.EGL_ALPHA_SIZE, 8,
                EGL10.EGL_DEPTH_SIZE, 24,
                EGL10.EGL_STENCIL_SIZE, 8,
                EGL10.EGL_RENDERABLE_TYPE, 4,
                EGL10.EGL_NONE
        };

        @Override
        public EGLConfig chooseConfig(EGL10 egl, EGLDisplay display) {

            int[] siirto=new int[1];
            EGLConfig[] konfiguraatio=new EGLConfig[1];
            if (!egl.eglChooseConfig(display, attributes1, konfiguraatio, 1, siirto) ||
                    siirto[0]<=0) {
                if (!egl.eglChooseConfig(display, attributes2, konfiguraatio, 1, siirto) ||
                        siirto[0]<=0) {
                    throw new IllegalArgumentException("eglChooseConfig failed!");
                }
            }
            return konfiguraatio[0];

        }
    }

    private void asetaRenderoija(Context context) {
        try {

            //Selvitetään OpenGL versio
            float siirto1=2.0f;
            float siirto2=2.0f;

            String versio= GLES20.glGetString(GLES20.GL_VERSION);
            if (versio!=null) {
                siirto1=parseFloat(versio);
            }
            versio=GLES20.glGetString(GLES20.GL_SHADING_LANGUAGE_VERSION);
            if (versio!=null) {
                siirto2 = parseFloat(versio);
            }

            //luodaan OpenGL konteksti. Jos laite tukee OpenGL ES 3.2 ja GLSL ES 3.2 ja
            int optio=mSing.mAktiviteetti1.mFragmentti1.annaOptio();
            if ((siirto1>=3.2 && siirto2>=3.2) || optio==2) {
                if (mSing.mAktiviteetti1.eiValaisua || mSing.mAktiviteetti1.mLowMemory) {

                    if (mSing.mAktiviteetti1.mLowMemory) {
                        Toast toast=Toast.makeText(getContext(), "Using simple graphics because of low memory.", Toast.LENGTH_LONG);
                        toast.show();
                    }

                    //jos käyttäjä haluaa yksinkertaista grafiikkaa tai muistia on vähän...
                    setEGLContextClientVersion(2);
                    setEGLConfigChooser(new MultisampleConfigChooser2());
                } else {

                    //jos käyttäjä haluaa hienoa grafiikkaa...
                    setEGLContextClientVersion(3);
                    setEGLConfigChooser(new MultisampleConfigChooser3());
                }
            } else {

                //Tämä laite ei sisällä hienoon grafiikkaan tarvittavaa OpenGL ES versiota
                mSing.mAktiviteetti1.mFragmentti1.mGraphics.setEnabled(false);
                setEGLContextClientVersion(2);
                setEGLConfigChooser(new MultisampleConfigChooser2());
            }

            //luodaan renderöijä GLSurfaceView:lle
            mRenderoija = new OpenGLRenderoija();
            setRenderer(mRenderoija);

            //määritetään renderöintityyliksi renderöinti erillisestä pyynnöstä
            setRenderMode(OmaSurfaceView.RENDERMODE_WHEN_DIRTY);

        } catch (Exception e) {

            //jokin on pielessä
            Toast toast = Toast.makeText(context, "Unable to create GLES context!",
                    Toast.LENGTH_LONG);
            toast.show();
            mSing.mAktiviteetti1.mFragmentti3.openglOnkoTuettu = false;
        }
    }

    private float parseFloat(String versio) {
        int ensimmainen=versio.indexOf(' ');
        int toinen=versio.indexOf(' ', ensimmainen+1);
        int kolmas=versio.indexOf(' ', toinen+1);
        String patka=versio.substring(toinen+1, kolmas);
        try {
            return Float.parseFloat(patka);
        } catch (Exception e) {
            return 2.0f;
        }
    }

    public OpenGLView(Context context, AttributeSet attrs) {
        super(context, attrs);

        //pyydetään Singleton luokan ilmentymä
        mSing = annaIlmentyma();
        asetaRenderoija(context);
    }

    public OpenGLView(Context context) {
        super(context);

        //pyydetään Singleton luokan ilmentymä
        mSing = annaIlmentyma();
        asetaRenderoija(context);
    }

    @Override
    public void onPause() {
        super.onPause();

    }

    @Override
    public void onResume() {
        super.onResume();
    }

    @Override
    public boolean onTouchEvent(MotionEvent event) {

        float x1=event.getX(0);
        float y1=event.getY(0);
        float x2=0;
        float y2=0;
//        float x3=0;
//        float y3=0;
        if (event.getPointerCount() == 1) {
            if (mSormia==2) {
                mSormia = 0;
            } else {
                mSormia = 1;
            }
        }
        if (event.getPointerCount() == 2) {
            x2 = event.getX(1);
            y2 = event.getY(1);
//            if (mSormia==3) {
//                mSormia = 0;
//            } else {
//                mSormia = 2;
//            }
            mSormia=2;
        }
        if (event.getPointerCount() > 2) {
//            x3 = event.getX(2);
//            y3 = event.getY(2);
            mSormia=3;
        }


        switch (event.getAction()) {

            case MotionEvent.ACTION_DOWN:
                X1=x1;
                Y1=y1;
                mSormia=1;
                break;

            case MotionEvent.ACTION_MOVE:
                if (mSormia==1) {

                    //jos vain yhtä sormea käytetään, käyttäjä kääntää kuvakulmaa 3D-maailmassa
                    mSing.mAktiviteetti1.mFragmentti3.hiiriLiikkuuVasen((int) x1, (int) y1,
                            (int) X1, (int) Y1);
                    X1=x1;
                    Y1=y1;
                    break;

                }
                if (mSormia==2){

                    //jos kaksi sormea on käytössä, käyttäjä haluaa liikkua 3D-maailmassa
                    if (Math.sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2)) >
                                    Math.sqrt((X1 - X2) * (X1 - X2) + (Y1 - Y2) * (Y1 - Y2))) {

                        //käyttäjä kulkee eteenpäin 3D-maailmassa
                        mSing.mAktiviteetti1.mFragmentti3.pyoraYlos();
                    } else {

                        //käyttäjä peruuttaa 3D-maailmassa
                        mSing.mAktiviteetti1.mFragmentti3.pyoraAlas();
                    }
                    X1=x1;
                    X2=x2;
                    Y1=y1;
                    Y2=y2;
                    break;
                }
                if (mSormia==3) {

                    //jos kolme sormea on näytöllä, käyttäjä liikkuu 3D-maailmassa 'hississä'
                    mSing.mAktiviteetti1.mFragmentti3.hiiriLiikkuuMolemmat((int) x1, (int) y1,
                                (int) X1, (int) Y1);
                    X1 = x1;
                    X2 = x2;
                    Y1 = y1;
                    Y2 = y2;
                    break;
                }

            case MotionEvent.ACTION_UP:

                if (abs(x1-X1)<TOLERANCE && abs(y1-Y1)<TOLERANCE && mSormia==0) {

                    //jos aiemmin näytölle laskettu sormi on paikallaan...
                    mSing.mAktiviteetti1.mFragmentti3.hiiriLiikkuuOikea((int) x1, (int) y1);
                } else {

                    //kakkossormesta tulee ykkössormi
                    X1 = X2;
                    Y1 = Y2;
/*
                    //kolmossormesta tulee ykkössormi
                    if (x1==0.0 && y1==0.0 && x2==0.0 && y2==0.0) {
                        X1 = x3;
                        Y1 = y3;
                    }
 */
                }
                mSormia=1;
                break;

            case MotionEvent.ACTION_CANCEL:
                mSormia=1;
                break;

            case MotionEvent.ACTION_OUTSIDE:
                mSormia=1;

                //käyttäjä siirtää sormensa 3D-näkymän ulkopuolelle
                mSing.mAktiviteetti1.mFragmentti3.hiiriPoistuu();
                break;
        }
        return true;
    }



}
