using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Huffman;
using Formatting = Newtonsoft.Json.Formatting;
using ArbolAVL;
using Labestu2.Objetos;
using Newtonsoft.Json;
using System;


namespace Labestu2
{
    internal class Program
    {
        public static AVLTree<Ingreso> participante = new AVLTree<Ingreso>();
        static void Main(string[] args)
        {
            string ruta = "";
            Console.WriteLine("Ingrese la ruta del csv");
            ruta = Console.ReadLine();
            var reader = new StreamReader(File.OpenRead(ruta));
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var value = line.Split(';');
                if (value[0] == "INSERT")
                {
                    var data = JsonConvert.DeserializeObject<participante>(value[1]);
                    participante trabajar = data;
                    List<string> dupli = trabajar.companies.Distinct().ToList();
                    trabajar.companies = dupli;
                    List<Compania> companias = new List<Compania>();
                    for (int i = 0; i < trabajar.companies.Count; i++)
                    {
                        Compania comp = new Compania();
                        comp.Name = trabajar.companies[i];
                        comp.Libreria.Build(comp.Name + "*" + trabajar.dpi);
                        comp.cript = comp.Libreria.Encode(comp.Name + "*" + trabajar.dpi);
                        companias.Add(comp);
                    }
                    Ingreso ingreso = new Ingreso();
                    ingreso.name = trabajar.name;
                    ingreso.dpi = trabajar.dpi;
                    ingreso.address = trabajar.address;
                    ingreso.dateBirth = trabajar.dateBirth;
                    ingreso.companies = companias;
                    participante.insert(ingreso, ComparacioDPI);
                    
                }
                else if (value[0] == "PATCH")
                {
                    var data = JsonConvert.DeserializeObject<participante>(value[1]);
                    participante trabajar = data;
                    Ingreso busqueda = new Ingreso();
                    busqueda.name = trabajar.name;
                    busqueda.dpi = trabajar.dpi;
                    if (participante.Search(busqueda, ComparacioDPI).name == trabajar.name)
                    {
                        if (trabajar.dateBirth != null)
                        {
                            participante.Search(busqueda, ComparacioDPI).dateBirth = trabajar.dateBirth;
                        }
                        if (trabajar.address != null)
                        {
                            participante.Search(busqueda, ComparacioDPI).address = trabajar.address;
                        }
                        if(trabajar.companies != null) 
                        {
                            List<string> doble = trabajar.companies.Distinct().ToList();
                            List<Compania> nop = new List<Compania>();
                            for (int i = 0; i < doble.Count; i++)
                            {
                                Compania comp = new Compania();
                                comp.Name = doble[i];
                                comp.Libreria.Build(comp.Name + "*" + trabajar.dpi);
                                comp.cript = comp.Libreria.Encode(comp.Name + "*" + trabajar.dpi);
                                nop.Add(comp);
                            }
                            participante.Search(busqueda, ComparacioDPI).companies = nop;
                        }
                    }
                }
                else if (value[0] == "DELETE")
                {
                    var data = JsonConvert.DeserializeObject<participante>(value[1]);
                    participante trabajar = data;
                    Ingreso ingreso = new Ingreso();
                    ingreso.dpi = trabajar.dpi;
                    List<Ingreso> trabajo = participante.getAll();
                    int cant = trabajo.Count();
                    for (int i = 0; i < trabajo.Count; i++)
                    {
                        if (trabajo[i].dpi == ingreso.dpi)
                        {
                            trabajo.RemoveAt(i);
                        }
                    }
                    participante = new AVLTree<Ingreso>();
                    int cant2 = trabajo.Count();
                    for (int j = 0; j < trabajo.Count; j++)
                    {
                        participante.insert(trabajo[j], ComparacioDPI);
                    }
                }
            }
            bool basta = false;
            string parabasta = "si";
            while (basta == false)
            {
                string dpi;
                string ruta1;
                Console.WriteLine("Ingrese un numero de DPI");
                dpi = Console.ReadLine();
                Ingreso participantebus = new Ingreso();
                Ingreso participantefin = new Ingreso();
                participantebus.dpi = dpi;
                participantefin = participante.Search(participantebus, ComparacioDPI);
                Console.WriteLine("Ruta para guardar el archivo (debe terminar en .json)");
                ruta1 = Console.ReadLine();
                List<Ingreso> solicitantelist = new List<Ingreso>();
                solicitantelist.Add(participantefin);
                Serializacion(solicitantelist, ruta1);
                Console.WriteLine("¿Quiere hacer otra busqueda? si/no");
                parabasta = Console.ReadLine();
                if (parabasta == "no") {
                    basta = true;

                }
            }       
        }
        public static void Serializacion(List<Ingreso> Lista, string path)
        {
            string solictanteJson = JsonConvert.SerializeObject(Lista.ToArray(), Formatting.Indented);
            File.WriteAllText(path, solictanteJson);
        }
        public static bool ComparacioDPI(Ingreso paciente, string operador, Ingreso paciente2)
        {
            int Comparacion = string.Compare(paciente.dpi,paciente2.dpi);
            if (operador == "<")
            {
                return Comparacion < 0;
            }
            else if (operador == ">")
            {
                return Comparacion > 0;
            }
            else if (operador == "==")
            {
                return Comparacion == 0;
            }
            else return false;
        }
    }
}
