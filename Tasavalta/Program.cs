using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Tasavalta
{

    public sealed class Singleton
    {
        private static Singleton ilmentyma = null;

        //Signleton luokka on olemassa vain tätä osoitinta varten
        public Valikko mValikko = null;

        //näin Singleton luokan konstruktoria ei voi käyttää
        Singleton()
        {

        }

        //Tämä metodi toimii Singleton luokan konstruktorina, joka luo
        //vain yhden ilmentymän tästä luokasta
        private static readonly object padlock = new object();
        public static Singleton AnnaIlmentyma
        {
            get
            {
                lock (padlock)
                {
                    if (ilmentyma == null)
                    {
                        ilmentyma = new Singleton();
                    }
                    return ilmentyma;
                }
            }
        }

        //Asettaa osoittimen, jos sitä ei vielä ole asetettu
        public void asetaValikko(Valikko valikko)
        {
            if (mValikko == null)
            {
                mValikko = valikko;
            }
        }
    }
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Valikko());
        }
    }
}
