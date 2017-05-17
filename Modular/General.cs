using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace Modular
{
    public class General
    {
        /**/
        public static string _ejecutar_tarea()
        {
            string _error="";
            string _ruta_carvajal_xml = "D:\\INTERFA\\CARVAJAL\\XML";
            string _ruta_carvajal_xml_copy = "D:\\INTERFA\\CARVAJAL\\XML\\copy\\";
            try
            {
                string[] _archivos_xml = Directory.GetFiles(@_ruta_carvajal_xml, "*.xml");

                if (!Directory.Exists(@_ruta_carvajal_xml_copy))
                {
                    Directory.CreateDirectory(@_ruta_carvajal_xml_copy);
                }

                if (_archivos_xml.Length > 0)
                {
                    for (Int32 a = 0; a < _archivos_xml.Length; ++a)
                    {
                        string _archivo = _archivos_xml[a].ToString();
                        string _nombrearchivo_xml = System.IO.Path.GetFileNameWithoutExtension(@_archivo);

                        byte[] _archivo_bytes = File.ReadAllBytes(@_archivo);
                        ServiceBata.ws_bataSoapClient _valor = new ServiceBata.ws_bataSoapClient();
                        string _error_service = _valor.ws_envio_xml(_archivo_bytes, _nombrearchivo_xml);

                        if (_error_service=="1")
                        {
                            string _archivo_copy = _ruta_carvajal_xml_copy + _nombrearchivo_xml + ".xml";
                            File.Copy(@_archivo, @_archivo_copy,true);
                            File.Delete(@_archivo);
                        }                                                                       

                    }
                }

            }
            catch(Exception exc)
            {
                _error = exc.Message;
            }
            return _error;
        }
    }
}
