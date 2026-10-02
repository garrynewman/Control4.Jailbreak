param(
    [string]$AssemblyPath = (Join-Path $PSScriptRoot '..\bin\Release\C4Jailbreak.exe'),
    [string]$ComposerInstallDir
)

# Run with Windows PowerShell -STA after building. All writes use a temporary directory.
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Windows.Forms
$assembly = [Reflection.Assembly]::LoadFrom((Resolve-Path -LiteralPath $AssemblyPath).Path)
$jailbreakType = $assembly.GetType('Garry.Control4.Jailbreak.UI.Jailbreak', $true)
$logType = $assembly.GetType('Garry.Control4.Jailbreak.UI.LogWindow', $true)
$instanceFlags = [Reflection.BindingFlags]'Instance,NonPublic'
$methodFlags = [Reflection.BindingFlags]'Instance,Static,NonPublic'
$jailbreak = [Runtime.Serialization.FormatterServices]::GetUninitializedObject($jailbreakType)
$log = [Runtime.Serialization.FormatterServices]::GetUninitializedObject($logType)
$textBox = New-Object System.Windows.Forms.RichTextBox
$logType.GetField('textBox', $instanceFlags).SetValue($log, $textBox)
$installDirField = $jailbreakType.GetField('<ComposerInstallDir>k__BackingField', $instanceFlags)
$testRoot = Join-Path ([IO.Path]::GetTempPath()) ('C4Jailbreak-tests-' + [Guid]::NewGuid().ToString('N'))
$originalDirectory = [Environment]::CurrentDirectory

function Assert-True($Condition, [string]$Message) {
    if (-not $Condition) { throw $Message }
}

function Invoke-JailbreakMethod([string]$Name, [object[]]$Arguments) {
    $method = $jailbreakType.GetMethod($Name, $methodFlags)
    for ($index = 0; $index -lt $Arguments.Length; $index++) {
        $Arguments[$index] = $Arguments[$index].PSObject.BaseObject
    }
    return $method.Invoke($jailbreak, $Arguments)
}

try {
    $installDir = Join-Path $testRoot 'Install'
    $configFolder = Join-Path $testRoot 'Control4'
    $composerFolder = Join-Path $configFolder 'Composer'
    [IO.Directory]::CreateDirectory($installDir) | Out-Null
    [IO.Directory]::CreateDirectory($composerFolder) | Out-Null
    [Environment]::CurrentDirectory = $testRoot
    $installDirField.SetValue($jailbreak, $installDir)
    $configPath = Join-Path $installDir 'ComposerPro.exe.config'
    $originalConfig = '<configuration><appSettings><add key="keep" value="yes" /></appSettings></configuration>'
    [IO.File]::WriteAllText($configPath, $originalConfig)

    Assert-True (Invoke-JailbreakMethod 'PatchComposerSettings' @($log, $configFolder)) 'Fresh Composer patch failed.'
    [xml]$config = [IO.File]::ReadAllText($configPath)
    Assert-True ($config.configuration.'system.net'.defaultProxy.proxy.proxyaddress -eq 'http://127.0.0.1:31337/') 'License-check proxy was not installed.'
    Assert-True ($config.configuration.'system.net'.defaultProxy.bypasslist.add.Count -eq 3) 'Update bypass list is missing.'
    Assert-True ($config.configuration.appSettings.add.value -eq 'yes') 'Unrelated configuration was changed.'
    Assert-True ([IO.File]::ReadAllText($configPath + '.backup') -eq $originalConfig) 'Original config was not backed up.'
    $licensePath = Join-Path $composerFolder 'license.xml'
    [xml]$license = [IO.File]::ReadAllText($licensePath)
    Assert-True ($license.License.Code -eq 'ProLicense') 'Composer license file was not created.'
    Assert-True ([IO.File]::Exists((Join-Path $configFolder 'dealeraccount.xml'))) 'Dealer account was not created.'
    $flagsPath = Join-Path $composerFolder 'FeaturesConfiguration.json'
    $flags = [IO.File]::ReadAllText($flagsPath) | ConvertFrom-Json
    Assert-True ($flags.'composer-x4-updatemanger-restrict-override'.Result -eq $true) 'Update Manager override is missing.'
    Assert-True ($flags.'connection-whitelist'.Result -eq $false) 'Connection whitelist is enabled.'
    Write-Output 'PASS: Fresh local settings patch, including a missing system.net section.'

    $patchedConfig = [IO.File]::ReadAllText($configPath)
    [IO.File]::Delete($licensePath)
    [IO.File]::WriteAllText($flagsPath, '{}')
    Assert-True (Invoke-JailbreakMethod 'PatchComposerSettings' @($log, $configFolder)) 'Repeat Composer patch failed.'
    Assert-True ([IO.File]::ReadAllText($configPath) -eq $patchedConfig) 'Repeat patch changed the config.'
    Assert-True ([IO.File]::ReadAllText($configPath + '.backup') -eq $originalConfig) 'Repeat patch replaced the original backup.'
    Assert-True ([IO.File]::Exists($licensePath)) 'Already-patched config skipped license creation.'
    $flags = [IO.File]::ReadAllText($flagsPath) | ConvertFrom-Json
    Assert-True ($flags.'connection-whitelist'.Config -eq '[]') 'Repeat patch did not restore feature flags.'
    Write-Output 'PASS: Repeat runs repair local settings even when the proxy is already patched.'

    $existingLicense = '<License><Name>Existing license</Name></License>'
    [IO.File]::WriteAllText($licensePath, $existingLicense)
    Assert-True (Invoke-JailbreakMethod 'PatchComposerSettings' @($log, $configFolder)) 'Existing-license patch failed.'
    Assert-True ([IO.File]::ReadAllText($licensePath) -eq $existingLicense) 'Existing license was overwritten.'
    Write-Output 'PASS: Existing license files are preserved.'

    foreach ($invalidConfig in @('<configuration>', '<unexpected />')) {
        [IO.File]::WriteAllText($configPath, $invalidConfig)
        Assert-True (-not (Invoke-JailbreakMethod 'PatchComposerSettings' @($log, $configFolder))) 'Invalid config was reported as successfully patched.'
    }
    [IO.File]::Delete($configPath)
    Assert-True (-not (Invoke-JailbreakMethod 'PatchComposerSettings' @($log, $configFolder))) 'Missing config was reported as successfully patched.'
    Write-Output 'PASS: Missing and invalid configuration stop local patching.'

    $certsFolder = Join-Path $testRoot 'Certs'
    [IO.Directory]::CreateDirectory($certsFolder) | Out-Null
    if ($ComposerInstallDir) {
        $installDirField.SetValue($jailbreak, (Resolve-Path -LiteralPath $ComposerInstallDir).Path)
        Copy-Item -LiteralPath (Join-Path $PSScriptRoot '..\Resources\openssl.cfg') -Destination $certsFolder
        Assert-True (Invoke-JailbreakMethod 'EnsureRootCaCerts' @($log)) 'Root CA generation failed.'
        $rootCa = [IO.File]::ReadAllText((Join-Path $certsFolder 'public.pem'))
        Assert-True (Invoke-JailbreakMethod 'EnsureRootCaCerts' @($log)) 'Existing root CA reuse failed.'
        Assert-True ([IO.File]::ReadAllText((Join-Path $certsFolder 'public.pem')) -eq $rootCa) 'Existing root CA was regenerated.'
        Assert-True (Invoke-JailbreakMethod 'GenerateComposerCert' @($log)) 'Composer certificate generation failed.'
        $clientCertPath = Join-Path $certsFolder 'composer.pem'
        $firstClientCert = [IO.File]::ReadAllText($clientCertPath)
        Assert-True (Invoke-JailbreakMethod 'GenerateComposerCert' @($log)) 'Composer certificate refresh failed.'
        Assert-True ([IO.File]::ReadAllText($clientCertPath) -ne $firstClientCert) 'Composer certificate was not refreshed.'
        $certificate = New-Object Security.Cryptography.X509Certificates.X509Certificate2($clientCertPath)
        Assert-True ($null -ne ($certificate.Extensions | Where-Object { $_.Oid.Value -eq '2.5.29.37' })) 'Composer certificate has no extended key usage.'
        $certificate.Dispose()
        Write-Output 'PASS: Existing CA reuse and fresh Composer certificates with actual OpenSSL.'
    }
    else {
        [IO.File]::WriteAllText((Join-Path $certsFolder 'cacert-dev.pem'), 'test CA')
        [IO.File]::WriteAllText((Join-Path $certsFolder 'composer.p12'), 'test Composer certificate')
        Write-Output 'SKIP: OpenSSL checks require -ComposerInstallDir.'
    }

    Invoke-JailbreakMethod 'DeployComposerFiles' @($log, $configFolder)
    foreach ($file in @('cacert-dev.pem', 'composer.p12')) {
        $source = [Convert]::ToBase64String([IO.File]::ReadAllBytes((Join-Path $certsFolder $file)))
        $deployed = [Convert]::ToBase64String([IO.File]::ReadAllBytes((Join-Path $composerFolder $file)))
        Assert-True ($deployed -eq $source) "Composer deployment did not copy $file."
    }
    Write-Output 'PASS: Composer certificate deployment.'
}
catch {
    Write-Output $textBox.Text
    throw
}
finally {
    [Environment]::CurrentDirectory = $originalDirectory
    $textBox.Dispose()
    if (Test-Path -LiteralPath $testRoot) {
        $resolvedRoot = (Resolve-Path -LiteralPath $testRoot).Path
        $expectedParent = [IO.Path]::GetFullPath([IO.Path]::GetTempPath()).TrimEnd('\')
        if ([IO.Path]::GetDirectoryName($resolvedRoot) -ne $expectedParent -or
            [IO.Path]::GetFileName($resolvedRoot) -notlike 'C4Jailbreak-tests-*') {
            throw "Refusing to remove unexpected test directory: $resolvedRoot"
        }
        Remove-Item -LiteralPath $resolvedRoot -Recurse -Force
    }
}
