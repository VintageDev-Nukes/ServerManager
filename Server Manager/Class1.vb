' [ INI File Manager ]
'
' // By Elektro H@cker

#Region " Usage Examples "

'' Set the initialization file path.
'INIFileManager.FilePath = IO.Path.Combine(Application.StartupPath, "Config.ini")

'' Create the initialization file.
'INIFileManager.File.Create()

'' Check that the initialization file exist.
'MsgBox(INIFileManager.File.Exist)

'' Writes a new entire initialization file with the specified text content.
'INIFileManager.File.Write(New List(Of String) From {"[Section Name 1]"})

'' Set an existing value or append it at the enf of the initialization file.
'INIFileManager.Key.Set("KeyName1", "Value1")

'' Set an existing value on a specific section or append them at the enf of the initialization file.
'INIFileManager.Key.Set("KeyName2", "Value2", "[Section Name 2]")

'' Gets the value of the specified Key name,
'MsgBox(INIFileManager.Key.Get("KeyName1"))

'' Gets the value of the specified Key name on the specified Section.
'MsgBox(INIFileManager.Key.Get("KeyName2", , "[Section Name 2]"))

'' Gets the value of the specified Key name and returns a default value if the key name is not found.
'MsgBox(INIFileManager.Key.Get("KeyName0", "I'm a default value"))

'' Gets the value of the specified Key name, and assign it to a control property.
'CheckBox1.Checked = CType(INIFileManager.Key.Get("KeyName1"), Boolean)

'' Checks whether a Key exists.
'MsgBox(INIFileManager.Key.Exist("KeyName1"))

'' Checks whether a Key exists on a specific section.
'MsgBox(INIFileManager.Key.Exist("KeyName2", "[First Section]"))

'' Remove a key name.
'INIFileManager.Key.Remove("KeyName1")

'' Remove a key name on the specified Section.
'INIFileManager.Key.Remove("KeyName2", "[Section Name 2]")

'' Add a new section.
'INIFileManager.Section.Add("[Section Name 3]")

'' Get the contents of a specific section.
'MsgBox(String.Join(Environment.NewLine, INIFileManager.Section.Get("[Section Name 1]")))

'' Remove an existing section.
'INIFileManager.Section.Remove("[Section Name 2]")

'' Checks that the initialization file contains at least one section.
'MsgBox(INIFileManager.Section.Has())

'' Sort the initialization file (And remove empty lines).
'INIFileManager.File.Sort(True)

'' Gets the initialization file section names.
'MsgBox(String.Join(", ", INIFileManager.Section.GetNames()))

'' Gets the initialization file content.
'MsgBox(String.Join(Environment.NewLine, INIFileManager.File.Get()))

'' Delete the initialization file from disk.
'INIFileManager.File.Delete()

#End Region

#Region " INI File Manager "

Public Class INIFileManager

#Region " Members "

#Region " Properties "

    ''' <summary>
    ''' Indicates the initialization file path.
    ''' </summary>
    Public Shared Property FilePath As String =
        IO.Path.Combine(Application.StartupPath, Process.GetCurrentProcess().ProcessName & ".ini")

#End Region

#Region " Variables "

    ''' <summary>
    ''' Stores the initialization file content.
    ''' </summary>
    Private Shared Content As New List(Of String)

    ''' <summary>
    ''' Stores the INI section names.
    ''' </summary>
    Private Shared SectionNames As String() = {String.Empty}

    ''' <summary>
    ''' Indicates the start element index of a section name.
    ''' </summary>
    Private Shared SectionStartIndex As Integer = -1

    ''' <summary>
    ''' Indicates the end element index of a section name.
    ''' </summary>
    Private Shared SectionEndIndex As Integer = -1

    ''' <summary>
    ''' Stores a single sorted section block with their keys and values.
    ''' </summary>
    Private Shared SortedSection As New List(Of String)

    ''' <summary>
    ''' Stores all the sorted section blocks with their keys and values.
    ''' </summary>
    Private Shared SortedSections As New List(Of String)

    ''' <summary>
    ''' Indicates the INI element index that contains the Key and value.
    ''' </summary>
    Private Shared KeyIndex As Integer = -1

    ''' <summary>
    ''' Indicates the culture to compare the strings.
    ''' </summary>
    Private Shared ReadOnly CompareMode As StringComparison = StringComparison.InvariantCultureIgnoreCase

#End Region

#Region " Exceptions "

    ''' <summary>
    ''' Exception is thrown when a section name parameter has invalid format.
    ''' </summary>
    Private Class SectionNameInvalidFormatException
        Inherits Exception

        Public Sub New()
            MyBase.New("Section name parameter has invalid format." &
                       Environment.NewLine &
                       "The rigth syntax is: [SectionName]")
        End Sub

        Public Sub New(message As String)
            MyBase.New(message)
        End Sub

        Public Sub New(message As String, inner As Exception)
            MyBase.New(message, inner)
        End Sub

    End Class

#End Region

#End Region

#Region " Methods "

    <System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>
    Private Shadows Sub ReferenceEquals()
    End Sub

    <System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>
    Private Shadows Sub Equals()
    End Sub

    Public Class [File]

        <System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>
        Private Shadows Sub ReferenceEquals()
        End Sub

        <System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>
        Private Shadows Sub Equals()
        End Sub

        ''' <summary>
        ''' Checks whether the initialization file exist.
        ''' </summary>
        ''' <returns>True if initialization file exist, otherwise False.</returns>
        Public Shared Function Exist() As Boolean
            Return IO.File.Exists(FilePath)
        End Function

        ''' <summary>
        ''' Creates the initialization file.
        ''' If the file already exist it would be replaced.
        ''' </summary>
        ''' <param name="Encoding">The Text encoding to write the initialization file.</param>
        ''' <returns>True if the operation success, otherwise False.</returns>
        Public Shared Function Create(Optional ByVal Encoding As System.Text.Encoding = Nothing) As Boolean

            Try
                IO.File.WriteAllText(FilePath,
                                     String.Empty,
                                     If(Encoding Is Nothing, System.Text.Encoding.Default, Encoding))
            Catch ex As Exception
                Throw
                Return False

            End Try

            Return True

        End Function

        ''' <summary>
        ''' Deletes the initialization file.
        ''' </summary>
        ''' <returns>True if the operation success, otherwise False.</returns>
        Public Shared Function Delete() As Boolean

            If Not [File].Exist Then Return False

            Try
                IO.File.Delete(FilePath)
            Catch ex As Exception
                Throw
                Return False

            End Try

            Content = Nothing

            Return True

        End Function

        ''' <summary>
        ''' Returns the initialization file content.
        ''' </summary>
        ''' <param name="Encoding">The Text encoding to read the initialization file.</param>
        Public Shared Function [Get](Optional ByVal Encoding As System.Text.Encoding = Nothing) As List(Of String)

            Content = IO.File.ReadAllLines(FilePath,
                                           If(Encoding Is Nothing, System.Text.Encoding.Default, Encoding)).ToList()

            Return Content

        End Function

        ''' <summary>
        ''' Sort the initialization file content by the Key names.
        ''' If the initialization file contains sections then the sections are sorted by their names also.
        ''' </summary>
        ''' <param name="RemoveEmptyLines">Remove empty lines.</param>
        ''' <param name="Encoding">The Text encoding to read the initialization file.</param>
        ''' <returns>True if the operation success, otherwise False.</returns>
        Public Shared Function Sort(Optional ByVal RemoveEmptyLines As Boolean = False,
                                    Optional ByVal Encoding As System.Text.Encoding = Nothing) As Boolean

            If Not [File].Exist() Then Return False

            [File].[Get](Encoding)

            Select Case Section.Has(Encoding)

                Case True ' initialization file contains at least one Section.

                    SortedSection.Clear()
                    SortedSections.Clear()

                    Section.GetNames(Encoding) ' Get the (sorted) section names

                    For Each name As String In SectionNames

                        SortedSection = Section.[Get](name, Encoding) ' Get the single section lines.

                        If RemoveEmptyLines Then ' Remove empty lines.
                            SortedSection = SortedSection.Where(Function(line) _
                                                                Not String.IsNullOrEmpty(line) AndAlso
                                                                Not String.IsNullOrWhiteSpace(line)).ToList
                        End If

                        SortedSection.Sort() ' Sort the single section keys.

                        SortedSections.Add(name) ' Add the section name to the sorted sections list.
                        SortedSections.AddRange(SortedSection) ' Add the single section to the sorted sections list.

                    Next name

                    Content = SortedSections

                Case False ' initialization file doesn't contains any Section.
                    Content.Sort()

                    If RemoveEmptyLines Then
                        Content = Content.Where(Function(line) _
                                                        Not String.IsNullOrEmpty(line) AndAlso
                                                        Not String.IsNullOrWhiteSpace(line)).ToList
                    End If

            End Select ' Section.Has()

            ' Save changes.
            Return [File].Write(Content, Encoding)

        End Function

        ''' <summary>
        ''' Writes a new initialization file with the specified text content..
        ''' </summary>
        ''' <param name="Content">Indicates the text content to write in the initialization file.</param>
        ''' <param name="Encoding">The Text encoding to read the initialization file.</param>
        ''' <returns>True if the operation success, otherwise False.</returns>
        Public Shared Function Write(ByVal Content As List(Of String),
                                     Optional ByVal Encoding As System.Text.Encoding = Nothing) As Boolean

            Try
                IO.File.WriteAllLines(FilePath,
                                      Content,
                                      If(Encoding Is Nothing, System.Text.Encoding.Default, Encoding))
            Catch ex As Exception
                Throw
                Return False

            End Try

            Return True

        End Function

    End Class

    Public Class [Key]

        <System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>
        Private Shadows Sub ReferenceEquals()
        End Sub

        <System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>
        Private Shadows Sub Equals()
        End Sub

        ''' <summary>
        ''' Return a value indicating whether a key name exist or not.
        ''' </summary>
        ''' <param name="KeyName">Indicates the key name that contains the value to modify.</param>
        ''' <param name="SectionName">Indicates the Section name where to find the key name.</param>
        ''' <param name="Encoding">The Text encoding to write the initialization file.</param>
        ''' <returns>True if the key name exist, otherwise False.</returns>
        Public Shared Function Exist(ByVal KeyName As String,
                                     Optional ByVal SectionName As String = Nothing,
                                     Optional ByVal Encoding As System.Text.Encoding = Nothing) As Boolean

            If Not [File].Exist() Then Return False

            [File].[Get](Encoding)

            [Key].GetIndex(KeyName, SectionName)

            Select Case SectionName Is Nothing

                Case True
                    Return Convert.ToBoolean(Not KeyIndex)

                Case Else
                    Return Convert.ToBoolean(Not (KeyIndex + SectionStartIndex))

            End Select

        End Function

        ''' <summary>
        ''' Set the value of an existing key name.
        ''' 
        ''' If the initialization file doesn't exists, or else the Key doesn't exist,
        ''' or else the Section parameter is not specified and the key name doesn't exist;
        ''' then the 'key=value' is appended to the end of the initialization file.
        ''' 
        ''' if the specified Section name exist but the Key name doesn't exist,
        ''' then the 'key=value' is appended to the end of the Section.
        ''' 
        ''' </summary>
        ''' <param name="KeyName">Indicates the key name that contains the value to modify.</param>
        ''' <param name="Value">Indicates the new value.</param>
        ''' <param name="SectionName">Indicates the Section name where to find the key name.</param>
        ''' <param name="Encoding">The Text encoding to write the initialization file.</param>
        ''' <returns>True if the operation success, otherwise False.</returns>
        Public Shared Function [Set](ByVal KeyName As String,
                                     ByVal Value As String,
                                     Optional ByVal SectionName As String = Nothing,
                                     Optional ByVal Encoding As System.Text.Encoding = Nothing) As Boolean

            If Not [File].Exist() Then [File].Create()

            [File].[Get](Encoding)

            [Key].GetIndex(KeyName, SectionName)

            ' If KeyName is not found and indicated Section is found, then...
            If KeyIndex = -1 AndAlso SectionEndIndex <> -1 Then

                ' If section EndIndex is the last line of file, then...
                If SectionEndIndex = Content.Count Then

                    Content(Content.Count - 1) = Content(Content.Count - 1) &
                                                         Environment.NewLine &
                                                         String.Format("{0}={1}", KeyName, Value)

                Else ' If not section EndIndex is the last line of file, then...

                    Content(SectionEndIndex) = String.Format("{0}={1}", KeyName, Value) &
                                                    Environment.NewLine &
                                                    Content(SectionEndIndex)
                End If

                ' If KeyName is found then...
            ElseIf KeyIndex <> -1 Then
                Content(KeyIndex) = String.Format("{0}={1}", KeyName, Value)

                ' If KeyName is not found and Section parameter is passed. then...
            ElseIf KeyIndex = -1 AndAlso SectionName IsNot Nothing Then
                Content.Add(SectionName)
                Content.Add(String.Format("{0}={1}", KeyName, Value))

                ' If KeyName is not found, then...
            ElseIf KeyIndex = -1 Then
                Content.Add(String.Format("{0}={1}", KeyName, Value))

            End If

            ' Save changes.
            Return [File].Write(Content, Encoding)

        End Function

        ''' <summary>
        ''' Get the value of an existing key name.
        ''' If the initialization file or else the Key doesn't exist then a 'Nothing' object is returned. 
        ''' </summary>
        ''' <param name="KeyName">Indicates the key name to retrieve their value.</param>
        ''' <param name="DefaultValue">Indicates a default value to return if the key name is not found.</param>
        ''' <param name="SectionName">Indicates the Section name where to find the key name.</param>
        ''' <param name="Encoding">The Text encoding to read the initialization file.</param>
        Public Shared Function [Get](ByVal KeyName As String,
                                     Optional ByVal DefaultValue As Object = Nothing,
                                     Optional ByVal SectionName As String = Nothing,
                                     Optional ByVal Encoding As System.Text.Encoding = Nothing) As Object

            If Not [File].Exist() Then Return DefaultValue

            [File].[Get](Encoding)

            [Key].GetIndex(KeyName, SectionName)

            Select Case KeyIndex

                Case Is <> -1 ' KeyName found.
                    Return Content(KeyIndex).Substring(Content(KeyIndex).IndexOf("=") + 1)

                Case Else ' KeyName not found.
                    Return DefaultValue

            End Select

        End Function

        ''' <summary>
        ''' Returns the initialization file line index of the key name.
        ''' </summary>
        ''' <param name="KeyName">Indicates the Key name to retrieve their value.</param>
        ''' <param name="SectionName">Indicates the Section name where to find the key name.</param>
        ''' <param name="Encoding">The Text encoding to read the initialization file.</param>
        Private Shared Sub GetIndex(ByVal KeyName As String,
                                    Optional ByVal SectionName As String = Nothing,
                                    Optional ByVal Encoding As System.Text.Encoding = Nothing)

            If Content Is Nothing Then [File].Get(Encoding)

            ' Reset the INI index elements to negative values.
            KeyIndex = -1
            SectionStartIndex = -1
            SectionEndIndex = -1

            If SectionName IsNot Nothing AndAlso Not SectionName Like "[[]?*[]]" Then
                Throw New SectionNameInvalidFormatException
                Exit Sub
            End If

            ' Locate the KeyName and set their element index.
            ' If the KeyName is not found then the value is set to "-1" to return an specified default value.
            Select Case String.IsNullOrEmpty(SectionName)

                Case True ' Any SectionName parameter is specified.

                    KeyIndex = Content.FindIndex(Function(line) line.StartsWith(String.Format("{0}=", KeyName),
                                                                              StringComparison.InvariantCultureIgnoreCase))

                Case False ' SectionName parameter is specified.

                    Select Case Section.Has(Encoding)

                        Case True ' INI contains at least one Section.

                            SectionStartIndex = Content.FindIndex(Function(line) line.Trim.Equals(SectionName.Trim, CompareMode))
                            If SectionStartIndex = -1 Then ' Section doesn't exist.
                                Exit Sub
                            End If

                            SectionEndIndex = Content.FindIndex(SectionStartIndex + 1, Function(line) line.Trim Like "[[]?*[]]")
                            If SectionEndIndex = -1 Then
                                ' This fixes the value if the section is at the end of file.
                                SectionEndIndex = Content.Count
                            End If

                            KeyIndex = Content.FindIndex(SectionStartIndex, SectionEndIndex - SectionStartIndex,
                                                                  Function(line) line.StartsWith(String.Format("{0}=", KeyName),
                                                                                      StringComparison.InvariantCultureIgnoreCase))

                        Case False ' INI doesn't contains Sections.
                            GetIndex(KeyName, , Encoding)

                    End Select ' Section.Has()

            End Select ' String.IsNullOrEmpty(SectionName)

        End Sub

        ''' <summary>
        ''' Remove an existing key name.
        ''' </summary>
        ''' <param name="KeyName">Indicates the key name to retrieve their value.</param>
        ''' <param name="SectionName">Indicates the Section name where to find the key name.</param>
        ''' <param name="Encoding">The Text encoding to read the initialization file.</param>
        ''' <returns>True if the operation success, otherwise False.</returns>
        Public Shared Function Remove(ByVal KeyName As String,
                                      Optional ByVal SectionName As String = Nothing,
                                      Optional ByVal Encoding As System.Text.Encoding = Nothing) As Boolean

            If Not [File].Exist() Then Return False

            [File].[Get](Encoding)

            [Key].GetIndex(KeyName, SectionName)

            Select Case KeyIndex

                Case Is <> -1 ' Key found.

                    ' Remove the element containing the key name.
                    Content.RemoveAt(KeyIndex)

                    ' Save changes.
                    Return [File].Write(Content, Encoding)

                Case Else ' KeyName not found.
                    Return False

            End Select

        End Function

    End Class

    Public Class Section

        <System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>
        Private Shadows Sub ReferenceEquals()
        End Sub

        <System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>
        Private Shadows Sub Equals()
        End Sub

        ''' <summary>
        ''' Adds a new section at bottom of the initialization file.
        ''' </summary>
        ''' <param name="SectionName">Indicates the Section name to add.</param>
        ''' <param name="Encoding">The Text encoding to read the initialization file.</param>
        ''' <returns>True if the operation success, otherwise False.</returns>
        Public Shared Function Add(Optional ByVal SectionName As String = Nothing,
                                   Optional ByVal Encoding As System.Text.Encoding = Nothing) As Boolean

            If Not [File].Exist() Then [File].Create()

            If Not SectionName Like "[[]?*[]]" Then
                Throw New SectionNameInvalidFormatException
                Exit Function
            End If

            [File].[Get](Encoding)

            Select Case Section.GetNames(Encoding).Where(Function(line) line.Trim.Equals(SectionName.Trim, CompareMode)).Any

                Case False ' Any of the existing Section names is equal to given section name.

                    ' Add the new section name.
                    Content.Add(SectionName)

                    ' Save changes.
                    Return [File].Write(Content, Encoding)

                Case Else ' An existing Section name is equal to given section name.
                    Return False

            End Select

        End Function

        ''' <summary>
        ''' Returns all the keys and values of an existing Section Name.
        ''' </summary>
        ''' <param name="Encoding">The Text encoding to read the initialization file.</param>
        ''' <param name="SectionName">Indicates the section name where to retrieve their keynames and values.</param>
        Public Shared Function [Get](ByVal SectionName As String,
                                     Optional ByVal Encoding As System.Text.Encoding = Nothing) As List(Of String)

            If Content Is Nothing Then [File].Get(Encoding)

            SectionStartIndex = Content.FindIndex(Function(line) line.Trim.Equals(SectionName.Trim, CompareMode))

            SectionEndIndex = Content.FindIndex(SectionStartIndex + 1, Function(line) line.Trim Like "[[]?*[]]")

            If SectionEndIndex = -1 Then
                SectionEndIndex = Content.Count ' This fixes the value if the section is at the end of file.
            End If

            Return Content.GetRange(SectionStartIndex, SectionEndIndex - SectionStartIndex).Skip(1).ToList

        End Function

        ''' <summary>
        ''' Returns all the section names of the initialization file.
        ''' </summary>
        ''' <param name="Encoding">The Text encoding to read the initialization file.</param>
        Public Shared Function GetNames(Optional ByVal Encoding As System.Text.Encoding = Nothing) As String()

            If Content Is Nothing Then [File].Get(Encoding)

            ' Get the Section names.
            SectionNames = (From line In Content Where line.Trim Like "[[]?*[]]").ToArray

            ' Sort the Section names.
            If SectionNames.Count <> 0 Then Array.Sort(SectionNames)

            ' Return the Section names.
            Return SectionNames

        End Function

        ''' <summary>
        ''' Gets a value indicating whether the initialization file contains at least one Section.
        ''' </summary>
        ''' <param name="Encoding">The Text encoding to read the initialization file.</param>
        ''' <returns>True if the INI contains at least one section, otherwise False.</returns>
        Public Shared Function Has(Optional ByVal Encoding As System.Text.Encoding = Nothing) As Boolean

            If Content Is Nothing Then [File].Get(Encoding)

            Return (From line In Content Where line.Trim Like "[[]?*[]]").Any()

        End Function

        ''' <summary>
        ''' Removes an existing section with all of it's keys and values.
        ''' </summary>
        ''' <param name="SectionName">Indicates the Section name to remove with all of it's key/values.</param>
        ''' <param name="Encoding">The Text encoding to read the initialization file.</param>
        ''' <returns>True if the operation success, otherwise False.</returns>
        Public Shared Function Remove(Optional ByVal SectionName As String = Nothing,
                                      Optional ByVal Encoding As System.Text.Encoding = Nothing) As Boolean

            If Not [File].Exist() Then Return False

            If Not SectionName Like "[[]?*[]]" Then
                Throw New SectionNameInvalidFormatException
                Exit Function
            End If

            [File].[Get](Encoding)

            Select Case [Section].GetNames(Encoding).Where(Function(line) line.Trim.Equals(SectionName.Trim, CompareMode)).Any

                Case True ' An existing Section name is equal to given section name.

                    ' Get the section StartIndex and EndIndex.
                    [Get](SectionName)

                    ' Remove the section range index.
                    Content.RemoveRange(SectionStartIndex, SectionEndIndex - SectionStartIndex)

                    ' Save changes.
                    Return [File].Write(Content, Encoding)

                Case Else ' Any of the existing Section names is equal to given section name.
                    Return False

            End Select

        End Function

    End Class

#End Region

End Class

#End Region

Public Enum ServerState
    Opened
    Closed
End Enum

Public Class ServerManager

    Public Shared Function GetMyIP(Optional ByVal webPath As String = "http://gimmeahit.x10host.com/c/getip.php") As String
        Return New System.Net.WebClient().DownloadString(webPath)
    End Function

End Class