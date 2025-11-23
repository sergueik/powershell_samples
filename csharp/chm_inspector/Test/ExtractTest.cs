using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using NUnit.Framework;
using Utils;

namespace Tests {

	[TestFixture]
	public class ExtractTest {

		private  List<string> files = new List<string>();
		private const string fileName = "api.chm";
		private string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

		[Test]
		public void test1() {
			files.Clear();
			files.AddRange(new string[]{ "file1", "file2", "file3" });
			var arg = Chm.buildArgument(files);

			Assert.NotNull(arg, "build7zFileSelectionArgument() returned null.");
			StringAssert.Contains(@"""file1""", arg, "expected filenames");
			StringAssert.Contains(" ", arg, "expected space separator between filenames");
		}

		[Test]
		public void test2() {
			files.Clear();
			files.AddRange(Enumerable.Repeat("filename", 100000));
			var arg = Chm.buildArgument(files);

			Assert.NotNull(arg, "build7zFileSelectionArgument() returned null.");
			StringAssert.StartsWith("@", arg, "expected list file");
		}

		[Test]
		public void test3() {
			files.Clear();
			files.AddRange(@"
				cmd_hh_alink_lookup.htm
cmd_hh_close_all.htm
cmd_hh_display_index.htm
cmd_hh_display_search.htm
cmd_hh_display_text_popup.htm
cmd_hh_display_toc.htm

 			              ".Split(new[] { Environment.NewLine },
				StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).Where(s => !string.IsNullOrWhiteSpace(s)).ToList());
			try {
				var result = Chm.extract_7zip(file, files);
				// https://documentation.help/7-Zip/exit_codes.htm
				Assert.NotNull(result, "extract_7zip() returned null.");
				// Assert.Equals(files.Count, result.Count);
				Assert.AreEqual(files.Count, result.Count, "expected all file");
				// The Equals method throws an AssertionException.
				// This is done to make sure there is no mistake by calling this function
				Console.WriteLine("Found {0} entries", result.Count);
			} catch (Exception e) {
				Assert.Fail("Exception thrown: " + e.Message);
			}

		}

		[Test]
		public void test4() {
			files.Clear();
			files.AddRange(@"
			cmd_hh_alink_lookup.htm
cmd_hh_close_all.htm
cmd_hh_display_index.htm
cmd_hh_display_search.htm
cmd_hh_display_text_popup.htm
cmd_hh_display_toc.htm
cmd_hh_display_topic.htm
cmd_hh_get_last_error.htm
cmd_hh_get_win_handle.htm
cmd_hh_get_win_type.htm
cmd_hh_help_context.htm
cmd_hh_initialize.htm
cmd_hh_keyword_lookup.htm
cmd_hh_pretranslatemessage.htm
cmd_hh_safe_display_topic.htm
cmd_hh_set_win_type.htm
cmd_hh_sync.htm

 			              ".Split(new[] { Environment.NewLine },
				StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).Where(s => !string.IsNullOrWhiteSpace(s)).ToList());
			try {
				var result = Chm.extract_7zip(file, files);
				// https://documentation.help/7-Zip/exit_codes.htm
				Assert.NotNull(result, "extract_7zip() returned null.");
				// Assert.Equals(files.Count, result.Count);
				Assert.AreEqual(files.Count, result.Count, "expected all file");
				// The Equals method throws an AssertionException.
				// This is done to make sure there is no mistake by calling this function
				Console.WriteLine("Found {0} entries", result.Count);
			} catch (Exception e) {
				Assert.Fail("Exception thrown: " + e.Message);
			}
		}

		[Test]
		public void test5() {
			files.Clear();
			files.AddRange(new string[]{ "nonexistent.htm"} );
			try {
				var result = Chm.extract_7zip(file, files);
				Assert.IsNotNull(result, "extract_7zip() returned non existent file.");
				Assert.AreEqual(new List<String>(), result, "expected no files");
			} catch (Exception e) {
				Assert.Fail("Exception thrown: " + e.Message);
			}
		}

		[Test]
		public void test6() {
			files.Clear();
			files.Add("");
			var e = Assert.Throws<Exception>(() => Chm.extract_7zip(file, files));
			// https://documentation.help/7-Zip/exit_codes.htm
			Assert.That(e.Message, Is.EqualTo("7-Zip failed with exit code 7"));
		}

	}

}
