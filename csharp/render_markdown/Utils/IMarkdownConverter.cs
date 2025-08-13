namespace Utils {
	public interface IMarkdownConverter {
		string convert();
		// no arguments
		string convert(string payload);
		string convertFile(string filePath);
	}
}
