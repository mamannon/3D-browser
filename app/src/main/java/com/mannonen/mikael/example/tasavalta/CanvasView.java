package com.mannonen.mikael.example.tasavalta;

import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.BitmapShader;
import android.graphics.Canvas;
import android.graphics.Color;
import android.graphics.DashPathEffect;
import android.graphics.Paint;
import android.graphics.Rect;
import android.graphics.RectF;
import android.graphics.Shader;
import android.graphics.Typeface;
import android.os.Handler;
import android.util.AttributeSet;
import android.view.MotionEvent;
import android.view.SurfaceHolder;
import android.view.SurfaceView;

import static com.mannonen.mikael.example.tasavalta.Singleton.annaIlmentyma;

public class CanvasView extends SurfaceView implements SurfaceHolder.Callback {

    private static final float TOLERANCE=5;
    private Singleton mSing;
    private Context mKonteksti;
    private JNIRenki mRenki;
    private Canvas mCanvas;
    private Bitmap mBitmap;
    private SurfaceHolder mSurfaceHolder;
    private float mX, mY;
    private float mXX, mYY;
    private int mAverageCharWidth=0;
    private int mSiveltimenVari=0;
    private int mTekstinVari=0;
    private int mTaustaVari=0;
    private Sivellin mSivellin;
    private Paint mTextPaint;
    private Paint mPenPaint;
    private int mLeveys;
    private int mKorkeus;
    private boolean mTulostetaanko=false;
    private boolean mVoikoScrollata;

    public boolean piirraScrolliMerkit;
    public Handler mHandler;

    public CanvasView(Context c) {
        super(c);
        init(c);
    }

    public CanvasView(Context c, AttributeSet attrs) {
        super(c, attrs);
        init(c);
    }

    public CanvasView(Context c, AttributeSet attrs, int defStyleAttr) {
        super(c, attrs, defStyleAttr);
        init(c);
    }

    private void init(Context c) {
        mKonteksti=c;

        if (mSurfaceHolder==null) {
            mSurfaceHolder=getHolder();
            mSurfaceHolder.addCallback(this);
        }

        //pyydetään Singleton luokan ilmentymä
        mSing=annaIlmentyma();

        //Pyydetään viittaus JNIRenki luokkaan
        mRenki=mSing.mAktiviteetti1.mRenki;

        //selvitetään, onko tämä ilmentymä tulostamista varten vai ei
        mTulostetaanko=mRenki.cTulostetaanko;

        //luodaan tarvittavia jäsenluokkia
        if (mHandler==null) {
            mHandler = new Handler();
        }

        //annetaan jotkin mitat, jotka eivät koskaan ole täsmälleen oikeat
        if (!mTulostetaanko) {
            if (mSing.mAktiviteetti1.mVaakaKerroin == 1) {
                mLeveys = mSing.mAktiviteetti1.mPoint.x;
                mKorkeus = mSing.mAktiviteetti1.mPoint.y;
            } else {
                mLeveys = mSing.mAktiviteetti1.mPoint.y
                        / mSing.mAktiviteetti1.mVaakaKerroin;
                mKorkeus = mSing.mAktiviteetti1.mPoint.x;
            }
        } else {
            mLeveys=mRenki.cLeveys;
            mKorkeus=mRenki.cKorkeus;
        }

        //luodaan lisää tarvittavia jäsenluokkia
        if (mBitmap==null) {
            mBitmap = Bitmap.createBitmap(mLeveys, mKorkeus, Bitmap.Config.ARGB_8888);
        }
        if (mCanvas==null) {
            mCanvas = new Canvas(mBitmap);
        }
    }

    @Override
    protected void onSizeChanged(int w, int h, int oldw, int oldh) {
        if (!mTulostetaanko) {
            super.onSizeChanged(w, h, oldw, oldh);

            //Näiden pitää olla tässä, muuten kaikki voi mennä persiilleen
            if (w <= 0) {
                if (mSing.mAktiviteetti1.mVaakaKerroin == 1) {
                    w = mSing.mAktiviteetti1.mPoint.x;
                } else {
                    w = mSing.mAktiviteetti1.mPoint.y
                            / mSing.mAktiviteetti1.mVaakaKerroin;
                }
            }
            if (h <= 0) {
                if (mSing.mAktiviteetti1.mVaakaKerroin == 1) {
                    h = mSing.mAktiviteetti1.mPoint.y;
                } else {
                    h = mSing.mAktiviteetti1.mPoint.x;
                }
            }

            mLeveys = w;
            mKorkeus = h;
            mBitmap.recycle();
            mBitmap = Bitmap.createBitmap(w, h, Bitmap.Config.ARGB_8888);
            mCanvas.setBitmap(mBitmap);
            mSing.mAktiviteetti1.mFragmentti2.muutaKokoa(w, h);
        }
    }

    @Override
    public void surfaceCreated(SurfaceHolder holder) {
        setWillNotDraw(false);
        Canvas canvas=holder.lockCanvas(null);
        draw(canvas);
        holder.unlockCanvasAndPost(canvas);
    }

    @Override
    public void surfaceChanged(SurfaceHolder holder, int formaatti, int w, int h) {
        Canvas canvas=holder.lockCanvas(null);
        draw(canvas);
        holder.unlockCanvasAndPost(canvas);
    }

    @Override
    public void surfaceDestroyed(SurfaceHolder holder) {

    }

    @Override
    protected void onDraw(Canvas kanvaasi) {
        super.onDraw(kanvaasi);

        //Piirretään aluksi taustavari kanvaasiin (siis ei mBitmappiin)
        kanvaasi.drawColor(mTaustaVari);

        //Kanvaasille piirretyt jutut on erikseen piirrettävä näkyviin
        kanvaasi.drawBitmap(mBitmap, 0, 0, null);

        //Jos skrollataan, piirretään scrollimerkit
        if (piirraScrolliMerkit) {
            piirraScrolliMerkit=false;
            scrolliMerkit(kanvaasi);
        }
    }

    @Override
    public boolean onTouchEvent(MotionEvent event) {
        float x=event.getX();
        float y=event.getY();

        switch (event.getAction()) {
            case MotionEvent.ACTION_DOWN:
                downTouch(x, y);
                invalidate();
                piirraScrolliMerkit=true;
                break;
            case MotionEvent.ACTION_MOVE:
                moveTouch(x, y);
                invalidate();
                piirraScrolliMerkit=true;
                break;
            case MotionEvent.ACTION_UP:
                upTouch(x, y);
                invalidate();
                break;
        }
        return true;
    }

    private void scrolliMerkit(Canvas c) {
        Paint paint=new Paint();
        paint.setColor(Color.GRAY);
        paint.setStyle(Paint.Style.FILL);
        float vaakaK=mSing.mAktiviteetti1.mFragmentti2.siirto3;
        float vaakaA=mSing.mAktiviteetti1.mFragmentti2.siirto5;
        float vaakaR=mSing.mAktiviteetti1.mFragmentti2.siirto7;
        float pystyK=mSing.mAktiviteetti1.mFragmentti2.siirto4;
        float pystyA=mSing.mAktiviteetti1.mFragmentti2.siirto6;
        float pystyR=mSing.mAktiviteetti1.mFragmentti2.siirto8;
        float vaakaKohta=vaakaK/(vaakaA-vaakaR)*vaakaR;
        float pystyKohta=pystyK/(pystyA-pystyR)*pystyR;
        c.drawCircle(vaakaKohta, this.getHeight()-40, 20, paint);
        c.drawCircle(this.getWidth()-40, pystyKohta, 20, paint);
    }

    private void downTouch(float x, float y) {

        //Käyttäjä kosketti laitteen näyttöä
        mHandler.removeCallbacksAndMessages(null);
        mX=x;
        mY=y;
        mXX=x;
        mYY=y;
    }

    private void moveTouch(float x, float y) {

        //käyttäjä koskettaa edelleen laitteen näyttöä
        mHandler.removeCallbacksAndMessages(null);
        float dx=-1*(x-mX);
        float dy=-1*(y-mY);
        mXX=mX;
        mYY=mY;
        mX=x;
        mY=y;
        float ddx=Math.abs(dx);
        float ddy=Math.abs(dy);
        if (ddx>=TOLERANCE || ddy>=TOLERANCE) {

            //Jos käyttäjän siirtää sormeaan näytöllä, siirtyy näkymä mukana
            int X, Y;
            if (ddx > ddy) {
                X = (int) dx;
                Y = 0;
            } else {
                X = 0;
                Y = (int) dy;
            }
            mSing.mAktiviteetti1.mFragmentti2.rullaus(X, Y);
        }
    }

    private void upTouch(float xx, float yy) {
        float dx=-1*(xx-mXX);
        float dy=-1*(yy-mYY);
        float ddx=Math.abs(dx);
        float ddy=Math.abs(dy);
        if (mXX!=mX && mYY!=mY) {

            //jos käyttäjä on pyyhkäissyt näyttöä, kytkeytyy rullaus päälle
            final int X, Y;
            if (ddx > ddy) {
                X = (int) dx;
                Y = 0;
            } else {
                X = 0;
                Y = (int) dy;
            }
            final Runnable r = new Runnable() {
                @Override
                public void run() {

                    //jos scrollaaminen on edelleen mahdollista...
                    if (mVoikoScrollata) {
                        piirraScrolliMerkit = true;
                        mSing.mAktiviteetti1.mFragmentti2.rullaus(X, Y);

                        mHandler.postDelayed(this, 100);
                    }
                }
            };
            mVoikoScrollata=true;
            mHandler.postDelayed(r, 100);
        } else {

            //jos käyttäjä on tökännyt näyttöä, se tulkitaan klikkaukseksi
            mSing.mAktiviteetti1.mFragmentti2.klikkaus((int) xx, (int) yy);
        }
    }

    //Tällä funktiolla saadaan bitmappi
    public Bitmap annaBitmap() {
        return mBitmap;
    }

    //Koko kanvaasin taustaväri asetetaan tällä funktiolla
    public void kokoTausta(int vari) {
        mTaustaVari=vari;
    }

    //Siveltimen väri asetetaan tällä funktiolla
    public int asetaTaustaVari(int vari)
    {
        int siirto=mSiveltimenVari;
        mSiveltimenVari=vari;
        return siirto;
    }

    //Tällä funktiolla kysytään siveltimen väriä
    public int annaTaustaVari() {
        return mSiveltimenVari;
    }

    //Tekstin väri asetetaan tällä funktiolla
    public int asetaTekstinVari(int vari)
    {
        int siirto=mTekstinVari;
        mTekstinVari=vari;
        return siirto;
    }

    //Tällä funktiolla kysytään tekstin väriä
    public int annaTekstinVari() {
        return mTekstinVari;
    }

    //tämä funktio luo fontin luomalla uuden textpaint luokan
    public int luoFontti() {
        String familyName = mRenki.mFaceName;
        Typeface tf = null;

        //jos haluttu fontti on olemassa
        if (!familyName.contains("EI_OLE")) {
            if (familyName.contains("Bold") && familyName.contains("Italic")) {
                tf = Typeface.create(familyName, Typeface.BOLD_ITALIC);
            } else if (familyName.contains("Italic")) {
                tf = Typeface.create(familyName, Typeface.ITALIC);
            } else if (familyName.contains("Bold")) {
                tf = Typeface.create(familyName, Typeface.BOLD);
            } else if (familyName.contains("Mono")) {
                tf = Typeface.MONOSPACE;
            } else {
                tf = Typeface.create(familyName, Typeface.NORMAL);
            }
            /*
            if (mRenki.mWeight == 100 || mRenki.mWeight == 200 || mRenki.mWeight == 300 ||
                    mRenki.mWeight == 400 || mRenki.mWeight == 500 || mRenki.mWeight == 600 ||
                    mRenki.mWeight == 700 || mRenki.mWeight == 800 || mRenki.mWeight == 900)
                tf = Typeface.create(tf, mRenki.mWeight);
*/
            //jos Typeface:n luonti onnistui, määritetään loput fontin ominaisuudet ja tallennetaan
            //fontti hashmappiin
            if (tf != null) {
                Paint paint = new Paint();
                paint.setTypeface(tf);
                if (mRenki.mUnderline != 0) paint.setUnderlineText(true);
                if (mRenki.mStrikeOut != 0) paint.setStrikeThruText(true);
                paint.setTextSize(mRenki.mHeight);
                int koodi = paint.hashCode();
                mRenki.cFontit.put(koodi, paint);
                return koodi;
            }
        }

        //jos haluttua fonttia ei ole tai sen luonti epäonnistui
        if (mRenki.mPitchAndFamily == 2) {
            tf = Typeface.SANS_SERIF;
        } else if (mRenki.mPitchAndFamily == 1 || mRenki.mPitchAndFamily == 5) {
            tf = Typeface.SERIF;
        } else if (mRenki.mPitchAndFamily == 3) {
            tf = Typeface.MONOSPACE;
        } else {
            tf = Typeface.DEFAULT;
        }

        if (mRenki.mItalic==0 && mRenki.mWeight<700) {
            tf = Typeface.create(tf, Typeface.NORMAL);
        } else if (mRenki.mItalic!=0 && mRenki.mWeight>699) {
            tf = Typeface.create(tf, Typeface.BOLD_ITALIC);
        } else if (mRenki.mItalic==0 && mRenki.mWeight>699) {
            tf = Typeface.create(tf, Typeface.BOLD);
        } else if (mRenki.mItalic!=0 && mRenki.mWeight<700) {
            tf = Typeface.create(tf, Typeface.ITALIC);
        }

        //jos Typeface:n luonti onnistui, määritetään loput fontin ominaisuudet ja tallennetaan
        //fontti hashmappiin
        if (tf != null) {
            Paint paint = new Paint();
            paint.setTypeface(tf);
            if (mRenki.mUnderline != 0) paint.setUnderlineText(true);
            if (mRenki.mStrikeOut != 0) paint.setStrikeThruText(true);
            paint.setTextSize(mRenki.mHeight);
            int koodi = paint.hashCode();
            mRenki.cFontit.put(koodi, paint);
            return koodi;
        }

        //jos kaikki vain menee pieleen, palautetaan nolla
        return 0;
    }

    //tämä funktio tuhoaa aiemmin luodun fontin
    public boolean tuhoaFontti(int fontti) {
        Paint siirto=mRenki.cFontit.remove(fontti);
        if (siirto==null) {
            return false;
        } else  {
            return true;
        }
    }

    //tämä funktio asettaa fontin käyttöön
    public int asetaFontti(int fontti) {
        int koodi=0;
        if (mTextPaint!=null) {
            koodi = mTextPaint.hashCode();
        }
        Paint siirto=mRenki.cFontit.get(fontti);
        if (siirto!=null) {
            mTextPaint=siirto;
        }
        return koodi;
    }

    //tämä funktio antaa käytössä olevan fontin mittatietoja
    public void annaTekstiMetriikka() {
        if (mTextPaint!=null) {
            Paint.FontMetricsInt fm = mTextPaint.getFontMetricsInt();
            mRenki.mAscent = (int) fm.ascent;
            mRenki.mDescent = (int) fm.descent;
            mRenki.mInternalLeading = (int) fm.top;
            mRenki.mExternalLeading = (int) fm.leading;
            mRenki.mHeight = (int) (Math.abs(fm.ascent) + Math.abs(fm.descent));
            Typeface tf = mTextPaint.getTypeface();
            mRenki.mItalic = (byte) (tf.isItalic() ? 1 : 0);

            //määritetään keskimääräinen kirjaimen leveys
            Rect rect = new Rect();
            mTextPaint.getTextBounds("AbCdEfGhIj", 0, 9, rect);
            mAverageCharWidth = rect.right / 10;
            mRenki.mAveCharWidth = mAverageCharWidth;
        }
    }

    //tämä funktio antaa annetulle tekstinpätkälle korkeuden ja leveyden
    public void annaTekstinUlottuvuus(String teksti) {
        mRenki.mWidth=(int) mTextPaint.measureText(teksti, 0, teksti.length());
        mRenki.mHeight = Math.abs(mTextPaint.getFontMetricsInt().top)+
                Math.abs(mTextPaint.getFontMetricsInt().descent);
    }

    //tämä funktion piirtää annetun tekstin haluttuun paikkaan kanvaasissa
    public boolean piirraTeksti(int x, int y, String teksti) {

        //asetetaan fontin väri
        mTextPaint.setColor(mTekstinVari);
        int siirto=mTextPaint.getFontMetricsInt().ascent;
        try {
            if (!mTulostetaanko) {
                x = x - mSing.mAktiviteetti1.mFragmentti2.siirto3;
                y = y - mSing.mAktiviteetti1.mFragmentti2.siirto4;
            }
            mCanvas.drawText(teksti, 0, teksti.length(), x, y-siirto, mTextPaint);
        } catch (Exception e) {
            return false;
        }
        return true;
    }

    //tämä funktio luo halutun siveltimen luomalla mSivellin luokan
    public int luoSivellin() {
        Sivellin s=new Sivellin();
        int siirto=0;

        switch (mRenki.mTyyppi) {
            case 'O':
                s.cVari=mRenki.mVari;
                s.cTyyppi=mRenki.mTyyppi;
                siirto=s.hashCode();
                mRenki.cSiveltimet.put(siirto, s);
                return siirto;

            case 'T':
                s.cVari=mRenki.mVari;
                s.cTyyppi=mRenki.mTyyppi;
                siirto=s.hashCode();
                mRenki.cSiveltimet.put(siirto, s);
                return siirto;

            case 'B':
                s.cBitmap=mRenki.mBitmap;
                s.cTyyppi=mRenki.mTyyppi;
                siirto=s.hashCode();
                mRenki.cSiveltimet.put(siirto, s);
                return siirto;
        }
        return 0;
    }

    //tämä funktio asettaa halutun siveltimen käyttöön
    public int asetaSivellin(int sivellin) {
        int koodi = 0;
        if (mSivellin!=null) {
            koodi = mSivellin.hashCode();
        }
        Sivellin siirto=mRenki.cSiveltimet.get(sivellin);

        //valitaan eri sivellintyyppien välillä
        if (siirto!=null) {
            mSivellin=siirto;
            switch (mSivellin.cTyyppi) {
                case 'O':
                    mSiveltimenVari = mSivellin.cVari;
                    break;

                case 'T':
                    mSiveltimenVari = mSivellin.cVari;
                    break;
            }
        }
        return koodi;
    }

    //tämä funktio tuhoaa valitun siveltimen
    public boolean tuhoaSivellin(int sivellin) {
        Sivellin siirto=mRenki.cSiveltimet.remove(sivellin);
        if (siirto==null) {
            return false;
        } else {
            return true;
        }
    }

    //tällä funktiolla täytetään neliö värillä tai kuvalla
    public boolean taytaNelio(int x1, int y1, int x2, int y2) {
        try {
            Paint paint=new Paint();
            if (mSivellin.cTyyppi=='B') {
                paint.setShader(new BitmapShader(mSivellin.cBitmap,
                        Shader.TileMode.REPEAT, Shader.TileMode.REPEAT));
                paint.setStyle(Paint.Style.FILL);
            } else {
                paint.setColor(mSiveltimenVari);
                paint.setStyle(Paint.Style.FILL);
            }
            if (!mTulostetaanko) {
                x1 = x1 - mSing.mAktiviteetti1.mFragmentti2.siirto3;
                y1 = y1 - mSing.mAktiviteetti1.mFragmentti2.siirto4;
                x2 = x2 - mSing.mAktiviteetti1.mFragmentti2.siirto3;
                y2 = y2 - mSing.mAktiviteetti1.mFragmentti2.siirto4;
            }
            mCanvas.drawRect(x1, y1, x2, y2, paint);
        } catch (Exception e) {
            return false;
        }
        return true;
    }

    //tällä funktiolla sovitetaan viivan paksuus näytön pikselimäärään
    private int maaritaPaksuus(int paksuus) {
/*
        WindowManager wm=(WindowManager) getContext().getSystemService(Context.WINDOW_SERVICE);
        Display display=wm.getDefaultDisplay();
        Point point=new Point();
        display.getSize(point);
        if (point.x>2000 || point.y>2000) {
            return paksuus*6;
        }
        if (point.x>1000 || point.y>1000) {
            return paksuus*4;
        }
*/
        return paksuus;
    }

    //tällä funktiolla luodaan uusi kyna
    public int luoKyna() {
        Paint kyna=new Paint();
        kyna.setStyle(Paint.Style.STROKE);
        kyna.setColor(mRenki.mVari);

        //Selvitetään, halutaanko luoda custom viiva
        int psgeometric=65536;
        int psuserstyle=7;
        if (((mRenki.mTyyli1&psgeometric) == psgeometric) &&
                ((mRenki.mTyyli1&psuserstyle) == psuserstyle)) {
            kyna.setStrokeWidth(mRenki.mLeveys);

            //katsotaan, haluataanko pelkän värin sijasta käyttää jotain kuvaa viivan värittämiseen
            int bspattern=3;
            if (((mRenki.mTyyli2&bspattern) == bspattern) && mRenki.mHatch!=0) {

                //kuvan numero luetaan ja luodaan shaderi
                int tunnus=mRenki.mHatch;
                Bitmap bitmap=mRenki.cKuvat.get(tunnus);
                BitmapShader patternBMPshader = new BitmapShader(bitmap,
                        Shader.TileMode.REPEAT, Shader.TileMode.REPEAT);
                kyna.setColor(0xFFFFFFFF);
                kyna.setShader(patternBMPshader);

            }

            //katsotaan, halutaanko tehdä custom katkoviiva
            if (mRenki.mPituus!=0) {

                //nyt on kyseessä custom viiva
                kyna.setPathEffect(new DashPathEffect(new float[] {mRenki.mDash1, mRenki.mDash2}, 0));
                kyna.setStrokeWidth(maaritaPaksuus(mRenki.mLeveys));

                //selvitetään viivan nivel- ja päättymistyylit
                int psround=1;  //round arvo on oikeasti 0
                if ((mRenki.mTyyli1&psround) == 0) {

                    //kulmat ovat pyöreitä
                    kyna.setStrokeCap(Paint.Cap.ROUND);
                    kyna.setStrokeJoin(Paint.Join.ROUND);
                }
                int psmiter=8192;
                if ((mRenki.mTyyli1&psmiter) == psmiter) {

                    //kulmat ovat teräviä
                    kyna.setStrokeCap(Paint.Cap.SQUARE);
                    kyna.setStrokeJoin(Paint.Join.MITER);
                    kyna.setStrokeMiter(mRenki.mLeveys);
                }
                int psbevel=4096;
                if ((mRenki.mTyyli1&psbevel) == psbevel) {

                    //kulmat ovat typistettyjä
                    kyna.setStrokeCap(Paint.Cap.BUTT);
                    kyna.setStrokeJoin(Paint.Join.BEVEL);
                }
                int koodi=kyna.hashCode();
                mRenki.cKynat.put(koodi, kyna);
                return koodi;
            }
        }

        //selvitetään, millaista ei-custom viivaa halutaan
        int psnull=5;
        if ((mRenki.mTyyli1&psnull) == psnull) {

            //null tarkoittaa näkymätöntä viivaa
            kyna.setStrokeWidth(mRenki.mLeveys);
            kyna.setColor(Color.argb(0, 0, 0, 0));
            int koodi=kyna.hashCode();
            mRenki.cKynat.put(koodi, kyna);
            return koodi;
        }
        int psdot=2;
        if ((mRenki.mTyyli1&psdot) == psdot) {

            //nyt on kyseessä pisteviiva
            kyna.setPathEffect(new DashPathEffect(
                    new float[] {mRenki.mLeveys, mRenki.mLeveys}, 0));
            kyna.setStrokeWidth(maaritaPaksuus(mRenki.mLeveys));

            //selvitetään viivan nivel- ja päättymistyylit
            int psround=1;  //round arvo on oikeasti 0
            if ((mRenki.mTyyli1&psround) == 0) {

                //kulmat ovat pyöreitä
                kyna.setStrokeCap(Paint.Cap.ROUND);
                kyna.setStrokeJoin(Paint.Join.ROUND);
            }
            int psmiter=8192;
            if ((mRenki.mTyyli1&psmiter) == psmiter) {

                //kulmat ovat teräviä
                kyna.setStrokeCap(Paint.Cap.SQUARE);
                kyna.setStrokeJoin(Paint.Join.MITER);
                kyna.setStrokeMiter(mRenki.mLeveys);
            }
            int psbevel=4096;
            if ((mRenki.mTyyli1&psbevel) == psbevel) {

                //kulmat ovat typistettyjä
                kyna.setStrokeCap(Paint.Cap.BUTT);
                kyna.setStrokeJoin(Paint.Join.BEVEL);
            }
            int koodi=kyna.hashCode();
            mRenki.cKynat.put(koodi, kyna);
            return koodi;
        }
        int psdash=1;
        if ((mRenki.mTyyli1&psdash) == psdash) {

            //nyt on kyseessä katkoviiva
            kyna.setPathEffect(new DashPathEffect(
                    new float[] {5*mRenki.mLeveys, 5*mRenki.mLeveys}, 0));
            kyna.setStrokeWidth(maaritaPaksuus(mRenki.mLeveys));

            //selvitetään viivan nivel- ja päättymistyylit
            int psround=1;  //round arvo on oikeasti 0
            if ((mRenki.mTyyli1&psround) == 0) {

                //kulmat ovat pyöreitä
                kyna.setStrokeCap(Paint.Cap.ROUND);
                kyna.setStrokeJoin(Paint.Join.ROUND);
            }
            int psmiter=8192;
            if ((mRenki.mTyyli1&psmiter) == psmiter) {

                //kulmat ovat teräviä
                kyna.setStrokeCap(Paint.Cap.SQUARE);
                kyna.setStrokeJoin(Paint.Join.MITER);
                kyna.setStrokeMiter(mRenki.mLeveys);
            }
            int psbevel=4096;
            if ((mRenki.mTyyli1&psbevel) == psbevel) {

                //kulmat ovat typistettyjä
                kyna.setStrokeCap(Paint.Cap.BUTT);
                kyna.setStrokeJoin(Paint.Join.BEVEL);
            }
            int koodi=kyna.hashCode();
            mRenki.cKynat.put(koodi, kyna);
            return koodi;
        }

        //solid tarkoittaa ihan perusviivaa. Asetetaan leveys
        kyna.setStrokeWidth(maaritaPaksuus(mRenki.mLeveys));

        //selvitetään viivan nivel- ja päättymistyylit
        int psround=1;  //round arvo on oikeasti 0
        if ((mRenki.mTyyli1&psround) == 0) {

            //kulmat ovat pyöreitä
            kyna.setStrokeCap(Paint.Cap.ROUND);
            kyna.setStrokeJoin(Paint.Join.ROUND);
        }
        int psmiter=8192;
        if ((mRenki.mTyyli1&psmiter) == psmiter) {

            //kulmat ovat teräviä
            kyna.setStrokeCap(Paint.Cap.SQUARE);
            kyna.setStrokeJoin(Paint.Join.MITER);
            kyna.setStrokeMiter(mRenki.mLeveys);
        }
        int psbevel=4096;
        if ((mRenki.mTyyli1&psbevel) == psbevel) {

            //kulmat ovat typistettyjä
            kyna.setStrokeCap(Paint.Cap.BUTT);
            kyna.setStrokeJoin(Paint.Join.BEVEL);
        }
        int koodi=kyna.hashCode();
        mRenki.cKynat.put(koodi, kyna);
        return koodi;
    }

    //tällä funktiolla tuhotaan valittu kynä
    public boolean tuhoaKyna(int kyna) {
        Paint siirto=mRenki.cKynat.remove(kyna);
        if (siirto==null) {
            return false;
        } else {
            return true;
        }
    }

    //tällä funktiolla otetaan käyttöön valittu kynä
    public int asetaKyna(int kyna) {
        int koodi = 0;
        if (mPenPaint!=null) {
            mPenPaint.hashCode();
        }
        Paint siirto=mRenki.cKynat.get(kyna);
        if (siirto!=null) {
            mPenPaint=siirto;
        }
        return koodi;
    }

    //tällä funktiolla piirretään polyline
    public boolean piirraPolyline(float[] pisteet, int pisteita) {
        try {
            if (!mTulostetaanko) {
                for (int i = 0; i < pisteita; i = 1 + 2) {
                    pisteet[i] = pisteet[i] - mSing.mAktiviteetti1.mFragmentti2.siirto3;
                    pisteet[i + 1] = pisteet[i + 1] - mSing.mAktiviteetti1.mFragmentti2.siirto4;
                }
            }
            mCanvas.drawLines(pisteet, 0, pisteita, mPenPaint);
        } catch (Exception e) {
            return false;
        }
        return true;
    }

    //tällä funktiolla piirretään viiva
    public boolean piirraViiva(int x1, int y1, int x2, int y2) {
        try {
            if (!mTulostetaanko) {
                x1 = x1 - mSing.mAktiviteetti1.mFragmentti2.siirto3;
                y1 = y1 - mSing.mAktiviteetti1.mFragmentti2.siirto4;
                x2 = x2 - mSing.mAktiviteetti1.mFragmentti2.siirto3;
                y2 = y2 - mSing.mAktiviteetti1.mFragmentti2.siirto4;
            }
            mCanvas.drawLine(x1, y1, x2, y2, mPenPaint);
        } catch (Exception e) {
            return false;
        }
        return true;
    }

    //tällä funktiolla piirretään neliö
    public boolean piirraNelio(int x1, int y1, int x2, int y2) {
        try {
            if (!mTulostetaanko) {
                x1 = x1 - mSing.mAktiviteetti1.mFragmentti2.siirto3;
                y1 = y1 - mSing.mAktiviteetti1.mFragmentti2.siirto4;
                x2 = x2 - mSing.mAktiviteetti1.mFragmentti2.siirto3;
                y2 = y2 - mSing.mAktiviteetti1.mFragmentti2.siirto4;
            }
            mCanvas.drawRect(x1, y1, x2, y2, mPenPaint);
        } catch (Exception e) {
            return false;
        }
        return true;
    }

    //tällä funktiolla piirretään kuva
    public boolean piirraBittiKartta(int x1, int y1, int x2, int y2, Bitmap bitmap) {
        try {
            if (!mTulostetaanko) {
                x2 = x2 - mSing.mAktiviteetti1.mFragmentti2.siirto3;
                y2 = y2 - mSing.mAktiviteetti1.mFragmentti2.siirto4;
            }
            mCanvas.drawBitmap(bitmap, x2, y2, null);
        } catch (Exception e) {
            return false;
        }
        return true;
    }

    //tällä funktiolla luodaan kuva
    public Bitmap luoBittiKartta(int leveys, int korkeus, int formaatti) {
        try {
            if (formaatti==4) {
                return Bitmap.createBitmap(leveys, korkeus, Bitmap.Config.ARGB_8888);
            } else {
                return Bitmap.createBitmap(leveys, korkeus, Bitmap.Config.RGB_565);
            }
        } catch (Exception e) {
            return null;
        }
    }

    //tällä funktiolla täytetään ellipsi värillä tai kuvalla
    public boolean taytaEllipsi(int x1, int y1, int x2, int y2) {
        try {
            Paint paint=new Paint();
            RectF rect=new RectF();
            if (mSivellin.cTyyppi=='P') {
                paint.setShader(new BitmapShader(mSivellin.cBitmap,
                        Shader.TileMode.REPEAT, Shader.TileMode.REPEAT));
                paint.setStyle(Paint.Style.FILL);
            } else {
                paint.setColor(mSiveltimenVari);
                paint.setStyle(Paint.Style.FILL);
            }
            if (!mTulostetaanko) {
                rect.top = y1 - mSing.mAktiviteetti1.mFragmentti2.siirto4;
                rect.bottom = y2 - mSing.mAktiviteetti1.mFragmentti2.siirto4;
                rect.left = x1 - mSing.mAktiviteetti1.mFragmentti2.siirto3;
                rect.right = x2 - mSing.mAktiviteetti1.mFragmentti2.siirto3;
            } else {
                rect.top = y1;
                rect.bottom = y2;
                rect.left = x1;
                rect.right = x2;
            }
            mCanvas.drawOval(rect, paint);
        } catch (Exception e) {
            return false;
        }
        return true;
    }

    //tällä funktiolla piirretään ellipsin ulkorajat
    public boolean piirraEllipsi(int x1, int y1, int x2, int y2) {
        try {
            RectF rect=new RectF();
            if (!mTulostetaanko) {
                rect.top = y1 - mSing.mAktiviteetti1.mFragmentti2.siirto4;
                rect.bottom = y2 - mSing.mAktiviteetti1.mFragmentti2.siirto4;
                rect.left = x1 - mSing.mAktiviteetti1.mFragmentti2.siirto3;
                rect.right = x2 - mSing.mAktiviteetti1.mFragmentti2.siirto3;
            } else {
                rect.top = y1;
                rect.bottom = y2;
                rect.left = x1;
                rect.right = x2;
            }
            mCanvas.drawOval(rect, mPenPaint);
        } catch (Exception e) {
            return false;
        }
        return true;
    }

    //tällä funktiolla täytetään pyöreäkulmainen neliö värillä tai kuvalla
    public boolean taytaPyoreaNelio(int x1, int y1, int x2, int y2, int ax, int ay) {
        try {
            Paint paint=new Paint();
            RectF rect=new RectF();
            if (mSivellin.cTyyppi=='P') {
                paint.setShader(new BitmapShader(mSivellin.cBitmap,
                        Shader.TileMode.REPEAT, Shader.TileMode.REPEAT));
                paint.setStyle(Paint.Style.FILL);
            } else {
                paint.setColor(mSiveltimenVari);
                paint.setStyle(Paint.Style.FILL);
            }
            if (!mTulostetaanko) {
                rect.top = y1 - mSing.mAktiviteetti1.mFragmentti2.siirto4;
                rect.bottom = y2 - mSing.mAktiviteetti1.mFragmentti2.siirto4;
                rect.left = x1 - mSing.mAktiviteetti1.mFragmentti2.siirto3;
                rect.right = x2 - mSing.mAktiviteetti1.mFragmentti2.siirto3;
            } else {
                rect.top = y1;
                rect.bottom = y2;
                rect.left = x1;
                rect.right = x2;
            }
            mCanvas.drawRoundRect(rect, ax, ay, paint);
        } catch (Exception e) {
            return false;
        }
        return true;
    }

    //tällä piirretään pyöreäkulmaisen neliön reunat
    public boolean piirraPyoreaNelio(int x1, int y1, int x2, int y2, int ax, int ay) {
        try {
            RectF rect=new RectF();
            if (!mTulostetaanko) {
                rect.top = y1 - mSing.mAktiviteetti1.mFragmentti2.siirto4;
                rect.bottom = y2 - mSing.mAktiviteetti1.mFragmentti2.siirto4;
                rect.left = x1 - mSing.mAktiviteetti1.mFragmentti2.siirto3;
                rect.right = x2 - mSing.mAktiviteetti1.mFragmentti2.siirto3;
            } else {
                rect.top = y1;
                rect.bottom = y2;
                rect.left = x1;
                rect.right = x2;
            }
            mCanvas.drawRoundRect(rect, ax, ay, mPenPaint);
        } catch (Exception e) {
            return false;
        }
        return true;
    }

    //tällä funktiolla väritetään yksittäinen pikseli
    public boolean setPixel(int x, int y, int vari) {
        try {
            Paint paint=new Paint();
            paint.setColor(vari);
            mCanvas.drawPoint(x, y, paint);
        } catch (Exception e) {
            return false;
        }
        return true;
    }

    //tällä funktiolla väritetään koko näkymä
    public boolean varitaNakyma(int vari) {
        try {
            mBitmap.eraseColor(vari);
        } catch (Exception e) {
            return false;
        }
        return true;
    }

    //tällä funktiolla selvitetään laitteen näytön tuumakoko
    public float annaNaytonTuumaKoko() {
        try {
            double x = Math.pow(mSing.mAktiviteetti1.mDm.widthPixels/
                    mSing.mAktiviteetti1.mDm.xdpi, 2);
            double y = Math.pow(mSing.mAktiviteetti1.mDm.heightPixels/
                    mSing.mAktiviteetti1.mDm.ydpi, 2);
            double tuumaKoko=Math.sqrt(x+y);
            return (float) tuumaKoko;
        } catch (Exception e) {
            return 0.0f;
        }
    }

    //tällä funktiolla selvitetään TekstiRuudun leveys
    public int annaIkkunanLeveys() {
        return mSing.mAktiviteetti1.mFragmentti2.siirto7;
    }

    //tällä funktiolla selvitetään TekstiRuudun korkeus
    public int annaIkkunanKorkeus() {
        return mSing.mAktiviteetti1.mFragmentti2.siirto8;
    }

    //tällä funktiolla pysäytetään scrollaaminen
    public void lopetaPaivitys() {
        mVoikoScrollata=false;
    }
}
