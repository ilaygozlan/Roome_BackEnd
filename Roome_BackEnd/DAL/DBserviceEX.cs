using System.Dynamic;
using System.Data.SqlClient;
using System.Data;
using static System.Net.Mime.MediaTypeNames;
using System.Net;

namespace serverSide.DAL
{
    public class DBserviceBookClub
    {
        public DBserviceBookClub() { }

        //--------------------------------------------------------------------------------------------------
        // This method creates a connection to the database according to the connectionString name in the web.config 
        //--------------------------------------------------------------------------------------------------
        public SqlConnection connect(String conString)
        {

            // read the connection string from the configuration file
            IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json").Build();
            string cStr = configuration.GetConnectionString("myProjDB");
            SqlConnection con = new SqlConnection(cStr);
            con.Open();
            return con;
        }

        //--------------------------------------------------------------------------------------------------
        // This method adds a new Club
        //--------------------------------------------------------------------------------------------------
        public int AddNewBookClub(string clubName, int userId, int bookId)
        {

            SqlConnection con;
            SqlCommand cmd;
            SqlParameter returnValue = new SqlParameter(); // add a return value parameter
            try
            {
                con = connect("myProjDB"); // create the connection
            }
            catch (Exception ex)
            {
                // write to log
                throw (ex);
            }

            cmd = CreateCommandWithStoredProcedureAddNewBookClub("creatNewBookClub", con, clubName, userId, bookId);             // create the command

            returnValue.ParameterName = "@RETURN_VALUE";
            returnValue.Direction = ParameterDirection.ReturnValue;
            cmd.Parameters.Add(returnValue);

            try
            {
                cmd.ExecuteNonQuery(); // execute the command

                int numEffected = (int)cmd.Parameters["@RETURN_VALUE"].Value; // get the return value
                return numEffected;
            }
            catch (Exception ex)
            {
                // write to log
                throw (ex);
            }

            finally
            {
                if (con != null)
                {
                    // close the db connection
                    con.Close();
                }
            }

        }


        //---------------------------------------------------------------------------------
        // Create the SqlCommand using a stored procedure to  adds a new Club
        //---------------------------------------------------------------------------------

        private SqlCommand CreateCommandWithStoredProcedureAddNewBookClub(String spName, SqlConnection con, string clubName, int userId, int bookId)
        {

            SqlCommand cmd = new SqlCommand(); // create the command object

            cmd.Connection = con;              // assign the connection to the command object

            cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 

            cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds

            cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

            cmd.Parameters.AddWithValue("@userId", userId);

            cmd.Parameters.AddWithValue("@title", clubName);

            cmd.Parameters.AddWithValue("@bookId", bookId);


            return cmd;
        }
    }