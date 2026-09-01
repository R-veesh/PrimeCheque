#define MyAppName "PrimeCheque"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "PrimeOne Global"
#define MyAppExeName "PrimeCheque.exe"

[Setup]
AppId={{8D6B4A21-5D6A-4A5E-9B25-123456789ABC}}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}

DefaultDirName={autopf}\PrimeCheque
DefaultGroupName={#MyAppName}

OutputDir=Output
OutputBaseFilename=PrimeCheque_Setup

Compression=lzma
SolidCompression=yes

ArchitecturesInstallIn64BitMode=x64
ArchitecturesAllowed=x64

PrivilegesRequired=admin

WizardStyle=modern

UninstallDisplayIcon={app}\{#MyAppExeName}

[Files]
Source: "D:\PrimeOneWork\C\beta1\code\PrimeCheque1\PrimeCheque\bin\win-x64\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "D:\PrimeOneWork\C\beta1\code\PrimeCheque1\payment-check.ico"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{autoprograms}\PrimeCheque"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\payment-check.ico"
Name: "{autodesktop}\PrimeCheque"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\payment-check.ico"; Tasks: desktopicon

[Tasks]
Name: "desktopicon"; Description: "Create a desktop shortcut"; GroupDescription: "Additional icons:"

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "Launch PrimeCheque"; Flags: nowait postinstall skipifsilent