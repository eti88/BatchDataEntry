; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "Batch Data Entry"
#define MyAppVersion "1.0.1.1"
#define MyAppPublisher "Modservice"
#define MyAppExeName "BatchDataEntry.exe"

[Setup]
AppId={{54919860-4D39-4EF7-B6D0-FB114EE46ABA}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={pf}\BatchDataEntry
DisableProgramGroupPage=yes
OutputDir=C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\Setup
OutputBaseFilename=setup-batchdataentry
SetupIconFile=C:\Users\etien\Desktop\Mesh-100.ico
Compression=lzma
SolidCompression=yes

[Languages]
Name: "italian"; MessagesFile: "compiler:Languages\Italian.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.1

[Files]
Source: "C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\BatchDataEntry\bin\x86\Release\BatchDataEntry.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\BatchDataEntry\bin\x86\Release\BatchDataEntry.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\BatchDataEntry\bin\x86\Release\BatchDataEntry.pdb"; DestDir: "{app}"; Flags: ignoreversion
;Source: "C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\BatchDataEntry\bin\x86\Release\BatchDataEntry.vshost.exe"; DestDir: "{app}"; Flags: ignoreversion
;Source: "C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\BatchDataEntry\bin\x86\Release\BatchDataEntry.vshost.exe.config"; DestDir: "{app}"; Flags: ignoreversion
;Source: "C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\BatchDataEntry\bin\x86\Release\BatchDataEntry.vshost.exe.manifest"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\BatchDataEntry\bin\x86\Release\CsvHelper.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\BatchDataEntry\bin\x86\Release\CsvHelper.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\BatchDataEntry\bin\x86\Release\database.db3"; DestDir: "{app}"; Flags: ignoreversion

Source: "C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\BatchDataEntry\bin\x86\Release\DotNetProjects.DataVisualization.Toolkit.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\BatchDataEntry\bin\x86\Release\DotNetProjects.DataVisualization.Toolkit.pdb"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\BatchDataEntry\bin\x86\Release\DotNetProjects.Input.Toolkit.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\BatchDataEntry\bin\x86\Release\DotNetProjects.Input.Toolkit.pdb"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\BatchDataEntry\bin\x86\Release\DotNetProjects.Layout.Toolkit.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\BatchDataEntry\bin\x86\Release\DotNetProjects.Layout.Toolkit.pdb"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\BatchDataEntry\bin\x86\Release\DotNetProjects.WPF.Themes.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\BatchDataEntry\bin\x86\Release\DotNetProjects.WPF.Themes.pdb"; DestDir: "{app}"; Flags: ignoreversion

Source: "C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\BatchDataEntry\bin\x86\Release\EntityFramework.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\BatchDataEntry\bin\x86\Release\EntityFramework.SqlServer.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\BatchDataEntry\bin\x86\Release\EntityFramework.SqlServer.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\BatchDataEntry\bin\x86\Release\EntityFramework.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\BatchDataEntry\bin\x86\Release\Gu.Wpf.NumericInput.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\BatchDataEntry\bin\x86\Release\Gu.Wpf.NumericInput.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\BatchDataEntry\bin\x86\Release\itextsharp.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\BatchDataEntry\bin\x86\Release\itextsharp.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\BatchDataEntry\bin\x86\Release\logFile.log"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\BatchDataEntry\bin\x86\Release\Microsoft.Expression.Interactions.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\BatchDataEntry\bin\x86\Release\Microsoft.Expression.Interactions.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\BatchDataEntry\bin\x86\Release\Newtonsoft.Json.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\BatchDataEntry\bin\x86\Release\Newtonsoft.Json.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\BatchDataEntry\bin\x86\Release\NLog.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\BatchDataEntry\bin\x86\Release\NLog.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\BatchDataEntry\bin\x86\Release\NLog.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\BatchDataEntry\bin\x86\Release\System.Data.SQLite.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\BatchDataEntry\bin\x86\Release\System.Data.SQLite.EF6.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\BatchDataEntry\bin\x86\Release\System.Data.SQLite.Linq.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\BatchDataEntry\bin\x86\Release\System.Data.SQLite.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\BatchDataEntry\bin\x86\Release\System.Windows.Interactivity.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\BatchDataEntry\bin\x86\Release\System.Windows.Interactivity.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\BatchDataEntry\bin\x86\Release\WpfControls.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\BatchDataEntry\bin\x86\Release\WpfControls.pdb"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\BatchDataEntry\bin\x86\Release\libmupdf.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\BatchDataEntry\bin\x86\Release\MoonPdfLib.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\BatchDataEntry\bin\x86\Release\MouseKeyboardActivityMonitor.dll"; DestDir: "{app}"; Flags: ignoreversion
; Sub Dirs
Source: "C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\BatchDataEntry\bin\x86\Release\x86\SQLite.Interop.dll"; DestDir: "{app}\x86"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\BatchDataEntry\bin\x86\Release\x64\SQLite.Interop.dll"; DestDir: "{app}\x64"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\BatchDataEntry\bin\x86\Release\de\Gu.Wpf.NumericInput.resources.dll"; DestDir: "{app}\de"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\BatchDataEntry\bin\x86\Release\nl\Gu.Wpf.NumericInput.resources.dll"; DestDir: "{app}\nl"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\BatchDataEntry\bin\x86\Release\sv\Gu.Wpf.NumericInput.resources.dll"; DestDir: "{app}\sv"; Flags: ignoreversion recursesubdirs createallsubdirs

; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{commonprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: quicklaunchicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[Code]
function IsDotNetDetected(version: string; service: cardinal): boolean;
// Indicates whether the specified version and service pack of the .NET Framework is installed.
//
// version -- Specify one of these strings for the required .NET Framework version:
//    'v1.1'          .NET Framework 1.1
//    'v2.0'          .NET Framework 2.0
//    'v3.0'          .NET Framework 3.0
//    'v3.5'          .NET Framework 3.5
//    'v4\Client'     .NET Framework 4.0 Client Profile
//    'v4\Full'       .NET Framework 4.0 Full Installation
//    'v4.5'          .NET Framework 4.5
//    'v4.5.1'        .NET Framework 4.5.1
//    'v4.5.2'        .NET Framework 4.5.2
//    'v4.6'          .NET Framework 4.6
//    'v4.6.1'        .NET Framework 4.6.1
//    'v4.6.2'        .NET Framework 4.6.2
//
// service -- Specify any non-negative integer for the required service pack level:
//    0               No service packs required
//    1, 2, etc.      Service pack 1, 2, etc. required
var
    key, versionKey: string;
    install, release, serviceCount, versionRelease: cardinal;
    success: boolean;
begin
    versionKey := version;
    versionRelease := 0;

    // .NET 1.1 and 2.0 embed release number in version key
    if version = 'v1.1' then begin
        versionKey := 'v1.1.4322';
    end else if version = 'v2.0' then begin
        versionKey := 'v2.0.50727';
    end

    // .NET 4.5 and newer install as update to .NET 4.0 Full
    else if Pos('v4.', version) = 1 then begin
        versionKey := 'v4\Full';
        case version of
          'v4.5':   versionRelease := 378389;
          'v4.5.1': versionRelease := 378675; // 378758 on Windows 8 and older
          'v4.5.2': versionRelease := 379893;
          'v4.6':   versionRelease := 393295; // 393297 on Windows 8.1 and older
          'v4.6.1': versionRelease := 394254; // 394271 on Windows 8.1 and older
          'v4.6.2': versionRelease := 394802; // 394806 on Windows 8.1 and older
        end;
    end;

    // installation key group for all .NET versions
    key := 'SOFTWARE\Microsoft\NET Framework Setup\NDP\' + versionKey;

    // .NET 3.0 uses value InstallSuccess in subkey Setup
    if Pos('v3.0', version) = 1 then begin
        success := RegQueryDWordValue(HKLM, key + '\Setup', 'InstallSuccess', install);
    end else begin
        success := RegQueryDWordValue(HKLM, key, 'Install', install);
    end;

    // .NET 4.0 and newer use value Servicing instead of SP
    if Pos('v4', version) = 1 then begin
        success := success and RegQueryDWordValue(HKLM, key, 'Servicing', serviceCount);
    end else begin
        success := success and RegQueryDWordValue(HKLM, key, 'SP', serviceCount);
    end;

    // .NET 4.5 and newer use additional value Release
    if versionRelease > 0 then begin
        success := success and RegQueryDWordValue(HKLM, key, 'Release', release);
        success := success and (release >= versionRelease);
    end;

    result := success and (install = 1) and (serviceCount >= service);
end;


function InitializeSetup(): Boolean;
begin
    if not IsDotNetDetected('v4.5.2', 0) then begin
        MsgBox('MyApp requires Microsoft .NET Framework 4.6.'#13#13
            'Please use Windows Update to install this version,'#13
            'and then re-run the MyApp setup program.', mbInformation, MB_OK);
        result := false;
    end else
        result := true;
end;
