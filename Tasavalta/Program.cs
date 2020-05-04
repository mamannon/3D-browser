using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Runtime.InteropServices;

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

        [StructLayout(LayoutKind.Sequential)]
        struct INITCOMMONCONTROLSEX
        {
            public int dwSize;
            public uint dwICC;
        }

        [DllImport("comctl32.dll", EntryPoint = "InitCommonControlsEx", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool InitCommonControlsEx(ref INITCOMMONCONTROLSEX iccex);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            INITCOMMONCONTROLSEX icc;
            icc.dwICC = 0x00004000; //ICC_STANDARD_CLASSES = 0x00004000
            icc.dwSize = Marshal.SizeOf(typeof(INITCOMMONCONTROLSEX));

            //Visual Styles pitää erikseen ladata alla olevalla funktiolla
            InitCommonControlsEx(ref icc);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Valikko());
        }
    }
}
