using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace Huffman
{
    /// <summary>
    /// Class that implements the Huffman encoding algorithm.
    /// </summary>
    public class Huffman
    {
        private const int MAXCHAR = 256;    // maximaal 256 verschillende letters in de ASCII-tabel

        private CharCount[] charCount;      // array met voor elke karakter een CharCount object
        private Knoop boom;                 // de boom met de (binaire) codes voor alle karakters in de boodschap

        public Huffman()
        {
            // voor elke letter/karakter wordt de bijbehorende binaire waarde en de frequentie opgeslagen
            charCount = new CharCount[MAXCHAR];
            for (int c = 0; c < MAXCHAR; c++)
                charCount[c] = new CharCount((char)c);
        }

        public string Codeer(string input, ListBox freqLijst)
        {
            // 1. verwerk de input letter voor letter en verhoog de betreffende index in (array) charCount
            // ...
            foreach (char c in input)
                charCount[c].count++;

            foreach (var c in charCount.Where(x => x.count > 0).OrderByDescending(x => x.count))
                freqLijst.Items.Add(c.character + " : " + c.count);

            // 2. sorteer het array d.m.v. binary sort (de sorteer functie kan je hergebruiken uit een vorige opdracht)
            // ...
            charCount = charCount.OrderByDescending(x => x.count).ToArray();

            // 3. maak voor alle relevante klassen in charCount knopen aan met als userObject de betreffende charCount
            // en plaats deze in een nieuw ArrayList (een ArrayList kun je dynamisch verkleinen, dat straks nodig is)
            ArrayList knopen = new ArrayList();
            // ...
            foreach (CharCount c in charCount.Where(x => x.count > 0))
                knopen.Add(new Knoop(c));

            // 4. herhaal nu (zolang de gesorteerde ArrayList met knopen groter is dan 1):
            //  maak een nieuwe knoop die de 2 knopen met laagste count als kinderen heeft en
            //  deze 2 knopen vervolgens vervangt in de array(list) (de letter 0 als char, de som van de counts als count)
            //  zorg er voor dat deze vervangende knoop op de juiste positie komt in de array en
            // dat de array 1 item kleiner wordt
            // ...
            //List<Knoop> knopen = new List<Knoop>();
            int charsLeft = knopen.Count - 1;
            while (charsLeft > 0)
            {
                Knoop knoop = new Knoop(new CharCount('0'));
                var knoop1 = (knopen[charsLeft] as Knoop);
                //knoop1.userObject.binaireWaarde += "0";
                var knoop2 = (knopen[charsLeft - 1] as Knoop);
                //knoop2.userObject.binaireWaarde += "1";

                knoop.userObject.count = knoop1.userObject.count + knoop2.userObject.count;

                knoop.AddKnoopLinks(knoop1);
                knoop.AddKnoopRechts(knoop2);
                knopen[charsLeft] = knoop;

                knopen.Remove(knoop2);
                //tmpKnopen.Sort();
                //tmpKnopen = (tmpKnopen as List<Knoop>).OrderBy(x => (x as Knoop).userObject.count);
                charsLeft = charsLeft - 1;
            }

            // 5.
            // geef nu alle blaadjes in de boom (van het type Knoop dus) hun bijbehorende binaire waarde
            // dit kan je doen een recursieve methode te maken die als parameter de "huidige" binaire waarde heeft en hier
            // aan de linker-aanroep een extra "0" toevoegt en aan de rechter-aanroep een extra "1"
            //zijn zowel de linkerkant als de rechterkant null, dan zit je in een blaadje en kan de huidige waarde worden toegekend

            Knoop tree = (Knoop)knopen[0];
            GetBinaryValue(tree, string.Empty);
            // ...

            // de tree is nu afgerond
            this.boom = tree;

            // 6. maak een loop over alle characters in input
            // hierbij kan je gebruik maken van de enumerator "IDictionaryEnumerator GetEnumerator" van de tree
            // zoek de knoop die hoort bij het huidige character
            // voeg de binaire waarde van de knoop toe aan een output string
            string outputStr = "";
            // ...
            var searcher = tree.GetEnumerator();
            foreach (char c in input)
            {
                bool gevonden = false;
                while (searcher.MoveNext())
                {
                    var charCount = (CharCount)searcher.Value;
                    if (charCount.character == c)
                    {
                        outputStr += charCount.binaireWaarde;
                        break;
                    }
                }
                Debug.WriteLine("Found char: " + c + "OutputString is: " + outputStr);
            }
            // 7. loop over alle knopen in de tree
            // voeg aan de freqlijst.Items nieuwe items toe in het format: "count x character"
            // ...

            // return de output string
            return outputStr;
        }

        #region Determine binary values
        public void GetBinaryValue(Knoop k, string code)
        {
            GoBinaryLeft(k, code);
            GoBinaryRight(k, code);
        }

        private void GoBinaryLeft(Knoop k, string code)
        {
            code += '0';
            if (k.links == null)
                k.userObject.binaireWaarde = code;
            else
                GetBinaryValue(k.links, code);
        }

        private void GoBinaryRight(Knoop k, string code)
        {
            code += '1';
            if (k.rechts == null)
                k.userObject.binaireWaarde = code;
            else
                GetBinaryValue(k.rechts, code);
        }
        #endregion

        #region Get characters from binary
        public void GetCharacterValue(Knoop k, string output, string input)
        {           
            if(input[0] == '0')
                GoCharacterLeft(k, output, input);
            else
                GoCharacterRight(k, output, input);
        }

        private void GoCharacterLeft(Knoop k, string output, string input)
        {
            
        }

        private void GoCharacterRight(Knoop k, string output, string input)
        {

        }
        #endregion
        public string Decodeer(string input)
        {
            string str = "";
            // loop over alle énen en nullen in de string input
            // als de huidige char een 1 is
            // probeer verder te gaan vanuit de rechter knoop
            // als dat niet lukt zit je in een blad
            // voeg dan de character toe aan de output string en ga rechts vanaf de boom
            // else
            // probeer verder te gaan vanuit de linker knoop
            // als dat niet lukt zit je in een blad
            // voeg dan de character toe aan de output string en ga links vanaf de boom

            var searcher = this.boom.GetEnumerator();
            foreach (char c in input)
            {
                while (searcher.MoveNext())
                {
                    var charCount = (CharCount)searcher.Value;
                    if (charCount.character == c)
                    {
                        outputStr += charCount.binaireWaarde;
                        break;
                    }
                }
                Debug.WriteLine("Found char: " + c + "OutputString is: " + outputStr);
            }


            // return output string
            return str;
        }

        // zet hier je sorteren methode neer van week3
    }
}