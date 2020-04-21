using System;
using System.IO;
using System.Xml.Linq;
using System.Linq;
using System.Xml;
using System.Collections.Generic;

[Serializable]
public class Muisti
{

    [NonSerialized]
	static string xml = @"
<root>
    <string-array name =""tiedostotietonimi"" >
        <!--3D 1-->
        <item>Lights</item>
        <item>Smoke</item>
        <!--3D 2-->
        <item>Frame_of_Sampo</item>
<!--        <item>Tendons_of_Sampo</item>
        <item>Gravity_Sphere</item>       -->
        <item>Ethanol_Generator</item>
        <!--3D 3-->
        <item>Space_circle_detailed</item>
<!--        <item>Quarter_roof</item>    -->
        <item>Frame_of_Sampo_balks</item>
        <item>Frame_of_Sampo_side_supports</item>
<!--        <item>Frame_of_Sampo_lower_rafters</item>
        <item>Frame_of_Sampo_upper_rafters</item>    -->
        <item>Crossbeam</item>
        <item>Big_bolt</item>
        <item>Small_bolt</item>
        <item>Big_cover_screw</item>
        <item>Small_cover_screw</item>
        <!--3D 4-->
        <item>SC_Horizontal_balk</item>
        <item>SC_Corner_stake</item>
        <item>SC_Spacing_piece</item>
        <item>SC_Diagonal_upright_stick</item>
        <item>SC_Horizontal_reinforcement</item>
        <item>Crossbeam_1</item>
        <item>Crossbeam_2</item>
        <item>Frame_of_Sampo_balk_1</item>
        <item>Frame_of_Sampo_balk_2</item>
        <item>Frame_of_Sampo_balk_3</item>
        <item>Frame_of_Sampo_balk_4</item>
        <item>Frame_of_Sampo_balk_5</item>
        <item>Frame_of_Sampo_balk_6</item>
        <item>Frame_of_Sampo_balk_7</item>
        <item>Frame_of_Sampo_balk_8</item>
        <item>Frame_of_Sampo_balk_9</item>
        <item>Frame_of_Sampo_balk_10</item>
        <item>Frame_of_Sampo_side_support_1</item>
        <item>Frame_of_Sampo_side_support_2</item>
        <item>Frame_of_Sampo_side_support_3</item>
        <item>Frame_of_Sampo_side_support_4</item>
        <item>Frame_of_Sampo_side_support_5</item>
        <item>Frame_of_Sampo_side_support_6</item>
<!--        <item>Frame_of_Sampo_low_rafter_1</item>
        <item>Frame_of_Sampo_low_rafter_2</item>
        <item>Frame_of_Sampo_low_rafter_3</item>
        <item>Frame_of_Sampo_low_rafter_4</item>
        <item>Frame_of_Sampo_low_rafter_5</item>
        <item>Frame_of_Sampo_low_rafter_6</item>
        <item>Frame_of_Sampo_low_rafter_7</item>
        <item>Frame_of_Sampo_low_rafter_8</item>
        <item>Frame_of_Sampo_low_rafter_9</item>
        <item>Frame_of_Sampo_low_rafter_10</item>
        <item>Frame_of_Sampo_low_rafter_11</item>
        <item>Frame_of_Sampo_low_rafter_12</item>
        <item>Frame_of_Sampo_low_rafter_13</item>
        <item>Frame_of_Sampo_triangle_piece</item>
        <item>Frame_of_Sampo_up_rafter_1</item>
        <item>Frame_of_Sampo_up_rafter_2</item>
        <item>Frame_of_Sampo_up_rafter_3</item>
        <item>Frame_of_Sampo_up_rafter_4</item>
        <item>Frame_of_Sampo_up_rafter_5</item>
        <item>Frame_of_Sampo_up_rafter_6</item>
        <item>Frame_of_Sampo_up_rafter_7</item>
        <item>Frame_of_Sampo_up_rafter_8</item>
        <item>Frame_of_Sampo_up_rafter_9</item>
        <item>Frame_of_Sampo_up_rafter_10</item>
        <item>Frame_of_Sampo_up_rafter_11</item>
        <item>Frame_of_Sampo_up_rafter_12</item>
        <item>Frame_of_Sampo_up_rafter_13</item>
        <item>Frame_of_Sampo_up_rafter_14</item>
        <item>Frame_of_Sampo_up_rafter_15</item>
        <item>Connecting_piece_1</item>
        <item>Connecting_piece_2</item>
        <item>Connecting_piece_3</item>
        <item>Connecting_piece_4</item>
        <item>Connecting_piece_5</item>
        <item>Connecting_piece_6</item>
        <item>Connecting_piece_7</item>
        <item>Connecting_piece_8</item>
        <item>Connecting_piece_9</item>   -->
        <item>Raising_piece</item>
<!--        <item>Iron_shoe</item>    -->
        <item>Cover_screw_big</item>
        <item>Cover_screw_small</item>
        <item>Ferrule_big</item>
        <item>Ferrule_small</item>
        <item>Thread_big_long</item>
        <item>Thread_small_long</item>
        <item>Thread_big_short</item>
        <item>Thread_small_short</item>
        <item>Bolt_big</item>
        <item>Bolt_small</item>
        <item>Nut_big</item>
        <item>Nut_small</item>
        <item>Pin_small</item>
        <!--3D 5-->
        <item>Balk_face_1</item>
        <item>Balk_face_2</item>
        <item>Balk_face_3</item>
        <item>Balk_face_4</item>
        <item>Balk_face_5</item>
        <item>Balk_face_6</item>
        <item>Balk_face_7</item>
        <item>Balk_face_8</item>
        <item>Balk_face_9</item>
        <item>Balk_face_10</item>
        <item>Balk_face_11</item>
        <item>Balk_face_12</item>
        <item>Balk_face_13</item>
<!--        <item>Anchor_1</item>
        <item>Anchor_2</item>
        <item>Anchor_3</item>
        <item>Anchor_4</item>
        <item>Anchor_5</item>   -->
        <!--Stories 1-->
        <item>Universe</item>
        <!--Stories 2-->
        <item>Ideal_Construction</item>
        <item>Ethanol_Fuel_Cell</item>
        <!--Stories 3-->
        <item>General_Relativity</item>
        <item>Mathematical_Formulas</item>
        <!--Stories 4-->

        <!--Stories 5-->

    </string-array>

    <integer-array name = ""tiedostotietovalikkosijainti"" >
        <!--3D 1-->
        <item>1</item>
        <item>1</item>
        <!--3D 2-->
        <item>2</item>
<!--        <item>2</item>
        <item>2</item>     -->
        <item>2</item>
        <!--3D 3-->
        <item>3</item>
<!--        <item>3</item>   -->
        <item>3</item>
        <item>3</item>
<!--        <item>3</item>
        <item>3</item>   -->
        <item>3</item>
        <item>3</item>
        <item>3</item>
        <item>3</item>
        <item>3</item>
        <!--3D 4-->
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
<!--        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>    -->
        <item>4</item>
<!--        <item>4</item>   -->
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <item>4</item>
        <!--3D 5-->
        <item>5</item>
        <item>5</item>
        <item>5</item>
        <item>5</item>
        <item>5</item>
        <item>5</item>
        <item>5</item>
        <item>5</item>
        <item>5</item>
        <item>5</item>
        <item>5</item>
        <item>5</item>
        <item>5</item>
<!--        <item>5</item>
        <item>5</item>
        <item>5</item>
        <item>5</item>
        <item>5</item>   -->
        <!--Stories 1-->
        <item>1</item>
        <!--Stories 2-->
        <item>2</item>
        <item>2</item>
        <!--Stories 3-->
        <item>3</item>
        <item>3</item>
        <!--Stories 4-->

        <!--Stories 5-->

    </integer-array>

    <integer-array name =""tiedostotietoonkonakyva"" >
        <!--3D 1-->
        <item>1</item>
        <item>1</item>
        <!--3D 2-->
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <!--3D 3-->
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <!--3D 4-->
<!--        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>   -->
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <!--3D 5-->
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <!--Stories 1-->
        <item>1</item>
        <!--Stories 2-->
        <item>1</item>
        <item>1</item>
        <!--Stories 3-->
        <item>1</item>
        <item>1</item>
        <!--Stories 4-->

        <!--Stories 5-->


    </integer-array>

    <integer-array name =""tiedostotietoonkoasetettu"" >
        <!--3D 1-->
        <item>1</item>
        <item>1</item>
        <!--3D 2-->
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <!--3D 3-->
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <!--3D 4-->
<!--        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>    -->
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <!--3D 5-->
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <!--Stories 1-->
        <item>1</item>
        <!--Stories 2-->
        <item>1</item>
        <item>1</item>
        <!--Stories 3-->
        <item>1</item>
        <item>1</item>
        <!--Stories 4-->

        <!--Stories 5-->

    </integer-array>

    <integer-array name =""tiedostotietoonko3d"" >
        <!--3D 1-->
        <item>1</item>
        <item>1</item>
        <!--3D 2-->
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <!--3D 3-->
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <!--3D 4-->
<!--        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>    -->
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <!--3D 5-->
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <item>1</item>
        <!--Stories 1-->
        <item>0</item>
        <!--Stories 2-->
        <item>0</item>
        <item>0</item>
        <!--Stories 3-->
        <item>0</item>
        <item>0</item>
        <!--Stories 4-->

        <!--Stories 5-->

    </integer-array>

    <integer name =""tiedostotietotiedostotietoja"" > 67 </integer >
    <integer name =""palstojenlukumaara"" > 8</integer>
</root>
	";

    int mIndex = -1;
	int kirjanMerkkeja = 9;
	int tiedostoTietoja;
	KIRJANMERKKI[] kirjanMerkki;
	TIEDOSTOTIETO[] tiedostoTieto;

	public Muisti()
	{
   
        //ensiksi pitää selvittää, kuinka monta tiedostoa käyttäjällä on avattavissa
        //Tasavalta-sovelluksessa. Luetaan sitä varten XML listaus, josta etsitään 
        //kohta tiedostotietotiedostotietoja
        var doc = XElement.Parse(xml, LoadOptions.None);
        var tulos = from tiedostoja in doc.Elements("integer") 
                               where (string)tiedostoja.Attribute("name") == "tiedostotietotiedostotietoja" 
                               select tiedostoja;

        //löydetystä kohdasta eristetään lukumäärä ja tallennetaan int lukuna
        string siirto = tulos.First().Value;
        tiedostoTietoja = Int32.Parse(siirto);

        //sitten alustetaan pari taulukkoa
        kirjanMerkki = new KIRJANMERKKI[kirjanMerkkeja];
        for (int i = 0; i < kirjanMerkkeja; i++)
        {
            kirjanMerkki[i] = new KIRJANMERKKI();
        }
        tiedostoTieto = new TIEDOSTOTIETO[tiedostoTietoja];
        for (int i = 0; i < tiedostoTietoja; i++)
        {
            tiedostoTieto[i] = new TIEDOSTOTIETO();
        }

        //lopuksi täytetään tiedostoTieto taulukko XML listauksen datalla
        //tiedostojen nimet:
        tulos = from nimet in doc.Elements("string-array")
                               from items in nimet.Elements("item")
                               select items;

        int j = 0;
        foreach (var nimi in tulos)
        {
            siirto = nimi.Value;
            tiedostoTieto[j].nimi = siirto;
            j++;
        }

        //tiedostojen avausnappien sijannit menupalkissa:
        tulos = from nimet in doc.Elements("integer-array")
                from items in nimet.Elements("item")
                where (string)nimet.Attribute("name") == "tiedostotietovalikkosijainti"
                select items;

        j = 0;
        foreach (var nimi in tulos)
        {
            siirto = nimi.Value;
            tiedostoTieto[j].valikkoSijainti = Int32.Parse(siirto);
            j++;
        }

        //ovatko avausnapit näkyvillä menupalkissa:
        tulos = from nimet in doc.Elements("integer-array")
                from items in nimet.Elements("item")
                where (string)nimet.Attribute("name") == "tiedostotietoonkonakyva"
                select items;

        j = 0;
        foreach (var nimi in tulos)
        {
            siirto = nimi.Value;
            tiedostoTieto[j].onkoNakyva = (Int32.Parse(siirto) == 0) ? false : true;
            j++;
        }

        //ovatko avausnapit aktiivisia menupalkissa:
        tulos = from nimet in doc.Elements("integer-array")
                from items in nimet.Elements("item")
                where (string)nimet.Attribute("name") == "tiedostotietoonkoasetettu"
                select items;

        j = 0;
        foreach (var nimi in tulos)
        {
            siirto = nimi.Value;
            tiedostoTieto[j].onkoAsetettu = (Int32.Parse(siirto) == 0) ? false : true;
            j++;
        }

        //mitä kukin avausnappi edustaa, CAD (true) vai HTML (false) tiedostoa:
        tulos = from nimet in doc.Elements("integer-array")
                from items in nimet.Elements("item")
                where (string)nimet.Attribute("name") == "tiedostotietoonko3d"
                select items;

        j = 0;
        foreach (var nimi in tulos)
        {
            siirto = nimi.Value;
            tiedostoTieto[j].onko3D = (Int32.Parse(siirto) == 0) ? false : true;
            j++;
        }

    }

    public TIEDOSTOTIETO AnnaTiedostoTieto(int index)
    {
        return tiedostoTieto[index];
    }

    public void AsetaTiedostoTieto(int index, TIEDOSTOTIETO tt)
    {
        tiedostoTieto[index] = tt;
    }

    public KIRJANMERKKI AnnaKirjanMerkki(int index)
    {
        return kirjanMerkki[index];
    }

    public int AnnaTiedostojenMaara()
    {
        return tiedostoTietoja;
    }

    public bool OnkoKirjanMerkkiAsetettu(int index)
    {
        return kirjanMerkki[index].onkoAsetettu;
    }

    public void TuhoaKirjanmerkit()
    {
        this.kirjanMerkki[0].onkoAsetettu = false;
        this.kirjanMerkki[1].onkoAsetettu = false;
        this.kirjanMerkki[2].onkoAsetettu = false;
        this.kirjanMerkki[3].onkoAsetettu = false;
        this.kirjanMerkki[4].onkoAsetettu = false;
        this.kirjanMerkki[5].onkoAsetettu = false;
        this.kirjanMerkki[6].onkoAsetettu = false;
        this.kirjanMerkki[7].onkoAsetettu = false;
        this.kirjanMerkki[8].onkoAsetettu = false;
        mIndex = -1;
    }

    public void AsetaKirjanMerkkiOrientaatioD(float[] orientaatio, string tiedosto) {
        bool alusta = false;

        //jos tämä metodi on ensimmäinen, etsitään ensimmäinen vapaa paikka
        if (mIndex==-1) 
        {
	        for (int i = 0; i< 9; i++) 
            {
	            if (!kirjanMerkki[i].onkoAsetettu) 
                {
		            mIndex = i;
		            kirjanMerkkeja++;
		            break;
	            }
	        }
        } 
        else 
        {
	        alusta=true;
        }

        //varmistetaan, että meillä on jotain
        if (tiedosto.Length == 0) return;

        if (mIndex != -1) 
        {

            //täytetään ensimminen vapaa paikka
	        for (int i=0; i< 65; i++) 
            {
	            kirjanMerkki[mIndex].orientaatioD[i]=orientaatio[i];
	        }
            kirjanMerkki[mIndex].tiedostoD = tiedosto;
	        kirjanMerkki[mIndex].onkoAsetettu=true;
        } 
        else 
        {

            //jos kaikki paikat ovat varatut, hylätään ensimmäinen ja siirretään muita niin,
            //että voimme täyttää viimeisen paikan
	        kirjanMerkki[0] = kirjanMerkki[1];
	        kirjanMerkki[1] = kirjanMerkki[2];
	        kirjanMerkki[2] = kirjanMerkki[3];
	        kirjanMerkki[3] = kirjanMerkki[4];
	        kirjanMerkki[4] = kirjanMerkki[5];
	        kirjanMerkki[5] = kirjanMerkki[6];
	        kirjanMerkki[6] = kirjanMerkki[7];
	        kirjanMerkki[7] = kirjanMerkki[8];

	        for (int i=0; i< 65; i++) 
            {
	            kirjanMerkki[8].orientaatioD[i]=orientaatio[i];
	        }
            kirjanMerkki[8].tiedostoD = tiedosto;
	        mIndex=8;
        }

        if (alusta) mIndex = -1;
    }

    public void AsetaKirjanMerkkiOrientaatioT(int[] orientaatio, string tiedosto)
    {
        bool alusta = false;

        //jos tämä metodi on ensimmäinen, etsitään ensimmäinen vapaa paikka
        if (mIndex == -1)
        {
            for (int i = 0; i < 9; i++)
            {
                if (!kirjanMerkki[i].onkoAsetettu)
                {
                    mIndex = i;
                    kirjanMerkkeja++;
                    break;
                }
            }
        }
        else
        {
            alusta = true;
        }

        //varmistetaan, että meillä on jotain
        if (tiedosto.Length == 0) return;

        if (mIndex != -1)
        {

            //täytetään ensimminen vapaa paikka
            kirjanMerkki[mIndex].orientaatioT[0] = orientaatio[0];
            kirjanMerkki[mIndex].orientaatioT[1] = orientaatio[1];
            kirjanMerkki[mIndex].orientaatioT[2] = orientaatio[2];
            kirjanMerkki[mIndex].orientaatioT[3] = orientaatio[3];
            kirjanMerkki[mIndex].tiedostoT = tiedosto;
            kirjanMerkki[mIndex].onkoAsetettu = true;
        }
        else
        {

            //jos kaikki paikat ovat varatut, hylätään ensimmäinen ja siirretään muita niin,
            //että voimme täyttää viimeisen paikan
            kirjanMerkki[0] = kirjanMerkki[1];
            kirjanMerkki[1] = kirjanMerkki[2];
            kirjanMerkki[2] = kirjanMerkki[3];
            kirjanMerkki[3] = kirjanMerkki[4];
            kirjanMerkki[4] = kirjanMerkki[5];
            kirjanMerkki[5] = kirjanMerkki[6];
            kirjanMerkki[6] = kirjanMerkki[7];
            kirjanMerkki[7] = kirjanMerkki[8];

            kirjanMerkki[8].orientaatioT[0] = orientaatio[0];
            kirjanMerkki[8].orientaatioT[1] = orientaatio[1];
            kirjanMerkki[8].orientaatioT[2] = orientaatio[2];
            kirjanMerkki[8].orientaatioT[3] = orientaatio[3];
            kirjanMerkki[8].tiedostoT = tiedosto;
        }

        if (alusta) mIndex = -1;
    }

    [Serializable]
	public class KIRJANMERKKI
	{
		public float[] orientaatioD = new float[65];
		public string tiedostoD = null;
		public int[] orientaatioT = new int[4];
		public string tiedostoT = null;
		public bool onkoAsetettu = false;
	};

    [Serializable]
	public class TIEDOSTOTIETO
	{
		public string nimi = null;
		public int valikkoSijainti = 0;
		public bool onkoNakyva = false;
		public bool onkoAsetettu = false;
		public bool onko3D = false;
	}
}
