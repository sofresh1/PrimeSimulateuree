using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.EntityFrameworkCore;
using PrimeSimulateur.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace PrimeSimulateur.GenerateDocuments
{
    public class PrimeSimulatorDocs
    {
        private List<byte[]> docList = new List<byte[]>();
        private readonly MyDbContext _context;

        public PrimeSimulatorDocs(MyDbContext context)
        {
            _context = context;
        }


        public async Task<byte[]> Generate(string userEmail)
        {
            return await CreateDevis(userEmail);
        }

        private async Task<byte[]> CreateDevis(string userEmail)
        {
            var font = new Font(Font.FontFamily.HELVETICA, 8, 0, BaseColor.BLACK);

            var sSourceFile = "";
            try
            {
                sSourceFile = Path.GetFullPath($"wwwroot/documents/Devis.pdf");
            }
            catch (Exception)
            {

            }

            MemoryStream output = new MemoryStream();
            PdfReader reader = new PdfReader(sSourceFile);
            PdfStamper stamper = new PdfStamper(reader, output)
            {
                RotateContents = false
            };

            PdfContentByte canvas = stamper.GetOverContent(1);

            var client = (from C in _context.Clients
                            where C.email == userEmail
                          select C).FirstOrDefault();
            List<trace> list_trace = await (from T in _context.trace
                                            where T.email == userEmail
                                            select T).ToListAsync();
          

            ColumnText.ShowTextAligned(canvas, Element.ALIGN_LEFT, new Phrase(client.ClientId), 107, 682, 0);
            ColumnText.ShowTextAligned(canvas, Element.ALIGN_LEFT, new Phrase(DateTime.Now.ToShortDateString()), 65, 667, 0);
            ColumnText.ShowTextAligned(canvas, Element.ALIGN_LEFT, new Phrase($"{client.lastName.ToUpper()} {client.firstName.ToUpper()}" ), 307, 703, 0);
            ColumnText.ShowTextAligned(canvas, Element.ALIGN_LEFT, new Phrase(client.email), 347, 682, 0);

            var totalPrime = 0.0;
            var y = 314;
            foreach (var item in list_trace)
            {
                totalPrime += item.prime; 
                ColumnText.ShowTextAligned(canvas, Element.ALIGN_LEFT, new Phrase(item.Nom),38, y, 0);
                ColumnText.ShowTextAligned(canvas, Element.ALIGN_LEFT, new Phrase(item.Type),198, y , 0);
                ColumnText.ShowTextAligned(canvas, Element.ALIGN_LEFT, new Phrase(item.Surface.ToString()), 370, y, 0);
                ColumnText.ShowTextAligned(canvas, Element.ALIGN_LEFT, new Phrase(item.prime.ToString()), 498, y, 0);
                y += 14;
            }

            ColumnText.ShowTextAligned(canvas, Element.ALIGN_LEFT, new Phrase(totalPrime.ToString()), 429, 136, 0);


            stamper.FormFlattening = true;
            stamper.Close();
            reader.Close();
            docList.Add(output.ToArray());
            return output.ToArray();
        }

        //private async Task<string> CaloCreateDossiersAAP()
        //{
        //    string path = $"wwwroot/documents/AppelPaiement/Dossier_{CaloAppelPaiement.Reference}.csv";
        //    StreamWriter file = new StreamWriter(@path, false, Encoding.UTF32);
        //    int i = 1;
        //    var csv = new CsvWriter(file, null);
        //    csv.Configuration.Delimiter = ",";
        //    csv.WriteField("N°");
        //    csv.WriteField("Nom");
        //    csv.WriteField("Surface");
        //    csv.NextRecord();

        //    try
        //    {

        //        var list_travaux = new List<Travail>()
        //        {
        //            new Travail{TravailId = 1, Name = "T1", surface = 100},
        //            new Travail{TravailId = 2, Name = "T2", surface = 150},
        //            new Travail{TravailId = 3, Name = "T3", surface = 110},

        //        };
        //        foreach (var item in list_travaux)
        //        {
        //            csv.WriteField(i++);
        //            csv.WriteField(item.TravailId);
        //            csv.WriteField(item.Name);
        //            csv.WriteField(item.surface);

        //            csv.NextRecord();
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    csv.Flush();
        //    file.Close();
        //    return "/Dossier.csv";
        //}


        public static StringBuilder GenerateExportHeader()
        {
            StringBuilder sb = new StringBuilder();

            // header line
            sb.Append("Nom;");
            sb.Append("Surface;");
            sb.Append("Type;");
            sb.Append("prime;");
            return sb;
        }

        public static StringBuilder GenerateExportRecordLine(DataRow objRD)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(objRD["Nom"].ToString().Replace(";", ",") + ";");
            sb.Append(objRD["Surface"].ToString().Replace(";", ",") + ";");
            sb.Append(objRD["Type"].ToString().Replace(";", ",") + ";");
            sb.Append(objRD["prime"].ToString().Replace(";", ",") + ";");

            return sb;
        }
        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }

            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }

                dataTable.Rows.Add(values);
            }

            return dataTable;
        }
    }
}
