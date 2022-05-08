Imports System.Net
Imports System.Web.Http
Imports System.ServiceProcess
Imports System.Net.NetworkInformation



Namespace Controllers
    <RoutePrefix("Api/ServerInfo")>
    Public Class InfoController
        Inherits ApiController

        <HttpGet>
        <Route("info")>
        Public Function GetValues() As IHttpActionResult

            Dim ping As New Ping
            Dim ip As IPAddress
            Dim info As New info
            Dim discos As New List(Of Discos)
            ip = IPAddress.Parse("127.0.0.1")
            Dim sIp As String = "127.0.0.1"
            For a = 0 To 5
                If My.Computer.Network.Ping(sIp) Then

                End If

            Next

            For i = 0 To 4
                Dim respose As PingReply = ping.Send(ip)
                If Not respose.Status.ToString.Equals("Success") Then
                End If
            Next



            For Each service In ServiceController.GetServices
                If service.ServiceName.Contains("Oracle") Then
                    If service.Status Then

                    End If
                    Dim status As String = service.ServiceName.Contains("TNSListener")
                End If


            Next

            For Each driver In My.Computer.FileSystem.Drives

                Dim type As String = driver.DriveType
                If type = 3 Then
                    Dim disco As New Discos
                    disco.espacioLibre = converTogb(driver.AvailableFreeSpace) & " GB"
                    disco.espaciototal = converTogb(driver.TotalSize) & " GB"
                    disco.label = driver.VolumeLabel
                    disco.unidad = driver.Name
                    discos.Add(disco)
                End If

                Dim drive As String = My.Computer.FileSystem.Drives.ToString
            Next


            Dim memoryT = converTogb(My.Computer.Info.TotalPhysicalMemory) & " GB"
            Dim memoryD = converTogb(My.Computer.Info.AvailablePhysicalMemory) & " GB"

            info.memoryDisponible = memoryD
            info.memoryTotala = memoryT
            info.discos = discos

            Return Ok(info)

        End Function
        Private Function converTogb(bites As String) As String
            Dim bytes = Double.Parse(bites) / 8
            Dim kb = bites / 1024
            Dim mb = kb / 1024
            Dim gb = mb / 1024
            Return CDec(gb).ToString
            'Return CInt(gb).ToString
        End Function

    End Class
End Namespace