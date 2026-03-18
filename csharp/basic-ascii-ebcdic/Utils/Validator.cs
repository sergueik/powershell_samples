using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

/**
 * Copyright 2026 Serguei Kouzmine
 */

namespace Utils {
    public class Validator {
        private static readonly Dictionary<string, string> CodepageAliases =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                { "cp037", "IBM1047" },
                { "ibm037", "IBM1047" },
                { "ascii", "ASCII" },
                { "us-ascii", "ASCII" },
                { "utf8", "UTF-8" },
                { "utf-8", "UTF-8" }
            };

        // Instance state
        private readonly byte[] data;
        private readonly string codePage;
        private readonly double? threshold;

        public Validator(byte[] data, string codePage, double? threshold) {
            if (data == null)
                throw new ArgumentNullException("data");

            this.data = data;
            this.codePage = Normalize(codePage);
            this.threshold = threshold;
        }

        public Validator(byte[] data) : this(data, "ASCII", null) { }

        public static string byteArrayToString(byte[] bytes) {
            return Encoding.Default.GetString(bytes);
        }

        private static string Normalize(string codePage) {
            if (string.IsNullOrEmpty(codePage))
                return "ASCII";

            string value;
            if (CodepageAliases.TryGetValue(codePage, out value))
                return value;

            return codePage.ToUpper();
        }

        private  Func<byte[], string> GetDecoder(string codePage) {
            if (codePage.Equals("UTF-8", StringComparison.OrdinalIgnoreCase))
                return delegate(byte[] data) { return new UTF8Encoding(false, true).GetString(data); };

            if (codePage.Equals("IBM1047", StringComparison.OrdinalIgnoreCase))
                return delegate(byte[] data) { return Encoding.GetEncoding(1047, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback).GetString(data); };

            if (codePage.Equals("ASCII", StringComparison.OrdinalIgnoreCase))
                return delegate(byte[] data) { return Encoding.ASCII.GetString(data); };

            return null;
        }

        private Predicate<int> GetPredicate(string codePage) {
            switch (codePage.ToUpper())
            {
                case "IBM037":
                case "IBM1047":
                    return delegate(int charCode)
                    {
                        return charCode == 0x40 ||
                               (charCode >= 0xF0 && charCode <= 0xF9) ||
                               (charCode >= 0xC1 && charCode <= 0xC9) || (charCode >= 0xD1 && charCode <= 0xD9) ||
                               (charCode >= 0xE2 && charCode <= 0xE9) ||
                               (charCode >= 0x81 && charCode <= 0x89) || (charCode >= 0x91 && charCode <= 0x99) ||
                               (charCode >= 0xA2 && charCode <= 0xA9) ||
                               (charCode >= 0x4A && charCode <= 0x6F) ||
                               charCode == 0x3F || charCode == 0x45 || charCode == 0x49
					|| charCode == 0x7D || charCode == 0xCE || charCode == 0xDE || charCode == 0xD3 || charCode == 0xC7 || charCode == 0xE9 || charCode == 0xDC;
                    };
                case "ASCII":
                    return delegate(int charCode) { return charCode <= 0x7F; };
                case "UTF-8":
                    return null;
                default:
                    return null;
            }
        }

        public ValidationResult Validate() {
            bool status = true;
            Func<byte[], string> decoder = this.GetDecoder(codePage);
            Predicate<int> rangeValidator =  this.GetPredicate(codePage);
            string message = null;
            int validCount = 0;

            if (decoder != null) {
                try { decoder(data); }
                catch (Exception e) {
                    return new ValidationResult(false, string.Format("failed to decode in code page {0}: {1}", codePage, e.Message));
                }
            }

            if (rangeValidator == null)
                return new ValidationResult(true, null);

            for (int pos = 0; pos < data.Length; pos++) {
                int charCode = data[pos] & 0xFF;

               if (charCode == 0)  {
                    return new ValidationResult(false , string.Format("null character in code page {0} at position {1}", codePage, pos));
               }

                bool valid = rangeValidator(charCode);
                if (valid) validCount++;

                if (!valid && threshold == null) {
                    return new ValidationResult(false , string.Format("invalid code page {0} character 0x{1:X2} at position {2}", codePage, charCode, pos));
                }
            }

            if (threshold != null) {
                double ratio = (double)validCount / data.Length;
                if (ratio < threshold.Value)
                {
                    status = false;
                    message = string.Format("valid byte ratio {0:F2} below threshold {1:F2} for code page {2}", ratio, threshold.Value, codePage);
                }
            }

            return new ValidationResult(status, message);
        }
    }

 }
