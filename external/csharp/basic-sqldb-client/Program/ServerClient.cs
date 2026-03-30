using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq.Expressions;
using System.Reflection;
using System.IO;
using System.Security.Cryptography;

namespace SQLDatabase.Net.Server.Client {

	public class DBColumnAttribute : Attribute {
		//Example 1 : [DBColumn(AutoIncrement = true, PrimaryKey = true)]
		//Example 2 : [DBColumn]
		//Example 3 : [DBColumn(Unique = true)]
		//Example of foreign key : [DBColumn (IsForeignKey = true, ForeignKeyTable = "Jobs", ForeignKeyColumn = "JobId")]
		public bool PrimaryKey { get; set; }
		public bool CombinedPrimaryKey { get; set; }
		public bool Unique { get; set; }
		public bool NotNull { get; set; }
		public bool AutoIncrement { get; set; }
		public bool ForeignKey { get; set; }
		public string ForeignKeyTable { get; set; }
		public string ForeignKeyColumn { get; set; }
	}

	sealed class BindChanger : System.Runtime.Serialization.SerializationBinder {
		public override Type BindToType(string assemblyName, string typeName) {   
			Type typeToDeserialize = null;            
			string currentAssembly = Assembly.GetExecutingAssembly().FullName;
			typeToDeserialize = Type.GetType(string.Format("{0}, {1}", typeName, currentAssembly));
			return typeToDeserialize;
		}
	}

	public enum DatabaseResponseFormats {
		None,
		Binary,
		XML,
		JSON
	}

	public enum ServerCommands {
		ExecuteReader,
		ExecuteNonQuery,
		ExecuteScalar,
		CacheGet,
		CacheAdd,
		CacheUpdate,
		CacheAddOrUpdate,
		CacheRemove,
		CacheSearch,
		CacheCollectionCacheIds,
		CacheCollectionCount,
		CacheCollectionList,
		CacheDropCollection
	}

	public enum ConnectionState {
		Open,
		Close,
		Sent,
		Wait,
	}

	public enum CacheDurations {
		Minutes,
		Hours,
		Days,
		Months,
		Years,
	}

	public class SQLDatabaseConnection {
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected void Dispose(bool disposing) {
			if (disposing) {
				Close();
				tcpClient = null;
			}
		}

		private TcpClient tcpClient = new TcpClient();
        
		const string EndOfLine = "\0<EOL>\0";
		const string EndOfHeader = "\0<EOH>\0";
		const string EndOfMessage = "\0<EOF>\0";
		const string ClientVersion = "2.0.0.0";
		private string[] DatabaseCommands = {
			"ExecuteNonQuery",
			"ExecuteScalar" ,
			"ExecuteReader"
		};
		private string[] CacheCommands = {
			"CacheGet", "CacheAdd", "CacheAddOrUpdate", "CacheUpdate"
                , "CacheRemove", "CacheSearch", "CacheCollectionCacheIds", "CacheCollectionCount"
                , "CacheCollectionList", "CacheDropCollection"
		};
		private ReaderWriterLockSlim StreamRWLS = new ReaderWriterLockSlim();
		private bool _IsAuthenticated = false;
		byte[] saltBytes = new byte[] {
			127,
			117,
			25,
			56,
			59,
			100,
			36,
			11,
			84,
			67,
			96,
			10,
			24,
			111,
			112,
			38
		};
		byte[] passwordBytes;
		private string InFlightAES256Password = "BytesSentOverNetworkEncrypionDecryptionPassword";
		private bool EncryptConnection = false;

		public string ServerCommand { get; set; }
		public string Server{ get; set; }
		public int Port { get; set; }
		public ConnectionState State { get; set; }
        
		public string Username { get; set; }
		public string Password { get; set; }

		public string ConnectionString { get; set; }
		public string DatabaseName { get; set; }
		public bool ReadCache { get; set; }
		public bool DoNotCacheResults { get; set; }
		public bool ExtendedResultSets { get; set; }
		public bool MultipleActiveResultSets { get; set; }

		public string CacheCollection { get; set; }
		public string CacheExpiresIn { get; set; }
        
		public DatabaseResponseFormats ResponseFormat { get; set; }
        
		public bool IsAuthenticated { get { return _IsAuthenticated; } }

		public SQLDatabaseConnection() {
			ResponseFormat = DatabaseResponseFormats.Binary;
		}
		public SQLDatabaseConnection(string Username, string Password) {
			this.Username = Username;
			this.Password = Password;
			ResponseFormat = DatabaseResponseFormats.Binary;
		}

		private byte[] AES_Encrypt(byte[] BytesToEncrypt) {

			if ((BytesToEncrypt == null) || (BytesToEncrypt.Length == 0) || (!EncryptConnection))
				return BytesToEncrypt;

			byte[] EncryptedBytes = null;

			using (MemoryStream ms = new MemoryStream()) {
				using (AesManaged AES = new AesManaged()) {
					AES.KeySize = 256;
					AES.BlockSize = 128;

					var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
					AES.Key = key.GetBytes(AES.KeySize / 8);
					AES.IV = key.GetBytes(AES.BlockSize / 8);

					AES.Mode = CipherMode.CBC;

					using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write)) {
						cs.Write(BytesToEncrypt, 0, BytesToEncrypt.Length);
						cs.Close();
					}
					EncryptedBytes = ms.ToArray();
				}
			}

			return EncryptedBytes;
		}

		private byte[] AES_Decrypt(byte[] BytesToDecrypt) {

			if ((BytesToDecrypt == null) || (BytesToDecrypt.Length == 0) || (!EncryptConnection))
				return BytesToDecrypt;

			byte[] DecryptedBytes = null;

			using (MemoryStream ms = new MemoryStream()) {
				using (AesManaged AES = new AesManaged()) {
					AES.KeySize = 256;
					AES.BlockSize = 128;

					var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
					AES.Key = key.GetBytes(AES.KeySize / 8);
					AES.IV = key.GetBytes(AES.BlockSize / 8);

					AES.Mode = CipherMode.CBC;

					using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write)) {
						cs.Write(BytesToDecrypt, 0, BytesToDecrypt.Length);
						cs.Close();
					}
					DecryptedBytes = ms.ToArray();
				}
			}

			return DecryptedBytes;
		}

		public void Open() {
			if ((tcpClient == null) || (!tcpClient.Connected))
				tcpClient = new TcpClient();

			tcpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

			if (tcpClient.Connected)
				throw new Exception("Connection is already open.");
            
			if (string.IsNullOrWhiteSpace(Server))
				throw new Exception("Property Server must be set either with ip or name of server.");

			if (Port < 1)
				throw new Exception("Property Port must be set.");

			if (string.IsNullOrWhiteSpace(Username))
				throw new Exception("Username is required.");

			if (string.IsNullOrWhiteSpace(Password))
				throw new Exception("Password is required.");

			passwordBytes = Encoding.UTF8.GetBytes(InFlightAES256Password);
			try {
				tcpClient.Connect(Server, Port);
				if (tcpClient.Connected)
					State = ConnectionState.Open;

				tcpClient.NoDelay = true;
			} catch (SocketException se) {
				throw se;
			}

			try {
                
				byte[] b = AuthenticationRequest();
				if (b != null) {
					if (tcpClient.GetStream().CanWrite) {
						if ((EncryptConnection) && (!string.IsNullOrWhiteSpace(InFlightAES256Password))) {
							b = AES_Encrypt(b);
							tcpClient.GetStream().Write(b, 0, b.Length);
						} else {
							tcpClient.GetStream().Write(b, 0, b.Length);
						}
                        
					} else
						throw new Exception("TCP connection read only");


					byte[] buffer = new byte[256];
					byte[] ResponseBytes = new byte[0];
					int bytes = -1;
					do {
						try {
							bytes = tcpClient.GetStream().Read(buffer, 0, buffer.Length);
						} catch (IOException e) {
							Close();
							throw e;
						}
                        
						if (bytes < 154) {
							//Auth request response is over 154 bytes.. something went wrong.
							Close();
							throw new Exception("Server sent an invalid response, check server services.");
						}

						ResponseBytes = new byte[bytes];
						Buffer.BlockCopy(buffer, 0, ResponseBytes, 0, bytes);

						if ((EncryptConnection) && (!string.IsNullOrWhiteSpace(InFlightAES256Password))) {
							ResponseBytes = AES_Decrypt(ResponseBytes);
						}

						if (Encoding.UTF8.GetString(ResponseBytes).ToString().IndexOf("\0<EOF>\0") != -1) {
							break;
						}

					} while (bytes != 0);

					if (ResponseBytes != null) {
						string z;
						if ((z = AuthenticationRespons(ResponseBytes)).Equals("SQLDATABASE_OK")) {                            
							_IsAuthenticated = true;                            
						} else {
							_IsAuthenticated = false;
							Close();
							throw new Exception(z);
						}
					}
                        
				} else {
					Close();
					throw new Exception("Unable to convert string to bytes array.");
				}
                
			} catch (SocketException se) {
				throw se;
			}
		}
        
		public void Close() {
			if (tcpClient.Connected) {
				try {
					byte[] b = SendToServer(Encoding.UTF8.GetBytes("Close()"));
				} catch {
				}

				try {
					tcpClient.GetStream().Close();
					tcpClient.Client.Close();
					tcpClient.Close();
				} catch {
				}
			}

			//Console.WriteLine("Close()");
			State = ConnectionState.Close;
		}
        
		private byte[] SendToServer(byte[] b) {
			if (!tcpClient.Connected)
				throw new Exception("Connection not open");

			StreamRWLS.EnterWriteLock();
			try {
				if ((EncryptConnection) && (!string.IsNullOrWhiteSpace(InFlightAES256Password))) {
					b = AES_Encrypt(b);
					tcpClient.GetStream().Write(b, 0, b.Length);
				} else {
					tcpClient.GetStream().Write(b, 0, b.Length);
				}
                                
				State = ConnectionState.Sent;
			} catch (IOException e) {
				throw e;
			} finally {
				StreamRWLS.ExitWriteLock();
			}

			StreamRWLS.EnterWriteLock();
			List<byte> returnedBytes = new List<byte>();
			try { 
				int bytes = -1;
				do {
					State = ConnectionState.Wait;
					bytes = -1;
					byte[] buffer = new byte[8192];
					bytes = tcpClient.GetStream().Read(buffer, 0, buffer.Length);
					for (int i = 0; i < bytes; i++)
						returnedBytes.Add(buffer[i]);

					if ((EncryptConnection) && (!string.IsNullOrWhiteSpace(InFlightAES256Password))) {
						returnedBytes = AES_Decrypt(returnedBytes.ToArray()).ToList();
					}

					if (Encoding.UTF8.GetString(returnedBytes.ToArray()).ToString().IndexOf("\0<EOF>\0") != -1) {
						break;
					}
				} while (bytes != 0);
                
			} catch (IOException ioe) {
				Console.WriteLine(ioe.Message);
			} finally {
				StreamRWLS.ExitWriteLock();
				State = ConnectionState.Open;
			}


			StreamRWLS.EnterReadLock();
			try {
				//integrity check
				if (Encoding.UTF8.GetString(returnedBytes.ToArray()).ToString().IndexOf(EndOfHeader) != -1) {
					return returnedBytes.ToArray();
				}
			} catch {
			} finally {
				StreamRWLS.ExitReadLock();
			}

			return null;

		}

		private byte[] CreateRequestBytes(byte[] header, byte[] data) {
			byte[] footer = GetRequestFooter();
			byte[] RequestBytes = new byte[header.Length + data.Length + footer.Length];

			Buffer.BlockCopy(header, 0, RequestBytes, 0, header.Length);
			Buffer.BlockCopy(data, 0, RequestBytes, header.Length, data.Length);
			Buffer.BlockCopy(footer, 0, RequestBytes, (header.Length + data.Length), footer.Length);

			return RequestBytes;
		}

		private byte[] GetRequestFooter() {
			StringBuilder sb = new StringBuilder();
			sb.Append(EndOfLine);
			sb.Append(EndOfMessage);

			return Encoding.UTF8.GetBytes(sb.ToString());
		}

		private string[] ProcessServerBytes(byte[] ResponseBytes, out byte[] data) {
			byte[] header = new byte[140];
			data = new byte[ResponseBytes.Length - 154];
			byte[] footer = new byte[7];

			Buffer.BlockCopy(ResponseBytes, 0, header, 0, 140);
			Buffer.BlockCopy(ResponseBytes, 147, data, 0, ResponseBytes.Length - 154);
			Buffer.BlockCopy(ResponseBytes, ResponseBytes.Length - 7, footer, 0, 7);

			string ResponseString = Encoding.UTF8.GetString(header);
			string[] separators = { "\0<EOL>\0" };
			string[] HeaderArray = ResponseString.Split(separators, StringSplitOptions.None);

			return HeaderArray;
		}

		private string AuthenticationRespons(byte[] ResponseBytes) {
			if (ResponseBytes.Length < 154)
				throw new Exception("Server reponse is minimum 147 bytes for authentication response.");

			string[] rsArray = ProcessServerBytes(ResponseBytes, out ResponseBytes);
			if (rsArray.Length > 2) {
				if ((!rsArray[0].Equals("Authenticate")) || (!rsArray[2].Equals("Binary")))
					return rsArray[1];

				return rsArray[1];

			} else {
				return "Login Error, invalid response from server.";
			}
            
		}
        
		private byte[] AuthenticationRequest() {
			if (string.IsNullOrWhiteSpace(Username))
				throw new Exception("Username is required.");

			if (string.IsNullOrWhiteSpace(Password))
				throw new Exception("Password is required.");

			StringBuilder sb = new StringBuilder();
			sb.Append(Username);
			sb.Append(EndOfLine);
			sb.Append(Password);
			sb.Append(EndOfLine);
			sb.Append(DatabaseResponseFormats.Binary.ToString());
			sb.Append(EndOfLine);
			sb.Append("Authenticate");
			sb.Append(EndOfLine);
			sb.Append(EndOfMessage);
			return Encoding.UTF8.GetBytes(sb.ToString());
		}

		private byte[] GetDatabaseRequestHeader(string Command = "ExecuteNonQuery") {
			if (State != ConnectionState.Open)
				throw new Exception("Connection is not open.");

			string conn_str = string.Empty;

			if (!DatabaseCommands.Any(c => c.Equals(Command)))
				throw new Exception("Invalid Command parameter.");

			if (ResponseFormat == DatabaseResponseFormats.None)
				throw new Exception("Response Format property must be set.");

			if ((string.IsNullOrWhiteSpace(ConnectionString)) || (ConnectionString.IndexOf("=") == -1)) {
				if (string.IsNullOrWhiteSpace(DatabaseName))
					throw new Exception("Database name is required.");
				else
					conn_str += "Database = " + DatabaseName + ";";

				if (ReadCache == false) //optional default is true since database cache is useful
                    conn_str += "ReadCache = false;";

				if (DoNotCacheResults == true) //optional default is false since cache is used for next request
                    conn_str += "DoNotCacheResults = true;";

				if (MultipleActiveResultSets == true) //optional default is false since cache is used for next request
                    conn_str += "MultipleActiveResultSets = true;";

				if (ExtendedResultSets == true) //optional default is false since cache is used for next request
                    conn_str += "ExtendedResultSets = true;";
			}

			if (!string.IsNullOrWhiteSpace(conn_str))
				ConnectionString = conn_str;
            
			StringBuilder sb = new StringBuilder();
			sb.Append(Username);
			sb.Append(EndOfLine);
			sb.Append(Password);
			sb.Append(EndOfLine);
			sb.Append(ResponseFormat.ToString());
			sb.Append(EndOfLine);
			sb.Append(Command);
			sb.Append(EndOfLine);
			sb.Append(ConnectionString.ToString());
			sb.Append(EndOfLine);
			return Encoding.UTF8.GetBytes(sb.ToString());
		}

		private byte[] GetDatabaseRequestFooter() {
			StringBuilder sb = new StringBuilder();
			sb.Append(EndOfLine);
			sb.Append(EndOfMessage);

			return Encoding.UTF8.GetBytes(sb.ToString());
		}
		private byte[] GetCacheRequestHeader(string Command = "CacheGet", string CacheId = "", string CacheTags = "", string ExpiresIn = "") {
			if (State != ConnectionState.Open)
				throw new Exception("Connection is not open.");

			string conn_str = string.Empty;

			if (string.IsNullOrWhiteSpace(CacheCollection))
				CacheCollection = "Default";

			if (!CacheCommands.Any(c => c.Equals(Command)))
				throw new Exception("Invalid Command parameter.");


			StringBuilder sb = new StringBuilder();
			sb.Append(Username);
			sb.Append(EndOfLine);
			sb.Append(Password);
			sb.Append(EndOfLine);
			sb.Append(DatabaseResponseFormats.Binary.ToString());//Cache server only responds as binary to return original object
			sb.Append(EndOfLine);
			sb.Append(Command);
			sb.Append(EndOfLine);
			sb.Append(CacheCollection);
			sb.Append(EndOfLine);
			sb.Append(CacheId);
			sb.Append(EndOfLine);
			sb.Append(CacheTags);
			sb.Append(EndOfLine);
			sb.Append(ExpiresIn);
			sb.Append(EndOfLine);            

			return Encoding.UTF8.GetBytes(sb.ToString());
		}

		private byte[] GetCacheRequestFooter() {
			StringBuilder sb = new StringBuilder();
			sb.Append(EndOfLine);
			sb.Append(EndOfMessage);

			return Encoding.UTF8.GetBytes(sb.ToString());
		}

		internal int ExecuteDatabaseCommand(string DatabaseServerCommand, byte[] b, out SQLDatabaseResultSet[] sqlDatabaseResults, out string XML, out string JSON, out string ErrorMessage) {
			sqlDatabaseResults = new SQLDatabaseResultSet[0];
			XML = string.Empty;            
			JSON = string.Empty;
			ErrorMessage = string.Empty;
                        
			byte[] RequestBytes = CreateRequestBytes(GetDatabaseRequestHeader(DatabaseServerCommand), b);

			byte[] ResponseBytes = null;
			try {
				ResponseBytes = SendToServer(RequestBytes);
			} catch (IOException e) {
				//Console.WriteLine(e.Message);
			}
          
			if ((ResponseBytes != null) && (ResponseBytes.Length >= 147)) {
               

				byte[] Responsedata;
				string[] rsArray = ProcessServerBytes(ResponseBytes, out Responsedata);

				if (rsArray == null)
					throw new Exception("Unable to read server response.");

				if ((!string.IsNullOrWhiteSpace(rsArray[1])) && (!rsArray[1].Equals("SQLDATABASE_OK"))) {
					ErrorMessage = rsArray[1];
					return 1;
				}

				if (rsArray[2].Trim().ToLowerInvariant().Equals("binary")) {
					sqlDatabaseResults = ObjectFromByteArray<SQLDatabaseResultSet[]>(Responsedata);  
					return 0;
				}

				if (rsArray[2].Trim().ToLowerInvariant().Equals("xml")) {
					XML = Encoding.UTF8.GetString(Responsedata);
					return 0;
				}

				if (rsArray[2].Trim().ToLowerInvariant().Equals("json")) {
					JSON = Encoding.UTF8.GetString(Responsedata);
					return 0;
				}

				return 0;
			} else {
				ErrorMessage = "Unable to read all the bytes from server. Check client server connectivity.";
				return 1;
			}
		}

		internal string ExecuteCacheCommand(string CacheServerCommand, object ObjectToCache, out string ErrorMessage, string CacheId = "", string CacheTags = "", string ExpiresIn = "")
		{
			ErrorMessage = string.Empty;

			byte[] b = new byte[0];
			if (ObjectToCache.GetType().Name.ToLowerInvariant() == "byte[]") {
				b = (byte[])ObjectToCache;
			} else if (ObjectToCache.GetType().Name.ToLowerInvariant() == "byte") {
				b[0] = (byte)ObjectToCache;
			} else {
				b = ObjectToByteArray(ObjectToCache);
			}
            
			if (string.IsNullOrWhiteSpace(ExpiresIn))
				ExpiresIn = "1 Day";
            
			byte[] RequestBytes = CreateRequestBytes(GetCacheRequestHeader(CacheServerCommand, CacheId, CacheTags, ExpiresIn), b);
			byte[] ResponseBytes = null;
			try {
				ResponseBytes = SendToServer(RequestBytes);
			} catch (Exception e) {
				//Console.WriteLine(e.Message);
			}
          
			if ((ResponseBytes != null) && (ResponseBytes.Length >= 154)) {
               
				byte[] Responsedata;
				string[] rsArray = ProcessServerBytes(ResponseBytes, out Responsedata);

				if ((!string.IsNullOrWhiteSpace(rsArray[1])) && (!rsArray[1].Equals("SQLDATABASE_OK"))) {
					ErrorMessage = rsArray[1];
					return null;
				}

				return Encoding.UTF8.GetString(Responsedata);
			} else {
				ErrorMessage = "Unable to read all the bytes from server. Check client server connectivity.";
				return null;
			}
		}

		internal int ExecuteGetCacheCommand(out string ErrorMessage, out object CacheObject, bool ReturnRawBytes = false, string CacheId = "", string CacheTags = "")
		{
            
			ErrorMessage = string.Empty;
			CacheObject = new object();
			byte[] b = new byte[0];
            
            
			byte[] RequestBytes = CreateRequestBytes(GetCacheRequestHeader(ServerCommands.CacheGet.ToString(), CacheId, CacheTags), b);
			byte[] ResponseBytes = null;
			try {
				ResponseBytes = SendToServer(RequestBytes);
			} catch (Exception e) {
				//Console.WriteLine(e.Message);
			}
            
			if ((ResponseBytes != null) && (ResponseBytes.Length >= 154)) {
              
				byte[] Responsedata;
				string[] rsArray = ProcessServerBytes(ResponseBytes, out Responsedata);

				if ((!string.IsNullOrWhiteSpace(rsArray[1])) && (!rsArray[1].Equals("SQLDATABASE_OK"))) {
					ErrorMessage = rsArray[1];
					return 1;
				}

				try {
					if (ReturnRawBytes)
						CacheObject = Responsedata;
					else
						CacheObject = ObjectFromByteArray(Responsedata);
					if (CacheObject == null) {
						ErrorMessage = "Unknown Error";
						return 1;
					}
				} catch (Exception e) {
					ErrorMessage = e.Message;
					return 1;
				}
         

				return 0;
			} else {
				ErrorMessage = "Unable to read all the bytes from server. Check client server connectivity.";
				return 1;
			}
		}

		internal int ExecuteRemoveCacheCommand(out string ErrorMessage, out bool Success, string CacheId = "")
		{

			ErrorMessage = string.Empty;
			Success = false;

			byte[] b = new byte[0];

            
			byte[] RequestBytes = CreateRequestBytes(GetCacheRequestHeader(ServerCommands.CacheRemove.ToString(), CacheId), b);


			byte[] ResponseBytes = null;
			try {
				ResponseBytes = SendToServer(RequestBytes);
			} catch (Exception e) {
				//Console.WriteLine(e.Message);
			}
           

			if ((ResponseBytes != null) && (ResponseBytes.Length >= 147)) {

				byte[] Responsedata;
				string[] rsArray = ProcessServerBytes(ResponseBytes, out Responsedata);

				string ResponsedataString = Encoding.UTF8.GetString(Responsedata);
                
				if ((!string.IsNullOrWhiteSpace(rsArray[1])) && (!rsArray[1].Equals("SQLDATABASE_OK"))) {
					ErrorMessage = rsArray[1];
					return 1;
				}

				if (ResponsedataString.EndsWith("remove successful"))
					Success = true;
				else
					Success = false;

				return 0;
			} else {
				ErrorMessage = "Unable to read all the bytes from server. Check client server connectivity.";
				return 1;
			}
		}


		internal int ExecuteSearchCacheCommand(out string ErrorMessage, out List<object> ObjectsToReturn, bool ReturnRawBytes = false, string CacheTags = "")
		{            
			ErrorMessage = string.Empty;
			ObjectsToReturn = new List<object>();
			byte[] b = new byte[0];


			byte[] RequestBytes = CreateRequestBytes(GetCacheRequestHeader(ServerCommands.CacheSearch.ToString(), "", CacheTags), b);
			byte[] ResponseBytes = null;
			try {
				ResponseBytes = SendToServer(RequestBytes);
			} catch (Exception e) {
				//Console.WriteLine(e.Message);
			}
         
			if ((ResponseBytes != null) && (ResponseBytes.Length >= 154)) {

				byte[] Responsedata;
				string[] rsArray = ProcessServerBytes(ResponseBytes, out Responsedata);

				if ((!string.IsNullOrWhiteSpace(rsArray[1])) && (!rsArray[1].Equals("SQLDATABASE_OK"))) {
					ErrorMessage = rsArray[1];
					return 1;
				}

				try {
					string[] NewLineChars = { "\0<EOL>\0" };
					string[] ObjectArrayLength = Encoding.UTF8.GetString(Responsedata).Split(NewLineChars, StringSplitOptions.None);
					int[] ObjectLength = new int[ObjectArrayLength[0].Split(',').Length];
					int offSet = Encoding.UTF8.GetString(Responsedata).IndexOf(NewLineChars[0]) + 7;

					int i = 0;
					foreach (string len in ObjectArrayLength[0].Split(',')) {
						int.TryParse(len, out ObjectLength[i]);
						i++;
					}         
                    
					for (i = 0; i < ObjectLength.Length; i++) {
						byte[] bArray = new byte[ObjectLength[i]];
						Buffer.BlockCopy(Responsedata, offSet, bArray, 0, bArray.Length);

						if (bArray.Length > 0) {
							try {
								if (ReturnRawBytes)
									ObjectsToReturn.Add(bArray);
								else
									ObjectsToReturn.Add(ObjectFromByteArray(bArray));
							} catch (Exception e) {
								ErrorMessage = e.Message;
								return 1;
							}
						}
                        
						offSet += bArray.Length;
					}

					return 0;                    
				} catch (Exception e) {
					ErrorMessage = e.Message;
					return 1;
				}

			} else {
				ErrorMessage = "Unable to read all the bytes from server. Check client server connectivity.";
				return 1;
			}
		}

		public static byte[] ObjectToByteArray(Object obj)
		{
			try {
				BinaryFormatter bf = new BinaryFormatter();
				bf.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
				using (var ms = new MemoryStream()) {
					bf.Serialize(ms, obj);
					return ms.ToArray();
				}
			} catch (Exception e) {
				throw new Exception(e.Message);                
			}
           
		}

		public Object ObjectFromByteArray(byte[] BytesArray)
		{            
			using (var memStream = new MemoryStream()) {              
				BinaryFormatter binForm = new BinaryFormatter();                  
				binForm.Binder = new BindChanger();
				memStream.Write(BytesArray, 0, BytesArray.Length);
				memStream.Seek(0, 0);                
				var obj = binForm.Deserialize(memStream);
                
				return obj;
			}
		}

		public T ObjectFromByteArray<T>(byte[] BytesArray)
		{
			if (BytesArray == null)
				return default(T);

			try {
				BinaryFormatter bf = new BinaryFormatter();
				bf.Binder = new BindChanger();
				using (MemoryStream ms = new MemoryStream(BytesArray)) {
					ms.Write(BytesArray, 0, BytesArray.Length);
					ms.Seek(0, 0);                    
					return (T)bf.Deserialize(ms);
				}
			} catch (Exception e) {
				//Console.WriteLine(e.Message);
				return default(T);
			}            
		}
	}
    
	public class SQLDatabaseParameter
	{
		public string ParameterName { get; set; }
		public object Value { get; set; }
	}

	public class SQLDatabaseCommand : IDisposable
	{

		private string EndOfLine = "\0<EOL>\0";
		public SQLDatabaseConnection Connection { get; set; }
		public string CommandText { get; set; }
		public List<SQLDatabaseParameter> Parameters { get; set; }

		public SQLDatabaseCommand()
		{
			Parameters = new List<SQLDatabaseParameter>();

		}
		public SQLDatabaseCommand(SQLDatabaseConnection connection)
		{
			Connection = connection;
		}
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected void Dispose(bool disposing)
		{   
			if (disposing) {
				//do nothing since connection can be used by cache client.
				CommandText = null;
			}
		}

		private void CheckIfConnectionIsOpen()
		{
			if (Connection == null)
				throw new Exception("SQLDatabaseCommand Connection property not set.");

			if (Connection.State != ConnectionState.Open)
				throw new Exception("Connection must be open before command can be executed.");
		}

		private byte[] GetCommandBytes()
		{
			if ((CommandText == null) || (string.IsNullOrWhiteSpace(CommandText)))
				throw new Exception("CommandText property must be set with valid sql command before command can be executed.");

			List<byte> lst = new List<byte>();
            
			lst.AddRange(Encoding.UTF8.GetBytes(CommandText));
			lst.AddRange(Encoding.UTF8.GetBytes(EndOfLine));

			if (Parameters.Count > 0) {
				foreach (SQLDatabaseParameter p in Parameters) {
					if (p.Value != null) {
						if (p.Value.GetType().Name.Equals("Byte[]")) {
							lst.AddRange((byte[])p.Value);
							lst.AddRange(Encoding.UTF8.GetBytes(EndOfLine));
						} else {
							string ParamToString = p.Value.ToString();
							lst.AddRange(Encoding.UTF8.GetBytes(ParamToString));
							lst.AddRange(Encoding.UTF8.GetBytes(EndOfLine));
						}
					}
                    
				}
			}

			return lst.ToArray();
		}

		public SQLDatabaseResultSet[] ExecuteNonQuery()
		{
			CheckIfConnectionIsOpen();

			byte[] b = GetCommandBytes();
			SQLDatabaseResultSet[] rs;
			string Xml = string.Empty;
			string Json = string.Empty;
			string ErrorMessage = string.Empty;
			Connection.ResponseFormat = DatabaseResponseFormats.Binary;
			int rc = Connection.ExecuteDatabaseCommand("ExecuteNonQuery", b, out rs, out Xml, out Json, out ErrorMessage);
			if (rc != 0)
				throw new Exception(ErrorMessage);
			else
				return rs;                        
		}
		public string ExecuteNonQueryXML()
		{
			CheckIfConnectionIsOpen();

			byte[] b = GetCommandBytes();
			SQLDatabaseResultSet[] rs;
			string Xml = string.Empty;
			string Json = string.Empty;
			string ErrorMessage = string.Empty;
			Connection.ResponseFormat = DatabaseResponseFormats.XML;
			int rc = Connection.ExecuteDatabaseCommand("ExecuteNonQuery", b, out rs, out Xml, out Json, out ErrorMessage);
			if (rc != 0)
				throw new Exception(ErrorMessage);
			else
				return Xml;
		}

		public string ExecuteNonQueryJson()
		{
			CheckIfConnectionIsOpen();

			byte[] b = GetCommandBytes();
			SQLDatabaseResultSet[] rs;
			string Xml = string.Empty;
			string Json = string.Empty;
			string ErrorMessage = string.Empty;
			Connection.ResponseFormat = DatabaseResponseFormats.JSON;
			int rc = Connection.ExecuteDatabaseCommand("ExecuteNonQuery", b, out rs, out Xml, out Json, out ErrorMessage);
			if (rc != 0)
				throw new Exception(ErrorMessage);
			else
				return Json;
		}


		public SQLDatabaseResultSet[] ExecuteScalar()
		{
			CheckIfConnectionIsOpen();

			byte[] b = GetCommandBytes();
			SQLDatabaseResultSet[] rs;
			string Xml = string.Empty;
			string Json = string.Empty;
			string ErrorMessage = string.Empty;
			Connection.ResponseFormat = DatabaseResponseFormats.Binary;
			int rc = Connection.ExecuteDatabaseCommand("ExecuteScalar", b, out rs, out Xml, out Json, out ErrorMessage);
			if (rc != 0)
				throw new Exception(ErrorMessage);
			else
				return rs;
		}

		public string ExecuteScalarXML()
		{
			CheckIfConnectionIsOpen();

			byte[] b = GetCommandBytes();
			SQLDatabaseResultSet[] rs;
			string Xml = string.Empty;
			string Json = string.Empty;
			string ErrorMessage = string.Empty;
			Connection.ResponseFormat = DatabaseResponseFormats.XML;
			int rc = Connection.ExecuteDatabaseCommand("ExecuteScalar", b, out rs, out Xml, out Json, out ErrorMessage);
			if (rc != 0)
				throw new Exception(ErrorMessage);
			else
				return Xml;
		}

		public string ExecuteScalarJson()
		{
			CheckIfConnectionIsOpen();

			byte[] b = GetCommandBytes();
			SQLDatabaseResultSet[] rs;
			string Xml = string.Empty;
			string Json = string.Empty;
			string ErrorMessage = string.Empty;
			Connection.ResponseFormat = DatabaseResponseFormats.JSON;
			int rc = Connection.ExecuteDatabaseCommand("ExecuteScalar", b, out rs, out Xml, out Json, out ErrorMessage);
			if (rc != 0)
				throw new Exception(ErrorMessage);
			else
				return Json;
		}


		public SQLDatabaseResultSet[] ExecuteReader()
		{
			CheckIfConnectionIsOpen();

			byte[] b = GetCommandBytes();
			SQLDatabaseResultSet[] rs;
			string Xml = string.Empty;
			string Json = string.Empty;
			string ErrorMessage = string.Empty;
			Connection.ResponseFormat = DatabaseResponseFormats.Binary;
			int rc = Connection.ExecuteDatabaseCommand("ExecuteReader", b, out rs, out Xml, out Json, out ErrorMessage);
			if (rc != 0)
				throw new Exception(ErrorMessage);
			else
				return rs;
		}


		public string ExecuteReaderXML()
		{
			CheckIfConnectionIsOpen();            

			byte[] b = GetCommandBytes();
			SQLDatabaseResultSet[] rs;
			string Xml = string.Empty;
			string Json = string.Empty;
			string ErrorMessage = string.Empty;
			Connection.ResponseFormat = DatabaseResponseFormats.XML;
			int rc = Connection.ExecuteDatabaseCommand("ExecuteReader", b, out rs, out Xml, out Json, out ErrorMessage);
			if (rc != 0)
				throw new Exception(ErrorMessage);
			else
				return Xml;
		}

		public string ExecuteReaderJson()
		{
			CheckIfConnectionIsOpen();

			byte[] b = GetCommandBytes();
			SQLDatabaseResultSet[] rs;
			string Xml = string.Empty;
			string Json = string.Empty;
			string ErrorMessage = string.Empty;
			Connection.ResponseFormat = DatabaseResponseFormats.JSON;
			int rc = Connection.ExecuteDatabaseCommand("ExecuteReader", b, out rs, out Xml, out Json, out ErrorMessage);
			if (rc != 0)
				throw new Exception(ErrorMessage);
			else
				return Json;
		}

		/// <summary>
		/// Converts array of SQLDatabaseResultSet object to System.Data.DataSet object
		/// </summary>
		/// <param name="ResultSets"></param>
		public System.Data.DataSet ConvertToDataSet(SQLDatabaseResultSet[] ResultSets)
		{
			if (ResultSets == null)
				throw new Exception("ResultsSet cannot be null");

			System.Data.DataSet ds = new System.Data.DataSet();

			int TableNumber = 0;
			foreach (SQLDatabaseResultSet rs in ResultSets) {
				TableNumber++;
				System.Data.DataTable dt = new System.Data.DataTable("Table" + TableNumber);

				for (int i = 0; i < rs.ColumnCount; i++) {
					if (rs.Columns[i] != null) {
						if (rs.DataTypes[i] != null) {
							string typename = DBTypeToClrType(rs.DataTypes[i]);

							Type t = Type.GetType(typename);
							dt.Columns.Add(rs.Columns[i].ToString(), t);
						} else {
							dt.Columns.Add(rs.Columns[i].ToString(), Type.GetType("System.Object"));
						}

					}
				}

				for (int i = 0; i < rs.RowCount; i++) {
					System.Data.DataRow dr = dt.NewRow();

					for (int c = 0; c < rs.ColumnCount; c++) {
						try {
							dr[c] = rs.Rows[i][c];
						} catch {
						}
					}


					dt.Rows.Add(dr);
				}

				ds.Tables.Add(dt);
			}

			return ds;
		}

		/// <summary>
		/// Convert SQLDatabaseResultSet object to System.Data.DataSet object
		/// </summary>
		/// <param name="ResultSets"></param>
		public System.Data.DataSet ConvertToDataSet(SQLDatabaseResultSet ResultsSet)
		{
			if (ResultsSet == null)
				throw new Exception("ResultsSet cannot be null");

			List<SQLDatabaseResultSet> lst = new List<SQLDatabaseResultSet>();
			lst.Add(ResultsSet);
			return ConvertToDataSet(lst.ToArray());
		}

		/// <summary>
		/// Convert SQLDatabaseResultSet object to System.Data.DataTable object
		/// </summary>
		/// <param name="ResultSets"></param>
		public System.Data.DataTable ConvertToDataTable(SQLDatabaseResultSet ResultsSet)
		{
			if (ResultsSet == null)
				throw new Exception("ResultsSet cannot be null");

			List<SQLDatabaseResultSet> lst = new List<SQLDatabaseResultSet>();
			lst.Add(ResultsSet);
			return ConvertToDataSet(lst.ToArray()).Tables[0];
		}

		private string DBTypeToClrType(string DBType)
		{
			DBType = DBType.ToLowerInvariant().Trim();

			switch (DBType) {
				case "DateTime":
					return "System.String";
				case "Text":
					return "System.String";
				case "Real":
					return "System.Double";
				case "integer":
					return "System.Int64";
				default:
					return "System.Object";
			}
		}
	}


	/// <summary>
	/// This class contain methods and routines to access cache server.
	/// It acts as Cache Server client, cache objects are temporary in memory objects which can be shared among all clients/users.
	/// </summary>
	public class SQLDatabaseCacheServer
	{
		string[] LineSeparator = { "\0<EOL>\0" };
		public SQLDatabaseConnection Connection { get; set; }

		public string ExpiresIn(int DurationNumber, CacheDurations Duration)
		{
			return DurationNumber + " " + Duration.ToString();
		}

		private void CheckIfConnectionIsOpen()
		{
			if (Connection == null)
				throw new Exception("SQLDatabaseCommand Connection property not set.");

			if (Connection.State != ConnectionState.Open)
				throw new Exception("Connection must be open before command can be executed.");
		}


		/// <summary>
		/// Add an object to cache
		/// </summary>
		public string Add<T>(object ObjectToCache, string CacheId = "", string Tags = "", string ExpiresIn = "1 Day")
		{
			Type t = typeof(T);

			if (ObjectToCache == null)
				throw new Exception("Cannot cache a null object.");

			CheckIfConnectionIsOpen();


			if (!string.IsNullOrWhiteSpace(t.FullName))
				Connection.CacheCollection = t.FullName;

          
			string NewCacheId = string.Empty;
			string ErrorMessage = string.Empty;

			NewCacheId = Connection.ExecuteCacheCommand("CacheAdd", ObjectToCache, out ErrorMessage, CacheId, Tags, ExpiresIn);

			if ((NewCacheId == null) && (!string.IsNullOrWhiteSpace(ErrorMessage)))
				throw new Exception(ErrorMessage);

			return NewCacheId;
		}

		/// <summary>
		/// Add raw bytes to cache, if you are using multiple different clients such as C#, Java use this method
		/// </summary>
		public string AddRaw(string CollectionName, byte[] ObjectToCache, string CacheId = "", string Tags = "", string ExpiresIn = "1 Day")
		{

			CheckIfConnectionIsOpen();

			if (string.IsNullOrWhiteSpace(CollectionName))
				throw new Exception("CollectionName is required.");
			else
				Connection.CacheCollection = CollectionName;

			if (ObjectToCache == null)
				throw new Exception("Cannot cache a null object.");
            
			string NewCacheId = string.Empty;
			string ErrorMessage = string.Empty;

			NewCacheId = Connection.ExecuteCacheCommand("CacheAdd", ObjectToCache, out ErrorMessage, CacheId, Tags, ExpiresIn);

			if ((NewCacheId == null) && (!string.IsNullOrWhiteSpace(ErrorMessage)))
				throw new Exception(ErrorMessage);

			return NewCacheId;
		}

		/// <summary>
		/// Get an object from cache 
		/// </summary>
		public T Get<T>(string CacheId, string Tags = "")
		{
			Type t = typeof(T);
           

			CheckIfConnectionIsOpen();

			if (!string.IsNullOrWhiteSpace(t.FullName))
				Connection.CacheCollection = t.FullName;

			bool ReturnRawBytes = false;

			if (t.Name == "Byte[]")
				ReturnRawBytes = true;


			string ErrorMessage = string.Empty;
			object obj = null;
			int rc = Connection.ExecuteGetCacheCommand(out ErrorMessage, out obj, ReturnRawBytes, CacheId, Tags);

			if ((rc != 0) && (!string.IsNullOrWhiteSpace(ErrorMessage)))
				throw new Exception(ErrorMessage);

			return (T)obj;
		}

		/// <summary>
		/// Get an object to cache
		/// </summary>
		public object Get(string CollectionName, string CacheId)
		{
           
			CheckIfConnectionIsOpen();

			if (string.IsNullOrWhiteSpace(CollectionName))
				throw new Exception("CollectionName is required.");
			else
				Connection.CacheCollection = CollectionName;
                        
			string ErrorMessage = string.Empty;
			object obj = null;
			int rc = Connection.ExecuteGetCacheCommand(out ErrorMessage, out obj, true, CacheId, "");

			if ((rc != 0) && (!string.IsNullOrWhiteSpace(ErrorMessage)))
				throw new Exception(ErrorMessage);

			return obj;
		}

		/// <summary>
		/// Get an object from cache, if it was stored by different programming language use this method.
		/// </summary>
		public byte[] GetRaw<T>(string CacheId, string Tags = "")
		{
			Type t = typeof(T);


			CheckIfConnectionIsOpen();

			if (!string.IsNullOrWhiteSpace(t.FullName))
				Connection.CacheCollection = t.FullName;

			bool ReturnRawBytes = true;

			if (t.Name == "Byte[]")
				ReturnRawBytes = true;


			string ErrorMessage = string.Empty;
			object obj = null;
			int rc = Connection.ExecuteGetCacheCommand(out ErrorMessage, out obj, ReturnRawBytes, CacheId, Tags);

			if ((rc != 0) && (!string.IsNullOrWhiteSpace(ErrorMessage)))
				throw new Exception(ErrorMessage);

			return (byte[])obj;
		}

		/// <summary>
		/// returns raw bytes to encode or decode via application.
		/// </summary>
		public byte[] GetRaw(string CollectionName, string CacheId, string Tags = "")
		{
            
			CheckIfConnectionIsOpen();

			if (string.IsNullOrWhiteSpace(CollectionName))
				throw new Exception("CollectionName is required.");
			else
				Connection.CacheCollection = CollectionName;

			string ErrorMessage = string.Empty;
			object obj = null;
			int rc = Connection.ExecuteGetCacheCommand(out ErrorMessage, out obj, true, CacheId, Tags);

			if ((rc != 0) && (!string.IsNullOrWhiteSpace(ErrorMessage)))
				throw new Exception(ErrorMessage);

			return (byte[])obj;
		}

		/// <summary>
		/// Remove an object from cache server, type T is the converted to collection name.
		/// </summary>
		public bool Remove<T>(string CacheId)
		{
			Type t = typeof(T);

			CheckIfConnectionIsOpen();

			if (!string.IsNullOrWhiteSpace(t.FullName))
				Connection.CacheCollection = t.FullName;

			bool Success = false;
			string ErrorMessage = string.Empty;
			int rc = Connection.ExecuteRemoveCacheCommand(out ErrorMessage, out Success, CacheId);

			if ((rc != 0) && (!string.IsNullOrWhiteSpace(ErrorMessage)))
				throw new Exception(ErrorMessage);

			return Success;
		}

		/// <summary>
		/// Remove an object from Cache server using collection name.
		/// </summary>
		public bool Remove(string CollectionName, string CacheId)
		{            
            
			CheckIfConnectionIsOpen();

			if (string.IsNullOrWhiteSpace(CollectionName))
				throw new Exception("CollectionName is required.");
			else
				Connection.CacheCollection = CollectionName;

			bool Success = false;
			string ErrorMessage = string.Empty;
			int rc = Connection.ExecuteRemoveCacheCommand(out ErrorMessage, out Success, CacheId);

			if ((rc != 0) && (!string.IsNullOrWhiteSpace(ErrorMessage)))
				throw new Exception(ErrorMessage);

			return Success;
		}

		/// <summary>
		/// Drop an entire collection all objects are removed.
		/// </summary>
		public bool DropCollection<T>()
		{
			Type t = typeof(T);

			CheckIfConnectionIsOpen();

			if (!string.IsNullOrWhiteSpace(t.FullName))
				Connection.CacheCollection = t.FullName;

			object obj = new object();
			string ErrorMessage = string.Empty;
			string rc = Connection.ExecuteCacheCommand("CacheDropCollection", obj, out ErrorMessage, "", "", "");

			if (!string.IsNullOrWhiteSpace(ErrorMessage))
				throw new Exception(ErrorMessage);

			if (rc.Equals("Success"))
				return true;
			else
				return false;
		}

		/// <summary>
		/// Drop collection by name.
		/// </summary>
		public bool DropCollection(string CollectionName)
		{
           
			CheckIfConnectionIsOpen();

			if (string.IsNullOrWhiteSpace(CollectionName))
				throw new Exception("CollectionName is required.");
			else
				Connection.CacheCollection = CollectionName;

			object obj = new object();
			string ErrorMessage = string.Empty;
			string rc = Connection.ExecuteCacheCommand("CacheDropCollection", obj, out ErrorMessage, "", "", "");

			if (!string.IsNullOrWhiteSpace(ErrorMessage))
				throw new Exception(ErrorMessage);

			if (rc.Equals("Success"))
				return true;
			else
				return false;
		}

		/// <summary>
		/// update an existing object.
		/// </summary>
		public string Update<T>(string CacheId, object ObjectToCache, string Tags = "", string ExpiresIn = "1 Day")
		{
			Type t = typeof(T);

			CheckIfConnectionIsOpen();

			if (ObjectToCache == null)
				throw new Exception("Cannot cache a null object.");

			if (string.IsNullOrWhiteSpace(CacheId))
				throw new Exception("Cache Id is required.");

            

			if (!string.IsNullOrWhiteSpace(t.FullName))
				Connection.CacheCollection = t.FullName;

            
			string ErrorMessage = string.Empty;
			String ReturnedCacheId = Connection.ExecuteCacheCommand("CacheUpdate", ObjectToCache, out ErrorMessage, CacheId, Tags, ExpiresIn);

			if ((ReturnedCacheId == null) && (!string.IsNullOrWhiteSpace(ErrorMessage)))
				throw new Exception(ErrorMessage);

			if (CacheId.Equals(ReturnedCacheId))
				return ReturnedCacheId;
			else
				throw new Exception("Integrity check failed, CacheId requested to update and cache id returned are different.");

		}

		/// <summary>
		/// Update an object using collection name
		/// </summary>
		public string Update(string CollectionName, string CacheId, object ObjectToCache, string Tags = "", string ExpiresIn = "1 Day")
		{

			CheckIfConnectionIsOpen();

			if (ObjectToCache == null)
				throw new Exception("Cannot cache a null object.");

			if (string.IsNullOrWhiteSpace(CacheId))
				throw new Exception("Cache Id to update, is required for update.");

			if (string.IsNullOrWhiteSpace(CollectionName))
				throw new Exception("CollectionName is required.");
			else
				Connection.CacheCollection = CollectionName;


			string ErrorMessage = string.Empty;
			String ReturnedCacheId = Connection.ExecuteCacheCommand("CacheUpdate", ObjectToCache, out ErrorMessage, CacheId, Tags, ExpiresIn);

			if ((ReturnedCacheId == null) && (!string.IsNullOrWhiteSpace(ErrorMessage)))
				throw new Exception(ErrorMessage);

			if (CacheId.Equals(ReturnedCacheId))
				return ReturnedCacheId;
			else
				throw new Exception("Integrity check failed, CacheId requested to update and cache id returned are different.");

		}

		/// <summary>
		/// Add or update an object in cache
		/// </summary>
		public string AddOrUpdate<T>(string CacheId, object ObjectToCache, string Tags = "", string ExpiresIn = "1 Day")
		{
			Type t = typeof(T);

			CheckIfConnectionIsOpen();

			if (ObjectToCache == null)
				throw new Exception("Cannot cache a null object.");

			if (string.IsNullOrWhiteSpace(CacheId))
				throw new Exception("Cache Id is required when using AddOrUpdate.");
            
			if (!string.IsNullOrWhiteSpace(t.FullName))
				Connection.CacheCollection = t.FullName;


			string ErrorMessage = string.Empty;
			String ReturnedCacheId = Connection.ExecuteCacheCommand("CacheAddOrUpdate", ObjectToCache, out ErrorMessage, CacheId, Tags, ExpiresIn);

			if ((ReturnedCacheId == null) && (!string.IsNullOrWhiteSpace(ErrorMessage)))
				throw new Exception(ErrorMessage);

			if (CacheId.Equals(ReturnedCacheId))
				return ReturnedCacheId;
			else
				throw new Exception("Integrity check failed, CacheId requested to add or update and cache id returned are different.");

		}

		/// <summary>
		/// Search by tags in type T collection.
		/// </summary>
		public T[] SearchByTags<T>(string Tags = "")
		{
			Type t = typeof(T);

			CheckIfConnectionIsOpen();

			if (!string.IsNullOrWhiteSpace(t.FullName))
				Connection.CacheCollection = t.FullName;

			List<object> returnedObject = new List<object>();
			string ErrorMessage = string.Empty;
			int rc = Connection.ExecuteSearchCacheCommand(out ErrorMessage, out returnedObject, false, Tags);

			if ((rc == 0) && (returnedObject.Count == 0))
				return default(T[]);

            
			if (rc == 0) {
				List<T> lstToReturn = new List<T>();
				foreach (object o in returnedObject)
					lstToReturn.Add((T)o);

				return (T[])lstToReturn.ToArray();
			} else
				throw new Exception(ErrorMessage);

		}

		/// <summary>
		/// Search by tags but only in given collection.
		/// </summary>
		public List<byte[]> SearchByTagsRaw(string CollectionName, string Tags = "")
		{
			List<byte[]> lstToReturn = new List<byte[]>();

			CheckIfConnectionIsOpen();

			if (string.IsNullOrWhiteSpace(CollectionName))
				throw new Exception("CollectionName is required.");
			else
				Connection.CacheCollection = CollectionName;

			List<object> returnedObject = new List<object>();
			string ErrorMessage = string.Empty;
			int rc = Connection.ExecuteSearchCacheCommand(out ErrorMessage, out returnedObject, false, Tags);

			if ((rc == 0) && (returnedObject.Count == 0))
				return lstToReturn;


			if (rc == 0) {                
				foreach (object o in returnedObject)
					lstToReturn.Add((byte[])o);

				return lstToReturn;
			} else
				throw new Exception(ErrorMessage);

		}

		/// <summary>
		/// Number of objects in any given collection.
		/// </summary>
		public Int64 Count<T>()
		{
			Type t = typeof(T);

			CheckIfConnectionIsOpen();

			if (!string.IsNullOrWhiteSpace(t.FullName))
				Connection.CacheCollection = t.FullName;

			object obj = new object();
			string ErrorMessage = string.Empty;
			string CollectionCountString = Connection.ExecuteCacheCommand("CacheCollectionCount", obj, out ErrorMessage, "", "", "");

			if (CollectionCountString != null) {
				Int64 CollectionCount = 0;
				Int64.TryParse(CollectionCountString, out CollectionCount);
				return CollectionCount;
			} else
				throw new Exception(ErrorMessage);
		}

		/// <summary>
		/// Number of objects in any given collection.
		/// </summary>
		public Int64 Count(string CollectionName)
		{
           
			CheckIfConnectionIsOpen();

			if (string.IsNullOrWhiteSpace(CollectionName))
				throw new Exception("CollectionName is required.");
			else
				Connection.CacheCollection = CollectionName;


			object obj = new object();
			string ErrorMessage = string.Empty;
			string CollectionCountString = Connection.ExecuteCacheCommand("CacheCollectionCount", obj, out ErrorMessage, "", "", "");

			if (CollectionCountString != null) {
				Int64 CollectionCount = 0;
				Int64.TryParse(CollectionCountString, out CollectionCount);
				return CollectionCount;
			} else
				throw new Exception(ErrorMessage);
		}

		/// <summary>
		/// List of all the collections on cache server.
		/// </summary>
		public List<string> CollectionList()
		{
			List<string> lstStrings = new List<string>();
            
			CheckIfConnectionIsOpen();

           
			object obj = new object();
			string ErrorMessage = string.Empty;
			string CollectionList = Connection.ExecuteCacheCommand("CacheCollectionList", obj, out ErrorMessage, "", "", "");

			if (CollectionList != null) {
				foreach (String Collection in CollectionList.Split(LineSeparator, StringSplitOptions.RemoveEmptyEntries))
					lstStrings.Add(Collection);
			} else
				throw new Exception(ErrorMessage);

			return lstStrings;
		}

		/// <summary>
		/// returns list of string of cache ids.
		/// </summary>
		public List<string> CollectionCacheIds<T>()
		{
			List<string> lstStrings = new List<string>();

			Type t = typeof(T);

			CheckIfConnectionIsOpen();

			if (!string.IsNullOrWhiteSpace(t.FullName))
				Connection.CacheCollection = t.FullName;

			object obj = new object();
			string ErrorMessage = string.Empty;
			string CollectionList = Connection.ExecuteCacheCommand("CacheCollectionCacheIds", obj, out ErrorMessage, "", "", "");

			if (CollectionList != null) {
                
				foreach (String CacheId in CollectionList.Split(LineSeparator, StringSplitOptions.RemoveEmptyEntries))
					lstStrings.Add(CacheId);
			} else
				throw new Exception(ErrorMessage);

			return lstStrings;
		}

		/// <summary>
		/// returns List of cache id's as string
		/// </summary>
		public List<string> CollectionCacheIds(string CollectionName)
		{
			List<string> lstStrings = new List<string>();
            
			CheckIfConnectionIsOpen();

			if (string.IsNullOrWhiteSpace(CollectionName))
				throw new Exception("CollectionName is required.");
			else
				Connection.CacheCollection = CollectionName;

			object obj = new object();
			string ErrorMessage = string.Empty;
			string CollectionList = Connection.ExecuteCacheCommand("CacheCollectionCacheIds", obj, out ErrorMessage, "", "", "");

			if (CollectionList != null) {
				string[] separator = { "\0<EOL>\0" };
				foreach (String CacheId in CollectionList.Split(separator, StringSplitOptions.RemoveEmptyEntries))
					lstStrings.Add(CacheId);
			} else
				throw new Exception(ErrorMessage);

			return lstStrings;
		}

	}

	/// <summary>
	/// This class contain methods and routines to perform tasks other than database CRUD.
	/// Most functions in this class require sys admin permission on server.
	/// </summary>
	public class SQLDatabaseUtility
	{
		public SQLDatabaseCommand Command { get; set; }

		private string BuildConnectionString(string DatabaseName, bool MARS = false, bool ExtRS = false)
		{
			StringBuilder sb = new StringBuilder();

			sb.Append("Database=");
			sb.Append(DatabaseName);
			sb.Append(";");

			if (MARS)
				sb.Append("MultipleActiveResultSets=true;");
			else
				sb.Append("MultipleActiveResultSets=false;");

			if (ExtRS)
				sb.Append("ExtendedResultSets=true;");
			else
				sb.Append("ExtendedResultSets=false;");


			return sb.ToString();
		}

		private SQLDatabaseResultSet ExecuteSQL(string SQLText)
		{
			Command.CommandText = SQLText;
			SQLDatabaseResultSet[] rs = Command.ExecuteNonQuery();

			if (rs != null) {
				return rs[0];                
			} else {
				throw new Exception("Invalid SQL command");
			}
		}

		/// <summary>
		/// Server version
		/// </summary>        
		public string ServerVersion()
		{
			string SQLText = string.Format("SElECT sys_version() AS ServerVersion;");

			Command.Connection.ConnectionString = BuildConnectionString("System");

			SQLDatabaseResultSet rs = ExecuteSQL(SQLText);
			if (string.IsNullOrWhiteSpace(rs.ErrorMessage))
				return rs.Rows[0][0].ToString();
			else
				throw new Exception(rs.ErrorMessage);
		}

		public bool CreateDatabase(string DatabaseName, string EncryptionKey = "")
		{
			if (string.IsNullOrWhiteSpace(DatabaseName))
				throw new Exception("Database Name is required.");

			string SQLText = string.Empty;

			if (string.IsNullOrWhiteSpace(EncryptionKey))
				SQLText = string.Format("Create Database {0} ;", DatabaseName);
			else {
				SQLText = string.Format("Create Database {0} WITH EncryptionKey {1} ;", DatabaseName, EncryptionKey);
			}

			Command.Connection.ConnectionString = BuildConnectionString("System");

			SQLDatabaseResultSet rs = ExecuteSQL(SQLText);
			if ((rs.RowCount > 0) && (rs.ColumnCount > 0) && (rs.Rows[0][0].Equals("SQLDATABASE_OK")) && (string.IsNullOrWhiteSpace(rs.ErrorMessage)))
				return true;
			else
				throw new Exception(rs.ErrorMessage);
		}
		public List<string> Databases()
		{
			List<string> lst = new List<string>();
			string SQLText = string.Format("SELECT DatabaseName FROM Databases;");

			Command.Connection.ConnectionString = BuildConnectionString("System");

			SQLDatabaseResultSet rs = ExecuteSQL(SQLText);
			if ((rs == null) || (!string.IsNullOrWhiteSpace(rs.ErrorMessage)))
				throw new Exception(rs.ErrorMessage);

			foreach (object[] row in rs.Rows) {
				if ((row != null) && (row[0] != null))
					lst.Add(row[0].ToString());
			}

			return lst;
		}

		public SQLDatabaseResultSet DatabasesDetails()
		{
			string SQLText = string.Format("SELECT * FROM Databases;");

			Command.Connection.ConnectionString = BuildConnectionString("System");

			SQLDatabaseResultSet rs = ExecuteSQL(SQLText);
			if (string.IsNullOrWhiteSpace(rs.ErrorMessage))
				return rs;
			else
				throw new Exception(rs.ErrorMessage);
		}


		public bool DropDatabase(string DatabaseName)
		{
			if (string.IsNullOrWhiteSpace(DatabaseName))
				throw new Exception("Database Name is required.");

			string SQLText = string.Empty;

			SQLText = string.Format("DROP Database {0};", DatabaseName);
            

			Command.Connection.ConnectionString = BuildConnectionString("System");

			SQLDatabaseResultSet rs = ExecuteSQL(SQLText);
			if ((rs.RowCount > 0) && (rs.ColumnCount > 0) && (rs.Rows[0][0].Equals("SQLDATABASE_OK")) && (string.IsNullOrWhiteSpace(rs.ErrorMessage)))
				return true;
			else
				throw new Exception(rs.ErrorMessage);
		}

		public bool CreateUser(string Username, string Password)
		{
			if ((string.IsNullOrWhiteSpace(Username)) || (string.IsNullOrWhiteSpace(Password)))
				throw new Exception("Username password are required.");

			string SQLText = string.Empty;

			SQLText = string.Format("Create User {0} WITH Password {1}", Username, Password);

			Command.Connection.ConnectionString = BuildConnectionString("System");

			SQLDatabaseResultSet rs = ExecuteSQL(SQLText);
			if ((rs.RowCount > 0) && (rs.ColumnCount > 0) && (rs.Rows[0][0].Equals("SQLDATABASE_OK")) && (string.IsNullOrWhiteSpace(rs.ErrorMessage)))
				return true;
			else
				throw new Exception(rs.ErrorMessage);
		}

		public bool DropUser(string Username)
		{
			if (string.IsNullOrWhiteSpace(Username))
				throw new Exception("Username is required.");

			string SQLText = string.Empty;

			SQLText = string.Format("Drop User {0} ", Username);

			Command.Connection.ConnectionString = BuildConnectionString("System");

			SQLDatabaseResultSet rs = ExecuteSQL(SQLText);
			if ((rs.RowCount > 0) && (rs.ColumnCount > 0) && (rs.Rows[0][0].Equals("SQLDATABASE_OK")) && (string.IsNullOrWhiteSpace(rs.ErrorMessage)))
				return true;
			else
				throw new Exception(rs.ErrorMessage);
		}

		public bool ConvertToSysAdmin(string Username)
		{
			if (string.IsNullOrWhiteSpace(Username))
				throw new Exception("Username is required.");

			string SQLText = string.Empty;

			SQLText = string.Format("UPDATE Users SET SysAdmin = 1 WHERE Username = '{0}' ", Username);

			Command.Connection.ConnectionString = BuildConnectionString("System");

			SQLDatabaseResultSet rs = ExecuteSQL(SQLText);
			if ((rs.RowCount > 0) && (rs.ColumnCount > 0) && (rs.Rows[0][0].Equals("SQLDATABASE_OK")) && (string.IsNullOrWhiteSpace(rs.ErrorMessage)))
				return true;
			else
				throw new Exception(rs.ErrorMessage);
		}

		public bool CreateSchema(string DatabaseName, string SchemaName)
		{
			if ((string.IsNullOrWhiteSpace(DatabaseName)) || (string.IsNullOrWhiteSpace(SchemaName)))
				throw new Exception("DatabaseName and SchemaName are required.");

            
			string SQLText = string.Empty;
			SQLText = string.Format("Create Schema {0}", SchemaName);

			Command.Connection.ConnectionString = BuildConnectionString(DatabaseName);

			SQLDatabaseResultSet rs = ExecuteSQL(SQLText);
			if ((rs.RowCount > 0) && (rs.ColumnCount > 0) && (rs.Rows[0][0].Equals("SQLDATABASE_OK")) && (string.IsNullOrWhiteSpace(rs.ErrorMessage)))
				return true;
			else
				throw new Exception(rs.ErrorMessage);
		}

		public bool DropSchema(string DatabaseName, string SchemaName)
		{
			if ((string.IsNullOrWhiteSpace(DatabaseName)) || (string.IsNullOrWhiteSpace(SchemaName)))
				throw new Exception("DatabaseName and SchemaName are required.");


			string SQLText = string.Empty;
			SQLText = string.Format("Drop Schema {0}", SchemaName);

			Command.Connection.ConnectionString = BuildConnectionString(DatabaseName);

			SQLDatabaseResultSet rs = ExecuteSQL(SQLText);
			if ((rs.RowCount > 0) && (rs.ColumnCount > 0) && (rs.Rows[0][0].Equals("SQLDATABASE_OK")) && (string.IsNullOrWhiteSpace(rs.ErrorMessage)))
				return true;
			else
				throw new Exception(rs.ErrorMessage);
		}

		public bool ChangePassword(string Username, string Password)
		{
			if ((string.IsNullOrWhiteSpace(Username)) || (string.IsNullOrWhiteSpace(Password)))
				throw new Exception("Username password are required.");

			string SQLText = string.Empty;

			SQLText = string.Format("ALTER User {0} WITH Password {1}", Username, Password);

			Command.Connection.ConnectionString = BuildConnectionString("System");

			SQLDatabaseResultSet rs = ExecuteSQL(SQLText);
			if ((rs.RowCount > 0) && (rs.ColumnCount > 0) && (rs.Rows[0][0].Equals("SQLDATABASE_OK")) && (string.IsNullOrWhiteSpace(rs.ErrorMessage)))
				return true;
			else
				throw new Exception(rs.ErrorMessage);
		}

		public bool Grant(string DatabaseName, string Username, string Permissions = "ALL")
		{
			if ((string.IsNullOrWhiteSpace(Username)) || (string.IsNullOrWhiteSpace(DatabaseName)))
				throw new Exception("Username password are required.");

			string SQLText = string.Empty;

			SQLText = string.Format("grant {0} on database {1} to {2}", Permissions, DatabaseName, Username);

			Command.Connection.ConnectionString = BuildConnectionString("System");

			SQLDatabaseResultSet rs = ExecuteSQL(SQLText);
			if ((rs.RowCount > 0) && (rs.ColumnCount > 0) && (rs.Rows[0][0].Equals("SQLDATABASE_OK")) && (string.IsNullOrWhiteSpace(rs.ErrorMessage)))
				return true;
			else
				throw new Exception(rs.ErrorMessage);
		}

		public bool Revoke(string DatabaseName, string Username, string Permissions = "ALL")
		{
			if ((string.IsNullOrWhiteSpace(Username)) || (string.IsNullOrWhiteSpace(DatabaseName)))
				throw new Exception("Username password are required.");

			string SQLText = string.Empty;

			SQLText = string.Format("revoke {0} on database {1} from {2}", Permissions, DatabaseName, Username);

			Command.Connection.ConnectionString = BuildConnectionString("System");

			SQLDatabaseResultSet rs = ExecuteSQL(SQLText);
			if ((rs.RowCount > 0) && (rs.ColumnCount > 0) && (rs.Rows[0][0].Equals("SQLDATABASE_OK")) && (string.IsNullOrWhiteSpace(rs.ErrorMessage)))
				return true;
			else
				throw new Exception(rs.ErrorMessage);
		}
		public List<string> Schemas(string DatabaseName)
		{
			List<string> lst = new List<string>();

			if (string.IsNullOrWhiteSpace(DatabaseName))
				throw new Exception("DatabaseName is required.");

			string SQLText = string.Empty;

			SQLText = string.Format("SYSCMD database_list");

			Command.Connection.ConnectionString = BuildConnectionString(DatabaseName);
			Command.CommandText = SQLText;
			SQLDatabaseResultSet rs = Command.ExecuteReader()[0];
			if ((rs.RowCount > 0) && (rs.ColumnCount > 0) && (string.IsNullOrWhiteSpace(rs.ErrorMessage)))
				foreach (object[] r in rs.Rows) {
					if (r[1] != null)
						lst.Add(r[1].ToString());
				}
			else
				throw new Exception(rs.ErrorMessage);

			return lst;
		}

		public bool RenameDefaultSchema(string DatabaseName, string SchemaName)
		{
            
			if (string.IsNullOrWhiteSpace(DatabaseName))
				throw new Exception("DatabaseName is required.");

			if (string.IsNullOrWhiteSpace(SchemaName))
				throw new Exception("SchemaName is required.");

			string SQLText = string.Format("UPDATE Databases SET SchemaName = '{0}' WHERE DatabaseName = '{1}' ;", SchemaName, DatabaseName);

			Command.Connection.ConnectionString = BuildConnectionString("System");

			SQLDatabaseResultSet rs = ExecuteSQL(SQLText);
			if ((rs.RowCount > 0) && (rs.ColumnCount > 0) && (rs.Rows[0][0].Equals("SQLDATABASE_OK")) && (string.IsNullOrWhiteSpace(rs.ErrorMessage)))
				return true;
			else
				throw new Exception(rs.ErrorMessage);
		}

		public bool CreateIndex(string DatabaseName, string IndexName, string TableAndSchemaName, string[] Columns)
		{
			if (string.IsNullOrWhiteSpace(IndexName))
				throw new Exception("IndexName is required.");

			if (string.IsNullOrWhiteSpace(DatabaseName))
				throw new Exception("DatabaseName is required.");

			if (string.IsNullOrWhiteSpace(TableAndSchemaName))
				throw new Exception("TableAndSchemaName is required.");

			if (Columns.Length == 0)
				throw new Exception("Columns are required.");

			StringBuilder columnsString = new StringBuilder();

			foreach (string column in Columns)
				columnsString.Append(column + ",");

			if (columnsString.ToString().EndsWith(","))
				columnsString.Remove(columnsString.Length - 1, 1);

			string SQLText = string.Empty;

			SQLText = string.Format("CREATE INDEX IF NOT EXISTS {0} ON {1} ( {2} ) ;", IndexName, TableAndSchemaName, columnsString.ToString());

			Command.Connection.ConnectionString = BuildConnectionString(DatabaseName);

			SQLDatabaseResultSet rs = ExecuteSQL(SQLText);
			if ((rs.RowCount > 0) && (rs.ColumnCount > 0) && (rs.Rows[0][0].Equals("SQLDATABASE_OK")) && (string.IsNullOrWhiteSpace(rs.ErrorMessage)))
				return true;
			else
				throw new Exception(rs.ErrorMessage);

		}

		private List<string> dbObjectList(string DatabaseName, string ObjectType, string SchemaName = "")
		{
			List<string> lst = new List<string>();

			if (string.IsNullOrWhiteSpace(DatabaseName))
				throw new Exception("DatabaseName is required.");

			string SystemTable = string.Empty;

			if (string.IsNullOrWhiteSpace(SchemaName))
				SystemTable = "sys_objects";
			else
				SystemTable = SchemaName + ".sys_objects";

			string SQLText = string.Format("SELECT name from {0} WHERE type = '{1}' ;", SystemTable, ObjectType);

			Command.Connection.ConnectionString = BuildConnectionString(DatabaseName);
			Command.CommandText = SQLText;
			SQLDatabaseResultSet[] rrs = Command.ExecuteReader();

			if (rrs == null || rrs.Length == 0)
				return lst;

			SQLDatabaseResultSet rs = rrs[0];

			if (!string.IsNullOrWhiteSpace(rs.ErrorMessage))
				throw new Exception(rs.ErrorMessage);

            
			foreach (object[] obj in rs.Rows) {
				if (obj != null)
					lst.Add(obj[0].ToString());
			}

			return lst;
		}

		public List<string> Indexes(string DatabaseName, string SchemaName = "")
		{
			return dbObjectList(DatabaseName, "index", SchemaName);
		}

		public List<string> Views(string DatabaseName, string SchemaName = "")
		{
			return dbObjectList(DatabaseName, "view", SchemaName);
		}

		public List<string> Tables(string DatabaseName, string SchemaName = "")
		{
			return dbObjectList(DatabaseName, "table", SchemaName);
		}

		public List<string> Triggers(string DatabaseName, string SchemaName = "")
		{
			return dbObjectList(DatabaseName, "trigger", SchemaName);
		}

		public bool RenameTable(string DatabaseName, string SchemaAndTableName, string NewTableName)
		{
			if (string.IsNullOrWhiteSpace(DatabaseName))
				throw new Exception("DatabaseName is required.");

			if (string.IsNullOrWhiteSpace(SchemaAndTableName))
				throw new Exception("SchemaAndTableName is required.");

			if (string.IsNullOrWhiteSpace(NewTableName))
				throw new Exception("NewTableName is required.");

			Command.Connection.ConnectionString = BuildConnectionString(DatabaseName);

			string SQLText = string.Format("ALTER TABLE {0} RENAME TO {1} ;", SchemaAndTableName, NewTableName);

			SQLDatabaseResultSet rs = ExecuteSQL(SQLText);
			if ((rs.RowCount > 0) && (rs.ColumnCount > 0) && (rs.Rows[0][0].Equals("SQLDATABASE_OK")) && (string.IsNullOrWhiteSpace(rs.ErrorMessage)))
				return true;
			else
				throw new Exception(rs.ErrorMessage);
		}

		public SQLDatabaseResultSet Table(string DatabaseName, string SchemaAndTableName)
		{
			if (string.IsNullOrWhiteSpace(DatabaseName))
				throw new Exception("DatabaseName is required.");

			if (string.IsNullOrWhiteSpace(SchemaAndTableName))
				throw new Exception("TableAndSchemaName is required.");

			Command.Connection.ConnectionString = BuildConnectionString(DatabaseName);

			string SQLText = string.Format("SYSCMD table_info ('{0}') ;", SchemaAndTableName);
			Command.CommandText = SQLText;
			SQLDatabaseResultSet rs = Command.ExecuteReader()[0];
			if ((rs.RowCount > 0) && (rs.ColumnCount > 0) && (string.IsNullOrWhiteSpace(rs.ErrorMessage)))
				return rs;
			else
				throw new Exception(rs.ErrorMessage);
		}

		public SQLDatabaseResultSet Index(string DatabaseName, string SchemaAndIndexName)
		{
			if (string.IsNullOrWhiteSpace(DatabaseName))
				throw new Exception("DatabaseName is required.");

			if (string.IsNullOrWhiteSpace(SchemaAndIndexName))
				throw new Exception("IndexAndSchemaName is required.");

			Command.Connection.ConnectionString = BuildConnectionString(DatabaseName);

			string SQLText = string.Format("SYSCMD index_info ('{0}') ;", SchemaAndIndexName);
			Command.CommandText = SQLText;
			SQLDatabaseResultSet rs = Command.ExecuteReader()[0];
			if ((rs.RowCount > 0) && (rs.ColumnCount > 0) && (string.IsNullOrWhiteSpace(rs.ErrorMessage)))
				return rs;
			else
				throw new Exception(rs.ErrorMessage);
		}

		public SQLDatabaseResultSet TableIndexes(string DatabaseName, string SchemaAndTableName)
		{
			if (string.IsNullOrWhiteSpace(DatabaseName))
				throw new Exception("DatabaseName is required.");

			if (string.IsNullOrWhiteSpace(SchemaAndTableName))
				throw new Exception("SchemaAndTableName is required.");

			Command.Connection.ConnectionString = BuildConnectionString(DatabaseName);

			string SQLText = string.Format("SYSCMD index_list ('{0}') ;", SchemaAndTableName);
			Command.CommandText = SQLText;
			SQLDatabaseResultSet rs = Command.ExecuteReader()[0];
			if ((rs.RowCount > 0) && (rs.ColumnCount > 0) && (string.IsNullOrWhiteSpace(rs.ErrorMessage)))
				return rs;
			else
				throw new Exception(rs.ErrorMessage);
		}

		public SQLDatabaseResultSet TableForeignKeys(string DatabaseName, string SchemaAndTableName)
		{
			if (string.IsNullOrWhiteSpace(DatabaseName))
				throw new Exception("DatabaseName is required.");

			if (string.IsNullOrWhiteSpace(SchemaAndTableName))
				throw new Exception("SchemaAndTableName is required.");

			Command.Connection.ConnectionString = BuildConnectionString(DatabaseName);

			string SQLText = string.Format("SYSCMD foreign_key_list ('{0}') ;", SchemaAndTableName);
			Command.CommandText = SQLText;
			SQLDatabaseResultSet rs = Command.ExecuteReader()[0];
			if ((rs.RowCount > 0) && (rs.ColumnCount > 0) && (string.IsNullOrWhiteSpace(rs.ErrorMessage)))
				return rs;
			else
				throw new Exception(rs.ErrorMessage);
		}

		public string TrackChanges(string DatabaseName, bool StatusOnly = true, bool Enable = false)
		{
			if (string.IsNullOrWhiteSpace(DatabaseName))
				throw new Exception("DatabaseName is required.");

			Command.Connection.ConnectionString = BuildConnectionString(DatabaseName);

			string EnableDisableText = string.Empty;

			if (Enable)
				EnableDisableText = "on";
			else
				EnableDisableText = "off";

			string SQLText = string.Format("SYSCMD track_changes_status {0};", StatusOnly == false ? Enable == true ? "= on" : "= off" : string.Empty);
			Command.CommandText = SQLText;
			SQLDatabaseResultSet rs = Command.ExecuteReader()[0];
			if ((rs.RowCount > 0) && (rs.ColumnCount > 0) && (string.IsNullOrWhiteSpace(rs.ErrorMessage)))
				return rs.Rows[0][0].ToString();
			else
				throw new Exception(rs.ErrorMessage);
		}

		public SQLDatabaseResultSet ViewChanges(string DatabaseName)
		{
			if (string.IsNullOrWhiteSpace(DatabaseName))
				throw new Exception("DatabaseName is required.");

			Command.Connection.ConnectionString = BuildConnectionString(DatabaseName);

			string SQLText = string.Format("SYSCMD view_tracked_changes;");
			Command.CommandText = SQLText;
			SQLDatabaseResultSet rs = Command.ExecuteReader()[0];
			if (string.IsNullOrWhiteSpace(rs.ErrorMessage))
				return rs;
			else
				throw new Exception(rs.ErrorMessage);
		}

		public SQLDatabaseResultSet Explain(string DatabaseName, string commandText)
		{
			if (string.IsNullOrWhiteSpace(DatabaseName))
				throw new Exception("DatabaseName is required.");

			Command.Connection.ConnectionString = BuildConnectionString(DatabaseName);            
			Command.CommandText = string.Format("EXPLAIN {0} ", commandText);            
			SQLDatabaseResultSet rs = Command.ExecuteReader()[0];
			if (string.IsNullOrWhiteSpace(rs.ErrorMessage))
				return rs;
			else
				throw new Exception(rs.ErrorMessage);
		}

		public SQLDatabaseResultSet ExplainQueryPlan(string DatabaseName, string commandText)
		{
			if (string.IsNullOrWhiteSpace(DatabaseName))
				throw new Exception("DatabaseName is required.");

			Command.Connection.ConnectionString = BuildConnectionString(DatabaseName);

			Command.CommandText = string.Format("EXPLAIN QUERY PLAN {0} ", commandText);
			SQLDatabaseResultSet rs = Command.ExecuteReader()[0];
			if (string.IsNullOrWhiteSpace(rs.ErrorMessage))
				return rs;
			else
				throw new Exception(rs.ErrorMessage);
		}

		public SQLDatabaseResultSet[] Analyze(string DatabaseName, string SchemaAndOrTableName = "")
		{
			if (string.IsNullOrWhiteSpace(DatabaseName))
				throw new Exception("DatabaseName is required.");

			Command.Connection.ConnectionString = BuildConnectionString(DatabaseName, true);

			Command.CommandText = string.Format("ANALYZE '{0}' ;", SchemaAndOrTableName);
			Command.ExecuteNonQuery();

			Command.CommandText = string.Format("SELECT * FROM sys_stat1; SELECT * FROM sys_stat2;");
			SQLDatabaseResultSet[] rs = Command.ExecuteReader();

			foreach (SQLDatabaseResultSet r in rs) {
				if (!string.IsNullOrWhiteSpace(r.ErrorMessage))
					throw new Exception(r.ErrorMessage);
			}

			return rs;
		}

		public bool SecureDelete(string DatabaseName, bool StatusOnly = true, bool Enable = false)
		{
			if (string.IsNullOrWhiteSpace(DatabaseName))
				throw new Exception("DatabaseName is required.");

			Command.Connection.ConnectionString = BuildConnectionString(DatabaseName);

			Command.CommandText = string.Format("SYSCMD secure_delete {0} ;", StatusOnly == false ? Enable == true ? "= 1" : "= 0" : string.Empty);
			SQLDatabaseResultSet rs = Command.ExecuteReader()[0];
			if (string.IsNullOrWhiteSpace(rs.ErrorMessage))
			if (rs.Rows[0][0].ToString().Trim().Equals("1"))
				return true;
			else
				return false;
			else
				throw new Exception(rs.ErrorMessage);
		}

		public SQLDatabaseResultSet SystemCommand(string DatabaseName, string SYSCMD, string CommandValue = "")
		{
			if (string.IsNullOrWhiteSpace(DatabaseName))
				throw new Exception("DatabaseName is required.");

			if (string.IsNullOrWhiteSpace(SYSCMD))
				throw new Exception("SYSCMD is required.");

			Command.Connection.ConnectionString = BuildConnectionString(DatabaseName);

			Command.CommandText = string.Format("SYSCMD {0} {1} ;", SYSCMD, CommandValue == "" ? string.Empty : " = " + CommandValue);
			SQLDatabaseResultSet rs = Command.ExecuteReader()[0];
			if (string.IsNullOrWhiteSpace(rs.ErrorMessage))
				return rs;
			else
				throw new Exception(rs.ErrorMessage);
		}

		public string Encoding(string DatabaseName)
		{
			if (string.IsNullOrWhiteSpace(DatabaseName))
				throw new Exception("DatabaseName is required.");

			Command.Connection.ConnectionString = BuildConnectionString(DatabaseName);
			Command.CommandText = string.Format("SYSCMD encoding ;");
			SQLDatabaseResultSet rs = Command.ExecuteReader()[0];
			if ((string.IsNullOrWhiteSpace(rs.ErrorMessage)) && (rs.Rows[0][0] != null))
				return rs.Rows[0][0].ToString();
			else
				throw new Exception(rs.ErrorMessage);
		}

		public SQLDatabaseResultSet Locks(string DatabaseName)
		{
			if (string.IsNullOrWhiteSpace(DatabaseName))
				throw new Exception("DatabaseName is required.");

			Command.Connection.ConnectionString = BuildConnectionString(DatabaseName);

			Command.CommandText = string.Format("SYSCMD lock_status ;");
			SQLDatabaseResultSet rs = Command.ExecuteReader()[0];
			if (string.IsNullOrWhiteSpace(rs.ErrorMessage))
				return rs;
			else
				throw new Exception(rs.ErrorMessage);
		}

		public List<string> CollationList(string DatabaseName)
		{
			List<string> lst = new List<string>();
			if (string.IsNullOrWhiteSpace(DatabaseName))
				throw new Exception("DatabaseName is required.");

			Command.Connection.ConnectionString = BuildConnectionString(DatabaseName);

			Command.CommandText = string.Format("SYSCMD collation_list ;");
			SQLDatabaseResultSet rs = Command.ExecuteReader()[0];
			if (string.IsNullOrWhiteSpace(rs.ErrorMessage))
				foreach (object[] r in rs.Rows) {
					if ((r.Length > 0) && (r[1] != null))
						lst.Add(r[1].ToString());
				}
			else
				throw new Exception(rs.ErrorMessage);

			return lst;
		}

		public string Collation(string DatabaseName, bool StatusOnly = true, string NewCollation = "")
		{            
			if (string.IsNullOrWhiteSpace(DatabaseName))
				throw new Exception("DatabaseName is required.");

			Command.Connection.ConnectionString = BuildConnectionString(DatabaseName);

			Command.CommandText = string.Format("SYSCMD collating_sequence {0} ;", StatusOnly == false ? "= '" + NewCollation + "'" : string.Empty);
			SQLDatabaseResultSet rs = Command.ExecuteReader()[0];
			if (string.IsNullOrWhiteSpace(rs.ErrorMessage))
				return rs.Rows[0][0].ToString();
			else
				throw new Exception(rs.ErrorMessage);

		}

		/// <summary>
		/// Integrity check verifies data pages in the file, function can be time consuming.
		/// </summary>
		/// <param name="DatabaseName"></param>
		public SQLDatabaseResultSet IntegrityCheck(string DatabaseName, bool QuickCheck = true)
		{
			if (string.IsNullOrWhiteSpace(DatabaseName))
				throw new Exception("DatabaseName is required.");

			Command.Connection.ConnectionString = BuildConnectionString(DatabaseName);

			Command.CommandText = string.Format("SYSCMD integrity_check {0} ;", QuickCheck == true ? "= 'q'" : string.Empty);
			SQLDatabaseResultSet rs = Command.ExecuteReader()[0];
			if (string.IsNullOrWhiteSpace(rs.ErrorMessage))
				return rs;
			else
				throw new Exception(rs.ErrorMessage);

		}

		/// <summary>
		/// rebuilds index, if name of table or index is omitted then all are rebuilt.
		/// </summary>
		/// <param name="DatabaseName"></param>
		public string ReIndex(string DatabaseName, string TableOrIndexName = "")
		{
			if (string.IsNullOrWhiteSpace(DatabaseName))
				throw new Exception("DatabaseName is required.");

			Command.Connection.ConnectionString = BuildConnectionString(DatabaseName);

			Command.CommandText = string.Format("REINDEX {0} ;", TableOrIndexName == "" ? string.Empty : "'" + TableOrIndexName + "'");
			SQLDatabaseResultSet rs = Command.ExecuteReader()[0];
			if (string.IsNullOrWhiteSpace(rs.ErrorMessage))
			if ((rs.RowCount == 0) && (rs.ColumnCount == 0))
				return "SQLDATABASE_OK";
			else
				return rs.Rows[0][0].ToString();
			else
				throw new Exception(rs.ErrorMessage);

		}


	}


	public class SQLDatabaseOrmClient<T> where T : class, new()
	{
		public SQLDatabaseConnection Connection { get; set; }

		public Int64 RowsAffected { get; set; }

		public SQLDatabaseOrmClient()
		{

		}
		public SQLDatabaseOrmClient(SQLDatabaseConnection ConnectionObject)
		{
			Connection = ConnectionObject;
		}

		public IList<T> Find(IFilter<T> filter)
		{
			T entity = new T();
			return ExecuteGet<T>(filter);
		}
		public IList<T> GetAll()
		{
			T entity = new T();
			return ExecuteGet(string.Format("SELECT * FROM [{0}]", entity.GetType().Name));
		}

		public IList<T> GetAll(string commandText)
		{
			return ExecuteGet(commandText);
		}

		public IList<TEntity> GetAll<TEntity>(string commandText)
            where TEntity : class, new()
		{
			return ExecuteGet<TEntity>(commandText);
		}

		private long Execute(string commandText, bool returnIdentity = false)
		{
			using (var cmd = new SQLDatabaseCommand()) {
				cmd.Connection = Connection;
				cmd.CommandText = commandText;
				SQLDatabaseResultSet[] reader;
				if (returnIdentity) {
					try {
						return (long)cmd.ExecuteScalar()[0].Rows[0][0];
					} catch {
						return 0;
					}
				} else {
					reader = cmd.ExecuteNonQuery();                    
					return 0;
				}
			}
		}
		/// <summary>
		/// Execute and return rows affected
		/// </summary>
		/// <param name="commandText"></param>        
		private long Execute(string commandText)
		{
			using (var cmd = new SQLDatabaseCommand()) {
				cmd.Connection = Connection;
				SQLDatabaseResultSet[] reader;
				cmd.CommandText = commandText;
				reader = cmd.ExecuteNonQuery();
				if (reader != null) {
					if (string.IsNullOrWhiteSpace(reader[0].ErrorMessage))
						return reader[0].RowsAffected;
					else
						throw new Exception(reader[0].ErrorMessage);
				} else
					return -1;
			}
		}

		private IList<T> ExecuteGet(string commandText) {
			using (var cmd = new SQLDatabaseCommand()) {
				cmd.Connection = Connection;
				cmd.CommandText = commandText;
				SQLDatabaseResultSet[] reader = cmd.ExecuteReader();
				return new EntityMapper().Map<T>(reader[0]);
			}
		}
		private IList<TEntity> ExecuteGet<TEntity>(string commandText)
            where TEntity : class, new() {
			using (var cmd = new SQLDatabaseCommand()) {
				cmd.Connection = Connection;
				cmd.CommandText = commandText;
				SQLDatabaseResultSet[] reader = cmd.ExecuteReader();
				return new EntityMapper().Map<TEntity>(reader[0]);
			}
		}
		private IList<TEntity> ExecuteGet<TEntity>(IFilter<TEntity> filter)
            where TEntity : class, new() {
			using (var cmd = new SQLDatabaseCommand()) {
				cmd.Connection = Connection;
				cmd.CommandText = filter.Query;
				SQLDatabaseResultSet[] reader = cmd.ExecuteReader();
				return new EntityMapper().Map<TEntity>(reader[0]);
			}
		}

		private IList<TEntity> ExecuteGet<TEntity>(SQLDatabaseResultSet reader)
            where TEntity : class, new() {
			return new EntityMapper().Map<TEntity>(reader);
		}
		private IList<PropertyInfo> GetPropertyInfoList(T entity) {
			return entity.GetType().GetProperties()
                .Where(p => p.CustomAttributes.FirstOrDefault(x => x.AttributeType == typeof(DBColumnAttribute)) != null).ToList();
		}
		private IList<PropertyInfo> GetPropertyInfoList<TEntity>(TEntity entity) {
			return entity.GetType().GetProperties()
                .Where(p => p.CustomAttributes.FirstOrDefault(x => x.AttributeType == typeof(DBColumnAttribute)) != null).ToList();
		}

		private object GetActualValue(object memberValue) {
			object Value = new object();

			string typename = ClrToDBType(memberValue.GetType().Name);

			if (typename.Equals("Text"))
				Value = string.Format("'{0}'", memberValue.ToString().Replace("'", "''"));
			else if (typename.Equals("Integer"))
				Value = string.Format("{0}", memberValue);
			else if (typename.Equals("Real"))
				Value = string.Format("{0}", memberValue);
			else if (typename.Equals("None"))
				Value = string.Format("'{0}'", memberValue).ToString().Replace("'", "''");
			else
				Value = string.Format("'{0}'", memberValue.ToString().Replace("'", "''"));

			return Value;
		}

		public void CreateTable(T entity) {
            
			StringBuilder primarykeys = new StringBuilder();
			StringBuilder foreignkeys = new StringBuilder();
			StringBuilder columns = new StringBuilder();
			StringBuilder Unique = new StringBuilder();

			IList<PropertyInfo> propertyInfos = GetPropertyInfoList(entity);

			bool IsFirst = true;
			bool HasPrimaryKey = false;

			foreach (PropertyInfo i in propertyInfos) {
				var ca = i.GetCustomAttribute(typeof(DBColumnAttribute)) as DBColumnAttribute;
                
				if (ca != null) {
					if (!IsFirst) {
						columns.Append(",");
					}
					columns.Append(string.Format("{0} {1} ", i.Name, ClrToDBType(i.PropertyType.Name)));

					if ((ca.NotNull) && (!ca.PrimaryKey))
						columns.Append(" NOT NULL ");

					if ((ca.Unique) && (!ca.PrimaryKey))
						columns.Append(" UNIQUE ");

					if (ca.PrimaryKey) {
						HasPrimaryKey = true;
						if (ca.AutoIncrement)
							columns.Append(" Primary Key AutoIncrement NOT NULL ");
						else
							columns.Append(" Primary Key NOT NULL ");
					}
                        

					if (ca.CombinedPrimaryKey) {
						if (HasPrimaryKey)
							throw new Exception("Primary key already defined. A table cannot have both column level primary key and table level primary keys.");
						else
							primarykeys.Append(i.Name + ",");
					}
                        

					if ((ca.ForeignKey) && (!string.IsNullOrWhiteSpace(ca.ForeignKeyTable)) && (!string.IsNullOrWhiteSpace(ca.ForeignKeyColumn))) {
						foreignkeys.Append(string.Format(" , FOREIGN KEY ({0}) REFERENCES {1}({2}) ", i.Name, ca.ForeignKeyTable, ca.ForeignKeyColumn));
					}
                    
				}
				IsFirst = false;
			}

			if (columns.ToString() != string.Empty) {

				if (columns.ToString().EndsWith(","))
					columns.Remove(columns.Length - 1, 1);

                
				string primarykey = string.Empty;

				if ((!string.IsNullOrWhiteSpace(primarykeys.ToString()))
				                && (columns.ToString().IndexOf("Primary Key") == -1)) {

					primarykeys.Remove(primarykeys.Length - 1, 1);
					primarykey = ", PRIMARY KEY (" + primarykeys.ToString() + ") ";
				} else {
					primarykey = string.Empty;
				}
                    

				StringBuilder qry = new StringBuilder();
				qry.Append(string.Format("CREATE TABLE IF NOT EXISTS [{0}] ( {1} {2} {3} );"
                    , entity.GetType().Name, columns, primarykey, foreignkeys.ToString()));


				//Console.WriteLine(qry.ToString());
				Execute(qry.ToString(), false);
                
			}

		}

	
		public void DropTable(T entity) {
			StringBuilder qry = new StringBuilder();
			qry.Append(string.Format("DROP TABLE IF EXISTS [{0}];", entity.GetType().Name));
			//Console.WriteLine(qry.ToString());
			Execute(qry.ToString(), false);
		}


		public long Add(T entity) {
			long identity = 0;
			bool hasIdentity = false;

			StringBuilder columns = new StringBuilder();
			StringBuilder values = new StringBuilder();

			IList<PropertyInfo> propertyInfos = GetPropertyInfoList(entity);

			foreach (PropertyInfo i in propertyInfos) {
				var ca = i.GetCustomAttribute(typeof(DBColumnAttribute)) as DBColumnAttribute;

				if (ca != null) {
					if (!ca.AutoIncrement) {
						columns.Append(string.Format("[{0}],", i.Name));
						values.Append(string.Format("{0},",
							i.GetValue(entity) == null ? "NULL" : string.Format("{0}", GetActualValue(i.GetValue(entity)))));
					} else {
						hasIdentity = true;
					}
				}
			}

			if (columns.ToString() != string.Empty) {

				columns.Remove(columns.Length - 1, 1); 
				values.Remove(values.Length - 1, 1); 

				StringBuilder qry = new StringBuilder();
				qry.Append(string.Format("INSERT INTO [{0}] ( {1} ) VALUES ( {2} ); SELECT last_insert_rowid();"
                    , entity.GetType().Name, columns, values));


				identity = hasIdentity ? Execute(qry.ToString(), true) : Execute(qry.ToString());
				//Console.WriteLine(qry.ToString());
			}

			return identity;
		}

		public void AddRange(List<T> entities) {
			StringBuilder qry = new StringBuilder();
			foreach (T entity in entities) {
				StringBuilder columns = new StringBuilder();
				StringBuilder values = new StringBuilder();

				IList<PropertyInfo> propertyInfos = GetPropertyInfoList(entity);

				foreach (PropertyInfo i in propertyInfos) {
					var ca = i.GetCustomAttribute(typeof(DBColumnAttribute)) as DBColumnAttribute;

					if (ca != null) {
						if (!ca.AutoIncrement) {
							columns.Append(string.Format("[{0}],", i.Name));

							values.Append(string.Format("{0},",
								i.GetValue(entity) == null ? "NULL" : string.Format("{0}", GetActualValue(i.GetValue(entity)))));
						}
					}
				}

				if (columns.ToString() != string.Empty) {
					columns.Remove(columns.Length - 1, 1); 
					values.Remove(values.Length - 1, 1);

					qry.AppendLine(string.Format("INSERT INTO [{0}] ( {1} ) VALUES ( {2} );"
                        , entity.GetType().Name, columns, values));
				}
			}

			try {
				Execute(qry.ToString());
			} catch (Exception ex) {

				throw ex;
			}

		}

		public void Remove(IFilter<T> filter){
			RowsAffected = Execute(filter.QueryDelete);
		}
		public void RemoveAll<T>() {
			Type t = typeof(T);
			RowsAffected = Execute(string.Format("DELETE FROM [{0}]", t.GetType().Name));
		}
		public void Remove(string commandText) {
			RowsAffected = Execute(commandText);
		}
		public void Remove(T entity) {
            
			StringBuilder clause = new StringBuilder();

			IList<PropertyInfo> propertyInfos = GetPropertyInfoList(entity);
			foreach (PropertyInfo i in propertyInfos) {
				var ca = i.GetCustomAttribute(typeof(DBColumnAttribute)) as DBColumnAttribute;

				if (ca != null) {
					if (clause.ToString() != string.Empty)
						clause.Append(" AND ");

					clause.Append(string.Format("[{0}] = {1}", i.Name, GetActualValue(i.GetValue(entity))));
				}
			}

			if (clause.ToString() != string.Empty) {
                
				StringBuilder qry = new StringBuilder();
				qry.Append(string.Format("DELETE FROM [{0}] WHERE {1} ;"
                    , entity.GetType().Name, clause));

				RowsAffected = Execute(qry.ToString());
			}
		}

		public void RemoveById(object id) {
			T entity = new T();
			StringBuilder clause = new StringBuilder();

			IList<PropertyInfo> pInfos = GetPropertyInfoList(entity);

			foreach (var pi in pInfos) {
				var pk = pi.GetCustomAttribute(typeof(DBColumnAttribute)) as DBColumnAttribute;
				if (pk != null && pk.PrimaryKey) {
					clause.Append(string.Format("[{0}]= {1} ", pi.Name, GetActualValue(id)));
					break;
				}
			}

			if (clause.ToString() != string.Empty) {
				StringBuilder qry = new StringBuilder();
				qry.Append(string.Format("DELETE FROM [{0}] WHERE {1}", entity.GetType().Name, clause));
				RowsAffected = Execute(qry.ToString());
			}
            
		}

		public void Update(T entity) {
			StringBuilder columns = new StringBuilder();
			StringBuilder clause = new StringBuilder();

			IList<PropertyInfo> propertyInfos = GetPropertyInfoList(entity);
			foreach (PropertyInfo i in propertyInfos) {
				var ca = i.GetCustomAttribute(typeof(DBColumnAttribute)) as DBColumnAttribute;

				if (ca != null) {
					if (!ca.PrimaryKey) {
						columns.Append(string.Format("[{0}] = {1},", i.Name,
							i.GetValue(entity) == null ? "NULL" : GetActualValue(i.GetValue(entity))));
					} else {
						clause.Append(string.Format("[{0}] = {1}", i.Name, GetActualValue(i.GetValue(entity))));
					}
				}
			}
            
			if (columns.ToString() != string.Empty) {
				if (columns.ToString().EndsWith(","))
					columns.Remove(columns.Length - 1, 1);

				StringBuilder qry = new StringBuilder();
				qry.Append(string.Format("UPDATE [{0}] SET {1} WHERE {2};"
                    , entity.GetType().Name, columns, clause));

				RowsAffected = Execute(qry.ToString());
			}
		}

		public void UpdateRange(IList<T> entities) {
			StringBuilder qry = new StringBuilder();
			foreach (T entity in entities) {
				StringBuilder columns = new StringBuilder();
				StringBuilder clause = new StringBuilder();
                               
				IList<PropertyInfo> propertyInfos = GetPropertyInfoList(entity);
				foreach (PropertyInfo i in propertyInfos) {
					var ca = i.GetCustomAttribute(typeof(DBColumnAttribute)) as DBColumnAttribute;

					if (ca != null) {
						if (!ca.PrimaryKey) {
							columns.Append(string.Format("[{0}] = {1},", i.Name,
								i.GetValue(entity) == null ? "NULL" : string.Format("'{0}'", GetActualValue(i.GetValue(entity)))));
						} else {
							clause.Append(string.Format("[{0}] = {1}", i.Name, GetActualValue(i.GetValue(entity))));
						}
					}
				}

				if (columns.ToString() != string.Empty) {
					if (columns.ToString().EndsWith(","))
						columns.Remove(columns.Length - 1, 1);

					qry.AppendLine(string.Format("UPDATE [{0}] SET {1} WHERE {2};"
                        , entity.GetType().Name, columns, clause));
				}
               
			}

			RowsAffected = Execute(qry.ToString());
		}

		public T GetById(object id) {
			T entity = new T();
			StringBuilder clause = new StringBuilder();

			IList<PropertyInfo> pInfos = GetPropertyInfoList(entity);

			foreach (var pi in pInfos) {
				var pk = pi.GetCustomAttribute(typeof(DBColumnAttribute)) as DBColumnAttribute;
				if (pk != null && pk.PrimaryKey) {
					clause.Append(string.Format("[{0}]='{1}'", pi.Name, id));
					break;
				}
			}

			if (clause.ToString() != string.Empty) {
				StringBuilder qry = new StringBuilder();
				qry.Append(string.Format("SELECT * FROM [{0}] WHERE {1}", entity.GetType().Name, clause));
				var _entities = ExecuteGet(qry.ToString());
				if (_entities != null && _entities.Count > 0)
					entity = _entities[0];
			}


			return entity;
		}

		public IList<T> Find(IEnumerable<object> ids) {
			IList<T> entities = new List<T>();
			StringBuilder clause = new StringBuilder();

			var entity = new T();
			IList<PropertyInfo> pInfos = GetPropertyInfoList(entity);

			foreach (var pi in pInfos) {
				var pk = pi.GetCustomAttribute(typeof(DBColumnAttribute)) as DBColumnAttribute;
				if (pk != null && pk.PrimaryKey) {
					string _ids = string.Empty;
					foreach (var id in ids) {
						if (_ids != string.Empty)
							_ids = _ids + ",";

						_ids = _ids + id.ToString();
					}

					clause.Append(string.Format("[{0}] IN ({1})", pi.Name, _ids));
					break;
				}
			}

			if (clause.ToString() != string.Empty) {
				StringBuilder qry = new StringBuilder();
				qry.Append(string.Format("SELECT * FROM [{0}] WHERE {1}", entity.GetType().Name, clause));
				entities = ExecuteGet(qry.ToString());
			}

			return entities;
		}

		public class EntityMapper {
			public IList<T> Map<T>(SQLDatabaseResultSet reader)
                where T : class, new() {
				IList<T> collection = new List<T>();
				for (int r = 0; r < reader.RowCount; r++) {
					T obj = new T();
					int c = 0;
					foreach (PropertyInfo i in obj.GetType().GetProperties()
                        .Where(p => p.CustomAttributes.FirstOrDefault(x => x.AttributeType == typeof(DBColumnAttribute)) != null).ToList()) {

						try {
							var ca = i.GetCustomAttribute(typeof(DBColumnAttribute));
                            
							if (ca != null) {
								//i.SetValue(obj, ColumnValue(reader, r, i.Name));
								//i.SetValue(obj, Convert.ChangeType(ColumnValue(reader, r, i.Name), i.PropertyType));
								if (ColumnValue(reader, r, i.Name) != DBNull.Value)
									i.SetValue(obj, ColumnValue(reader, r, i.Name));
							}
						} catch (Exception ex) {
							Console.WriteLine(ex.Message);
							Console.ReadLine();
							throw ex;
						}
						c++;
					}

					collection.Add(obj);
				}

				return collection;
			}

			public object ColumnValue(SQLDatabaseResultSet rs, int RowIndex, int ColumnIndex) {
				if ((RowIndex > -1) && (ColumnIndex > -1)) {
					if (RowIndex > rs.RowCount)
						throw new Exception("Row index is out of range");

					if (ColumnIndex > rs.ColumnCount)
						throw new Exception("Column index is out of range");

					return rs.Rows[RowIndex][ColumnIndex];
				} else {
					throw new Exception("Invalid Row or Column index");
				}

			}

			public object ColumnValue(SQLDatabaseResultSet rs, int RowIndex, string ColumnName) {
				if ((RowIndex > -1) && (!string.IsNullOrWhiteSpace(ColumnName))) {
					if (RowIndex > rs.RowCount)
						throw new Exception("Row index is out of range");

					int ColIndex = -1;
					foreach (string col in rs.Columns) {
						ColIndex++;
						if (col.Equals(ColumnName, StringComparison.CurrentCultureIgnoreCase))
							break;
					}

					if ((ColIndex == -1) || (ColIndex > rs.ColumnCount))
						throw new Exception(string.Format("Column {0} not found.", ColumnName));

					return rs.Rows[RowIndex][ColIndex];

				} else {
					throw new Exception("Column name and valid row index are required.");
				}

			}

		}


		public interface IFilter<T> where T : class, new() {
			string EntityName { get; }
			string Query { get; }
			string Clause { get; }
			string QueryDelete { get; }

			void Add(Expression<Func<T, object>> memberExpression, object memberValue);
		}

		public class Filter<T> : IFilter<T> where T : class, new() {

			public Filter() {
				_Query = new StringBuilder();
				EntityName = typeof(T).Name;
			}

			private object GetMemberValue(object memberValue) {
				object Value = new object();

				string typename = ClrToDBType(memberValue.GetType().Name);

				if (typename.Equals("Text"))
					Value = string.Format("'{0}'", memberValue.ToString().Replace("'", "''"));
				else if (typename.Equals("Integer"))
					Value = string.Format("{0}", memberValue);
				else if (typename.Equals("Real"))
					Value = string.Format("{0}", memberValue);
				else if (typename.Equals("None"))
					Value = string.Format("'{0}'", memberValue.ToString().Replace("'", "''"));
				else
					Value = string.Format("'{0}'", memberValue.ToString().Replace("'", "''"));

				return Value;
			}

			public void Add(Expression<Func<T, object>> memberExpression, object memberValue) {

				if (_Query.ToString() != string.Empty)
					_Query.Append(" AND ");

				_Query.Append(string.Format(" [{0}] = {1}", NameOf(memberExpression), memberValue == null ? "NULL" : GetMemberValue(memberValue)));
                
			}

			public void AddOr(Expression<Func<T, object>> memberExpression, object memberValue)	{

				if (_Query.ToString() != string.Empty)
					_Query.Append(" OR ");

				_Query.Append(string.Format(" [{0}] = {1}", NameOf(memberExpression), memberValue == null ? "NULL" : GetMemberValue(memberValue)));

			}

			public void Contains(Expression<Func<T, object>> memberExpression, object memberValue) {

				if (_Query.ToString() != string.Empty)
					_Query.Append(" AND ");

				_Query.Append(string.Format(" [{0}] LIKE {1}", NameOf(memberExpression), memberValue == null ? "NULL" : string.Format("'%{0}%'", memberValue)));

			}

			public void OrderBy(Expression<Func<T, object>> memberExpression, object sortOrder) {

				if (_Query.ToString() != string.Empty) {
					if (_Query.ToString().IndexOf(" ORDER BY ") > -1)
						_Query.Append(" , ");
					else
						_Query.Append(" ORDER BY ");
				}

				_Query.Append(string.Format("{0} {1}", NameOf(memberExpression), sortOrder == null ? "ASC" : string.Format("{0}", sortOrder)));
			}

			public void Limit(int Limit, int OffSet = 0) {

				if (_Query.ToString() != string.Empty)
					_Query.Append("");

				_Query.Append(string.Format(" LIMIT {0} OFFSET {1}", Limit, OffSet));

			}

			public string EntityName { get; private set; }

			private readonly StringBuilder _Query;

			public string Query {
				get {
					return string.Format("SELECT * FROM [{0}] {1} {2};"
                        , EntityName
                        , _Query.ToString() == string.Empty ? string.Empty : "WHERE"
                        , _Query.ToString());
				}
			}

			public string Clause {
				get {
					return string.Format("{0} {1} ;"                        
                        , _Query.ToString() == string.Empty ? string.Empty : "WHERE"
                        , _Query.ToString());
				}
			}
			public string QueryDelete {
				get {
					return string.Format("DELETE FROM [{0}] {1} {2};"
                        , EntityName
                        , _Query.ToString() == string.Empty ? string.Empty : "WHERE"
                        , _Query.ToString());
				}
			}

			private string NameOf(Expression<Func<T, object>> exp) {
				MemberExpression body = exp.Body as MemberExpression;

				if (body == null) {
					UnaryExpression ubody = (UnaryExpression)exp.Body;
					body = ubody.Operand as MemberExpression;
				}

				return body.Member.Name;
			}
		}

		private static string ClrToDBType(string ClrType)
		{
			ClrType = ClrType.ToLowerInvariant().Trim();

			switch (ClrType) {
				case "dateTimeOffset":
				case "dateTime":
				case "char":
				case "string":
					return "Text";
				case "double":
				case "decimal":
				case "float":
					return "Real";
				case "integer":
				case "int":
				case "int16":
				case "int32":
				case "int64":
				case "uint":
				case "uint16":
				case "uint32":
				case "uint64":
				case "bool":
				case "boolean":
					return "Integer";
				default:
					return "None";
			}
		}
		private static string DBTypeToClrType(string DBType)
		{
			DBType = DBType.ToLowerInvariant().Trim();

			switch (DBType) {               
				case "Text":
					return "string";
				case "Real":
					return "double";
				case "integer":               
					return "Int64"; 
				default:
					return "object";
			}
		}
	}
}

[Serializable()] public class SQLDatabaseResultSet
{
	public int Position { get; set; }
	public string SQLText { get; set; }
	public List<string> Schemas { get; set; }
	public List<string> Tables { get; set; }
	public List<string> Parameters { get; set; }
	public long ProcessingTime { get; set; }
	public List<string> Columns { get; set; }
	public List<string> DataTypes { get; set; }
	public List<object[]> Rows { get; set; }
	public int RowCount { get; set; }
	public int ColumnCount { get; set; }
	public int RowsAffected { get; set; }
	public string ErrorMessage { get; set; }
	public SQLDatabaseResultSet()
	{ 
		Position = 0; 
		SQLText = ""; 
		Schemas = new List<string>(); 
		Tables = new List<string>(); 
		Parameters = new List<string>(); 
		ProcessingTime = 0L; 
		Columns = new List<string>();
		DataTypes = new List<string>(); 
		Rows = new List<object[]>(); 
		RowCount = 0;
		ColumnCount = 0; 
		RowsAffected = 0;
		ErrorMessage = string.Empty; 
	}
}
