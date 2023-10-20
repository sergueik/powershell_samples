using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using HttpMultipartParser;
using NUnit.Framework;

namespace HttpMultipartParserUnitTest {
	/// <summary>
	///     The http multipart form parser unit test.
	/// </summary>
	/// <remarks>
	/// It's important to implement IDisposable. It allows us to write test initialization
	/// code in the construtor and test cleanup code in the 'Dispose' method. This is the
	/// equivalent of decorating a method with the [TestInitialize] attribute and decorating
	/// another method with the [TestCleanup] when using mstest. 
	/// </remarks>
	[TestFixture]
	public class BasicTest : IDisposable
	{
		#region Static Fields

		private static readonly string SingleFileTestData = TestUtil.TrimAllLines(@"--boundry
              Content-Disposition: form-data; name=""file""; filename=""data.txt"";
              Content-Type: text/plain

              I am the first data1
              --boundry--");


		/// <summary>
		///     Test case for single files.
		/// </summary>
		private static readonly TestData SingleFileTestCase = new TestData(
			                                                            SingleFileTestData,
			                                                            new List<ParameterPart> { },
			                                                            new List<FilePart> {
				new FilePart("file", "data.txt", TestUtil.StringToStreamNoBom("I am the first data1"), "text/plain", "form-data")
			}
		                                                            );

		/// <summary>
		///     Raw multipart/form-data for the
		///     <see cref="TestData">
		///         <c>ObjNamehEre</c>
		///     </see>
		///     test object.
		/// </summary>
		private static readonly string MultipleParamsAndFilesTestData = TestUtil.TrimAllLines(@"--boundry
              Content-Disposition: form-data; name=""text""
              
              textdata
              --boundry
              Content-Disposition: form-data; name=""after"";TestForTextWithoutSplit
              
              afterdata
              --boundry
              Content-Disposition: form-data; name=""file""; filename=""data.txt""
              Content-Type: text/plain

              I am the first data 
              --boundry
              Content-Disposition: form-data;TestForTextWithoutSplit; name=""newfile""; filename=""superdata.txt""
              Content-Type: text/plain

              I am the second data
              --boundry
              Content-Disposition: form-data; name=""never""

              neverdata 
              --boundry
              Content-Disposition: form-data; name=""waylater""

              waylaterdata 
              --boundry--");

		/// <summary>
		///     Test case for multiple parameters and files.
		/// </summary>
		private static readonly TestData MultipleParamsAndFilesTestCase = new TestData(
			                                                                        MultipleParamsAndFilesTestData,
			                                                                        new List<ParameterPart> {
				new ParameterPart("text", "textdata"),
				new ParameterPart("after", "afterdata"),
				new ParameterPart("never", "neverdata"),
				new ParameterPart("waylater", "waylaterdata")
			},
			                                                                        new List<FilePart> {
				new FilePart("file", "data.txt", TestUtil.StringToStreamNoBom("I am the first data")),
				new FilePart("newfile", "superdata.txt", TestUtil.StringToStreamNoBom("I am the second data"))
			}
		                                                                        );

		/// <summary>
		///     The small request.
		/// </summary>
		private static readonly string SmallTestData =
			TestUtil.TrimAllLines(@"-----------------------------265001916915724
                Content-Disposition: form-data; name=""textdata""
                
                Testdata
                -----------------------------265001916915724
                Content-Disposition: form-data; name=""file""; filename=""data.txt""
                Content-Type: application/octet-stream

                This is a small file
                -----------------------------265001916915724
                Content-Disposition: form-data; name=""submit""

                Submit
                -----------------------------265001916915724--");

		/// <summary>
		///     The small data test case with expected outcomes
		/// </summary>
		private static readonly TestData SmallTestCase = new TestData(
			                                                       SmallTestData,
			                                                       new List<ParameterPart> {
				new ParameterPart("textdata", "Testdata"),
				new ParameterPart("submit", "Submit"),
			},
			                                                       new List<FilePart> {
				new FilePart("file", "data.txt", TestUtil.StringToStreamNoBom("This is a small file"), "application/octet-stream", "form-data")
			}
		                                                       );

		/// <summary>
		///     Raw multipart/form-data for the <see cref="TestData" /> object.
		/// </summary>
		private static readonly string TinyTestData = TestUtil.TrimAllLines(@"--boundry
            Content-Disposition: form-data; name=""text""

            textdata
            --boundry
            Content-Disposition: form-data; name=""file""; filename=""data.txt""
            Content-Type: text/plain

            tiny
            --boundry
            Content-Disposition: form-data; name=""after""

            afterdata
            --boundry--");

		/// <summary>
		///     The tiny data test case with expected outcomes
		/// </summary>
		private static readonly TestData TinyTestCase = new TestData(
			                                                      TinyTestData,
			                                                      new List<ParameterPart> {
				new ParameterPart("text", "textdata"),
				new ParameterPart("after", "afterdata")
			},
			                                                      new List<FilePart> {
				new FilePart("file", "data.txt", TestUtil.StringToStreamNoBom("tiny"))
			}
		                                                      );

		private static readonly string ExactBufferTruncateTestData = TestUtil.TrimAllLines(@"--boundry
            Content-Disposition: form-data; name=""text""

            textdata
            --boundry
            Content-Disposition: form-data; name=""file""; filename=""data.txt""
            Content-Type: text/plain

            1234567890123456789012
            --boundry--");

		/// <summary>
		///     This test has the buffer split such that the final '--' of the end boundary
		///     falls into the next buffer.
		/// </summary>
		private static readonly TestData ExactBufferTruncateTestCase = new TestData(
			                                                                     ExactBufferTruncateTestData,
			                                                                     new List<ParameterPart> {
				new ParameterPart("text", "textdata")
			},
			                                                                     new List<FilePart> {
				new FilePart("file", "data.txt", TestUtil.StringToStreamNoBom("1234567890123456789012"))
			}
		                                                                     );


		/// <summary>
		///     Raw test data for testing a multipart with the file as the last data section.
		/// </summary>
		private static readonly string FileIsLastTestData = TestUtil.TrimAllLines(
			                                                          @"-----------------------------41952539122868
            Content-Disposition: form-data; name=""adID""

            1425
            -----------------------------41952539122868
            Content-Disposition: form-data; name=""files[]""; filename=""Capture.JPG""
            Content-Type: image/jpeg

             BinaryData
             -----------------------------41952539122868--");

		/// <summary>
		///     Test for cases where the file is last with expected outcomes.
		/// </summary>
		private static readonly TestData FileIsLastTestCase = new TestData(
			                                                            FileIsLastTestData,
			                                                            new List<ParameterPart> {
				new ParameterPart("adID", "1425")
			},
			                                                            new List<FilePart> {
				new FilePart("files[]", "Capture.JPG", TestUtil.StringToStreamNoBom("BinaryData"), "image/jpeg", "form-data")
			}
		                                                            );

		private static readonly string MixedUnicodeWidthAndAsciiWidthCharactersTestData = TestUtil.TrimAllLines(
			                                                                                        @"--boundary_.oOo._MjQ1NTU=OTk3Ng==MjcxODE=
              Content-Disposition: form-data; name=""psAdTitle""

              Bonjour poignée 
              --boundary_.oOo._MjQ1NTU=OTk3Ng==MjcxODE=--"
		                                                                                        );

		private static readonly TestData MixedUnicodeWidthAndAsciiWidthCharactersTestCase = new TestData(
			                                                                                          MixedUnicodeWidthAndAsciiWidthCharactersTestData,
			                                                                                          new List<ParameterPart> {
				new ParameterPart("psAdTitle", "Bonjour poignée")
			},
			                                                                                          new List<FilePart>()
		                                                                                          );

		private static readonly string MixedSingleByteAndMultiByteWidthTestData = TestUtil.TrimAllLines(
			                                                                                @"-----------------------------41952539122868
            Content-Disposition: form-data; name=""تت""

            1425
            -----------------------------41952539122868
            Content-Disposition: form-data; name=""files[]""; filename=""تست.jpg""
            Content-Type: image/jpeg

             BinaryData
             -----------------------------41952539122868--"
		                                                                                );

		private static readonly TestData MixedSingleByteAndMultiByteWidthTestCase = new TestData(
			                                                                                  MixedSingleByteAndMultiByteWidthTestData,
			                                                                                  new List<ParameterPart> { 
				new ParameterPart("تت", "1425")
			},
			                                                                                  new List<FilePart> {
				new FilePart("files[]", "تست.jpg", TestUtil.StringToStreamNoBom("BinaryData"), "image/jpeg", "form-data")
			}
		                                                                                  );

		private static readonly string FullPathAsFileNameTestData = TestUtil.TrimAllLines(
			                                                                  @"-----------------------------7de6cc440a46
            Content-Disposition: form-data; name=""file""; filename=""C:\test\test;abc.txt""
            Content-Type: text/plain

            test
            -----------------------------7de6cc440a46--"
		                                                                  );

		private static readonly TestData FullPathAsFileNameWithSemicolon = new TestData(
			                                                                         FullPathAsFileNameTestData,
			                                                                         new List<ParameterPart>(),
			                                                                         new List<FilePart> {
				new FilePart("file", "test;abc.txt", TestUtil.StringToStream("test"), "text/plain", "form-data")
			}
		                                                                         );

		private static readonly string SeveralValuesWithSamePropertyTestData = TestUtil.TrimAllLines(
			                                                                             @"------B6u9lJxB4ByPiGPZ
            Content-Disposition: form-data; name=""options""

            value0
            ------B6u9lJxB4ByPiGPZ
            Content-Disposition: form-data; name=""options""

            value1
            ------B6u9lJxB4ByPiGPZ
            Content-Disposition: form-data; name=""options""

            value2
            ------B6u9lJxB4ByPiGPZ--"
		                                                                             );

		private static readonly TestData SeveralValuesWithSameProperty = new TestData(
			                                                                       SeveralValuesWithSamePropertyTestData,
			                                                                       new List<ParameterPart> {
				new ParameterPart("options", "value0"),
				new ParameterPart("options", "value1"),
				new ParameterPart("options", "value2")
			},
			                                                                       new List<FilePart>()
		                                                                       );

		private static readonly string UnclosedBoundaryTestData = TestUtil.TrimAllLines(
			                                                                @"------51523
              Content-Disposition: form-data; name=""value""
              
              my value
              ------51523"
		                                                                );

		private static readonly TestData UnclosedBoundary = new TestData(
			                                                          UnclosedBoundaryTestData,
			                                                          new List<ParameterPart> {
				new ParameterPart("value", "my value")
			},
			                                                          new List<FilePart>()
		                                                          );

		#endregion

		#region Constructor


		// Initializes the test data before each run, which primarily consists of resetting data stream positions.
		public BasicTest() {
			var testData = new[] {
				TinyTestCase,
				SmallTestCase,
				MultipleParamsAndFilesTestCase,
				SingleFileTestCase
			};
			foreach (TestData data in testData) {
				foreach (var filePart in data.ExpectedFileData) {
					filePart.Data.Position = 0;
				}
			}
		}

		#endregion

		#region Public Methods and Operators

		[Test]
		public void ConstructingWithNullStreamFails()
		{
			Assert.Throws<ArgumentNullException>(() => new MultipartFormDataParser(Stream.Null));
		}

		/// <summary>
		///     Tests for correct detection of the boundary in the input stream.
		/// </summary>
		[Test]
		public void CanAutoDetectBoundary()
		{
			using (Stream stream = TestUtil.StringToStream(TinyTestCase.Request, Encoding.UTF8)) {
				var parser = new MultipartFormDataParser(stream);
				Assert.True(TinyTestCase.Validate(parser));
			}
		}

		/// <summary>
		///     Tests that the final '--' ending up in a seperate chunk doesn't break everything.
		/// </summary>
		[Test]
		public void CanHandleFinalDashesInSeperateBufferFromEndBinary()
		{
			using (Stream stream = TestUtil.StringToStream(ExactBufferTruncateTestCase.Request, Encoding.UTF8)) {
				var parser = new MultipartFormDataParser(stream, "boundry", Encoding.UTF8, 16);
				Assert.True(ExactBufferTruncateTestCase.Validate(parser));
			}
		}

		/// <summary>
		///     Ensures that boundary detection works even when the boundary spans
		///     two different buffers.
		/// </summary>
		[Test]
		public void CanDetectBoundariesCrossBuffer()
		{
			using (Stream stream = TestUtil.StringToStream(TinyTestCase.Request, Encoding.UTF8)) {
				var parser = new MultipartFormDataParser(stream, "boundry", Encoding.UTF8, 16);
				Assert.True(TinyTestCase.Validate(parser));
			}
		}

		/// <summary>
		///     The correctly handle mixed newline formats.
		/// </summary>
		[Test]
		public void CorrectlyHandleMixedNewlineFormats()
		{
			// Replace the first '\n' with '\r\n'
			var regex = new Regex(Regex.Escape("\n"));
			string request = regex.Replace(TinyTestCase.Request, "\r\n", 1);
			using (Stream stream = TestUtil.StringToStream(request, Encoding.UTF8)) {
				var parser = new MultipartFormDataParser(stream, "boundry", Encoding.UTF8);
				Assert.True(TinyTestCase.Validate(parser));
			}
		}

		/// <summary>
		///     Tests for correct handling of <c>crlf (\r\n)</c> in the input stream.
		/// </summary>
		[Test]
		public void CorrectlyHandlesCRLF()
		{
			string request = TinyTestCase.Request.Replace("\n", "\r\n");
			using (Stream stream = TestUtil.StringToStream(request, Encoding.UTF8)) {
				var parser = new MultipartFormDataParser(stream, "boundry", Encoding.UTF8);
				Assert.True(TinyTestCase.Validate(parser));
			}
		}

		[Test]
		public void CorrectlyHandlesMultilineParameter()
		{
			string request = TestUtil.TrimAllLines(
				                          @"-----------------------------41952539122868
                Content-Disposition: form-data; name=""multilined""

                line 1
                line 2
                line 3
                -----------------------------41952539122868--");

			using (Stream stream = TestUtil.StringToStream(request, Encoding.UTF8)) {
				var parser = new MultipartFormDataParser(stream, Encoding.UTF8);
				 Assert.AreEqual(parser.GetParameterValue("multilined"), "line 1\r\nline 2\r\nline 3");
				Assert.AreEqual(parser.GetParameterValues("multilined").First(), "line 1\r\nline 2\r\nline 3");
			}
		}

		[Test]
		public void MultipleFilesAndParamsTest() {
			using (Stream stream = TestUtil.StringToStream(MultipleParamsAndFilesTestCase.Request, Encoding.UTF8)) {
				var parser = new MultipartFormDataParser(stream, "boundry", Encoding.UTF8, 16);
				Assert.True(MultipleParamsAndFilesTestCase.Validate(parser));
			}
		}

		[Test]
		public void SingleFileTest() {
			using (Stream stream = TestUtil.StringToStream(SingleFileTestCase.Request, Encoding.UTF8)) {
				var parser = new MultipartFormDataParser(stream, "boundry", Encoding.UTF8, 16);
				Assert.True(SingleFileTestCase.Validate(parser));
			}
		}
		[Test]
		public void SmallDataTest() {
			using (Stream stream = TestUtil.StringToStream(SmallTestCase.Request)) {
				// The boundry is missing the first two -- in accordance with the multipart
				// spec. (A -- is added by the parser, this boundry is what would be sent in the
				// requset header)
				var parser = new MultipartFormDataParser(stream, "---------------------------265001916915724");
				Assert.True(SmallTestCase.Validate(parser));
			}
		}

		[Test]
		public void TinyDataTest() {
			using (Stream stream = TestUtil.StringToStream(TinyTestCase.Request, Encoding.UTF8)) {
				var parser = new MultipartFormDataParser(stream, "boundry", Encoding.UTF8);
				Assert.True(TinyTestCase.Validate(parser));
			}
		}

		[Test]
		public void CanHandleFileAsLastSection() {
			using (Stream stream = TestUtil.StringToStream(FileIsLastTestCase.Request, Encoding.UTF8)) {
				var parser = new MultipartFormDataParser(stream, Encoding.UTF8);
				Assert.True(FileIsLastTestCase.Validate(parser));
			}
		}

		[Test]
		public void CanHandleUnicodeWidthAndAsciiWidthCharacters() {
			using (
				Stream stream = TestUtil.StringToStream(MixedUnicodeWidthAndAsciiWidthCharactersTestCase.Request,
					                            Encoding.UTF8)) {
				var parser = new MultipartFormDataParser(stream, Encoding.UTF8);
				Assert.True(MixedUnicodeWidthAndAsciiWidthCharactersTestCase.Validate(parser));
			}
		}

		[Test]
		public void CanHandleMixedSingleByteAndMultiByteWidthCharacters() {
			using (
				Stream stream = TestUtil.StringToStream(MixedSingleByteAndMultiByteWidthTestCase.Request, Encoding.UTF8)) {
				var parser = new MultipartFormDataParser(stream, Encoding.UTF8);
				Assert.True(MixedSingleByteAndMultiByteWidthTestCase.Validate(parser));
			}
		}

		[Test]
		public void HandlesFullPathAsFileNameWithSemicolonCorrectly() {
			using (Stream stream = TestUtil.StringToStream(FullPathAsFileNameWithSemicolon.Request, Encoding.UTF8)) {
				var parser = new MultipartFormDataParser(stream, Encoding.UTF8);
				Assert.True(FullPathAsFileNameWithSemicolon.Validate(parser));
			}
		}

		[Test]
		public void AcceptSeveralValuesWithSameProperty() {
			using (Stream stream = TestUtil.StringToStream(SeveralValuesWithSameProperty.Request, Encoding.UTF8)) {
				var parser = new MultipartFormDataParser(stream, Encoding.UTF8);
				Assert.True(SeveralValuesWithSameProperty.Validate(parser));
			}
		}

		[Test]
		public void DoesntInfiniteLoopOnUnclosedInput() {
			using (Stream stream = TestUtil.StringToStream(UnclosedBoundary.Request, Encoding.UTF8)) {
				// We expect this to throw!
				Assert.Throws<MultipartParseException>(() => new MultipartFormDataParser(stream, Encoding.UTF8));
			}
		}

		[Test]
		public void DoesNotCloseTheStream() {
			using (Stream stream = TestUtil.StringToStream(TinyTestCase.Request, Encoding.UTF8)) {
				var parser = new MultipartFormDataParser(stream, "boundry", Encoding.UTF8);
				Assert.True(TinyTestCase.Validate(parser));

				stream.Position = 0;
				Assert.True(true, "A closed stream would throw ObjectDisposedException");
			}
		}

		[Test]
		public void GetParameterValueReturnsNullIfNoParameterFound()
		{
			using (Stream stream = TestUtil.StringToStream(TinyTestCase.Request, Encoding.UTF8)) {
				var parser = new MultipartFormDataParser(stream, "boundry", Encoding.UTF8);
				Assert.Null(parser.GetParameterValue("does not exist"));
			}
		}

		[Test]
		public void HandlesFileWithLastCrLfAtBufferLength()
		{
			string request =
				@"------WebKitFormBoundaryphElSb1aBJGfLyAP
Content-Disposition: form-data; name=""fileName""

Testfile
------WebKitFormBoundaryphElSb1aBJGfLyAP
Content-Disposition: form-data; name=""file""; filename=""Testfile""
Content-Type: application/pdf

"
				+ new string('\0', 8147)
				+ @"
------WebKitFormBoundaryphElSb1aBJGfLyAP--
";

			using (Stream stream = TestUtil.StringToStream(request, Encoding.UTF8)) {
				var parser = new MultipartFormDataParser(stream, Encoding.UTF8);
			}
		}

		[Test]
		public void HandlesFileWithLastCrLfImmediatlyAfterBufferLength()
		{
			string request =
				@"------WebKitFormBoundaryphElSb1aBJGfLyAP
Content-Disposition: form-data; name=""fileName""

Testfile
------WebKitFormBoundaryphElSb1aBJGfLyAP
Content-Disposition: form-data; name=""file""; filename=""Testfile""
Content-Type: application/pdf

"
				+ new string('\0', 8149)
				+ @"
------WebKitFormBoundaryphElSb1aBJGfLyAP--
";

			using (Stream stream = TestUtil.StringToStream(request, Encoding.UTF8)) {
				var parser = new MultipartFormDataParser(stream, Encoding.UTF8);
			}
		}

		[Test]
		public void CanDetectBoundriesWithNewLineInNextBuffer()
		{
			for (int i = 16; i < TinyTestCase.Request.Length; i++) {
				using (Stream stream = TestUtil.StringToStream(TinyTestCase.Request, Encoding.UTF8)) {
					var parser = new MultipartFormDataParser(stream, "boundry", Encoding.UTF8, i);
					Assert.True(TinyTestCase.Validate(parser), String.Format("Failure in buffer length {0}", i));
				}
			}
		}

		public void Dispose()
		{
		}

		#endregion

		/// <summary>
		///     Represents a single parsing test case and the expected parameter/file outputs
		///     for a given request.
		/// </summary>
		private class TestData
		{
			#region Constructors and Destructors

			public TestData( string request, List<ParameterPart> expectedParams, List<FilePart> expectedFileData) {
				Request = request;
				ExpectedParams = expectedParams;
				ExpectedFileData = expectedFileData;
			}

			#endregion

			public List<FilePart> ExpectedFileData { get; set; }
			public List<ParameterPart> ExpectedParams { get; set; }
			public string Request { get; set; }

			#region Public Methods and Operators

			public bool Validate(MultipartFormDataParser parser)
			{
				// Deal with all the parameters who are only expected to have one value.
				var expectedParametersWithSingleValue = ExpectedParams
                    .GroupBy(p => p.Name)
                    .Where(g => g.Count() == 1)
                    .Select(g => g.Single());

				foreach (var expectedParameter in expectedParametersWithSingleValue) {
					if (!parser.HasParameter(expectedParameter.Name)) {
						return false;
					}

					var actualValue = parser.GetParameterValue(expectedParameter.Name);
					var actualValueFromValues = parser.GetParameterValues(expectedParameter.Name).Single();

					if (actualValue != actualValueFromValues) {
						Console.WriteLine("GetParameterValue vs. GetParameterValues mismatch! ({0} != {1})", actualValue, actualValueFromValues);
						return false;
					}

					Console.WriteLine("Expected {0} = {1}. Found {2} = {3}", expectedParameter.Name, expectedParameter.Data, expectedParameter.Name, actualValue);

					if (expectedParameter.Data != actualValue) {
						return false;
					}
				}

				// Deal with the parameters who are expected to have more then one value
				var expectedParametersWithMultiValues = ExpectedParams
                    .GroupBy(p => p.Name)
                    .Where(a => a.Count() > 1);

				foreach (var expectedParameters in expectedParametersWithMultiValues) {
					var key = expectedParameters.Key;
					if (!parser.HasParameter(key)) {
						return false;
					}

					var actualValues = parser.GetParameterValues(key);

					Console.WriteLine("Expected {0} = {1}. Found {2} = {3}", 
						key, 
						string.Join(",", expectedParameters.Select(p => p.Data)),
						key, 
						string.Join(",", actualValues)
					);

					if (actualValues.Count() != expectedParameters.Count() || actualValues.Zip(expectedParameters, Tuple.Create).Any(t => t.Item1 != t.Item2.Data)) {
						return false;
					}
				}

				// Validate files
				foreach (var filePart in ExpectedFileData) {
					var foundPairMatch = false;
					foreach (var file in parser.Files) {
						if (filePart.Name == file.Name) {
							foundPairMatch = true;

							FilePart expectedFile = filePart;
							FilePart actualFile = file;

							if (expectedFile.Name != actualFile.Name || expectedFile.FileName != actualFile.FileName) {
								return false;
							}

							if (expectedFile.ContentType != actualFile.ContentType ||
							                         expectedFile.ContentDisposition != actualFile.ContentDisposition) {
								return false;
							}

							// Read the data from the files and see if it's the same
							if (expectedFile.Data.CanSeek) {
								expectedFile.Data.Position = 0;
							}

							var reader = new StreamReader(expectedFile.Data);
							string expectedFileData = reader.ReadToEnd();

							reader = new StreamReader(actualFile.Data);
							string actualFileData = reader.ReadToEnd();

							if (expectedFileData != actualFileData) {
								return false;
							}

							break;
						}
					}

					if (!foundPairMatch) {
						return false;
					}
				}

				return true;
			}

			#endregion
		}
	}
}