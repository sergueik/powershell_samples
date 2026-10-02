param(
  $filename = $null
)
# Open the file as a read-only filestream
$stream = [System.IO.File]::Open((resolve-path -path $filename).path, [System.IO.FileMode]::Open, [System.IO.FileAccess]::Read)
$reader = [System.IO.BinaryReader]::New($stream)

# Get the e_lfanew offset to find the PE header start
$stream.Position = 0x3C
$peHeaderOffset = $reader.ReadInt32()

# Read the Machine type (2 bytes) at offset 0x4 from the PE signature
$stream.Position = $peHeaderOffset + 4
$machine = $reader.ReadInt16()

# Close streams to release the file
$reader.Close()
$stream.Dispose()

# Map the machine code to the architecture
switch ($machine) {
    0x014c { "x86 (32-bit)" }
    0x8664 { "x64 (64-bit)" }
    0x0200 { "Itanium (64-bit)" }
    0xaa64 { "ARM64 (64-bit)" }
    default { "Unknown machine type: $machine" }
}