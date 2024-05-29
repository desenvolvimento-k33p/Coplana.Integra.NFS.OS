using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coplana.Integracao.NfsOs.Core
{
    public class Criptografia
    {
        private static Criptografia _instancia;

        private Criptografia() { }

        public static Criptografia Instancia
        {
            get
            {
                if (_instancia == null)
                    _instancia = new Criptografia();

                return _instancia;
            }
        }

        public string Descriptografar(string texto)
        {
            //string criptografia = string.Empty;

            //for (int i = 0; i < texto.Length; i++)
            //{
            //    // Devolve o código ASCII da letra
            //    int ascii = (int)texto[i];

            //    // Coloca a chave fixa de 10 posições a mais no número da tabela ASCII
            //    int asciic = ascii - 10;

            //    criptografia += Char.ConvertFromUtf32(asciic);
            //}

            //return criptografia;
            string _result = string.Empty;
            char[] temp = texto.ToCharArray();
            foreach (var _singleChar in temp)
            {
                var i = (int)_singleChar;
                i = i + 2;
                _result += (char)i;
            }
            return _result;
        }


    }
}
