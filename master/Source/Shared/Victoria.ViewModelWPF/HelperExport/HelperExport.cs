using iTextSharp.text;
using iTextSharp.text.pdf;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Victoria.ViewModelWPF.HelperExport
{
    public static class HelperExport
    {
        public static void ExportStagesToExcel(IList<StageViewModelBase> stages, string simulacionFileName)
        {
            try
            {
                OfficeOpenXml.ExcelPackage pck = new OfficeOpenXml.ExcelPackage();

                foreach (StageViewModel s in stages)
                {                    
                    var ws = pck.Workbook.Worksheets.Add(s.Name);
                    ws.View.ShowGridLines = false;

                    int row = 2;

                    #region Table Result

                    #region Look and feel

                    ws.Cells[1, 4].Value = "Variable";
                    ws.Cells[1, 5].Value = "Valor Inicial";
                    ws.Cells[1, 6].Value = "Valor Simulación";
                    ws.Cells[1, 4].Style.Font.Color.SetColor(Color.White);
                    ws.Cells[1, 5].Style.Font.Color.SetColor(Color.White);
                    ws.Cells[1, 6].Style.Font.Color.SetColor(Color.White);

                    ws.Cells[1, 4].Style.Font.Bold = ws.Cells[1, 5].Style.Font.Bold = ws.Cells[1, 6].Style.Font.Bold = true;

                    ws.Cells[1, 4].Style.Fill.PatternType = ws.Cells[1, 5].Style.Fill.PatternType = ws.Cells[1, 6].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ws.Cells[1, 4].Style.Fill.BackgroundColor.SetColor(Color.Blue);
                    ws.Cells[1, 5].Style.Fill.BackgroundColor.SetColor(Color.Blue);
                    ws.Cells[1, 6].Style.Fill.BackgroundColor.SetColor(Color.Blue);

                    ws.Cells[1, 4].Style.Border.Bottom.Style = ws.Cells[1, 41].Style.Border.Left.Style = ws.Cells[1, 4].Style.Border.Right.Style = ws.Cells[1, 4].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    ws.Cells[1, 5].Style.Border.Bottom.Style = ws.Cells[1, 5].Style.Border.Left.Style = ws.Cells[1, 5].Style.Border.Right.Style = ws.Cells[1, 5].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    ws.Cells[1, 6].Style.Border.Bottom.Style = ws.Cells[1, 6].Style.Border.Left.Style = ws.Cells[1, 6].Style.Border.Right.Style = ws.Cells[1, 6].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

                    #endregion Looh and feel

                    foreach (ModelWPF.Variable v in s.Variables)
                    {
                        ws.Cells[row, 4].Value = v.Name;
                        ws.Cells[row, 5].Value = v.InitialValue;
                        ws.Cells[row, 6].Value = v.ActualValue;

                        #region Look and feel

                        ws.Cells[row, 4].Style.Fill.PatternType = ws.Cells[row, 5].Style.Fill.PatternType = ws.Cells[row, 6].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws.Cells[row, 4].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                        ws.Cells[row, 5].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                        ws.Cells[row, 6].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);

                        ws.Cells[row, 4].Style.Border.Bottom.Style = ws.Cells[row, 4].Style.Border.Left.Style = ws.Cells[row, 4].Style.Border.Right.Style = ws.Cells[row, 4].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        ws.Cells[row, 5].Style.Border.Bottom.Style = ws.Cells[row, 5].Style.Border.Left.Style = ws.Cells[row, 5].Style.Border.Right.Style = ws.Cells[row, 5].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        ws.Cells[row, 6].Style.Border.Bottom.Style = ws.Cells[row, 6].Style.Border.Left.Style = ws.Cells[row, 6].Style.Border.Right.Style = ws.Cells[row, 6].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

                        #endregion Look and feel

                        row++;
                    }

                    ws.Cells.AutoFitColumns();

                    #endregion Table Result

                    #region Chart Result

                    int graficos = 0;

                    foreach (ChartViewModel c in s.Charts)
                    {
                        MemoryStream stream = new MemoryStream();

                        ((ChartViewModel)c).ExportChart(stream, 800, 600);
                        
                        var imagen = Bitmap.FromStream(stream);
                        stream.Close();
                     
                        OfficeOpenXml.Drawing.ExcelPicture pic = ws.Drawings.AddPicture(c.Name, imagen);
                        int topPosition = (int)20 * (row - 1) + 2;
                        pic.SetPosition(topPosition + (600 * graficos++), 0);
                    }

                    #endregion Chart Result                    
                }

                pck.SaveAs(new FileInfo(simulacionFileName));
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static void ExportStagesToPDF(IList<StageViewModelBase> stages, string simulacionFileName)
        {
            Document doc = null;
            iTextSharp.text.pdf.PdfWriter writer = null;
            try
            {
                doc = new Document(iTextSharp.text.PageSize.LETTER);
                writer = iTextSharp.text.pdf.PdfWriter.GetInstance(doc, new FileStream(simulacionFileName, FileMode.Create));
                doc.AddTitle("Simulación");
                doc.AddCreator("JALUPEGUI Sistemas");
                doc.Open();

                iTextSharp.text.Font _standardFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

                doc.Add(new Paragraph("Resultados Simulación"));
                doc.Add(Chunk.NEWLINE);

                foreach (StageViewModel s in stages)
                {
                    doc.Add(new Paragraph(s.Name));
                    doc.Add(Chunk.NEWLINE);

                    PdfPTable tblDatos = new PdfPTable(3);
                    tblDatos.WidthPercentage = 100;

                    #region Look and Feel

                    PdfPCell clVariable = new PdfPCell(new Phrase("Variable", _standardFont));
                    clVariable.BorderWidth = 0;
                    clVariable.BorderWidthBottom = 0.75f;

                    PdfPCell clValorInicial = new PdfPCell(new Phrase("Valor Inicial", _standardFont));
                    clValorInicial.BorderWidth = 0;
                    clValorInicial.BorderWidthBottom = 0.75f;

                    PdfPCell clValorSimulacion = new PdfPCell(new Phrase("Valor Simulación", _standardFont));
                    clValorSimulacion.BorderWidth = 0;
                    clValorSimulacion.BorderWidthBottom = 0.75f;

                    tblDatos.AddCell(clVariable);
                    tblDatos.AddCell(clValorInicial);
                    tblDatos.AddCell(clValorSimulacion);

                    #endregion Look and Feel

                    foreach (ModelWPF.Variable v in s.Variables)
                    {
                        clVariable = new PdfPCell(new Phrase(v.Name, _standardFont));
                        clVariable.BorderWidth = 0;

                        clValorInicial = new PdfPCell(new Phrase(v.InitialValue.ToString(), _standardFont));
                        clValorInicial.BorderWidth = 0;

                        clValorSimulacion = new PdfPCell(new Phrase(v.ActualValue.ToString(), _standardFont));
                        clValorSimulacion.BorderWidth = 0;

                        tblDatos.AddCell(clVariable);
                        tblDatos.AddCell(clValorInicial);
                        tblDatos.AddCell(clValorSimulacion);
                    }

                    doc.Add(tblDatos);

                    foreach (ChartViewModel c in s.Charts)
                    {
                        MemoryStream memStream = new MemoryStream();

                        ((ChartViewModel)c).ExportChart(memStream, 800, 600);

                        var img = Bitmap.FromStream(memStream);
                        iTextSharp.text.Image imagen = iTextSharp.text.Image.GetInstance(img, BaseColor.WHITE);
                        imagen.BorderWidth = 0;
                        imagen.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                        imagen.ScalePercent(50);

                        doc.Add(imagen);

                        memStream.Flush();
                        memStream.Close();
                    }

                    doc.NewPage();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                doc.Close();
                writer.Close();
            }
        }
    }
}
