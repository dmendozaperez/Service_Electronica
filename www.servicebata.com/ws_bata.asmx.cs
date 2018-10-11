using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Data;
using System.IO;

namespace www.servicebata.com
{
    /// <summary>
    /// Descripción breve de ws_bata
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    // [System.Web.Script.Services.ScriptService]
    public class ws_bata
    {
        #region<METODO PARA DESARROLLO>
        [WebMethod]
        public string ws_envio_xml_desarrollo(byte[] _archivo_xml, string _name)
        {
            string valida = "";
            try
            {
                valida = Basico.exportar_archivo_xml_desarrollo(_archivo_xml, _name);

                //valida = "XXXX";

            }
            catch (Exception exc)
            {
                valida = exc.Message;
            }

            //String[] _respuesta = new String[] { "codigo", "descripcion" };
            String _respuesta = "";
            string _error_codigo = "";
            string _mensaje = "";

            if (valida.Length == 0)
            {
                _error_codigo = "1";
                _mensaje = "transmision exitosa";

                //_respuesta[0] = _error_codigo.ToString();
                //_respuesta[1] = _mensaje.ToString();
                _respuesta = _error_codigo.ToString();
            }
            else
            {

                _error_codigo = valida;
                _mensaje = valida;
                //_respuesta[0] = _error_codigo.ToString();
                //_respuesta[1] = _mensaje.ToString();
                _respuesta = _error_codigo.ToString();
            }

            return _respuesta;
        }
        #endregion

        [WebMethod]
        public string ws_envio_xml_tropi(byte[] _archivo_xml, string _name)
        {
            string valida = "";
            try
            {
                valida = Basico.exportar_archivo_xml(_archivo_xml, _name,"T");
                //valida = "XX";

            }
            catch (Exception exc)
            {
                valida = exc.Message;
            }

            //String[] _respuesta = new String[] { "codigo", "descripcion" };
            String _respuesta = "";
            string _error_codigo = "";
            string _mensaje = "";

            if (valida.Length == 0)
            {
                _error_codigo = "1";
                _mensaje = "transmision exitosa";

                //_respuesta[0] = _error_codigo.ToString();
                //_respuesta[1] = _mensaje.ToString();
                _respuesta = _error_codigo.ToString();
            }
            else
            {

                _error_codigo = "0";
                _mensaje = valida;
                //_respuesta[0] = _error_codigo.ToString();
                //_respuesta[1] = _mensaje.ToString();
                _respuesta = _error_codigo.ToString();
            }

            return _respuesta;
        }

        [WebMethod]
        public string ws_envio_xml(byte[] _archivo_xml, string _name)
        {
            string valida = "";
            try
            {
                valida = Basico.exportar_archivo_xml(_archivo_xml, _name);

                //valida = "XXXX";                           
               
            }
            catch (Exception exc)
            {
                valida = exc.Message;
            }

            //String[] _respuesta = new String[] { "codigo", "descripcion" };
            String _respuesta = "";
            string _error_codigo = "";
            string _mensaje = "";
           
            if (valida.Length == 0)
            {
                _error_codigo = "1";
                _mensaje = "transmision exitosa";               

                //_respuesta[0] = _error_codigo.ToString();
                //_respuesta[1] = _mensaje.ToString();
                _respuesta = _error_codigo.ToString();
            }
            else
            {
                
                _error_codigo = "0";
                _mensaje = valida;
                //_respuesta[0] = _error_codigo.ToString();
                //_respuesta[1] = _mensaje.ToString();
                _respuesta = _error_codigo.ToString();
            }

            return _respuesta;
        }

        [WebMethod]
        public string ws_control_error(byte[] _archivo,string _cod_tda,string _nom_archivo,string _error)
        {
            string valida = "";
            try
            {
                //insertando en la tabla de errores

                valida = "XXX";

                //valida = Basico._insertar_error( _cod_tda, _nom_archivo, _error);



                //if (valida.Length==0)
                //{
                //    valida=Basico.exportar_archivo_errores(_archivo,"TD" + _cod_tda, _nom_archivo);
                //}


            }
            catch (Exception exc)
            {
                valida = exc.Message;
            }

            //String[] _respuesta = new String[] { "codigo", "descripcion" };
            String _respuesta = "";
            string _error_codigo = "";
            string _mensaje = "";

            if (valida.Length == 0)
            {
                _error_codigo = "1";
                _mensaje = "transmision exitosa";

                //_respuesta[0] = _error_codigo.ToString();
                //_respuesta[1] = _mensaje.ToString();
                _respuesta = _error_codigo.ToString();
            }
            else
            {

                _error_codigo = "0";
                _mensaje = valida;
                //_respuesta[0] = _error_codigo.ToString();
                //_respuesta[1] = _mensaje.ToString();
                _respuesta = _error_codigo.ToString();
            }

            return _respuesta;
        }
        #region<Generacion de pdf electronico>
        [WebMethod]
        public Byte[] ws_genera_pdf_bytes(String _tipo,string _serie,string _num_ini,string _num_fin)
        {
            Byte[] ws = null;
            try
            {
                ws = Basico.get_pdf(_tipo,_serie, Convert.ToDecimal(_num_ini),Convert.ToDecimal(_num_fin));
            }
            catch
            {                
                ws = null;
            }

            if (ws == null)
            {             
                String abc = "0";
                byte[] numbers = System.Text.Encoding.ASCII.GetBytes(abc);
                ws = numbers;

            }

            return ws;

        }
            #endregion
      }
}
