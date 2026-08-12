[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{5A8E5C41-9FBD-47E4-B27D-89C405B60875}
AppName=PrimeCheque
AppVersion=1.0.0
;AppVerName=PrimeCheque 1.0.0
AppPublisher=PrimeOne
AppPublisherURL=https://www.example.com/
AppSupportURL=https://www.example.com/
AppUpdatesURL=https://www.example.com/
DefaultDirName={autopf}\PrimeCheque
DisableProgramGroupPage=yes
; Uncomment the following line to run in non administrative install mode (install for current user only.)
;PrivilegesRequired=lowest
OutputDir=Output
OutputBaseFilename=PrimeChequeSetup
Compression=lzma
SolidCompression=yes
WizardStyle=modern

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "PrimeCheque\bin\Release\net8.0-windows10.0.19041.0\win-x64\publish-unpackaged\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{autoprograms}\PrimeCheque"; Filename: "{app}\PrimeCheque.exe"
Name: "{autodesktop}\PrimeCheque"; Filename: "{app}\PrimeCheque.exe"; Tasks: desktopicon

[Run]
Filename: "{app}\PrimeCheque.exe"; Description: "{cm:LaunchProgram,PrimeCheque}"; Flags: nowait postinstall skipifsilent
