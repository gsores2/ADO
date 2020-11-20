using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace ADO
{
    public class ConnectedMode // serve dappertutto 
    {
        // stringa di connessione mi serve per tutti i metodi
        
        const string connectionString = @"Persist Security Info=false; Integrated Security=true; Initial Catalog = CinemaDb; Server = WINAP4R3GZJOYFF\SQLEXPRESS";
    
        // persist security info mi dice se voglio salvare o no la password ( per future riconnessioni)
        // integrated security sto accedendo tramite windows authentication  ( se metto false qui devo dare username e password)
        // initial catalog nome db
        // server è nome server 
        


        // creo un metodo per fare una connected mode che seleziona tutti i film
        public static void Connected()
        {
            // Creare Connessione (esistono più metodi)

            // Metodo 1: 
            //SqlConnection connection = new SqlConnection();
            //connection.ConnectionString = connectionString; // setto proprietà dell'oggetto connection

            // Metodo 2: 
            //SqlConnection connection = new SqlConnection(connectionString); // overload del costruttore 

            // Metodo 3
            using (SqlConnection connection = new SqlConnection(connectionString))  //1. Creare connessione
            {
                // 2. Aprire Connessione
                connection.Open();



                // 3. Creare un command 
                SqlCommand command = new SqlCommand(); // esiste un overload per metterlo già nel costruttore, METTO TRA PARENTESI "select * from movies", connection)
                command.Connection = connection; // esiste ache connection.CreateCommand
                command.CommandType = System.Data.CommandType.Text; // select, insert ecc (sempre tranne che per le stored procedure)
                command.CommandText = "SELECT * FROM Movies";
               


                // 3.bis Creare eventualmente dei parametri (qui non ne ho)



                // 4. Eseguire il command (DataReader, NonQuery, Scalar)
                // dato che mi ridà una tabella uso DataReader --> execute reader
                SqlDataReader reader = command.ExecuteReader(); // lui ha i dati
                


                // 5. Leggere i dati a schermo
                // devo fare un ciclo perchè li legge riga per riga i dati dalla tabella ritornata
                while (reader.Read()) // finchè li sta leggendo 
                { // dopo averlo letto ci conviene chiudere il reader
                    Console.WriteLine("{0} - {1} {2} {3}", 
                        reader["ID"],
                        reader["Titolo"],
                        reader["Genere"],
                        reader["Durata"]);
                }



                // 6. Chiudere l'eventuale reader e la connessione 
                reader.Close();
                connection.Close();
            }



        }



        // creo metodo che filtra i film per genere (gli devo dare un parametro)
        public static void ConnectedWithParameter()
        {
            using (SqlConnection connection = new SqlConnection(connectionString)) // 1. Creare connessione
            {

                // voglio leggere parametro da riga di comando
                Console.WriteLine(" Inserire il Genere del Film: ");
                string genere;
                genere = Console.ReadLine();


                // 2. Aprire Connessione
                connection.Open();


                // 3. Creare un command 
                SqlCommand command = new SqlCommand(); //posso averne più id uno
                command.Connection = connection; 
                command.CommandType = System.Data.CommandType.Text; 
                command.CommandText = "SELECT * FROM Movies WHERE Genere=@genere"; // Genere è un parametro quindi vuole la @


                // 3bis. Creare eventualmente il parametro che mi serve

                //SqlParameter genereParam = new SqlParameter(); // va a farmi da parametro questo oggetto
                //genereParam.ParameterName = "@Genere"; //@Genere deve sapere che è un parametro 
                //genereParam.Value = genere; // quello che gli passo io
                //command.Parameters.Add(genereParam); // qui aggiungo il parametro al mio comando specifico

                // in alternativa alle 2 righe commetate
                command.Parameters.AddWithValue("@Genere", genere); // Vuole il nome del parametro e poi il valore del parametro 



                // 4. Eseguire il command
                // dato che mi ridà una tabella  uso DataReader

                SqlDataReader reader = command.ExecuteReader(); // lui ha i dati

                // 5.Leggere i dati 
                // devo fare un ciclo perchè li legge riga per riga i dati dalla tabella ritornata

                while (reader.Read()) // finchè li sta leggendo 
                {
                    Console.WriteLine("{0} - {1} {2} ",
                        reader["ID"],
                        reader["Titolo"],
                        reader["Genere"]);
                }


                // 6. Chiudere la connessione 
                reader.Close();
                connection.Close();
            }
        }



        // creo un metodo con una stored procedure (select actors by cachet range)
        public static void ConnectedStoredProcedure()
        {
            using (SqlConnection connection = new SqlConnection(connectionString)) //1. Creare Connessione
            {
                // 2. Aprire Connessione
                connection.Open();


                // 3. Creare un command 
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandType = System.Data.CommandType.StoredProcedure; // STORED PROCEDURE
                command.CommandText = "stpGetActorByCachetRange"; // nome stored procedure (la va a cercare)
                // dopo aver eseguito questa riga capisce che gli servono tre parametri


                // 3BIS. Creare eventualmente dei parametri (ne aveva 2 in input e 1 in output la stored procedure)
                command.Parameters.AddWithValue("@min_cachet", 5000); // DEVO USARE GLI STESSI NOMI
                command.Parameters.AddWithValue("@max_cachet", 9000);
                // in questo caso ho anche un valore di ritorno (nella stored procedure avevo un count che devo creare come valore di ritorno)
                // lo definisco come parametro de lo voglio considerare 
                SqlParameter returnValue = new SqlParameter();
                returnValue.ParameterName = "@returnedCount"; //STESSO NOME CHE IN SMSS
                returnValue.SqlDbType = System.Data.SqlDbType.Int;// dovrebbe comunqe capirlo da solo (è necessario se ho nvarchar perchè devo dare lunghezza)
                // direction definisce se un parametro è inputo outputù


                //IMPORTANTE
                returnValue.Direction = System.Data.ParameterDirection.Output;// così gli dico che questo è un parametro di output (altrimenti lo prenderebbe come input)
                command.Parameters.Add(returnValue);




                // 4. Eseguire il command
                // mi ridà due cose: da una parte una tabella per cui uso DataReader

                // ho un oggetto che legge
                SqlDataReader reader = command.ExecuteReader(); // lui ha i dati

                while (reader.Read()) // finchè li sta leggendo 
                { // dopo averlo letto ci conviene chiudere il reader
                    Console.WriteLine("{0} - {1} {2} {3}",
                        reader["ID"],
                        reader["FirstName"],
                        reader["LastName"],
                        reader["Cachet"]);
                }

                // // chiude oggeto reader
                reader.Close();


                // se invece mi serve solo il conteggio posso non ettere tutta la parte dell'executre reader e 
                // fare direttamente command.ExecuteNonQuery();

                // la seconda cosa che mi ridà è un parametro di ritorno che ha salvato 
                Console.WriteLine("#Actors: {0}", command.Parameters["@returnedCount"].Value);
              
                // Chiudere la connessione
                connection.Close();

              
            }



        }



        // creo un metodo che restituisca uno scalare
        public static void ConnectedScalar()
        {
            using (SqlConnection connection = new SqlConnection(connectionString)) //1. Creo connessione
            {
                // 2. Aprire Connessione
                connection.Open();

                // 3. Creare un command 
                SqlCommand scalarcommand = new SqlCommand(); 
                scalarcommand.Connection = connection; 
                scalarcommand.CommandType = System.Data.CommandType.Text; // voglio uno scalare quindi text va bene 
                scalarcommand.CommandText = "SELECT COUNT(*) FROM Movies"; // sto cercando uno scalare quindi faccio funzione aggregante

               
                // 4. Eseguire il command
                // ho un oggetto che legge, in questo caso non deve essere di tipo datareader perchè ho un numero ma un int
                int count = (int) scalarcommand.ExecuteScalar(); // mi dà errore se non lo casto da oggetto con 1 riga a 1 colonna a intero
                                                                 // execute reader ritorna ciò che ho in prima riga e prima colonna 


                //// 5. Leggere i dati 
                Console.WriteLine("Conteggio dei film: {0}", count);
                        
               
   
                // 6. Chiudere la connessione
                connection.Close();
            }

            }

        
        }
   
}
