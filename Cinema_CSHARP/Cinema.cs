using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

namespace Cinema_CSHARP
{
	static class Cinema
	{

		//Dichiarazione costanti
		const int FULL = 12;

		//Dichiarazione variabili globali
		static int[] posto; //La variabile del posto
		static char[] fila; //La variabile della fila
		static string[] film = { "Doraemon - Il Film", "Creed - Nato per Combattere", "Point Break", "Joy" };
		static bool[] selectingfilmz = { true, false, false, false };
		static bool selectfilmbool = false;
		static string[] sale = { "A", "B", "C", "D" };
		public static string filmsel = "Doraemon - Il Film";
		public static string selsala = "A";
		static bool[,] occupati; //La variabile dei posti occupati
		static bool end = false; //La variabile che indica se il programma è da terminare oppure no.
		static bool[,] selected; //La variabile del posto attualmente selezionato.
		static int selected_count = 0; //La variabile numerica del posto attualmente selezionato.
		static bool selecting = false; //La variabile che indica se si sta selezionando un posto.
		static int sel_par1; //La variabile parziale del posto/fila, con la sel_par2, servono a mostrare i posti nel biglietto o sullo schermo.
		static int sel_par2;
		static bool error = false; //La variabile che indica se il posto è già prenotato.
		static int count = 0; //La variabile che indica quanti posti sono stati prenotati dall'utente.
		static float price = 6.00F; //La variabile del prezzo. Prossimamente sarà possibile modificare il prezzo tramite un secondo programma d'amministrazione.
		public static float total_price = 0.00F; // Il prezzo totale calcolato.
		static bool checkout = false; //La variabile che indica se l'utente sta eseguendo il checkout
		static bool retry = false; //La variabile che indica se l'utente deve re-immettere i dati
		static bool logged = false; //La variabile che indica se l'utente ha fatto il login oppure no
		static bool loginscreen = false; //La variabile che indica se l'utente è all'interno della schermata di login oppure no
		static string loggeduser = ""; //La variabile che indica l'utente attualmente loggato.
		static string[,] credentials; //La variabile delle credenziali d'accesso.
		static int userindex; //L'ID dell'utente.
		public static string places; //I posti prenotati dall'utente.
		static string path = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]); //La stringa che definisce il percorso del file eseguibile.
		static int index;

		//Costanti relative all'Encrypt della password
		public const int SALT_BYTE_SIZE = 24;
		public const int HASH_BYTE_SIZE = 24;
		public const int PBKDF2_ITERATIONS = 1000;
		public const int ITERATION_INDEX = 0;
		public const int SALT_INDEX = 1;
		public const int PBKDF2_INDEX = 2;

		static void LoginScreen()
		{
			if(loginscreen == true)
			{
				Console.Clear();
				Intestazione();
				Console.ForegroundColor = ConsoleColor.White;
				Console.WriteLine("Premi INVIO per selezionare il film.");
				Console.WriteLine("Premi CANC per cancellare tutte le prenotazioni.");
				Console.WriteLine("Premi BACKSPACE in qualsiasi momento per fare il logout.");
				ConsoleKeyInfo keypress;
				keypress = Console.ReadKey();
				switch (keypress.Key)
				{
					case ConsoleKey.Enter:
						loginscreen = false;
						selectfilmbool = true;
						SelectFilm();
							break;
					case ConsoleKey.Backspace:
						loginscreen = false;
						logged = false;
						Login();
							break;
					case ConsoleKey.Delete:
						Console.Write("Cancellando le prenotazioni della sala A....");
						System.Threading.Thread.Sleep(800);
						File.WriteAllText(path + "\\posti_sala_A.txt", "");
						Console.Write("\tFATTO.");
						Console.WriteLine();
						Console.Write("Cancellando le prenotazioni della sala B....");
						System.Threading.Thread.Sleep(800);
						File.WriteAllText(path + "\\posti_sala_B.txt", "");
						Console.Write("\tFATTO.");
						Console.WriteLine();
						Console.Write("Cancellando le prenotazioni della sala C....");
						System.Threading.Thread.Sleep(800);
						File.WriteAllText(path + "\\posti_sala_C.txt", "");
						Console.Write("\tFATTO.");
						Console.WriteLine();
						Console.Write("Cancellando le prenotazioni della sala D....");
						System.Threading.Thread.Sleep(800);
						File.WriteAllText(path + "\\posti_sala_D.txt", "");
						Console.Write("\tFATTO.");
						System.Threading.Thread.Sleep(2000);
						LoginScreen();
						break;
					default:
						LoginScreen();
                            break;
				}
			}
		}

		static void SelectFilm()
		{
			Console.Clear();
			Intestazione();
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine("Naviga tra i film con le freccette.");
			Console.WriteLine("Premi INVIO per scegliere il film.");

			for (int i = 0; i < film.Count(); i++)
			{
				if(selectingfilmz[i] == true)
				{
					Console.BackgroundColor = ConsoleColor.White;
					Console.ForegroundColor = ConsoleColor.Black;
					Console.WriteLine("= " + film[i]);
					Console.BackgroundColor = ConsoleColor.Black;
					Console.ForegroundColor = ConsoleColor.White;
				}
				else
				{
					Console.ForegroundColor = ConsoleColor.White;
					Console.WriteLine("- " + film[i]);
				}
			}
			UserInput2();
		}

		static void UserInput2()
		{
			
			ConsoleKeyInfo keypress;
			keypress = Console.ReadKey();
			switch (keypress.Key)
			{
				case ConsoleKey.DownArrow:
					for(int i = 0; i < film.Count(); i++)
					{
						if (selectingfilmz[i] == true)
						{
							if(selectingfilmz[3] == false)
							{
								selectingfilmz[i] = false;
								selectingfilmz[i + 1] = true;
								index = i + 1;
								break;
							}
							else if(selectingfilmz[3] == true)
							{
								selectingfilmz[3] = false;
								selectingfilmz[0] = true;
								index = 0;
								break;
							}
						}
                    }
					SelectFilm();
					break;

				case ConsoleKey.UpArrow:
					for (int i = 0; i < film.Count(); i++)
					{
						if (selectingfilmz[i] == true)
						{
							if (selectingfilmz[0] == false)
							{
								selectingfilmz[i] = false;
								selectingfilmz[i - 1] = true;
								index = i - 1;
								break;
							}
							else if (selectingfilmz[0] == true)
							{
								selectingfilmz[0] = false;
								selectingfilmz[3] = true;
								index = 3;
								break;
							}
						}
					}
					SelectFilm();
					break;
				case ConsoleKey.Enter:
					selectfilmbool = false;
					filmsel = film[index];
					selsala = sale[index];
					GeneratePostiOccupati();
					Refresh();
					break;
				default:
					SelectFilm();
					break;
			}
		}

		public static int ReadLinesFromFile(string file)
		{
			if (File.Exists(file))
			{
				int lineCount = File.ReadLines(file).Count();
				return lineCount;
			}
			else
			{
				return 0;
			}
		}

		private static string ReadFileToArray(string file, int line)
		{
			string nativearray;
			nativearray = File.ReadLines(file).Skip(line).Take(1).First();
			return nativearray;
		}

        public static void GeneratePostiOccupati()
        {
            for (int i = 0; i < ReadLinesFromFile(path + "\\posti_sala_" + selsala + ".txt"); i++)
            {
                string[] partials = ReadFileToArray(path + "\\posti_sala_" + selsala + ".txt", i).Split('^');

                    int occplace1 = 0;
                    int occplace2 = 0;
                    Int32.TryParse(partials[0], out occplace1);
                    Int32.TryParse(partials[1], out occplace2);
                    occupati[occplace1,occplace2] = true;
            }

        }
      
        public static void GenerateCredentials()
		{
			for (int i = 0; i < ReadLinesFromFile(path + "\\credentials.txt"); i++)
			{
				string[] combo = ReadFileToArray(path + "\\credentials.txt", i).Split('^');
				int x = -1;
				foreach(string wordz in combo)
				{
					x++;
					credentials[i, x] = wordz;
				}
			} 
		}

		public static void Login()
		{
			Console.Clear();
			Intestazione();
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("");
			Console.WriteLine("");
			Console.Write(" Username: ");
			Console.ForegroundColor = ConsoleColor.White;
			string tryuserlogin = Console.ReadLine();
			Console.WriteLine("");
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.Write(" Password: ");
			Console.ForegroundColor = ConsoleColor.White;
			string trypasslogin = Orb.App.Console.ReadPassword();
			Console.WriteLine("");
			
			//Test di connessione al DATABASE
			/*using (SqlConnection con = new SqlConnection(connectionString))
			{
				con.Open();
				SqlCommand command = new SqlCommand("SELECT [password] from [dbo].[Table] ");
				using (var reader = command.ExecuteReader())
				{
					reader.Read();
					hash = reader.GetString(0);
				}
				command = new SqlCommand("SELECT [username] from [dbo].[Table] ");
				using (var reader = command.ExecuteReader())
				{
					reader.Read();
					user = reader.GetString(0);
				}
				con.Close();
			}*/

			for (int i = 0; i < ReadLinesFromFile(path + "\\credentials.txt"); i++)
					{
						if (tryuserlogin == credentials[i, 0] && Encrypt.ValidatePassword(trypasslogin, credentials[i,1]) == true)
						{
							Console.CursorVisible = false;
							Console.Write(" Stai per essere collegato al server");
							loggeduser = credentials[i, 0];
							userindex = i;
							for (int k = 0; k < 10; k++)
							{
								System.Threading.Thread.Sleep(150);
								Console.Write(".");
							}
							loginscreen = true;
							logged = true;
							LoginScreen();
						return;
						}
				}
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(" Username o Password errati!");
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine(" Premi qualsiasi tasto per tornare al Login.");
			ConsoleKeyInfo keypress;
			keypress = Console.ReadKey();
			switch (keypress.Key)
			{
				default:
					Login();
					break;
			}
		}
		static void FullCheck()
		{
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine("");
			Console.WriteLine(" Grazie per aver prenotato dei posti nel nostro cinema!");
			Console.WriteLine(" Purtroppo e' possibile prenotare max {0} posti alla volta.", FULL);
			Console.WriteLine(" Il prezzo complessivo è di {0} Euro!", total_price);
			ExitCheckout();
		}
		static void Checkout()
		{
			if (total_price > 0.00)
			{
				Console.ForegroundColor = ConsoleColor.White;
				Console.WriteLine("");
				Console.WriteLine(" Grazie per aver prenotato dei posti nel nostro cinema!");
				Console.WriteLine(" Il prezzo complessivo è di {0} Euro!", total_price);
				Console.WriteLine(" Si ricordi di stampare il biglietto!");
				ExitCheckout();
			}
			else
			{
				checkout = false;
				retry = true;
			}
		}

		static void Retry()
		{
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine("");
			Console.WriteLine(" Devi selezionare almeno un posto, prima di completare la prenotazione!");
			Console.WriteLine(" Quando hai scelto il posto, premi il tasto INVIO.");
			Console.WriteLine(" Premere BARRA SPAZIATRICE se vuoi terminare la prenotazione.");
			retry = false;
		}

		static void ExitCheckout()
		{
			{
				Console.ForegroundColor = ConsoleColor.White;
				Console.WriteLine(" ");
				Console.WriteLine(" Premi BACKSPACE per uscire e stampare il biglietto...");
				ConsoleKeyInfo keypress;
				keypress = Console.ReadKey();
				switch (keypress.Key)
				{
					case ConsoleKey.Backspace:
						Print.PrintTicket();
						File.AppendAllText(path + "\\prenotazioni.txt", Print.Whole() + Environment.NewLine);
						Environment.Exit(0);
						break;
					default:
						break;
				}

			}
		}

		static void Pause()
		{
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine(" ");
			Console.WriteLine(" Premi BACKSPACE per continuare...");
			ConsoleKeyInfo keypress;
			keypress = Console.ReadKey();
			switch(keypress.Key)
			{
				case ConsoleKey.Backspace:
					Environment.Exit(0);
					break;
				default:
					break;
			}
			
        }

		static void Intestazione()	
		{
			if (logged == true && loginscreen == false && selectfilmbool == false)
			{
				Console.BackgroundColor = ConsoleColor.Black;
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine(" -------------------------------------------------------------------------------------------------------");
				Console.WriteLine(" -------------------------------------------------------------------------------------------------------");
				Console.ForegroundColor = ConsoleColor.White;
				Console.WriteLine(" ---------------------------------------- B E N V E N U T O ! ! ----------------------------------------");
				Console.WriteLine(" ------------------------------ P R E N O T A  I L  T U O  P O S T O ! ! -------------------------------");
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(" -------------------------------------------------------------------------------------------------------");
				Console.WriteLine(" -------------------------------------------------------------------------------------------------------");
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine(" Ciao, {0}! Ecco la tua interfaccia utente.", loggeduser);
				Console.ForegroundColor = ConsoleColor.White;
				Console.WriteLine(" Stai prenotando dei posti nella sala {0}, che proietterà il film '{1}'", selsala, filmsel);
			}
			else if (selectfilmbool == true)
			{
				Console.BackgroundColor = ConsoleColor.Black;
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine(" -------------------------------------------------------------------------------------------------------");
				Console.WriteLine(" -------------------------------------------------------------------------------------------------------");
				Console.ForegroundColor = ConsoleColor.White;
				Console.WriteLine(" ---------------------------------------- B E N V E N U T O ! ! ----------------------------------------");
				Console.WriteLine(" --------------------------------- S E L E Z I O N A  I L  F I L M ! ! ---------------------------------");
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(" -------------------------------------------------------------------------------------------------------");
				Console.WriteLine(" -------------------------------------------------------------------------------------------------------");
			}
			else if (logged == false)
			{
				Console.BackgroundColor = ConsoleColor.Black;
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine(" -------------------------------------------------------------------------------------------------------");
				Console.WriteLine(" -------------------------------------------------------------------------------------------------------");
				Console.ForegroundColor = ConsoleColor.White;
				Console.WriteLine(" ---------------------------------------- B E N V E N U T O ! ! ----------------------------------------");
				Console.WriteLine(" ---------------------------------- E F F E T T U A  I L  L O G I N ! ----------------------------------");
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(" -------------------------------------------------------------------------------------------------------");
				Console.WriteLine(" -------------------------------------------------------------------------------------------------------");
			}
			else if (loginscreen == true && logged == true)
			{
				Console.BackgroundColor = ConsoleColor.Black;
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine(" -------------------------------------------------------------------------------------------------------");
				Console.WriteLine(" -------------------------------------------------------------------------------------------------------");
				Console.ForegroundColor = ConsoleColor.White;
				Console.WriteLine(" ---------------------------------------- B E N V E N U T O ! ! ----------------------------------------");
				Console.WriteLine(" ------------------------------------ C O S A  V U O I  F A R E ? --------------------------------------");
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(" -------------------------------------------------------------------------------------------------------");
				Console.WriteLine(" -------------------------------------------------------------------------------------------------------");

			}
		}

		static void Initialize()
		{
			for (int i = 0; i < 15; i++)
			{
				int filexxx = 65 + i;
				fila[i] = (char)filexxx;
				for (int k = 0; k < 20; k++)
				{
					posto[k] = k + 1;
                    
					occupati[i, k] = false;
					selected[i, k] = false;
				}
			}
			selected[0, 0] = true;
		}

		static void UserInput() // Questa funzione serve a far navigare l'utente con le frecce direzionali.
		{
			ConsoleKeyInfo keypress;
			keypress = Console.ReadKey();
			if (selecting == false)
			{
				switch (keypress.Key)
				{
					case ConsoleKey.RightArrow:
						for (int i = 0; i < 15; i++)
						{
							for (int k = 0; k < 20; k++)
							{
								if (selected[i, k] == true)
								{
									if (selected[i, 19] == false)
									{
										selected[i, k] = false;
										k++;
										selected[i, k] = true;
										k--;
										sel_par1 = i;
										sel_par2 = k+1;
										selected_count++;
										return;
									}
									else
									{
										selected[i, 19] = false;
										selected[i, 0] = true;
										sel_par1 = i;
										sel_par2 = 0;
										selected_count -= 19;
										return;
									}
								}
							}
						}
						break;

					case ConsoleKey.LeftArrow:
						for (int i = 0; i < 15; i++)
						{
							for (int k = 0; k < 20; k++)
							{
								if (selected[i, k] == true)
								{
									if (selected[i, 0] == false)
									{
										selected[i, k] = false;
										k--;
										selected[i, k] = true;
										k++;
										sel_par1 = i;
										sel_par2 = k-1;
										selected_count--;
										return;
									}
									else
									{
										selected[i, 0] = false;
										selected[i, 19] = true;
										sel_par1 = i;
										sel_par2 = 19;
										selected_count += 19;
										return;
									}
								}
							}
						}
						break;

					case ConsoleKey.DownArrow:
						for (int i = 0; i < 15; i++)
						{
							for (int k = 0; k < 20; k++)
							{
								if (selected[i, k] == true)
								{
									if (selected[14, k] == false)
									{
										selected[i, k] = false;
										i++;
										selected[i, k] = true;
										i--;
										sel_par1 = i+1;
										sel_par2 = k;
										selected_count += 20;
										return;
									}
									else
									{
										selected[14, k] = false;
										selected[0, k] = true;
										sel_par1 = 0;
										sel_par2 = k;
										selected_count -= 140;
										return;
									}
								}
							}
						}
						break;

					case ConsoleKey.UpArrow:
						for (int i = 0; i < 15; i++)
						{
							for (int k = 0; k < 20; k++)
							{
								if (selected[i, k] == true)
								{
									if (selected[0, k] == false)
									{
										selected[i, k] = false;
										i--;
										selected[i, k] = true;
										sel_par1 = i;
										sel_par2 = k;
										i++;
										selected_count -= 20;
										return;
									}
									else
									{
										selected[0, k] = false;
										selected[14, k] = true;
										sel_par1 = 14;
										sel_par2 = k;
										selected_count += 140;
										return;
									}
								}
							}
						}
						break;

					case ConsoleKey.Enter:
						if(selecting == false)
						{
							if (occupati[sel_par1, sel_par2] == true)
							{
								error = true;
								Refresh();
							}
							else
							{
								error = false;
								selecting = true;
								Refresh();
							}
						}
						break;

					case ConsoleKey.Spacebar:
						checkout = true;
						Refresh();
						break;

				}
			}
			else
			{

			}
		}

		static void Error() // Questa schermata verrà visualizzata qualora il posto selezionato sia già occupato.
		{
			
			Console.WriteLine("");
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine(" Prezzo Complessivo: {0} Euro", total_price);
			Console.ForegroundColor = ConsoleColor.Red;
			Console.Write(" ERRORE! Il posto è già occupato! ");
			Console.ForegroundColor = ConsoleColor.White;
			Console.Write("Selezionane un altro, per favore.");
			Console.WriteLine("");
			Console.WriteLine(" Quando hai scelto il posto, premi il tasto INVIO.");
			Console.WriteLine(" Premere BARRA SPAZIATRICE se vuoi terminare la prenotazione.");
		}
		static void Instructions()
		{
			if(selecting == false)
			{
				Console.ForegroundColor = ConsoleColor.White;
				Console.WriteLine(" ");
				Console.WriteLine(" Prezzo Complessivo: {0} Euro", total_price);
				Console.WriteLine(" Naviga con le freccette per selezionare il posto.");
				Console.WriteLine(" Quando hai scelto il posto, premi il tasto INVIO.");
				Console.WriteLine(" Premere BARRA SPAZIATRICE se vuoi terminare la prenotazione.");
			}
			else
			{
				Console.ForegroundColor = ConsoleColor.White;
				Console.WriteLine("");
				Console.WriteLine(" Prezzo Complessivo: {0} Euro", total_price);
				Console.Write(" Stai selezionando il posto ");
				Console.Write(fila[sel_par1]);
				Console.Write(posto[sel_par2]);
				Console.Write(".");
				Console.WriteLine("");
				Console.WriteLine(" Ne sei sicuro? Premi INVIO per confermare, CANC per annullare.");
				ConsoleKeyInfo keypress;
				keypress = Console.ReadKey();
				switch (keypress.Key)
				{
					case ConsoleKey.Enter:
                        occupati[sel_par1, sel_par2] = true;
						selecting = false;
						count++;
						total_price += price;
						string stringaposto_fila = fila[sel_par1].ToString() + posto[sel_par2].ToString();
						places += stringaposto_fila + ",";
                        File.AppendAllText(path + "\\posti_sala_" + selsala + ".txt", sel_par1.ToString() + "^" + sel_par2.ToString() + Environment.NewLine);
						Refresh();
						break;
					case ConsoleKey.Delete:
						selecting = false;
						Refresh();
						break;
					default:
						break;
				}
			}
		}
		static void Refresh()
		{
			Console.Clear();
			Intestazione(); // Mostra l'intestazione.
			
			for (int i = 0; i < 15; i++) //Visualizza i posti liberi, quelli occupati e quelli selezionati
			{
				Console.Write("                 ");

				for(int k = 0; k < 20; k++)
				{
					if (selected[i, k] == true)
					{
						Console.BackgroundColor = ConsoleColor.White;
						if(occupati[i, k] == true)
						{
							Console.ForegroundColor = ConsoleColor.Red;
						}
						else
						{
							Console.ForegroundColor = ConsoleColor.Black;
						}
						Console.Write(fila[i]);
						Console.Write(posto[k]);
						Console.ResetColor();
						Console.Write(" ");
					}
					else if (occupati[i,k] == false)
					{
						Console.ForegroundColor = ConsoleColor.Green;
						Console.Write(fila[i]);
						Console.Write(posto[k]);
						Console.Write(" ");
						Console.ResetColor();
					}
					else if (occupati[i,k] == true)
					{
						Console.ForegroundColor = ConsoleColor.Red;
						Console.Write(fila[i]);
						Console.Write(posto[k]);
						Console.Write(" ");
						Console.ResetColor();
					}
				}
				Console.WriteLine(" ");
			}
			//La if qui sotto, serve a scegliere quale sottosezione della pagina di prenotazione visualizzare.
			if (error == true)
			{
				Error();
			}
			else if (retry == true)
			{
				Retry();
			}
			else if (checkout == true)
			{
				Checkout();
			}
			else if (count == FULL)
			{
				FullCheck();
			}

			else if (error == false)
			{
				Instructions();
			}
		}

		static void Main(string[] args)
		{    
			int largestWidth = Console.LargestWindowWidth;
			int largestHeight = Console.LargestWindowHeight;
			Console.SetWindowSize(105, 30); // Imposta la grandezza della finestra a 105,30 (righe)
			Console.SetBufferSize(105, 30); // Imposta la grandezza del buffer della finestra a 105,30 (righe)
            credentials = new string[ReadLinesFromFile(path + "\\credentials.txt"), 2]; // Inizializza la variabile "credentials", che è la variabile generale delle credenziali, facendole avere la stessa quantità di colonne del numero di righe del file.
			posto = new int[25]; // Inizializza i posti
			fila = new char[20]; // Inizializza le file
			occupati = new bool[15, 20]; // Inizializza i posti occupati
			selected = new bool[15, 20]; // Inizializza i posti selezionati
			Initialize(); // Questa Funzione serve ad inizializzare le matrici del programma.
			GenerateCredentials(); // Questa Funzione serve a generare le variabili per le credenziali, prendendole dal file "credentials.txt"
            Login(); // Mostra la schermata di Login
			do
			{
				Refresh();
				UserInput();
			} while (end == false); // All'interno di questo ciclo, l'utilizzatore del programma potrà selezionare i posti da prenotare.
			Pause(); // Termina il programma.
		}
	}
}