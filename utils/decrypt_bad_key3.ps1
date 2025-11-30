
Add-Type @"
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

public class StreamUtil
{
    public static IStream Wrap(Stream s)
    {
        return new ManagedIStream(s);
    }

    private class ManagedIStream : IStream
    {
        private Stream _stream;

        public ManagedIStream(Stream stream)
        {
            _stream = stream;
        }

        public void Read(byte[] pv, int cb, IntPtr pcbRead)
        {
            int read = _stream.Read(pv, 0, cb);
            if (pcbRead != IntPtr.Zero)
                Marshal.WriteInt32(pcbRead, read);
        }

        public void Write(byte[] pv, int cb, IntPtr pcbWritten)
        {
            _stream.Write(pv, 0, cb);
            if (pcbWritten != IntPtr.Zero)
                Marshal.WriteInt32(pcbWritten, cb);
        }

        public void Seek(long dlibMove, int dwOrigin, IntPtr plibNewPosition)
        {
            long pos = _stream.Seek(dlibMove, (SeekOrigin)dwOrigin);
            if (plibNewPosition != IntPtr.Zero)
                Marshal.WriteInt64(plibNewPosition, pos);
        }

        public void SetSize(long libNewSize)
        {
            _stream.SetLength(libNewSize);
        }

        public void CopyTo(IStream pstm, long cb, IntPtr pcbRead, IntPtr pcbWritten)
        {
            // optional implement or leave empty
        }

        public void Commit(int grfCommitFlags) { }

        public void Revert() { }

        public void LockRegion(long libOffset, long cb, int dwLockType) { }

        public void UnlockRegion(long libOffset, long cb, int dwLockType) { }

        public void Stat(out  System.Runtime.InteropServices.ComTypes.STATSTG pstatstg, int grfStatFlag)
        {
            pstatstg = new System.Runtime.InteropServices.ComTypes.STATSTG();
        }

        public void Clone(out IStream ppstm)
        {
            ppstm = null;
        }
    }
}
"@
$Base64 = 'AQAAANCMnd8BFdERjHoAwE/Cl+sBAAAAOoJXZWw0vUKxB/NH5T4fNgQAAAACAAAAAAAQZgAAAAEAACAAAACH3nsMnlrYJG/mQfB4/K8xyarBbd/C2QgjF4X8/k5FugAAAAAOgAAAAAIAACAAAADSMfeC+3pn6ivnUp2uArlh4ld82tOkInqmvDbPLvRi9vAAAACT853kRuy1mMT9gkjDKs04FynJ+LI8mk2Xgh6btVneIjeB66WKy7xLN0ndN91EhvUbD2aqN1AAUJ9zfexSudddCbim3QuLRItN7oiPh7yRKB2VQoUEtZtV/bB4x4K7t5xpEpHAV5FUn+99LLljn5g+u20h5ZRQlUoWkOu2wBOpeo1VCqooug/otsY+KYpZKaC+VkgcAmeqZzPRggpF4yT1vgw0o0yQG7aiHda5NY+QG781yvqcLDG7RdXnjP68+VqzsRrH88Xh3vLsKH7vAYwZY89UoRivxQ8NJXROcVF8nuTPcHulQoKClV2lVRtpXHxAAAAAow+YDHZhMFnh0GOGu0rE0ZY0U7diLEaDuGHWMSq1Ojk2xsC+QF0vBqF/faVEcyn6UIMCkCRAua9g7eUlnC2opQ=='

$raw = [Convert]::FromBase64String($Base64)
$ms  = New-Object System.IO.MemoryStream (,$raw)

$comStream = [StreamUtil]::Wrap($ms)
# now need to browse the $comSteam to find out what is inside
 
