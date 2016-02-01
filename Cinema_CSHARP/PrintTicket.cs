using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Printing;
using BarcodeLib.Barcode;
using System.IO;

namespace Cinema_CSHARP
{
	public class Print
	{
		static string filepath = Path.GetTempPath() + Guid.NewGuid().ToString() + ".png";
		static string path2 = filepath;
		static string path = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
		static string number = Cinema.ReadLinesFromFile(path + "\\prenotazioni.txt").ToString();
		static Random rnd = new Random();
		static int randomsala2 = rnd.Next(65, 68);
		static char randomchar = (char)randomsala2;
		public static string sala = randomchar.ToString();

		public static void QRCode(string data, int size)
		{
			QRCode qrbarcode = new QRCode();

			// Select QR Code data encoding type: numeric, alphanumeric, byte, and Kanji to select from.
			qrbarcode.Encoding = QRCodeEncoding.Auto;
			qrbarcode.Data = data;

			// Adjusting QR Code barcode module size and quiet zones on four sides.
			qrbarcode.ModuleSize = size;
			qrbarcode.LeftMargin = 12;
			qrbarcode.RightMargin = 12;
			qrbarcode.TopMargin = 12;
			qrbarcode.BottomMargin = 12;

			// Select QR Code Version (Symbol Size), available from V1 to V40, i.e. 21 x 21 to 177 x 177 modules.
			qrbarcode.Version = QRCodeVersion.V1;

			// Set QR-Code bar code Reed Solomon Error Correction Level: L(7%), M (15%), Q(25%), H(30%)
			qrbarcode.ECL = QRCodeErrorCorrectionLevel.L;
			qrbarcode.ImageFormat = System.Drawing.Imaging.ImageFormat.Png;

			// More barcode settings here, like ECI, FNC1, Structure Append, etc.

			// save barcode image into your system
			qrbarcode.drawBarcode(filepath);
		}
		public static void PrintPage(object o, PrintPageEventArgs e)
		{
			
			const int SPACE = 145;
			string title = path + "\\Title.png";
			Graphics g = e.Graphics;
			g.DrawRectangle(Pens.Black, 5, 5, 770, 515);
			Point pp = new Point(5, 120);
			Point pp2 = new Point(775, 120);
			int rndx = rnd.Next(1, 3204);
			g.DrawImage(Image.FromFile(title), 250, 10);
			Font fBody = new Font("Lucida Console", 15, FontStyle.Bold);
			Font fBody1 = new Font("Lucida Console", 15, FontStyle.Regular);
			Font rs = new Font("Stencil", 25, FontStyle.Bold);
			Font fTType = new Font("", 150, FontStyle.Bold);
			SolidBrush sb = new SolidBrush(Color.Black);

			g.DrawLine(Pens.Black, pp, pp2);
			//g.DrawString("----------------------------------------------------------", fBody1, sb, 10, 120);

			g.DrawString("Data:", fBody, sb, 10, SPACE);
			g.DrawString(DateTime.Now.ToShortDateString(), fBody1, sb, 80, SPACE);

			g.DrawString("Ora:", fBody, sb, 10, SPACE + 30);
			g.DrawString(DateTime.Now.ToShortTimeString(), fBody1, sb, 70, SPACE + 30);

			g.DrawString("TicketNo.:", fBody, sb, 10, SPACE + 60);
			g.DrawString(number, fBody1, sb, 150, SPACE + 60);

			g.DrawString("Nome Film:", fBody, sb, 10, SPACE + 90);
			g.DrawString(Cinema.filmsel, fBody1, sb, 150, SPACE + 90);

			//g.DrawString("DriverName:", fBody, sb, 10, SPACE+120);
			//g.DrawString(txtDriveName.Text, fBody1, sb, 153, SPACE + 120);

			g.DrawString("Sala:", fBody, sb, 10, SPACE + 120);
			g.DrawString(Cinema.selsala, fBody1, sb, 80, SPACE + 120);
			g.DrawString("Posti:", fBody, sb, 10, SPACE + 150);
			g.DrawString(Cinema.places, fBody1, sb, 90, SPACE + 150);

			g.DrawString("Prezzo:", fBody, sb, 10, SPACE + 180);
			g.DrawString(Cinema.total_price.ToString() + " Euro", fBody1, sb, 110, SPACE + 180);
			QRCode(Whole(), 3);
			g.DrawImage(Image.FromFile(filepath), 10, SPACE + 200);
		}
		public static string Whole()
		{
			string sep = " | ";
			string all = DateTime.Now.ToShortDateString() + sep + DateTime.Now.ToShortTimeString() + sep + number + sep + "3B Info Story" + sep + sala + sep + Cinema.places + sep + Cinema.total_price.ToString() + " euro";
			return all;
		}
		public static void PrintTicket()
		{
			PrintDocument pd = new PrintDocument();
			PaperSize ps = new PaperSize("", 780, 530);
			PrintPageEventHandler ciao = new PrintPageEventHandler(PrintPage);
			pd.PrintPage += new PrintPageEventHandler(PrintPage);
			pd.PrintController = new StandardPrintController();
			pd.DefaultPageSettings.Margins.Left = 0;
			pd.DefaultPageSettings.Margins.Right = 0;
			pd.DefaultPageSettings.Margins.Top = 0;
			pd.DefaultPageSettings.Margins.Bottom = 0;
			pd.DefaultPageSettings.PaperSize = ps;
			pd.Print();
		}
	}
}