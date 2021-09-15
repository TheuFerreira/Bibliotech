; Script generated by the Inno Script Studio Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "Bibliotech"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "Ferreira"
#define MyAppURL "https://github.com/TheuFerreira/Bibliotech"
#define MyAppExeName "Bibliotech.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{FE4520AC-89D2-4B0A-BA77-1BED7C6404AC}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\{#MyAppPublisher}\{#MyAppName}
DefaultGroupName={#MyAppName}
OutputDir=Installer
OutputBaseFilename=Bibliotech_Installer_{#MyAppVersion}
SetupIconFile=Bibliotech\Resources\ico_bibliotech.ico
Compression=lzma
SolidCompression=yes
UninstallDisplayIcon={app}\{#MyAppExeName}
UninstallDisplayName={#MyAppName}
WizardStyle=modern

[Languages]
Name: "portuguese"; MessagesFile: "compiler:Languages\Portuguese.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "Bibliotech\bin\Debug\BouncyCastle.Crypto.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "Bibliotech\bin\Debug\BouncyCastle.Crypto.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "Bibliotech\bin\Debug\Common.Logging.Core.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "Bibliotech\bin\Debug\Common.Logging.Core.pdb"; DestDir: "{app}"; Flags: ignoreversion
Source: "Bibliotech\bin\Debug\Common.Logging.Core.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "Bibliotech\bin\Debug\Common.Logging.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "Bibliotech\bin\Debug\Common.Logging.pdb"; DestDir: "{app}"; Flags: ignoreversion
Source: "Bibliotech\bin\Debug\Common.Logging.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "Bibliotech\bin\Debug\Enums.NET.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "Bibliotech\bin\Debug\Enums.NET.pdb"; DestDir: "{app}"; Flags: ignoreversion
Source: "Bibliotech\bin\Debug\Enums.NET.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "Bibliotech\bin\Debug\ICSharpCode.SharpZipLib.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "Bibliotech\bin\Debug\ICSharpCode.SharpZipLib.pdb"; DestDir: "{app}"; Flags: ignoreversion
Source: "Bibliotech\bin\Debug\ICSharpCode.SharpZipLib.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "Bibliotech\bin\Debug\itext.barcodes.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "Bibliotech\bin\Debug\itext.barcodes.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "Bibliotech\bin\Debug\itext.forms.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "Bibliotech\bin\Debug\itext.forms.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "Bibliotech\bin\Debug\itext.io.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "Bibliotech\bin\Debug\itext.io.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "Bibliotech\bin\Debug\itext.kernel.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "Bibliotech\bin\Debug\itext.kernel.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "Bibliotech\bin\Debug\itext.layout.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "Bibliotech\bin\Debug\itext.layout.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "Bibliotech\bin\Debug\itext.pdfa.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "Bibliotech\bin\Debug\itext.pdfa.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "Bibliotech\bin\Debug\itext.sign.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "Bibliotech\bin\Debug\itext.sign.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "Bibliotech\bin\Debug\itext.styledxmlparser.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "Bibliotech\bin\Debug\itext.styledxmlparser.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "Bibliotech\bin\Debug\itext.svg.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "Bibliotech\bin\Debug\itext.svg.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "Bibliotech\bin\Debug\iTextSharp.LGPLv2.Core.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "Bibliotech\bin\Debug\iTextSharp.LGPLv2.Core.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "Bibliotech\bin\Debug\MySqlConnector.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "Bibliotech\bin\Debug\MySqlConnector.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "Bibliotech\bin\Debug\NPOI.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "Bibliotech\bin\Debug\NPOI.OOXML.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "Bibliotech\bin\Debug\NPOI.OOXML.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "Bibliotech\bin\Debug\NPOI.OpenXml4Net.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "Bibliotech\bin\Debug\NPOI.OpenXml4Net.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "Bibliotech\bin\Debug\NPOI.OpenXmlFormats.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "Bibliotech\bin\Debug\NPOI.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "Bibliotech\bin\Debug\System.Buffers.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "Bibliotech\bin\Debug\System.Buffers.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "Bibliotech\bin\Debug\System.Memory.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "Bibliotech\bin\Debug\System.Memory.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "Bibliotech\bin\Debug\System.Runtime.CompilerServices.Unsafe.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "Bibliotech\bin\Debug\System.Runtime.CompilerServices.Unsafe.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "Bibliotech\bin\Debug\System.Threading.Tasks.Extensions.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "Bibliotech\bin\Debug\System.Threading.Tasks.Extensions.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "Bibliotech\bin\Debug\Bibliotech.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "Bibliotech\bin\Debug\Bibliotech.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "Bibliotech\bin\Debug\Bibliotech.pdb"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{autoprograms}\{#MyAppPublisher}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{userdesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent
