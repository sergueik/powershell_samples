$Base64 = "AQAAANCMnd8BFdERjHoAwE/Cl+sBAAAAOoJXZWw0vUKxB/NH5T4fNgQAAAACAAAAAAAQZgAAAAEAACAAAACH3nsMnlrYJG/mQfB4/K8xyarBbd/C2QgjF4X8/k5FugAAAAAOgAAAAAIAACAAAADSMfeC+3pn6ivnUp2uArlh4ld82tOkInqmvDbPLvRi9vAAAACT853kRuy1mMT9gkjDKs04FynJ+LI8mk2Xgh6btVneIjeB66WKy7xLN0ndN91EhvUbD2aqN1AAUJ9zfexSudddCbim3QuLRItN7oiPh7yRKB2VQoUEtZtV/bB4x4K7t5xpEpHAV5FUn+99LLljn5g+u20h5ZRQlUoWkOu2wBOpeo1VCqooug/otsY+KYpZKaC+VkgcAmeqZzPRggpF4yT1vgw0o0yQG7aiHda5NY+QG781yvqcLDG7RdXnjP68+VqzsRrH88Xh3vLsKH7vAYwZY89UoRivxQ8NJXROcVF8nuTPcHulQoKClV2lVRtpXHxAAAAAow+YDHZhMFnh0GOGu0rE0ZY0U7diLEaDuGHWMSq1Ojk2xsC+QF0vBqF/faVEcyn6UIMCkCRAua9g7eUlnC2opQ=="

$Data = [Convert]::FromBase64String($Base64)

$signature = @"
using System;
using System.Runtime.InteropServices;
public static class DPAPI {
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct DATA_BLOB {
        public int cbData;
        public IntPtr pbData;
    }

    [DllImport("crypt32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern bool CryptUnprotectData(
        ref DATA_BLOB pDataIn,
        string szDataDescr,
        IntPtr pOptionalEntropy,
        IntPtr pvReserved,
        IntPtr pPromptStruct,
        int dwFlags,
        ref DATA_BLOB pDataOut);

    public static byte[] Unprotect(byte[] data) {
        DATA_BLOB blobIn = new DATA_BLOB();
        DATA_BLOB blobOut = new DATA_BLOB();
        try {
            blobIn.cbData = data.Length;
            blobIn.pbData = Marshal.AllocHGlobal(data.Length);
            Marshal.Copy(data, 0, blobIn.pbData, data.Length);

            if (!CryptUnprotectData(ref blobIn, null, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, 0, ref blobOut))
                throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());

            byte[] decrypted = new byte[blobOut.cbData];
            Marshal.Copy(blobOut.pbData, decrypted, 0, blobOut.cbData);
            return decrypted;
        }
        finally {
            if (blobIn.pbData != IntPtr.Zero) Marshal.FreeHGlobal(blobIn.pbData);
            if (blobOut.pbData != IntPtr.Zero) Marshal.FreeHGlobal(blobOut.pbData);
        }
    }
}
"@

Add-Type $signature

try {
    $out = [DPAPI]::Unprotect($Data)
    [Text.Encoding]::UTF8.GetString($out)
} catch {
    "ERROR: $($_.Exception)"
}

