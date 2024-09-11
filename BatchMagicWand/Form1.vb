Imports System.Drawing.Imaging
Imports System.IO

Public Class Form1
    Public myimg As Bitmap
    Public myimgorig As Bitmap
    Public allpoints As New HashSet(Of Integer())
    Public mythresh As Integer

    Public myr As Integer
    Public myg As Integer
    Public myb As Integer
    Public mya As Integer

    Public myr2 As Integer
    Public myg2 As Integer
    Public myb2 As Integer
    Public mya2 As Integer

    Public myclr As Color
    Public myclrorig As Color

    Public myx As Integer
    Public myy As Integer

    Public mythreshr As Integer
    Public mythreshg As Integer
    Public mythreshb As Integer

    Public myfiles As FileInfo()
    Public myindex As Integer
    Public myload As Boolean
    Public shouldexit As Boolean

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        ProcessIMGCatch()
    End Sub

    Private Sub ProcessIMGCatch()
        Try
            ProcessIMG()
        Catch ex As Exception
        End Try
    End Sub

    Public Sub ProcessIMG()
        myimg = New Bitmap(myimgorig)
        If TabControl1.SelectedIndex = 0 Then
            mythreshr = Integer.Parse(TextBox3.Text)
            mythreshg = Integer.Parse(TextBox3.Text)
            mythreshb = Integer.Parse(TextBox3.Text)
        End If
        If TabControl1.SelectedIndex = 1 Then
            mythreshr = Integer.Parse(TextBox8.Text)
            mythreshg = Integer.Parse(TextBox9.Text)
            mythreshb = Integer.Parse(TextBox10.Text)
        End If
        myx = Integer.Parse(TextBox1.Text)
        myy = Integer.Parse(TextBox2.Text)
        myclrorig = myimg.GetPixel(myx, myy)

        myr = Integer.Parse(TextBox4.Text)
        myg = Integer.Parse(TextBox5.Text)
        myb = Integer.Parse(TextBox6.Text)
        mya = Integer.Parse(TextBox7.Text)

        myr2 = myclrorig.R
        myg2 = myclrorig.G
        myb2 = myclrorig.B
        mya2 = myclrorig.A
        myclr = Color.FromArgb(mya, myr, myg, myb)
        allpoints.Clear()

        SetColor(Integer.Parse(TextBox1.Text), Integer.Parse(TextBox2.Text))

        If CheckBox1.Checked Then
            Dim bgcolormy As Color
            bgcolormy = Color.FromArgb(Integer.Parse(TextBox20.Text), Integer.Parse(TextBox21.Text), Integer.Parse(TextBox23.Text), Integer.Parse(TextBox22.Text))

            For y = 1 To myimg.Height
                For x = 1 To myimg.Width
                    If Not myimg.GetPixel(x - 1, y - 1).Equals(myclr) Then
                        myimg.SetPixel(x - 1, y - 1, bgcolormy)
                    End If
                Next
            Next
        End If

        PictureBox1.Image = myimg
    End Sub

    Private Sub SetColor(startX As Integer, startY As Integer)
        ' Use a Queue to handle the pixels to be processed iteratively
        Dim pixelsToProcess As New Queue(Of Point)
        pixelsToProcess.Enqueue(New Point(startX, startY))

        ' Set to keep track of processed points to prevent processing the same pixel multiple times
        Dim processedPoints As New HashSet(Of Point)()

        While pixelsToProcess.Count > 0
            Dim currentPoint As Point = pixelsToProcess.Dequeue()
            Dim x As Integer = currentPoint.X
            Dim y As Integer = currentPoint.Y

            ' Ensure pixel coordinates are within bounds
            If x >= 0 AndAlso y >= 0 AndAlso x < myimg.Width AndAlso y < myimg.Height Then
                If Not processedPoints.Contains(currentPoint) Then
                    processedPoints.Add(currentPoint)

                    Dim currentColor As Color = myimg.GetPixel(x, y)

                    ' Check color similarity using a threshold
                    If Math.Abs(currentColor.R - myr2) < mythreshr AndAlso
                   Math.Abs(currentColor.G - myg2) < mythreshg AndAlso
                   Math.Abs(currentColor.B - myb2) < mythreshb Then

                        ' Set the new color
                        myimg.SetPixel(x, y, myclr)

                        ' Add neighboring pixels to the queue to be processed
                        pixelsToProcess.Enqueue(New Point(x - 1, y)) ' Left
                        pixelsToProcess.Enqueue(New Point(x + 1, y)) ' Right
                        pixelsToProcess.Enqueue(New Point(x, y - 1)) ' Up
                        pixelsToProcess.Enqueue(New Point(x, y + 1)) ' Down
                    End If
                End If
            End If
        End While

        ' Refresh the PictureBox to display the updated image
        PictureBox1.Image = myimg
        PictureBox1.Refresh()
    End Sub


    Private Sub SetColor3(v1 As Integer, v2 As Integer)
        Try
            If (v1 > 0 AndAlso v2 > 0 AndAlso v1 < myimg.Width AndAlso v2 < myimg.Height) AndAlso allpoints.Where(Function(x) x(0) = v1 AndAlso x(1) = v2).Count = 0 Then
                allpoints.Add({v1, v2})
                Dim mycolortm As Color
                mycolortm = myimg.GetPixel(v1, v2)
                If Math.Abs(mycolortm.R - myr) < mythresh AndAlso Math.Abs(mycolortm.G - myg) < mythresh AndAlso Math.Abs(mycolortm.B - myb) < mythresh Then
                    myimg.SetPixel(v1, v2, myclr)
                    SetColor(v1 - 1, v2)
                    SetColor(v1 + 1, v2)
                    SetColor(v1, v2 - 1)
                    SetColor(v1, v2 + 1)
                    'Application.DoEvents()
                End If
            End If
        Catch ex As StackOverflowException
        End Try
    End Sub

    Private Sub SetColor2(v1 As Integer, v2 As Integer)
        ' Use a Stack to handle the pixels to be processed iteratively
        Dim pixelsToProcess As Stack(Of Point)
        pixelsToProcess = New Stack(Of Point)
        pixelsToProcess.Push(New Point(v1, v2))

        ' Set to keep track of processed points to prevent processing the same pixel multiple times
        Dim processedPoints As HashSet(Of Point)
        processedPoints = New HashSet(Of Point)
        While pixelsToProcess.Count > 0
            Dim currentPoint As Point = pixelsToProcess.Pop()
            v1 = currentPoint.X
            v2 = currentPoint.Y

            ' Check if the point is within bounds and hasn't been processed yet
            If (v1 >= 0 AndAlso v2 >= 0 AndAlso v1 < myimg.Width AndAlso v2 < myimg.Height) AndAlso Not processedPoints.Contains(currentPoint) Then
                processedPoints.Add(currentPoint)

                Dim mycolortm As Color = myimg.GetPixel(v1, v2)

                ' Check color similarity with the threshold
                If Math.Abs(mycolortm.R - myr) < mythreshr AndAlso Math.Abs(mycolortm.G - myg) < mythreshg AndAlso Math.Abs(mycolortm.B - myb) < mythreshb Then
                    ' Set the new color
                    myimg.SetPixel(v1, v2, myclr)

                    ' Add neighboring pixels to the stack to be processed
                    pixelsToProcess.Push(New Point(v1 - 1, v2)) ' Left
                    pixelsToProcess.Push(New Point(v1 + 1, v2)) ' Right
                    pixelsToProcess.Push(New Point(v1, v2 - 1)) ' Up
                    pixelsToProcess.Push(New Point(v1, v2 + 1)) ' Down
                End If
            End If
        End While
    End Sub


    Private Sub Form1_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        myimgorig = New Bitmap("C:/Users/ad48/Documents/building96.png")
        myimg = myimgorig
        PictureBox1.Image = myimg
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        ColorDialog1.ShowDialog()
        TextBox4.Text = ColorDialog1.Color.R
        TextBox5.Text = ColorDialog1.Color.G
        TextBox6.Text = ColorDialog1.Color.B
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim mydialog As New OpenFileDialog()
        mydialog.ShowDialog()

        If File.Exists(mydialog.FileName) Then
            LoadImage(mydialog.FileName)
        End If
    End Sub

    Private Sub LoadImage(myfile As String)
        myimgorig = New Bitmap(myfile)
        myimgorig = myimgorig.Clone(New Rectangle(0, 0, myimgorig.Width, myimgorig.Height), PixelFormat.Format32bppArgb)
        myimg = myimgorig
        PictureBox1.Image = myimg
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        If TextBox11.Text = "" Then

            Dim mydialog As New FolderBrowserDialog()
            mydialog.ShowDialog()

            If Directory.Exists(mydialog.SelectedPath) Then
                ProcessDir(mydialog.SelectedPath)
            End If
        Else
            If Not Directory.Exists(TextBox11.Text) Then
                Directory.CreateDirectory(TextBox11.Text)
            End If

            ProcessDir(TextBox11.Text)
        End If
        myload = True
    End Sub

    Private Sub ProcessDir(mydirtm As String)
        TextBox11.Text = TextBox11.Text.Replace("\", "/")
        TextBox12.Text = TextBox12.Text.Replace("\", "/")

        While TextBox12.Text.EndsWith("/")
            TextBox12.Text = TextBox12.Text.Substring(0, TextBox12.Text.Length - 1)
        End While

        myfiles = {}
        Dim mydirinfo As DirectoryInfo
        mydirinfo = New DirectoryInfo(mydirtm)
        myfiles = mydirinfo.GetFiles()

        If myfiles.Length > 0 Then
            LoadImage(myfiles(0).FullName)
            ProcessIMGCatch()
        End If

        TextBox14.Text = "0"
    End Sub

    Private Sub RunDir(outputdir As String)

        Dim myi As Integer
        myi = 0

        For Each itemmy In myfiles
            myi += 1

            Dim myitem2 As String
            myitem2 = ""

            If Integer.Parse(TextBox7.Text) = 255 Then
                myitem2 = itemmy.Name
            Else
                myitem2 = itemmy.Name.Replace(itemmy.Extension.Replace(".", ""), "png")
            End If

            If Not File.Exists(outputdir + "/" + myitem2) Then
                LoadImage(itemmy.FullName)
                ProcessIMGCatch()

                TextBox13.Text = myi.ToString() + " / " + myfiles.Length.ToString() + " = " + myitem2
                myimg.Save(outputdir + "/" + myitem2)
            End If

            If shouldexit Then
                Exit Sub
            End If

            Application.DoEvents()
        Next
    End Sub

    Private Sub Form1_DragDrop(sender As System.Object, e As System.Windows.Forms.DragEventArgs) Handles TextBox16.DragDrop
        Dim files() As String = e.Data.GetData(DataFormats.FileDrop)
        TextBox16.Text = files(0)
    End Sub

    Private Sub Form1_DragEnter(sender As System.Object, e As System.Windows.Forms.DragEventArgs) Handles TextBox16.DragEnter
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.Copy
        End If
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        shouldexit = False
        If Not Directory.Exists(TextBox12.Text) Then
            Directory.CreateDirectory(TextBox12.Text)
        End If
        RunDir(TextBox12.Text)
    End Sub

    Private Sub ProcessIMGChange()
        LoadImage(myfiles(myindex).FullName)
        ProcessIMGCatch()
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        myindex = Integer.Parse(TextBox14.Text) - 1
        If myindex > 0 Then
            TextBox14.Text = myindex.ToString()
        End If
        ProcessIMGChange()
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        myindex = Integer.Parse(TextBox14.Text) + 1
        If myindex < myfiles.Count Then
            TextBox14.Text = myindex.ToString()
        End If
        ProcessIMGChange()
    End Sub

    Private Sub TextBox14_TextChanged(sender As Object, e As EventArgs) Handles TextBox14.TextChanged
        If Integer.TryParse(TextBox14.Text, 1) AndAlso myload Then
            myindex = Integer.Parse(TextBox14.Text)
            If myindex > 0 AndAlso myindex < myfiles.Count Then
                ProcessIMGChange()
            End If
        End If
    End Sub

    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
        TextBox16.Text = TextBox16.Text.Replace("\", "/")
        TextBox15.Text = TextBox15.Text.Replace("\", "/")

        While TextBox15.Text.EndsWith("/")
            TextBox15.Text = TextBox15.Text.Substring(0, TextBox15.Text.Length - 1)
        End While

        If Not Directory.Exists(TextBox15.Text) Then
            Directory.CreateDirectory(TextBox15.Text)
        End If

        If TextBox16.Text = "" Then
            Dim mydialog As New OpenFileDialog()
            mydialog.ShowDialog()

            If Directory.Exists(mydialog.FileName) Then
                TextBox16.Text = mydialog.FileName
            End If
        End If

        ConvertVideoToImages()

        If TextBox15.Text = "" Then
            Dim mydialog As New FolderBrowserDialog()
            mydialog.ShowDialog()

            If Directory.Exists(mydialog.SelectedPath) Then
                TextBox15.Text = mydialog.SelectedPath
            End If
        End If
        myload = True

        ProcessDir(TextBox15.Text)
    End Sub

    Public Sub ConvertVideoToImages()
        Dim myprocessinfo As New ProcessStartInfo
        myprocessinfo.FileName = textbox17.Text
        myprocessinfo.Arguments = "-i " + ChrW(34) + TextBox16.Text + ChrW(34) + " " + ChrW(34) + TextBox15.Text + "/%1d.png" + ChrW(34)
        Dim myprocess As Process
        myprocess = Process.Start(myprocessinfo)
        myprocess.WaitForExit()
    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        shouldexit = False

        If myfiles Is Nothing Then
            ProcessDir(TextBox15.Text)
        End If

        TextBox19.Text = TextBox19.Text.Replace("\", "/")
        While TextBox19.Text.EndsWith("/")
            TextBox19.Text = TextBox19.Text.Substring(0, TextBox19.Text.Length - 1)
        End While
        If Not Directory.Exists(TextBox19.Text) Then
            Directory.CreateDirectory(TextBox19.Text)
        End If
        RunDir(TextBox19.Text)

        TextBox18.Text = TextBox18.Text.Replace("\", "/")

        If shouldexit Then
            Exit Sub
        End If

        ConvertImagesToVideo()
    End Sub

    Public Sub ConvertImagesToVideo()
        Dim myprocessinfo As New ProcessStartInfo
        myprocessinfo.FileName = TextBox17.Text
        myprocessinfo.Arguments = "-i " + ChrW(34) + TextBox15.Text + "/%1d.png" + ChrW(34) + " -c:v libx264 " + ChrW(34) + TextBox18.Text + ChrW(34)
        Dim myprocess As Process
        myprocess = Process.Start(myprocessinfo)
        myprocess.WaitForExit()
    End Sub

    Private Sub Button10_Click(sender As Object, e As EventArgs) Handles Button10.Click
        shouldexit = True
        TextBox13.Text = ""
    End Sub
End Class
