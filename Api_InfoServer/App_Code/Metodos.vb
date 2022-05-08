Imports Ctec
Imports System.Data
Imports System.Threading
Imports System.IO

Public Class Metodos



#Region "PUBLIC"




    ''' <summary>
    ''' Permite Registrar un Log con el mensaje enviado.
    ''' </summary>
    ''' <param name="formulario">Fomulario, controlador donde se genera el mensaje</param>
    ''' <param name="funcion">Funcion que genera el mensaje</param>
    ''' <param name="numero">Paso</param>
    ''' <param name="Mensaje">Mensaje a registrar</param>
    ''' <param name="X">Indica si se genera linea con(*) (0) no, (1) al inicio, (2) al final, (3) inicio y final </param>
    Public Sub registrarLog(ByVal formulario As String, ByVal funcion As String, ByVal numero As Integer,
                                ByVal Mensaje As String, Optional X As Integer = 0)
        'Declara las variables.
        Dim year, mes, dia, segundos, ruta As String
        Dim fechaEvento As String
        'Se define la variable de hora actual
        Dim HoraActual As String = Now.Hour & ":" & Now.Minute & ":" & Now.Second
        'se definen las constantes de las horas en las cuales se ejecutara el hilo de borrado de logs
        Const HORA1 As String = "09:00:00"
        Const HORA2 As String = "09:30:00"
        Try
            'se crea instancia del hilo.
            Dim th As New Thread(AddressOf borrarLog)

            'Inicializa las variables.
            year = Now.Year.ToString
            mes = Now.Month.ToString
            dia = Now.Day.ToString
            segundos = Now.Second.ToString
            fechaEvento = Now.ToShortDateString & " " & Now.ToLongTimeString
            Dim appPath = HttpContext.Current.Request.ApplicationPath
            Dim physicalPath = HttpContext.Current.Request.MapPath(appPath)

            'validamos que existe la ruta y se crea de no existir.
            If (Directory.Exists(physicalPath & "\Logs") = False) Then
                Directory.CreateDirectory(physicalPath & "\Logs")
            End If

            'se define el nombre del archivo al cual se le registrara el log
            ruta = physicalPath & "\Logs\Log_(" & dia & "-" & mes & "-" & year & ").log"

            'valiamos que sea la hora para la ejecucion del hilo borrador de logs
            If (CDate(HoraActual) > CDate(HORA1)) And (CDate(HoraActual) < CDate(HORA2)) Then
                'se valida el estado del hilo para no volverlo a ejecutar
                If th.ThreadState = ThreadState.Unstarted Or th.ThreadState = ThreadState.Aborted Or th.ThreadState = ThreadState.Stopped Then
                    th.Start(physicalPath & "\Logs")
                End If
            End If

            Dim streamWriter As StreamWriter = New StreamWriter(ruta, True)

            'variable con 50(*)
            Dim asterisk As String = "**************************************************"
            'Escribe en el archivo de log.
            If X = 1 Or X = 3 Then
                streamWriter.WriteLine(vbNewLine & asterisk & asterisk & asterisk & asterisk & asterisk & asterisk)
            End If
            streamWriter.WriteLine(fechaEvento & vbTab & formulario & vbTab & funcion & vbTab & "Paso: " & numero.ToString & vbTab & Mensaje)
            If X = 2 Or X = 3 Then
                streamWriter.WriteLine(asterisk & asterisk & asterisk & asterisk & asterisk & asterisk & vbNewLine)
            End If
            streamWriter.Flush()
            streamWriter.Close()

        Catch ex As Exception
            Dim appPath = HttpContext.Current.Request.ApplicationPath
            Dim physicalPath = HttpContext.Current.Request.MapPath(appPath)
            Dim file As FileInfo = New FileInfo(physicalPath & "\Logs\")
            Directory.CreateDirectory(file.DirectoryName)
            registrarLog("Logs", "registrarLog", 0, ex.Message, 3)
        End Try
    End Sub
#End Region

#Region "PRIVATE"
    ''' <summary>
    ''' Funcion que elimina el regstro de archivos en la ruta enviada con mas de 60 dias
    ''' </summary>
    ''' <param name="RUTA">Ruta en la cual se eliminaran los archivos</param>
    Private Sub borrarLog(ByVal RUTA As String)
        Try

            Dim Fecha As DateTime = DateTime.Now

            For Each archivo As String In My.Computer.FileSystem.GetFiles(RUTA, FileIO.SearchOption.SearchTopLevelOnly)

                Dim Fecha_Archivo As DateTime = My.Computer.FileSystem.GetFileInfo(archivo).LastWriteTime
                Dim diferencia = (CType(Fecha, DateTime) - CType(Fecha_Archivo, DateTime)).TotalDays

                If diferencia >= 60 Then
                    File.Delete(archivo)
                End If

            Next

        Catch ex As Exception
            registrarLog("Logs", "BorrarLog ", 0, ex.Message, 3)
        End Try
    End Sub

#End Region

End Class
