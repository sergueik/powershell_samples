$Base64 = 'AQAAANCMnd8BFdERjHoAwE/Cl+sBAAAAOoJXZWw0vUKxB/NH5T4fNgQAAAACAAAAAAAQZgAAAAEAACAAAACH3nsMnlrYJG/mQfB4/K8xyarBbd/C2QgjF4X8/k5FugAAAAAOgAAAAAIAACAAAADSMfeC+3pn6ivnUp2uArlh4ld82tOkInqmvDbPLvRi9vAAAACT853kRuy1mMT9gkjDKs04FynJ+LI8mk2Xgh6btVneIjeB66WKy7xLN0ndN91EhvUbD2aqN1AAUJ9zfexSudddCbim3QuLRItN7oiPh7yRKB2VQoUEtZtV/bB4x4K7t5xpEpHAV5FUn+99LLljn5g+u20h5ZRQlUoWkOu2wBOpeo1VCqooug/otsY+KYpZKaC+VkgcAmeqZzPRggpF4yT1vgw0o0yQG7aiHda5NY+QG781yvqcLDG7RdXnjP68+VqzsRrH88Xh3vLsKH7vAYwZY89UoRivxQ8NJXROcVF8nuTPcHulQoKClV2lVRtpXHxAAAAAow+YDHZhMFnh0GOGu0rE0ZY0U7diLEaDuGHWMSq1Ojk2xsC+QF0vBqF/faVEcyn6UIMCkCRAua9g7eUlnC2opQ=='
$bytes = [Convert]::FromBase64String($Base64)

write-output ('Length: {0} bytes' -f $bytes.Length ) 

# Show first 32 bytes as hex signature
write-output (($bytes[0..31] | ForEach-Object {write-output ($_.ToString('X2')) } ) -join ' ' )

# Guess compression/encryption signatures
switch -regex ($bytes[0..3] -join ',') {
    '31,139'             { 'Looks like GZIP' }
    '120,156'            { 'Looks like zlib/deflate' }
    '66,87'              { 'Looks like Blowfish' }
    '82,68'              { 'Looks like RDP settings file' }
    '3,0,0,0'            { 'Possible DPAPI header' }
    Default              { 'Unknown header – may be custom or XOR-obfuscated' }
}

