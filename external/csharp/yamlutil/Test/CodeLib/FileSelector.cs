using System;
using System.Windows.Forms;

namespace QiHe.CodeLib
{
	public enum FileType
	{
		Txt,
		Rtf,
		Html,
		Xml,
		PDF,
		Bin,
		Zip,
		Img,
		Excel97,
		Excel2007,
		All
	}

	public static class FileSelector
	{

		public static string TitleSingleFile = "Please choose a file";
		public static string TitleMultiFile = "Please choose files";
		public static string Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
		/// <summary>
		public static FileType FileExtension {
			set {
				switch (value) {
					case FileType.Txt:
						Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
						break;
					case FileType.Rtf:
						Filter = "Rtf files (*.rtf)|*.rtf|All files (*.*)|*.*";
						break;
					case FileType.Html:
						Filter = "Html files (*.htm;*.html)|*.htm;*.html|All files (*.*)|*.*";
						break;
					case FileType.Xml:
						Filter = "XML files (*.xml)|*.xml|Config files (*.config)|*.config|All files (*.*)|*.*";
						break;
					case FileType.PDF:
						Filter = "PDF files (*.pdf)|*.pdf|PDF form files (*.fdf)|*.fdf|All files (*.*)|*.*";
						break;
					case FileType.Bin:
						Filter = "Application files(*.exe;*.dll)|*.exe;*.dll|Binary files (*.bin)|*.bin|All files (*.*)|*.*";
						break;
					case FileType.Zip:
						Filter = "Zip files (*.zip)|*.zip|All files (*.*)|*.*";
						break;
					case FileType.Img:
						Filter = "Gif(*.gif)|*.gif|Jpeg(*.jpg)|*.jpg|Emf(*.emf)|*.emf|Bmp(*.bmp)|*.bmp|Png(*.png)|*.png";
						break;
					case FileType.Excel97:
						Filter = "Excel files (*.xls)|*.xls|All files (*.*)|*.*";
						break;
					case FileType.Excel2007:
						Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
						break;
					case FileType.All:
						Filter = "All files (*.*)|*.*";
						break;
				}
			}
		}

		public static System.Drawing.Imaging.ImageFormat ImageFormat {
			get {
				switch (SFD.FilterIndex) {
					case 1:
						return System.Drawing.Imaging.ImageFormat.Gif;

					case 2:
						return System.Drawing.Imaging.ImageFormat.Jpeg;

					case 3:
						return System.Drawing.Imaging.ImageFormat.Emf;

					case 4:
						return System.Drawing.Imaging.ImageFormat.Bmp;

					case 5:
						return System.Drawing.Imaging.ImageFormat.Png;

					default:
						return System.Drawing.Imaging.ImageFormat.Png;
				}
			}
		}

		public static OpenFileDialog OFD = new OpenFileDialog();
		public static SaveFileDialog SFD = new SaveFileDialog();

		public static string InitialPath {
			get { return OFD.InitialDirectory; }
			set {
				OFD.InitialDirectory = value;
				SFD.InitialDirectory = value;
			}
		}

		public static string FileName {
			get { return OFD.FileName; }
			set {
				OFD.FileName = value;
				SFD.FileName = value;
			}
		}

		static FileSelector()
		{
			OFD.RestoreDirectory = false;
		}
		public static string BrowseFile()
		{
			OFD.Title = TitleSingleFile;
			OFD.Filter = Filter;
			OFD.Multiselect = false;
			if (OFD.ShowDialog() == DialogResult.OK) {
				return OFD.FileName;
			} else {
				return null;
			}
		}

		public static string[] BrowseFiles()
		{
			OFD.Title = TitleMultiFile;
			OFD.Filter = Filter;
			OFD.Multiselect = true;
			if (OFD.ShowDialog() == DialogResult.OK) {
				return OFD.FileNames;
			} else {
				return null;
			}
		}

		public static string BrowseFileForSave()
		{
			SFD.Title = TitleSingleFile;
			SFD.Filter = Filter;
			if (SFD.ShowDialog() == DialogResult.OK) {
				return SFD.FileName;
			} else {
				return null;
			}
		}

		public static string BrowseFile(FileType type)
		{
			FileExtension = type;
			return BrowseFile();
		}

		public static string BrowseFile(string filter)
		{
			Filter = filter;
			return BrowseFile();
		}


		public static string[] BrowseFiles(FileType type)
		{
			FileExtension = type;
			return BrowseFiles();
		}


		public static string[] BrowseFiles(string filter)
		{
			Filter = filter;
			return BrowseFiles();
		}


		public static string BrowseFileForSave(FileType type)
		{
			FileExtension = type;
			return BrowseFileForSave();
		}


		public static string BrowseFileForSave(string filter)
		{
			Filter = filter;
			return BrowseFileForSave();
		}
	}
}
