using System;
using System.Data.SqlClient;
using System.Globalization;

namespace Esercitazione20_11
{
    public class ConnectedMode
    {
        const string connectionString = @"Persist Security Info=false; Integrated Security=true; Initial Catalog = Polizia; Server = WINAP4R3GZJOYFF\SQLEXPRESS";
        
        //metodo per selezionare gli agenti in base all'area
        public static void Connected()
        {
         
            using (SqlConnection connection = new SqlConnection(connectionString))  // Creo connessione
            {

                // voglio leggere parametro da riga di comando
                Console.WriteLine(" Inserire Codice Area: ");
                string codice;
                codice = Console.ReadLine();


                // Apro Connessione
                connection.Open();



                // Creo un command 
                SqlCommand command = new SqlCommand(); 
                command.Connection = connection;
                command.CommandType = System.Data.CommandType.Text; 
                command.CommandText = "SELECT Agente.ID, Agente.Nome, Agente.Cognome, Agente.CodiceFiscale, Agente.DatadiNascita, Agente.AnniServizio, Area.Codice, Area.AltoRischi" +
                    " FROM Agente " +
                    "INNER JOIN Servizio " +
                    "ON Agente.ID=Servizio.AgenteID " +
                    "INNER JOIN Area " +
                    "ON Area.ID = Servizio.AreaID " +
                    "WHERE Area.Codice = @AreaCodice";



                // Creo parametro
                command.Parameters.AddWithValue("@AreaCodice", codice);


                // Eseguo comando
                // dato che mi ridà una tabella uso DataReader --> execute reader
                SqlDataReader reader = command.ExecuteReader(); 



                // Leggo dati a schermo
                
                while (reader.Read()) 
                { 
                    Console.WriteLine("ID: {0},   Agente: {1} {2},   CF: {3},   Data di Nascita: {4},   Anni di Servizio: {5}",
                        reader["ID"],
                        reader["Nome"],
                        reader["Cognome"],
                        reader["CodiceFiscale"],
                        reader["DatadiNascita"],
                        reader["AnniServizio"]);
                       //reader["Codice"], se volessi anche info area
                      //  reader["AltoRischi"]);
                }



                // 6. chiudo reader e connessione
                reader.Close();
                connection.Close();
            }



        }

        //metodo per inserire un record di agente
        public static void ConnectedInsert()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))  //creo connessione
            {

                // voglio leggere parametro da riga di comando
                Console.WriteLine(" Inserire Nome Agente,Cognome Agente,Codice Fiscale,Data di Nascita,Anni di servizio: ");
                
                //split di quello che ho ricevuto
                string ricevuto = Console.ReadLine();
                char[] chararray = new char[1];
                chararray[0] = ',';
                String[] resultarray = ricevuto.Split(chararray);

                string nome;
                string cognome;
                string CF;
                string DOB;
                string anniservizio;

                nome = resultarray[0];
                cognome = resultarray[1];
                CF = resultarray[2];
                DOB = resultarray[3];
                anniservizio = resultarray[4];



                //apro connessione
                connection.Open();



                // Creo command 
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = "INSERT INTO Agente " +
                    "VALUES (@Nome, @Cognome, @CodiceFiscale, @DatadiNascita, @AnniServizio)";


                // creo parametri

                SqlParameter nomeParam = new SqlParameter();
                SqlParameter cognomeParam = new SqlParameter();
                SqlParameter CFParam = new SqlParameter();
                SqlParameter DOBParam = new SqlParameter();
                SqlParameter anniservizioParam = new SqlParameter();

                //nome parametri
                nomeParam.ParameterName = "@Nome";
                cognomeParam.ParameterName = "@Cognome";
                CFParam.ParameterName = "@CodiceFiscale";
                DOBParam.ParameterName = "@DatadiNascita";
                anniservizioParam.ParameterName = "@AnniServizio";

                //valore parametri
                nomeParam.Value = nome;
                cognomeParam.Value = cognome;
                CFParam.Value = CF;
                DOBParam.Value = DateTime.ParseExact(DOB, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                anniservizioParam.Value = Int32.Parse(anniservizio);

                // 
                nomeParam.SqlDbType = System.Data.SqlDbType.NVarChar;
                nomeParam.Size = 30;

                cognomeParam.SqlDbType = System.Data.SqlDbType.NVarChar;
                cognomeParam.Size = 50;


                CFParam.SqlDbType = System.Data.SqlDbType.NVarChar;
                CFParam.Size = 16;


                //aggiunta parametri
                command.Parameters.Add(nomeParam); 
                command.Parameters.Add(cognomeParam); 
                command.Parameters.Add(CFParam);
                command.Parameters.Add(DOBParam);
                command.Parameters.Add(anniservizioParam);


                // Eseguo il command (non mi aspetto niente di ritoro, quindi ExecuteNonQuery)
                command.ExecuteNonQuery(); // avrei potuto ricavarmi le righe affected mettendo int number of rows = 


                // chiusura connessione
                connection.Close();

            }

        }

        //metodo per visualizzare tutti i record di agente e vedere se ho inserito correttamente
        public static void ConnectedCheck ()
        {
           
            using (SqlConnection connection = new SqlConnection(connectionString)) //creo connessione
            {
               // apro connessione
                connection.Open();


                // Creo command 
                SqlCommand command = new SqlCommand();  
                command.Connection = connection;
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = "SELECT * FROM Agente";

                //eseguo command (mi aspetto tabella qindi datareader)
                SqlDataReader reader = command.ExecuteReader(); 

                //Leggo dati a schermo
                while (reader.Read()) 
                {
                    Console.WriteLine("ID: {0},   Agente: {1} {2},   CF: {3},   Data di Nascita: {4},   Anni di Servizio: {5}",
                        reader["ID"],
                        reader["Nome"],
                        reader["Cognome"],
                        reader["CodiceFiscale"],
                        reader["DatadiNascita"],
                        reader["AnniServizio"]);
                }

               // chiudo reader e connessione
                reader.Close();
                connection.Close();
            }



        
        }
    }
}
