package com.mannonen.mikael.example.tasavalta;

import static org.junit.Assert.assertEquals;
import static org.mockito.ArgumentMatchers.anyInt;
import static org.mockito.ArgumentMatchers.anyString;
import static org.mockito.Mockito.when;
import android.content.Context;
import android.content.pm.PackageInfo;
import android.content.pm.PackageManager;
import android.content.res.Resources;
import org.junit.Before;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.mockito.Mock;
import org.mockito.junit.MockitoJUnitRunner;

@RunWith(MockitoJUnitRunner.class)
public class UnitTestMuisti {

    private int[] tiedostoTietoOnko3D = {1, 1, 0, 0};
    private String[] tiedostoTietoNimi = {"CAD1", "CAD2", "Teksti1", "Teksti2"};
    private int[] tiedostoTietoValikkoSijainti = {1, 2, 3, 4};
    private int[] tiedostoTietoOnkoNakyva = {1, 0, 1, 0};
    private int[] tiedostoTietoOnkoAsetettu = {1, 1, 1, 1};
    private String packageName = "Tasavalta";
    private int versio = 1;

    @Mock
    private Context mockContext;

    @Mock
    private Resources mockContextResources;

//    @Mock
//    private SharedPreferences mockSharedPreferences;

    @Mock
    private PackageManager mockPackageMAnager;

    @Mock
    private PackageInfo mockPackageInfo;

    private Muisti muisti;

    @Before
    public void init() {

        when(mockContext.getResources()).thenReturn(mockContextResources);
//        when(mockContext.getSharedPreferences(anyString(), anyInt())).thenReturn(mockSharedPreferences);
        when(mockContext.getPackageManager()).thenReturn(mockPackageMAnager);

        when(mockContext.getResources().getStringArray(R.array.tiedostotietonimi)).
                thenReturn(tiedostoTietoNimi);
        when(mockContext.getResources().getIntArray(R.array.tiedostotietovalikkosijainti)).
                thenReturn(tiedostoTietoValikkoSijainti);
        when(mockContext.getResources().getIntArray(R.array.tiedostotietoonkonakyva)).
                thenReturn(tiedostoTietoOnkoNakyva);
        when(mockContext.getResources().getIntArray(R.array.tiedostotietoonkoasetettu)).
                thenReturn(tiedostoTietoOnkoAsetettu);
        when(mockContext.getResources().getIntArray(R.array.tiedostotietoonko3d)).
                thenReturn(tiedostoTietoOnko3D);
        when(mockContext.getPackageName()).thenReturn(packageName);
        try {
            when(mockContext.getPackageManager().getPackageInfo(anyString(), anyInt())).
                    thenReturn(mockPackageInfo);
            when(mockPackageInfo.versionCode).
                    thenReturn(1);
        } catch (Exception e) {
            //        Assert.fail();
        }

        muisti = new Muisti(mockContext);

    }

    @Test
    public void annaTiedostojenNimiOK() {
        assertEquals(muisti.annaTiedostonNimi(0), "CAD1");
    }

    @Test
    public void annaValikkoSijaintiOK() {
        assertEquals(muisti.annaValikkoSijainti(1), 2);
    }

    @Test
    public void annaOnkoAsetettuOK() {
        assertEquals(muisti.annaOnkoAsetettu(2), true);
    }

    @Test
    public void annaOnkoNakyvaOK() {
        assertEquals(muisti.annaOnkoNakyva(3), false);
    }
}

