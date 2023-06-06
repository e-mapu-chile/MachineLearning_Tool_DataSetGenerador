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


namespace DataSetGenerador
{
   
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }



        private void button1_Click(object sender, EventArgs e)
        {
            BackgroundWorker backgroundWorker = new BackgroundWorker();
            progressBar1.Maximum = 100;
            progressBar1.Step = 1;
            progressBar1.Value = 0;

            progressBar2.Maximum = 100;
            progressBar2.Step = 1;
            progressBar2.Value = 0;

            //  backgroundWorker.RunWorkerAsync();

            // 1 SEPARAMOS LA ESTRUCTURA DEL TEXTO INICIAL
            try
            {
                var input = textBox1.Text.Split(';');
                string region = input[0];
                string especie = input[1];
                string enfermedad = input[2];

                // 2 VAMOS A BUSCAR A SANIDAD LOS RESULTADOS DE LA REGION SEGUN ESPECIE Y ENFERMEDAD
                //    RUPTOMAMUESTRA Y RUPORIGEN SERA NUESTRO FILTRO
                // 3 CADA RESULTADO VENDRA CON SU PESO CORRESPONDIENTE

                ObtenerResultadosSSA(especie, region, enfermedad);




                // 4 SEGUN RUP DE ORIGEN OBTENER NIVEL CONTAGIO

                // 5 APLICAR ESCALA PARA EL VALOR FINAL DEL RESULTADO SEGUN PESOS ANTERIORES

                // 6 DEJAR EN UN ARCHIVO CSV 
            }
            catch (Exception ex)
            {

            }

        }


        void ObtenerResultadosSSA(string especie, string region, string enfermedad)
        {

            File.WriteAllText(@"C:\Emprendimientos\IA_Python\PrediccionEnfermedades\DataSet\DataSet_Generador\dataSetTBB.csv", "");
            File.WriteAllText(@"C:\Emprendimientos\IA_Python\PrediccionEnfermedades\DataSet\DataSet_Generador\dataSetTBB.csv", "fecha,resultado");

            string connectionString = "Server=192.168.1.237;Database=sanidadanimal;User Id=usr.sanidad;Password=usr.sanidad1409";

            string queryString = "";
            queryString += " select  convert(varchar(10),FechaCreacion,120) as FechaCreacion, rupTomaMuestra,   case when rupOrigen is null or RupOrigen = '0' then RupTomaMuestra else RupOrigen end as [rup],  (case when ContieneEnfermedad = 1 ";
            queryString += " then(select top 1 cast(t.pesochar as decimal) ";
            queryString += " from PesosEnfermedadTecnica t  where t.enfermedadId = " + enfermedad + " and t.tecnicaId = TecnicaId) ";
            queryString += " else 0.0  end) as [contiene] ";
            queryString += " from bitacoramuestraAnalisis ";
            queryString += " where especieId = "+ especie+" and regionId = "+ region + " and enfermedadId = "+ enfermedad + " ";
            queryString += " and MuestraCerrada = 1 and FechaCreacion is not null  and FechaCreacion >= (select dateadd(year, -1, getdate()))   order by fecharesultado";

            List<estructuraResultado> lista = new List<estructuraResultado>();

            List<estructuraResultado> listaFiltrada = new List<estructuraResultado>();
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
                            estructuraResultado obj = new estructuraResultado();

                            string contiene = dr["contiene"].ToString();
                            string rup = dr["rup"].ToString();
                            int nivelContagio = AplicarNivelContagio(rup);
                            int positivoMedicion = ResultadoEpimediologico(contiene, nivelContagio);
                            string fecha = dr["FechaCreacion"].ToString();


                            obj.fecha = fecha;
                            obj.resultado = positivoMedicion;

                            lista.Add(obj);





                            indice++;


                            var porcentaje = (indice * 100) / table.Rows.Count;
                            progressBar1.Value = porcentaje;
                        }

                    }

                    var q = from s in lista
                            group s by s.fecha into g
                            select new { fecha = g.Key, MaxUid = g.Max(s => s.resultado) };

                    var newLine1 = string.Format("{0},{1}", "fecha", "resultado");
                    csv.AppendLine(newLine1);

                    var dates = new List<DateTime>();
                    DateTime fechaIn = Convert.ToDateTime(q.ToList()[0].fecha.ToString());

                    var ultimoVal = 0;
                    for (var dt = fechaIn; dt <= DateTime.Now; dt = dt.AddDays(1))
                    {
                        //  string ffe = dt.Year + "-" + dt.Month + "-" + dt.Day;
                        var ienc = q.FirstOrDefault(r => Convert.ToDateTime(r.fecha) == dt);
                        if (ienc != null)
                        {
                            ultimoVal = ienc.MaxUid;
                            var newLine = string.Format("{0},{1}", ienc.fecha, ienc.MaxUid);
                            csv.AppendLine(newLine);
                        }
                        else
                        {
                            string ffe = dt.Year + "-" + dt.Month + "-" + dt.Day;
                            var newLine = string.Format("{0},{1}", ffe, ultimoVal);
                            csv.AppendLine(newLine);
                        }
                        dates.Add(dt);

                    }

                    //foreach (var r in q)
                    //{
                    //    var newLine = string.Format("{0},{1}", r.fecha, r.MaxUid);
                    //    csv.AppendLine(newLine);
                    //}

                    File.WriteAllText(@"C:\Emprendimientos\IA_Python\PrediccionEnfermedades\DataSet\DataSet_Generador\dataSetTBB.csv", csv.ToString());


                }
                catch (Exception ex)
                {
                    var s = ex.Message;
                }
                finally
                {
                    // Always call Close when done reading.
                    // reader.Close();
                }
            }
        }


        void functionThatTakesASecondOrTwoToRun()
        {

        }

        int AplicarNivelContagio(string rup)
        {
            string constr = "User Id=SIPEC;Password=sipec_testsm.2019;Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=192.168.51.249)(PORT=1540)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=testsm)))";

            OracleConnection con = new OracleConnection(constr);
            con.Open();
            String sql = "";


            progressBar2.Value = 0;


            sql += "select  DISTINCT  ";
            sql += " a.rup, a.ID_REGI, tipo.id as idTEstab, tipo.descripcion , ";
            sql += " CE.Id_Rubro, CE.DESC_RUBRO ";
            sql += " FROM ";
            sql += " SIPEC.SIP_D_ESTABLECIMIENTOS a ";
            sql += " INNER JOIN SIPEC.sip_d_establecimientos_ties tie ";
            sql += " ON a.id = tie.id_esta ";
            sql += " INNER JOIN  SIPEC.sip_d_tipos_establecimientos tipo ";
            sql += " ON tie.id_ties = tipo.id ";
            sql += " INNER JOIN  SIPEC.SIP_D_ENCARGADOS_REGIONALES ER ";
            sql += " ON a.id_regi = ER.idregion ";
            sql += " INNER JOIN SIPEC.SIP_D_ENCARGADOS_SECTORIALES ES ";
            sql += " ON a.ID_OFSE = ES.IDSECTORIAL ";
            sql += " INNER JOIN MS_REGION_VW R ";
            sql += " ON a.id_regi = R.IDREGION ";
            sql += " INNER JOIN MS_SECTORIAL_VW OS ";
            sql += " ON a.ID_OFSE = OS.IDSECTORIAL ";
            sql += " LEFT JOIN SIPEC.SIP_D_CLAS_ASIG_ESTAB CE ";
            sql += " ON(a.ID = CE.ID_ESTA AND CE.vigente = 1) ";
            sql += " LEFT JOIN SIPEC.sip_d_especies E ";
            sql += " ON E.ID = CE.ID_ESPE ";
            sql += " left JOIN SIPEC.sip_d_grupos_especies GE ";
            sql += "  ON GE.ID = E.ID_GRES ";
            sql += "where ";
            //sql += " a.ID_REGI = 10 ";
            //sql += " and GE.ID = 1 ";
            sql += "  a.rup = '" + rup + "' ";
            sql += "and tie.fecha_termino is null ";

            OracleDataAdapter datos = new OracleDataAdapter(sql, con);

            DataSet data = new DataSet();
            datos.Fill(data);
            int i;
            int peso = 0;
            if (data.Tables[0].Rows.Count == 0)
            {
                // NO HAY CLASIFICACION; SE ASIGNA EL PESO MAYOR
                peso = 10;
                con.Close();
                return peso;
            }
            for (i = 0; i < data.Tables[0].Rows.Count; i++)
            {
                Console.WriteLine(data.Tables[0].Rows[i][4].ToString());
                var te = data.Tables[0].Rows[i][2].ToString();
                var rubro = data.Tables[0].Rows[i][4].ToString();
                //1 PREDIO / 1 LECHE => 10 
                //1 PREDIO / 2 CARNE => 5
                //148 FERIA / ...    => 10

                // sumar el peso si tiene mas de un rubro
                if (te == "1" && rubro == "1")
                {
                    peso = peso + 10;
                }
                else
                {
                    if (te == "148")
                    {
                        peso = peso + 10;
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

                progressBar2.Value = 99;
            }

            con.Close();
            return peso;
        }

        int ResultadoEpimediologico(string pesoPositivo, int nivelContagio)
        {
            if (pesoPositivo == "0,0")
            {
                Random r = new Random();
                int rInt = r.Next(10, 50);
                return rInt;
            }

            if (pesoPositivo == "1,0" && nivelContagio == 10)
            {
                Random r = new Random();
                int rInt = r.Next(300, 350);
                return rInt;
            }
            if (pesoPositivo == "0,5" && nivelContagio >= 10)
            {
                Random r = new Random();
                int rInt = r.Next(260, 290);
                return rInt;
            }
            if (pesoPositivo == "1,0" && (nivelContagio >= 5 && nivelContagio < 10))
            {
                Random r = new Random();
                int rInt = r.Next(200, 250);
                return rInt;
            }
            if (pesoPositivo == "0,5" && (nivelContagio >= 5 && nivelContagio < 10))
            {
                Random r = new Random();
                int rInt = r.Next(160, 190);
                return rInt;
            }
            if (pesoPositivo == "1,0" && (nivelContagio >= 1 && nivelContagio < 5))
            {
                Random r = new Random();
                int rInt = r.Next(100, 150);
                return rInt;
            }
            if (pesoPositivo == "0,5" && (nivelContagio >= 1 && nivelContagio < 5))
            {
                Random r = new Random();
                int rInt = r.Next(60, 90);
                return rInt;
            }


            return 400;
        }








        private void Calculate(int i)
        {
            double pow = Math.Pow(i, i);
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var backgroundWorker = sender as BackgroundWorker;
            for (int j = 0; j < 100000; j++)
            {
                Calculate(j);
                backgroundWorker.ReportProgress((j * 100) / 100000);
            }
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // TODO: do something with final calculation.
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
    }
    public class estructuraResultado
    {
        public string fecha { get; set; }
        public int resultado { get; set; }
    }

}
