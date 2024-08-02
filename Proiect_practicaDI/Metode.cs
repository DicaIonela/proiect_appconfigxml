using LibrarieClase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using NivelStocareDate;
using System.Runtime.InteropServices;
using InterfataUtilizator;
//using System.Windows.Forms;
using System.Data.SqlTypes;
using System.IO.Ports;
//using System.Threading;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using System.Timers;
namespace Proiect_practicaDI
{
    public static class Metode
    {
        /*INITIALIZARI PENTRU A PUTEA ASCUNDE CONSOLA LA RULARE*/
        //[DllImport("kernel32.dll")]/*Se importa functii pentru a ascunde/afisa consola*/
        //static extern IntPtr GetConsoleWindow();
        //[DllImport("user32.dll")]
        //static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        //const int SW_HIDE = 0;
        //const int SW_SHOW = 5;
        private static bool shouldContinue = true;
        static SerialPort serialPort;
        static StringBuilder messageBuffer = new StringBuilder();
        static StringBuilder callBuffer = new StringBuilder();
        static StringBuilder dataBuffer = new StringBuilder();
        private static Timer messagePollingTimer;
        static Timer messageTimer;
        [STAThread]
        public static void Start()
        {
            //IntPtr handle = GetConsoleWindow();/*Obtine handle-ul ferestrei consolei*/
            //ShowWindow(handle, SW_HIDE);/*Ascunde consola*/
            ///*Setam aplicatia pentru a folosi Windows Forms*/
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            ///*Afisam fereastra de alegere*/
            //DialogResult result = MessageBox.Show(
            //    "Doriti sa intrati in interfata grafica a aplicatiei?\n\n" +
            //    "*Selectand butonul No alegeti optiunea de a folosi aplicatia in consola.\n",
            //    "Selectați modul de utilizare",
            //    MessageBoxButtons.YesNo,
            //    MessageBoxIcon.Question);
            //if (result == DialogResult.Yes)
            //{
            //    Application.Run(new InterfataGrafica());/*Deschide interfata grafica*/
            //}
            //else
            //{
            //    ShowWindow(handle, SW_SHOW);/*reafiseaza consola*/
                StartCommandPromptMode(); /*Continua cu modul Command Prompt*/
            //}
        }
        public static void Meniu()
        {
            Console.WriteLine("----MENIU----");
            Console.WriteLine("C. Citire utilizator.");
            Console.WriteLine("S. Salvare utilizator.");
            Console.WriteLine("A. Afisare utilizatori din fisier.");
            Console.WriteLine("L. Cautare utilizator dupa nume.");
            Console.WriteLine("M. Afiseaza adresa MAC a acestui PC.");
            Console.WriteLine("E. Sterge un utilizator din fisier.");
            Console.WriteLine("N. Modifica un utilizator deja existent.");
        }
        public static void StartCommandPromptMode()
        {
            Init.Initialize(out Administrare_FisierText admin, out Utilizator utilizatornou); /*initializari*/
            ListenToSerialPort("COM4", 115200);
            StartMessagePolling();
            string optiune;
            do
            {
                //StartMessagePolling();
                optiune = Console.ReadLine();
            } while (optiune != "X");
            //StopMessagePolling();
            //Meniu();/*text meniu*/
            //do
            //{
            //    Console.WriteLine("\nIntrodu optiunea dorita:");
            //    string optiune = Console.ReadLine();
            //    switch (optiune)
            //    {
            //        case "C":
            //            utilizatornou = CitireUtilizatorTastatura();
            //            break;
            //        case "S":
            //            /* Verificare daca a fost introdus un utilizator nou */
            //            if (utilizatornou.Nume != string.Empty)
            //            {
            //                admin.AddUtilizator(utilizatornou);/*daca a fost introdus un utilizator nou, se adauga in fisier*/
            //                Console.WriteLine("Utilizatorul a fost adaugat cu succes.");
            //            }
            //            else
            //            {
            //                Console.WriteLine("Salvare nereusita. Nu ati introdus niciun utilizator nou.");
            //            }
            //            break;
            //        case "A":
            //            Utilizator[] utilizatori = admin.GetUtilizatori(out int nrUtilizatori);/*SE CREEAZA UN TABLOU DE OBIECTE*/
            //            AfisareUtilizatori(utilizatori, nrUtilizatori);
            //            break;
            //        case "L":
            //            Console.WriteLine("Introduceti criteriul de cautare:");
            //            string criteriu = Console.ReadLine();
            //            Utilizator[] utilizatoriGasiti = admin.CautaUtilizator(criteriu);
            //            if (utilizatoriGasiti.Length > 0)
            //            {
            //                AfisareUtilizatori(utilizatoriGasiti, utilizatoriGasiti.Length);
            //            }
            //            else
            //            {
            //                Console.WriteLine("Nu s-au găsit utilizatori care să corespundă criteriului.");
            //            }
            //            break;
            //        case "M":
            //            string adresam = GetMacAddress();
            //            Console.WriteLine("Adresa MAC a calculatorului este: " + adresam);
            //            break;
            //        case "E":
            //            Console.WriteLine("Introdu numele utilizatorului de sters:");
            //            string numedesters = Console.ReadLine();
            //            admin.StergeUtilizator(numedesters);
            //            break;
            //        case "N":
            //            Console.WriteLine("Introduceti numele complet al utilizatorului de modificat.");
            //            string username= Console.ReadLine();
            //            Console.WriteLine("Introduceti noul nume al utilizatorului:");
            //            string nume = Console.ReadLine();
            //            Console.WriteLine("Introduceti noul numar de telefon:");
            //            string numar = Console.ReadLine();
            //            Console.WriteLine("Introduceti noua adresa MAC:");
            //            string adresa = Console.ReadLine();
            //            Utilizator utilizator = new Utilizator(nume, numar, adresa);
            //            admin.UpdateUtilizator(username, utilizator);
            //            break;
            //    }
            //} while (true);
        }

        private static void StartMessagePolling()
        {
            try
            {
                messagePollingTimer = new Timer(5000); // Interval de 500 ms-nu e functional, se suprapune cu apelurile
                messagePollingTimer.Elapsed += OnMessagePollingEvent;
                messagePollingTimer.AutoReset = true;
                messagePollingTimer.Enabled = true;
                messagePollingTimer.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Eroare la deschiderea portului serial: " + ex.Message);
            }
        }

        public static void AfisareUtilizatori(Utilizator[] utilizatori, int nrUtilizatori)
        {
            if (utilizatori.Length > 0)
            {
                Console.WriteLine("Utilizatorii salvati in fisier gasiti sunt:");
                for (int contor = 0; contor < nrUtilizatori; contor++)/*SE PARCURGE TABLOUL DE OBIECTE SI SE AFISEAZA INFORMATIILE IN FORMATUL CORESPUNZATOR*/
                {
                    string infoLocuri = utilizatori[contor].Info();
                    Console.WriteLine(infoLocuri);
                }
            }
            else
                Console.WriteLine("Nu au fost gasiti utilizatori.");
        }
        public static string ValidareSiCorectareNumar(string numar)
        {
            if (numar.StartsWith("+40"))
            {
                if (numar.Length == 13 && numar.Substring(3).All(char.IsDigit))/*Verificare daca numarul are exact 13 caractere si sunt doar cifre dupa +40*/
                {
                    return "0040"+numar.Substring(3);
                }
                else
                {
                    return null; /*Invalid*/
                }
            }
            if (numar.StartsWith("0040"))
            {
                if (numar.Length == 13 && numar.Substring(3).All(char.IsDigit))/*Verificare daca numarul are exact 13 caractere si sunt doar cifre dupa +40*/
                {
                    return numar;
                }
                else
                {
                    return null; /*Invalid*/
                }
            }
            else
            {
                if (numar.Length == 10 && numar.All(char.IsDigit) && numar.StartsWith("0"))/*Verificare daca numarul are exact 10 caractere si sunt doar cifre*/
                {
                    return "004" + numar; /*Adaugam prefixul +4 -> 0 fiind deja inclus*/
                }
                else
                {
                    return null; /*Invalid*/
                }
            }
        }
        public static string ValidareSiFormatareAdresaMac(string adresamac)
        {
            var cleanAddress = new string(adresamac
                .Where(c => "0123456789ABCDEF".Contains(char.ToUpper(c)))
                .ToArray());/*Elimina toate caracterele non-hexadecimale si normalizeaza*/
            if (cleanAddress.Length == 12)/*Verificare daca are exact 12 caractere (6 octeti)*/
            {
                return string.Join("-", Enumerable.Range(0, cleanAddress.Length / 2)
                    .Select(i => cleanAddress.Substring(i * 2, 2)));/*Formateaza in formatul 00-11-22-33-44-55*/
            }
            else
            {
                return null; /*Invalid*/
            }
        }
        public static Utilizator CitireUtilizatorTastatura()
        {
            Console.WriteLine("Introduceti datele utilizatorului:");
            Console.WriteLine("Nume:");
            string nume = Console.ReadLine();
            string numarcitit;
            do
            {
                Console.WriteLine("Numar:");
                numarcitit = Console.ReadLine();
                numarcitit = ValidareSiCorectareNumar(numarcitit);
                if (numarcitit == null)
                {
                    Console.WriteLine("Numarul de telefon introdus nu este valid. Te rugam sa incerci din nou.");
                }
            } while (numarcitit == null);
            string adresamac;
            do
            {
                Console.WriteLine("Adresa MAC (format: 00-11-22-33-44-55):");
                adresamac = Console.ReadLine();
                adresamac = ValidareSiFormatareAdresaMac(adresamac);
                if (adresamac == null)
                {
                    Console.WriteLine("Adresa MAC introdusa nu este valida. Te rugam sa incerci din nou.");
                }
            } while (adresamac == null);
            Utilizator utilizator = new Utilizator(nume, numarcitit, adresamac);
            return utilizator;
        }
        public static string GetMacAddress()
        {
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();/*Obtine lista de placi de retea*/
            foreach (var networkInterface in networkInterfaces)/*Cautam prima placa de retea activa si obtinem adresa MAC*/
            {
                if (networkInterface.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                    networkInterface.OperationalStatus == OperationalStatus.Up)/*Verifica daca placa de retea nu este de tip Loopback si este activs*/
                {
                    var macAddress = networkInterface.GetPhysicalAddress();
                    if (macAddress != null)
                    {
                        return string.Join("-", macAddress.GetAddressBytes().Select(b => b.ToString("X2")));/*Formateaza adresa MAC intr-un sir de caractere hexazecimale*/
                    }
                }
            }
            return "Adresa MAC nu a putut fi gasita";
        }
        static void ListenToSerialPort(string portName, int baudRate)
        {
            serialPort = new SerialPort(portName);
            /*Setarile portului serial cu valori implicite pentru parametrii care nu sunt specificati*/
            serialPort.BaudRate = baudRate;
            serialPort.Parity = Parity.None; /* Fara paritate*/
            serialPort.DataBits = 8; /*8 biti de date*/
            serialPort.StopBits = StopBits.One; /*Un bit de stop*/
            serialPort.Handshake = Handshake.None; /*Fara control al fluxului*/
            /*Evenimentul care se declanseaza cand se primesc date*/
            serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            serialPort.Open();
            if(serialPort!=null && serialPort.IsOpen)
            {
                Console.WriteLine("Device is listening:");
            }
            SendCommand("AT+CLIP=1");
        }
        private static void OnMessagePollingEvent(object sender, ElapsedEventArgs e)
        {
            // Trimite comanda pentru a verifica mesajele necitite
            SendCommand("AT+CMGF=1");
            //WaitForOkResponse();
            SendCommand("AT+CMGL=\"REC UNREAD\"");
            //WaitForOkResponse();
        }
        private static void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            System.Threading.Thread.Sleep(500);
            string indata = sp.ReadExisting();

            dataBuffer.Append(indata);

            if ((indata.Contains("REC UNREAD") || indata.Contains("+CMGL"))&& !indata.Contains("CLIP"))
            {
                //messageBuffer.Append(indata);
                ProcessMessageBuffer();  // Procesează datele de mesaj
                dataBuffer.Clear();
            }
            else if (indata.Contains("+CLIP"))
            {
                //callBuffer.Append(indata);
                ProcessBufferCall();  // Procesează datele de apel
                //SendCommand("AT");
                dataBuffer.Clear();
            }
        }
        private static string ExtractCallerNumber(string message)
        {
            /*Extrage numarul apelantului din mesajul +CLIP*/
            int startIndex = message.IndexOf('"') + 1;
            int endIndex = message.IndexOf('"', startIndex);
            if (startIndex > 0 && endIndex > startIndex)
            {
                return message.Substring(startIndex, endIndex - startIndex);
            }
            return string.Empty;
        }

        private static void ProcessBufferCall()
        {
            Init.Initialize(out Administrare_FisierText admin, out Utilizator utilizatornou); /*initializari*/
            string delimiter = "\nRING";/*delimitator pentru a verifica daca buffer ul a stocat toate datele complete*/
            while (dataBuffer.ToString().Contains(delimiter))
            {
                /*Extrage mesajul complet din buffer*/
                int delimiterIndex = dataBuffer.ToString().IndexOf(delimiter);
                string completeMessage = dataBuffer.ToString().Substring(0, delimiterIndex);
                /*Elimina mesajul complet din buffer*/
                dataBuffer.Remove(0, delimiterIndex + delimiter.Length);
                string cleanedMessage = completeMessage.Trim();
                string callerNumber = ExtractPhoneNumber(cleanedMessage);
                if (cleanedMessage.Contains("+CLIP:"))
                {
                    Console.WriteLine("Date primite: " + cleanedMessage+"\nNumar:"+callerNumber);
                    do
                    {
                        System.Threading.Thread.Sleep(1000);
                        SendCommand("ATH");
                    } while (WaitForOkResponse() != true);
                    
                    if (admin.CautaUtilizator(callerNumber) != null&&callerNumber.Length>11)
                    {
                        Utilizator[] utilizatorigasiti = admin.CautaUtilizator(callerNumber);
                        AfisareUtilizatori(utilizatorigasiti, utilizatorigasiti.Length);
                        if(utilizatorigasiti.Length==1)
                        {
                            try
                            {
                                // Trimiterea pachetului WoL
                                SendWakeOnLan(utilizatorigasiti[0].AdresaMAC);
                                Console.WriteLine("Pachetul WoL a fost trimis.");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Eroare la trimiterea pachetului WoL: " + ex.Message);
                            }
                        }

                        else
                            Console.WriteLine("Nu s-a trimis niciun pachet. Acces neautorizat.");
                    }
                    SendCommand("AT");
                    WaitForOkResponse();
                }
            }
        }



        /*DE LUCRAT LA PRELUCRAREA DATELOR PRIMITE*/
        private static void ProcessMessageBuffer()
        {
            //string delimiter = "\n"; // Delimitator pentru mesaje
            //string data = dataBuffer.ToString();
            //string[] lines = data.Split(new[] { delimiter }, StringSplitOptions.RemoveEmptyEntries);
            //dataBuffer.Clear(); // Resetează buffer-ul pentru a procesa datele viitoare
            //foreach (string line in lines)
            //{
            //    if ( line != "OK")
            //    {
            //        if (line.StartsWith("+CMGL:") || line.Contains("UNREAD"))
            //        {
            //            // Linia care începe cu +CMGL conține informații despre mesaj
            //            // Extragem detaliile mesajului din linia următoare
            //            Console.WriteLine(line);
            //            string[] parts = line.Split(',');
            //            if (parts.Length >= 3)
            //            {
            //                string phoneNumber = ExtractPhoneNumberFromParts(parts);
            //                Console.WriteLine("Număr: " + phoneNumber);
            //            }
            //        }
            //        else
            //        {
            //            Console.WriteLine(line);
            //            // Linia care urmează liniei +CMGL conține mesajul efectiv
            //            if (!string.IsNullOrWhiteSpace(line) && line.Trim() != "OK"&& line.Trim() != "CMGL")
            //            {
            //                string message = line.Trim();
            //                Console.WriteLine("Mesaj: " + message);
            //            }
            //        }
            //    }

            //}
            string delimiter = "+CMGL:"; // Delimitator pentru mesaje
            string data = dataBuffer.ToString();
            string[] dateseparate = data.Split(new[] { delimiter }, StringSplitOptions.RemoveEmptyEntries);
            dataBuffer.Clear(); // Resetează buffer-ul pentru a procesa datele viitoare
            foreach (string datemesaj in dateseparate)
                {
                Console.WriteLine("Date mesaj:"+datemesaj);
                string delimiter_mesaj = "\n";
                string[] parti = datemesaj.Split('\n');
                string[] parts = parti[0].Split(',');
                string phoneNumber = ExtractPhoneNumberFromParts(parts);
                Console.WriteLine("Număr: " + phoneNumber);
                if (parti.Length > 1)
                {
                    string message = parti[1];
                    Console.WriteLine("Mesaj: " + message);
                }
                else
                    Console.WriteLine("nu merge");
            }
         }

        private static string ExtractPhoneNumberFromParts(string[] parts)
        {
            if (parts.Length >= 3)
            {
                string phoneNumberPart = parts[2].Trim('"');
                phoneNumberPart = FormatPhoneNumber(phoneNumberPart);
                return phoneNumberPart;
            }
            return string.Empty;
        }
        private static string FormatPhoneNumber(string number)
        {
            // Înlătură prefixul "+" și transformă în "004"
            if (number.StartsWith("+40"))
            {
                return "0040" + number.Substring(3); // Formatează în "00407"
            }
            else if (number.StartsWith("40"))
            {
                return "0040" + number.Substring(2); // Formatează în "00407"
            }
            else if (number.StartsWith("07") && number.Length == 10)
            {
                return "0040" + number; // Adaugă prefixul "0040"
            }
            else
            {
                // Returnează numărul original dacă nu se potrivește niciun format specificat
                return number;
            }
        }
        static void SendWakeOnLan(string macAddress)
        {
            /*Convertim adresa MAC intr-un array de octeti*/
            byte[] macBytes = GetMacBytes(macAddress);
            /*Construim pachetul magic WoL*/
            byte[] packet = new byte[102];
            /*Primele 6 octeti trebuie sa fie 0xFF*/
            for (int i = 0; i < 6; i++)
            {
                packet[i] = 0xFF;
            }
            /*Urmatorii 16 * 6 octeti trebuie sa fie adresa MAC repetata de 16 ori*/
            for (int i = 1; i <= 16; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    packet[i * 6 + j] = macBytes[j];
                }
            }
            /*Trimiterea pachetului prin UDP*/
            using (UdpClient client = new UdpClient())
            {
                client.Connect(IPAddress.Broadcast, 9); // Portul 9 este utilizat de obicei pentru WoL
                client.Send(packet, packet.Length);
            }
        }
        static byte[] GetMacBytes(string macAddress)
        {
            string[] macParts = macAddress.Split('-');
            if (macParts.Length != 6)
            {
                throw new ArgumentException("Format de adresa MAC invalid.");
            }
            byte[] macBytes = new byte[6];
            for (int i = 0; i < 6; i++)
            {
                macBytes[i] = Convert.ToByte(macParts[i], 16);
            }
            return macBytes;
        }
        private static void SendCommand(string command)
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                try
                {
                    /*Adauga o noua linie dupa comanda AT*/
                    serialPort.WriteLine(command + "\r");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Eroare la trimiterea comenzii: " + ex.Message);
                }
            }
        }
        private static bool WaitForOkResponse()
        {
            serialPort.DataReceived -= new SerialDataReceivedEventHandler(DataReceivedHandler);
            string okResponse = "OK\r"; /*Raspunsul asteptat*/
            string receivedData = string.Empty;
            int attemptCnt = 0;
            int attemptLimit = 5;
            /*Continua să verifice pana cand gaseste confirmarea OK*/
            while (true && attemptCnt++<attemptLimit)
            {
                /*Verifica daca exista date disponibile in buffer*/
                if (serialPort.BytesToRead > 0)
                {
                    /*Citeste datele din portul serial*/
                    receivedData += serialPort.ReadExisting();
                    /*Verifica daca datele contin confirmarea OK*/
                    if (receivedData.Contains(okResponse))
                    {
                        //Elimina confirmarea OK din bufferul de date*/
                        int okIndex = receivedData.IndexOf(okResponse);
                        receivedData = receivedData.Substring(okIndex + okResponse.Length);
                        Console.WriteLine("Confirmare primită: OK");
                        serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                        return true; /*Iese din bucla dupa ce a primit confirmarea*/
                    }
                }
                /*Asteapta putin inainte de a verifica din nou*/
                System.Threading.Thread.Sleep(100); /*100 ms pentru a preveni consumul excesiv de CPU*/
            }
            serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            return false;
        }
        private static string ExtractPhoneNumber(string message)
        {
            /*Cauta inceputul si sfarsitul numarului de telefon intre ghilimele*/
            int startIndex = message.IndexOf('"') + 1;
            int endIndex = message.IndexOf('"', startIndex);
            /*Verifica daca indecsii sunt valizi*/
            if (startIndex > 0 && endIndex > startIndex)
            {
                return message.Substring(startIndex, endIndex - startIndex);
            }
            return string.Empty; /*Returneaza un sir gol daca numarul nu a fost gasit*/
        }
    }
}