using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml;
using Common.Entities.UBL;
using Bata.FEPE.TemplateEngine.Support.Sunat;
using System.Xml.Xsl;
using Carvajal.FEPE.TemplateEngine.Support.Sunat;

namespace www.servicebata.com
{
    public class Basico
    {
        private static string conexion
        {
            get {return "Server=posperu.bgr.pe;Database=BdWSBata;User ID=pos_oracle;Password=Bata2018**;Trusted_Connection=False;"; }
        }
        private static string conexion_carvajal
        {
            get { return "Server=10.10.10.28;Database=FEPE_SC;User ID=dmendoza;Password=Bata2013;Trusted_Connection=False;"; }
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

        #region <GENERA PDF EXTENDIDO>
        private static DataTable get_compro_anexoID(String _tip_doc, String _ser_doc, Decimal _num_ini, Decimal _nu_fin)
        {

            String sqlquery = "[GET_XML_DOC]";
            SqlConnection cn = null;
            SqlCommand cmd = null;
            SqlDataAdapter da = null;
            DataTable dt = null;
            try
            {
                cn = new SqlConnection(conexion_carvajal);
                cmd = new SqlCommand(sqlquery, cn);
                cmd.CommandTimeout = 0;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ran_fecha", false);
                cmd.Parameters.AddWithValue("@fec_ini", DateTime.Today);
                cmd.Parameters.AddWithValue("@fec_fin", DateTime.Today);
                cmd.Parameters.AddWithValue("@tip_doc", _tip_doc);
                cmd.Parameters.AddWithValue("@ser_doc", _ser_doc);
                cmd.Parameters.AddWithValue("@num_ini", _num_ini);
                cmd.Parameters.AddWithValue("@num_fin", _nu_fin);
                cmd.Parameters.AddWithValue("@solo_xml_rango", true);
                da = new SqlDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);
            }
            catch
            {
                dt = null;
            }
            return dt;
        }
        private static Byte[] get_img(Decimal _id)
        {
            String sqlquery = "USP_GET_BYTES_XMLCDR";
            SqlConnection cn = null;
            SqlCommand cmd = null;
            Byte[] _image = null;
            try
            {
                cn = new SqlConnection(conexion_carvajal);
                if (cn.State == 0) cn.Open();
                cmd = new SqlCommand(sqlquery, cn);
                cmd.CommandTimeout = 0;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID", _id);
                cmd.Parameters.Add("@IMG", SqlDbType.VarBinary, -1);
                cmd.Parameters["@IMG"].Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                _image = (Byte[])cmd.Parameters["@IMG"].Value;
            }
            catch (Exception exc)
            {
                _image = null;
                if (cn != null)
                    if (cn.State == ConnectionState.Open) cn.Close();
            }
            if (cn != null)
                if (cn.State == ConnectionState.Open) cn.Close();
            return _image;
        }
        private static string GZipDecompress(byte[] zippedData)
        {
            string str1 = string.Empty;
            try
            {
                MemoryStream memoryStream = new MemoryStream(zippedData);
                GZipStream gzipStream = new GZipStream((Stream)memoryStream, CompressionMode.Decompress);
                StreamReader streamReader = new StreamReader((Stream)gzipStream, Encoding.GetEncoding("ISO-8859-1"));
                str1 = streamReader.ReadToEnd();
                streamReader.Close();
                gzipStream.Close();
                memoryStream.Close();
            }
            catch
            {
                str1 = "";
            }
            return str1;
        }
        
        private static string get_xml(Byte[] _xml)
        {
            string str = "";
            try
            {
                str = GZipDecompress(_xml);
             
            }
            catch
            {
                str = "";
            }
            return str;
        }
        private static string GetCdpType(XmlDocument xmlDocument)
        {
            new XmlNamespaceManager(xmlDocument.NameTable).AddNamespace(xmlDocument.DocumentElement.Prefix, xmlDocument.DocumentElement.NamespaceURI);
            if (xmlDocument.DocumentElement.LocalName.ToLower().Equals("invoice"))
            {
                XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("cbc:InvoiceTypeCode");
                if (elementsByTagName.Count > 0)
                    return elementsByTagName[0].InnerText ?? "";
            }
            else
            {
                if (xmlDocument.DocumentElement.LocalName.ToLower().Equals("creditnote"))
                    return 7.ToString();
                if (xmlDocument.DocumentElement.LocalName.ToLower().Equals("debitnote"))
                    return 8.ToString();
            }
            return "";
        }
        private static string TransformXMLToHTML(string inputXml, string xsltString)
        {
            try
            {
                //XmlDocument xmlDocument = new XmlDocument();
                //xmlDocument.LoadXml(inputXml);
                //string cdpType = GetCdpType(xmlDocument);
                //XmlNamespaceManager xmlNamespaceManager = XmlNamespaceManagerFactory.ForPaymentReceiptFile(cdpType, xmlDocument.NameTable);
                //SunatBarcode sunatBarcode = new SunatBarcodeFactory(cdpType, xmlNamespaceManager).Build(xmlDocument);
                //XsltArgumentList arguments = new XsltArgumentList();
                //arguments.AddParam("codigoBarras", string.Empty, (object)sunatBarcode.ToBase64());
                //arguments.AddParam("hash", string.Empty, (object)sunatBarcode.DigestValue);
                //XsltSettings settings = new XsltSettings(true, true);
                //XslCompiledTransform compiledTransform = new XslCompiledTransform();
                //using (XmlReader stylesheet = XmlReader.Create((TextReader)new StringReader(xsltString)))
                //    compiledTransform.Load(stylesheet, settings, (XmlResolver)new XmlUrlResolver());
                //StringWriter stringWriter = new StringWriter();
                //using (XmlReader input = XmlReader.Create((TextReader)new StringReader(inputXml)))
                //    compiledTransform.Transform(input, arguments, (TextWriter)stringWriter);
                //return stringWriter.ToString();
            }
            catch (Exception exc)
            {
                //MessageBox.Show(exc.Message);
            }
            return "";
        }
        private static string genera_html_str(string strPathXML, string strPathXSLT, string strPathPDFFolder, string _tipo_doc)
        {
            string str = "";
            try
            {
               
                string xsltString = File.ReadAllText(strPathXSLT);

                string _ruta_exe_local = AppDomain.CurrentDomain.BaseDirectory;

                xsltString = xsltString.Replace("../ReferenceData/ISO.xslt", @_ruta_exe_local + "/ReferenceData/ISO.xslt");
                xsltString = xsltString.Replace("../ReferenceData/INEI.xslt", @_ruta_exe_local + "/ReferenceData/INEI.xslt");                

                Encoding iso = Encoding.GetEncoding("iso8859-1");
                
                string msg = iso.GetString(iso.GetBytes(strPathXML));

                str = TransformXMLToHTML(msg, xsltString);

            }
            catch
            {
                str = "";
            }

            return str;
        }
        private static byte[] returnpdf_bytes(string str, string _tipo_doc)
        {
            byte[] pdf_bytes;
            try
            {                       
            var htmlContent = str;            
            var htmlToPdf = new NReco.PdfGenerator.HtmlToPdfConverter();

            if (_tipo_doc == "20") /*retencion*/
            {
                htmlToPdf.PageHeight = 742;
                htmlToPdf.PageWidth = 670;
            }
            else
            {
                htmlToPdf.PageHeight = 242;
                htmlToPdf.PageWidth = 170;
            }
           
            var margins = new NReco.PdfGenerator.PageMargins();
            margins.Bottom = 2;
            margins.Top = 1;
            margins.Left = 2;
            margins.Right = 5;
            htmlToPdf.Margins = margins;
            htmlToPdf.Orientation = NReco.PdfGenerator.PageOrientation.Portrait;
            htmlToPdf.Zoom = 1F;
            htmlToPdf.Size = NReco.PdfGenerator.PageSize.A4;


            pdf_bytes = htmlToPdf.GeneratePdf(htmlContent);
          
            }
            catch
            {
                pdf_bytes = null;
            }
            return pdf_bytes;

        }
        private static string _ruta_Xslt_doc(string _tipo, string _emp = "E")
        {
            string _ruta_xslt = "";            
           
            string _ruta_exe_local = AppDomain.CurrentDomain.BaseDirectory;
            try
            {              
                if (_emp == "E")
                {
                    switch (_tipo)
                    {
                        case "01":
                            //factura
                            return _ruta_exe_local + "\\XSLT\\FA_FA.xslt";

                        case "03":
                            //boleta
                            return _ruta_exe_local + "\\XSLT\\BO_BO.xslt";
                        case "07":
                            //nota de credito
                            return _ruta_exe_local + "\\XSLT\\NC_NC.xslt";
                        case "08":
                            //nota de debito
                            return _ruta_exe_local + "\\XSLT\\ND_ND.xslt";
                        case "20":
                            return _ruta_exe_local + "\\XSLT\\RE_RE.xslt";
                    }
                }
                if (_emp == "T")
                {
                    switch (_tipo)
                    {
                        case "01":
                            //factura
                            return _ruta_exe_local + "\\XSLT\\FA.xslt";

                        case "03":
                            //boleta
                            return _ruta_exe_local + "\\XSLT\\BO.xslt";
                        case "07":
                            //nota de credito
                            return _ruta_exe_local + "\\XSLT\\NC.xslt";
                        case "08":
                            //nota de debito
                            return _ruta_exe_local + "\\XSLT\\ND.xslt";
                    }
                }
            }
            catch
            {
                throw;
            }
            return _ruta_xslt;
        }
        public static Byte[] get_pdf(String _tip_doc, String _ser_doc, Decimal _num_ini, Decimal _nu_fin)
        {
            Byte[] ws = null;
            try
            {
                _nu_fin = _num_ini;
                DataTable dt_anexos = get_compro_anexoID(_tip_doc, _ser_doc, _num_ini, _nu_fin);
                string _ruta_xslt = _ruta_Xslt_doc(_tip_doc);
                if (dt_anexos!=null)
                {
                    if (dt_anexos.Rows.Count>0)
                    {
                        string str_html = "";
                        foreach (DataRow fila in dt_anexos.Rows)
                        {
                            Decimal _id = Convert.ToDecimal(fila["id"]);
                            Byte[] _xml = get_img(_id);
                            string _str_xml=get_xml(_xml);
                            str_html += genera_html_str(_str_xml, _ruta_xslt, "", _tip_doc);                                                    
                        }
                        ws= returnpdf_bytes(str_html, _tip_doc);
                    }
                }
            }
            catch
            {
                ws = null;
            }
       
            return ws;
        }
        #endregion
    }
}