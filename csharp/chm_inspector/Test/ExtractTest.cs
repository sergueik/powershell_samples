using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using NUnit.Framework;
using Utils;

namespace Tests {
    [TestFixture]
    public class ExtractTest  {
  		private  List<string> files = new List<string>();    
      	private const string fileName ="api.chm";
    	private string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName );

  		[Test]
        public void test1() {
        		files.AddRange(new string[]{"file1","file2","file3"});
                var arg = Chm.buildArgument(files);
    
                Assert.NotNull(arg, "build7zFileSelectionArgument() returned null.");
                StringAssert.Contains(@"""file1""",arg,"expected filenames");
               StringAssert.Contains(" ",arg,"expected space separator between filenames");
        }
  
 		[Test]
        public void test2() {

				files.AddRange(Enumerable.Repeat("filename", 100000)); 
                var arg = Chm.buildArgument(files);
    
                Assert.NotNull(arg, "build7zFileSelectionArgument() returned null.");
                StringAssert.StartsWith("@",arg,"expected list file");
        }

 		[Test]
        public void test3() {

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
nonexistent.htm
 			              ".Split(new[] { Environment.NewLine },
 			                       StringSplitOptions.RemoveEmptyEntries));
                var result = Chm.extract_7zip(file, files);
    
                Assert.NotNull(result, "extract_7zip() returned null.");
                // TODO: better test for nonexistent file
                Assert.Greater(files.Count,result.Count,"expected all file");
                               Console.WriteLine("Found {0} entries", result.Count);
 
        }
    }

}
