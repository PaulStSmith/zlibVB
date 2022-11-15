'**************************************************
' FILE:         CompressedFile.vb
' AUTHOR:       Paulo Santos
' CREATED:      2007.OCT.05
' COPYRIGHT:    Copyright © 2005-2007 
'               PJ on Development
'               All Rights Reserved.
' DESCRIPTION:
'       Represents an Compressed File Structure,
'       as well its uncompressed data.
'
' MODIFICATION HISTORY:
' 01    2007.OCT.05
'       Paulo Santos
'       Extracted from gzip.vb
'---------------------------------------------------
' GZIP is Copyright © 1992-1993 Jean-loup Gailly
'***************************************************

Namespace GZIP

    ''' <summary>
    ''' Represents an Compressed File Structure, as well its uncompressed data.
    ''' </summary>
    Public Class CompressedFile

        Private __FileName As String
        Private __TimeStamp As Date
        Private __Content As Byte()

#Region " Constructors "

        ''' <summary>
        ''' Initializes an instance of the <see cref="CompressedFile" /> class.
        ''' This is the default constructor for this class.
        ''' </summary>
        <DebuggerStepThrough()> _
        Friend Sub New()
            __FileName = ""
            __TimeStamp = Nothing
            __Content = New Byte() {}
        End Sub

        ''' <summary>
        ''' Initializes an instance of the <see cref="CompressedFile" /> class.
        ''' </summary>
        ''' <param name="FileName">The original file name.</param>
        <DebuggerStepThrough()> _
        Friend Sub New(ByVal FileName As String)
            Me.New()
            __FileName = FileName
        End Sub

        ''' <summary>
        ''' Initializes an instance of the <see cref="CompressedFile" /> class.
        ''' </summary>
        ''' <param name="FileName">The original file name.</param>
        ''' <param name="TimeStamp">The date and time wich the file was created or modified.</param>
        <DebuggerStepThrough()> _
        Friend Sub New(ByVal FileName As String, ByVal TimeStamp As Date)
            Me.New(FileName)
            __TimeStamp = TimeStamp
        End Sub

        ''' <summary>
        ''' Initializes an instance of the <see cref="CompressedFile" /> class.
        ''' </summary>
        ''' <param name="FileName">The original file name.</param>
        ''' <param name="TimeStamp">The date and time wich the file was created or modified.</param>
        ''' <param name="Content">An <see cref="Array"/> of <see cref="Byte"/> with the uncompressed content of the file.</param>
        <DebuggerStepThrough()> _
        Friend Sub New(ByVal FileName As String, ByVal TimeStamp As Date, ByVal Content As Byte())
            Me.New(FileName, TimeStamp)
            __Content = Content
        End Sub

#End Region

#Region " Public Properties "

        ''' <summary>
        ''' The original file name.
        ''' </summary>
        Public ReadOnly Property FileName() As String
            Get
                Return __FileName
            End Get
        End Property

        ''' <summary>
        ''' The date and time wich the file was created or modified.
        ''' </summary>
        Public ReadOnly Property TimeStamp() As Date
            Get
                Return __TimeStamp
            End Get
        End Property

        ''' <summary>
        ''' An <see cref="Array"/> of <see cref="Byte"/> that contains the uncompressed content of the file.
        ''' </summary>
        Public ReadOnly Property Content() As Byte()
            Get
                Return __Content
            End Get
        End Property

#End Region

#Region " Friend Methods "

        ''' <summary>
        ''' Sets the file name property of the <see cref="CompressedFile"/> object.
        ''' </summary>
        ''' <param name="FileName">The original file name.</param>
        Friend Sub SetFileName(ByVal FileName As String)
            __FileName = FileName
        End Sub

        ''' <summary>
        ''' Sets the date and time wich the file was created or modified.
        ''' </summary>
        ''' <param name="TimeStamp">The date and time wich the file was created or modified.</param>
        Friend Sub SetTimeStamp(ByVal TimeStamp As Date)
            __TimeStamp = TimeStamp
        End Sub

        ''' <summary>
        ''' Sets the uncompressed content of the gzipCompressedFile.
        ''' </summary>
        ''' <param name="Content">An <see cref="Array"/> of <see cref="Byte"/> that contains the uncompressed content of the file.</param>
        Friend Sub SetContent(ByVal Content As Byte())
            __Content = Content
        End Sub

#End Region

    End Class

End Namespace
