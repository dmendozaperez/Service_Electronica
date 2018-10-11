using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Proy_Prueba
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {



            //Modular.General._ejecutar_tarea();

            //String _patch__xml = @"D:\DOC_XML\20408990816_03_B805_3658.xml";
            //string _nombrearchivo_xml = System.IO.Path.GetFileNameWithoutExtension(@_patch__xml);
            //string _tienda = _nombrearchivo_xml.Substring(1, 3);

            //byte[] _archivo_bytes = File.ReadAllBytes(@_patch__xml);
            ServiceBata.ws_bataSoapClient _valor = new ServiceBata.ws_bataSoapClient();
            //object _msg = _valor.ws_control_error(_archivo_bytes, _tienda, _nombrearchivo_xml, "error de archivo");


           // byte[] _archivo_bytes = _valor.ws_genera_pdf_bytes("20", "R001","1","1");

            byte[] _archivo_bytes = _valor.ws_genera_pdf_bytes("01", "F030", "54", "54");

            System.IO.File.WriteAllBytes(@"D:\prueba.pdf", _archivo_bytes);

            //object _msg = _valor.ws_envio_xml_tropi(_archivo_bytes, _nombrearchivo_xml);






        }
    }
}
