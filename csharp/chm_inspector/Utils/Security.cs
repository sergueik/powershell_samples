using System;
using System.IO;

namespace Utils {

	// Mark-of-the-Web (MOTW), is a security feature in Windows - 
	// implemented through com.apple.quarantine extended attribute on MacOS
	public class Security {
		
        public static Nullable<int> PeekMotwZone(string filePath) {
			
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException("filePath");

            if (!File.Exists(filePath))
                return null;
			
            string motwPath = filePath + ":Zone.Identifier";

            if (!File.Exists(motwPath))
                return null;
			//  open alternate data streams
            try {
                using (var reader = new StreamReader(motwPath)) {
                    string line;
                    while ((line = reader.ReadLine()) != null)  {
                        line = line.Trim();
                        if (line.StartsWith("ZoneId=", StringComparison.OrdinalIgnoreCase)) {
                            string value = line.Substring("ZoneId=".Length);
                            int zone;
                            if (Int32.TryParse(value, out zone))
                                return new Nullable<int>(zone);
                        }
                    }
                }
            } catch {
                // Unable to read stream, silently return null
            }
            return null;
        }

        public static void RemoveMotw(string filePath) {
            if (string.IsNullOrEmpty(filePath))
                return;

            string motwPath = filePath + ":Zone.Identifier";

            if (!File.Exists(motwPath))
                return;

            try {
                File.Delete(motwPath);
            } catch {
                // Ignore errors; can't remove stream
            }
        }
	}
}
