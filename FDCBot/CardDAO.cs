using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDCBot
{
    public class CardDAO
    {

        private static volatile SqlConnection connection;

        private static SqlConnection GetConnection()
        {
              //  if(connection == null)
             //   {
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                    builder.DataSource = "fdchackathon.database.windows.net";
                    builder.UserID = "hackadmin";
                    builder.Password = "H@ckathon";
                    builder.InitialCatalog = "carddb";
                    connection = new SqlConnection(builder.ConnectionString);
            //    }
                return connection;
        }


        public static Boolean isValidCard(String lastFourDigits)
        {
            Boolean cardExists = false;
            try { 
                using (SqlConnection connection = GetConnection())
                {
                    connection.Open();

                    String query = "SELECT * from [dbo].[Card_Info] c where c.Card_Last_Four_Digits = " + lastFourDigits;

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine("{0} {1}", reader.GetString(0), reader.GetString(1));
                                cardExists = true;
                                break;
                            }
                        }
                    }

                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                if(connection != null)
                {
                    connection.Close();
                }
            }
            return cardExists;
        }
        
        public static Boolean isValidCvvForCard(String lastFourDigitsOfCard, String cvv)
        {
            Boolean validCvvForCard = false;
            try
            {
                using (SqlConnection connection = GetConnection())
                {
                    connection.Open();

                    String query = "SELECT * from [dbo].[Card_Info] c where c.Card_Last_Four_Digits = " + lastFourDigitsOfCard +
                        " AND c.Card_Security_Code = " + cvv;

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine("{0} {1}", reader.GetString(0), reader.GetString(1));
                                validCvvForCard = true;
                                break;
                            }
                        }
                    }

                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
            return validCvvForCard;
        }

        public static Boolean isValidSsnForCard(string lastFourDigitsOfCard, string cvv, string ssn)
        {

            Boolean validSsnForCard = false;
            try
            {
                using (SqlConnection connection = GetConnection())
                {
                    connection.Open();

                    String query = "SELECT * from [dbo].[Card_Info] c where c.Card_Last_Four_Digits = " + lastFourDigitsOfCard +
                        " AND c.Card_Security_Code = " + cvv + " AND c.SSN_Last_Four_Digits = " + ssn;

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine("{0} {1}", reader.GetString(0), reader.GetString(1));
                                validSsnForCard = true;
                                break;
                            }
                        }
                    }

                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
            return validSsnForCard;
        }

        public static Boolean activateCard(string lastFourDigitsOfCard)
        {
            return true;
        }
    }




}
