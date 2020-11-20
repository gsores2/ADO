using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace ADO
{
    public class DisconnectedMode
    {
        const string connectionString = @"Persist Security Info=false; Integrated Security=true; Initial Catalog = CinemaDb; Server = WINAP4R3GZJOYFF\SQLEXPRESS";
        
        public static void Disconnected()
        {
            using (SqlConnection connection = new SqlConnection(connectionString)) // 1. creo connessione
            {
                // 2. Costruzione Adapter (1 per ogni dataset, poi posso farci diversi comandi, anche due insert diversi)
                SqlDataAdapter adapter = new SqlDataAdapter();

                

                // 3. creazione comandi da associare all'adapter
                // a. select
                SqlCommand selectCommand = new SqlCommand();
                selectCommand.Connection = connection;
                selectCommand.CommandType = System.Data.CommandType.Text;
                selectCommand.CommandText = "SELECT * FROM Movies"; 
                // PER INSERIRE NUOVI FILM DEVO SELEZIONARE TUTTI I FILM PER VEDERLI E POI UN INSERT
                // select mi serve per far si che adapter sappia come sono fatti i dati nel database e che me li faccia bedere 
                // anche per riconciliare alla fine 

                // b. insert
                SqlCommand insertCommand = new SqlCommand();
                insertCommand.Connection = connection;
                insertCommand.CommandType = System.Data.CommandType.Text;
                insertCommand.CommandText = "INSERT INTO Movies VALUES (@Titolo, @Genere, @Durata)"; // gli do dei parametri 
                



                // 3b. ora devo definire i paramteri percheè lui capisca che quello che ho dopo la chiocciola sono paramteri
                insertCommand.Parameters.Add("@Titolo", System.Data.SqlDbType.NVarChar, 255, "Titolo"); // sono nvarchar quindi non posso usare add with values 
                insertCommand.Parameters.Add("@Genere", System.Data.SqlDbType.NVarChar, 255, "Genere"); // nome parametro, tipo, nome colonna ("campo")
                insertCommand.Parameters.Add("@Durata", System.Data.SqlDbType.Int, 500, "Durata");// definisco un max per il numero di minuti 

                // datatype di campo e parametro deve corrispondere


                // per completezza dovrei fare anche delete e update (posso evitarlo per demo)



                // 4. associazione comandi che ho creato all'adapter
                adapter.SelectCommand = selectCommand;
                adapter.InsertCommand = insertCommand; // a sx dell'uguale ho lo stesso sempre, se voglio definire più insert devo definirlo in diversi punti 
                // devono agire in momenti diversi


                // 5. creazione dataset 
                DataSet dataset = new DataSet(); // dataset sta in locale, non gli interessa quale sia la connessione 
                try
                {    // 6. scarica la tabella dal database che gli ho dato con la connection string
                    connection.Open();


                    adapter.Fill(dataset, "Movies"); //7. riempio dataset: dò quello che sta in Movies (tabella da cui leggere) al dataset
                    // nel fill mi serve la SELECT (legge il comando che ho creato)

                    foreach ( DataRow row in dataset.Tables["Movies"].Rows) // per stampare a video
                    {
                        Console.WriteLine("row: {0}", row["Titolo"].ToString());
                    }


                    // da qui sto in locale
                    // 8 creazione Record (definisco una riga)
                    DataRow movie = dataset.Tables["Movies"].NewRow(); // definisco riga vuota
                    
                    movie["Titolo"] = "V per Vendetta"; // riempio la riga (uso questi dati per popolare i parametri dell'insert)
                    movie["Genere"] = "Azione";
                    movie["Durata"] = 125;

                    // 9. aggiungo riga al dataset (a datatable che si chiama movies)

                    dataset.Tables["Movies"].Rows.Add(movie);

                    // 10. update del db tramite adapter
                    adapter.Update(dataset, "Movies"); // qui dentro usa i suoi metodi adapter.select, adapter.insert ecc dopo aver guardato il db iniziale e aver cercato le differenze
                    // è qui che esegue i comandi dopo aver conrollato il database originale e aver trovato le differenze con i cambiamenti in local
                    // update=aggiornamento in senso lato del database leggendo i comani che ho creato
                }

                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                finally
                {
                    connection.Close();
                }
            }
        }
    }
}
