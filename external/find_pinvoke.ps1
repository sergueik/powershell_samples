function Find-Pinvoke {
  <#
    .SYNOPSIS
        Finds PInvokes in the specified assembly which has been loaded in
        current AppDomain.
    .PARAMETER AssemblyName
        An assembly name, e.g. System.
    .EXAMPLE
        PS C:\> Find-Pinvoke System
        Finds information on all the PInvokes in the System.dll assembly.
  #>
  param(
    [Parameter()]
    [ValidateNotNullOrEmpty()]
    [String]$AssemblyName = 'CommonLanguageRuntimeLibrary'
  )
  
  begin {
    if (!$AssemblyName.EndsWith('.dll') -and $AssemblyName -ne 'CommonLanguageRuntimeLibrary') {
      $AssemblyName += '.dll'
    }
    
    if (($Assembly = [AppDomain]::CurrentDomain.GetAssemblies() |
      Where-Object {
      $_.ManifestModule.ScopeName.Equals($AssemblyName)
    }) -eq $null) {
      Write-Warning "the assembly has not been found in current AppDomain."
      break
    }
  }
  process {
    foreach ($type in $Assembly.GetTypes()) {
      $type.GetMethods([Reflection.BindingFlags]60) | ForEach-Object {
        if (($_.Attributes -band 0x2000) -eq 0x2000) {
          $sig = [Reflection.CustomAttributeData]::GetCustomAttributes(
            $_ #pinvoke data
          ) | Where-Object {$_.ToString() -cmatch 'DllImportAttribute'}
          New-Object PSObject -Property @{
            Module     = if (![IO.Path]::HasExtension(
              ($$ = $sig.ConstructorArguments[0].Value)
            )) { "$$.dll" } else { $$ }
            EntryPoint = ($sig.NamedArguments | Where-Object {
              $_.MemberInfo.Name -eq 'EntryPoint'
            }).TypedValue.Value
            MethodName = $_.Name
            Attributes = $_.Attributes
            TypeName   = $type.FullName
            Signature  = $_.ToString() -replace '(\S+)\s+(.*)', '$2 as $1'
            DllImport  = $sig
          } | Select-Object Module, EntryPoint, MethodName, Attributes, `
          Signature, DllImport
        }
      } #foreach
    } #foreach
  }
  end {}
}

