using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.IO;
namespace www.servicebata.com
{
    public class Basico
    {
        private static string conexion
        {
            get {return "Server=10.10.10.208;Database=BdWSBata;User ID=sa;Password=Bata2013;Trusted_Connection=False;";}
        }


        private static DataTable _retorna_ruta_tda()
        {
            string sqlquery = "USP_Leer_RutaTda";
            SqlConnection cn = null;
            SqlCommand cmd = null;
            SqlDataAdapter da = null;
            DataTable dt = null;
            try
            {
                cn = new SqlConnection(conexion);
                cmd = new SqlCommand(sqlquery, cn);
                cmd.CommandTimeout = 0;
                cmd.CommandType = CommandType.StoredProcedure;              
                da = new SqlDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);
            }
            catch
            {
                dt = null;
            }
            if (cn.State == ConnectionState.Open) cn.Close();
            return dt;
        }


        private static DataSet _retorna_tabla_ruta(string _codigo)
        {
            string sqlquery="USP_Consulta_RutaXml";
            SqlConnection cn=null;
            SqlCommand cmd=null;
            SqlDataAdapter da=null;
            DataSet ds=null;
            try
            {
                cn =new SqlConnection(conexion);
                cmd=new SqlCommand(sqlquery,cn);
                cmd.CommandTimeout=0;
                cmd.CommandType=CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@codigo",_codigo);
                da=new SqlDataAdapter(cmd);
                ds=new DataSet();
                da.Fill(ds);

            }
            catch
            {
                ds=null;
            }
            if (cn.State == ConnectionState.Open) cn.Close();
            return ds;
        }

        //insertar en la tabla de errores
        public static string _insertar_error(string _codtda,string _namearchivo,string _error)
        {
            string sqlquery = "USP_Control_Errores";
            SqlConnection cn = null;
            SqlCommand cmd = null;
            string _valida = "";
            try
            {
                cn = new SqlConnection(conexion);
                if (cn.State == 0) cn.Open();
                cmd = new SqlCommand(sqlquery, cn);
                cmd.CommandTimeout = 0;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Cod_Tda", _codtda);
                cmd.Parameters.AddWithValue("@Archivo", _namearchivo);
                cmd.Parameters.AddWithValue("@Error", _error);
                cmd.ExecuteNonQuery();
            }
            catch(Exception exc)
            {
                _valida = exc.Message;
            }
            if (cn.State == ConnectionState.Open) cn.Close();
            return _valida;
        }


        public static string exportar_archivo_errores(byte[] _archivo_xml,string _tienda, string _tienda_archivo)
        {
            string _archivo_ruta = "";
            string _error = "";
            try
            {               
                DataTable dtruta = _retorna_ruta_tda();

                if (dtruta != null)
                {
                    for (Int32 i = 0; i < dtruta.Rows.Count; ++i)
                    {

                        string _ruta_carpeta = dtruta.Rows[i]["ruta"].ToString() + "\\" + _tienda + "\\error_txt";

                        if (!Directory.Exists(@dtruta.Rows[i]["ruta"].ToString()))
                        {
                            Directory.CreateDirectory(@dtruta.Rows[i]["ruta"].ToString());
                        }

                        if (!Directory.Exists(@dtruta.Rows[i]["ruta"].ToString() + "\\" + _tienda))
                        {
                            Directory.CreateDirectory(@dtruta.Rows[i]["ruta"].ToString() + "\\" + _tienda);
                        }
                        if (!Directory.Exists(@_ruta_carpeta))
                        {
                            Directory.CreateDirectory(@_ruta_carpeta);
                        }


                        //string _ruta_carpeta = dt_ruta.Rows[i]["ruta_xml"].ToString() + "\\";
                        //        //creando la carpeta de la tienda                                            

                        _archivo_ruta = _ruta_carpeta + "\\" + _tienda_archivo + ".txt";

                        File.WriteAllBytes(@_archivo_ruta, _archivo_xml);

                    }

                }
                else
                {
                    _error = "error de conexion";
                }

            }
            catch (Exception exc)
            {

                _error = exc.Message;
                //throw;
            }
            return _error;
        }

        public static string exportar_archivo_xml(byte[] _archivo_xml, string _tienda_archivo,string _empresa="E")
        {           
            string _archivo_ruta = "";
            string _error = "";
            try
            {

                string _tipo = _tienda_archivo.Substring(12, 2);

                string _codigo_ruta = "";

                #region<EMPRESAS COMERCIALES Y TROPICALZA>    
                if (_empresa=="E")
                { 
                    switch (_tipo)
                    {
                        case "01":
                            _codigo_ruta = "01";
                            break;
                        case "03":
                            _codigo_ruta = "01";
                            break;
                        case "07":
                            _codigo_ruta = "02";
                            break;
                        case "08":
                            _codigo_ruta = "02";
                            break;
                        case "20":
                            /*RETENCIONES*/
                            _codigo_ruta = "05";
                            break;
                    }
                }
                else
                {
                    switch (_tipo)
                    {
                        case "01":
                            _codigo_ruta = "03";
                            break;
                        case "03":
                            _codigo_ruta = "03";
                            break;
                        case "07":
                            _codigo_ruta = "04";
                            break;
                        case "08":
                            _codigo_ruta = "04";
                            break;
                        case "20":
                            /*RETENCIONES*/
                            _codigo_ruta = "05";
                            break;
                    }
                }
                #endregion


                DataSet dsruta = _retorna_tabla_ruta(_codigo_ruta);

                if (dsruta != null)
                {
                    DataTable dt_ruta = dsruta.Tables[0];
                    DataTable dt_credenciales = dsruta.Tables[1];
                    string _user = dt_credenciales.Rows[0]["Usuario"].ToString();
                    string _contraseña = dt_credenciales.Rows[0]["password"].ToString();

                    for (Int32 i = 0; i < dt_ruta.Rows.Count; ++i)
                    {
                        string _ruta_carpeta = dt_ruta.Rows[i]["ruta_xml"].ToString() + "\\";
                        //        //creando la carpeta de la tienda                    

                        NetworkShare.ConnectToShare(@dt_ruta.Rows[i]["ruta_xml"].ToString(), _user, _contraseña);


                        _archivo_ruta = _ruta_carpeta + "\\" + _tienda_archivo + ".xml";

                        File.WriteAllBytes(@_archivo_ruta, _archivo_xml);

                    }

                }
                else
                {
                    _error = "error de conexion";
                }

            }
            catch (Exception exc)
            {

                _error = exc.Message;
                //throw;
            }
            return _error;
        }



        #region<METODO PARA DESARROLLO>
        public static string exportar_archivo_xml_desarrollo(byte[] _archivo_xml, string _tienda_archivo)
        {
            string _archivo_ruta = "";
            string _error = "";
            try
            {

                string _tipo = _tienda_archivo.Substring(12, 2);

                string _codigo_ruta = "";

                switch (_tipo)
                {
                    case "01":
                        _codigo_ruta = "01";
                        break;
                    case "03":
                        _codigo_ruta = "01";
                        break;
                    case "07":
                        _codigo_ruta = "02";
                        break;
                    case "08":
                        _codigo_ruta = "02";
                        break;
                    case "20":
                        _codigo_ruta = "05";
                        break;
                }

                DataSet dsruta = _retorna_tabla_ruta(_codigo_ruta);

                if (dsruta != null)
                {
                    DataTable dt_ruta = dsruta.Tables[0];
                    DataTable dt_credenciales = dsruta.Tables[1];
                    string _user = dt_credenciales.Rows[0]["Usuario"].ToString();
                    string _contraseña = dt_credenciales.Rows[0]["password"].ToString();

                    for (Int32 i = 0; i < dt_ruta.Rows.Count; ++i)
                    {
                        string _ruta_carpeta = dt_ruta.Rows[i]["rut_des"].ToString() + "\\";
                        //        //creando la carpeta de la tienda                    

                        NetworkShare.ConnectToShare(@dt_ruta.Rows[i]["rut_des"].ToString() + "\\", _user, _contraseña);


                        _archivo_ruta = _ruta_carpeta + "\\" + _tienda_archivo + ".xml";

                        File.WriteAllBytes(@_archivo_ruta, _archivo_xml);

                    }

                }
                else
                {
                    _error = "error de conexion";
                }
            }
            catch (Exception exc)
            {

                _error = exc.Message;
                //throw;
            }
            return _error;
        }
        #endregion
    }
}