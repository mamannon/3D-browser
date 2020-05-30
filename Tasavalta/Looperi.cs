using System;
using System.Collections.Generic;
using System.Threading;
using System.Reflection;

namespace Tasavalta
{

    public class Looperi
    {

        public delegate void funktio();
        static readonly object lukko = new object();
        bool sammutus = true;
        bool lopetus = false;
        Thread saie;
        List<Delegate> delegaatit;

        //tämä metodi tästä luokasta suoritetaan erillisessä säikeessä
        void toiminta()
        {

            //tästä silmukasta poistuminen lopettaa säikeen
            while (!this.lopetus)
            {

                //varmistetaan, voiko funktioita suorittaa
                if (delegaatit.Count > 0 && !this.sammutus)
                {

                    //suoritetaan funktiolistasta ensimmäinen funktio ja 
                    //poistetaan se listasta
                    try
                    {
                        lock (lukko)
                        {
                            delegaatit[0].DynamicInvoke(null);
                            delegaatit.RemoveAt(0);
                        }
                    }
                    catch (TargetInvocationException e)
                    {
                        throw e.InnerException;
                    }
                }
                else
                {

                    //jos tarjolla ei ole funktiota, laitetaan säie tauolle
                    Thread.Sleep(10);
                }
            }

        }

        //kun Looperi luokka tuhotaan, säikeen pitää lopettaa toimintansa
        ~Looperi()
        {

            //tämä ei ole välttämättä tarpeen, jos säie on taustasäie,
            //joka lopettaa toimintanasa ilmankin emosäikeen lopettaessa
            //foreground säikeen tapauksessa tämä on välttämätön.
            lopetus = true;
        }

        //konstruktorissa luodaan säie, joka pysyy samana koko Luuperi -luokan
        //ilmentymän elinajan ajan
        public Looperi()
        {
            delegaatit = new List<Delegate>();
            saie = new Thread(toiminta);
            saie.Name = "toiminta";
            saie.IsBackground = false;
            saie.Priority = ThreadPriority.BelowNormal;
            saie.SetApartmentState(ApartmentState.MTA);
            saie.Start();
        }

        //Tämä metodi sijoittaa funktion jonoon ja odottaa, että se on suoritettu
        public bool RunAndWait(funktio f)
        {

            //jos looperi on pysäytetty, emme tee mitään
            if (sammutus) return false;

            //Annettu funktio liitetään jonon jatkoksi.
            lock (lukko)
            {
                delegaatit.Add(f);
            }

            //odotetaan, että jono on tyhjä
            while (delegaatit.Count != 0)
            {
                Thread.Sleep(50);
            }
            return true;

        }

        //tämä metodi sijoittaa funktion jonoon ja sijoita ja unohda -tyylisesti
        public bool Run(funktio f)
        {

            //jos looperi on pysäytetty, emme tee mitään
            if (sammutus) return false;

            int index = delegaatit.Count - 1;
            if (index > 0 && Delegate.Equals(delegaatit[index], f))
            {

                //Emme toista peräkkäin kahta samaa funktiota. Näin estämme
                //OpenGL näkymän päivitysten ruuhkautumisen, jos laite
                //ei kykene riittävään päivitystiheyteen.
                return false;
            }
            else
            {

                //Annettu funktio liitetään jonon jatkoksi.
                lock (lukko)
                {
                    delegaatit.Add(f);
                }
                return true;
            }
        }

        //Tällä metodilla pysäytetään looperi, mutta ei lopeteta säiettä 
        public void Stop()
        {
            sammutus = true;
        }

        //Tällä metodilla käynnistetään luuperi
        public void Reset()
        {
            sammutus = false;
        }

        //tällä metodilla voidaan tiedustella, suorittaako luuperi mitään
        public bool OnkoPysäytetty()
        {
            return sammutus;
        }

        //tällä metodilla voidaan tyhjentää looperin funktiojono
        public void TyhjennaJono()
        {
            lock (lukko)
            {
                delegaatit.Clear();
            }
        }
    }
}

