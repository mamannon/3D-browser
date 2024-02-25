package com.mannonen.mikael.example.tasavalta;

import static androidx.test.espresso.Espresso.onView;
import static androidx.test.espresso.matcher.ViewMatchers.assertThat;
import static androidx.test.espresso.matcher.ViewMatchers.withId;
import static androidx.test.platform.app.InstrumentationRegistry.getInstrumentation;
import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertNotNull;
import static org.junit.Assert.assertNull;
import static org.hamcrest.Matchers.*;
import android.content.Context;
import android.view.View;
import androidx.fragment.app.testing.FragmentScenario;
import androidx.test.core.app.ActivityScenario;
import androidx.test.core.app.ApplicationProvider;
import androidx.test.ext.junit.runners.AndroidJUnit4;
import androidx.test.internal.runner.junit4.statement.UiThreadStatement;
import org.junit.Before;
import org.junit.Test;
import org.junit.runner.RunWith;

@RunWith(AndroidJUnit4.class)
public class InstrumentedTestit {

    private ActivityScenario<Aktiviteetti1> activityScenario;

    @Before
    public void alustukset() {
        activityScenario = ActivityScenario.launch(Aktiviteetti1.class);
    }

    @Test
    public void testaaKonteksti() {
        Context context = getInstrumentation().getTargetContext();
        Context appContext = ApplicationProvider.getApplicationContext();

        assertEquals("com.mannonen.mikael.example.tasavalta", context.getPackageName());
        assertEquals(appContext, context.getApplicationContext());
    }

    @Test
    public void testaaAktiviteetti1() {
        assertNotNull(activityScenario);
        activityScenario.onActivity(activity -> {
            assertNotNull(activity.mFragmentti1);
            assertNull(activity.mFragmentti4);
            assertNull(activity.mFragmentti2);
            assertNull(activity.mFragmentti3);
        });
    }

    @Test
    public void testaaPaaValikko() {
        assertNotNull(activityScenario);
        try(FragmentScenario<PaaValikko> scenario = FragmentScenario.launch(PaaValikko.class)) {
            scenario.onFragment(fragment -> {
                assertNotNull(fragment.annaNakyma());
                assertThat(fragment.annaOptio(), anyOf(equalTo(0), equalTo(1), equalTo(2)));
                onView(withId(R.id.sisaltoOikea)).noActivity();
                onView(withId(R.id.sisaltoVasen)).noActivity();
            });
        }
    }

    @Test
    public void testaaCADRuutu() {
        assertNotNull(activityScenario);
        try(FragmentScenario<CADRuutu> scenario = FragmentScenario.launch(CADRuutu.class)) {
            scenario.onFragment(fragment -> {
                assertNotNull(fragment.annaNakyma());
                assertThat(fragment.onkoLaajennuksia(0), anyOf(equalTo(1), equalTo(2)));
            });
        }
    }

    @Test
    public void testaaTekstiRuutu() {
        assertNotNull(activityScenario);
        try(FragmentScenario<TekstiRuutu> scenario = FragmentScenario.launch(TekstiRuutu.class)) {
            scenario.onFragment(fragment -> {
                assertNotNull(fragment.annaNakyma());
                assertThat(fragment.HistoryCanBack(), is(false));
                assertThat(fragment.HistoryCanForward(), is(false));
            });
        }
    }

    @Test
    public void testaaAbout() {
        assertNotNull(activityScenario);
        try(FragmentScenario<About> scenario = FragmentScenario.launch(About.class)) {
            scenario.onFragment(fragment -> {
                assertNotNull(fragment.annaNakyma());

                final View view = fragment.annaNakyma().findViewById(R.id.about_otsikko);
                boolean clickable = view.isClickable();
                try {
                    UiThreadStatement.runOnUiThread(new Runnable() {
                        @Override
                        public void run() {
                            view.setClickable(false);
                        }
                    });
                    getInstrumentation().waitForIdleSync();
                } catch (Throwable e) {

                }
                assertEquals(view.isClickable(), false);
                try {
                    UiThreadStatement.runOnUiThread(new Runnable() {
                        @Override
                        public void run() {
                            view.setClickable(true);
                        }
                    });
                    getInstrumentation().waitForIdleSync();
                } catch (Throwable e) {

                }
                assertEquals(view.isClickable(), true);
                view.setClickable(clickable);
            });
        }
    }
}