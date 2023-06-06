using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClosedXML.Excel;

namespace DataSetGenerador
{



    public partial class DS_MovimientoVigilancia : Form
    {

        List<PredioNivelContagio> listaNivelContagio = new List<PredioNivelContagio>();
        List<PredioCantidadAMov> listaCantidadMovSalida = new List<PredioCantidadAMov>();
        List<PredioCantidadAMov> listaCantidadMovEntrada = new List<PredioCantidadAMov>();
        List<PredioNivelMasaMovimiento> listaMasaAnimal = new List<PredioNivelMasaMovimiento>();
        List<PredioResultados> listaResultados = new List<PredioResultados>();

        List<ResultadosProtocolo> listaResultadosProtocolo = new List<ResultadosProtocolo>();
        List<ResultadoProtocoloPc> listaResultadoPc = new List<ResultadoProtocoloPc>();
        List<ClasificacionPredial> listaClasificacion = new List<ClasificacionPredial>();
        List<CantidadProtocolos> listaCantidadProtocolos = new List<CantidadProtocolos>();
        List<DataSetMovimientoVigilancia> listaFinal = new List<DataSetMovimientoVigilancia>();
        public DS_MovimientoVigilancia()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            BackgroundWorker backgroundWorker = new BackgroundWorker();
            progressBar1.Maximum = 100;
            progressBar1.Step = 1;
            progressBar1.Value = 0;

            listaNivelContagio.Clear();

            try
            {
                await Task.Run(() =>
                {
                    richTextBox1.Invoke(new MethodInvoker(delegate
                    {
                        this.richTextBox1.AppendText("SE INICIA PROCESO DE GENERACION DATA SET TRAIN");
                        this.richTextBox1.ScrollToCaret();
                    }));

                    ////////////IR A BUSCAR LAS CLASIFICACIONES DE LA REGION PARA TBB 
                    ////////////POR CADA RUP CLASIFICADO IR A BUSCAR EL  ULTIMO PROTOCOLO (PC O LAB) 
                    ////////////TENER LA CANTIDAD DE NEGATIVOS Y CANTIDAD DE POSITIVOS

                    //////////this.AplicarNivelContagio("10");//coordenadas del predio
                    //////////this.AplicarPesoEntrada("10", 730);//2 años atras
                    //////////this.AplicarPesoSalida("10", 730);
                    //////////this.PesoInOutMovimiento();
                    //////////this.ClasificacionSanidad(174, 39, 10);//1 año atras
                    //////////this.ResultadosProtocoloRegionTM(174, 39, 10, 2);// 2 años atras
                    //////////this.ResultadosProtocoloRegionOrigen(174, 39, 10, 2);// 2 años atras
                    //////////this.ResultadosProtocoloPCTBBobivno(174, 39, 10, 2);// 2 años atras
                    //////////this.CantidadProtocolosRegion(10, 2);// 2 años atras

                    //////////this.ProcesoConsolidacion("TBB_Bovino_Train", 1);


                    ProcesoGeneracionDSEntrenamiento("TBB_Bovino_Train");
                });


            }
            catch (Exception ex)
            {

            }
        }
        private async void button2_Click(object sender, EventArgs e)
        {
            BackgroundWorker backgroundWorker = new BackgroundWorker();
            progressBar1.Maximum = 100;
            progressBar1.Step = 1;
            progressBar1.Value = 0;

            listaNivelContagio.Clear();

            try
            {
                await Task.Run(() =>
                {
                    richTextBox1.Invoke(new MethodInvoker(delegate
                    {
                        this.richTextBox1.AppendText("Se inicia proceso");
                        this.richTextBox1.ScrollToCaret();
                    }));

                    //IR A BUSCAR LAS CLASIFICACIONES DE LA REGION PARA TBB 
                    //POR CADA RUP CLASIFICADO IR A BUSCAR EL  ULTIMO PROTOCOLO (PC O LAB) 
                    //TENER LA CANTIDAD DE NEGATIVOS Y CANTIDAD DE POSITIVOS
                    if (textBox1.Text != "")
                    {
                        int regionId = Int32.Parse(textBox1.Text.ToString());
                        this.AplicarNivelContagio(regionId.ToString());//coordenadas del predio
                        this.AplicarPesoEntrada(regionId.ToString(), 730);//2 años atras
                        this.AplicarPesoSalida(regionId.ToString(), 730);
                        this.PesoInOutMovimiento();
                        this.ClasificacionSanidad(174, 39, regionId);//1 año atras
                        this.ResultadosProtocoloRegionTM(174, 39, regionId, 2);// 2 años atras
                        this.ResultadosProtocoloRegionOrigen(174, 39, regionId, 2);// 2 años atras
                        this.ResultadosProtocoloPCTBBobivno(174, 39, regionId, 2);// 2 años atras
                        this.CantidadProtocolosRegion(regionId, 2);// 2 años atras

                        string nombreFile = "Region" + textBox1.Text.ToString();
                        this.ProcesoConsolidacion(nombreFile, 0);
                    }


                });


            }
            catch (Exception ex)
            {

            }
        }

        void ProcesoGeneracionDSEntrenamiento(string fileName)
        {
            progressTrain.Invoke(new MethodInvoker(delegate
            {
                progressTrain.Maximum = 100;
                progressTrain.Step = 1;
                progressTrain.Value = 1;
            }));

            richTextBox1.Invoke(new MethodInvoker(delegate
            {
                richTextBox1.AppendText("\r\n" + "Se inicia proceso generacion data set TRAIN");
                richTextBox1.ScrollToCaret();
            }));
            if (textBox2.Text != "")
            {
                int filasCruce = Int32.Parse(textBox2.Text.ToString());

                string filePath = @"C:\Emprendimientos\IA_Python\PrediccionEnfermedades\Tools\DataSetGenerador\DataSetGenerador\Matriz\MatrizBaseTrain.xlsx";


                using (XLWorkbook workBook = new XLWorkbook(filePath))
                {
                    //Read the first Sheet from Excel file.
                    IXLWorksheet workSheet = workBook.Worksheet(1);

                    //Create a new DataTable.
                    DataTable dt = new DataTable();

                    //Loop through the Worksheet rows.
                    bool firstRow = true;
                    foreach (IXLRow row in workSheet.Rows())
                    {
                        //Use the first row to add columns to DataTable.
                        if (firstRow)
                        {
                            foreach (IXLCell cell in row.Cells())
                            {
                                dt.Columns.Add(cell.Value.ToString());
                            }
                            firstRow = false;
                        }
                        else
                        {
                            //Add rows to DataTable.
                            dt.Rows.Add();
                            int i = 0;
                            foreach (IXLCell cell in row.Cells())
                            {
                                dt.Rows[dt.Rows.Count - 1][i] = cell.Value.ToString();
                                i++;
                            }
                        }


                    }
                    var csv = new StringBuilder();
                    int indice = 0;



                    int flag = 0;
                    //int contador1 = 0;
                    //int contador2 = 0;
                    //int contador3 = 0;
                    //int contador4 = 0;
                    //int contador5 = 0;
                    //int contador6 = 0;
                    //int contador7 = 0;
                    //int contador8 = 0;
                    //int contador9 = 0;
                    //int contador10 = 0;
                    //// debe llegar a 240 los contadores
                    //List<MatrizEntrenamiento> matr = new List<MatrizEntrenamiento>();
                    //for (var i = 0; i < dt.Rows.Count; i++)
                    //{
                    //    //LISTA BASE 
                    //    MatrizEntrenamiento ob = new MatrizEntrenamiento();
                    //    ob.PesoNivelContagio = Int32.Parse(dt.Rows[i].ItemArray[0].ToString());
                    //    ob.PesoMasaEntrada = Int32.Parse(dt.Rows[i].ItemArray[1].ToString());
                    //    ob.PesoMasaSalida = Int32.Parse(dt.Rows[i].ItemArray[2].ToString());
                    //    ob.PesoNivelContagio = Int32.Parse(dt.Rows[i].ItemArray[3].ToString());
                    //    ob.TodosNegativosLab = Int32.Parse(dt.Rows[i].ItemArray[4].ToString());
                    //    ob.AlMenosUnPositivoLab = Int32.Parse(dt.Rows[i].ItemArray[5].ToString());
                    //    ob.TodosNegativosPc = Int32.Parse(dt.Rows[i].ItemArray[6].ToString());
                    //    ob.AlMenosUnPositivoPc = Int32.Parse(dt.Rows[i].ItemArray[7].ToString());
                    //    ob.RiesgoMovimiento = Int32.Parse(dt.Rows[i].ItemArray[8].ToString());

                    //    matr.Add(ob);

                    //}

                    for (var i = 0; i < dt.Rows.Count; i++)
                    {

                        for (var x = 0; x < filasCruce; x++)
                        {

                            Random r = new Random();
                            Int32 rInt = r.Next(1, 5000000);
                            int idRandom = rInt + i + x;

                            if (flag == 0)
                            {
                                flag++;
                                csv.AppendLine("Id;RUP;Oficina;CoordenadaX;CoordenadaY;Huso;Latitud;Longitud;" +
                            "Peso Nivel Contagio;" +
                            "Peso Masa Entrada;Peso Masa Salida;Nivel Vigilancia;ProtocoloLab;NegativosLab;" +
                            "PositivosLab;" +
                            "ProtocoloPc; NegativosPc; PositivosPc;TodosNegativosLab;AlMenosUnPositivoLab;" +
                            "TodosNegativosPc;AlMenosUnPositivoPc;" +
                            "ClasificacionSanitaria;" +
                            " RiesgoMovimiento; TextoRiesgo");
                            }

                            string newLine = "";
                            //newLine = "" + dt.Rows[i].ItemArray[0] +
                            //                         ";" + dt.Rows[i].ItemArray[1] +
                            //                         ";" + dt.Rows[i].ItemArray[2] +
                            //                         ";" + dt.Rows[i].ItemArray[3] +
                            //                         ";" + dt.Rows[i].ItemArray[4] +
                            //                         ";" + dt.Rows[i].ItemArray[5] +
                            //                         ";" + dt.Rows[i].ItemArray[6] +
                            //                         ";" + dt.Rows[i].ItemArray[7] +
                            //                         ";" + dt.Rows[i].ItemArray[8] + "";
                            //csv.AppendLine(newLine);

                            newLine += "" + idRandom + "; RUP_TRAIN;OficinaTRAIN;0;0;0;0;0" +
                                ";" + dt.Rows[i].ItemArray[0] +
                                                      ";" + dt.Rows[i].ItemArray[1] +
                                                      ";" + dt.Rows[i].ItemArray[2] +
                                                      ";" + dt.Rows[i].ItemArray[3] +
                                                      ";0" +
                                                      ";0" +
                                                      ";0" +
                                                      ";0" +
                                                      ";0" +
                                                      ";0" +
                                                      ";" + dt.Rows[i].ItemArray[4] +
                                                      ";" + dt.Rows[i].ItemArray[5] +
                                                      ";" + dt.Rows[i].ItemArray[6] +
                                                      ";" + dt.Rows[i].ItemArray[7] +
                                                      ";0" +
                                                      ";" + dt.Rows[i].ItemArray[8] +
                                                      ";";
                            csv.AppendLine(newLine);



                        }


                        indice++;
                        var porcentaje = (indice * 100) / dt.Rows.Count;
                        progressTrain.Invoke(new MethodInvoker(delegate
                        {
                            progressTrain.Value = porcentaje;
                        }));
                    }
                    File.WriteAllText(@"C:\Emprendimientos\IA_Python\PrediccionEnfermedades\DataSet\DataSet_Generador\" + fileName + ".csv", csv.ToString());
                    richTextBox1.Invoke(new MethodInvoker(delegate
                    {
                        richTextBox1.AppendText("\r\n" + "Se FINALIZA proceso de generacion data set TRAIN");
                        richTextBox1.ScrollToCaret();
                    }));

                }
            }
        }
        int AplicarNivelContagio(string regionId)
        {
            string constr = "User Id=SIPEC;Password=sipec_testsm.2019;Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=192.168.51.249)(PORT=1540)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=testsm)))";

            OracleConnection con = new OracleConnection(constr);
            con.Open();
            String sql = "";


            progressBar1.Invoke(new MethodInvoker(delegate
            {
                progressBar1.Value = 1;
            }));
            richTextBox1.Invoke(new MethodInvoker(delegate
            {
                richTextBox1.AppendText("\r\n" + "Buscando Predios clasificados");
                richTextBox1.ScrollToCaret();
            }));


            sql += "select  DISTINCT  ";
            sql += " a.id, a.rup, a.ID_REGI, tipo.id as idTEstab, tipo.descripcion , ";
            sql += " CE.Id_Rubro, CE.DESC_RUBRO, a.ID_OFSE ,a.COORDENADA_X	,a.COORDENADA_Y	,a.HUSO";
            sql += " FROM ";
            sql += " SIPEC.SIP_D_ESTABLECIMIENTOS a ";
            sql += " INNER JOIN SIPEC.sip_d_establecimientos_ties tie ";
            sql += " ON a.id = tie.id_esta ";
            sql += " INNER JOIN  SIPEC.sip_d_tipos_establecimientos tipo ";
            sql += " ON tie.id_ties = tipo.id ";
            //sql += " INNER JOIN  SIPEC.SIP_D_ENCARGADOS_REGIONALES ER ";
            //sql += " ON a.id_regi = ER.idregion ";
            //sql += " INNER JOIN SIPEC.SIP_D_ENCARGADOS_SECTORIALES ES ";
            //sql += " ON a.ID_OFSE = ES.IDSECTORIAL ";
            //sql += " INNER JOIN MS_REGION_VW R ";
            //sql += " ON a.id_regi = R.IDREGION ";
            //sql += " INNER JOIN MS_SECTORIAL_VW OS ";
            //sql += " ON a.ID_OFSE = OS.IDSECTORIAL ";
            sql += " LEFT JOIN SIPEC.SIP_D_CLAS_ASIG_ESTAB CE ";
            sql += " ON(a.ID = CE.ID_ESTA AND CE.vigente = 1) ";
            sql += " LEFT JOIN SIPEC.sip_d_especies E ";
            sql += " ON E.ID = CE.ID_ESPE ";
            sql += " left JOIN SIPEC.sip_d_grupos_especies GE ";
            sql += "  ON GE.ID = E.ID_GRES ";
            sql += "where ";
            sql += " a.ID_REGI = " + regionId + " ";
            sql += " and GE.ID = 1 ";
            // sql += "  a.rup = '" + rup + "' ";
            sql += "and tie.fecha_termino is null ";

            OracleDataAdapter datos = new OracleDataAdapter(sql, con);

            DataSet data = new DataSet();
            richTextBox1.Invoke(new MethodInvoker(delegate
            {
                richTextBox1.AppendText("\r\n" + "Esperando respuesta desde BD");
                richTextBox1.ScrollToCaret();
            }));

            datos.Fill(data);
            int i;
            int peso = 0;
            richTextBox1.Invoke(new MethodInvoker(delegate
            {
                richTextBox1.AppendText("\r\n" + "Se inicia proceso de Nivel Contagio");
                richTextBox1.ScrollToCaret();
            }));
            if (data.Tables[0].Rows.Count == 0)
            {
                // NO HAY CLASIFICACION; SE ASIGNA EL PESO MAYOR
                peso = 10;
                con.Close();
                return peso;
            }
            for (i = 0; i < data.Tables[0].Rows.Count; i++)
            {
                PredioNivelContagio obj = new PredioNivelContagio();

                Console.WriteLine(data.Tables[0].Rows[i][4].ToString());
                var te = data.Tables[0].Rows[i][3].ToString();
                var rubro = data.Tables[0].Rows[i][5].ToString();
                //1 PREDIO / 1 LECHE => 10 
                //1 PREDIO / 2 CARNE => 5
                //148 FERIA / ...    => 15
                peso = 0;
                // sumar el peso si tiene mas de un rubro
                if (te == "1" && rubro == "1")
                {
                    peso = peso + 10;
                }
                else
                {
                    if (te == "148")
                    {
                        peso = peso + 15;
                    }
                    else
                    {
                        if (te == "1" && rubro == "2")
                        {
                            peso = peso + 5;
                        }
                        else
                        {
                            peso = peso + 1;
                        }

                    }
                }


                //ADD AL LIST
                obj.Rup = data.Tables[0].Rows[i][1].ToString();
                obj.Peso = peso;
                obj.IdRup = Int32.Parse(data.Tables[0].Rows[i][0].ToString());
                obj.OficinaId = Int32.Parse(data.Tables[0].Rows[i][7].ToString());
                obj.CoordenadaX = data.Tables[0].Rows[i][8].ToString();
                obj.CoordenadaY = data.Tables[0].Rows[i][9].ToString();
                obj.Huso = data.Tables[0].Rows[i][10].ToString();

                if (obj.Peso < 15)
                    listaNivelContagio.Add(obj);
                var porcentaje = (i * 100) / data.Tables[0].Rows.Count;
                progressBar1.Invoke(new MethodInvoker(delegate
                {
                    progressBar1.Value = porcentaje;
                }));


            }

            con.Close();
            richTextBox1.Invoke(new MethodInvoker(delegate
            {
                richTextBox1.AppendText("\r\n" + "Se finaliza proceso Nivel Contagio");
                richTextBox1.ScrollToCaret();
            }));
            return peso;
        }
        string ObtenerResultadoCantidad(string rup)
        {
            var data = listaResultados.Where(r => r.Rup == rup);

            if (data.Count() > 0)
            {
                int totalAnalisis = 0;
                double valorMaximo = 0.0;
                string valor = "0.0";
                double c = 0;
                int flag = 0;
                foreach (var s in data)
                {
                    totalAnalisis = totalAnalisis + s.cantidadAnalisis;

                    c = Convert.ToDouble(s.resultado);
                    if (flag == 0)
                    {
                        flag++;
                        valorMaximo = c;
                        valor = s.resultado;
                    }
                    if (c > valorMaximo)
                    {
                        valorMaximo = c;
                        valor = s.resultado;
                    }


                }
                return valor + "|" + totalAnalisis.ToString();
            }
            else
            {
                return "0.0|0";
            }
        }
        void PesoInOutMovimiento()
        {
            try
            {
                var csv = new StringBuilder();

                progressBar2.Invoke(new MethodInvoker(delegate
                {
                    progressBar2.Maximum = 100;
                    progressBar2.Step = 1;
                    progressBar2.Value = 0;
                }));

                richTextBox1.Invoke(new MethodInvoker(delegate
                {
                    richTextBox1.AppendText("\r\n" + "Se inicia proceso de escalado de movimiento y TE");
                    richTextBox1.ScrollToCaret();
                }));

                int indice = 0;
                foreach (var i in listaNivelContagio)
                {
                    PredioNivelMasaMovimiento obj = new PredioNivelMasaMovimiento();
                    var outData = listaCantidadMovSalida.FirstOrDefault(f => f.Rup == i.Rup);
                    int pesoSalidaAnimales = 0;
                    if (outData != null)
                        pesoSalidaAnimales = outData.Cantidad;
                    var inData = listaCantidadMovEntrada.FirstOrDefault(f => f.Rup == i.Rup);
                    int pesoEntradaAnimales = 0;
                    if (inData != null)
                        pesoEntradaAnimales = inData.Cantidad;

                    //richTextBox1.Invoke(new MethodInvoker(delegate
                    //{
                    //    richTextBox1.AppendText("\r\n" + "Obtener VIGILANCIA para el RUP: " + i.Rup + "");
                    //    richTextBox1.ScrollToCaret();
                    //}));
                    //string dataSSA = AplicarNivelVigilancia(i.Rup, i.Peso, especie, enfermedad);
                    //int cantidadAnalisis = 0;
                    //int cantidadPositivos = 0;
                    //if (dataSSA.Length > 0)
                    //{
                    //    var dataarr = dataSSA.Split('|');
                    //    // tienePositivo.ToString() + "|" + cantidadAnalisis.ToString();
                    //    cantidadPositivos = Int32.Parse(dataarr[0]);
                    //    cantidadAnalisis = Int32.Parse(dataarr[1]);
                    //}
                    //richTextBox1.Invoke(new MethodInvoker(delegate
                    //{
                    //    richTextBox1.AppendText("\r\n" + "RESULTADO DE VIGILANCIA para el RUP: " + i.Rup + " ES: " + dataSSA + "");
                    //    richTextBox1.ScrollToCaret();
                    //}));
                    obj.IdRup = i.IdRup;
                    obj.Rup = i.Rup;
                    obj.PesoContagio = i.Peso.ToString();
                    obj.PesoMasaEntrada = EscalaMasaAnimales(pesoEntradaAnimales).ToString();
                    obj.PesoMasaSalida = EscalaMasaAnimales(pesoSalidaAnimales).ToString();
                    obj.OficinaId = i.OficinaId;
                    obj.CoordenadaX = i.CoordenadaX;
                    obj.CoordenadaY = i.CoordenadaY;
                    obj.Huso = i.Huso;

                    listaMasaAnimal.Add(obj);
                    //richTextBox1.Invoke(new MethodInvoker(delegate
                    //{
                    //    richTextBox1.AppendText("\r\n" + "NUEVA LINEA CSV");
                    //    richTextBox1.ScrollToCaret();
                    //}));
                    //var newLine = string.Format("{0},{1},{2},{3},{4},{5}", obj.Rup, obj.PesoContagio,
                    //  obj.PesoMasaEntrada, obj.PesoMasaSalida, obj.NivelVigilancia, obj.CantidadPositivos);
                    //csv.AppendLine(newLine);
                    //File.WriteAllText(@"C:\Emprendimientos\IA_Python\PrediccionEnfermedades\DataSet\DataSet_Generador\dataSetMovAnalisisTBB.csv", csv.ToString());

                    indice++;
                    var porcentaje = (indice * 100) / listaNivelContagio.Count;
                    progressBar2.Invoke(new MethodInvoker(delegate
                    {
                        progressBar2.Value = porcentaje;
                    }));

                }
            }
            catch (Exception ex)
            {
                var s = ex.Message;
            }
            richTextBox1.Invoke(new MethodInvoker(delegate
            {
                richTextBox1.AppendText("\r\n" + "Se FINALIZA proceso de escalado de movimiento y TE");
                richTextBox1.ScrollToCaret();
            }));




        }
        void ClasificacionSanidad(int enfermedadId, int especieId, int regionId)
        {
            string connectionString = "Server =192.168.1.237;Database=sanidadanimal;User Id=usr.sanidad_lee;Password=sanidad.lee_2022";

            string queryString = "";



            richTextBox1.Invoke(new MethodInvoker(delegate
            {
                richTextBox1.AppendText("\r\n" + "Se inicia proceso de obtener Clasificaciones SSA");
                richTextBox1.ScrollToCaret();
            }));

            queryString += " select rup,TipoClasificacionId from BitacoraClasificacionPredial where RegionId = " + regionId + " and EspecieId = " + especieId + " ";
            queryString += " and EnfermedadId = " + enfermedadId + " ";
            queryString += " and FechaClasificacion between(select dateadd(year, -1, getdate())) and getdate() ";
            queryString += " and clasificacionactual = 1";
            queryString += " order by rup ";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(queryString, connection);

                    connection.Open();
                    SqlDataAdapter da = new SqlDataAdapter(command);

                    try
                    {
                        var csv = new StringBuilder();
                        DataSet ds = new DataSet("Tabla");
                        da.Fill(ds, "NuevaTabla");
                        var data = ds;
                        int indice = 0;
                        foreach (DataTable table in ds.Tables)
                        {
                            foreach (DataRow dr in table.Rows)
                            {

                                string rup = dr["rup"].ToString();
                                int clasificacion = Int32.Parse(dr["TipoClasificacionId"].ToString());

                                ClasificacionPredial o = new ClasificacionPredial();
                                o.Rup = rup;
                                o.Clasificacion = clasificacion;
                                listaClasificacion.Add(o);

                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        var rar = ex.Message;

                    }
                }
                richTextBox1.Invoke(new MethodInvoker(delegate
                {
                    richTextBox1.AppendText("\r\n" + "Se FINALIZA proceso de obtener Clasificacion SSA");
                    richTextBox1.ScrollToCaret();
                }));
            }
            catch (Exception ex)
            {
                richTextBox1.Invoke(new MethodInvoker(delegate
                {
                    richTextBox1.AppendText("\r\n" + "ERROR ");
                    richTextBox1.ScrollToCaret();
                }));
            }

        }
        void ResultadosProtocoloRegionTM(int enfermedadId, int especieId, int regionId, int cantidadAniosAtras)
        {
            string connectionString = "Server =192.168.1.237;Database=sanidadanimal;User Id=usr.sanidad_lee;Password=sanidad.lee_2022";

            string queryString = "";

            progressResultados.Invoke(new MethodInvoker(delegate
            {
                progressResultados.Maximum = 100;
                progressResultados.Step = 1;
                progressResultados.Value = 0;
            }));

            richTextBox1.Invoke(new MethodInvoker(delegate
            {
                richTextBox1.AppendText("\r\n" + "Se inicia proceso de obtener Resultados Region Toma Muestra SSA");
                richTextBox1.ScrollToCaret();
            }));

            queryString += " select  RupTomaMuestra, RupOrigen,bm.ProtocoloId, ";
            queryString += " (select count(id) from bitacoramuestraanalisis where protocoloid = bm.ProtocoloId ";

            queryString += " and ContieneEnfermedad = 0) as [negativos], ";
            queryString += " (select count(id) from bitacoramuestraanalisis where protocoloid = bm.ProtocoloId ";

            queryString += " and ContieneEnfermedad = 1) as [positivos] ";
            queryString += " from bitacoramuestraanalisis bm ";
            queryString += " where muestracerrada = 1 ";
            queryString += " and EspecieId = " + especieId + " and EnfermedadId = " + enfermedadId + " ";
            queryString += " and FechaCreacion between(select dateadd(year, - " + cantidadAniosAtras + ", getdate())) and getdate() ";
            queryString += " and RegionId = " + regionId + " ";
            queryString += " group by RupTomaMuestra,RupOrigen,ProtocoloId ";
            queryString += " order by ProtocoloId desc ";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(queryString, connection);

                    connection.Open();
                    SqlDataAdapter da = new SqlDataAdapter(command);

                    try
                    {
                        var csv = new StringBuilder();
                        DataSet ds = new DataSet("Tabla");
                        da.Fill(ds, "NuevaTabla");
                        var data = ds;
                        int indice = 0;
                        foreach (DataTable table in ds.Tables)
                        {
                            foreach (DataRow dr in table.Rows)
                            {


                                ResultadosProtocolo o = new ResultadosProtocolo();
                                o.RupTomaMuestra = dr["RupTomaMuestra"].ToString();
                                o.RupOrigen = dr["RupOrigen"].ToString();
                                o.ProtocoloLab = Int32.Parse(dr["ProtocoloId"].ToString());
                                o.CantidadNegativosLab = Int32.Parse(dr["negativos"].ToString());
                                o.CantidadPositivosLab = Int32.Parse(dr["positivos"].ToString());


                                listaResultadosProtocolo.Add(o);

                                indice++;
                                var porcentaje = (indice * 100) / table.Rows.Count;
                                progressResultados.Invoke(new MethodInvoker(delegate
                                {
                                    progressResultados.Value = porcentaje;
                                }));

                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        var rar = ex.Message;

                    }
                }
                richTextBox1.Invoke(new MethodInvoker(delegate
                {
                    richTextBox1.AppendText("\r\n" + "Se FINALIZA proceso de obtener Resultados Region Toma Muestra SSA");
                    richTextBox1.ScrollToCaret();
                }));
            }
            catch (Exception ex)
            {
                richTextBox1.Invoke(new MethodInvoker(delegate
                {
                    richTextBox1.AppendText("\r\n" + "ERROR ");
                    richTextBox1.ScrollToCaret();
                }));
            }

        }
        void ResultadosProtocoloRegionOrigen(int enfermedadId, int especieId, int regionId, int cantidadAniosAtras)
        {
            string connectionString = "Server =192.168.1.237;Database=sanidadanimal;User Id=usr.sanidad_lee;Password=sanidad.lee_2022";

            string queryString = "";

            progressResultados.Invoke(new MethodInvoker(delegate
            {
                progressResultados.Maximum = 100;
                progressResultados.Step = 1;
                progressResultados.Value = 0;
            }));

            richTextBox1.Invoke(new MethodInvoker(delegate
            {
                richTextBox1.AppendText("\r\n" + "Se inicia proceso de obtener Resultados Region Origen SSA");
                richTextBox1.ScrollToCaret();
            }));

            queryString += " select  RupTomaMuestra, RupOrigen,bm.ProtocoloId, ";
            queryString += " (select count(id) from bitacoramuestraanalisis where protocoloid = bm.ProtocoloId ";

            queryString += " and ContieneEnfermedad = 0) as [negativos], ";
            queryString += " (select count(id) from bitacoramuestraanalisis where protocoloid = bm.ProtocoloId ";

            queryString += " and ContieneEnfermedad = 1) as [positivos] ";
            queryString += " from bitacoramuestraanalisis bm ";
            queryString += " where muestracerrada = 1 ";
            queryString += " and EspecieId = " + especieId + " and EnfermedadId = " + enfermedadId + " ";
            queryString += " and FechaCreacion between(select dateadd(year, - " + cantidadAniosAtras + ", getdate())) and getdate() ";
            queryString += " and RegionIdOrigen = " + regionId + " ";
            queryString += " group by RupTomaMuestra,RupOrigen,ProtocoloId ";
            queryString += " order by ProtocoloId desc ";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(queryString, connection);

                    connection.Open();
                    SqlDataAdapter da = new SqlDataAdapter(command);

                    try
                    {
                        var csv = new StringBuilder();
                        DataSet ds = new DataSet("Tabla");
                        da.Fill(ds, "NuevaTabla");
                        var data = ds;
                        int indice = 0;
                        foreach (DataTable table in ds.Tables)
                        {
                            foreach (DataRow dr in table.Rows)
                            {

                                ResultadosProtocolo o = new ResultadosProtocolo();
                                o.RupTomaMuestra = dr["RupTomaMuestra"].ToString();
                                o.RupOrigen = dr["RupOrigen"].ToString();
                                o.ProtocoloLab = Int32.Parse(dr["ProtocoloId"].ToString());
                                o.CantidadNegativosLab = Int32.Parse(dr["negativos"].ToString());
                                o.CantidadPositivosLab = Int32.Parse(dr["positivos"].ToString());


                                listaResultadosProtocolo.Add(o);
                                indice++;
                                var porcentaje = (indice * 100) / table.Rows.Count;
                                progressResultados.Invoke(new MethodInvoker(delegate
                                {
                                    progressResultados.Value = porcentaje;
                                }));

                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        var rar = ex.Message;

                    }
                }
                richTextBox1.Invoke(new MethodInvoker(delegate
                {
                    richTextBox1.AppendText("\r\n" + "Se FINALIZA proceso de obtener Resultados Region Origen SSA");
                    richTextBox1.ScrollToCaret();
                }));
            }
            catch (Exception ex)
            {
                richTextBox1.Invoke(new MethodInvoker(delegate
                {
                    richTextBox1.AppendText("\r\n" + "ERROR ");
                    richTextBox1.ScrollToCaret();
                }));
            }

        }
        void ResultadosProtocoloPCTBBobivno(int enfermedadId, int especieId, int regionId, int cantidadAniosAtras)
        {
            string connectionString = "Server =192.168.1.237;Database=sanidadanimal;User Id=usr.sanidad_lee;Password=sanidad.lee_2022";

            string queryString = "";

            progressResultados.Invoke(new MethodInvoker(delegate
            {
                progressResultados.Maximum = 100;
                progressResultados.Step = 1;
                progressResultados.Value = 0;
            }));

            richTextBox1.Invoke(new MethodInvoker(delegate
            {
                richTextBox1.AppendText("\r\n" + "Se inicia proceso de obtener Resultados SSA PC TBB");
                richTextBox1.ScrollToCaret();
            }));

            queryString += "select rupPredio,ProtocoloId,CantidadNegativos,CantidadPositivos from BitacoraPruebaCampoCerrada ";
            queryString += " where EspecieId = " + especieId + " and EnfermedadId = " + enfermedadId + " and RegionId = " + regionId + " ";
            queryString += " and FechaCierre between(select dateadd(year, -" + cantidadAniosAtras + ", getdate())) and getdate() ";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(queryString, connection);

                    connection.Open();
                    SqlDataAdapter da = new SqlDataAdapter(command);

                    try
                    {
                        var csv = new StringBuilder();
                        DataSet ds = new DataSet("Tabla");
                        da.Fill(ds, "NuevaTabla");
                        var data = ds;
                        int indice = 0;
                        foreach (DataTable table in ds.Tables)
                        {
                            foreach (DataRow dr in table.Rows)
                            {

                                ResultadoProtocoloPc o = new ResultadoProtocoloPc();
                                o.RupOrigen = dr["RupPredio"].ToString();
                                o.ProtocoloPc = Int32.Parse(dr["ProtocoloId"].ToString());
                                o.CantidadNegativosPc = Int32.Parse(dr["CantidadNegativos"].ToString());
                                o.CantidadPositivosPc = Int32.Parse(dr["CantidadPositivos"].ToString());

                                listaResultadoPc.Add(o);
                                indice++;
                                var porcentaje = (indice * 100) / table.Rows.Count;
                                progressResultados.Invoke(new MethodInvoker(delegate
                                {
                                    progressResultados.Value = porcentaje;
                                }));
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        var rar = ex.Message;

                    }
                }
                richTextBox1.Invoke(new MethodInvoker(delegate
                {
                    richTextBox1.AppendText("\r\n" + "Se FINALIZA proceso de obtener Resultados SSA PC TBB");
                    richTextBox1.ScrollToCaret();
                }));
            }
            catch (Exception ex)
            {
                richTextBox1.Invoke(new MethodInvoker(delegate
                {
                    richTextBox1.AppendText("\r\n" + "ERROR ");
                    richTextBox1.ScrollToCaret();
                }));
            }

        }

        void CantidadProtocolosRegion(int regionId, int cantidadAniosAtras)
        {
            string connectionString = "Server =192.168.1.237;Database=sanidadanimal;User Id=usr.sanidad_lee;Password=sanidad.lee_2022";

            string queryString = "";

            progressResultados.Invoke(new MethodInvoker(delegate
            {
                progressResultados.Maximum = 100;
                progressResultados.Step = 1;
                progressResultados.Value = 0;
            }));

            richTextBox1.Invoke(new MethodInvoker(delegate
            {
                richTextBox1.AppendText("\r\n" + "Se inicia proceso de obtener Cantidad Protocolos");
                richTextBox1.ScrollToCaret();
            }));

            queryString += " SELECT count(PK_ProtocoloId) [canitdad],Rup from Protocolo ";
            queryString += " where RegionId = " + regionId + " and ";
            queryString += " FechaCreacion between(select dateadd(year, - " + cantidadAniosAtras + ", getdate())) and getdate() ";
            queryString += " group by rup ";
            queryString += " order by rup ";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(queryString, connection);

                    connection.Open();
                    SqlDataAdapter da = new SqlDataAdapter(command);

                    try
                    {
                        var csv = new StringBuilder();
                        DataSet ds = new DataSet("Tabla");
                        da.Fill(ds, "NuevaTabla");
                        var data = ds;
                        int indice = 0;
                        foreach (DataTable table in ds.Tables)
                        {
                            foreach (DataRow dr in table.Rows)
                            {

                                CantidadProtocolos o = new CantidadProtocolos();
                                o.Cantidad = Int32.Parse(dr["canitdad"].ToString());
                                o.Rup = dr["Rup"].ToString();

                                listaCantidadProtocolos.Add(o);
                                indice++;
                                var porcentaje = (indice * 100) / table.Rows.Count;
                                progressResultados.Invoke(new MethodInvoker(delegate
                                {
                                    progressResultados.Value = porcentaje;
                                }));
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        var rar = ex.Message;

                    }
                }
                richTextBox1.Invoke(new MethodInvoker(delegate
                {
                    richTextBox1.AppendText("\r\n" + "Se FINALIZA proceso de obtener Cantidad Protocolos");
                    richTextBox1.ScrollToCaret();
                }));
            }
            catch (Exception ex)
            {
                richTextBox1.Invoke(new MethodInvoker(delegate
                {
                    richTextBox1.AppendText("\r\n" + "ERROR ");
                    richTextBox1.ScrollToCaret();
                }));
            }

        }



        void ResultadosSanidadPCTBBobivno()
        {
            string connectionString = "Server =192.168.1.237;Database=sanidadanimal;User Id=usr.sanidad_lee;Password=sanidad.lee_2022";

            string queryString = "";

            progressResultados.Invoke(new MethodInvoker(delegate
            {
                progressResultados.Maximum = 100;
                progressResultados.Step = 1;
                progressResultados.Value = 0;
            }));

            richTextBox1.Invoke(new MethodInvoker(delegate
            {
                richTextBox1.AppendText("\r\n" + "Se inicia proceso de obtener Resultados SSA PC TBB");
                richTextBox1.ScrollToCaret();
            }));

            queryString += "select rupPredio,CantidadPositivos,CantidadPruebas from BitacoraPruebaCampoCerrada ";
            queryString += " where EspecieId = 39 and EnfermedadId = 174 and RegionId = 10 ";
            queryString += " and FechaCierre between(select dateadd(year, -2, getdate())) and getdate() ";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(queryString, connection);

                    connection.Open();
                    SqlDataAdapter da = new SqlDataAdapter(command);

                    try
                    {
                        var csv = new StringBuilder();
                        DataSet ds = new DataSet("Tabla");
                        da.Fill(ds, "NuevaTabla");
                        var data = ds;
                        int indice = 0;
                        foreach (DataTable table in ds.Tables)
                        {
                            foreach (DataRow dr in table.Rows)
                            {

                                string rup = dr["rupPredio"].ToString();
                                string resultado = dr["CantidadPositivos"].ToString();
                                int cantidad = Int32.Parse(dr["CantidadPruebas"].ToString());


                                int cantidadPositivos = Int32.Parse(resultado);

                                if (cantidadPositivos > 0)
                                {
                                    resultado = "0.3";//positivo pc
                                }

                                PredioResultados o = new PredioResultados();
                                o.Rup = rup;
                                o.resultado = resultado;
                                o.cantidadAnalisis = cantidad;
                                listaResultados.Add(o);
                                indice++;
                                var porcentaje = (indice * 100) / table.Rows.Count;
                                progressResultados.Invoke(new MethodInvoker(delegate
                                {
                                    progressResultados.Value = porcentaje;
                                }));
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        var rar = ex.Message;

                    }
                }
                richTextBox1.Invoke(new MethodInvoker(delegate
                {
                    richTextBox1.AppendText("\r\n" + "Se FINALIZA proceso de obtener Resultados SSA PC TBB");
                    richTextBox1.ScrollToCaret();
                }));
            }
            catch (Exception ex)
            {
                richTextBox1.Invoke(new MethodInvoker(delegate
                {
                    richTextBox1.AppendText("\r\n" + "ERROR ");
                    richTextBox1.ScrollToCaret();
                }));
            }

        }
        void ResultadosSanidad(int enfermedadId, int especieId)
        {
            string connectionString = "Server =192.168.1.237;Database=sanidadanimal;User Id=usr.sanidad_lee;Password=sanidad.lee_2022";

            string queryString = "";

            progressResultados.Invoke(new MethodInvoker(delegate
            {
                progressResultados.Maximum = 100;
                progressResultados.Step = 1;
                progressResultados.Value = 0;
            }));

            richTextBox1.Invoke(new MethodInvoker(delegate
            {
                richTextBox1.AppendText("\r\n" + "Se inicia proceso de obtener Resultados SSA");
                richTextBox1.ScrollToCaret();
            }));

            queryString += " DECLARE @AnioResultados AS TABLE  ";
            queryString += " (rup nvarchar(50), ";
            queryString += " resultado nvarchar(50), ";
            queryString += " resultadoInt float) ";

            queryString += " insert into @AnioResultados ";
            queryString += " select ruporigen, (case when ContieneEnfermedad = 1 ";
            queryString += " then(select top 1 pesochar ";
            queryString += " from PesosEnfermedadTecnica t where t.enfermedadId = 174 ";
            queryString += " and t.tecnicaId = TecnicaId) else '0.0'   end) as [contiene] , ";
            queryString += "  (case when ContieneEnfermedad = 1 then ";
            queryString += "  (select top 1 cast(t.pesochar as decimal) ";
            queryString += "  from PesosEnfermedadTecnica t where t.enfermedadId = EnfermedadId and ";
            queryString += "      t.tecnicaId = TecnicaId) else 0.0  end) as [resultado] ";
            queryString += " from BitacoraMuestraAnalisis ";
            queryString += " where EspecieId = " + especieId + " ";
            queryString += " and EnfermedadId = " + enfermedadId + " ";
            queryString += " and FechaCreacion between(select dateadd(year, -1, getdate())) and getdate() ";
            queryString += " and MuestraCerrada = 1 ";
            queryString += " and RegionIdOrigen = 10  ";
            queryString += " select rup, resultado, count(rup) [cantidad_analisis] from @AnioResultados ";
            queryString += " group by rup, resultado ";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(queryString, connection);

                    connection.Open();
                    SqlDataAdapter da = new SqlDataAdapter(command);

                    try
                    {
                        var csv = new StringBuilder();
                        DataSet ds = new DataSet("Tabla");
                        da.Fill(ds, "NuevaTabla");
                        var data = ds;
                        int indice = 0;
                        foreach (DataTable table in ds.Tables)
                        {
                            foreach (DataRow dr in table.Rows)
                            {

                                string rup = dr["rup"].ToString();
                                string resultado = dr["resultado"].ToString();
                                int cantidad = Int32.Parse(dr["cantidad_analisis"].ToString());

                                PredioResultados o = new PredioResultados();
                                o.Rup = rup;
                                o.resultado = resultado;
                                o.cantidadAnalisis = cantidad;
                                listaResultados.Add(o);
                                indice++;
                                var porcentaje = (indice * 100) / table.Rows.Count;
                                progressResultados.Invoke(new MethodInvoker(delegate
                                {
                                    progressResultados.Value = porcentaje;
                                }));
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        var rar = ex.Message;

                    }
                }
                richTextBox1.Invoke(new MethodInvoker(delegate
                {
                    richTextBox1.AppendText("\r\n" + "Se FINALIZA proceso de obtener Resultados SSA");
                    richTextBox1.ScrollToCaret();
                }));
            }
            catch (Exception ex)
            {
                richTextBox1.Invoke(new MethodInvoker(delegate
                {
                    richTextBox1.AppendText("\r\n" + "ERROR ");
                    richTextBox1.ScrollToCaret();
                }));
            }

        }


        string GetClasificacion(string rup)
        {
            var data = listaClasificacion.FirstOrDefault(f => f.Rup == rup);
            if (data != null)
            {
                return data.Clasificacion.ToString();
            }

            return "";
        }
        ObjetoSanitario GetObjetoSanitario(string rup)
        {
            ObjetoSanitario o = new ObjetoSanitario();
            List<ResultadosProtocolo> listaPraa = new List<ResultadosProtocolo>();
            listaPraa = listaResultadosProtocolo.OrderByDescending(x => x.ProtocoloLab).ToList();

            var data1 = listaPraa.FirstOrDefault(r => r.RupTomaMuestra == rup || r.RupOrigen == rup);

            List<ResultadoProtocoloPc> listaPcRAA = new List<ResultadoProtocoloPc>();
            listaPcRAA = listaResultadoPc.OrderByDescending(r => r.ProtocoloPc).ToList();
            var data2 = listaPcRAA.FirstOrDefault(r => r.RupOrigen == rup);

            int total = listaResultadosProtocolo.Where(r => r.RupTomaMuestra == rup || r.RupOrigen == rup).Count() + listaResultadoPc.Where(r => r.RupOrigen == rup).Count();

            if (data1 != null)
            {
                o.CantidadNegativosLab = data1.CantidadNegativosLab;
                o.CantidadPositivosLab = data1.CantidadPositivosLab;
                o.ProtocoloLab = data1.ProtocoloLab;
            }
            else
            {
                o.CantidadNegativosLab = 0;
                o.CantidadPositivosLab = 0;
                o.ProtocoloLab = 0;
            }
            if (data2 != null)
            {
                o.CantidadNegativosPc = data2.CantidadNegativosPc;
                o.CantidadPositivosPc = data2.CantidadPositivosPc;
                o.ProtocoloPc = data2.ProtocoloPc;
            }
            else
            {
                o.CantidadNegativosPc = 0;
                o.CantidadPositivosPc = 0;
                o.ProtocoloPc = 0;
            }

            o.TotalAnalisisPruebas = total;

            return o;
        }

        #region ToLatLon
        public void ToLatLon(double utmX, double utmY, string utmZone, out double latitude, out double longitude)
        {
            bool isNorthHemisphere = utmZone.Last() >= 'S';

            var diflat = -0.00066286966871111111111111111111111111;
            var diflon = -0.0003868060578;

            var zone = Int32.Parse(utmZone);
            var c_sa = 6378137.000000;
            var c_sb = 6356752.314245;
            var e2 = Math.Pow((Math.Pow(c_sa, 2) - Math.Pow(c_sb, 2)), 0.5) / c_sb;
            var e2cuadrada = Math.Pow(e2, 2);
            var c = Math.Pow(c_sa, 2) / c_sb;
            var x = utmX - 500000;
            var y = isNorthHemisphere ? utmY : utmY - 10000000;

            var s = ((zone * 6.0) - 183.0);
            var lat = y / (c_sa * 0.9996);
            var v = (c / Math.Pow(1 + (e2cuadrada * Math.Pow(Math.Cos(lat), 2)), 0.5)) * 0.9996;
            var a = x / v;
            var a1 = Math.Sin(2 * lat);
            var a2 = a1 * Math.Pow((Math.Cos(lat)), 2);
            var j2 = lat + (a1 / 2.0);
            var j4 = ((3 * j2) + a2) / 4.0;
            var j6 = ((5 * j4) + Math.Pow(a2 * (Math.Cos(lat)), 2)) / 3.0;
            var alfa = (3.0 / 4.0) * e2cuadrada;
            var beta = (5.0 / 3.0) * Math.Pow(alfa, 2);
            var gama = (35.0 / 27.0) * Math.Pow(alfa, 3);
            var bm = 0.9996 * c * (lat - alfa * j2 + beta * j4 - gama * j6);
            var b = (y - bm) / v;
            var epsi = ((e2cuadrada * Math.Pow(a, 2)) / 2.0) * Math.Pow((Math.Cos(lat)), 2);
            var eps = a * (1 - (epsi / 3.0));
            var nab = (b * (1 - epsi)) + lat;
            var senoheps = (Math.Exp(eps) - Math.Exp(-eps)) / 2.0;
            var delt = Math.Atan(senoheps / (Math.Cos(nab)));
            var tao = Math.Atan(Math.Cos(delt) * Math.Tan(nab));

            longitude = ((delt * (180.0 / Math.PI)) + s) + diflon;
            latitude = ((lat + (1 + e2cuadrada * Math.Pow(Math.Cos(lat), 2) - (3.0 / 2.0) * e2cuadrada * Math.Sin(lat) * Math.Cos(lat) * (tao - lat)) * (tao - lat)) * (180.0 / Math.PI)) + diflat;
        }
        #endregion

        void ProcesoConsolidacion(string fileName, int sw)
        {
            try
            {
                var csv = new StringBuilder();

                progressConsolidacion.Invoke(new MethodInvoker(delegate
                {
                    progressConsolidacion.Maximum = 100;
                    progressConsolidacion.Step = 1;
                    progressConsolidacion.Value = 1;
                }));

                richTextBox1.Invoke(new MethodInvoker(delegate
                {
                    richTextBox1.AppendText("\r\n" + "Se inicia proceso de Consolidacion");
                    richTextBox1.ScrollToCaret();
                }));

                int indice = 0;



                int flag = 0;

                foreach (var i in listaMasaAnimal)
                {
                    DataSetMovimientoVigilancia obj = new DataSetMovimientoVigilancia();

                    var data = GetObjetoSanitario(i.Rup);


                    richTextBox1.Invoke(new MethodInvoker(delegate
                    {
                        richTextBox1.AppendText("\r\n" + "Obtener VIGILANCIA para el RUP: " + i.Rup + "");
                        richTextBox1.ScrollToCaret();
                    }));
                    //string dataSSA = AplicarNivelVigilancia(i.Rup, i.Peso, especie, enfermedad);
                    //int cantidadAnalisis = 0;
                    //int cantidadPositivos = 0;
                    //if (dataSSA.Length > 0)
                    //{
                    //    var dataarr = dataSSA.Split('|');
                    //    // tienePositivo.ToString() + "|" + cantidadAnalisis.ToString();
                    //    cantidadPositivos = Int32.Parse(dataarr[0]);
                    //    cantidadAnalisis = Int32.Parse(dataarr[1]);
                    //}
                    richTextBox1.Invoke(new MethodInvoker(delegate
                    {
                        richTextBox1.AppendText("\r\n" + "TOTAL DE VIGILANCIA para el RUP: " + i.Rup + " ES: " + data.TotalAnalisisPruebas + "");
                        richTextBox1.ScrollToCaret();
                    }));


                    double latitude = 0;
                    double longitude = 0;

                    obj.IdRup = i.IdRup;
                    obj.Rup = i.Rup;
                    obj.OficinaId = i.OficinaId;
                    obj.CoordenadaX = i.CoordenadaX;
                    obj.CoordenadaY = i.CoordenadaY;
                    obj.Huso = i.Huso;
                    if (i.CoordenadaX.Length > 0 && i.CoordenadaY.Length > 0 && i.Huso.Length > 0)
                    {
                        double x = Convert.ToDouble(i.CoordenadaX);
                        double y = Convert.ToDouble(i.CoordenadaY);
                        ToLatLon(x, y, obj.Huso, out latitude, out longitude);
                    }
                    obj.Latitud = latitude.ToString();
                    obj.Longitud = longitude.ToString();

                    obj.PesoContagio = i.PesoContagio;
                    obj.PesoMasaEntrada = i.PesoMasaEntrada;
                    obj.PesoMasaSalida = i.PesoMasaSalida;
                    obj.NivelVigilancia = nivelVigilancia(i.Rup).ToString();
                    obj.ClasificacionSanitaria = GetClasificacion(i.Rup);
                    obj.ProtocoloLab = data.ProtocoloLab;
                    obj.CantidadNegativosLab = data.CantidadNegativosLab;
                    obj.CantidadPositivosLab = data.CantidadPositivosLab;
                    obj.ProtocoloPc = data.ProtocoloPc;
                    obj.CantidadNegativosPc = data.CantidadNegativosPc;
                    obj.CantidadPositivosPc = data.CantidadPositivosPc;

                    if (obj.ProtocoloLab > obj.ProtocoloPc)
                    {
                        if (data.CantidadPositivosLab > 0)
                            obj.AlMenosUnPositivoLab = 1;
                        else
                            obj.AlMenosUnPositivoLab = 0;

                        if (data.CantidadNegativosLab > 0 && data.CantidadPositivosLab < 1)
                            obj.TodosNegativosLab = 1;
                        else
                            obj.TodosNegativosLab = 0;
                    }
                    if (obj.ProtocoloPc > obj.ProtocoloLab)
                    {
                        if (data.CantidadPositivosPc > 0)
                            obj.AlMenosUnPositivoPc = 1;
                        else
                            obj.AlMenosUnPositivoPc = 0;

                        if (data.CantidadNegativosPc > 0 && data.CantidadPositivosPc < 1)
                            obj.TodosNegativosPc = 1;
                        else
                            obj.TodosNegativosPc = 0;
                    }




                    var dataIs = AplicarRiesgoMovimiento(obj);
                    obj.RiesgoMovimiento = dataIs.Valor;
                    obj.TextoRiesgoMovimiento = dataIs.Texto;




                    listaFinal.Add(obj);
                    richTextBox1.Invoke(new MethodInvoker(delegate
                    {
                        richTextBox1.AppendText("\r\n" + "NUEVA LINEA CSV");
                        richTextBox1.ScrollToCaret();
                    }));

                    if (flag == 0)
                    {
                        flag++;
                        csv.AppendLine("Id;RUP;Oficina;CoordenadaX;CoordenadaY;Huso;Latitud;Longitud;" +
                            "Peso Nivel Contagio;" +
                            "Peso Masa Entrada;Peso Masa Salida;Nivel Vigilancia;ProtocoloLab;NegativosLab;" +
                            "PositivosLab;" +
                            "ProtocoloPc; NegativosPc; PositivosPc;TodosNegativosLab;AlMenosUnPositivoLab;" +
                            "TodosNegativosPc;AlMenosUnPositivoPc;" +
                            "ClasificacionSanitaria;" +
                            " RiesgoMovimiento; TextoRiesgo");


                    }
                    //  csv.AppendLine("RUP,Peso Nivel Contagio,Peso Masa Entrada,Peso Masa Salida,Nivel Vigilancia,IndicadorResultado");

                    if (obj.RiesgoMovimiento != 99)
                    {
                        //SOLO GENERAR DATA SET CON VIGILANCIA, EL RESTO SE ASUME COMO "SIN CONTROL"
                        //Y SIN ERROR
                        string newLine = "";
                        if (sw == 1)
                        {
                            newLine = "" + obj.IdRup +
                                                       ";" + obj.Rup +
                                                       ";" + obj.OficinaId +
                                                       ";" + obj.CoordenadaX +
                                                       ";" + obj.CoordenadaY +
                                                       ";" + obj.Huso +
                                                       ";" + obj.Latitud +
                                                       ";" + obj.Longitud +
                                                       ";" + obj.PesoContagio +
                                                       ";" + obj.PesoMasaEntrada +
                                                       ";" + obj.PesoMasaSalida +
                                                       ";" + obj.NivelVigilancia +
                                                       ";" + obj.ProtocoloLab +
                                                       ";" + obj.CantidadNegativosLab +
                                                       ";" + obj.CantidadPositivosLab +
                                                       ";" + obj.ProtocoloPc +
                                                       ";" + obj.CantidadNegativosPc +
                                                       ";" + obj.CantidadPositivosPc +
                                                       ";" + obj.TodosNegativosLab +
                                                       ";" + obj.AlMenosUnPositivoLab +
                                                       ";" + obj.TodosNegativosPc +
                                                       ";" + obj.AlMenosUnPositivoPc +
                                                       ";" + obj.ClasificacionSanitaria +
                                                       ";" + obj.RiesgoMovimiento +
                                                       ";" + obj.TextoRiesgoMovimiento + "";
                            csv.AppendLine(newLine);
                            File.WriteAllText(@"C:\Emprendimientos\IA_Python\PrediccionEnfermedades\DataSet\DataSet_Generador\" + fileName + ".csv", csv.ToString());
                        }
                        else
                        {
                            newLine = "" + obj.IdRup +
                                                       ";" + obj.Rup +
                                                       ";" + obj.OficinaId +
                                                       ";" + obj.CoordenadaX +
                                                       ";" + obj.CoordenadaY +
                                                       ";" + obj.Huso +
                                                       ";" + obj.Latitud +
                                                       ";" + obj.Longitud +
                                                       ";" + obj.PesoContagio +
                                                       ";" + obj.PesoMasaEntrada +
                                                       ";" + obj.PesoMasaSalida +
                                                       ";" + obj.NivelVigilancia +
                                                       ";" + obj.ProtocoloLab +
                                                       ";" + obj.CantidadNegativosLab +
                                                       ";" + obj.CantidadPositivosLab +
                                                       ";" + obj.ProtocoloPc +
                                                       ";" + obj.CantidadNegativosPc +
                                                       ";" + obj.CantidadPositivosPc +
                                                       ";" + obj.TodosNegativosLab +
                                                       ";" + obj.AlMenosUnPositivoLab +
                                                       ";" + obj.TodosNegativosPc +
                                                       ";" + obj.AlMenosUnPositivoPc +
                                                       ";" + obj.ClasificacionSanitaria +
                                                       ";;";
                            csv.AppendLine(newLine);
                            File.WriteAllText(@"C:\Emprendimientos\IA_Python\PrediccionEnfermedades\DataSet\DataSet_Generador\" + fileName + ".csv", csv.ToString());
                        }

                    }
                    indice++;
                    var porcentaje = (indice * 100) / listaNivelContagio.Count;
                    progressConsolidacion.Invoke(new MethodInvoker(delegate
                    {
                        progressConsolidacion.Value = porcentaje;
                    }));

                }
            }
            catch (Exception ex)
            {
                var s = ex.Message;
            }
            richTextBox1.Invoke(new MethodInvoker(delegate
            {
                richTextBox1.AppendText("\r\n" + "Se FINALIZA proceso Consolidacion");
                richTextBox1.ScrollToCaret();
            }));




        }

        int nivelVigilancia(string rup)
        {

            var data = listaCantidadProtocolos.FirstOrDefault(f => f.Rup == rup);
            if (data != null)
            {
                //entre 1 y 4 vigilancia baja
                //entre 5 y 10 vigilancia media
                //entre 11 y mas vigilancia alta
                if (data.Cantidad >= 1 && data.Cantidad < 5)
                    return 1;//VIGILANCIA BAJA
                if (data.Cantidad >= 5 && data.Cantidad < 11)
                    return 2;//VIGILANCIA MEDIA
                if (data.Cantidad >= 10)
                    return 3; //VIGILANCIA ALTA

            }
            return 0; //SIN VIGILANCIA
        }

        //string AplicarNivelVigilancia(string rup, int pesoContagio, int enfermedadId, int especie)
        //{


        //    int cantidadAnalisis = 0;
        //    int positivoMayor = 0;


        //    foreach (var d in listaResultados)
        //    {
        //        cantidadAnalisis++;
        //        string contiene = dr["contiene"].ToString();

        //        int positivoMedicion = ResultadoEpimediologico(contiene, pesoContagio);

        //        if (positivoMedicion > 50)
        //        {
        //            if (positivoMedicion > positivoMayor)
        //            {
        //                positivoMayor = positivoMedicion;
        //            }
        //        }

        //    }


        //    return positivoMayor.ToString() + "|" + cantidadAnalisis.ToString();

        //}
        int ResultadoEpimediologico(string pesoPositivo, int nivelContagio)
        {
            if (pesoPositivo == "0,0" || pesoPositivo == "0.0")
            {
                Random r = new Random();
                int rInt = r.Next(10, 50);
                return rInt;
            }

            if ((pesoPositivo == "1,0" || pesoPositivo == "1.0") && nivelContagio >= 10)
            {
                Random r = new Random();
                int rInt = r.Next(300, 350);
                return rInt;
            }
            if ((pesoPositivo == "0,5" || pesoPositivo == "0.5") && nivelContagio >= 10)
            {
                Random r = new Random();
                int rInt = r.Next(260, 290);
                return rInt;
            }
            if ((pesoPositivo == "1,0" || pesoPositivo == "1.0") && (nivelContagio >= 5 && nivelContagio < 10))
            {
                Random r = new Random();
                int rInt = r.Next(200, 250);
                return rInt;
            }
            if ((pesoPositivo == "0,5" || pesoPositivo == "0.5") && (nivelContagio >= 5 && nivelContagio < 10))
            {
                Random r = new Random();
                int rInt = r.Next(160, 190);
                return rInt;
            }
            if ((pesoPositivo == "1,0" || pesoPositivo == "1.0") && (nivelContagio >= 1 && nivelContagio < 5))
            {
                Random r = new Random();
                int rInt = r.Next(100, 150);
                return rInt;
            }
            if ((pesoPositivo == "0,5" || pesoPositivo == "0.5") && (nivelContagio >= 1 && nivelContagio < 5))
            {
                Random r = new Random();
                int rInt = r.Next(60, 90);
                return rInt;
            }


            return 400;
        }
        int EscalaMasaAnimales(int cant)
        {
            if (cant > 1000)
                return 10; //ALTA
            if (cant > 300 && cant < 1000)
                return 7; //MEDIA ALTA
            if (cant > 100 && cant < 300)
                return 3; // MEDIA
            if (cant < 100)
                return 1; // BAJA

            return 0;
        }
        int EscalaNivelVigilancia(int cant)
        {
            if (cant > 1000)
                return 10; //vigilancia alta
            if (cant > 300 && cant < 1000)
                return 7; //vigilancia media

            if (cant < 300 && cant > 0)
                return 1;//vigilancia baja
            return 0; // sin vigilancia
        }
        int AplicarPesoSalida(string regionId, int diasAtras)
        {

            string constr = "User Id=SIPEC;Password=sipec_testsm.2019;Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=192.168.51.249)(PORT=1540)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=testsm)))";

            OracleConnection con = new OracleConnection(constr);

            progressMovOut.Invoke(new MethodInvoker(delegate
            {
                progressMovOut.Maximum = 100;
                progressMovOut.Step = 1;
                progressMovOut.Value = 0;
            }));


            richTextBox1.Invoke(new MethodInvoker(delegate
            {
                richTextBox1.AppendText("\r\n" + "Se inicia proceso para el peso de movimiento Salida");
                richTextBox1.ScrollToCaret();
            }));

            con.Open();
            String sql = "";

            sql += "select F.RUP_ORI, SUM(CANT_ANIMALES) as TOT_CANT_ANIMALES, COUNT(*) AS CANT_FMA ";
            sql += " from( ";
            sql += " select EO.RUP AS RUP_ORI, ";
            sql += "  ( ";
            sql += "   select COUNT(*) ";
            sql += "   from SIPEC.SIP_D_DETALLES_MOV_DIIO DFM ";
            sql += "  where DFM.ID_FOMO = FMA.ID ";
            sql += "  ) as CANT_ANIMALES ";
            sql += " from SIPEC.SIP_D_FORMULARIOS_MOV_DIIO FMA ";
            sql += "    inner join SIPEC.SIP_D_ESTABLECIMIENTOS EO on FMA.ID_ESTA_ORI = EO.ID ";
            sql += " where FMA.ID_REGI_ORI = " + regionId + " ";
            sql += " and FMA.ID_ESFM IN(1,5) ";
            sql += " and FMA.FECHA_FORMULARIO between(sysdate - " + diasAtras + ") and sysdate ";
            sql += " ) F group by F.RUP_ORI ";

            OracleDataAdapter datos = new OracleDataAdapter(sql, con);

            DataSet data = new DataSet();
            datos.Fill(data);
            richTextBox1.Invoke(new MethodInvoker(delegate
            {
                richTextBox1.AppendText("\r\n" + "Datos desde la BD OK");
                richTextBox1.ScrollToCaret();
            }));
            int i;
            for (i = 0; i < data.Tables[0].Rows.Count; i++)
            {

                var totalAnimales = data.Tables[0].Rows[i][1].ToString();

                int totalAnimalesMov = Int32.Parse(totalAnimales);
                string rupS = data.Tables[0].Rows[i][0].ToString();
                string cantFma = data.Tables[0].Rows[i][2].ToString();
                int cantidadFma = Int32.Parse(cantFma);
                PredioCantidadAMov o = new PredioCantidadAMov();

                o.Cantidad = totalAnimalesMov;
                o.CantidadFma = cantidadFma;
                o.Rup = rupS;

                listaCantidadMovSalida.Add(o);
                var porcentaje = (i * 100) / data.Tables[0].Rows.Count;
                progressMovOut.Invoke(new MethodInvoker(delegate
                {
                    progressMovOut.Value = porcentaje;
                }));
            }

            con.Close();
            return 0;
        }
        int AplicarPesoEntrada(string regionId, int diasAtras)
        {

            string constr = "User Id=SIPEC;Password=sipec_testsm.2019;Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=192.168.51.249)(PORT=1540)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=testsm)))";

            //string constr = "User Id=SIPEC;Password=sipec_testsm.2019;Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=192.168.51.249)(PORT=1540)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=testsm)))";

            OracleConnection con = new OracleConnection(constr);
            con.Open();
            String sql = "";
            progressMovIn.Invoke(new MethodInvoker(delegate
            {
                progressMovIn.Maximum = 100;
                progressMovIn.Step = 1;
                progressMovIn.Value = 0;
            }));


            richTextBox1.Invoke(new MethodInvoker(delegate
            {
                richTextBox1.AppendText("\r\n" + "Se inicia proceso para el peso de movimiento Entrada");
                richTextBox1.ScrollToCaret();
            }));

            sql += "select F.RUP_DES, SUM(CANT_ANIMALES) as TOT_CANT_ANIMALES, COUNT(*) AS CANT_FMA ";
            sql += " from( ";
            sql += " select EO.RUP AS RUP_DES, ";
            sql += "  ( ";
            sql += "   select COUNT(*) ";
            sql += "   from SIPEC.SIP_D_DETALLES_MOV_DIIO DFM ";
            sql += "  where DFM.ID_FOMO = FMA.ID ";
            sql += "  ) as CANT_ANIMALES ";
            sql += " from SIPEC.SIP_D_FORMULARIOS_MOV_DIIO FMA ";
            sql += "    inner join SIPEC.SIP_D_ESTABLECIMIENTOS EO on FMA.ID_ESTA_DES = EO.ID ";
            sql += " where FMA.ID_REGI_DES = " + regionId + " ";
            sql += " and FMA.ID_ESFM IN(1,5) ";
            sql += " and FMA.FECHA_FORMULARIO between(sysdate - " + diasAtras + ") and sysdate ";
            sql += " ) F group by F.RUP_DES ";

            OracleDataAdapter datos = new OracleDataAdapter(sql, con);

            DataSet data = new DataSet();
            datos.Fill(data);
            richTextBox1.Invoke(new MethodInvoker(delegate
            {
                richTextBox1.AppendText("\r\n" + "Datos desde la BD OK");
                richTextBox1.ScrollToCaret();
            }));
            int i;
            for (i = 0; i < data.Tables[0].Rows.Count; i++)
            {

                var totalAnimales = data.Tables[0].Rows[i][1].ToString();

                int totalAnimalesMov = Int32.Parse(totalAnimales);
                string rupS = data.Tables[0].Rows[i][0].ToString();
                string cantFma = data.Tables[0].Rows[i][2].ToString();
                int cantidadFma = Int32.Parse(cantFma);
                PredioCantidadAMov o = new PredioCantidadAMov();

                o.Cantidad = totalAnimalesMov;
                o.CantidadFma = cantidadFma;
                o.Rup = rupS;

                listaCantidadMovEntrada.Add(o);

                var porcentaje = (i * 100) / data.Tables[0].Rows.Count;
                progressMovIn.Invoke(new MethodInvoker(delegate
                {
                    progressMovIn.Value = porcentaje;
                }));

            }

            con.Close();
            return 0;
        }


        UltimoResultado GetUltimoResultadoCon(DataSetMovimientoVigilancia obj)
        {
            UltimoResultado o = new UltimoResultado();

            if (obj.ProtocoloLab > obj.ProtocoloPc)
            {
                //ULTIMO PROTOCOLO ES DEL LAB
                if (obj.CantidadPositivosLab > 0)
                {
                    //ES POSITIVO DEL LAB
                    o.EsPC = false;
                    o.ContienePositivo = true;
                    return o;
                }
                else
                {
                    o.EsPC = false;
                    o.ContienePositivo = false;
                    return o;
                }
            }
            if (obj.ProtocoloLab < obj.ProtocoloPc)
            {
                //ULTIMO PROTOCOLO ES PC
                if (obj.CantidadPositivosPc > 0)
                {
                    //ES POSITIVO DEL PC
                    o.EsPC = true;
                    o.ContienePositivo = true;
                    return o;
                }
                else
                {
                    o.EsPC = true;
                    o.ContienePositivo = false;
                    return o;
                }
            }

            return o;
        }
        EscalaMovimientoSanidad AplicarRiesgoMovimiento(DataSetMovimientoVigilancia obj)
        {
            EscalaMovimientoSanidad o = new EscalaMovimientoSanidad();
            if (obj.PesoMasaEntrada == "0")
                obj.PesoMasaEntrada = "1";
            if (obj.PesoMasaSalida == "0")
                obj.PesoMasaSalida = "1";
            string llave = obj.PesoContagio + "-" + obj.PesoMasaEntrada + "-" + obj.PesoMasaSalida;
            var ssaR = GetUltimoResultadoCon(obj);
            if (obj.NivelVigilancia == "0")
            {
                o.Texto = "Sin Control";
                o.Valor = 10;
                return o;
            }
            if (ssaR != null && ssaR.EsPC != null && ssaR.ContienePositivo != null)
            {
                #region nivelVigilancia BAJA
                if (obj.NivelVigilancia == "1")
                {
                    #region 10-1-1
                    if (llave == "10-1-1")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Alto";
                            o.Valor = 7;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Alto";
                            o.Valor = 7;
                            return o;
                        }
                    }
                    #endregion
                    #region 10-1-3
                    if (llave == "10-1-3")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Alto";
                            o.Valor = 7;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Alto";
                            o.Valor = 7;
                            return o;
                        }
                    }
                    #endregion
                    #region 10-1-7
                    if (llave == "10-1-7")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                    }
                    #endregion
                    #region 10-1-10
                    if (llave == "10-1-10")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                    }
                    #endregion
                    #region 10-3-1
                    if (llave == "10-3-1")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Alto";
                            o.Valor = 7;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Alto";
                            o.Valor = 7;
                            return o;
                        }
                    }
                    #endregion
                    #region 10-7-1
                    if (llave == "10-7-1")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Alto";
                            o.Valor = 7;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Alto";
                            o.Valor = 7;
                            return o;
                        }
                    }
                    #endregion
                    #region 10-10-1
                    if (llave == "10-10-1")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                    }
                    #endregion
                    #region 10-10-10
                    if (llave == "10-10-10")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                    }
                    #endregion
                    #region 10-3-3
                    if (llave == "10-3-3")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Alto";
                            o.Valor = 7;
                            return o;
                        }
                    }
                    #endregion
                    #region 10-7-7
                    if (llave == "10-7-7")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                    }
                    #endregion

                    #region 10-10-7
                    if (llave == "10-10-7")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                    }
                    #endregion
                    #region 10-3-7
                    if (llave == "10-3-7")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                    }
                    #endregion
                    #region 10-7-10
                    if (llave == "10-7-10")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                    }
                    #endregion
                    #region 10-3-10
                    if (llave == "10-3-10")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                    }
                    #endregion
                    #region 10-10-3
                    if (llave == "10-10-3")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Alto";
                            o.Valor = 7;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Alto";
                            o.Valor = 7;
                            return o;
                        }
                    }
                    #endregion
                    #region 10-7-3
                    if (llave == "10-7-3")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Alto";
                            o.Valor = 7;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Alto";
                            o.Valor = 7;
                            return o;
                        }
                    }
                    #endregion

                    #region 5-1-1
                    if (llave == "5-1-1")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Medio Alto";
                            o.Valor = 6;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio Alto";
                            o.Valor = 6;
                            return o;
                        }
                    }
                    #endregion
                    #region 5-1-3
                    if (llave == "5-1-3")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Alto";
                            o.Valor = 7;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Alto";
                            o.Valor = 7;
                            return o;
                        }
                    }
                    #endregion
                    #region 5-1-7
                    if (llave == "5-1-7")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Alto";
                            o.Valor = 7;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Alto";
                            o.Valor = 7;
                            return o;
                        }
                    }
                    #endregion
                    #region 5-1-10
                    if (llave == "5-1-10")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                    }
                    #endregion
                    #region 5-3-1
                    if (llave == "5-3-1")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Medio Alto";
                            o.Valor = 6;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio Alto";
                            o.Valor = 6;
                            return o;
                        }
                    }
                    #endregion
                    #region 5-7-1
                    if (llave == "5-7-1")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Medio Alto";
                            o.Valor = 6;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio Alto";
                            o.Valor = 6;
                            return o;
                        }
                    }
                    #endregion
                    #region 5-10-1
                    if (llave == "5-10-1")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                    }
                    #endregion
                    #region 5-10-10
                    if (llave == "5-10-10")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                    }
                    #endregion
                    #region 5-3-3
                    if (llave == "5-3-3")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Medio Alto";
                            o.Valor = 6;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio Alto";
                            o.Valor = 6;
                            return o;
                        }
                    }
                    #endregion
                    #region 5-7-7
                    if (llave == "5-7-7")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                    }
                    #endregion

                    #region 5-10-7
                    if (llave == "5-10-7")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Alto";
                            o.Valor = 7;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Alto";
                            o.Valor = 7;
                            return o;
                        }
                    }
                    #endregion
                    #region 5-3-7
                    if (llave == "5-3-7")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Alto";
                            o.Valor = 7;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Alto";
                            o.Valor = 7;
                            return o;
                        }
                    }
                    #endregion
                    #region 5-7-10
                    if (llave == "5-7-10")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                    }
                    #endregion
                    #region 5-3-10
                    if (llave == "5-3-10")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                    }
                    #endregion
                    #region 5-10-3
                    if (llave == "5-10-3")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Alto";
                            o.Valor = 7;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Alto";
                            o.Valor = 7;
                            return o;
                        }
                    }
                    #endregion
                    #region 5-7-3
                    if (llave == "5-7-3")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Alto";
                            o.Valor = 7;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Alto";
                            o.Valor = 7;
                            return o;
                        }
                    }
                    #endregion


                    #region 1-1-1
                    if (llave == "1-1-1")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo";
                            o.Valor = 5;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo";
                            o.Valor = 5;
                            return o;
                        }
                    }
                    #endregion
                    #region 1-1-3
                    if (llave == "1-1-3")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Medio Alto";
                            o.Valor = 6;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio Alto";
                            o.Valor = 6;
                            return o;
                        }
                    }
                    #endregion
                    #region 1-1-7
                    if (llave == "1-1-7")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                    }
                    #endregion
                    #region 1-1-10
                    if (llave == "1-1-10")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                    }
                    #endregion
                    #region 1-3-1
                    if (llave == "1-3-1")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Medio Alto";
                            o.Valor = 6;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio Alto";
                            o.Valor = 6;
                            return o;
                        }
                    }
                    #endregion
                    #region 1-7-1
                    if (llave == "1-7-1")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Medio Alto";
                            o.Valor = 6;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio Alto";
                            o.Valor = 6;
                            return o;
                        }
                    }
                    #endregion
                    #region 1-10-1
                    if (llave == "1-10-1")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                    }
                    #endregion
                    #region 1-10-10
                    if (llave == "1-10-10")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                    }
                    #endregion
                    #region 1-3-3
                    if (llave == "1-3-3")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Medio Alto";
                            o.Valor = 6;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio Alto";
                            o.Valor = 6;
                            return o;
                        }
                    }
                    #endregion
                    #region 1-7-7
                    if (llave == "1-7-7")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                    }
                    #endregion

                    #region 1-10-7
                    if (llave == "1-10-7")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                    }
                    #endregion
                    #region 1-3-7
                    if (llave == "1-3-7")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                    }
                    #endregion
                    #region 1-10-3
                    if (llave == "1-10-3")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Medio Alto";
                            o.Valor = 6;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio Alto";
                            o.Valor = 6;
                            return o;
                        }
                    }
                    #endregion
                    #region 1-7-3
                    if (llave == "1-7-3")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Medio Alto";
                            o.Valor = 6;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio Alto";
                            o.Valor = 6;
                            return o;
                        }
                    }
                    #endregion
                    #region 1-7-10
                    if (llave == "1-7-10")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                    }
                    #endregion
                    #region 1-3-10
                    if (llave == "1-3-10")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                    }
                    #endregion
                }
                #endregion

                #region nivelVigilancia MEDIA
                if (obj.NivelVigilancia == "2")
                {
                    #region 10-1-1
                    if (llave == "10-1-1")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Alto";
                            o.Valor = 7;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio";
                            o.Valor = 3;
                            return o;
                        }
                    }
                    #endregion
                    #region 10-1-3
                    if (llave == "10-1-3")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Alto";
                            o.Valor = 7;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio";
                            o.Valor = 3;
                            return o;
                        }
                    }
                    #endregion
                    #region 10-1-7
                    if (llave == "10-1-7")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio Peligroso";
                            o.Valor = 4;
                            return o;
                        }
                    }
                    #endregion
                    #region 10-1-10
                    if (llave == "10-1-10")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio Peligroso";
                            o.Valor = 4;
                            return o;
                        }
                    }
                    #endregion
                    #region 10-3-1
                    if (llave == "10-3-1")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Alto";
                            o.Valor = 7;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio";
                            o.Valor = 3;
                            return o;
                        }
                    }
                    #endregion
                    #region 10-7-1
                    if (llave == "10-7-1")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Alto";
                            o.Valor = 7;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio";
                            o.Valor = 3;
                            return o;
                        }
                    }
                    #endregion
                    #region 10-10-1
                    if (llave == "10-10-1")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio";
                            o.Valor = 3;
                            return o;
                        }
                    }
                    #endregion
                    #region 10-10-10
                    if (llave == "10-10-10")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio Peligroso";
                            o.Valor = 4;
                            return o;
                        }
                    }
                    #endregion
                    #region 10-3-3
                    if (llave == "10-3-3")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio";
                            o.Valor = 3;
                            return o;
                        }
                    }
                    #endregion
                    #region 10-7-7
                    if (llave == "10-7-7")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio Peligroso";
                            o.Valor = 4;
                            return o;
                        }
                    }
                    #endregion

                    #region 10-10-7
                    if (llave == "10-10-7")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio Peligroso";
                            o.Valor = 4;
                            return o;
                        }
                    }
                    #endregion
                    #region 10-3-7
                    if (llave == "10-3-7")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio Peligroso";
                            o.Valor = 4;
                            return o;
                        }
                    }
                    #endregion
                    #region 10-7-10
                    if (llave == "10-7-10")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio Peligroso";
                            o.Valor = 4;
                            return o;
                        }
                    }
                    #endregion
                    #region 10-3-10
                    if (llave == "10-3-10")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio Peligroso";
                            o.Valor = 4;
                            return o;
                        }
                    }
                    #endregion
                    #region 10-10-3
                    if (llave == "10-10-3")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Alto";
                            o.Valor = 7;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio";
                            o.Valor = 3;
                            return o;
                        }
                    }
                    #endregion
                    #region 10-7-3
                    if (llave == "10-7-3")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Alto";
                            o.Valor = 7;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio";
                            o.Valor = 3;
                            return o;
                        }
                    }
                    #endregion

                    #region 5-1-1
                    if (llave == "5-1-1")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Medio Alto";
                            o.Valor = 6;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio";
                            o.Valor = 3;
                            return o;
                        }
                    }
                    #endregion
                    #region 5-1-3
                    if (llave == "5-1-3")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Alto";
                            o.Valor = 7;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio";
                            o.Valor = 3;
                            return o;
                        }
                    }
                    #endregion
                    #region 5-1-7
                    if (llave == "5-1-7")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Alto";
                            o.Valor = 7;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio";
                            o.Valor = 3;
                            return o;
                        }
                    }
                    #endregion
                    #region 5-1-10
                    if (llave == "5-1-10")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio Peligroso";
                            o.Valor = 4;
                            return o;
                        }
                    }
                    #endregion
                    #region 5-3-1
                    if (llave == "5-3-1")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Medio Alto";
                            o.Valor = 6;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio";
                            o.Valor = 3;
                            return o;
                        }
                    }
                    #endregion
                    #region 5-7-1
                    if (llave == "5-7-1")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Medio Alto";
                            o.Valor = 6;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio";
                            o.Valor = 3;
                            return o;
                        }
                    }
                    #endregion
                    #region 5-10-1
                    if (llave == "5-10-1")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio Peligroso";
                            o.Valor = 4;
                            return o;
                        }
                    }
                    #endregion
                    #region 5-10-10
                    if (llave == "5-10-10")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio Peligroso";
                            o.Valor = 4;
                            return o;
                        }
                    }
                    #endregion
                    #region 5-3-3
                    if (llave == "5-3-3")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Medio Alto";
                            o.Valor = 6;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio";
                            o.Valor = 3;
                            return o;
                        }
                    }
                    #endregion
                    #region 5-7-7
                    if (llave == "5-7-7")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio Peligroso";
                            o.Valor = 4;
                            return o;
                        }
                    }
                    #endregion

                    #region 5-10-7
                    if (llave == "5-10-7")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio Peligroso";
                            o.Valor = 4;
                            return o;
                        }
                    }
                    #endregion
                    #region 5-3-7
                    if (llave == "5-3-7")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio Peligroso";
                            o.Valor = 4;
                            return o;
                        }
                    }
                    #endregion
                    #region 5-7-10
                    if (llave == "5-7-10")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio Peligroso";
                            o.Valor = 4;
                            return o;
                        }
                    }
                    #endregion
                    #region 5-3-10
                    if (llave == "5-3-10")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio Peligroso";
                            o.Valor = 4;
                            return o;
                        }
                    }
                    #endregion
                    #region 5-10-3
                    if (llave == "5-10-3")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Medio Alto";
                            o.Valor = 6;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio";
                            o.Valor = 3;
                            return o;
                        }
                    }
                    #endregion
                    #region 5-7-3
                    if (llave == "5-7-3")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Medio Alto";
                            o.Valor = 6;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio";
                            o.Valor = 3;
                            return o;
                        }
                    }
                    #endregion

                    #region 1-1-1
                    if (llave == "1-1-1")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo";
                            o.Valor = 5;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio";
                            o.Valor = 3;
                            return o;
                        }
                    }
                    #endregion
                    #region 1-1-3
                    if (llave == "1-1-3")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Medio Alto";
                            o.Valor = 6;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio";
                            o.Valor = 3;
                            return o;
                        }
                    }
                    #endregion
                    #region 1-1-7
                    if (llave == "1-1-7")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio Peligroso";
                            o.Valor = 4;
                            return o;
                        }
                    }
                    #endregion
                    #region 1-1-10
                    if (llave == "1-1-10")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio Peligroso";
                            o.Valor = 4;
                            return o;
                        }
                    }
                    #endregion
                    #region 1-3-1
                    if (llave == "1-3-1")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Medio Alto";
                            o.Valor = 6;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio";
                            o.Valor = 3;
                            return o;
                        }
                    }
                    #endregion
                    #region 1-7-1
                    if (llave == "1-7-1")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Medio Alto";
                            o.Valor = 6;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio";
                            o.Valor = 3;
                            return o;
                        }
                    }
                    #endregion
                    #region 1-10-1
                    if (llave == "1-10-1")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio Peligroso";
                            o.Valor = 4;
                            return o;
                        }
                    }
                    #endregion
                    #region 1-10-10
                    if (llave == "1-10-10")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio Peligroso";
                            o.Valor = 4;
                            return o;
                        }
                    }
                    #endregion
                    #region 1-3-3
                    if (llave == "1-3-3")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Medio Alto";
                            o.Valor = 6;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio";
                            o.Valor = 3;
                            return o;
                        }
                    }
                    #endregion
                    #region 1-7-7
                    if (llave == "1-7-7")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio Peligroso";
                            o.Valor = 4;
                            return o;
                        }
                    }
                    #endregion

                    #region 1-10-7
                    if (llave == "1-10-7")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio Peligroso";
                            o.Valor = 4;
                            return o;
                        }
                    }
                    #endregion
                    #region 1-3-7
                    if (llave == "1-3-7")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio Peligroso";
                            o.Valor = 4;
                            return o;
                        }
                    }
                    #endregion
                    #region 1-10-3
                    if (llave == "1-10-3")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Medio Alto";
                            o.Valor = 6;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio";
                            o.Valor = 3;
                            return o;
                        }
                    }
                    #endregion
                    #region 1-7-3
                    if (llave == "1-7-3")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Medio Alto";
                            o.Valor = 6;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio";
                            o.Valor = 3;
                            return o;
                        }
                    }
                    #endregion
                    #region 1-7-10
                    if (llave == "1-7-10")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio Peligroso";
                            o.Valor = 4;
                            return o;
                        }
                    }
                    #endregion
                    #region 1-3-10
                    if (llave == "1-3-10")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Medio Peligroso";
                            o.Valor = 4;
                            return o;
                        }
                    }
                    #endregion



                }
                #endregion

                #region nivelVigilancia ALTA
                if (obj.NivelVigilancia == "3")
                {
                    #region 10-1-1
                    if (llave == "10-1-1")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Alto";
                            o.Valor = 7;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Bajo Control";
                            o.Valor = 1;
                            return o;
                        }
                    }
                    #endregion
                    #region 10-1-3
                    if (llave == "10-1-3")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Alto";
                            o.Valor = 7;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Bajo Control";
                            o.Valor = 1;
                            return o;
                        }
                    }
                    #endregion
                    #region 10-1-7
                    if (llave == "10-1-7")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Bajo Control";
                            o.Valor = 1;
                            return o;
                        }
                    }
                    #endregion
                    #region 10-1-10
                    if (llave == "10-1-10")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Peligro Bajo Control";
                            o.Valor = 2;
                            return o;
                        }
                    }
                    #endregion
                    #region 10-3-1
                    if (llave == "10-3-1")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Alto";
                            o.Valor = 7;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Bajo Control";
                            o.Valor = 1;
                            return o;
                        }
                    }
                    #endregion
                    #region 10-7-1
                    if (llave == "10-7-1")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Alto";
                            o.Valor = 7;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Bajo Control";
                            o.Valor = 1;
                            return o;
                        }
                    }
                    #endregion
                    #region 10-10-1
                    if (llave == "10-10-1")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Bajo Control";
                            o.Valor = 1;
                            return o;
                        }
                    }
                    #endregion
                    #region 10-10-10
                    if (llave == "10-10-10")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Peligro Bajo Control";
                            o.Valor = 2;
                            return o;
                        }
                    }
                    #endregion
                    #region 10-3-3
                    if (llave == "10-3-3")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Bajo Control";
                            o.Valor = 1;
                            return o;
                        }
                    }
                    #endregion
                    #region 10-7-7
                    if (llave == "10-7-7")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Peligro Bajo Control";
                            o.Valor = 2;
                            return o;
                        }
                    }
                    #endregion

                    #region 10-10-7
                    if (llave == "10-10-7")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Peligro Bajo Control";
                            o.Valor = 2;
                            return o;
                        }
                    }
                    #endregion
                    #region 10-3-7
                    if (llave == "10-3-7")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Peligro Bajo Control";
                            o.Valor = 2;
                            return o;
                        }
                    }
                    #endregion
                    #region 10-7-10
                    if (llave == "10-7-10")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Peligro Bajo Control";
                            o.Valor = 2;
                            return o;
                        }
                    }
                    #endregion
                    #region 10-3-10
                    if (llave == "10-3-10")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Peligro Bajo Control";
                            o.Valor = 2;
                            return o;
                        }
                    }
                    #endregion
                    #region 10-10-3
                    if (llave == "10-10-3")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Bajo Control";
                            o.Valor = 1;
                            return o;
                        }
                    }
                    #endregion
                    #region 10-7-3
                    if (llave == "10-7-3")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Bajo Control";
                            o.Valor = 1;
                            return o;
                        }
                    }
                    #endregion

                    #region 5-1-1
                    if (llave == "5-1-1")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Medio Alto";
                            o.Valor = 6;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Bajo Control";
                            o.Valor = 1;
                            return o;
                        }
                    }
                    #endregion
                    #region 5-1-3
                    if (llave == "5-1-3")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Alto";
                            o.Valor = 7;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Bajo Control";
                            o.Valor = 1;
                            return o;
                        }
                    }
                    #endregion
                    #region 5-1-7
                    if (llave == "5-1-7")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Alto";
                            o.Valor = 7;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Peligro Bajo Control";
                            o.Valor = 2;
                            return o;
                        }
                    }
                    #endregion
                    #region 5-1-10
                    if (llave == "5-1-10")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Peligro Bajo Control";
                            o.Valor = 2;
                            return o;
                        }
                    }
                    #endregion
                    #region 5-3-1
                    if (llave == "5-3-1")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Medio Alto";
                            o.Valor = 6;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Bajo Control";
                            o.Valor = 1;
                            return o;
                        }
                    }
                    #endregion
                    #region 5-7-1
                    if (llave == "5-7-1")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Medio Alto";
                            o.Valor = 6;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Bajo Control";
                            o.Valor = 1;
                            return o;
                        }
                    }
                    #endregion
                    #region 5-10-1
                    if (llave == "5-10-1")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Bajo Control";
                            o.Valor = 1;
                            return o;
                        }
                    }
                    #endregion
                    #region 5-10-10
                    if (llave == "5-10-10")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Peligro Bajo Control";
                            o.Valor = 2;
                            return o;
                        }
                    }
                    #endregion
                    #region 5-3-3
                    if (llave == "5-3-3")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Medio Alto";
                            o.Valor = 6;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Bajo Control";
                            o.Valor = 1;
                            return o;
                        }
                    }
                    #endregion
                    #region 5-7-7
                    if (llave == "5-7-7")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Peligro Bajo Control";
                            o.Valor = 2;
                            return o;
                        }
                    }
                    #endregion

                    #region 5-10-7
                    if (llave == "5-10-7")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Peligro Bajo Control";
                            o.Valor = 2;
                            return o;
                        }
                    }
                    #endregion
                    #region 5-3-7
                    if (llave == "5-3-7")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Peligro Bajo Control";
                            o.Valor = 2;
                            return o;
                        }
                    }
                    #endregion
                    #region 5-7-10
                    if (llave == "5-7-10")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Peligro Bajo Control";
                            o.Valor = 2;
                            return o;
                        }
                    }
                    #endregion
                    #region 5-3-10
                    if (llave == "5-3-10")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Peligro Bajo Control";
                            o.Valor = 2;
                            return o;
                        }
                    }
                    #endregion
                    #region 5-10-3
                    if (llave == "5-10-3")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Medio Alto";
                            o.Valor = 6;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Bajo Control";
                            o.Valor = 1;
                            return o;
                        }
                    }
                    #endregion
                    #region 5-7-3
                    if (llave == "5-7-3")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Medio Alto";
                            o.Valor = 6;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Bajo Control";
                            o.Valor = 1;
                            return o;
                        }
                    }
                    #endregion

                    #region 1-1-1
                    if (llave == "1-1-1")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo";
                            o.Valor = 5;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Bajo Control";
                            o.Valor = 1;
                            return o;
                        }
                    }
                    #endregion
                    #region 1-1-3
                    if (llave == "1-1-3")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Medio Alto";
                            o.Valor = 6;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Bajo Control";
                            o.Valor = 1;
                            return o;
                        }
                    }
                    #endregion
                    #region 1-1-7
                    if (llave == "1-1-7")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Peligro Bajo Control";
                            o.Valor = 2;
                            return o;
                        }
                    }
                    #endregion
                    #region 1-1-10
                    if (llave == "1-1-10")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Peligro Bajo Control";
                            o.Valor = 2;
                            return o;
                        }
                    }
                    #endregion
                    #region 1-3-1
                    if (llave == "1-3-1")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Medio Alto";
                            o.Valor = 6;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Bajo Control";
                            o.Valor = 1;
                            return o;
                        }
                    }
                    #endregion
                    #region 1-7-1
                    if (llave == "1-7-1")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Medio Alto";
                            o.Valor = 6;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Bajo Control";
                            o.Valor = 1;
                            return o;
                        }
                    }
                    #endregion
                    #region 1-10-1
                    if (llave == "1-10-1")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Bajo Control";
                            o.Valor = 1;
                            return o;
                        }
                    }
                    #endregion
                    #region 1-10-10
                    if (llave == "1-10-10")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Peligro Bajo Control";
                            o.Valor = 2;
                            return o;
                        }
                    }
                    #endregion
                    #region 1-3-3
                    if (llave == "1-3-3")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Medio Alto";
                            o.Valor = 6;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Bajo Control";
                            o.Valor = 1;
                            return o;
                        }
                    }
                    #endregion
                    #region 1-7-7
                    if (llave == "1-7-7")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Peligro Bajo Control";
                            o.Valor = 2;
                            return o;
                        }
                    }
                    #endregion

                    #region 1-10-7
                    if (llave == "1-10-7")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Peligro Bajo Control";
                            o.Valor = 2;
                            return o;
                        }
                    }
                    #endregion
                    #region 1-3-7
                    if (llave == "1-3-7")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Peligro Bajo Control";
                            o.Valor = 2;
                            return o;
                        }
                    }
                    #endregion
                    #region 1-10-3
                    if (llave == "1-10-3")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Medio Alto";
                            o.Valor = 6;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Bajo Control";
                            o.Valor = 1;
                            return o;
                        }
                    }
                    #endregion
                    #region 1-7-3
                    if (llave == "1-7-3")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Medio Alto";
                            o.Valor = 6;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Riesgo Bajo Control";
                            o.Valor = 1;
                            return o;
                        }
                    }
                    #endregion
                    #region 1-7-10
                    if (llave == "1-7-10")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Peligro Bajo Control";
                            o.Valor = 2;
                            return o;
                        }
                    }
                    #endregion
                    #region 1-3-10
                    if (llave == "1-3-10")
                    {
                        if (ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ PC
                            o.Texto = "Riesgo Peligroso";
                            o.Valor = 8;
                            return o;
                        }
                        if (!ssaR.EsPC.Value && ssaR.ContienePositivo.Value)
                        {
                            //+ LAB
                            o.Texto = "Peligroso";
                            o.Valor = 9;
                            return o;
                        }
                        if (!ssaR.ContienePositivo.Value)
                        {
                            //- pc y lab
                            o.Texto = "Peligro Bajo Control";
                            o.Valor = 2;
                            return o;
                        }
                    }
                    #endregion

                }
                #endregion
            }
            else
            {
                o.Texto = "ERROR";
                o.Valor = 99;
                return o;
            }

            return o;
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }

    public class EscalaMovimientoSanidad
    {
        public string Texto { get; set; }
        public int Valor { get; set; }
    }
    public class UltimoResultado
    {
        public bool? EsPC { get; set; }
        public bool? ContienePositivo { get; set; }

    }
    public class PredioNivelContagio
    {
        public int IdRup { get; set; }
        public string Rup { get; set; }
        public int Peso { get; set; }
        public int OficinaId { get; set; }
        public string CoordenadaX { get; set; }
        public string CoordenadaY { get; set; }
        public string Huso { get; set; }

    }

    public class PredioCantidadAMov
    {
        public string Rup { get; set; }
        public int Cantidad { get; set; }
        public int CantidadFma { get; set; }
    }

    public class PredioResultados
    {
        public string Rup { get; set; }
        public string resultado { get; set; }
        public int cantidadAnalisis { get; set; }
    }

    public class ResultadosProtocolo
    {
        public string RupTomaMuestra { get; set; }
        public string RupOrigen { get; set; }
        public int ProtocoloLab { get; set; }
        public int CantidadNegativosLab { get; set; }
        public int CantidadPositivosLab { get; set; }


    }

    public class ResultadoProtocoloPc
    {
        public string RupOrigen { get; set; }
        public int ProtocoloPc { get; set; }
        public int CantidadNegativosPc { get; set; }
        public int CantidadPositivosPc { get; set; }
    }

    public class ClasificacionPredial
    {
        public string Rup { get; set; }
        public int Clasificacion { get; set; }
    }

    public class PredioNivelMasaMovimiento
    {
        public int IdRup { get; set; }
        public string Rup { get; set; }
        public string PesoContagio { get; set; }
        public string PesoMasaSalida { get; set; }
        public string PesoMasaEntrada { get; set; }
        public int OficinaId { get; set; }
        public string CoordenadaX { get; set; }
        public string CoordenadaY { get; set; }
        public string Huso { get; set; }

    }

    public class CantidadProtocolos
    {
        public int Cantidad { get; set; }
        public string Rup { get; set; }
    }
    public class ObjetoSanitario
    {
        public int ProtocoloPc { get; set; }
        public int CantidadNegativosPc { get; set; }
        public int CantidadPositivosPc { get; set; }
        public int ProtocoloLab { get; set; }
        public int CantidadNegativosLab { get; set; }
        public int CantidadPositivosLab { get; set; }
        public int TotalAnalisisPruebas { get; set; }
    }
    public class DataSetMovimientoVigilancia
    {
        public int IdRup { get; set; }
        public string Rup { get; set; }
        public int OficinaId { get; set; }
        public string CoordenadaX { get; set; }
        public string CoordenadaY { get; set; }
        public string Huso { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public string PesoContagio { get; set; }
        public string PesoMasaSalida { get; set; }
        public string PesoMasaEntrada { get; set; }
        public string NivelVigilancia { get; set; }
        public int ProtocoloPc { get; set; }
        public int CantidadNegativosPc { get; set; }
        public int CantidadPositivosPc { get; set; }
        public int ProtocoloLab { get; set; }
        public int CantidadNegativosLab { get; set; }
        public int CantidadPositivosLab { get; set; }
        public int TodosNegativosLab { get; set; }
        public int AlMenosUnPositivoLab { get; set; }
        public int TodosNegativosPc { get; set; }
        public int AlMenosUnPositivoPc { get; set; }
        public string ClasificacionSanitaria { get; set; }
        public string TextoRiesgoMovimiento { get; set; }
        public int RiesgoMovimiento { get; set; }

    }

    public class MatrizEntrenamiento
    {
        public int PesoNivelContagio { get; set; }
        public int PesoMasaEntrada { get; set; }
        public int PesoMasaSalida { get; set; }
        public int Vigilancia { get; set; }
        public int TodosNegativosLab { get; set; }
        public int AlMenosUnPositivoLab { get; set; }
        public int TodosNegativosPc { get; set; }
        public int AlMenosUnPositivoPc { get; set; }
        public int RiesgoMovimiento { get; set; }
    }
}
